using UnityEngine;
using GameCreator.Runtime.Characters;
using RootMotion.FinalIK;

namespace vwgamedev.GameCreator
{
    public class InteractiveAnimator : MonoBehaviour
    {
        public bool RootMotionOverride = false;
        private Animator animator;
        public Character character;

        public InteractiveMonoBehaviour interactiveMonoBehaviour;

        // IK Multipliers (0f-1f) to allow to temporarily disable IK for a specific body part, e.g. when the character is aiming.
        
        [Range(0f, 1f)]public float LeftArmMultiplier = 1;
        [Range(0f, 1f)]public float RightArmMultiplier = 1;
        [Range(0f, 1f)]public float LeftLegMultiplier = 1;
        [Range(0f, 1f)]public float RightLegMultiplier = 1;
        [Range(0f, 1f)]public float BodyMultiplier = 1;

        private FullBodyBipedIK ik;

        // IK Weights which are controlled internally by animation events
        private float leftArmWeight = 0;
        private float rightArmWeight = 0;
        private float leftLegWeight = 0;
        private float rightLegWeight = 0;
        private float bodyWeight = 0;

        // Target weights for smooth transitions
        private float targetLeftArmWeight = 0;
        private float targetRightArmWeight = 0;
        private float targetLeftLegWeight = 0;
        private float targetRightLegWeight = 0;
        private float targetBodyWeight = 0;

        public void DestroyHelper()
        {
            Destroy(this);
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            character = this.transform.parent.parent.GetComponent<Character>();
            ik = GetComponent<FullBodyBipedIK>();
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

            UpdateIKWeights();
        }

        private void UpdateIKWeights()
        {
            if (ik != null)
            {
                UpdateEffectorWeights(ik.solver.leftHandEffector, targetLeftArmWeight);
                UpdateEffectorWeights(ik.solver.rightHandEffector, targetRightArmWeight);
                UpdateEffectorWeights(ik.solver.leftFootEffector, targetLeftLegWeight);
                UpdateEffectorWeights(ik.solver.rightFootEffector, targetRightLegWeight);
                UpdateEffectorWeights(ik.solver.bodyEffector, targetBodyWeight);
            }
        }

        private void UpdateEffectorWeights(IKEffector effector, float weight)
        {
            if (effector.target != null)
            {
                effector.positionWeight = weight;
                effector.rotationWeight = weight;
            }
            else
            {
                effector.positionWeight = 0;
                effector.rotationWeight = 0;
            }
        }

        public void IK(string s)
        {
            if (interactiveMonoBehaviour != null && ik != null)
            {
                var splitStrings = s.Split('_');
                string transformString = splitStrings[0];
                string ikGoalString = splitStrings[1];

                Transform transform = null;
                var ikPoints = interactiveMonoBehaviour.GetCharacterIKPoints();
                if (ikPoints != null)
                {
                    for (int i = 0; i < ikPoints.Length; i++)
                    {
                        if (ikPoints[i].name == transformString)
                        {
                            transform = ikPoints[i];
                            break;
                        }
                    }
                }

                if (transform == null) return;

                switch (ikGoalString)
                {
                    case "LeftHand":
                        ik.solver.leftHandEffector.target = transform;
                        leftArmWeight = leftArmWeight == 1f ? 0f : 1f;
                        break;
                    case "RightHand":
                        ik.solver.rightHandEffector.target = transform;
                        rightArmWeight = rightArmWeight == 1f ? 0f : 1f;
                        break;
                    case "LeftFoot":
                        ik.solver.leftFootEffector.target = transform;
                        leftLegWeight = leftLegWeight == 1f ? 0f : 1f;
                        break;
                    case "RightFoot":
                        ik.solver.rightFootEffector.target = transform;
                        rightLegWeight = rightLegWeight == 1f ? 0f : 1f;
                        break;
                    default:
                        return;
                }
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