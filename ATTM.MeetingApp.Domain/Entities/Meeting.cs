using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Domain.Entities
{
    public class Meeting : AudibleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public LocationsEnum Location { get; set; }
        public int Seats { get; set; }
    }
}
