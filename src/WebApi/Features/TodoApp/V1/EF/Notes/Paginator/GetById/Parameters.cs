namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.GetById;

using Microsoft.AspNetCore.Mvc;

public struct Parameters
{
    [FromQuery]
    public string Token { get; set; }
}
