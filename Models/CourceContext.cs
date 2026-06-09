using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BDcource.Models;

public partial class CourceContext : DbContext
{
    public CourceContext()
    {
    }

    public CourceContext(DbContextOptions<CourceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blueprint> Blueprints { get; set; }
    public virtual DbSet<Material> Materials { get; set; }
    public virtual DbSet<MaterialIssuance> MaterialIssuances { get; set; }
    public virtual DbSet<Operation> Operations { get; set; }
    public virtual DbSet<OperationMaterialsUsage> OperationMaterialsUsages { get; set; }
    public virtual DbSet<OperationToolsUsage> OperationToolsUsages { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductsOperation> ProductsOperations { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Tool> Tools { get; set; }
    public virtual DbSet<ToolIssuance> ToolIssuances { get; set; }
    public virtual DbSet<ToolType> ToolTypes { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<WorkOrder> WorkOrders { get; set; }
    public virtual DbSet<Workshop> Workshops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=cource;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blueprint>(entity =>
        {
            entity.HasKey(e => e.BlueprintNumber).HasName("PK__Blueprin__D2252EDADDAC9811");
            entity.ToTable("Blueprint");
            entity.Property(e => e.BlueprintNumber).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C5061317E49B179B");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<MaterialIssuance>(entity =>
        {
            entity.HasKey(e => e.IssuanceId).HasName("PK__Material__79C4FB6C29BF6529");
            entity.ToTable("MaterialIssuance", tb => tb.HasTrigger("trg_MaterialIssuance_Insert"));
            entity.Property(e => e.IssuanceId).HasColumnName("IssuanceID");
            entity.Property(e => e.IssueDateTime).HasColumnType("datetime");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkOrderId).HasColumnName("WorkOrderID");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialIssuances)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialIssuance_Materials");

            entity.HasOne(d => d.Operation).WithMany(p => p.MaterialIssuances)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialIssuance_Operations");

            entity.HasOne(d => d.User).WithMany(p => p.MaterialIssuances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialIssuance_Users");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.MaterialIssuances)
                .HasForeignKey(d => d.WorkOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialIssuance_WorkOrder");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.OperationId).HasName("PK__Operatio__A4F5FC64645F96B1");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.BlueprintNumber).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.WorkshopId).HasColumnName("WorkshopID");

            entity.HasOne(d => d.BlueprintNumberNavigation).WithMany(p => p.Operations)
                .HasForeignKey(d => d.BlueprintNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Operations_Blueprint");

            entity.HasOne(d => d.Workshop).WithMany(p => p.Operations)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Operations_Workshops");
        });

        modelBuilder.Entity<OperationMaterialsUsage>(entity =>
        {
            entity.HasKey(e => new { e.OperationId, e.MaterialId });
            entity.ToTable("OperationMaterialsUsage");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

            entity.HasOne(d => d.Material).WithMany(p => p.OperationMaterialsUsages)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationMaterialsUsage_Materials");

            entity.HasOne(d => d.Operation).WithMany(p => p.OperationMaterialsUsages)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationMaterialsUsage_Operations");
        });

        modelBuilder.Entity<OperationToolsUsage>(entity =>
        {
            entity.HasKey(e => new { e.OperationId, e.ToolTypeId });
            entity.ToTable("OperationToolsUsage");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.ToolTypeId).HasColumnName("ToolTypeID");

            entity.HasOne(d => d.Operation).WithMany(p => p.OperationToolsUsages)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationToolsUsage_Operations");

            entity.HasOne(d => d.ToolType).WithMany(p => p.OperationToolsUsages)
                .HasForeignKey(d => d.ToolTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationToolsUsage_ToolTypes");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED3ECE3954");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ProductsOperation>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.OperationId });
            entity.ToTable("ProductsOperations", tb => tb.UseSqlOutputClause(false));
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");

            entity.HasOne(e => e.Product)
                .WithMany(p => p.ProductsOperations)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductsOperations_Products");

            entity.HasOne(e => e.Operation)
                .WithMany(o => o.ProductsOperations)
                .HasForeignKey(e => e.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductsOperations_Operations");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A8D8C2E53");
            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61608AE9209B").IsUnique();
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => e.SerialNumber).HasName("PK__Tools__048A0009B9E6E917");
            entity.ToTable(tb =>
            {
                tb.HasTrigger("trg_Tools_Delete");
                tb.HasTrigger("trg_Tools_Insert");
            });
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.CurrentWorkOrderId).HasColumnName("CurrentWorkOrderID");
            entity.Property(e => e.ToolTypeId).HasColumnName("ToolTypeID");

            entity.HasOne(d => d.ToolType).WithMany(p => p.Tools)
                .HasForeignKey(d => d.ToolTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tools_ToolTypes");

            entity.HasOne(d => d.CurrentWorkOrder).WithMany(p => p.Tools)
                .HasForeignKey(d => d.CurrentWorkOrderId)
                .HasConstraintName("FK_Tools_WorkOrder");
        });

        modelBuilder.Entity<ToolIssuance>(entity =>
        {
            entity.HasKey(e => e.IssuanceId).HasName("PK__ToolIssu__79C4FB6C89D8FDC7");
            entity.ToTable("ToolIssuance", tb =>
            {
                tb.HasTrigger("trg_ToolIssuance_Insert");
                tb.HasTrigger("trg_ToolIssuance_Update");
            });
            entity.Property(e => e.IssuanceId).HasColumnName("IssuanceID");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkOrderId).HasColumnName("WorkOrderID");
            entity.Property(e => e.WorkshopId).HasColumnName("WorkshopID");

            entity.HasOne(d => d.Operation).WithMany(p => p.ToolIssuances)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToolIssuance_Operations");

            entity.HasOne(d => d.SerialNumberNavigation).WithMany(p => p.ToolIssuances)
                .HasForeignKey(d => d.SerialNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToolIssuance_Tools");

            entity.HasOne(d => d.User).WithMany(p => p.ToolIssuances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToolIssuance_Users");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.ToolIssuances)
                .HasForeignKey(d => d.WorkOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToolIssuance_WorkOrder");

            entity.HasOne(d => d.Workshop).WithMany(p => p.ToolIssuances)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToolIssuance_Workshops");
        });

        modelBuilder.Entity<ToolType>(entity =>
        {
            entity.HasKey(e => e.ToolTypeId).HasName("PK__ToolType__56E90A491A7AC893");
            entity.Property(e => e.ToolTypeId).HasColumnName("ToolTypeID");
            entity.Property(e => e.Allocated).HasDefaultValue(0);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC1AD8A504");
            entity.HasIndex(e => e.Login, "UQ__Users__5E55825B1342D7D3").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(128);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.WorkshopName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Role");
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.WorkOrderId).HasName("PK__WorkOrde__AE755175C2F32628");
            entity.ToTable("WorkOrder");
            entity.Property(e => e.WorkOrderId).HasColumnName("WorkOrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.WorkOrders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkOrder_Products");
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("PK__Workshop__7A008C2A604A318A");
            entity.HasIndex(e => e.WorkshopName, "UQ__Workshop__FE55B59587C0F9FC").IsUnique();
            entity.Property(e => e.WorkshopId).HasColumnName("WorkshopID");
            entity.Property(e => e.WorkshopName).HasMaxLength(100);
            entity.Property(e => e.Adress).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}