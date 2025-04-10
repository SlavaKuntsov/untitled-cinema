using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieService.Domain.Entities.Movies;

namespace MovieService.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<MovieEntity>
{
	public void Configure(EntityTypeBuilder<MovieEntity> builder)
	{
		builder.ToTable("Movie");

		builder.HasKey(m => m.Id);

		builder.Property(m => m.Title)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(m => m.Poster)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(m => m.ReleaseDate)
			.HasColumnType("timestamptz")
			.HasConversion(
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(m => m.CreatedAt)
			.IsRequired()
			.HasColumnType("timestamptz")
			.HasConversion(
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(m => m.UpdatedAt)
			.IsRequired()
			.HasColumnType("timestamptz")
			.HasConversion(
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.HasMany(m => m.Sessions)
			.WithOne(s => s.Movie)
			.HasForeignKey(s => s.MovieId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.HasMany(m => m.MovieFrames)
			.WithOne(f => f.Movie)
			.HasForeignKey(f => f.MovieId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(m => m.Title)
			.HasDatabaseName("IX_Movies_Title");
	}
}