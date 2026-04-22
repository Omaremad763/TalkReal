using Application.DTOS;

namespace Application.Contracts.IService;
public interface IMessageService
{
    Task<bool> SendMessageAsync(MessageDto dto);
    IQueryable<MessageDto> GetChatHistoryQuery(ChatHistoryRequestDto criteria);

}
