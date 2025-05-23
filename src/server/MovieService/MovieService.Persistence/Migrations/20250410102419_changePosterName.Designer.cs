﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieService.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    [DbContext(typeof(MovieServiceDBContext))]
    [Migration("20250410102419_changePosterName")]
    partial class changePosterName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MovieService.Domain.Entities.DayEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamptz");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamptz");

                    b.HasKey("Id");

                    b.ToTable("Day", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.GenreEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Genre", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.HallEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SeatsArrayJson")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<short>("TotalSeats")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Hall", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte>("AgeLimit")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<short>("DurationMinutes")
                        .HasColumnType("smallint");

                    b.Property<string>("InRoles")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Poster")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Producer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamptz");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .HasDatabaseName("IX_Movies_Title");

                    b.ToTable("Movie", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieFrameEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("MovieFrame", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieGenreEntity", b =>
                {
                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GenreId")
                        .HasColumnType("uuid");

                    b.HasKey("MovieId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("MovieGenre", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SeatEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Column")
                        .HasColumnType("integer");

                    b.Property<Guid>("HallId")
                        .HasColumnType("uuid");

                    b.Property<int>("Row")
                        .HasColumnType("integer");

                    b.Property<Guid>("SeatTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("HallId");

                    b.HasIndex("SeatTypeId");

                    b.ToTable("Seat", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SeatTypeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("PriceModifier")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.ToTable("SeatType", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("0d0581ed-7e0b-4272-b7f1-00d2b1625800"),
                            Name = "Стандарт",
                            PriceModifier = 1m
                        });
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SessionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DayId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("HallId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("PriceModifier")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamptz");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.HasIndex("HallId");

                    b.HasIndex("MovieId");

                    b.ToTable("Session", (string)null);
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieFrameEntity", b =>
                {
                    b.HasOne("MovieService.Domain.Entities.Movies.MovieEntity", "Movie")
                        .WithMany("MovieFrames")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieGenreEntity", b =>
                {
                    b.HasOne("MovieService.Domain.Entities.GenreEntity", "Genre")
                        .WithMany("MovieGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieService.Domain.Entities.Movies.MovieEntity", "Movie")
                        .WithMany("MovieGenres")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SeatEntity", b =>
                {
                    b.HasOne("MovieService.Domain.Entities.HallEntity", "Hall")
                        .WithMany("Seats")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieService.Domain.Entities.SeatTypeEntity", "SeatType")
                        .WithMany("Seats")
                        .HasForeignKey("SeatTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Hall");

                    b.Navigation("SeatType");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SessionEntity", b =>
                {
                    b.HasOne("MovieService.Domain.Entities.DayEntity", "Day")
                        .WithMany("Sessions")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieService.Domain.Entities.HallEntity", "Hall")
                        .WithMany("Sessions")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieService.Domain.Entities.Movies.MovieEntity", "Movie")
                        .WithMany("Sessions")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");

                    b.Navigation("Hall");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.DayEntity", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.GenreEntity", b =>
                {
                    b.Navigation("MovieGenres");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.HallEntity", b =>
                {
                    b.Navigation("Seats");

                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.Movies.MovieEntity", b =>
                {
                    b.Navigation("MovieFrames");

                    b.Navigation("MovieGenres");

                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("MovieService.Domain.Entities.SeatTypeEntity", b =>
                {
                    b.Navigation("Seats");
                });
#pragma warning restore 612, 618
        }
    }
}
