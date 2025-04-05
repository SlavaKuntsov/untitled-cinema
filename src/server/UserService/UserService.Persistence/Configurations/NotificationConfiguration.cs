using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
	public void Configure(EntityTypeBuilder<NotificationEntity> builder)
	{
		builder.ToTable("Notification");

		builder.HasKey(n => n.Id);

		builder.Property(n => n.Message)
			.IsRequired()
			.HasMaxLength(500);

		builder.Property(n => n.CreatedAt)
			.IsRequired();

		builder.Property(r => r.IsDeleted)
			.IsRequired();

		builder.HasOne(n => n.User)
			.WithMany(u => u.Notifications)
			.HasForeignKey(n => n.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}