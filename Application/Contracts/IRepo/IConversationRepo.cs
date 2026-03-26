using Domain.Entities;

namespace Application.Contracts.IRepo;

public interface IConversationRepo
{
    Task<bool> AddConversationAsync(Conversation Conversation);
    Task<Conversation?> GetBetweenUsersAsync(string userId1, string userId2);
}