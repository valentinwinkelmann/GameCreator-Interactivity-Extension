using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
namespace  vwgamedev.GameCreator{
[Version(0, 0, 1)]
    
    [Title("Stop Interaction")]
    [Description("This Instruction stops the interaction of a character and an InteractiveMonoBehaviour")]

    [Category("Characters/Interaction/Stop Interaction")]
    
    [Parameter("Character", "The character that whichs interaction will be stopped, if used within a InteractiveMonoBehaviour Instruction, keep it on self")]

    [Keywords("Interact", "Interactive", "Character")]
    
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Red)]
    
        [Serializable]
    public class InstructionStopInteraction : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectSelf.Create();
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get(args).GetComponent<Character>();
            if(character == null) return Task.CompletedTask;

            InteractiveAnimator interactive = character.Animim.Animator.transform.gameObject.GetComponent<InteractiveAnimator>();
            if(interactive == null) return Task.CompletedTask;
            //...
            return Task.CompletedTask;

        }
   
    }
}