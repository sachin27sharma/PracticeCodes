using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtualCashCard.Core;
using VirtualCashCard.Interface;

namespace VirtualCashCard
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddTransient<IDataContext, FileReader>();
            services.AddTransient<ICustomerAccountRepository, CustomerAccountRepository>();
            services.AddSingleton<ICardService, CardService>();
        }
    }
}
