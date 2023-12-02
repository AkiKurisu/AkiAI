using System.Collections.Generic;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.AkiAI
{
    public class AIBlackBoard : MonoBehaviour, IAIBlackBoard
    {
        [SerializeReference]
        private List<SharedVariable> sharedVariables = new();
        public List<SharedVariable> SharedVariables => sharedVariables;
    }
}