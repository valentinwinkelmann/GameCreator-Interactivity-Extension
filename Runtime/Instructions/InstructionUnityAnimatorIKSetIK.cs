using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
namespace vwgamedev.GameCreator{
    [Version(0, 0, 1)]
        
        [Title("Set Animator IK")]
        [Description("Set the Unity Animator IK weight and Target for a specific body part. If IK Target is not set, the weight will be set to its current IK Target")]

        [Category("Characters/IK/Set Unity Animator IK")]
        
        [Parameter("Character", "The character that will get the IK set")]
        [Parameter("IK Goal", "The IK goal that will get set")]
        [Parameter("IK Target", "The target Transform that will be set")]
        [Parameter("Weight", "The weight that will be set")]

        [Keywords("IK", "FinalIK", "Character", "Animation")]
        
        [Image(typeof(IconIK), ColorTheme.Type.Green, typeof(OverlayPlus))]
        
        [Serializable]

    public class InstructionUnityAnimatorIKSetIK : Instruction
    {
        public override string Title => $"Set IK of {this.m_Character} to {this.m_IKTarget} with weight {this.m_Weight}";
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private IKGoal m_IKGoal = IKGoal.LeftHand;
        [SerializeField] private PropertyGetGameObject m_IKTarget = new PropertyGetGameObject();
        [SerializeField] private PropertyGetDecimal m_Weight = new PropertyGetDecimal(1.0f);
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get(args).GetComponent<Character>();
            if (character == null) return Task.CompletedTask;

            Transform target = this.m_IKTarget.Get(args).transform;
            float weight = (float)this.m_Weight.Get(args);
            
            UnityAnimatorIKRig rig = character.IK.GetRig<UnityAnimatorIKRig>();
            if (rig == null) return Task.CompletedTask;

            
            if(target != null){
                rig.SetIKGoal(m_IKGoal, target, weight);
            } else {
                rig.SetIKGoal(m_IKGoal, weight);
            }

            return DefaultResult;
        }
    }
}