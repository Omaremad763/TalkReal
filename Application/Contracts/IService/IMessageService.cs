using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts.IService;
public interface IMessageService
{
    Task<bool> SendMessageAsync(MessageDto dto);
    IQueryable<MessageDto> GetChatHistoryQuery(ChatHistoryRequestDto criteria);
        
 }
