using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters.IK;

namespace vwgamedev.GameCreator
{
    [Title("Unity Animator IK Rig")]
    [Category("Unity Animator IK Rig")]

    [Image(typeof(IconIK), ColorTheme.Type.Green)]
    [Description("Custom IK rig using Unity's OnAnimatorIK method")]

    [Serializable]
    public class UnityAnimatorIKRig : TRigAnimatorIK
    {
        private InteractiveAnimator interactiveAnimator;

        public override string Title => "Unity Animator IK Rig";
        public override string Name => "UnityAnimatorIKRig";
        public override bool RequiresHuman => true;
        public override bool DisableOnBusy => true;

        protected override void DoStartup(Character character)
        {
            var animator = character.Animim.Animator;

            // Add the InteractiveAnimator component at runtime if it doesn't already exist
            interactiveAnimator = animator.gameObject.GetComponent<InteractiveAnimator>();
            if (interactiveAnimator == null)
            {
                interactiveAnimator = animator.gameObject.AddComponent<InteractiveAnimator>();
            }
        }

        protected override void DoEnable(Character character)
        {
            // Optionally handle enable logic
        }

        protected override void DoDisable(Character character)
        {
            // Optionally handle disable logic
        }

        protected override void DoUpdate(Character character)
        {
            // Optionally update logic
        }

        // Public methods to set the IK targets and control root motion override
        /// <summary>
        /// Sets the IK target and weight for a specific body part
        /// </summary>
        /// <param name="goal">The body part to set the IK target for</param>
        /// <param name="target">The target transform to set</param>
        /// <param name="weight">The weight to set</param>
        /// <param name="force">If true, also the multiplier will be set</param>
        public void SetIKGoal(IKGoal goal, Transform target, float weight, bool force = false)
        {
            switch (goal)
            {
                case IKGoal.LeftHand:
                    if(force)interactiveAnimator.LeftArmMultiplier = weight;
                    interactiveAnimator.SetIKWeight("LeftHand", weight);
                    interactiveAnimator.leftHandTarget = target;
                    break;
                case IKGoal.RightHand:
                    if(force)interactiveAnimator.RightArmMultiplier = weight;
                    interactiveAnimator.SetIKWeight("RightHand", weight);
                    interactiveAnimator.rightHandTarget = target;
                    break;
                case IKGoal.LeftFoot:
                    if(force)interactiveAnimator.LeftLegMultiplier = weight;
                    interactiveAnimator.SetIKWeight("LeftFoot", weight);
                    interactiveAnimator.leftFootTarget = target;
                    break;
                case IKGoal.RightFoot:
                    interactiveAnimator.RightLegMultiplier = weight;
                    interactiveAnimator.SetIKWeight("RightFoot", weight);
                    interactiveAnimator.rightFootTarget = target;
                    break;
                case IKGoal.Body:
                    interactiveAnimator.BodyMultiplier = weight;
                    interactiveAnimator.SetIKWeight("Body", weight);
                    break;
            }
        }
        /// <summary>
        /// Sets the IK weight for a specific body part
        /// </summary>
        /// <param name="goal">The body part to set the IK weight for</param>
        /// <param name="weight">The weight to set</param>
        /// <param name="force">If true, also the multiplier will be set</param>
        public void SetIKGoal(IKGoal goal, float weight, bool force = false){
            switch (goal)
            {
                case IKGoal.LeftHand:
                    if(force)interactiveAnimator.LeftArmMultiplier = weight;
                    interactiveAnimator.SetIKWeight("LeftHand", weight);
                    break;
                case IKGoal.RightHand:
                    if(force)interactiveAnimator.RightArmMultiplier = weight;
                    interactiveAnimator.SetIKWeight("RightHand", weight);
                    break;
                case IKGoal.LeftFoot:
                    if(force)interactiveAnimator.LeftLegMultiplier = weight;
                    interactiveAnimator.SetIKWeight("LeftFoot", weight);
                    break;
                case IKGoal.RightFoot:
                    if(force)interactiveAnimator.RightLegMultiplier = weight;
                    interactiveAnimator.SetIKWeight("RightFoot", weight);
                    break;
                case IKGoal.Body:
                    if(force)interactiveAnimator.BodyMultiplier = weight;
                    interactiveAnimator.SetIKWeight("Body", weight);
                    break;
            }
        }
        /// <summary>
        /// Sets the IK multiplier for a specific body part for overriding the weight
        /// </summary>
        /// <param name="goal">The body part to set the IK multiplier for</param>
        /// <param name="weight">The weight to set</param>
        public void SetIKMultiplier(IKGoal goal, float weight){
            switch (goal)
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
        }

        public void UnsetIKGoal(IKGoal goal){
            switch (goal)
            {
                case IKGoal.LeftHand:
                    interactiveAnimator.SetIKWeight("LeftHand", 0);
                    interactiveAnimator.leftHandTarget = null;
                    break;
                case IKGoal.RightHand:
                    interactiveAnimator.SetIKWeight("RightHand", 0);
                    interactiveAnimator.rightHandTarget = null;
                    break;
                case IKGoal.LeftFoot:
                    interactiveAnimator.SetIKWeight("LeftFoot", 0);
                    interactiveAnimator.leftFootTarget = null;
                    break;
                case IKGoal.RightFoot:
                    interactiveAnimator.SetIKWeight("RightFoot", 0);
                    interactiveAnimator.rightFootTarget = null;
                    break;
                case IKGoal.Body:
                    interactiveAnimator.SetIKWeight("Body", 0);
                    break;
            }
        }
        public void ResetIK(){
            interactiveAnimator.ResetIK();
        }

        /// <summary>
        /// Sets the RootMotionOverride mode which will make the character animate by the InteractiveAnimator,
        /// Usefull when the character is mounted.
        /// </summary>
        /// <param name="enable"></param>
        public void SetRootMotionOverride(bool enable)
        {
            if (interactiveAnimator != null)
            {
                interactiveAnimator.RootMotionOverride = enable;
            }
        }
        public InteractiveAnimator instance => interactiveAnimator;
    }
}
