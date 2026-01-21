namespace MON.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MON.Models.Audit;
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Z.EntityFramework.Plus;

    public partial class Audit
    {
        public static Audit From(AuditEntry entry, DbContext context)
        {
            IUserInfo userInfo = (context as MONContext)?.UserInfo;

            IEnumerable<AuditEntryPropertyModel> auditEntryProperties = entry.Properties
               .Where(x => x.OldValue != x.NewValue)
               .Select(x => new AuditEntryPropertyModel
               {
                   RelationName = x.RelationName,
                   PropertyName = x.PropertyName,
                   OldValue = x.OldValueFormatted,
                   NewValue = x.NewValueFormatted,
                   IsKey = x.IsKey
               });

            Person person = (context as MONContext).People
                .AsNoTracking()
                .DeferredFirstOrDefault(x => x.PersonId == userInfo.PersonId)
                .FromCache(new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) }, "UserInfo");

            int? objectId = (int?)entry.Properties.FirstOrDefault(x => x.PropertyName == "Id")?.NewValue;
            short? schoolYear = (short?)entry.Properties.FirstOrDefault(x => x.PropertyName == "SchoolYear")?.NewValue;
            string data = JsonConvert.SerializeObject(auditEntryProperties);

            Audit customAudit = new Audit
            {
                Username = userInfo?.Username,
                FirstName = person?.FirstName,
                MiddleName = person?.MiddleName,
                LastName = person?.LastName,
                PersonId = userInfo?.PersonId,
                SysUserId = userInfo?.SysUserID,
                SysRoleId = userInfo?.SysRoleID,
                RemoteIpAddress = userInfo?.ClientIp,
                UserAgent = userInfo?.UserAgent,
                InstId = userInfo?.InstitutionID,
                Action = entry.StateName,
                ObjectName = entry.EntitySetName?.Substring(0, Math.Min(entry.EntitySetName.Length, 50)),
                AuditCorrelationId = context.ContextId.InstanceId.ToString(),
                DateUtc = DateTime.UtcNow,
                ObjectId = objectId,
                AuditModuleId = (int)AuditModuleEnum.Students,
                Data = data,
                LoginSessionId = userInfo.LoginSessionId,
                SchoolYear = schoolYear
            };

            return customAudit;
        }
    }
}
