using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace vwgamedev.GameCreator{
    /// <summary>
/// Those class provides some single threaded async awaitables.
/// It will allow you to wait for various things and therefore make it easier to chain up complex tasks.
/// 
/// Note: The Current most compatible way is to disable the CharacterController of the character while mounting and unmounting.
/// This will prevent the charachter from moving on its own while we move the parent and it also prevents the character from going into airborn state.
/// It will eliminate the possibility of RootMotion which is a big downsid.
/// </summary>
public static class InteractiveUtility
{
    public static async Task WaitForInput(KeyCode keyCode)
        {
            while (true)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    break;
                }
                await Task.Yield();
            }
        }
        public static async Task WaitForInputAction(InputAction action)
        {
            while (true)
            {
                if (action.triggered)
                {
                    break;
                }
                await Task.Yield();
            }
        }
        /// <summary>
        /// Plays a tween and awaits its completion.
        /// </summary>
        /// <param name="tweenTarget">A Tween is running in a Coroutine and needs a GameObject to run on</param>
        /// <param name="tween">The Tween that should be played</param>
        /// <returns></returns>
        public static async Task WaitForTween(GameObject tweenTarget, ITweenInput tween)
        {
            Tween.To(tweenTarget, tween);
            while (true)
            {
                if (tween.IsComplete)
                {
                    break;
                }
                await Task.Yield();
            }
        }
        public static async Task WaitForSeconds(float seconds)
        {
            float time = 0;
            while (time < seconds)
            {
                time += Time.deltaTime;
                await Task.Yield();
            }
        }
        public static async Task WaitForFrames(int frames)
        {
            int frame = 0;
            while (frame < frames)
            {
                frame++;
                await Task.Yield();
            }
        }
        public static async Task<bool> WaitForNavigation(Character character, Location location, float stopDistance = 0.1f)
        {
            var tcs = new TaskCompletionSource<bool>();

            bool isCallbackCalled = false;

            character.Motion.MoveToLocation(
                location,
                stopDistance,
                (Character c, bool success) => {
                    if (!isCallbackCalled)
                    {
                        isCallbackCalled = true;
                        tcs.SetResult(success);
                    }
                }
            );

            return await tcs.Task;
        }





        
        public static async Task WaitForEnterState(Character character, State state)
            {

                await WaitForFrames(10);

                _ = character.States.SetState(state, 10, BlendMode.Blend, new ConfigState(0, 1, 1, 0.15f, 0.15f)); 

                float entryDuration = state.EntryClip.length;
                await Task.Delay((int)(entryDuration * 1000));
                

            }
        public static async Task WaitForExitState(Character character, State state)
        {

            character.States.Stop(10, 0, 0.15f);
            float exitDuration = state.ExitClip.length;
            await Task.Delay((int)(exitDuration * 1000));

        }


        /// <summary>
        /// Mounts the character to the marker and tweens from its current position to the markers position ( Player get local pos/rot zeroed )
        /// After this we are safe to play any state or animation without being worried about the character being in the wrong position.
        /// Tweening also makes this process look smooth.
        /// </summary>
        /// <param name="character">The character that should be mounted</param>
        /// <param name="marker">The marker the character should be mounted to</param>
        /// <param name="duration">The time the tween should take</param>
        /// <param name="motionValues">The motion values of the character before mounting, we need to save them in a ref to restore them at unmount</param>
        /// <returns></returns>
        public static Task WaitForMount(Character character, GameObject mountObject, float duration, ref CharacterMotionValues motionValues){
            motionValues.LinearSpeed = character.Motion.LinearSpeed;
            motionValues.AngularSpeed = character.Motion.AngularSpeed;
            motionValues.TerminalVelocity = character.Motion.TerminalVelocity;
            character.Motion.LinearSpeed = 0;
            character.Motion.AngularSpeed = 0;
            character.Motion.TerminalVelocity = 0;
            character.GetComponent<CharacterController>().detectCollisions = false;
            character.GetComponent<CharacterController>().enabled = false;
            
            
            character.transform.parent = mountObject.transform;
            TweenTransform currentTransform = new TweenTransform(character.transform);
            TweenTransform targetTransform = new TweenTransform(Vector3.zero - character.Animim.Mannequin.transform.localPosition, Quaternion.identity);
            ITweenInput tweenInput = new TweenInput<TweenTransform>(
                currentTransform,
                targetTransform,
                duration,
                (a, b, t) =>
                {
                    a.position = Vector3.Lerp(currentTransform.position, targetTransform.position, t);
                    a.rotation = Quaternion.Lerp(currentTransform.rotation, targetTransform.rotation, t);
                    character.transform.localPosition = a.position;
                    character.transform.localRotation = a.rotation;
                },
                Tween.GetHash(typeof(Transform), "transform"),
                Easing.Type.QuadInOut
            );
            return WaitForTween(character.gameObject, tweenInput);
            //return Task.CompletedTask;
            
        }
        /// <summary>
        /// The Unmount mehtod will unparent the character from the marker and restore its motion values. It will also tween the charachter back in a 0,*,0 rotation, as the charachter might have been rotated in the meantime, e.g. inside a car.
        /// </summary>
        /// <param name="character">The character that should be unmounted</param>
        /// <param name="duration">The time the tween should take</param>
        /// <param name="motionValues">The previous motion values of the character to restore</param>
        /// <returns></returns>
        public static async Task WaitForUnmount(Character character, float duration, CharacterMotionValues motionValues){
            character.transform.parent = null;
            TweenTransform currentTransform = new TweenTransform(character.transform);
            TweenTransform targetTransform = new TweenTransform(character.transform);
            targetTransform.rotation.x = 0;
            targetTransform.rotation.z = 0;
            ITweenInput tweenInput = new TweenInput<TweenTransform>(
                currentTransform,
                targetTransform,
                duration,
                (a, b, t) =>
                {
                    a.position = Vector3.Lerp(currentTransform.position, targetTransform.position, t);
                    a.rotation = Quaternion.Lerp(currentTransform.rotation, targetTransform.rotation, t);
                    character.transform.localPosition = a.position;
                    character.transform.localRotation = a.rotation;
                },
                Tween.GetHash(typeof(Transform), "transform"),
                Easing.Type.QuadInOut
            );
            await WaitForTween(character.gameObject, tweenInput);
            character.Motion.LinearSpeed = motionValues.LinearSpeed;
            character.Motion.AngularSpeed = motionValues.AngularSpeed;
            character.Motion.TerminalVelocity = motionValues.TerminalVelocity;
            character.GetComponent<CharacterController>().detectCollisions = true;
            character.GetComponent<CharacterController>().enabled = true;
        }

        
        public static InteractiveStateMarker GetNearestStateMarker(InteractiveStateMarker[] characterStateMarkers, Character character, LayerMask layerMask)
        {
            if (characterStateMarkers.Length == 1)
            {
                // If there's only one marker, check conditions and reachability
                if (IsValidMarker(characterStateMarkers[0], character))
                {
                    return characterStateMarkers[0];
                }
                return null; // Return null if the single marker doesn't pass the checks
            }

            InteractiveStateMarker nearestMarker = null;
            float nearestDistance = float.MaxValue;

            foreach (InteractiveStateMarker marker in characterStateMarkers)
            {
                // Check if the marker conditions are met before evaluating distance
                if (!IsValidMarker(marker, character))
                {
                    continue;
                }

                // Calculate the distance between the marker and the character
                float distance = Vector3.Distance(marker.marker.transform.position, character.transform.position);

                // Check if this marker is closer than the previous nearest one
                if (distance < nearestDistance && IsMarkerReachable(marker, character, layerMask))
                {
                    nearestDistance = distance;
                    nearestMarker = marker;
                }
            }

            return nearestMarker;
            }

            private static bool IsValidMarker(InteractiveStateMarker marker, Character character)
            {
                // Check the conditions directly
                bool conditionsMet = marker.runConditionsList.Check(new Args(character));

                // If the conditions are not met, ignore this marker
                return conditionsMet;
            }

            private static bool IsMarkerReachable(InteractiveStateMarker marker, Character character, LayerMask layerMask)
            {
                // Calculate the position to check, applying the offset to the y-axis
                Vector3 positionToCheck = marker.marker.transform.position + Vector3.up;

                // Perform the overlap sphere check
                Collider[] hitColliders = Physics.OverlapSphere(positionToCheck, 0.5f, layerMask);

                // If any collider is found, the marker is blocked
                return hitColliders.Length == 0;
            }




    }
    public enum CancelReason{
        NotReachable,
        AlreadyInUse
    }
    public struct TweenTransform
    {
        public Vector3 position;
        public Quaternion rotation;

        public TweenTransform(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        public TweenTransform(Transform transform)
        {
            this.position = transform.localPosition;
            this.rotation = transform.localRotation;
        }
    }
    public struct CharacterMotionValues
    {
        public float LinearSpeed;
        public float AngularSpeed;
        public float TerminalVelocity;
    }
}
