using System;
using Kurisu.AkiBT;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kurisu.AkiAI
{
    public abstract class AITask
    {
        public TaskStatus Status { get; private set; }
        public void Stop()
        {
            Status = TaskStatus.Disabled;
        }
        public void Start()
        {
            Status = TaskStatus.Enabled;
        }
        public void Pause()
        {
            Status = TaskStatus.Pending;
        }
    }
    [Serializable]
    public class BehaviorTask : AITask, IAITask
    {
        [SerializeField, TaskID]
        private string taskID;
        public string TaskID => taskID;
        [SerializeField]
        private bool isPersistent;
        public bool IsPersistent => isPersistent;
        [SerializeField]
        private BehaviorTreeSO behaviorTree;
        private BehaviorTreeSO instanceTree;
        public BehaviorTreeSO InstanceTree => instanceTree;
        public void Init(IAIHost host)
        {
            instanceTree = Object.Instantiate(behaviorTree);
            foreach (var variable in instanceTree.SharedVariables)
                variable.MapTo(host.BlackBoard);
            instanceTree.Init(host.Object);
        }
        public void Tick()
        {
            instanceTree.Update();
        }
    }
}