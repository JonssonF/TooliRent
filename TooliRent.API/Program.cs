
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TooliRent.Application.Authentication;
using Microsoft.EntityFrameworkCore;
using TooliRent.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using TooliRent.Infrastructure.Identity;
using System.Text;
using TooliRent.Domain.Users;
using TooliRent.Infrastructure.Users;
using TooliRent.Application.Users;
using TooliRent.Infrastructure.Authentication;
using TooliRent.Infrastructure.Seed;
using TooliRent.Application.Tools;
using TooliRent.Infrastructure.Tools;
using TooliRent.Domain.Tools;
using System.Text.Json.Serialization;
using FluentValidation;
using TooliRent.Application.Bookings.Validation;
using TooliRent.Application.Users.Mapping;
using TooliRent.Application.Bookings;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Bookings;
using TooliRent.Infrastructure.Loans;
using TooliRent.Application.Loans;

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

            // Services
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ILoanService, LoanService>();

            // Validators
            builder.Services.AddValidatorsFromAssembly(typeof(BookingCreateRequestValidator).Assembly);

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            //LETS GO!
            app.Run();
        }
    }
}
