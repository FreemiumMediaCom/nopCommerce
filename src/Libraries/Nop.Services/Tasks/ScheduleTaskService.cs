using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Tasks;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class ScheduleTaskService : IScheduleTaskService
    {
        #region Fields

        private readonly IRepository<ScheduleTask> _taskRepository;

        #endregion

        #region Ctor

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public async virtual System.Threading.Tasks.Task DeleteTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.Delete(task);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        public async virtual Task<ScheduleTask> GetTaskById(int taskId)
        {
            if (taskId == 0)
                return null;

            return await _taskRepository.GetById(taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        public async virtual Task<ScheduleTask> GetTaskByType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = await query.FirstOrDefaultAsync();
            return task;
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Tasks</returns>
        public async virtual Task<IList<ScheduleTask>> GetAllTasks(bool showHidden = false)
        {
            var query = _taskRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }

            query = query.OrderByDescending(t => t.Seconds);

            var tasks = await query.ToListAsync();
            return tasks;
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public async virtual System.Threading.Tasks.Task InsertTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.Insert(task);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public async virtual System.Threading.Tasks.Task UpdateTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.Update(task);
        }

        #endregion
    }
}