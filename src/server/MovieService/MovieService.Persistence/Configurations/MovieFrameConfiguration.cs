using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities.Movies;

namespace MovieService.Persistence.Configurations;

public class MovieFrameConfiguration : IEntityTypeConfiguration<MovieFrameEntity>
{
	public void Configure(EntityTypeBuilder<MovieFrameEntity> builder)
	{
		builder.ToTable("MovieFrame");

		builder.HasKey(mf => mf.Id);

		builder.Property(mf => mf.Image)
			.IsRequired()
			.HasColumnType("bytea");

		builder.HasOne(mf => mf.Movie)
			.WithMany(m => m.MovieFrames)
			.HasForeignKey(mf => mf.MovieId)
			.OnDelete(DeleteBehavior.Cascade); 
	}
}