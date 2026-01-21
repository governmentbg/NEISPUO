namespace MON.API.Pluralizer
{
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.DependencyInjection;

    internal class CustomDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            => serviceCollection.AddSingleton<IPluralizer, HumanizerPluralizer>();
    }

}
