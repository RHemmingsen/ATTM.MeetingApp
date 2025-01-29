using ATTM.MeetingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Contracts
{
    public interface IMeetingRepository : IAsyncRepository<Meeting>
    {
    }
}
