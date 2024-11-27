using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;
using Combat.Status;
using Controllers.Characters;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Controllers.Animation
{

    /// <summary>
    /// Alters the height of the collider while near cover and not moving, and plays a different movement anim.
    /// </summary>
    public class CoverCrouch : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs

        CharacterManager character { get { return mActionController.mCharacterManager; } }
        CapsuleCollider col;
        float defaultColliderHeight;
        float defaultColliderCenterY;

        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {

            //Get the collider, cache its current dimensions, and make it smaller.
            col = character.GetComponent<CapsuleCollider>();

            defaultColliderHeight = col.height;
            defaultColliderCenterY = col.center.y;

            col.height = col.height / 2;
            col.center = new Vector3(0f, defaultColliderCenterY / 2, 0f);
            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            //Used cached dimensions to return collider to normal on exit.
            col.height = defaultColliderHeight;
            col.center = new Vector3(0,defaultColliderCenterY,0);
            base.Deactivate();
        }

        public override void Deactivate(IAction queuedMotion)
        {
            character.GetComponent<CapsuleCollider>().height = defaultColliderHeight;
            base.Deactivate(queuedMotion);
        }

        /// <summary>
        /// Anything that needs to be maintained every frame goes here.
        /// </summary>
        public override void Tick()
        {
            base.Tick();
        }


        /// <summary>
        /// Tests if we should be activating the motion. Returns TRUE by default.
        /// </summary>
        /// <returns></returns>
        public override bool TestActivate()
        {
            if (character.StatusManager.GetIsActiveStatus<Status_Cover>() == false) 
            {
                return false; 
            }
            return base.TestActivate();
        }

        /// <summary>
        /// Tests if we should be updating the motion. Returns TRUE by default.
        /// </summary>
        public override bool TestTick()
        {
            return base.TestTick();
        }

        /// <summary>
        /// Tests if we should end the motion. Returns FALSE by default.
        /// </summary>
        public override bool TestDeactivate()
        {
            if (!character.StatusManager.GetIsActiveStatus<Status_Cover>()) { return true; }
            return base.TestDeactivate();
        }

        /// <summary>
        /// If we want to do funny stuff with the root motion, we do it here. Base method applies default root motion.
        /// </summary>
        /// <param name="rb"></param>
        public override void OnAnimatorMove(Rigidbody rb)
        {
            rb.AddForce(Physics.gravity, ForceMode.Acceleration);
            base.OnAnimatorMove(rb);
        }
        #endregion
    }
}
