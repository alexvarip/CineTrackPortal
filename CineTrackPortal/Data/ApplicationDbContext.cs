using CineTrackPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineTrackPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActorModel> Actors { get; set; }
        public DbSet<MovieModel> Movies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many relationship between Movies and Actors
            modelBuilder.Entity<MovieModel>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies);


            // Seed Sample Actors
            modelBuilder.Entity<ActorModel>().HasData(
                new ActorModel 
                {
                    ActorId = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                    FirstName = "Tom",
                    LastName = "Hanks" 
                },
                new ActorModel 
                {
                    ActorId = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                    FirstName = "Meryl", 
                    LastName = "Streep" 
                }
            );


            // Seed Sample Movies
            modelBuilder.Entity<MovieModel>().HasData(
                new MovieModel 
                { 
                    MovieId = Guid.Parse("00000000-0000-0000-1111-000000000001"),
                    Title = "Forrest Gump", 
                    Date = new DateTime(1994, 7, 6) },
                new MovieModel 
                { 
                    MovieId = Guid.Parse("00000000-0000-0000-1111-000000000002"), 
                    Title = "The Post",
                    Date = new DateTime(2017, 12, 22) 
                }
            );


            // Seed Movie-Actor relationships (join table)
            modelBuilder.Entity<MovieModel>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies)
                .UsingEntity(j => j.HasData(
                    new {
                        MoviesMovieId = Guid.Parse("00000000-0000-0000-1111-000000000001"),
                        ActorsActorId = Guid.Parse("00000000-0000-0000-0000-000000000001") }, // Forrest Gump - Tom Hanks
                    new {
                        MoviesMovieId = Guid.Parse("00000000-0000-0000-1111-000000000002"), 
                        ActorsActorId = Guid.Parse("00000000-0000-0000-0000-000000000001") }, // The Post - Tom Hanks
                    new { 
                        MoviesMovieId = Guid.Parse("00000000-0000-0000-1111-000000000002"),
                        ActorsActorId = Guid.Parse("00000000-0000-0000-0000-000000000002") }  // The Post - Meryl Streep
                ));



            base.OnModelCreating(modelBuilder);
        }




    }
}
