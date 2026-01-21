using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace MON.Report.Service
{
    public static class ServiceConfigurationsUtil
    {

        /// <summary>
        /// Помощен метод, който регистрира в DI контейнера всички наследници на даден клас или интерфейс.
        /// Би могъл да се изнесе в проектно-независима библиотека. По подобен начин работи MediatR.
        /// </summary>
        /// 
        public static void AddScopedAll<BaseT>(this IServiceCollection services, Assembly fromAssembly)
        {
            Type baseType = typeof(BaseT);
            if (fromAssembly == null)
            {
                fromAssembly = baseType.Assembly;
            }
            foreach (Type serviceClass in fromAssembly.DefinedTypes.Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)))
            {
                services.AddScoped(serviceClass);
            }
        }
    }
}
