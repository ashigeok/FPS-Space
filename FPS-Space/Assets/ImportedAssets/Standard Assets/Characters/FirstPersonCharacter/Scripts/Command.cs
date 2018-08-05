using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    //The parent class
    public abstract class Command
    {

        //Do a command
        public abstract void Execute();

        //Undo a command
        public virtual void Undo() { }

    }


    //
    // Child classes
    //

    public class MoveForward : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_vertical += 1f;
        }
    }


    public class MoveReverse : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_vertical -= 1f;
        }
    }


    public class MoveLeft : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_horizontal -= 1f;
        }
    }


    public class MoveRight : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_horizontal += 1f;
        }
    }

    //For keys with no binding
    public class NoCommand : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            //Nothing will happen if we press this key
        }
    }
    
    
    public class SetIsCrouching : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_isCrouching = true;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_isCrouching = false;
        }
    }
        
    public class SetIsWalking : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_isWalking = false;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_isWalking = true;
        }
    }
    
    public class SetJump : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_jump = true;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_jump = false;
        }
    }
}