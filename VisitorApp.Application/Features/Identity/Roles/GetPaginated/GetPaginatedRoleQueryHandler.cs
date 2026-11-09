using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Application.Features.Identity.Common;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.DTOs;
using VisitorApp.Contract.Features.Identity.Roles.GetPaginated;

namespace VisitorApp.Application.Features.Identity.Roles.GetPaginated;

public class GetPaginatedRoleQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IEnumerable<IValidatorService<GetPaginatedRoleQueryRequest, PaginatedResponse<GetPaginatedRoleQueryResponse>>> validators) 
    : RequestHandlerBase<GetPaginatedRoleQueryRequest, PaginatedResponse<GetPaginatedRoleQueryResponse>>(validators)
{
    public override async Task<PaginatedResponse<GetPaginatedRoleQueryResponse>> Handler(GetPaginatedRoleQueryRequest request, CancellationToken cancellationToken)
    {
        var query = roleManager.Roles.AsQueryable();

        var filter = request.Filter;
        if (filter != null)
        {
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(r => 
                    r.Name!.ToLower().Contains(searchTerm) ||
                    r.Description.ToLower().Contains(searchTerm));
            }

            // Apply active filter
            if (filter.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == filter.IsActive.Value);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortDescending 
                    ? query.OrderByDescending(r => r.Name) 
                    : query.OrderBy(r => r.Name),
                "description" => filter.SortDescending 
                    ? query.OrderByDescending(r => r.Description) 
                    : query.OrderBy(r => r.Description),
                "createdat" => filter.SortDescending 
                    ? query.OrderByDescending(r => r.CreatedAt) 
                    : query.OrderBy(r => r.CreatedAt),
                _ => query.OrderBy(r => r.Name)
            };
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pageSize = request.PageSize ?? 10;
        var page = request.Page ?? 1;
        
        var roles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var roleDtos = mapper.Map<List<GetPaginatedRoleQueryResponse>>(roles);

        return new PaginatedResponse<GetPaginatedRoleQueryResponse>(roleDtos, totalCount);
    }
} 