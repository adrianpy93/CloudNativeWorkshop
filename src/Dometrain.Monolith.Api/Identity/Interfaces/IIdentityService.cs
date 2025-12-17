namespace Dometrain.Monolith.Api.Identity.Interfaces;

public interface IIdentityService
{
    string GenerateToken(Guid userId, string email);
}