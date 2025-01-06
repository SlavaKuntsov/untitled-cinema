using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class HallConfiguration : IEntityTypeConfiguration<HallEntity>
{
	public void Configure(EntityTypeBuilder<HallEntity> builder)
	{
		builder.ToTable("Hall");

		builder.HasKey(h => h.Id);

		builder.Property(h => h.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(h => h.TotalSeats)
			.IsRequired();

		builder.HasMany(h => h.Seats)
			.WithOne(s => s.Hall)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(h => h.Sessions)
			.WithOne(s => s.Hall)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}