using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Persistence.Configurations;
using ATTM.MeetingApp.Persistence.Repositories;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(options => 
            {
                var keyVaultUri = configuration["KeyVault:VaultUri"];
                var credential = new DefaultAzureCredential();
                var client = new SecretClient(new Uri(keyVaultUri), credential);

                var secret = client.GetSecret("MeetingDbConnectionString");
                options.MeetingDbConnectionString = secret.Value.Value;
            });
            services.AddDbContext<MeetingDbContext>((provider, options) =>
            {
                var settings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                options.UseNpgsql(settings.MeetingDbConnectionString)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            });
                
            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IMeetingRepository, MeetingRepository>();

            return services;
        }
    }
}
