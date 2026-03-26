using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.CQRS;

public record UpdateUserStatusCommand(UpdateUserStatusDTO DTO) : IRequest<bool>;
public record GetOnlineUsersQuery() : IRequest<List<UserStatusDto>>;
public class PresenceHandler(ITalkRealServices services) :
    IRequestHandler<UpdateUserStatusCommand,bool>,
    IRequestHandler<GetOnlineUsersQuery, List<UserStatusDto>>
{
    public async Task<bool> Handle(UpdateUserStatusCommand request, CancellationToken ct)
    {
        return await services.UseService.UpdateUserStatus(request.DTO, ct);
    }
    public async Task<List<UserStatusDto>> Handle(GetOnlineUsersQuery request, CancellationToken ct)
    {
        return await services.UseService.GetUserStatus(ct);
    }
}