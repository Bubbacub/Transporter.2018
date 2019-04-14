namespace Imd.Transporter.Viewer.Data
{
    using System.Data.Entity;

    public partial class TransporterTasksContext : DbContext, ITransporterTasksContext
    {
        public TransporterTasksContext()
            : base("name=TransporterTasksContext")
        {
        }

        public virtual IDbSet<Task> Tasks { get; set; }
        public virtual IDbSet<TaskTransfer> TaskTransfers { get; set; }
        public virtual IDbSet<Transfer> Transfers { get; set; }
        public virtual IDbSet<Status> Statuses { get; set; }
        public virtual IDbSet<Server> Servers { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
