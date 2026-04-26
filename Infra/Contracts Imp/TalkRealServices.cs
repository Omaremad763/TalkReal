using Application.Contracts;
using Application.Contracts.IService;

using Infra.Presistence;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace Infra.Contracts_Imp;
public class TalkRealServices(IUnitofWork unitofWork,
    IHubContext<PresenceHub> hubContext,
    ICloudinaryService CloudinaryService,
    IConfiguration config
    ) : ITalkRealServices
{
    public IUserService UseService => new UserService(unitofWork);

    public IMessageService MessageService => new MessageService(unitofWork, CloudinaryService);

    public INotificationService NotificationService => new NotificationService(hubContext);
    public IAttachmentService AttachmentService => new AttachmentService(unitofWork, hubContext);

    public ITokenService TokenService =>  new TokenService(unitofWork,config);
}
