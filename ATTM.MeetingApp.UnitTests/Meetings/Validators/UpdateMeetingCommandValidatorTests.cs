using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Domain.Entities;

namespace ATTM.MeetingApp.UnitTests.Meetings.Validators;

public class UpdateMeetingCommandValidatorTests
{
    private static readonly Guid Id = new("00000000-0000-0000-0000-000000000001");
    private readonly UpdateMeetingCommandValidator _validator = new();

    [Fact]
    public void UpdateMeetingCommandValidator_ValidCommand_Passes()
        => Assert.True(_validator.Validate(new UpdateMeetingCommand(Id, "Planning", LocationsEnum.ConferenceRoom, 8)).IsValid);

    [Fact]
    public void UpdateMeetingCommandValidator_EmptyId_Fails()
        => Assert.False(_validator.Validate(new UpdateMeetingCommand(Guid.Empty, "Planning", LocationsEnum.Office, 8)).IsValid);

    [Fact]
    public void UpdateMeetingCommandValidator_EmptyName_Fails()
        => Assert.False(_validator.Validate(new UpdateMeetingCommand(Id, "", LocationsEnum.Office, 8)).IsValid);

    [Fact]
    public void UpdateMeetingCommandValidator_NameTooLong_Fails()
        => Assert.False(_validator.Validate(new UpdateMeetingCommand(Id, new string('x', Meeting.NameMaxLength + 1), LocationsEnum.Office, 8)).IsValid);

    [Fact]
    public void UpdateMeetingCommandValidator_UndefinedLocation_Fails()
        => Assert.False(_validator.Validate(new UpdateMeetingCommand(Id, "Planning", (LocationsEnum)99, 8)).IsValid);

    [Fact]
    public void UpdateMeetingCommandValidator_NonPositiveSeats_Fails()
        => Assert.False(_validator.Validate(new UpdateMeetingCommand(Id, "Planning", LocationsEnum.Office, 0)).IsValid);
}
