using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PostgreSql.EFCore.DBModel
{
    public partial class pgtestdbContext : DbContext
    {
        public pgtestdbContext()
        {
        }

        public pgtestdbContext(DbContextOptions<pgtestdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pgsql> Pgsqls { get; set; }
        public virtual DbSet<Userinfo> Userinfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("PORT=5432;DATABASE=pgtestdb;HOST=127.0.0.1;PASSWORD=123456;USER ID=postgres;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack");

            modelBuilder.Entity<Pgsql>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pgsql");

                entity.Property(e => e.Addtime).HasColumnName("addtime");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Cent)
                    .HasPrecision(18, 2)
                    .HasColumnName("cent");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Money)
                    .HasColumnType("money")
                    .HasColumnName("money");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Userinfo>(entity =>
            {
                entity.ToTable("userinfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Addtime).HasColumnName("addtime");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Cent)
                    .HasPrecision(18, 2)
                    .HasColumnName("cent");

                entity.Property(e => e.Money)
                    .HasColumnType("money")
                    .HasColumnName("money");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
