using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Categories.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Interfaces;

namespace TooliRent.Application.Categories
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _catRep;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CategoryCreateRequest> _createValidator;
        private readonly IValidator<CategoryUpdateRequest> _updateValidator;

        public CategoryService(
            ICategoryRepository catRep, 
            IUnitOfWork uow, 
            IValidator<CategoryCreateRequest> createValidator, 
            IValidator<CategoryUpdateRequest> updateValidator)
        {
            _catRep = catRep;
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await _catRep.GetAllAsync(cancellationToken);
            return rows.Select(r => new CategoryDto(r.Id, r.Name, r.Description, r.ToolCount)).ToList();
        }

        public async Task<(bool ok, string? error, CategoryDto? data)> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errors, null);
            }

            if ( await _catRep.ExistsByNameAsync(request.Name, null, cancellationToken))
            {
                return (false, "A category with the same name already exists.", null);
            }

            var entity = new ToolCategory 
            { 
                Name = request.Name, 
                Description = request.Description 
            };
            await _catRep.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var rows = await _catRep.GetAllAsync(cancellationToken);
            var row = rows.First(r => r.Id == entity.Id);
            return (true, null, new CategoryDto(row.Id, row.Name, row.Description, row.ToolCount));
        }
        public async Task<(bool ok, string? error, CategoryDto? data)> UpdateAsync(int id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult =  _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errors, null);
            }

            var entity = await _catRep.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return (false, "Category not found.", null);
            }

            if(await _catRep.ExistsByNameAsync(request.Name, id, cancellationToken))
            {
                return (false, "A category with the same name already exists.", null);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;

            await _uow.SaveChangesAsync(cancellationToken);

            var rows = await _catRep.GetAllAsync(cancellationToken);
            var row = rows.First(r => r.Id == entity.Id);
            return (true, null, new CategoryDto(row.Id, row.Name, row.Description, row.ToolCount));
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _catRep.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return (false, "Category not found.");
            }

            await _catRep.RemoveAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
    }
}
