using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
namespace  vwgamedev.GameCreator{
[Version(0, 0, 1)]
    
    [Title("Interact with Interactive")]
    [Description("This instruction forces a character to interact with an interactive object")]

    [Category("Characters/Interaction/Interact with Interactive")]
    
    [Parameter("Character", "The character that will interact with the interactive object")]
    [Parameter("Interactive", "The interactive object that will be interacted with, must implement IInteractive")]

    [Keywords("Interact", "Interactive", "Character")]
    
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green, typeof(OverlayPlus))]
    
    [Serializable]
public class InstructionInteractWithInteractive : Instruction
{
        // MEMBERS: -------------------------------------------------------------------------------
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetGameObject m_Interactive = GetGameObjectTarget.Create();
        

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Character} Interacts with {this.m_Interactive}";
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get(args).GetComponent<Character>();
            if(character == null) return Task.CompletedTask;

            IInteractive interactive = this.m_Interactive.Get(args).GetComponent<IInteractive>();
            if(interactive == null) return Task.CompletedTask;

            interactive.Interact(character);
            return Task.CompletedTask;

        }
}
}