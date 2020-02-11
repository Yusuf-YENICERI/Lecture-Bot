namespace DersBotu
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class botDatabase : DbContext
    {
        public botDatabase()
            : base("name=botDatabase")
        {
        }

        public virtual DbSet<Ana> Anas { get; set; }
        public virtual DbSet<Kategoriler> Kategorilers { get; set; }
        public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }
        public virtual DbSet<KullaniciNotlar> KullaniciNotlars { get; set; }
        public virtual DbSet<Oyun> Oyuns { get; set; }
        public virtual DbSet<Sayfalar> Sayfalars { get; set; }
        public virtual DbSet<Yorumlar> Yorumlars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ana>()
                .HasMany(e => e.Kategorilers)
                .WithOptional(e => e.Ana)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Kategoriler>()
                .HasMany(e => e.Sayfalars)
                .WithOptional(e => e.Kategoriler)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Kullanicilar>()
                .HasMany(e => e.KullaniciNotlars)
                .WithOptional(e => e.Kullanicilar)
                .HasForeignKey(e => e.KullaniciId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Kullanicilar>()
                .HasOptional(e => e.Oyun)
                .WithRequired(e => e.Kullanicilar)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Kullanicilar>()
                .HasMany(e => e.Yorumlars)
                .WithOptional(e => e.Kullanicilar)
                .HasForeignKey(e => e.KullaniciId)
                .WillCascadeOnDelete();
        }
    }
}
