
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
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            // DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            builder.Services.AddScoped<IUserReadRepository, UserReadRepository>();

            // Services
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(TooliRent.Application.Mapping.UserMappingProfile).Assembly);


            // Identity
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
            if (string.IsNullOrWhiteSpace(jwt.Key) || Encoding.UTF8.GetByteCount(jwt.Key) < 32)        
                throw new InvalidOperationException("Jwt:Key is missing or is to short, min 32 chars.");
            if (string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience))
                throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience must exist.");

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
                    Description = "Se nu till att f� r�tt l�nga super kod.",
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
