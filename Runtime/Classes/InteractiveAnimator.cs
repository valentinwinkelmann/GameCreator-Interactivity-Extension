using UnityEngine;
using GameCreator.Runtime.Characters;
using System.Collections.Generic;

namespace vwgamedev.GameCreator
{
    public class InteractiveAnimator : MonoBehaviour
    {
        public bool RootMotionOverride = false;
        private Animator animator;
        public Character character;

        private List<Transform> characterIKPoints = new List<Transform>();

        // IK Multipliers (0f-1f) to allow to temporarily disable IK for a specific body part, e.g. when the character is aiming.
        [Range(0f, 1f)] public float LeftArmMultiplier = 1;
        [Range(0f, 1f)] public float RightArmMultiplier = 1;
        [Range(0f, 1f)] public float LeftLegMultiplier = 1;
        [Range(0f, 1f)] public float RightLegMultiplier = 1;
        [Range(0f, 1f)] public float BodyMultiplier = 1;

        // IK Weights which are controlled internally by animation events
        [SerializeField]private float leftArmWeight = 0;
        [SerializeField]private float rightArmWeight = 0;
        [SerializeField]private float leftLegWeight = 0;
        [SerializeField]private float rightLegWeight = 0;
        [SerializeField]private float bodyWeight = 0;

        // Target weights for smooth transitions
        private float targetLeftArmWeight = 0;
        private float targetRightArmWeight = 0;
        private float targetLeftLegWeight = 0;
        private float targetRightLegWeight = 0;
        private float targetBodyWeight = 0;

        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Transform leftFootTarget;
        public Transform rightFootTarget;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            character = this.transform.parent.parent.GetComponent<Character>();
        }

        private void Update()
        {
            if (character.GetComponent<CharacterController>().enabled == true)
            {
                return;
            }

            if (animator != null && RootMotionOverride)
            {
                character.transform.position += character.Animim.RootMotionDeltaPosition;
            }

            // Smoothly update the actual IK weights over time
            float lerpSpeed = Time.deltaTime / 0.05f;

            targetLeftArmWeight = Mathf.Lerp(targetLeftArmWeight, leftArmWeight * LeftArmMultiplier, lerpSpeed);
            targetRightArmWeight = Mathf.Lerp(targetRightArmWeight, rightArmWeight * RightArmMultiplier, lerpSpeed);
            targetLeftLegWeight = Mathf.Lerp(targetLeftLegWeight, leftLegWeight * LeftLegMultiplier, lerpSpeed);
            targetRightLegWeight = Mathf.Lerp(targetRightLegWeight, rightLegWeight * RightLegMultiplier, lerpSpeed);
            targetBodyWeight = Mathf.Lerp(targetBodyWeight, bodyWeight * BodyMultiplier, lerpSpeed);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null) return;

            // Update hand IK
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, targetLeftArmWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, targetLeftArmWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, targetRightArmWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, targetRightArmWeight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }

            // Update foot IK
            if (leftFootTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, targetLeftLegWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, targetLeftLegWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
            }

            if (rightFootTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, targetRightLegWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, targetRightLegWeight);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
            }
        }

        public void IK(string s)
        {
            Debug.Log("Recive Animator Event: " + s);
                var splitStrings = s.Split('_');
                string transformString = splitStrings[0];
                string ikGoalString = splitStrings[1];

                Transform targetTransform = null;
                var ikPoints = characterIKPoints.ToArray();
                if (ikPoints != null)
                {
                    for (int i = 0; i < ikPoints.Length; i++)
                    {
                        if (ikPoints[i].name == transformString)
                        {
                            targetTransform = ikPoints[i];
                            break;
                        }
                    }
                }

                if (targetTransform == null) return;
                Debug.Log("IK: " + transformString + " " + ikGoalString + " " + targetTransform.position);
                switch (ikGoalString)
                {
                    case "LeftHand":
                        leftHandTarget = targetTransform;
                        leftArmWeight = leftArmWeight == 1f ? 0f : 1f;
                        break;
                    case "RightHand":
                        rightHandTarget = targetTransform;
                        rightArmWeight = rightArmWeight == 1f ? 0f : 1f;
                        break;
                    case "LeftFoot":
                        leftFootTarget = targetTransform;
                        leftLegWeight = leftLegWeight == 1f ? 0f : 1f;
                        break;
                    case "RightFoot":
                        rightFootTarget = targetTransform;
                        rightLegWeight = rightLegWeight == 1f ? 0f : 1f;
                        break;
                    default:
                        return;
                }
        }

        // This method allows external scripts to set the weight directly, typically from animation events.
        public void SetIKWeight(string bodyPart, float weight)
        {
            switch (bodyPart)
            {
                case "LeftHand":
                    leftArmWeight = weight;
                    break;
                case "RightHand":
                    rightArmWeight = weight;
                    break;
                case "LeftFoot":
                    leftLegWeight = weight;
                    break;
                case "RightFoot":
                    rightLegWeight = weight;
                    break;
                case "Body":
                    bodyWeight = weight;
                    break;
            }
        }
        public void ResetIK()
        {
            leftHandTarget = null;
            rightHandTarget = null;
            leftFootTarget = null;
            rightFootTarget = null;
            leftArmWeight = 0;
            rightArmWeight = 0;
            leftLegWeight = 0;
            rightLegWeight = 0;
            bodyWeight = 0;
            LeftArmMultiplier = 1;
            RightArmMultiplier = 1;
            LeftLegMultiplier = 1;
            RightLegMultiplier = 1;
            BodyMultiplier = 1;
        }

        public void SetCharacterIKPoints(Transform[] ikPoints)
        {
            characterIKPoints.Clear();
            characterIKPoints.AddRange(ikPoints);
        }
        public void AddCharacterIKPoint(Transform ikPoint)
        {
            characterIKPoints.Add(ikPoint);
        }
        public void ClearCharacterIKPoints()
        {
            characterIKPoints.Clear();
        }

    }
        public enum IKGoal
        {
            LeftHand,
            RightHand,
            LeftFoot,
            RightFoot,
            Body
        }
}