namespace Imd.Transporter.Viewer.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;

    using Imd.Transporter.Viewer.Data;
    using Imd.Transporter.Viewer.Properties;

    public class TaskTransferRepository : ITaskTransferRepository
    {
        public ITransporterTasksContext Context { get; set; }

        private readonly int taskCreatedSinceMinutes;
        
        public TaskTransferRepository()
        {
            this.Context = new TransporterTasksContext();
            this.taskCreatedSinceMinutes = Settings.Default.recentTaskAgeInMinutes;
        }

        /// <summary>
        /// Generic method, allowing you to pass in linq filter and ordering queries.
        /// </summary>
        /// <param name="filter">e.g. t => t.Transporter == serverName</param>
        /// <param name="orderBy">e.g. , o => o.OrderBy(ts => ts.CreatedDate)</param>
        /// <returns></returns>
        public IEnumerable<TaskTransfer> Get(
            Expression<Func<TaskTransfer, bool>> filter = null,
            Func<IQueryable<TaskTransfer>, IOrderedQueryable<TaskTransfer>> orderBy = null
            )
        {
            IQueryable<TaskTransfer> query = this.Context.TaskTransfers;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public IEnumerable<TaskTransfer> GetAll()
        {
            return Context.TaskTransfers.OrderByDescending(t => t.TaskId).Distinct().Take(500).ToList();
        }

        public IEnumerable<TaskTransfer> GetByStatus(string status)
        {
            var tasks =
                    Context.TaskTransfers
                        .Where(t => t.Status == status)
                        .OrderBy(t => t.Transporter);
            return tasks.Distinct();
        }

        public IEnumerable<TaskTransfer> GetActive()
        {
            var nMinutesAgo = DateTime.Now.AddMinutes(-taskCreatedSinceMinutes);
            // Allows the user to see what's just been processed, as well as what is active now.
            // Concat will remove any dupes, so there's no need to exclude "pending" and "in progress" statuses.
            var mostRecentTasks =
                Context.TaskTransfers.Where(l => l.CreatedDate >= nMinutesAgo);
            // Active + most recent.           
            return Context.TaskTransfers.Where(t => t.Status == "in progress" || t.Status == "pending")
                .Concat(mostRecentTasks).Distinct().OrderBy(a => a.CompletedDate).ThenBy(a => a.StatusId);
        }

        public IEnumerable<TaskStats> GetTodaysTaskStatsSummary()
        {
            var todays =
                Context.TaskTransfers.Where(t => t.CreatedDate >= DateTime.Today)
                    .GroupBy(t => new { t.Transporter });

            var results = 
                todays.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                ServerName = task.Key.Transporter,
                                Volume = task.Count(),
                                AverageTransferSpeedMbps = Convert.ToDouble(task.Sum(t => t.TransferSpeed ?? 0) / task.Count() / 1024)
                            })
                    .OrderBy(t => t.ServerName)
                    .ThenByDescending(t => t.Volume);
            return results;
        }

        public IEnumerable<TaskStats> GetTodaysTaskStatsForServer(string serverName)
        {
            var todays =
                Context.TaskTransfers.Where(t => t.CreatedDate >= DateTime.Today && t.Transporter == serverName)
                    .GroupBy(t => new { t.Transporter, t.Status });

            return
                todays.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                StatusText = task.Key.Status,
                                ServerName = task.Key.Transporter,
                                Volume = task.Count(),
                                AverageTransferSpeedMbps = Convert.ToDouble(task.Sum(t=> t.TransferSpeed ?? 0)/task.Count() / 1024)
                            })
                    .OrderBy(t => t.ServerName)
                    .ThenByDescending(t => t.Volume);
        }

        public IEnumerable<TaskStats> GetLastSixWeeksStats()
        {
            var sixWeeksAgo = DateTime.Now.AddDays(-42);
            var lastSix =
                 Context.TaskTransfers.Where(t => t.CreatedDate >= sixWeeksAgo).ToList()
                    .GroupBy(t => new {
                        t.Transporter,
                        WeekNo = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Convert.ToDateTime(t.CreatedDate), CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                        Convert.ToDateTime(t.CreatedDate).Year
                    });
            return
                lastSix
                    .Select(
                        task =>
                            new TaskStats
                            {
                                ServerName = task.Key.Transporter,
                                WeekOfYear = task.Key.WeekNo,
                                Year = task.Key.Year,
                                Volume = task.Count()
                            })
                    .OrderBy(t => t.Year)
                    .ThenBy(t => t.WeekOfYear)
                    .ThenByDescending(t => t.Volume);
        }

        public IEnumerable<string> GetTaskStatusCodes()
        {
            var statuses = Context.Statuses.Select(s => s.Description).ToList();
            return statuses;
        }

        public IEnumerable<string> GetTransporters()
        {
            return Context.Servers.Where(s => s.Active).OrderBy(s => s.Name).Select(n => n.Name);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}