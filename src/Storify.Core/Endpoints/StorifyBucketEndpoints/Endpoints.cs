namespace Storify.Core.Endpoints.BucketEndpoints;

public class Endpoints
    (ILogger<Endpoints> logger, IServiceProvider serviceProvider)
    : StorifyApiEndpoint(logger, serviceProvider)
{
    public override void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        throw new NotImplementedException();
    }
}