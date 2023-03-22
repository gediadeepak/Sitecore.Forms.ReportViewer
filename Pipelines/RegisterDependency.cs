using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Forms.Report.Viewer.Controllers;
using Sitecore.Forms.Report.Viewer.DataProvider;
using Sitecore.Forms.Report.Viewer.Helpers;
using Sitecore.Forms.Report.Viewer.Repository;

namespace Sitecore.Forms.Report.Viewer.Pipelines
{
    public class RegisterDependency : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<FormReportController>();
            serviceCollection.AddTransient<ISqlHelper, SqlHelper>();
            serviceCollection.AddTransient<ISqlDataProvider, SqlDataProvider>();
            serviceCollection.AddTransient<IFormReportRepository, FormReportRepository>();
        }
    }
}