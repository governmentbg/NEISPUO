using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RegStamps.Data.Entities;

public partial class DataStampsContext : DbContext
{
    public DataStampsContext(DbContextOptions<DataStampsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CodeFileType> CodeFileTypes { get; set; }

    public virtual DbSet<CodeRequestStatus> CodeRequestStatuses { get; set; }

    public virtual DbSet<CodeRequestType> CodeRequestTypes { get; set; }

    public virtual DbSet<CodeStampStatus> CodeStampStatuses { get; set; }

    public virtual DbSet<CodeStampType> CodeStampTypes { get; set; }

    public virtual DbSet<RefRequestStamp> RefRequestStamps { get; set; }

    public virtual DbSet<RequestForStampToApprove> RequestForStampToApproves { get; set; }

    public virtual DbSet<RequestsAllStamp> RequestsAllStamps { get; set; }

    public virtual DbSet<Ruodetail> Ruodetails { get; set; }

    public virtual DbSet<SchoolDetail> SchoolDetails { get; set; }

    public virtual DbSet<TOccupation> TOccupations { get; set; }

    public virtual DbSet<TbErrorLog> TbErrorLogs { get; set; }

    public virtual DbSet<TbKeepPlace> TbKeepPlaces { get; set; }

    public virtual DbSet<TbKeeper> TbKeepers { get; set; }

    public virtual DbSet<TbLog> TbLogs { get; set; }

    public virtual DbSet<TbMoncert> TbMoncerts { get; set; }

    public virtual DbSet<TbRequest> TbRequests { get; set; }

    public virtual DbSet<TbRequestFile> TbRequestFiles { get; set; }

    public virtual DbSet<TbStamp> TbStamps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CodeFileType>(entity =>
        {
            entity.HasKey(e => e.FileTypeId);

            entity.ToTable("codeFileType");

            entity.Property(e => e.FileTypeId).ValueGeneratedNever();
            entity.Property(e => e.FileTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<CodeRequestStatus>(entity =>
        {
            entity.HasKey(e => e.RequestStatusId);

            entity.ToTable("codeRequestStatus");

            entity.Property(e => e.RequestStatusId).ValueGeneratedNever();
            entity.Property(e => e.RequestStatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<CodeRequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId);

            entity.ToTable("codeRequestType");

            entity.Property(e => e.RequestTypeId).ValueGeneratedNever();
            entity.Property(e => e.RequestTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<CodeStampStatus>(entity =>
        {
            entity.HasKey(e => e.StampStatusId);

            entity.ToTable("codeStampStatus");

            entity.Property(e => e.StampStatusId).ValueGeneratedNever();
            entity.Property(e => e.StampStatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<CodeStampType>(entity =>
        {
            entity.HasKey(e => e.StampTypeId);

            entity.ToTable("codeStampType");

            entity.Property(e => e.StampTypeId).ValueGeneratedNever();
            entity.Property(e => e.StampTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<RefRequestStamp>(entity =>
        {
            entity.HasKey(e => new { e.RequestId, e.StampId, e.SchoolId, e.KeeperId, e.KeepPlaceId });

            entity.ToTable("refRequestStamp");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.KeepPlace).WithMany(p => p.RefRequestStamps)
                .HasForeignKey(d => d.KeepPlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refRequestStamp_tbKeepPlace");

            entity.HasOne(d => d.Keeper).WithMany(p => p.RefRequestStamps)
                .HasForeignKey(d => d.KeeperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refRequestStamp_tbKeeper");

            entity.HasOne(d => d.MonCert).WithMany(p => p.RefRequestStamps)
                .HasForeignKey(d => d.MonCertId)
                .HasConstraintName("FK_refRequestStamp_tbMONCerts");

            entity.HasOne(d => d.TbRequest).WithMany(p => p.RefRequestStamps)
                .HasForeignKey(d => new { d.RequestId, d.SchoolId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refRequestStamp_tbRequest");

            entity.HasOne(d => d.TbStamp).WithMany(p => p.RefRequestStamps)
                .HasForeignKey(d => new { d.StampId, d.SchoolId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refRequestStamp_tbStamp");
        });

        modelBuilder.Entity<RequestForStampToApprove>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RequestForStampToApprove");

            entity.Property(e => e.MunicipalityName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Municipality Name");
            entity.Property(e => e.Name)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.OblName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Obl Name");
            entity.Property(e => e.RequestStatusName).HasMaxLength(100);
            entity.Property(e => e.RequestTypeName).HasMaxLength(100);
            entity.Property(e => e.SchlMidName)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.StampStatusName).HasMaxLength(100);
            entity.Property(e => e.StampTypeName).HasMaxLength(100);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestsAllStamp>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RequestsAllStamps");

            entity.Property(e => e.MunicipalityName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Municipality Name");
            entity.Property(e => e.Name)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.OblName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Obl Name");
            entity.Property(e => e.RequestStatusName).HasMaxLength(100);
            entity.Property(e => e.RequestTypeName).HasMaxLength(100);
            entity.Property(e => e.SchlMidName)
                .HasMaxLength(18)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName)
                .HasMaxLength(17)
                .IsUnicode(false);
            entity.Property(e => e.StampStatusName).HasMaxLength(100);
            entity.Property(e => e.StampTypeName).HasMaxLength(100);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Ruodetail>(entity =>
        {
            entity.HasKey(e => e.SchoolId);

            entity.ToTable("RUODetails");

            entity.Property(e => e.SchoolId)
                .ValueGeneratedNever()
                .HasColumnName("SchoolID");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.BudgetFromName)
                .HasMaxLength(100)
                .HasColumnName("BudgetFrom Name");
            entity.Property(e => e.BulstatNo)
                .HasMaxLength(20)
                .HasColumnName("Bulstat No");
            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("E-mail");
            entity.Property(e => e.MunicipalityName)
                .HasMaxLength(50)
                .HasColumnName("Municipality Name");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OblName)
                .HasMaxLength(50)
                .HasColumnName("Obl Name");
            entity.Property(e => e.PostCode)
                .HasMaxLength(10)
                .HasColumnName("Post Code");
            entity.Property(e => e.SchlMidName).HasMaxLength(50);
            entity.Property(e => e.SchoolName).HasMaxLength(200);
            entity.Property(e => e.SchoolType).HasMaxLength(100);
            entity.Property(e => e.SchoolTypeFin).HasMaxLength(100);
            entity.Property(e => e.Telephone1).HasMaxLength(50);
            entity.Property(e => e.TelephoneF).HasMaxLength(50);
        });

        modelBuilder.Entity<SchoolDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("SchoolDetails");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.BudgetFromName)
                .HasMaxLength(100)
                .HasColumnName("BudgetFrom Name");
            entity.Property(e => e.BulstatNo)
                .HasMaxLength(20)
                .HasColumnName("Bulstat No");
            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("E-mail");
            entity.Property(e => e.MunicipalityName)
                .HasMaxLength(50)
                .HasColumnName("Municipality Name");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OblName)
                .HasMaxLength(50)
                .HasColumnName("Obl Name");
            entity.Property(e => e.PostCode).HasColumnName("Post Code");
            entity.Property(e => e.SchlMidName).HasMaxLength(50);
            entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
            entity.Property(e => e.SchoolName).HasMaxLength(200);
            entity.Property(e => e.SchoolType).HasMaxLength(100);
            entity.Property(e => e.SchoolTypeFin).HasMaxLength(100);
            entity.Property(e => e.Telephone1).HasMaxLength(50);
            entity.Property(e => e.TelephoneF).HasMaxLength(50);
        });

        modelBuilder.Entity<TOccupation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("tOccupation");

            entity.Property(e => e.CodeOccMon).HasColumnName("CodeOccMON");
            entity.Property(e => e.NoccupId1).HasColumnName("NOccupID_1");
            entity.Property(e => e.NoccupId2).HasColumnName("NOccupID_2");
            entity.Property(e => e.OccupId).HasColumnName("OccupID");
            entity.Property(e => e.OccupName)
                .HasMaxLength(35)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId);

            entity.ToTable("tbErrorLog");

            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.ControllerName).HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<TbKeepPlace>(entity =>
        {
            entity.HasKey(e => e.PlaceId);

            entity.ToTable("tbKeepPlace");

            entity.Property(e => e.KeepPlaceName).HasMaxLength(500);
        });

        modelBuilder.Entity<TbKeeper>(entity =>
        {
            entity.HasKey(e => e.KeeperId);

            entity.ToTable("tbKeeper");

            entity.Property(e => e.IdNumber).HasMaxLength(50);
            entity.Property(e => e.Idtype).HasColumnName("IDType");
            entity.Property(e => e.Name1).HasMaxLength(200);
            entity.Property(e => e.Name2).HasMaxLength(200);
            entity.Property(e => e.Name3).HasMaxLength(200);
        });

        modelBuilder.Entity<TbLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("tbLog");

            entity.HasOne(d => d.MonCert).WithMany(p => p.TbLogs)
                .HasForeignKey(d => d.MonCertId)
                .HasConstraintName("FK_tbLog_tbMONCerts");
        });

        modelBuilder.Entity<TbMoncert>(entity =>
        {
            entity.ToTable("tbMONCerts");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CertId)
                .HasMaxLength(255)
                .HasColumnName("CertID");
            entity.Property(e => e.PersonName).HasMaxLength(255);
        });

        modelBuilder.Entity<TbRequest>(entity =>
        {
            entity.HasKey(e => new { e.RequestId, e.SchoolId }).HasName("PK_tbRequest_1");

            entity.ToTable("tbRequest");

            entity.Property(e => e.RequestId).ValueGeneratedOnAdd();
            entity.Property(e => e.RequestDate).HasColumnType("datetime");

            entity.HasOne(d => d.RequestStatusNavigation).WithMany(p => p.TbRequests)
                .HasForeignKey(d => d.RequestStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbRequest_codeRequestStatus");

            entity.HasOne(d => d.RequestTypeNavigation).WithMany(p => p.TbRequests)
                .HasForeignKey(d => d.RequestType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbRequest_codeRequestType");
        });

        modelBuilder.Entity<TbRequestFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK_tbRequestFile_1");

            entity.ToTable("tbRequestFile");

            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.FileTypeNavigation).WithMany(p => p.TbRequestFiles)
                .HasForeignKey(d => d.FileType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbRequestFile_codeFileType");

            entity.HasOne(d => d.TbRequest).WithMany(p => p.TbRequestFiles)
                .HasForeignKey(d => new { d.RequestId, d.SchoolId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbRequestFile_tbRequest");
        });

        modelBuilder.Entity<TbStamp>(entity =>
        {
            entity.HasKey(e => new { e.StampId, e.SchoolId }).HasName("PK_tbStamp_1");

            entity.ToTable("tbStamp");

            entity.Property(e => e.FirstUseDate).HasColumnType("datetime");
            entity.Property(e => e.FirstUsePerson).HasMaxLength(200);
            entity.Property(e => e.LetterDate).HasColumnType("datetime");
            entity.Property(e => e.LetterNumber).HasMaxLength(200);

            entity.HasOne(d => d.StampStatusNavigation).WithMany(p => p.TbStamps)
                .HasForeignKey(d => d.StampStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbStamp_codeStampStatus");

            entity.HasOne(d => d.StampTypeNavigation).WithMany(p => p.TbStamps)
                .HasForeignKey(d => d.StampType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbStamp_codeStampType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
