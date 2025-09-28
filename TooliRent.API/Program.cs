
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TooliRent.Application.Authentication;
using Microsoft.EntityFrameworkCore;
using TooliRent.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using TooliRent.Infrastructure.Identity;
using System.Text;
using TooliRent.Infrastructure.Users;
using TooliRent.Application.Users;
using TooliRent.Infrastructure.Authentication;
using TooliRent.Infrastructure.Seed;
using TooliRent.Application.Tools;
using TooliRent.Infrastructure.Tools;
using System.Text.Json.Serialization;
using FluentValidation;
using TooliRent.Application.Bookings.Validation;
using TooliRent.Application.Users.Mapping;
using TooliRent.Application.Bookings;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Bookings;
using TooliRent.Infrastructure.Loans;
using TooliRent.Application.Loans;
using TooliRent.Application.Categories.Validation;
using TooliRent.Infrastructure.Categories;
using TooliRent.Application.Categories;
using TooliRent.Application.Tools.Validation;
using TooliRent.Application.Bookings.Mapping;
using TooliRent.Application.Tools.Mapping;
using TooliRent.Application.Loans.Validation;

namespace TooliRent.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services
                .AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Repositories
            builder.Services.AddScoped<IUserReadRepository, UserReadRepository>();
            builder.Services.AddScoped<IToolReadRepository, ToolReadRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ILoanRepository, LoanRepository>();
            builder.Services.AddScoped<IToolAdminRepository, ToolAdminRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryReadRepository, CategoryRepository>();

            // Services
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IToolAdminService, ToolAdminService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            
            // Validators
            builder.Services.AddValidatorsFromAssemblies(new[]
            {
                typeof(ToolCreateRequestValidator).Assembly,
                typeof(ToolUpdateReqeustValidator).Assembly,
                typeof(CategoryUpdateRequestValidator).Assembly,
                typeof(CategoryCreateRequestValidator).Assembly,
                typeof(BookingCreateRequestValidator).Assembly,
                typeof(PickupCommandValidator).Assembly,
                typeof(ReturnCommandValidator).Assembly
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(BookingMappingProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ToolMappingProfile).Assembly);

            // Seeding
            builder.Services.AddScoped<ToolDataSeeder>();
            builder.Services.AddScoped<IdentitySeeder>();

            builder.Services.AddIdentityCore<AppUser>(o =>
            {
                // Password specs
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
                // User specs
                o.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Token Service
            builder.Services.AddScoped<ITokenService, TokenService>();

            // JWT Authentication
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;

                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = signingKey,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine($"JWT auth failed: {ctx.Exception.GetType().Name} - {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            Console.WriteLine("[JWT] Token validated");
                            return Task.CompletedTask;
                        }

                    };
                });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TooliRent API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Se nu till att få rätt långa super kod.",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
                await seeder.SeedAsync();

                var toolSeeder = scope.ServiceProvider.GetRequiredService<ToolDataSeeder>();
                await toolSeeder.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Use(async (context, next) =>
            {
                var start = DateTime.UtcNow;
                await next();

                var elapsedTime = DateTime.UtcNow - start;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Request to {context.Request.Path} took {Math.Round(elapsedTime.TotalMilliseconds)} ms");
                Console.ResetColor();
            });

            // Rate limiting
            var requestLog = new Dictionary<string, List<DateTime>>();
            int limit = 100;

            app.Use(async (context, next) =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                if(ip != null)
                {
                    if(!requestLog.ContainsKey(ip))
                    {
                        requestLog[ip] = new List<DateTime>();
                    }
                    var now = DateTime.UtcNow;

                    requestLog[ip].RemoveAll(t => (now - t).TotalSeconds > 60);

                    if (requestLog[ip].Count >= limit)
                    {
                        context.Response.StatusCode = 429; // Too Many Requests
                        await context.Response.WriteAsync("Too many requests. Please try again later.");
                        return;
                    }

                    requestLog[ip].Add(now);
                }
                await next();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Use(async (context, next) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Request: {DateTime.Now} : {context.Request.Method} - {context.Request.Path}");
                Console.WriteLine($"From: {context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                    ?? context.Connection.RemoteIpAddress?.ToString() 
                    ?? context.Connection.LocalIpAddress?.ToString() 
                    ?? "unknown"} - {context.User?.Identity?.Name?.ToString() ?? "unknown"}");
                Console.ResetColor();
                await next();
            });


            
            //LETS GO!
            app.Run();
        }
    }
}
