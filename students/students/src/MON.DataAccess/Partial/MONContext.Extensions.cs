using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using MON.DataAccess.Dto;
using MON.Models;
using MON.Models.Absence;
using MON.Models.ASP;
using MON.Models.Dashboards;
using MON.Models.HealthInsurance;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Class;
using MON.Shared;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.DataAccess
{
    public partial class MONContext
    {
        private readonly IUserInfo _userInfo;

        private readonly HashSet<EntityState> TrackedStates = new HashSet<EntityState> { EntityState.Added, EntityState.Modified, EntityState.Deleted };

        public DbSet<NomenclatureType> NomenclatureTypes { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<StudentResourceSopDetails> StudentResourceSopDetails { get; set; }
        public DbSet<JsonStrResult> JsonStrResults { get; set; }
        public DbSet<Subject> SubjectsForInstitution { get; set; }
        public DbSet<AbsenceExportModel> ExportAbsences { get; set; }
        public DbSet<StudentForAdmissionModel> InstitutionStudentsForEnrollment { get; set; }
        public DbSet<ASPMonthlyBenefitReportDetailsDto> ASPMonthlyBenefitReportDetails { get; set; }

        public DbSet<StudentGradeEvaluationDto> StudentGradeEvaluations { get; set; }

        public IUserInfo UserInfo => this._userInfo;


        static MONContext()
        {
            ConfigureAuditManager();
        }

        public MONContext(DbContextOptions<MONContext> options,
            IUserInfo userInfo,
            IOptions<ChangeTrackerLogConfig> changeTrackerLogConfig)
            : base(options)
        {
            _userInfo = userInfo;
        }

        partial void OnModelCreatingPartial(ModelBuilder builder)
        {
            builder.Entity<NomenclatureType>().HasNoKey();
            builder.Entity<JsonStrResult>().HasNoKey();
            builder.Entity<SchoolYear>()
                .HasKey(x => x.CurrentYearID);
            builder.Entity<StudentSearchDto>().HasNoKey().ToView(null);
            builder.Entity<StudentResourceSopDetails>().HasNoKey().ToView(null);
            builder.Entity<DbTableInfoDto>().HasNoKey().ToView(null);
            builder.Entity<StudentProfessionDto>().HasNoKey().ToView(null);
            builder.Entity<SubjectWithCurrentGradesForTermDto>().HasNoKey().ToView(null);
            builder.Entity<AbsenceExportModel>().HasNoKey().ToView(null);
            builder.Entity<StudentForAdmissionModel>().HasNoKey().ToView(null);
            builder.Entity<ASPMonthlyBenefitReportDetailsDto>().HasNoKey().ToView(null);
            builder.Entity<StudentClassAbsencesDto>().HasNoKey().ToView(null);
            builder.Entity<StudentByInstitutionOccupationIntervalDto>().HasNoKey().ToView(null);
            builder.Entity<FunctionStringResult>().HasNoKey().ToView(null);
            builder.Entity<DataReferencesResult>().HasNoKey().ToView(null);
            builder.Entity<HealthInsuranceDto>().HasNoKey().ToView(null);
            builder.Entity<StudentGradeForBasicClassDto>().HasNoKey().ToView(null);
            builder.Entity<StudentLodEvaluationListDto>().HasNoKey().ToView(null);
            builder.Entity<StudentGradeEvaluationDto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null);
                entity.Property(e => e.GradeFirstTerm).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.GradeSecondTerm).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.FinalGrade).HasColumnType("decimal(12, 2)");
            });
            builder.Entity<UnsignedStudentLodListDto>().HasNoKey().ToView(null);
            builder.Entity<StudentsMainClassesDto>().HasNoKey().ToView(null);
            builder.Entity<StudentClassTemporal>().HasNoKey().ToView(null);
            builder.Entity<ClassStudentsToEnrollDto>().HasNoKey().ToView(null);
        }

        /// <summary>
        /// Записва ChangeTracker логове на промените в комбинация с журнални данни.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (EntityEntry<StudentClass> entry in ChangeTracker.Entries<StudentClass>())
            {
                if (entry.State == EntityState.Modified)
                {
                    Add(StudentClassHistory.FromStudentClassEntity(entry.GetDatabaseValues().ToObject() as StudentClass));
                }
            }

            if (_userInfo != null)
            {
                DateTime utcNow = DateTime.UtcNow;

                foreach (EntityEntry<ICreatable> entry in ChangeTracker.Entries<ICreatable>())
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedBySysUserId = _userInfo.SysUserID;
                        entry.Entity.CreateDate = utcNow;
                    }
                }

                foreach (EntityEntry<IUpdatable> entry in ChangeTracker.Entries<IUpdatable>())
                {
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.ModifiedBySysUserId = _userInfo.SysUserID;
                        entry.Entity.ModifyDate = utcNow;
                    }
                }
            }

            foreach (EntityEntry<ILodFinalizationCheckable> entry in ChangeTracker.Entries<ILodFinalizationCheckable>())
            {
                if (TrackedStates.Contains(entry.State) && await this.Lodfinalizations.AnyAsync(x => x.PersonId == entry.Entity.PersonId && x.SchoolYear == entry.Entity.SchoolYear && x.IsFinalized))
                {
                    throw new ApiException(GlobalConstants.LodIsFinalizedError(entry.Entity.SchoolYear));
                }
            }

            foreach (EntityEntry<INavigationPropertyLodFinalizationCheckable> entry in ChangeTracker.Entries<INavigationPropertyLodFinalizationCheckable>())
            {
                foreach (NavigationEntry navProp in entry.Navigations.Where(x => x.CurrentValue != null))
                {
                    if (TrackedStates.Contains(navProp.EntityEntry.State)
                        && navProp.CurrentValue is ILodFinalizationCheckable item
                        && await this.Lodfinalizations.AnyAsync(x => x.PersonId == item.PersonId && x.SchoolYear == item.SchoolYear && x.IsFinalized))
                    {
                        throw new ApiException(GlobalConstants.LodIsFinalizedError(item.SchoolYear));
                    }
                }
            }

            var audit = new Z.EntityFramework.Plus.Audit();
            audit.PreSaveChanges(this);
            int rowAffecteds = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null)
            {
                audit.Configuration.AutoSavePreAction(this, audit);
                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return rowAffecteds;
        }

        public override int SaveChanges()
        {
            foreach (EntityEntry<StudentClass> entry in ChangeTracker.Entries<StudentClass>())
            {
                if (entry.State == EntityState.Modified)
                {
                    Add(StudentClassHistory.FromStudentClassEntity(entry.GetDatabaseValues().ToObject() as StudentClass));
                }
            }

            if (_userInfo != null)
            {
                DateTime utcNow = DateTime.UtcNow;

                foreach (EntityEntry<ICreatable> entry in ChangeTracker.Entries<ICreatable>())
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedBySysUserId = _userInfo.SysUserID;
                        entry.Entity.CreateDate = utcNow;
                    }
                }

                foreach (EntityEntry<IUpdatable> entry in ChangeTracker.Entries<IUpdatable>())
                {
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.ModifiedBySysUserId = _userInfo.SysUserID;
                        entry.Entity.ModifyDate = utcNow;
                    }
                }
            }

            foreach (EntityEntry<ILodFinalizationCheckable> entry in ChangeTracker.Entries<ILodFinalizationCheckable>())
            {
                if (TrackedStates.Contains(entry.State) && this.Lodfinalizations.Any(x => x.PersonId == entry.Entity.PersonId && x.SchoolYear == entry.Entity.SchoolYear && x.IsFinalized))
                {
                    throw new ApiException(GlobalConstants.LodIsFinalizedError(entry.Entity.SchoolYear));
                }
            }

            foreach (EntityEntry<INavigationPropertyLodFinalizationCheckable> entry in ChangeTracker.Entries<INavigationPropertyLodFinalizationCheckable>())
            {
                foreach (NavigationEntry navProp in entry.Navigations.Where(x => x.CurrentValue != null))
                {
                    if (TrackedStates.Contains(navProp.EntityEntry.State)
                        && navProp.CurrentValue is ILodFinalizationCheckable item
                        && this.Lodfinalizations.Any(x => x.PersonId == item.PersonId && x.SchoolYear == item.SchoolYear && x.IsFinalized))
                    {
                        throw new ApiException(GlobalConstants.LodIsFinalizedError(item.SchoolYear));
                    }
                }
            }

            var audit = new Z.EntityFramework.Plus.Audit();
            audit.PreSaveChanges(this);
            int rowAffecteds = base.SaveChanges();
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null)
            {
                audit.Configuration.AutoSavePreAction(this, audit);
                base.SaveChanges();
            }

            return rowAffecteds;
        }

        public static void ConfigureAuditManager()
        {
            // If an action for the property AutoSavePreAction is set,
            // audit entries will automatically be saved in the database when SaveChanges or SaveChangesAsync methods are called.
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
            {
                IEnumerable<Audit> customAudits = audit.Entries.Select(x => Audit.From(x, context));
                (context as MONContext).Audits.AddRange(customAudits);
            };

            // You can choose to ignore or not property unchanged with IgnorePropertyUnchanged.
            // By default, properties unchanged are ignored unless it's part of the primary key.
            //AuditManager.DefaultConfiguration.IgnorePropertyUnchanged = true;

            AuditManager.DefaultConfiguration.Exclude(x => true); // Exclude ALL
            AuditManager.DefaultConfiguration.Include<IAuditable>();

            AuditManager.DefaultConfiguration.ExcludeProperty<ICreatable>(x => new { x.CreatedBySysUserId, x.CreateDate });
            AuditManager.DefaultConfiguration.ExcludeProperty<IUpdatable>(x => new { x.ModifiedBySysUserId, x.ModifyDate });
        }
    }
}
