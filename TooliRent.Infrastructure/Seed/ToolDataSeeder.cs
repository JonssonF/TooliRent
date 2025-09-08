using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Seed
{
    public sealed class ToolDataSeeder
    {
        private readonly AppDbContext _context;

        public ToolDataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync(CancellationToken cancellationToken)
        {
            
            if (await _context.Tools.AnyAsync(cancellationToken))
            {
                return;
            }

            var catPower = new ToolCategory { Name = "Power Tools", Description = "Electric & cordless" };
            var catHand = new ToolCategory { Name = "Hand Tools", Description = "Manual tools" };
            _context.Categories.AddRange(catPower, catHand);
            await _context.SaveChangesAsync(cancellationToken);

            _context.Tools.AddRange(
                new Tool
                {
                    Name = "Cordless Drill",
                    Description = "18V cordless drill with two batteries",
                    Manufacturer = "Makita",
                    CategoryId = catPower.Id,
                    PricePerDay = 15.00m,
                    Status = Domain.Enums.ToolStatus.Available
                },
                new Tool
                {
                    Name = "Circular Saw",
                    Description = "7-1/4 inch circular saw for wood cutting",
                    Manufacturer = "DeWalt",
                    CategoryId = catPower.Id,
                    PricePerDay = 20.00m,
                    Status = Domain.Enums.ToolStatus.Available
                },
                new Tool
                {
                    Name = "Hammer",
                    Description = "16 oz claw hammer",
                    Manufacturer = "Bacho",
                    CategoryId = catHand.Id,
                    PricePerDay = 5.00m,
                    Status = Domain.Enums.ToolStatus.Available
                },
                new Tool
                {
                    Name = "Screwdriver Set",
                    Description = "Set of 6 screwdrivers with various heads",
                    Manufacturer = "Stanley",
                    CategoryId = catHand.Id,
                    PricePerDay = 7.50m,
                    Status = Domain.Enums.ToolStatus.Available
                }
            );
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
