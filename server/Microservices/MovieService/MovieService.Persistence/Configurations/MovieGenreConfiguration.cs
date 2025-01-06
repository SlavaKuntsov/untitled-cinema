using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenreEntity>
{
	public void Configure(EntityTypeBuilder<MovieGenreEntity> builder)
	{
		builder.ToTable("MovieGenre");

		builder
			.HasKey(mg => new { mg.MovieId, mg.GenreId }); 

		builder
			.HasOne(mg => mg.Movie)
			.WithMany(m => m.MovieGenres)
			.HasForeignKey(mg => mg.MovieId);

		builder
			.HasOne(mg => mg.Genre)
			.WithMany(g => g.MovieGenres)
			.HasForeignKey(mg => mg.GenreId);
	}
}