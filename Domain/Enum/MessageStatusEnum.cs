using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum;
public enum MessageStatusEnum
{
    Pending = 0,              
    Sent = 1,                 

    Delivered = 2,            
    Seen = 3,              
    Failed =4
}
