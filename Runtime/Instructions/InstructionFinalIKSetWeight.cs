using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
namespace vwgamedev.GameCreator{
    [Version(0, 0, 1)]
        
        [Title("Set Final IK Weight")]
        [Description("Set the weight of the Final IK")]

        [Category("Characters/FinalIK/Set Final IK Weight")]
        
        [Parameter("Character", "The character that will get the Final IK weight set")]
        [Parameter("IK Goal", "The IK goal that will get the weight set")]
        [Parameter("Weight", "The weight that will be set")]

        [Keywords("IK", "FinalIK", "Character", "Animation")]
        
        [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green, typeof(OverlayPlus))]
        
        [Serializable]

    public class InstructionFinalIKSetWeight : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private IKGoal m_IKGoal = IKGoal.LeftHand;

        [SerializeField] private PropertyGetDecimal m_Weight = new PropertyGetDecimal(1.0f);

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get(args).GetComponent<Character>();
            if (character == null) return Task.CompletedTask;
            float weight = (float)this.m_Weight.Get(args);

            InteractiveAnimator interactiveAnimator = character.Animim.Animator.transform.gameObject.GetComponent<InteractiveAnimator>();
            if (interactiveAnimator == null) return Task.CompletedTask;

            switch (this.m_IKGoal)
            {
                case IKGoal.LeftHand:
                    interactiveAnimator.LeftArmMultiplier = weight;
                    break;
                case IKGoal.RightHand:
                    interactiveAnimator.RightArmMultiplier = weight;
                    break;
                case IKGoal.LeftFoot:
                    interactiveAnimator.LeftLegMultiplier = weight;
                    break;
                case IKGoal.RightFoot:
                    interactiveAnimator.RightLegMultiplier = weight;
                    break;
                case IKGoal.Body:
                    interactiveAnimator.BodyMultiplier = weight;
                    break;
            }

            return DefaultResult;
        }
    }

}
