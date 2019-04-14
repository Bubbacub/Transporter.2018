using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Imd.Transporter.Viewer.Data;
using System.Linq;
using Imd.Transporter.Viewer.Controllers;
using Imd.Transporter.Viewer.Repository;

namespace Imd.Transporter.Viewer.Tests
{
    using System.Collections.Generic;

    [TestClass]
    public class TaskTransferTests
    {
        private ITaskTransferRepository repo;
        [TestInitialize]
        public void SetUp()
        {
            repo = new TaskTransferRepository();
        }

        [TestCleanup]
        public void TearDown()
        {
            repo.Dispose();
        }
        [TestMethod, Ignore]
        public void GetAllTasks()
        {
            var tasks = repo.Get();
            Assert.IsNotNull(tasks);
        }

        [TestMethod]
        public void GetActiveTasks()
        {
            var tasks = repo.GetActive();

            if (tasks != null)
            {
                var taskTransfers = tasks as IList<TaskTransfer> ?? tasks.ToList();
                Assert.IsTrue(taskTransfers.First().CreatedDate > DateTime.Now.AddMinutes(-20));
            }
        }

        [TestMethod]
        public void GetTasksByServer()
        {
            var tasks = repo.Get(t=> t.Transporter.Contains("transport6"));
            if (tasks != null)
            {
                var taskTransfers  = tasks as IList<TaskTransfer> ?? tasks.ToList();
                Assert.IsNotNull(taskTransfers.FirstOrDefault(t=> t.Transporter.ToLower().Contains("transport6")));
                Assert.IsNull(taskTransfers.FirstOrDefault(t => t.Transporter.ToLower().Contains("transport1")));
            }
        }
        [TestMethod]
        public void GetTasksByFileNameUsingRepo()
        {
            var controller = new TaskTransferController();
            var tasks = controller.GetByFileName("XX_PLAN_GULL_TRON_0001_008_X_HD");
            var testTask = tasks.FirstOrDefault();
            Assert.IsTrue(testTask != null && testTask.Filename.ToUpper().Contains("XX_PLAN_GULL_TRON_0001_008_X_HD"));
        }

        [TestMethod]
        public void GetTasksByFileNameUsingController()
        {
            var tasks = repo.Get(t => t.Filename.Contains("XX_PLAN_GULL_TRON_0001_008_X_HD"));
            var testTask = tasks.FirstOrDefault();
            Assert.IsTrue(testTask != null && testTask.Filename.ToUpper().Contains("XX_PLAN_GULL_TRON_0001_008_X_HD"));
        }

        [TestMethod]
        public void TaskTransferControllerGetByStatusError()
        {
            var controller = new TaskTransferController();
            var tasks = controller.GetByStatus("Error");
            Assert.IsTrue(tasks.Where(t => t.Status != "Error").ToList().Count == 0);
        }

        [TestMethod]
        public void TaskTransferControllerGetByStatusCompleted()
        {
            var controller = new TaskTransferController();
            var tasks = controller.GetByStatus("Completed");
            Assert.IsTrue(tasks.Where(t => t.Status != "Completed").ToList().Count == 0);
        }

        [TestMethod]
        public void GetTaskStatuses()
        {
            var controller = new TaskTransferController();
            var statusCodes = controller.GetTaskStatuses();
            Assert.IsNotNull(statusCodes);
            Assert.IsTrue(statusCodes.Contains("Error"));
        }
        [TestMethod]
        public void GetTransporterServers()
        {
            var controller = new TaskTransferController();
            var servers = controller.GetTransporters();
            Assert.IsNotNull(servers);
        }
        [TestMethod]
        public void GetBuildVersion()
        {
            var controller = new BannerController();
            var version = controller.GetBuildNumber();
            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void GetTasksWithNoFilter()
        {
            var controller = new TaskTransferController();
            var noFilters = controller.GetWithFilterCriteria();

            foreach (var task in noFilters)
            {
                Assert.IsTrue(task.CreatedDate >= DateTime.Now.AddMinutes(-5) || (task.Status == "Pending" || task.Status == "In Progress"));
            }
        }

        [TestMethod]
        public void GetTasksWithStatusFilter()
        {
            var controller = new TaskTransferController();
            var statusFilter = controller.GetWithFilterCriteria("statusText=Completed");
            foreach (var task in statusFilter)
            {
                Assert.AreEqual(task.Status, "Completed");
            }
        }

        [TestMethod]
        public void GetTasksWithServerFilter()
        {
            var controller = new TaskTransferController();
            var serverFilter = controller.GetWithFilterCriteria("serverName=SVR-TRANSPORT1");
            foreach (var task in serverFilter)
            {
                Assert.AreEqual(task.Transporter, "SVR-TRANSPORT1");
            }
        }

        [TestMethod]
        public void GetTasksWithFileNameFilter()
        {
            var controller = new TaskTransferController();
            var fileFilter = controller.GetWithFilterCriteria("fileName=TESTCASSE");
            foreach (var task in fileFilter)
            {
                Assert.IsTrue(task.Filename.Contains("TESTCASSE"));
            }
        }

        [TestMethod]
        public void GetTasksWithAllFilters()
        {
            var controller = new TaskTransferController();
            var allFilter = controller.GetWithFilterCriteria("statusText=Cancelled|serverName=SVR-TRANSPORT6|fileName=test");
            foreach (var task in allFilter)
            {
                Assert.AreEqual(task.Status, "Cancelled");
                Assert.AreEqual(task.Transporter, "SVR-TRANSPORT6");
                Assert.IsTrue(task.Filename.ToLower().Contains("test"));
            }
        }

        [TestMethod]
        public void GetTodaysStats()
        {
            var stats = repo.GetTodaysTaskStatsForServer("SVR-TRANSPORT2");

            var statsList = stats as IList<TaskStats> ?? stats.ToList();
            if (statsList.Any())
            {
                foreach (var stat in statsList)
                {
                    Assert.IsTrue(stat.Volume > 0);
                    Assert.AreEqual(stat.ServerName, "SVR-TRANSPORT2");
                }
            }
        }

        [TestMethod]
        public void GetTodaysStatsSummary()
        {
            var summary = repo.GetTodaysTaskStatsSummary();
            var summaryList = summary as IList<TaskStats> ?? summary.ToList();

            if (summaryList.Any())
            {
                summaryList.ToList().ForEach(s=> Assert.IsTrue(s.Volume > 0));
            }
        }

        [TestMethod]
        public void GetLastSixWeeksStats()
        {
            var summary = repo.GetLastSixWeeksStats();
            var summaryList = summary as IList<TaskStats> ?? summary.ToList();

            if (summaryList.Any())
            {
                summaryList.ToList().ForEach(s => Assert.IsTrue(s.Volume >= 0));
            }
        }
    }
}
