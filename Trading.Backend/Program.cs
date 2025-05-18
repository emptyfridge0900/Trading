
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Trading.Backend.Hubs;
using Trading.Backend.Interfaces;
using Trading.Backend.Persistance;
using Trading.Backend.Services;
using Trading.Common.Models;

namespace Trading.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<TradingDbContext>(options =>
            {
                options.UseInMemoryDatabase("inmemory");
            });

            builder.Services.AddSignalR();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "WhoCreateToken",
                    ValidAudience = "WhoIsThisTokenIntendedFor",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourWeakSecretKeyIsLongEnoughToGenerateToken")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateLifetime = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var headers = context.Request.Headers;

                        // If the request is for trading hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/trading")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            
            builder.Services.AddSingleton<ITickerService,TickerService>();
            builder.Services.AddSingleton<ITradingService, TradingService>();
            builder.Services.AddHostedService<PriceUpdateService>(); 
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Tickers.AddRange(new List<Ticker>{
                        new Ticker { Symbol = "AAPL", Price = 230.45f },
                        new Ticker { Symbol = "MSFT", Price = 420.10f },
                        new Ticker { Symbol = "GOOGL", Price = 175.20f },
                        new Ticker { Symbol = "AMZN", Price = 190.50f },
                        new Ticker { Symbol = "TSLA", Price = 340.75f },
                        new Ticker { Symbol = "NVDA", Price = 135.30f },
                        new Ticker { Symbol = "META", Price = 510.25f },
                        new Ticker { Symbol = "JPM", Price = 205.15f },
                        new Ticker { Symbol = "V", Price = 280.90f },
                        new Ticker { Symbol = "WMT", Price = 75.60f }
                    });
                dbContext.SaveChanges();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<TickerHub>("/ticker");
            app.MapHub<TradingHub>("/trading");
            app.Run();
        }
    }
}
