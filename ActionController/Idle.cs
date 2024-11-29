using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;


namespace Controllers.Animation
{

    /// <summary>
    /// Default state while grounded and without other game states.
    /// </summary>
    public class Idle : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs


        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {
            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Deactivate(IAction queuedMotion)
        {
            base.Deactivate(queuedMotion);
        }

        /// <summary>
        /// Anything that needs to be maintained every frame goes here.
        /// </summary>
        public override void Tick()
        {
            mActionController.Base_ActionParam = 0;
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
        /// By default applies root motion.
        /// </summary>
        /// <param name="rb"></param>
        public override void OnAnimatorMove(Rigidbody rb)
        {
            base.OnAnimatorMove(rb);
        }
        #endregion
    }
}
