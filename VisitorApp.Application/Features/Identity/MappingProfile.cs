using AutoMapper;
using VisitorApp.Contract.Features.Identity.Common;
using VisitorApp.Contract.Features.Identity.Users.Create;
using VisitorApp.Contract.Features.Identity.Users.Update;
using VisitorApp.Contract.Features.Identity.Users.GetById;
using VisitorApp.Contract.Features.Identity.Users.GetPaginated;
using VisitorApp.Contract.Features.Identity.Users.ChangeState;
using VisitorApp.Contract.Features.Identity.Roles.Create;
using VisitorApp.Contract.Features.Identity.Roles.Update;
using VisitorApp.Contract.Features.Identity.Roles.GetById;
using VisitorApp.Contract.Features.Identity.Roles.GetPaginated;
using VisitorApp.Contract.Features.Identity.Roles.ChangeState;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Application.Features.Identity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ApplicationUser to UserDto and derived classes
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<ApplicationUser, CreateUserCommandResponse>();
        CreateMap<ApplicationUser, UpdateUserCommandResponse>();
        CreateMap<ApplicationUser, GetByIdUserQueryResponse>();
        CreateMap<ApplicationUser, ChangeStateUserCommandResponse>();
        CreateMap<ApplicationUser, GetPaginatedUserQueryResponse>();

        // ApplicationRole to RoleDto and derived classes
        CreateMap<ApplicationRole, RoleDto>();
        CreateMap<ApplicationRole, CreateRoleCommandResponse>();
        CreateMap<ApplicationRole, UpdateRoleCommandResponse>();
        CreateMap<ApplicationRole, GetByIdRoleQueryResponse>();
        CreateMap<ApplicationRole, ChangeStateRoleCommandResponse>();
        CreateMap<ApplicationRole, GetPaginatedRoleQueryResponse>();
    }
} 