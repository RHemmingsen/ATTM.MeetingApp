using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATTM.MeetingApp.Persistence.Repositories;

public class MeetingRepository(MeetingDbContext dbContext)
    : BaseRepository<Meeting>(dbContext), IMeetingRepository
{
    public async Task<IReadOnlyList<Meeting>> ListOrderedBySeatsAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Meetings.AsNoTracking().OrderBy(x => x.Seats).ToListAsync(cancellationToken);
}
