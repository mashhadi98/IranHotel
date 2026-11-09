using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisitorApp.Domain.Features.Identity.Entities;
using Shared.Helpers;

namespace VisitorApp.Persistence.Features.Identity.Configurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("UserRefreshTokens");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("user_refresh_token_id")
            .HasColumnType("uniqueidentifier");
            
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
            
        builder.Property(x => x.Token)
            .HasColumnName("token")
            .HasColumnType("nvarchar(500)")
            .IsRequired();
            
        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("datetime2")
            .IsRequired();
            
        builder.Property(x => x.IsRevoked)
            .HasColumnName("is_revoked")
            .HasDefaultValue(false);
            
        builder.Property(x => x.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasColumnType("nvarchar(200)");
            
        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("datetime2");
            
        builder.Property(x => x.ReplacedByToken)
            .HasColumnName("replaced_by_token")
            .HasColumnType("nvarchar(500)");

        // Foreign key relationship with ApplicationUser
        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for performance
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
    }
} 