using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infra.Presistence;
public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(200);

        builder.HasMany(c => c.Messages)
               .WithOne()
               .HasForeignKey(m => m.ConversationId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.ParticipantAId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.ParticipantBId).IsRequired().HasMaxLength(450);
        builder.HasIndex(x => new { x.ParticipantAId, x.ParticipantBId })
           .IsUnique()
           .HasDatabaseName("IX_Unique_Conversation_Participants");
    }
}
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
        builder.Property(m => m.SenderId).IsRequired();

        builder.Property(m => m.Status)
               .HasConversion<string>();

        builder.OwnsOne(m => m.Attachment, a =>
        {
            a.Property(p => p.Url).HasColumnName("AttachmentUrl").HasMaxLength(500);
            a.Property(p => p.Type).HasColumnName("AttachmentType").HasMaxLength(50);
            a.Property(p => p.Size).HasColumnName("AttachmentSize");
        });
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.LastSeen).IsRequired();
        builder.Property(u => u.IsOnline).HasDefaultValue(false);
    }
}

public class UserConversationConfiguration : IEntityTypeConfiguration<UserConversation>
{
    public void Configure(EntityTypeBuilder<UserConversation> builder)
    {
        builder.HasKey(uc => new { uc.UserId, uc.ConversationId });

        builder.HasOne(uc => uc.User)
               .WithMany()
               .HasForeignKey(uc => uc.UserId);

        builder.HasOne(uc => uc.Conversation)
               .WithMany()
               .HasForeignKey(uc => uc.ConversationId);
    }
}
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);
        builder.Property(x => x.ErrorCount)
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.ProcessedOnUtc, x.ErrorCount })
            .HasDatabaseName("IX_OutboxMessages_Processing_Status");
    }
}