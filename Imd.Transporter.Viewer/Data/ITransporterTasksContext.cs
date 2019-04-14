using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imd.Transporter.Viewer.Data
{
    using System.Data.Entity;

    public interface ITransporterTasksContext : IDisposable
    {
        IDbSet<Task> Tasks { get; set; }
        IDbSet<TaskTransfer> TaskTransfers { get; set; }
        IDbSet<Transfer> Transfers { get; set; }
        IDbSet<Status> Statuses { get; set; }
        IDbSet<Server> Servers { get; set; }
    }
}
