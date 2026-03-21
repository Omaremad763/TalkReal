using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.CQRS;
using Application.DTOS;

using HotChocolate;

using MediatR;

namespace Infra.Presistence;
public class GraphQL
{
    public async Task<List<UserStatusDto>> GetOnlineUsers(
        [Service] IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new GetOnlineUsersQuery(), ct);
    }
}
