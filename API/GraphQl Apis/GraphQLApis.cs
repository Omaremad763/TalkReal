using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.CQRS;
using Application.DTOS;

using Domain.Events;

using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

using MediatR;

namespace API.graphqlAPis;
public class GraphQLApis([Service] IMediator mediator)
{
    public async Task<List<UserStatusDto>> GetOnlineUsers(CancellationToken ct)
     =>  await mediator.Send(new GetOnlineUsersQuery(), ct);
    [UseProjection]        
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<MessageDto>> GetChatHistory(ChatHistoryRequestDto ChatHistoryDTO, CancellationToken ct)
    => await mediator.Send(new GetChatHistoryQuery(ChatHistoryDTO), ct);
}
