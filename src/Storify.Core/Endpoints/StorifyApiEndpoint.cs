using Storify.Core.Models;

namespace Storify.Core.Endpoints;

public interface IApiEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}

public abstract class StorifyApiEndpoint : IApiEndpoint
{
    protected readonly ILogger<StorifyApiEndpoint> Logger;
    
    protected readonly IHttpContextAccessor HttpContextAccessor;
    
    protected StorifyApiEndpoint(ILogger<StorifyApiEndpoint> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    }

    public abstract void MapEndpoints(IEndpointRouteBuilder app);

    public StorifyUserClaims GetUserClaims()
    {
        var accessToken = HttpContextAccessor.HttpContext?.Request.Headers
            .FirstOrDefault(x => x.Key.Equals("Authorization"))
            .Value
            .ToString()
            .Split(" ")
            .LastOrDefault();
        if (string.IsNullOrWhiteSpace(accessToken)) throw new NullReferenceException("Access token is null");
        throw new NotImplementedException();
    }
}