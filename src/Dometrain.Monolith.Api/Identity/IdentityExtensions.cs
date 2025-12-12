namespace Dometrain.Monolith.Api.Identity;

public static class IdentityExtensions
{
    extension(HttpContext context)
    {
        public Guid? GetUserId()
        {
            var userId = context.User.Claims.SingleOrDefault(x => x.Type == "user_id");

            if (Guid.TryParse(userId?.Value, out var parsedId)) return parsedId;

            return null;
        }

        public bool IsAdmin()
        {
            var isAdmin = context.User.Claims.SingleOrDefault(x => x is { Type: "is_admin", Value: "true" });
            return isAdmin is not null;
        }
    }
}