namespace Dometrain.Monolith.Api.Identity.Api;

public static class IdentityEndpointExtensions
{
    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapPost("/identity/login", IdentityEndpoints.Login)
            .AllowAnonymous();

        return app;
    }
}