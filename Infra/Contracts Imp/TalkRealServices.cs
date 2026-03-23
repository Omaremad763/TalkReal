using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;

using AutoMapper;

using Infra.Presistence;

using MediatR;

using Microsoft.AspNetCore.SignalR;

namespace Infra.Contracts_Imp;
public class TalkRealServices(IUnitofWork unitofWork,IMapper mapper,IMediator mediator,
    IHubContext<PresenceHub> hubContext
    ) : ITalkRealServices
{
    public IUserService UseService =>  new UserService(unitofWork, mapper);

    public IMessageService MessageService =>  new MessageService(unitofWork, mediator);

    public INotificationService NotificationService =>  new NotificationService(hubContext);
}
