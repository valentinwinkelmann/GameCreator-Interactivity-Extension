using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

[Serializable]
public class InstructionDisableCharacterController : Instruction
{
    [SerializeField] private PropertyGetGameObject m_Character = new PropertyGetGameObject();
    protected override Task Run(Args args)
    {
        GameObject character = this.m_Character.Get(args);

        character.GetComponent<CharacterController>().enabled = false;
        return DefaultResult;
    }
}
