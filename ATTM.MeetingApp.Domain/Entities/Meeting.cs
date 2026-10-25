namespace ATTM.MeetingApp.Domain.Entities;

public class Meeting
{
    public const int NameMaxLength = 100;

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LocationsEnum Location { get; set; }
    public int Seats { get; set; }
}
