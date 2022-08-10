using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Konkurs> Konkursy { get; set; } = default!;
        public DbSet<Hodowca> Hodowcy { get; set; } = default!;
        public DbSet<CechaSpiewuCOM> CechySpiewuCOM { get; set; } = default!;
        public DbSet<Kolekcja> Kolekcje { get; set; } = default!;
        public DbSet<Klatka> Klatki { get; set; } = default!;
        public DbSet<Przesluchanie> Przesluchania { get; set; } = default!;
        public DbSet<Wynik> Wyniki { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Konkurs>()
                .HasAlternateKey(k => new { k.Data, k.Miejscowosc, k.Ranga });

            modelBuilder.Entity<Kolekcja>()
                .HasAlternateKey(k => new { k.SygnumWlasciciela, k.ID_Konkursu });

            modelBuilder.Entity<Klatka>()
                .HasAlternateKey(k => new { k.NrKlatki, k.ID_Kolekcji });

            modelBuilder.Entity<Wynik>()
                .HasAlternateKey(w => new { w.ID_Klatki, w.NazwaCechySpiewu });

            // TO-DO: Klucz kandydujący Przesłuchania (?)
        }
    }
}