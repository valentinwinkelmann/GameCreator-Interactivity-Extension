using System;
using System.Reflection;
using UnityEngine;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

/// Todo:
/// - Remove all unnecessary code.
/// - Check if every mode works, also without state, without mount and without marker.
/// - Check if is working in AOT and IL2CPP platforms ( WebGL ).
/// - Maybe rename the whole thing
/// - Make FinalIK Optional by player compile symbols
/// - Make it a Package structure and publish on Github
/// - Making a refflection less version
namespace vwgamedev.GameCreator{
    /// <summary>
    /// This Behaviour is meant to be inherited from any component that should be interactable by a character.
    /// It works like the MonoBehaviour, but with the additional functionality of being interactable.
    /// 
    /// Limitations:
    /// RootMotion will only work if the object where the character will be mounted on is static or kinematic.
    /// While in RootMotion, the charachter can't be stay fixed to the objects position/rotation.
    /// Best practice is to make vehicle static when starting the interaction and dynamic when the interaction is finished.
    /// For Example OnBeforeInteract Set to Kinematic, and OnStop Set to Dynamic.
    /// 
    /// Following methods can be implemented to react to interactions:
    /// - OnInteract(Character character)
    /// - OnStop(Character character)
    /// - OnFail(CancelReason reason)
    /// - OnBeforeInteract(Character character) // starts when the character starts entering the State, usefull for animation or kinematic changes
    /// - OnBeforeStop(Character character) // starts when the character starts leaving the State usefull for animations
    /// </summary>
    public class InteractiveMonoBehaviour : MonoBehaviour, IInteractive, ISpatialHash
    {
        [field: NonSerialized] public Args Args { get; private set; }
        [SerializeField] private PropertyGetBool m_CharacterBusy = new PropertyGetBool(false);
        [SerializeField] private PropertyGetBool m_CharacterControllable = new PropertyGetBool(false);
        [SerializeField] private PropertyGetBool m_CharacterMount = new PropertyGetBool(false);
        [SerializeField] private PropertyGetLocation m_CharacterLocation = GetLocationNavigationMarker.Create;
        [SerializeField] private PropertyGetGameObject m_MountObject = new PropertyGetGameObject();
        [SerializeField] private State m_CharacterState = null;
        [SerializeField] private Transform[] m_CharacterIKPoints = null;

        [SerializeField] private RunInstructionsList m_OnBeforeInteract = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnInteract = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnBeforeStop = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnStop = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnFail = new RunInstructionsList();


        protected virtual bool CharacterBusy => m_CharacterBusy.Get(this.Args);
        protected virtual bool CharacterControllable => m_CharacterControllable.Get(this.Args);
        protected virtual bool CharacterMount => m_CharacterMount.Get(this.Args);
        protected virtual Location CharacterLocation => m_CharacterLocation.Get(this.Args);
        protected virtual GameObject MountObject => m_MountObject.Get(this.Args);
        protected virtual State CharacterState => m_CharacterState;
        protected virtual Transform[] CharacterIKPoints => m_CharacterIKPoints;
        
        [NonSerialized] private Vector3 m_LastPosition;
        [NonSerialized] private int m_InstanceID;
        [NonSerialized] private bool m_IsInteracting;
        [NonSerialized] private Character m_Character;
        [NonSerialized] private CharacterMotionValues m_CharacterMotionValues;

        [NonSerialized] private bool freezeCharacterControler = false;

        private MethodInfo onInteractMethod;
        private MethodInfo onStopMethod;
        private MethodInfo onFailMethod;
        private MethodInfo onBeforeInteractMethod;
        private MethodInfo onBeforeStopMethod;

        private void Awake()
        {
            this.Args = new Args(this);
            this.m_InstanceID = this.gameObject.GetInstanceID();
        }

        private void OnEnable()
        {
            this.m_LastPosition = this.transform.position;
            SpatialHashInteractions.Insert(this);
            
        }

        private void OnDisable()
        {
            SpatialHashInteractions.Remove(this);
        }
        private void Update(){
            if(this.m_Character != null && this.freezeCharacterControler){
                this.m_Character.Motion.LinearSpeed = 0;
                this.m_Character.Motion.AngularSpeed = 0;
                this.m_Character.Motion.TerminalVelocity = 0;
            }
            if(this.m_Character != null){
                this.m_Character.Animim.Animator.SetFloat("Grounded", 1f); // Every player should be grounded while interacting no matter what the physics say
            }
            
        }

