using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools
{
    public sealed class ToolAdminService : IToolAdminService
    {
        private readonly IToolAdminRepository _tools;
        private readonly IToolReadRepository _toolRead;
        private readonly IUnitOfWork _uow;
        private readonly ICategoryRepository _categories;
        private readonly IValidator<ToolCreateRequest> _createValidator;
        private readonly IValidator<ToolUpdateRequest> _updateValidator;

        public ToolAdminService(
            IToolAdminRepository tools,
            IToolReadRepository toolRead,
            IUnitOfWork uow,
            ICategoryRepository categories,
            IValidator<ToolCreateRequest> createValidator,
            IValidator<ToolUpdateRequest> updateValidator)
        {
            _tools = tools;
            _toolRead = toolRead;
            _uow = uow;
            _categories = categories;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        

        public async Task<(bool ok, string? error, int id)> CreateAsync(ToolCreateRequest request, CancellationToken cancellationToken = default)
        {
            var vr = await _createValidator.ValidateAsync(request, cancellationToken);
            if(!vr.IsValid)
            {
                var errors = string.Join("; ", vr.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                return (false, errors, 0);
            }

            var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if(category == null)
            {
                return (false, "Category not found.", 0);
            }

            var tool = new Tool
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                PricePerDay = request.PricePerDay,
                Manufacturer = request.Manufacturer,
                SerialNumber = request.SerialNumber,
                Status = ToolStatus.Available
            };

            await _tools.AddAsync(tool, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return (true, null, tool.Id);
        }
        public async Task<(bool ok, string? error)> UpdateAsync(int toolId, ToolUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var vr = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!vr.IsValid)
            {
                var errors = string.Join("; ", vr.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                return (false, errors);
            }

            var tool = await _tools.GetByIdAsync(toolId, cancellationToken);
            if(tool == null)
            {
                return (false, "Tool not found.");
            }

            var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return (false, "Category not found.");
            }

            tool.Name = request.Name;
            tool.Description = request.Description;
            tool.CategoryId = request.CategoryId;
            tool.PricePerDay = request.PricePerDay;
            tool.Manufacturer = request.Manufacturer;
            tool.SerialNumber = request.SerialNumber;

            if (request.Status.HasValue)
            {
                tool.Status = (ToolStatus)request.Status;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int toolId, CancellationToken cancellationToken = default)
        {
            var tool = await _tools.GetByIdAsync(toolId, cancellationToken);
            if (tool == null)
            {
                return (false, "Tool not found.");
            }

            if (tool.Status == ToolStatus.Rented)
            {
                return (false, "Cannot delete a tool that is currently rented.");
            }

            await _tools.DeleteAsync(tool, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return (true, null);
        }

        public async Task<int> CompleteMaintenanceAsync(CancellationToken cancellationToken = default)
        {
            var inMaintenance = await _tools.GetByStatusAsync(ToolStatus.Maintenance, cancellationToken);
            if(inMaintenance.Count == 0)
            {
               return 0;
            }

            var lastReturns = await _toolRead.GetLastReturnDatesAsync(inMaintenance.Select(t => t.Id), cancellationToken);

            
            foreach(var tool in inMaintenance)
            {
                tool.Status = ToolStatus.Available;

                if(lastReturns.TryGetValue(tool.Id, out var returnDate) && returnDate.HasValue)
                {
                    tool.LastMaintenanceDate = returnDate.Value.Date;
                }
                else
                {
                    tool.LastMaintenanceDate = DateTime.UtcNow.Date;
                }
            }

            await _uow.SaveChangesAsync(cancellationToken);
            return inMaintenance.Count;
        }
    }
}
