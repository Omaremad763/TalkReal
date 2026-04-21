using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Object;
public record Attachment(
    string Url,
    string Type,    
    long Size,
    string PublicId,         
    bool IsProcessed = false
);
