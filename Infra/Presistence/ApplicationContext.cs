namespace Infra.Presistence;
using System.Reflection;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{

    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<UserConversation> UserConversations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<UserConversation>()
            .HasKey(uc => new { uc.UserId, uc.ConversationId });

        builder.Entity<Message>().OwnsOne(m => m.Attachment, a =>
        {
            a.Property(p => p.Url).HasColumnName("AttachmentUrl").HasMaxLength(500);
            a.Property(p => p.Type).HasColumnName("AttachmentType").HasMaxLength(50);
            a.Property(p => p.Size).HasColumnName("AttachmentSize");
        });

        builder.Entity<Message>()
            .Property(m => m.Status)
            .HasConversion<string>();
    }
}
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true)
          .AddEnvironmentVariables()
          .Build();
        var databaseConfig = config["TalkRealdbConfig"];

        var optionsBuilder = 
        new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(databaseConfig);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
