using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Kurisu.GOAP;
namespace Kurisu.AkiAI
{
    [RequireComponent(typeof(AIBlackBoard))]
    [RequireComponent(typeof(GOAPPlanner))]
    public abstract class AIAgent : MonoBehaviour, IAIHost
    {
        [SerializeField]
        private GOAPSet dataSet;
        protected GOAPSet DataSet => dataSet;
        private GOAPPlanner planner;
        protected GOAPPlanner Planner => planner;
        private readonly Dictionary<string, IAITask> taskMap = new();
        protected Dictionary<string, IAITask> TaskMap => taskMap;
        public bool IsAIEnabled { get; private set; }
        public virtual Transform Transform => transform;
        public virtual GameObject Object => gameObject;
        private AIBlackBoard blackBoard;
        public IAIBlackBoard BlackBoard => blackBoard;
        public WorldState WorldState { get; private set; }
        public abstract IAIContext Context { get; }
        protected void Awake()
        {
            WorldState = GetComponent<WorldState>();
            blackBoard = GetComponent<AIBlackBoard>();
            planner = GetComponent<GOAPPlanner>();
            OnAwake();
        }
        protected virtual void OnAwake() { }
        protected virtual void Start()
        {
            SetupGOAP();
            OnStart();
        }
        protected virtual void OnStart() { }
        protected abstract void SetupGOAP();
        protected void Update()
        {
            if (!IsAIEnabled) return;
            TickTasks();
            OnUpdate();
        }

        private void TickTasks()
        {
            foreach (var task in taskMap.Values)
            {
                if (task.Status == TaskStatus.Enabled) task.Tick();
            }
        }
        protected virtual void OnUpdate() { }
        public virtual void EnableAI()
        {
            planner.enabled = true;
            IsAIEnabled = true;
            foreach (var task in taskMap.Values)
            {
                if (task.IsPersistent || task.Status == TaskStatus.Pending)
                    task.Start();
            }
        }
        public virtual void DisableAI()
        {
            planner.enabled = false;
            IsAIEnabled = false;
            foreach (var task in taskMap.Values)
            {
                //Pend running tasks
                if (task.Status == TaskStatus.Enabled)
                    task.Pause();
            }
        }
        protected void OnEnable()
        {
            EnableAI();
        }
        protected void OnDisable()
        {
            DisableAI();
        }
        public IAITask GetTask(string taskID)
        {
            return taskMap[taskID];
        }
        public void AddTask(IAITask task)
        {
            if (!task.IsPersistent && taskMap.ContainsKey(task.TaskID))
            {
                Debug.LogWarning($"Already contained task with same id: {task.TaskID}");
                return;
            }
            task.Init(this);
            taskMap.Add(task.TaskID, task);
            if (task.IsPersistent && IsAIEnabled)
            {
                task.Start();
            }
        }
        public IEnumerable<IAITask> GetAllTasks()
        {
            return taskMap.Values;
        }
    }
    /// <summary>
    /// AI Agent for custom context (eg. Data model, gameplay components)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AIAgent<T> : AIAgent, IAIHost<T> where T : IAIContext
    {
        public sealed override IAIContext Context => TContext;
        public abstract T TContext { get; }
        protected override void SetupGOAP()
        {
            var goals = DataSet.GetGoals();
            foreach (var goal in goals.OfType<AIGoal<T>>())
            {
                goal.Setup(this);
            }
            var actions = DataSet.GetActions();
            foreach (var action in actions.OfType<AIAction<T>>())
            {
                action.Setup(this);
            }
            Planner.InjectGoals(goals);
            Planner.InjectActions(actions);
        }
    }
    /// <summary>
    /// AI Agent embedding Behavior Tasks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BehaviorBasedAIAgent<T> : AIAgent<T> where T : IAIContext
    {
        [SerializeField]
        private BehaviorTask[] behaviorTasks;
        protected sealed override void Start()
        {
            SetupBehaviorTree();
            SetupGOAP();
            OnStart();
        }
        private void SetupBehaviorTree()
        {
            foreach (var task in behaviorTasks)
            {
                AddTask(task);
            }
        }
    }
}

