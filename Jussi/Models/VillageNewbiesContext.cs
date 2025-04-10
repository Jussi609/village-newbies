using System;
using System.Data.Entity;

namespace VillageNewbies.Models
{
    /// <summary>
    /// VillageNewbiesContext on Entity Framework DbContext-luokka, joka toimii 
    /// yhteyspisteenä sovelluksen ja tietokannan välillä
    /// </summary>
    public class VillageNewbiesContext : DbContext
    {
        /// <summary>
        /// Konstruktori
        /// </summary>
        public VillageNewbiesContext() : base("name=VillageNewbiesConnection")
        {
            // Otetaan käyttöön lazy loading
            this.Configuration.LazyLoadingEnabled = true;
            // Estetään muutosten seuraaminen oletuksena
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// Alueet-tietokantataulun DbSet
        /// </summary>
        public DbSet<Alue> Alueet { get; set; }

        /// <summary>
        /// Asiakkaat-tietokantataulun DbSet
        /// </summary>
        public DbSet<Asiakas> Asiakkaat { get; set; }

        /// <summary>
        /// Mokit-tietokantataulun DbSet
        /// </summary>
        public DbSet<Mokki> Mokit { get; set; }

        /// <summary>
        /// Palvelut-tietokantataulun DbSet
        /// </summary>
        public DbSet<Palvelu> Palvelut { get; set; }

        /// <summary>
        /// Varaukset-tietokantataulun DbSet
        /// </summary>
        public DbSet<Varaus> Varaukset { get; set; }

        /// <summary>
        /// VaraustenPalvelut-tietokantataulun DbSet
        /// </summary>
        public DbSet<VarauksenPalvelu> VaraustenPalvelut { get; set; }

        /// <summary>
        /// Konfiguroi tietokantataulujen suhteet ja ominaisuudet
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Määritellään Asiakas-Varaus -suhde (1-n)
            modelBuilder.Entity<Asiakas>()
                .HasMany(a => (System.Collections.Generic.ICollection<Varaus>)a.Varaukset)
                .WithRequired(v => v.Asiakas)
                .HasForeignKey(v => v.AsiakasID)
                .WillCascadeOnDelete(false);

            // Määritellään Mokki-Varaus -suhde (1-n)
            modelBuilder.Entity<Mokki>()
                .HasMany(m => (System.Collections.Generic.ICollection<Varaus>)m.Varaukset)
                .WithRequired(v => v.Mokki)
                .HasForeignKey(v => v.MokkiID)
                .WillCascadeOnDelete(false);

            // Määritellään Alue-Mokki -suhde (1-n)
            modelBuilder.Entity<Alue>()
                .HasMany(a => a.Mokit)
                .WithRequired(m => m.Alue)
                .HasForeignKey(m => m.AlueID)
                .WillCascadeOnDelete(false);

            // Määritellään Alue-Palvelu -suhde (1-n)
            modelBuilder.Entity<Alue>()
                .HasMany(a => a.Palvelut)
                .WithRequired(p => p.Alue)
                .HasForeignKey(p => p.AlueID)
                .WillCascadeOnDelete(false);

            // Määritellään Varaus-VarauksenPalvelu -suhde (1-n)
            modelBuilder.Entity<Varaus>()
                .HasMany(v => v.VarauksenPalvelut)
                .WithRequired(vp => vp.Varaus)
                .HasForeignKey(vp => vp.VarausID)
                .WillCascadeOnDelete(false);

            // Määritellään Palvelu-VarauksenPalvelu -suhde (1-n)
            modelBuilder.Entity<Palvelu>()
                .HasMany(p => (System.Collections.Generic.ICollection<VarauksenPalvelu>)p.VarauksenPalvelut)
                .WithRequired(vp => vp.Palvelu)
                .HasForeignKey(vp => vp.PalveluID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
} 