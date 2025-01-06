using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<GenreEntity>
{
	public void Configure(EntityTypeBuilder<GenreEntity> builder)
	{
		builder.ToTable("Genre");

		builder.HasKey(m => m.Id);
	}
}