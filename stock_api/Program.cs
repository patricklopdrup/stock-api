using stock_api.Dal;
using stock_api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CustomDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", () => "Hej");

app.MapGet("/user", async (CustomDbContext db) =>
{
    return await db.Users.ToListAsync();
});

app.MapPost("/user", async (UserDto userDto, CustomDbContext db) =>
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

app.MapDelete("/user/{id}", async (int id, CustomDbContext db) =>
{
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
});

app.Run();



internal record UserDto(string FirstName, string LastName, string UserName, string Email);