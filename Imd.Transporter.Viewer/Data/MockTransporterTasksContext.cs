using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Imd.Transporter.Viewer.Data
{
    /// <summary>
    /// This class is merely here for mocking up test transporter data. It does nothing at all, which means I've probably implemented a bad design pattern.
    /// </summary>
    public class MockTransporterTasksContext : ITransporterTasksContext
    {
        public IDbSet<Server> Servers { get; set; }

        public IDbSet<Status> Statuses { get; set; }

        public IDbSet<Task> Tasks { get; set; }

        public IDbSet<TaskTransfer> TaskTransfers { get; set; }

        public IDbSet<Transfer> Transfers { get; set; }

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
        // ~MockTransporterTasksContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}