using ATTM.MeetingApp.Application;
using AutoMapper;

namespace ATTM.MeetingApp.UnitTests.Mocks;

public static class MapperFactory
{
    public static IMapper Create()
        => new MapperConfiguration(c => c.AddProfile<MappingProfiles>()).CreateMapper();
}
