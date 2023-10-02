namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

using Microsoft.AspNetCore.Mvc;

public struct Parameters
{
    [FromQuery]
    public string Token { get; set; }
}
