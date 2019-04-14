using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable All

namespace Imd.Transporter.Viewer.Tests
{
    using Imd.Transporter.Viewer.Data;
    using Imd.Transporter.Viewer.Repository;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MockTaskTransferTests
    {
        private ITaskTransferRepository repo;
        [TestInitialize]
        public void SetUp()
        {
            repo = new MockTaskTransferRepository();
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void GetMockServers()
        {
            var servers = repo.GetTransporters();
            Assert.AreEqual(servers.Count(), 6);
        }

        [TestMethod]
        public void GetMockStatusCodes()
        {
            var statusCodes = repo.GetTaskStatusCodes();
            Assert.IsNotNull(statusCodes);
            Assert.IsTrue(statusCodes.Contains("Pending"));
            Assert.IsTrue(statusCodes.Contains("In Progress"));
            Assert.IsTrue(statusCodes.Contains("Cancelled"));
            Assert.IsTrue(statusCodes.Contains("Error"));
            Assert.IsTrue(statusCodes.Contains("Completed"));
        }

        [TestMethod]
        public void GetAllMockTasks()
        {
            var tasks = repo.Get();

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.IsNotNull(task.TaskId);
                    Assert.IsNotNull(task.Status);
                    Assert.IsNotNull(task.Filename);
                }
            }
        }

        [TestMethod]
        public void GetCompletedMockTasks()
        {
            var tasks = repo.Get(t=> t.StatusId == 1);

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.AreEqual(task.StatusId, 1);
                    Assert.IsNotNull(task.CompletedDate);
                }
            }
        }

        [TestMethod]
        public void GetActiveMockTasks()
        {
            var tasks = repo.GetActive();

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.IsTrue(task.CreatedDate >= DateTime.Now.AddMinutes(-20) || task.Status == "Pending" || task.Status == "In Progress");
                }
            }
        }

        [TestMethod]
        public void GetMockTasksByServer()
        {
            var tasks = repo.Get(t => t.Transporter == "TRANSPORT1");

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.IsTrue(task.Transporter == "TRANSPORT1");
                }
            }
        }

        [TestMethod]
        public void GetMockTasksByStatus()
        {
            var tasks = repo.Get(t => t.Status == "Completed");

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.IsTrue(task.Status == "Completed");
                }
            }
        }

        [TestMethod]
        public void GetMockTasksByFileName()
        {
            var tasks = repo.Get(t => t.Filename.EndsWith("_SD.mxf"));

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Assert.IsTrue(task.Filename.EndsWith("_SD.mxf"));
                }
            }
        }

        [TestMethod]
        public void GetMockTaskStatsSummary()
        {
            var stats = repo.GetTodaysTaskStatsSummary();
            if (stats.Any())
            {
                stats.ToList().ForEach(s=> Assert.IsTrue(s.Volume > 0));
            }
        }

        [TestMethod]
        public void GetMockTaskStats()
        {
            var stats = repo.GetTodaysTaskStatsForServer("TRANSPORT1").OrderBy(s=> s.ServerName).ThenByDescending(s=> s.Volume);
            if (stats.Any())
            {
                foreach (var stat in stats)
                {
                    Assert.IsTrue(stat.Volume > 0);
                    Assert.AreEqual(stat.ServerName, "TRANSPORT1");
                }
            }
        }
    }
}
