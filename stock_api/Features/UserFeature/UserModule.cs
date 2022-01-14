using stock_api.Models;

namespace stock_api.Features.UserFeature
{
    public class UserModule : IModule
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/user", async (CustomDbContext db) =>
            {
                return await db.Users.ToListAsync();
            });

            endpoints.MapPost("/user", async (UserDto userDto, CustomDbContext db) =>
            {
                var user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Email = userDto.Email
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return user;
            });

            endpoints.MapDelete("/user/{id}", async (int id, CustomDbContext db) =>
            {
                if (await db.Users.FindAsync(id) is User user)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return Results.Ok(user);
                }

                return Results.NotFound();
            });

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }
    }
}
