using System;
using Kurisu.AkiBT;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kurisu.AkiAI
{
    [Serializable]
    public class BehaviorTask : IAITask
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
        public bool Enabled { get; private set; }
        public void Init(IAIHost host)
        {
            instanceTree = Object.Instantiate(behaviorTree);
            foreach (var variable in instanceTree.SharedVariables)
                variable.MapTo(host.BlackBoard);
            instanceTree.Init(host.Object);
        }
        public void Stop()
        {
            Enabled = false;
        }
        public void Start()
        {
            Enabled = true;
        }
        public void Tick()
        {
            instanceTree.Update();
        }
    }
}