using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserService.Domain.Entities;

namespace UserService.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
	public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
	{
		builder.ToTable("RefreshToken");

		builder.HasKey(r => r.Id);

		builder.Property(r => r.Token)
			.IsRequired()
			.HasMaxLength(500);

		builder.Property(r => r.ExpiresAt)
			.IsRequired();

		builder.Property(r => r.CreatedAt)
			.IsRequired();

		builder.Property(r => r.IsRevoked)
			.IsRequired();

		builder.HasOne(r => r.User)
			.WithOne(u => u.RefreshToken)
			.HasForeignKey<RefreshTokenEntity>(r => r.UserId);
	}
}