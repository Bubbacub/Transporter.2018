using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Imd.Transporter.Viewer.Data;

namespace Imd.Transporter.Viewer.Repository
{
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Threading;
    using System.Web.Hosting;

    using Imd.Transporter.Viewer.Properties;

    public class MockTaskTransferRepository : ITaskTransferRepository
    {
        private readonly Random rand = RandomProvider.GetThreadRandom();
        //public ITransporterTasksContext Context { get;set;}
        private static DateTime lastRefreshDate = DateTime.Now;
        private IQueryable<TaskTransfer> MockTasks { get; set; }
        public string Requester { get; } = HttpContext.Current?.Request.LogonUserIdentity?.Name; // In order for this to work, the website's Authentication needs to be switched from Anonymous to Windows in IIS.

        public MockTaskTransferRepository()
        {
            //this.Context = new MockTransporterTasksContext();
        }

        public IEnumerable<TaskTransfer> Get(Expression<Func<TaskTransfer, bool>> filter = null, Func<IQueryable<TaskTransfer>, IOrderedQueryable<TaskTransfer>> orderBy = null)
        {
            this.CreateTaskTransfers();
            IQueryable<TaskTransfer> query = MockTasks;

            if (filter != null)
            {
                Logging.Logger.Debug($"The user {Requester} has performed a search with criteria: {filter}.");
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

       

        public IEnumerable<TaskTransfer> GetActive()
        {
            var active = this.Get(t => t.Status == "pending" || t.Status == "in progress");
            // this is just a mock repo, so I don't care about the hard-coding.
            var recent = this.Get(t => t.CreatedDate > DateTime.Now.AddMinutes(-5));
            return active.Concat(recent).Distinct().OrderBy(a => a.CompletedDate).ThenBy(a => a.StatusId);
        }

        public IEnumerable<TaskTransfer> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TaskTransfer> GetByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTaskStatusCodes()
        {
            return new List<string> { "Pending", "In Progress", "Cancelled", "Error", "Completed" };
        }
        public IEnumerable<string> GetTransporters()
        {
            var transporters = new List<string>
            {
                "Transport1",
                "Transport2",
                "Transport3",
                "Transport4",
                "Transport5",
                "Transport6"
            };

            return transporters;
        }

        private void CreateTaskTransfers()
        {
            if (MockTasks != null && lastRefreshDate >= DateTime.Now.AddSeconds(-20))
            {
                return;
            }
            var tasks = new List<TaskTransfer>();
            var maxTasks = rand.Next(1, 40);

            for (var i = 0; i < maxTasks; i++)
            {
                tasks.Add(this.CreateRandomTask(i));
            }
            MockTasks = tasks.AsQueryable();
            lastRefreshDate = DateTime.Now;

            // Simulate some elapsed time.
            var milli = MockTasks.Count() * 10;
            var mockWaitTime = rand.Next(milli, milli * 2);
            Thread.Sleep(mockWaitTime);
        }

        private TaskTransfer CreateRandomTask(int taskId)
        {
            var task = new TaskTransfer();
            int statId;

            task.TaskId = taskId;
            task.Status = GetRandomStatus(out statId);
            task.StatusId = statId;
            task.Destination = GetRandomDestination();
            task.CreatedDate = DateTime.Now.AddMinutes(-rand.Next(1, 20));
            task.Filename = $"{Guid.NewGuid()}{GetRandomExtension()}";
            task.Transporter = GetRandomTransporter();

            if (task.Status == "Error")
            {
                task.ErrorDate = DateTime.Now;
                task.ErrorText = $"Generic Error{taskId}";
            }
            else if (task.Status == "Completed")
            {
                task.CompletedDate = DateTime.Now;
                task.ProcessingStartedDate = task.CompletedDate.Value.AddSeconds(-rand.Next(3, 40));
                task.TimeWaitingForFreeThread = rand.Next(0, 40);
                task.Destination = $@"\\test\destination\Folder{taskId}";
                task.ProcessingTime = 1;
                task.TransferCompleted = task.ProcessingStartedDate.Value.AddSeconds((int)task.ProcessingTime);
            }
            return task;
        }

        private string GetRandomExtension()
        {
            var extensions = new List<string> { "_SD.mov", "_SD.mxf", "_HD.mxf" };
            return extensions[rand.Next(0, extensions.Count)];
        }

        private string GetRandomTransporter()
        {
            var servers = this.GetTransporters().ToArray();
            return servers[rand.Next(0, servers.Length)];
        }

        private string GetRandomDestination()
        {
            var destinations = new List<string>
            {
                @"\\transporter\test\ToOmneonHDNoAudio",
                @"\\transporter\test\ToOmneonSD-10",
                @"\\transporter\test\ToOmneonSDNoAudio",
                @"\\transporter\test\ToOmneonTest"
            };

            return destinations.ToArray()[rand.Next(0, destinations.Count)];
        }

        private string GetRandomStatus(out int statusId)
        {
            Dictionary<int, string> statuses = GetStatusCodesWithDescription();
            statusId = rand.Next(-2, 3);
            return statuses[statusId];
        }

        private static Dictionary<int, string> GetStatusCodesWithDescription()
        {
            return new Dictionary<int, string>
            {
                { 0, "Pending" },
                { -2, "In Progress" },
                { 2, "Cancelled" },
                { -1, "Error" },
                { 1, "Completed" }
            };
        }

        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MockTaskTransferRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        public IEnumerable<TaskStats> GetTodaysTaskStatsSummary()
        {
            this.CreateTaskTransfers();
            var today = MockTasks.GroupBy(t => new { t.Transporter });
            return
                today.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                ServerName = task.Key.Transporter,
                                Volume = task.Count()
                            })
                    .OrderBy(t => t.ServerName)
                    .ThenByDescending(t => t.Volume);
        }

