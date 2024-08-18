using UnityEngine;

namespace vwgamedev.GameCreator
{
    public class UnityAnimatorIK : MonoBehaviour
    {
        public Transform LeftHandTarget;
        public Transform RightHandTarget;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null) return;

            if (LeftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            }

            if (RightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

                animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
            }
        }
    }
}
