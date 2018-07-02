﻿using System.Collections;
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
            FirstPersonController.vertical += 1f;
        }
    }


    public class MoveReverse : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.vertical -= 1f;
        }
    }


    public class MoveLeft : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.horizontal -= 1f;
        }
    }


    public class MoveRight : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.horizontal += 1f;
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
            FirstPersonController.m_IsCrouching = true;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_IsCrouching = false;
        }
    }
        
    public class SetIsWalking : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_IsWalking = false;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_IsWalking = true;
        }
    }
    
    public class SetJump : Command
    {
        //Called when we press a key
        public override void Execute()
        {
            FirstPersonController.m_Jump = true;
        }
        
        //Undo a command
        public override void Undo()
        {
            FirstPersonController.m_Jump = false;
        }
    }
}