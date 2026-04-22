using Application.CQRS;
using Application.DTOS;

using HotChocolate;
using HotChocolate.Data;

using MediatR;

namespace API.graphqlAPis;
public class GraphQLApis([Service] IMediator mediator)
{
    public async Task<List<UserStatusDto>> GetOnlineUsers(CancellationToken ct)
     => await mediator.Send(new GetOnlineUsersQuery(), ct);
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<MessageDto>> GetChatHistory(ChatHistoryRequestDto ChatHistoryDTO, CancellationToken ct)
    => await mediator.Send(new GetChatHistoryQuery(ChatHistoryDTO), ct);
}
