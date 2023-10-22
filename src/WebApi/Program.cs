namespace WebApi;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;
using Prometheus;
using Prometheus.DotNetRuntime;
using Refit;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Authentication;
using WebApi.Authorization;
using WebApi.Data.Ef;
using WebApi.FeatureHandlers;
using WebApi.IdempotentRequests;
using WebApi.MessageHandlers;
using WebApi.Middleware;
using WebApi.SwApi;

public static class Program
{
    private static readonly string Version = typeof(Program).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? string.Empty;

    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        TaskScheduler.UnobservedTaskException += (_, eventArgs)
            => Log.Error(eventArgs.Exception, "Faulted task has not been observed");

        var collector = DotNetRuntimeStatsBuilder
            .Customize()
            .WithGcStats()
            .WithThreadPoolStats()
            .StartCollecting();

        try
        {
            WebApplication
                .CreateSlimBuilder(args)
                .AddConfigurationSources()
                .ConfigureSerilog()
                .ConfigureServices()
                .Build()
                .ConfigureApplication()
                .Run();

            return 0;
        }
        catch (HostAbortedException e)
        {
            Log.Debug(e, "Host aborted");
            return 0;
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            collector.Dispose();
            Log.CloseAndFlush();
        }
    }

    private static WebApplicationBuilder AddConfigurationSources(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Local.json", true);
        }

        return builder;
    }

    private static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Application", "TODO App");

            if (context.HostingEnvironment.IsDevelopment())
            {
                configuration.WriteTo.Console();
            }
            else
            {
                configuration.WriteTo.Console(new CompactJsonFormatter());
            }
        });

        return builder;
    }

    private static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services
            .AddLogging()
            .AddEndpointsApiExplorer()
            .AddProblemDetails()
            .AddMemoryCache(o =>
            {
                o.TrackStatistics = false;
            })
            .AddValidatorsFromAssemblyContaining(typeof(Program), ServiceLifetime.Singleton)
            .AddSingleton<IDateTime, UtcDateTime>()
            .AddCors(o =>
            {
                o.AddDefaultPolicy(b =>
                {
                    b
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .WithExposedHeaders("x-request-id")
                        .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            })
            .AddPolicyRegistry();

        services
            .Configure<RouteHandlerOptions>(o =>
            {
                o.ThrowOnBadRequest = true;
            })
            .ConfigureHttpJsonOptions(o =>
            {
                o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services
            .AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder(ApiKeyDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                o.AddPolicy(Policies.CanManageApiKeys, b =>
                {
                    b.RequireAuthenticatedUser().RequireRole("Admin");
                });
            })
            .AddAuthentication()
            .AddApiKey();

        services.Scan(scan =>
        {
            scan
                .FromAssembliesOf(typeof(Program))
                .AddClasses(c => c.AssignableTo<IEndpointBuilder>())
                .As<IEndpointBuilder>()
                .WithTransientLifetime();

            scan
                .FromAssembliesOf(typeof(Program))
                .AddClasses(c => c.AssignableTo<FeatureHandler>())
                .AsSelf()
                .WithTransientLifetime();
        });

        builder
            .AddSwagger()
            .AddFeatureManagement()
            .AddDynamoDb()
            .AddIdempotentResults()
            .AddRedis()
            .AddSwApi()
            .AddEf();

        return builder;
    }

    private static WebApplication ConfigureApplication(this WebApplication app)
    {
        app
            .Map("/", () => Results.Redirect("/api-docs"))
            .ShortCircuit()
            .WithGroupName(ApiGroup.Internal);

        app
            .MapGet("health", () => Results.Ok())
            .ShortCircuit()
            .WithGroupName(ApiGroup.Internal);

        app
            .MapGet("api", () => Results.Ok(new { Version }))
            .ShortCircuit()
            .WithGroupName(ApiGroup.Internal);

        foreach (var endpoint in app.Services.GetServices<IEndpointBuilder>())
        {
            endpoint.Setup(app);
        }

        app.UseMiddleware<InstrumentationMiddleware>();
        app.UseMiddleware<TracingMiddleware>();
        app.UseSerilogRequestLogging(o =>
        {
            o.MessageTemplate = "Downstream {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            };
        });
        app.UseRouting();
        app.UseCors();
        app.UseMiddleware<FeatureFlagMiddleware>();
        app.UseStatusCodePages();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseSwagger(o => { o.RouteTemplate = "api-docs/{documentName}/swagger.json"; });
        app.UseSwaggerUI(o =>
        {
            o.DocumentTitle = "TODO App";
            o.RoutePrefix = "api-docs";

            o.EnableFilter();
            o.DefaultModelsExpandDepth(-1);
            o.EnableDeepLinking();

            o.SwaggerEndpoint($"{ApiGroup.Identity.V1}/swagger.json", "Identity V1");
            o.SwaggerEndpoint($"{ApiGroup.TodoApp.V1}/swagger.json", "TODO App V1");
            o.SwaggerEndpoint($"{ApiGroup.TodoApp.V2}/swagger.json", "TODO App V2");
            o.SwaggerEndpoint($"{ApiGroup.TodoApp.V3}/swagger.json", "TODO App V3");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapMetrics();

        return app;
    }

    private static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(t => t.FullName);
            o.CustomOperationIds(m => m.TryGetMethodInfo(out var method) ? method.DeclaringType?.FullName : default);

            o.SwaggerDoc(ApiGroup.Identity.V1, new OpenApiInfo
            {
                Title = "Identity",
                Version = "V1",
            });

            o.SwaggerDoc(ApiGroup.TodoApp.V1, new OpenApiInfo
            {
                Title = "TODO App",
                Version = "V1",
            });
            o.SwaggerDoc(ApiGroup.TodoApp.V2, new OpenApiInfo
            {
                Title = "TODO App",
                Version = "V2",
            });
            o.SwaggerDoc(ApiGroup.TodoApp.V3, new OpenApiInfo
            {
                Title = "TODO App",
                Version = "V2",
            });

            o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey",
                },
            });

            o.OperationFilter<SecurityRequirementFilter>();

            o.EnableAnnotations();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            o.IncludeXmlComments(xmlPath);
        });

        return builder;
    }

    private static WebApplicationBuilder AddDynamoDb(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        if (builder.Environment.IsDevelopment())
        {
            services.AddHostedService<DynamoDbSeeder>();
            services.AddAWSService<IAmazonDynamoDB>(new AWSOptions
            {
                Credentials = new BasicAWSCredentials("development", "development"),
                DefaultClientConfig = { ServiceURL = builder.Configuration.GetConnectionString("DynamoDb") },
            });
        }

        services.AddOptions<DynamoDbConfiguration>()
            .BindConfiguration("DynamoDb")
            .ValidateOnStart();

        services.AddDefaultAWSOptions(new AWSOptions
        {
            Region = RegionEndpoint.EUCentral1,
        });

        services.AddTransient<DynamoDb>();

        return builder;
    }

    private static WebApplicationBuilder AddFeatureManagement(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<FeatureConfiguration>()
            .BindConfiguration("FeatureManagement")
            .ValidateOnStart();

        return builder;
    }

    private static WebApplicationBuilder AddIdempotentResults(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        // services.AddTransient<IIdempotentResults, DynamoDbResults>();
        // services.AddSingleton<IIdempotentResults, RedisResults>();
        services.AddSingleton<IIdempotentResults, InMemoryResults>();

        return builder;
    }

    private static WebApplicationBuilder AddRedis(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        return builder;
    }

    private static WebApplicationBuilder AddSwApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<CircuitBreakerMessageHandler>();
        services.AddTransient<LoggingMessageHandler>();
        services.AddTransient<InstrumentationMessageHandler>();

        // Retry until overall timeout is reached
        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .Or<BrokenCircuitException>()
            .WaitAndRetryForeverAsync(
                (attemptNumber, _) =>
                {
                    // Exponential backoff, first failure is retried immediately
                    return attemptNumber == 1
                        ? TimeSpan.Zero
                        : TimeSpan.FromSeconds(Math.Pow(2, attemptNumber) / 2);
                });

        var perCallTimeout = Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5));

        var overallTimeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(60));

        services
            .AddRefitClient<ISwApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.Timeout = Timeout.InfiniteTimeSpan;
                c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TODOApp", Version));
                c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("SwApi"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    UseCookies = false,
                    AllowAutoRedirect = false,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                    ActivityHeadersPropagator = DistributedContextPropagator.CreateNoOutputPropagator(),
                    AutomaticDecompression = DecompressionMethods.All,
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
            .AddPolicyHandler(overallTimeout)
            .AddPolicyHandler(retry)
            .AddHttpMessageHandler<CircuitBreakerMessageHandler>()
            .AddPolicyHandler(perCallTimeout)
            .AddHttpMessageHandler<LoggingMessageHandler>()
            .AddHttpMessageHandler<InstrumentationMessageHandler>();
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

        return builder;
    }

    private static WebApplicationBuilder AddEf(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddDbContextPool<TodoAppContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("pg"));

                if (builder.Environment.IsDevelopment())
                {
                    o.EnableSensitiveDataLogging();
                }
            })
            .AddPooledDbContextFactory<TodoAppContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("pg"));

                if (builder.Environment.IsDevelopment())
                {
                    o.EnableSensitiveDataLogging();
                }
            });

        return builder;
    }
}
