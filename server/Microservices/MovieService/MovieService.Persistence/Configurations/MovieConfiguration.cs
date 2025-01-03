using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain;

namespace MovieService.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<MovieModel>
{
	public void Configure(EntityTypeBuilder<MovieModel> builder)
	{
		builder.ToTable("Movie");

		builder.HasKey(m => m.Id);

		builder.Property(m => m.Title)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(m => m.ReleaseDate)
			.HasColumnType("date")
			.HasConversion(
				v => v.Date,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(m => m.CreatedAt)
			.IsRequired()
			.HasDefaultValueSql("CURRENT_TIMESTAMP")
			.HasColumnType("timestamp")
			.HasConversion(
				v => v,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(m => m.UpdatedAt)
			.IsRequired()
			.HasDefaultValueSql("CURRENT_TIMESTAMP")
			.HasColumnType("timestamp")
			.HasConversion(
				v => v,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(m => m.Genres)
			   .HasConversion(
				   genres => string.Join(',', genres),
				   genresString => genresString.Split(',', StringSplitOptions.RemoveEmptyEntries)
			   );

		builder.HasMany(m => m.Sessions)
			   .WithOne(s => s.Movie)
			   .HasForeignKey(s => s.MovieId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}