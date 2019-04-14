using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using Imd.Transporter.Viewer.Data;
using Imd.Transporter.Viewer.Repository;
using Imd.Transporter.Viewer.Logging;

namespace Imd.Transporter.Viewer.Controllers
{
    using System.Web;

    using Ninject.Infrastructure;

    [Route("api/stats")]
    public class StatsController : ApiController
    {
        private readonly ITaskTransferRepository repository;

        public string Requester { get; } = HttpContext.Current?.Request.LogonUserIdentity?.Name;

        public StatsController(ITaskTransferRepository repository)
        {
            this.repository = repository;
        }

        [Route("api/stats/summary")]
        public IEnumerable<TaskStats> GetSummary()
        {
            Logger.Info($"User {this.Requester} has requested Today's Stats Summary.");
            return repository.GetTodaysTaskStatsSummary();
        }

        [Route("api/stats/detail/{serverName}")]
        public IEnumerable<TaskStats> GetServerSummary(string serverName )
        {
            Logger.Info($"User {this.Requester} has requested today's stats for server {serverName}.");
            return repository.GetTodaysTaskStatsForServer(serverName);
        }

        [Route("api/stats/lastSixWeeks")]
        public IEnumerable<TaskStats> GetLastSixWeeksSummary()
        {
            return repository.GetLastSixWeeksStats();
        }


    }
}
