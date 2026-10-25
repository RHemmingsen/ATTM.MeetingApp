using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Domain.Entities;

namespace ATTM.MeetingApp.UnitTests.Meetings.Validators;

public class CreateMeetingCommandValidatorTests
{
    private readonly CreateMeetingCommandValidator _validator = new();

    [Theory]
    [InlineData(LocationsEnum.ConferenceRoom)]
    [InlineData(LocationsEnum.Office)]
    public void CreateMeetingCommandValidator_ValidCommand_Passes(LocationsEnum location)
        => Assert.True(_validator.Validate(new CreateMeetingCommand("Planning", location, 8)).IsValid);

    [Fact]
    public void CreateMeetingCommandValidator_EmptyName_Fails()
        => Assert.False(_validator.Validate(new CreateMeetingCommand("", LocationsEnum.Office, 8)).IsValid);

    [Fact]
    public void CreateMeetingCommandValidator_NameTooLong_Fails()
        => Assert.False(_validator.Validate(new CreateMeetingCommand(new string('x', Meeting.NameMaxLength + 1), LocationsEnum.Office, 8)).IsValid);

    [Fact]
    public void CreateMeetingCommandValidator_UndefinedLocation_Fails()
        => Assert.False(_validator.Validate(new CreateMeetingCommand("Planning", (LocationsEnum)99, 8)).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateMeetingCommandValidator_NonPositiveSeats_Fails(int seats)
        => Assert.False(_validator.Validate(new CreateMeetingCommand("Planning", LocationsEnum.Office, seats)).IsValid);
}
