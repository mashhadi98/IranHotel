using AutoMapper;
using VisitorApp.Contract.Features.Identity.Users.Create;
using VisitorApp.Contract.Features.Identity.Users.Update;
using VisitorApp.Contract.Features.Identity.Users.Delete;
using VisitorApp.Contract.Features.Identity.Users.GetById;
using VisitorApp.Contract.Features.Identity.Users.GetPaginated;
using VisitorApp.Contract.Features.Identity.Users.ChangeState;
using VisitorApp.Contract.Features.Identity.Roles.Create;
using VisitorApp.Contract.Features.Identity.Roles.Update;
using VisitorApp.Contract.Features.Identity.Roles.Delete;
using VisitorApp.Contract.Features.Identity.Roles.GetById;
using VisitorApp.Contract.Features.Identity.Roles.GetPaginated;
using VisitorApp.Contract.Features.Identity.Roles.ChangeState;
using VisitorApp.Application.Features.Identity.Users.Create;
using VisitorApp.Application.Features.Identity.Users.Update;
using VisitorApp.Application.Features.Identity.Users.Delete;
using VisitorApp.Application.Features.Identity.Users.GetById;
using VisitorApp.Application.Features.Identity.Users.GetPaginated;
using VisitorApp.Application.Features.Identity.Users.ChangeState;
using VisitorApp.Application.Features.Identity.Roles.Create;
using VisitorApp.Application.Features.Identity.Roles.Update;
using VisitorApp.Application.Features.Identity.Roles.Delete;
using VisitorApp.Application.Features.Identity.Roles.GetById;
using VisitorApp.Application.Features.Identity.Roles.GetPaginated;
using VisitorApp.Application.Features.Identity.Roles.ChangeState;

namespace VisitorApp.API.Features.Identity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings - API to Application
        CreateMap<CreateUserRequest, CreateUserCommandRequest>();
        CreateMap<UpdateUserRequest, UpdateUserCommandRequest>();
        CreateMap<DeleteUserRequest, DeleteUserCommandRequest>();
        CreateMap<GetByIdUserRequest, GetByIdUserQueryRequest>();
        CreateMap<GetPaginatedUserRequest, GetPaginatedUserQueryRequest>();
        CreateMap<ChangeStateUserRequest, ChangeStateUserCommandRequest>();
        
        // Filter mappings
        CreateMap<VisitorApp.Contract.Features.Identity.Users.GetPaginated.GetPaginatedUserFilter, VisitorApp.Application.Features.Identity.Users.GetPaginated.GetPaginatedUserFilter>();

        // Role mappings - API to Application
        CreateMap<CreateRoleRequest, CreateRoleCommandRequest>();
        CreateMap<UpdateRoleRequest, UpdateRoleCommandRequest>();
        CreateMap<DeleteRoleRequest, DeleteRoleCommandRequest>();
        CreateMap<GetByIdRoleRequest, GetByIdRoleQueryRequest>();
        CreateMap<GetPaginatedRoleRequest, GetPaginatedRoleQueryRequest>();
        CreateMap<ChangeStateRoleRequest, ChangeStateRoleCommandRequest>();
        
        // Filter mappings
        CreateMap<VisitorApp.Contract.Features.Identity.Roles.GetPaginated.GetPaginatedRoleFilter, VisitorApp.Application.Features.Identity.Roles.GetPaginated.GetPaginatedRoleFilter>();
    }
} 