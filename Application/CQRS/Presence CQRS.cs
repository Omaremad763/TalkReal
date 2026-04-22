using Application.Contracts;
using Application.DTOS;

using MediatR;

namespace Application.CQRS;

public record UpdateUserStatusCommand(UpdateUserStatusDto DTO) : IRequest<bool>;
public record GetOnlineUsersQuery() : IRequest<List<UserStatusDto>>;
public class PresenceHandler(ITalkRealServices services) :
    IRequestHandler<UpdateUserStatusCommand, bool>,
    IRequestHandler<GetOnlineUsersQuery, List<UserStatusDto>>
{
    public async Task<bool> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        return await services.UseService.UpdateUserStatus(request.DTO, cancellationToken);
    }
    public async Task<List<UserStatusDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        return await services.UseService.GetUserStatus(cancellationToken);
    }
}