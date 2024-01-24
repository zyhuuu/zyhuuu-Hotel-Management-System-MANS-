using MANS._55DR.HotelManagement.WebApi.Consts;
using MANS._55DR.HotelManagement.WebApi.Database;
using MANS._55DR.HotelManagement.WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

//ASP .net core uses Kester by default
//https://learn.microsoft.com/pl-pl/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-6.0#get-started
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<HotelDbContext>(options => options.UseSqlite(configuration.GetConnectionString("HotelDbContext")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HotelDbContext>();

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => // Adding Jwt Bearer
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = (context) =>
            {
                context.Token = context.Request.Cookies["Authorization"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
    if (!await roleManager.RoleExistsAsync(UserRoles.User))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

    var user = await userManager.FindByNameAsync(configuration["AdminUser:Name"]);
    if (user == null)
    {
        user = new User
        {
            Email = configuration["AdminUser:Email"],
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = configuration["AdminUser:Name"],
        };
        var result = await userManager.CreateAsync(user, configuration["AdminUser:Password"]);
        await userManager.AddToRoleAsync(user, UserRoles.Admin);
        await userManager.AddToRoleAsync(user, UserRoles.User);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.Run();
