namespace Domain.Value_Object;
public record Attachment(
    string Url,
    string Type,
    long Size,
    string PublicId,
    bool IsProcessed = false
);