        public IEnumerable<TaskStats> GetTodaysTaskStats()
        {
            this.CreateTaskTransfers();
            var today = MockTasks.GroupBy(t => new { t.Status, t.Transporter });
            return
                today.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                StatusText = task.Key.Status,
                                ServerName = task.Key.Transporter,
                                Volume = task.Count()
                            })
                    .OrderBy(t => t.ServerName)
                    .ThenByDescending(t => t.Volume);
        }

        public IEnumerable<TaskStats> GetTodaysTaskStatsForServer(string serverName)
        {
            this.CreateTaskTransfers();
            var today = MockTasks.Where(t=> t.Transporter == serverName).GroupBy(t => new { t.Status, t.Transporter });
            return
                today.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                StatusText = task.Key.Status,
                                ServerName = task.Key.Transporter,
                                Volume = task.Count()
                            })
                    .OrderBy(t => t.ServerName)
                    .ThenByDescending(t => t.Volume);
        }

        public IEnumerable<TaskStats> GetLastSixWeeksStats()
        {
            this.CreateTaskTransfers();
            var sixWeeksAgo = DateTime.Now.AddDays(-42);
            var lastSix =
                MockTasks.Where(t => t.CreatedDate >= sixWeeksAgo)
                    .GroupBy(t => new { t.Transporter,
                        WeekNo = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Convert.ToDateTime(t.CreatedDate), CalendarWeekRule.FirstDay, DayOfWeek.Monday) });
            return
                lastSix.ToList()
                    .Select(
                        task =>
                            new TaskStats
                            {
                                ServerName = task.Key.Transporter,
                                WeekOfYear = task.Key.WeekNo,
                                Volume = task.Count()
                            })
                    .OrderBy(t => t.WeekOfYear)
                    .ThenByDescending(t => t.Volume);
        }

        #endregion
    }

    public static class RandomProvider
    {
        private static int seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> RandomWrapper = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment(ref seed))
        );

        public static Random GetThreadRandom()
        {
            return RandomWrapper.Value;
        }
    }
}