using Application.CQRS;
using Application.DTOS;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using TalkReal.Infrastructure.Presence;
namespace Infra.Presistence;
[Authorize]
public class PresenceHub(IMediator mediator) : Hub
{
    private readonly IMediator _mediator = mediator;
    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        UpdateUserStatusDTO dto = new()
        {
            UserId = userId,
            IsOnline =true
        };
        await _mediator.Send(new UpdateUserStatusCommand(dto));
        await Clients.Others.SendAsync("UserStatusChanged", userId, true);
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        await Task.Delay(5000);
        UpdateUserStatusDTO dto = new()
        {
            UserId = userId,
            IsOnline = false
        };
        await _mediator.Send(new UpdateUserStatusCommand(dto));
        await Clients.Others.SendAsync("UserStatusChanged", userId, false);
        await base.OnDisconnectedAsync(exception);
    }
}

//[Authorize]
public class PresenceGrpcService(ApplicationDbContext context) : PresenceProto.PresenceProtoBase
{

    private readonly ApplicationDbContext _context = context;
    public override async Task<UserStatusResponse> GetUserStatus(UserStatusRequest request, ServerCallContext context)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == Guid.Parse(request.UserId));
        return new UserStatusResponse { IsOnline = user?.IsOnline ?? false };

    }
}
