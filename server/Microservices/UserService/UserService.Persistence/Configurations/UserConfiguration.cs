using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Persistence.Configurations;

public partial class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.ToTable("User");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(256);

		builder.Property(u => u.Password)
			.IsRequired();

		builder.Property(u => u.Role)
			.HasConversion(
				role => role.ToString(),
				value => Enum.Parse<Role>(value)
			);

		builder.Property(d => d.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.DateOfBirth)
			.IsRequired();

		builder.Property(p => p.DateOfBirth)
			.IsRequired()
			.HasColumnType("date")
			.HasConversion(
				v => v.Date,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.HasOne(u => u.RefreshToken)
			.WithOne(r => r.User)
			.HasForeignKey<RefreshTokenEntity>(r => r.UserId);

		builder.HasData(
			new UserEntity
			{
				Id = Guid.NewGuid(),
				Email = "admin@email.com",
				Password = BCrypt.Net.BCrypt.EnhancedHashPassword("qweQWE123"),
				Role = Role.Admin,
				FirstName = "admin",
				LastName = "admin"
			});
	}
}