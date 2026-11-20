using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Data;

public partial class GimnasioContext : DbContext
{
    public GimnasioContext(DbContextOptions<GimnasioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asistencia> Asistencias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PlanesMembresium> PlanesMembresia { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.HasKey(e => e.IdAsistencia).HasName("PK__Asistenc__3956DEE619BB464F");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Asistencia)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asistenci__IdCli__5AEE82B9");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Clientes__D5946642FF19055F");

            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.IdMembresia).HasName("PK__Membresi__A76E8B165C8CDFF8");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Membresia)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Membresia__IdCli__5441852A");

            entity.HasOne(d => d.IdPlanNavigation).WithMany(p => p.Membresia)
                .HasForeignKey(d => d.IdPlan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Membresia__IdPla__5535A963");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pagos__FC851A3A08D1466D");

            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMembresiaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMembresia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pagos__IdMembres__5812160E");
        });

        modelBuilder.Entity<PlanesMembresium>(entity =>
        {
            entity.HasKey(e => e.IdPlan).HasName("PK__PlanesMe__FB8102AE7F3156FE");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__2A49584CB7E2B462");

            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF97D3CB491A");

            entity.HasIndex(e => e.Usuario1, "UQ__Usuarios__E3237CF7D927F89A").IsUnique();

            entity.Property(e => e.ContrasenaHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__IdRol__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