        GameObject IInteractive.Instance => this.gameObject;
        int IInteractive.InstanceID => this.m_InstanceID;
        bool IInteractive.IsInteracting => this.m_IsInteracting;

        async void IInteractive.Interact(Character character)
        {
            if(this.m_Character == character || character.Busy.IsBusy) return;
            if(this.m_IsInteracting || this.m_Character != null){
                InteractionCancel(CancelReason.AlreadyInUse);
                return;
            }

            this.m_IsInteracting = true;
            this.m_Character = character;

            if(!await InteractiveUtility.WaitForNavigation(character, this.CharacterLocation)){ // If the character can't reach the marker, cancel the interaction
                InteractionCancel(CancelReason.NotReachable);
                return;
            }
            if(character.Animim.Animator.transform.gameObject.GetComponent<InteractiveAnimator>() != null){
                character.Animim.Animator.transform.gameObject.GetComponent<InteractiveAnimator>().DestroyHelper();
            }
            InteractiveAnimator _interactiveAnimator = character.Animim.Animator.transform.gameObject.AddComponent<InteractiveAnimator>();
            _interactiveAnimator.interactiveMonoBehaviour = this;

            // if (this.CharacterBusy) character.Busy.SetBusy();
            if (this.CharacterBusy) character.Busy.AddState(Busy.Limb.Legs);
            if (this.CharacterControllable) character.Player.IsControllable = false;

            if(this.CharacterMount){

                await InteractiveUtility.WaitForMount(character, this.MountObject,0.5f, ref m_CharacterMotionValues);
            }
            InteractiveReflectionUtility.InvokeMethod(this, ref onBeforeInteractMethod, "OnBeforeInteract", this.m_Character);
            await this.m_OnBeforeInteract.Run(new Args(character.gameObject));

            if(this.CharacterState != null){
                _interactiveAnimator.RootMotionOverride = true;
                await InteractiveUtility.WaitForEnterState(character, this.CharacterState);
            }


            InteractiveReflectionUtility.InvokeMethod(this, ref onInteractMethod, "OnInteract", this.m_Character);
            await this.m_OnInteract.Run(new Args(character.gameObject));
        }

        async void IInteractive.Stop()
        {
            if (!this.m_IsInteracting) return;
            this.freezeCharacterControler = true;
            InteractiveReflectionUtility.InvokeMethod(this, ref onBeforeStopMethod, "OnBeforeStop", this.m_Character);
            await this.m_OnBeforeStop.Run(new Args(this.m_Character.gameObject));
            InteractiveAnimator _interactiveAnimator = this.m_Character.Animim.Animator.transform.gameObject.GetComponent<InteractiveAnimator>();
            if(this.CharacterState != null){
                await InteractiveUtility.WaitForExitState(this.m_Character, this.CharacterState);
                _interactiveAnimator.RootMotionOverride = false;
            }
            this.freezeCharacterControler = false;
            if(this.CharacterMount){
                await InteractiveUtility.WaitForUnmount(this.m_Character, 0.25f, m_CharacterMotionValues);
            }
            


            InteractiveReflectionUtility.InvokeMethod(this, ref onStopMethod, "OnStop", this.m_Character);
            await this.m_OnStop.Run(new Args(this.m_Character.gameObject));

            _interactiveAnimator.DestroyHelper();
            
            if(this.CharacterBusy) this.m_Character.Busy.SetAvailable();
            if(this.CharacterControllable) this.m_Character.Player.IsControllable = true;

            await InteractiveUtility.WaitForFrames(300);
            this.m_IsInteracting = false;
            this.m_Character = null;
        }

        public void InteractionStop()
        {
            if (!this.m_IsInteracting) return;
            (this as IInteractive).Stop();
        }

        private void InteractionCancel(CancelReason reason)
        {
            if (!this.m_IsInteracting) return;

            this.m_IsInteracting = false;
            this.m_Character = null;
            InteractiveReflectionUtility.InvokeMethod(this, ref onFailMethod, "OnFail", reason);
            _ = m_OnFail.Run(new Args(this.gameObject));
        }

        public Transform[] GetCharacterIKPoints()
        {
            return this.CharacterIKPoints;
        }

    }
}