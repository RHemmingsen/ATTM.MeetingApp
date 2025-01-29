using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddDbContext<MeetingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("MeetingDbConnectionString"))
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IMeetingRepository, MeetingRepository>();
            return services;
        }
    }
}
