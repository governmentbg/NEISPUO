namespace MON.Services.Implementations.DiplomaCode
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Models.Enums.UserManagement;
    using MON.Models.UserManagement;
    using MON.Services.Interfaces;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Diploma_3_34_Code : CodeService
    {
        public Diploma_3_34_Code(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies, serviceProvider)
        {
        }
    }
}
