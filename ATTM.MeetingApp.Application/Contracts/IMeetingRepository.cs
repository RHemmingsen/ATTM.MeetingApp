using ATTM.MeetingApp.Domain.Entities;

namespace ATTM.MeetingApp.Application.Contracts;

public interface IMeetingRepository : IAsyncRepository<Meeting>
{
    Task<IReadOnlyList<Meeting>> ListOrderedBySeatsAsync(CancellationToken cancellationToken = default);
}
