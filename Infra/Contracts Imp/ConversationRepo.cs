
using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class ConversationRepo(ApplicationDbContext context) : IConversationRepo
{
    public async Task<bool> AddConversationAsync(Conversation Conversation)
    {
        await context.Conversations.AddAsync(Conversation);
        return true;
    }

    public async Task<Conversation?> GetBetweenUsersAsync(string userId1, string userId2)
    {
        return await context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.ParticipantAId == userId1 && c.ParticipantBId == userId2) ||
                    (c.ParticipantAId == userId2 && c.ParticipantBId == userId1));
    }
}