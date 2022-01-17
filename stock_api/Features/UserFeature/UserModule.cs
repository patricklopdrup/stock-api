using stock_api.Models;

namespace stock_api.Features.UserFeature
{
    public class UserModule : IModule
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/user", GetUsers);
            endpoints.MapGet("/user/{id}/stock", GetUserStocks);
            endpoints.MapGet("/user/{id}/totalvalue", GetUsersTotalStockValue);
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


        internal async Task<List<Stock>> GetUserStocks(int id, CustomDbContext db)
        {
            return await db.Stocks.Where(stock => stock.UserId == id).ToListAsync();
        }

        internal async Task<double> GetUsersTotalStockValue(int id, CustomDbContext db)
        {
            double total = 0.0;
            var userStocks = await GetUserStocks(id, db);
            foreach (var stock in userStocks.OrderBy(stock => stock.CreatedDate).DistinctBy(stock => stock.Name))
            {
                total += stock.GetTotalValue();
            }
            
            return total;
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
