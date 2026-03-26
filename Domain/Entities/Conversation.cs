using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Conversation
{
    public Guid Id { get; set; }
    public string? Title { get; set; }    
    public DateTime CreatedAt { get; set; }
    public string ParticipantAId { get; set; } = string.Empty;
    public string ParticipantBId { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; }
}

