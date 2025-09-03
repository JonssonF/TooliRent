
using Microsoft.EntityFrameworkCore;
using TooliRent.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using TooliRent.Infrastructure.Identity;
using InfraAppUser = TooliRent.Infrastructure.Identity.AppUser;

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

            // Identity
            builder.Services.AddScoped<IdentitySeeder>();

            builder.Services.AddIdentityCore<InfraAppUser>(o =>
            {
                // Password specs
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
                // User specs
                o.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            //Repositories

            //Services

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {

                var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
                await seeder.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
