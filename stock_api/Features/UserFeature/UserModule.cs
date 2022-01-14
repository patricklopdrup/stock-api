using stock_api.Models;

namespace stock_api.Features.UserFeature
{
    public class UserModule : IModule
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/user", GetUsers);
            endpoints.MapPost("/user", AddUser);
            endpoints.MapDelete("/user/{id}", DeleteUserById);

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }


        #region CRUD methods
        internal async Task<List<User>> GetUsers(CustomDbContext db)
        {
            return await db.Users.ToListAsync();
        }

        internal async Task<IResult> AddUser(UserDto userDto, CustomDbContext db)
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

            return Results.Ok(user);
        }

        internal async Task<IResult> DeleteUserById(int id, CustomDbContext db)
        {
            if (await db.Users.FindAsync(id) is User user)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return Results.Ok(user);
            }

            return Results.NotFound();
        }


        #endregion
    }
}
