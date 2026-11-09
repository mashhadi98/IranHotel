using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Application.Features.Identity.Common;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.DTOs;
using VisitorApp.Contract.Features.Identity.Users.GetPaginated;

namespace VisitorApp.Application.Features.Identity.Users.GetPaginated;

public class GetPaginatedUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IEnumerable<IValidatorService<GetPaginatedUserQueryRequest, PaginatedResponse<GetPaginatedUserQueryResponse>>> validators) 
    : RequestHandlerBase<GetPaginatedUserQueryRequest, PaginatedResponse<GetPaginatedUserQueryResponse>>(validators)
{
    public override async Task<PaginatedResponse<GetPaginatedUserQueryResponse>> Handler(GetPaginatedUserQueryRequest request, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsQueryable();

        var filter = request.Filter;
        if (filter != null)
        {
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            // Apply active filter
            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "firstname" => filter.SortDescending 
                    ? query.OrderByDescending(u => u.FirstName) 
                    : query.OrderBy(u => u.FirstName),
                "lastname" => filter.SortDescending 
                    ? query.OrderByDescending(u => u.LastName) 
                    : query.OrderBy(u => u.LastName),
                "email" => filter.SortDescending 
                    ? query.OrderByDescending(u => u.Email) 
                    : query.OrderBy(u => u.Email),
                "createdat" => filter.SortDescending 
                    ? query.OrderByDescending(u => u.CreatedAt) 
                    : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderBy(u => u.FirstName)
            };
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pageSize = request.PageSize ?? 10;
        var page = request.Page ?? 1;
        
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var userDtos = mapper.Map<List<GetPaginatedUserQueryResponse>>(users);

        return new PaginatedResponse<GetPaginatedUserQueryResponse>(userDtos, totalCount);
    }
} 