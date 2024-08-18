using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace vwgamedev.GameCreator
{
    [Serializable]
    public class InstructionCustomHandIK : Instruction
    {
        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        [SerializeField]
        private PropertyGetGameObject m_LeftHandTarget;

        [SerializeField]
        private PropertyGetGameObject m_RightHandTarget;

        protected override Task Run(Args args)
        {   
            
            return DefaultResult;
        }
    }
}
