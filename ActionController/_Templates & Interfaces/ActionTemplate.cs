using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;
using Unity.AI.Navigation;
using UnityEngine.AI;


namespace Controllers.Animation
{
    //              --IMPORTANT--
    //
    // Please bear in mind when implementing:
    //
    //      TestActivate() is called across _all_ available actions _every_ frame.
    //      TestDeactivate() is called across _all_ active actions _every_ frame.
    //      TestTick() is called across _all_ active actions _every_ frame.
    //
    // Keep the logic in these methods light. 

    /// <summary>
    /// Implementation of template class to copypaste for new motion code. Each Action is resposible for its own activation, deactivation and maintenance.
    /// </summary>
    public class Action_RENAME_ME : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs
        /*
         * Everything we need to work with action-specific logic should go here. 
         */
        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        /// <summary>
        /// Called after TestActivate() returns true. Put state entry logic here.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }

        /// <summary>
        /// Called if TestDeactivate() returns true. Ends the current action and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Ends the current action and resets animator parameters, then attempts to activate the passed action.
        /// </summary>
        /// <param name="queuedMotion"></param>
        public override void Deactivate(IAction queuedMotion)
        {
            base.Deactivate(queuedMotion);
        }

        /// <summary>
        /// Called every frame after TestTick returns true. Update logic goes here.
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
            return base.TestDeactivate();
        }

        /// <summary>
        /// Receives messages placed at specific frames in the playing animation. Used for activating VFX, SFX, and other timing-specific things.
        /// </summary>
        /// <param name="Event"></param>
        public virtual void OnAnimationMessage(string Event)
        {

        }

        /// <summary>
        /// Any logic pertaining _specifically_ to root motion should happen here (like applying non-standard gravity for wallrunning).
        /// Any gameplay behaviour should happen in Tick().
        /// </summary>
        /// <param name="rb"></param>
        public override void OnAnimatorMove(Rigidbody rb)
        {
            //If there's root motion applied to the anims and we don't want it, we can add logic to prevent this call.
            base.OnAnimatorMove(rb);
        }
        #endregion
    }
}
