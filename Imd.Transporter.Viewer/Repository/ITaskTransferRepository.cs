namespace Imd.Transporter.Viewer.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Imd.Transporter.Viewer.Data;

    public interface ITaskTransferRepository : IDisposable
    {
        IEnumerable<TaskTransfer> Get(
            Expression<Func<TaskTransfer, bool>> filter = null,
            Func<IQueryable<TaskTransfer>, IOrderedQueryable<TaskTransfer>> orderBy = null);

        IEnumerable<TaskTransfer> GetAll();

        IEnumerable<TaskTransfer> GetByStatus(string status);

        IEnumerable<TaskTransfer> GetActive();

        IEnumerable<string> GetTaskStatusCodes();

        IEnumerable<string> GetTransporters();

        IEnumerable<TaskStats> GetTodaysTaskStatsSummary();
        IEnumerable<TaskStats> GetTodaysTaskStatsForServer(string serverName);

        IEnumerable<TaskStats> GetLastSixWeeksStats();
    }

    
}