using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyModels = HMS.Core.Models;
using HMS.Infrastructure.Data;
using HMS.Application.Interfaces;
using HMS.Infrastructure.Repositories;
using HMS.Application.Services;
using Scalar.AspNetCore;
using HMS.Infrastructure.DbInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HMS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<MyModels.ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IManagerService, ManagerService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IGuestService, GuestService>();
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                    ValidAudience = builder.Configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Secret"]!)) 
                };
            });

            var app = builder.Build();

            await DbInitializer.SeedAsync(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}