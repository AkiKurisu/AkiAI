using UnityEngine;
using Kurisu.AkiBT;
using Kurisu.GOAP;
namespace Kurisu.AkiAI
{
    [AkiGroup("AkiGOAP")]
    [AkiInfo("Action: Set State of GOAP WorldState")]
    public class SetState : Action
    {
        [SerializeField]
        private SharedTObject<WorldState> worldState;
        [SerializeField]
        private SharedString stateName;
        [SerializeField]
        private SharedBool value;
        [SerializeField]
        private SharedBool isGlobal;
        public override void Awake()
        {
            InitVariable(worldState);
            InitVariable(stateName);
            InitVariable(value);
            InitVariable(isGlobal);
        }

        protected override Status OnUpdate()
        {
            worldState.Value.SetState(stateName.Value, value.Value, isGlobal.Value);
            return Status.Success;
        }
    }
}
