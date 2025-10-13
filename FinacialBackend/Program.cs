using DotNetEnv;
using FinacialBackend.Data;
using FinacialBackend.Interfaces;
using FinacialBackend.Model;
using FinacialBackend.Repository;
using FinacialBackend.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace FinacialBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ?? Log connection strings (debugging)
            foreach (var kv in builder.Configuration.GetSection("ConnectionStrings").GetChildren())
            {
                Console.WriteLine($"==> Found connection string key={kv.Key}, value={kv.Value}");
            }

            // ========================
            // Services Configuration
            // ========================

            // DbContext original
            //        builder.Services.AddDbContext<ApplicationDBContext>(options =>
            //        {
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            //});

            // Load environment variables from .env
            DotNetEnv.Env.Load();
            Console.WriteLine($"Loaded connection string: {Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")}");


            // Get the connection string from environment variable
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            // Register DbContext using the env variable directly
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseNpgsql(connectionString));

            //DbContext Testing with Render sql lite
            // builder.Services.AddDbContext<ApplicationDBContext>(options =>
            //options.UseSqlite("DefaultConnection"));


            // Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>();

            // Authentication & JWT
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) if you want it to get from cookies 

            var jwtKey = builder.Configuration["JWT:SigningKey"] ?? Environment.GetEnvironmentVariable("JWT__SigningKey");
            var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? Environment.GetEnvironmentVariable("JWT__Issuer");
            var jwtAudience = builder.Configuration["JWT:Audience"] ?? Environment.GetEnvironmentVariable("JWT__Audience");

            if (string.IsNullOrEmpty(jwtKey))
            {
                Console.WriteLine("JWT SigningKey is missing! Check environment variables on Render.");
            }
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
            };
        });

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = builder.Configuration["JWT:Issuer"],
            //            ValidateAudience = true,
            //            ValidAudience = builder.Configuration["JWT:Audience"],
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
            //            )
            //        };
            //    });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessdenied";

                options.Events.OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = 401; // Instead of redirect
                    return Task.CompletedTask;
                };
            });


            // Repositories & Services
            builder.Services.AddScoped<IStockRepository, StockRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            builder.Services.AddHttpClient<IFMPServices, FMPService>();

            // Controllers
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Financial API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            //  CORS: Allow all origins for testing
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:5173",
                        "https://finacialfrontend.onrender.com" // Replace with your actual frontend render URL
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });


            var app = builder.Build();

            // automatically add migration in render after update
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                db.Database.Migrate();
            }


            // ========================
            // Database Connection Check
            // ========================
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                try
                {
                    if (dbContext.Database.CanConnect())
                        Console.WriteLine("? Database connection successful!");
                    else
                        Console.WriteLine("? Database connection failed!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Error while connecting to database: {ex.Message}");
                }
            }

            // ========================
            // Middleware Pipeline
            // ========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            //app.UseCors(x => x
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .AllowCredentials()
            //    .SetIsOriginAllowed(origin => true));

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
