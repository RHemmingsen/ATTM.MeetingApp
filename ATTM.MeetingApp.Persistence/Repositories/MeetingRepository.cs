using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Persistence.Repositories
{
    public class MeetingRepository : BaseRepository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
