using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Domain.Entities
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LocationsEnum Location { get; set; }
        public int Seats { get; set; }
    }
}
