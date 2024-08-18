using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
namespace vwgamedev.GameCreator{
    [Version(0, 0, 1)]
        
        [Title("Unset Animator IK")]
        [Description("Unset the Unity Animator IK for a specific body part.")]

        [Category("Characters/IK/Unset Unity Animator IK")]
        
        [Parameter("Character", "The character that will get the IK unset")]
        [Parameter("IK Goal", "The IK goal that will get unset")]


        [Keywords("IK", "FinalIK", "Character", "Animation")]
        
        [Image(typeof(IconIK), ColorTheme.Type.Red, typeof(OverlayMinus))]
        
        [Serializable]

    public class InstructionUnityAnimatorIKUnsetIK : Instruction
    {
        public override string Title => $"Unsets IK of {this.m_Character}";
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private IKGoal m_IKGoal = IKGoal.LeftHand;
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get(args).GetComponent<Character>();
            if (character == null) return Task.CompletedTask;
            
            UnityAnimatorIKRig rig = character.IK.GetRig<UnityAnimatorIKRig>();
            if (rig == null) return Task.CompletedTask;

            rig.UnsetIKGoal(m_IKGoal);
            
            return DefaultResult;
        }
    }
}