using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities.Movies;

namespace MovieService.Persistence.Configurations;

public class MovieFrameConfiguration : IEntityTypeConfiguration<MovieFrameEntity>
{
	public void Configure(EntityTypeBuilder<MovieFrameEntity> builder)
	{
		builder.ToTable("MovieFrame");

		builder.HasKey(f => f.Id); 
        
		builder.Property(f => f.FrameName)
			.IsRequired()
			.HasMaxLength(255);
              
		builder.Property(f => f.Order)
			.IsRequired();
	}
}