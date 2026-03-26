using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

using Domain.Entities;

namespace Application.Contracts.IRepo;
public interface IMessageRepo
{
    Task<bool> AddMessageAsync(Message message);
    IQueryable<Message> GetChatHistory(ChatHistoryRequestDto criteria);
}
