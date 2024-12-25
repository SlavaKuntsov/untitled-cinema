using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserService.Persistence.Entities;

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
			.IsRequired();

		builder.Property(d => d.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.DateOfBirth)
			.IsRequired();

		//builder.HasOne(u => u.Data)
		//	.WithOne(d => d.User)
		//	.HasForeignKey<UserDataEntity>(d => d.Id);

		builder.HasOne(u => u.RefreshToken)
			.WithOne(r => r.User)
			.HasForeignKey<RefreshTokenEntity>(r => r.UserId);
	}
}