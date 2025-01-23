using System.Net;
using Storify.Core.Extensions;

namespace Storify.Core.Models;

public class StorifyResult
{
    public DateTimeOffset Timestamp => DateTimeExtensions.Now;
    
    public HttpStatusCode Code { get; set; }
    
    public bool Success => (int)Code >= 200 && (int)Code <= 299;
    
    public string? Message { get; set; }
    
    public string[]? Errors { get; set; }

    public static IResult Ok(string? message = null)
    {
        return Results.Ok(new StorifyResult
        {
            Code = HttpStatusCode.OK,
            Message = message,
        });
    }
    
    public static IResult BadRequest(params string[]? errors)
    {
        return Results.BadRequest(new StorifyResult
        {
            Code = HttpStatusCode.BadRequest,
            Errors = errors
        });
    }
}

public class StorifyResult<T> : StorifyResult
{
    public T Data { get; set; } = default!;
    
    public static IResult Ok(T data, string? message = null)
    {
        return Results.Ok(new StorifyResult<T>
        {
            Code = HttpStatusCode.OK,
            Data = data,
            Message = message,
        });
    }
}