using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TooliRent.Application.Users.DTOs;
using TooliRent.Domain.Common;
using TooliRent.Domain.Interfaces;

namespace TooliRent.Application.Users
{
    public sealed class UserService : IUserService
    {
        private readonly IUserReadRepository _repo;
        private readonly IMapper _mapper;

        public UserService(IUserReadRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // Get a page of users with optional search and role filtering
        public async Task<PagedResult<UserDto>> GetUsersPageAsync(int page, int pageSize, string? search, string? role, CancellationToken cancellationToken)
        {
            // Gets the rows and total count from the repository
            var (rows, total) = await _repo.GetUsersPageAsync(page, pageSize, search, role, cancellationToken);

            // Maps the rows to DTOs
            var dtos = _mapper.Map<List<UserDto>>(rows);

            // Fetches roles for the users and assigns them to the DTOs
            var rolesMap = await _repo.GetRolesForUsersAsync(rows.Select(r => r.Id), cancellationToken);
            for (int i = 0; i < dtos.Count; i++)
            {
                var d = dtos[i];
                if (rolesMap.TryGetValue(d.Id, out var rs))
                    dtos[i] = d with { Roles = rs };
            }

            return new PagedResult<UserDto>(dtos, total, page, pageSize);
        }

        // Get all users (up to 'max') sorted by their highest priority role and then by name/email
        public async Task<IReadOnlyList<UserDto>> GetAllUsersSortedByRoleAsync(int max, CancellationToken cancellationToken)
        {
            if (max < 1) max = 1;

            var (rows, _) = await _repo.GetUsersPageAsync(page: 1, pageSize: max, search: null, role: null, cancellationToken);

            var dtos = _mapper.Map<List<UserDto>>(rows);

            var rolesMap = await _repo.GetRolesForUsersAsync(rows.Select(r => r.Id), cancellationToken);
            for (int i = 0; i < dtos.Count; i++)
            {
                var d = dtos[i];
                if (rolesMap.TryGetValue(d.Id, out var rs))
                    dtos[i] = d with { Roles = rs };
            }
            static int Rank(IReadOnlyList<string> roles)
                => roles.Contains("Admin") ? 0 : roles.Contains("Member") ? 1 : 2;

            return dtos
                .OrderBy(d => Rank(d.Roles))
                .ThenBy(d => d.FullName ?? d.Email)
                .ToList();
        }
    }
}
