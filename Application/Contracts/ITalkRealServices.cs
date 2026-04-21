using Application.Contracts.IService;

namespace Application.Contracts;

    public interface ITalkRealServices
    {
         IUserService UseService {  get; }
         IMessageService MessageService {  get; }
         INotificationService NotificationService { get; }
         IAttachmentService AttachmentService { get; }
    }

  
