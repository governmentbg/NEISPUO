namespace Helpdesk.DataAccess
{
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class HelpdeskContext
    {
        private readonly IUserInfo _userInfo;

        public HelpdeskContext(DbContextOptions<HelpdeskContext> options,
            IUserInfo userInfo)
            : base(options)
        {
            _userInfo = userInfo;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
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

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
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

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }
}
