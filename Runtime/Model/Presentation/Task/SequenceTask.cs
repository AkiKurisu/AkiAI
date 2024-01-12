using System;
using System.Collections;
using System.Collections.Generic;
namespace Kurisu.AkiAI
{
    /// <summary>
    /// Sequence task running outside of agent
    /// </summary>
    public class SequenceTask : ITask, IEnumerable<ITask>
    {
        public event Action OnEnd;
        private readonly Queue<ITask> tasks = new();
        public TaskStatus Status { get; private set; }
        public SequenceTask()
        {
            OnEnd = null;
            Status = TaskStatus.Pending;
        }
        public SequenceTask(Action callBack)
        {
            OnEnd = callBack;
            Status = TaskStatus.Pending;
        }
        public SequenceTask(ITask firstTask, Action callBack)
        {
            OnEnd = callBack;
            tasks.Enqueue(firstTask);
            Status = TaskStatus.Enabled;
            TaskRunner.RegisterTask(this);
        }
        public SequenceTask(IReadOnlyList<ITask> sequence, Action callBack)
        {
            OnEnd = callBack;
            foreach (var task in sequence)
                tasks.Enqueue(task);
            Status = TaskStatus.Enabled;
            TaskRunner.RegisterTask(this);
        }
        /// <summary>
        /// Append a task to the end of sequence
        /// </summary>
        /// <param name="task"></param>
        public SequenceTask Append(ITask task)
        {
            tasks.Enqueue(task);
            if (Status == TaskStatus.Pending)
            {
                Status = TaskStatus.Enabled;
                TaskRunner.RegisterTask(this);
            }
            return this;
        }
        public void Tick()
        {
            if (tasks.TryPeek(out ITask first))
            {
                first.Tick();
                if (first.Status == TaskStatus.Disabled)
                {
                    tasks.Dequeue();
                    if (tasks.Count == 0)
                    {
                        Status = TaskStatus.Disabled;
                        OnEnd?.Invoke();
                        OnEnd = null;
                    }
                    else
                    {
                        Tick();
                    }
                }
            }
            else
            {
                Status = TaskStatus.Disabled;
                OnEnd?.Invoke();
                OnEnd = null;
            }
        }
        public void Abort()
        {
            Status = TaskStatus.Disabled;
            OnEnd = null;
        }
        public IEnumerator<ITask> GetEnumerator()
        {
            return tasks.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return tasks.GetEnumerator();
        }
    }
}
