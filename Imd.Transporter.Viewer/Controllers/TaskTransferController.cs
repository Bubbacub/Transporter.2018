namespace Imd.Transporter.Viewer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Data;

    using Imd.Transporter.Viewer.Logging;

    using Properties;
    using Repository;

    [Route("api/tasks")]
    public class TaskTransferController : ApiController
    {
        public struct FilterCriteria
        {
            public string StatusText { get; set; }
            public string ServerName { get; set; }
            public string FileName { get; set; }

            public bool HasNoFilters => string.IsNullOrWhiteSpace(this.StatusText) && string.IsNullOrWhiteSpace(this.ServerName)
                                        && string.IsNullOrWhiteSpace(this.FileName);
        }

        public string Status { get; set; }
        private readonly ITaskTransferRepository taskRepository;
        private readonly int maxTasks = Settings.Default.MaxTasksReturned;
        private readonly DateTime earliestCreated = DateTime.Now.AddDays(-Settings.Default.oldestTaskAgeInDays);
        
        public string Requester { get; } = HttpContext.Current?.Request.LogonUserIdentity?.Name; // In order for this to work, the website's Authentication needs to be switched from Anonymous to Windows in IIS.

        public TaskTransferController()
        {
            this.taskRepository = new TaskTransferRepository();
        }

        public TaskTransferController(ITaskTransferRepository repository)
        {
            this.taskRepository = repository;
        }

        public IEnumerable<TaskTransfer> Get()
        {
            return taskRepository.GetActive();
        }

        [HttpGet, Route("api/tasksWithFilters/{filterDelimited?}")]
        public IEnumerable<TaskTransfer> GetWithFilterCriteria(string filterDelimited = "")
        {
            var filterOptions = ParseFilterString(filterDelimited);

            if (filterOptions.HasNoFilters)
            {
                return Get();
            }
            Logger.Info($"The user {this.Requester} has performed a search using filters: {filterDelimited}.");
            return
                taskRepository.Get(
                    t => t.Status.Contains(filterOptions.StatusText) 
                    && t.Transporter.Contains(filterOptions.ServerName) 
                    && t.Filename.Contains(filterOptions.FileName),
                    ob => ob.OrderByDescending(ts => ts.TaskId)).Distinct().Take(maxTasks);
        }

        /// <summary>
        /// Converts the filterCriteria string into a more useable FilterCriteria object.
        /// </summary>
        /// <param name="filterDelimited">Passed in the format: "statusText=something|serverName=something|fileName=something"</param>
        /// <returns></returns>
        private FilterCriteria ParseFilterString(string filterDelimited)
        {
            if (string.IsNullOrWhiteSpace(filterDelimited))
            {
                return new FilterCriteria();
            }

            var filterDictionary = filterDelimited.Split('|').ToDictionary(f => f.Split('=')[0], f => f.Split('=')[1]);
            
            var criteria = new FilterCriteria()
            {
                StatusText = filterDictionary.ContainsKey("statusText") ? filterDictionary["statusText"] : "",
                ServerName = filterDictionary.ContainsKey("serverName") ? filterDictionary["serverName"] : "",
                FileName = filterDictionary.ContainsKey("fileName") ? filterDictionary["fileName"] : ""
            };
            return criteria;
        }

        /// <summary>
        /// Replaced by GetWithFilterCriteria method.
        /// </summary>
        /// <param name="statusText"></param>
        /// <returns></returns>
        [HttpGet, Route("api/tasksByStatus/{statusText}")]
        public IEnumerable<TaskTransfer> GetByStatus(string statusText)
        {
            return taskRepository.Get(t => t.Status == statusText
                                    && t.CreatedDate >= earliestCreated,
                                    o => o.OrderByDescending(ts => ts.CreatedDate))
                                    .Distinct()
                                    .Take(maxTasks);
        }

        /// <summary>
        /// Replaced by GetWithFilterCriteria method.
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [HttpGet, Route("api/tasksByServer/{serverName}")]
        public IEnumerable<TaskTransfer> GetByServer(string serverName)
        {
            return taskRepository.Get(t => t.Transporter == serverName, 
                                        o => o.OrderByDescending(ts =>ts.CreatedDate)
                                        .ThenBy(ts => ts.StatusId))
                                        .Distinct()
                                        .Take(maxTasks);
        }

        /// <summary>
        /// Replaced by GetWithFilterCriteria method.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet, Route("api/tasksByFileName/{fileName}")]
        public IEnumerable<TaskTransfer> GetByFileName(string fileName)
        {
            return taskRepository.Get(t => t.Filename.Contains(fileName), 
                                        o => o.OrderByDescending(ts => ts.CreatedDate))
                                        .Distinct()
                                        .Take(maxTasks);
        }

        [HttpGet, Route("api/statuses")]
        public IEnumerable<string> GetTaskStatuses()
        {
            return taskRepository.GetTaskStatusCodes();
        }


        [HttpGet, Route("api/transporters")]
        public IEnumerable<string> GetTransporters()
        {
            return taskRepository.GetTransporters();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                taskRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
