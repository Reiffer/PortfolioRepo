using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;


namespace Controllers.Animation
{

    /// <summary>
    /// Mecanim SSM for grounded movement on a horizontal surface. Assumes traversal is handled in root motion.
    /// </summary>
    public class JogRunLean : Action, IAction
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs

        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {
            base.Activate();

            mActionController.isWallRunned = false;

            //Read the cached inputs from the AC:

            //Forward right, forward, forward left 
            if (mActionController.animX >= 0.5f && mActionController.animZ >= 0.5f)                                     { mActionController.Base_ActionParam = 0; }
            if (mActionController.animX < 0.5f && mActionController.animX > -0.5f && mActionController.animZ >= 0.5f)   { mActionController.Base_ActionParam = 1; }
            if (mActionController.animX < -0.5f && mActionController.animZ >= 0.5f)                                     { mActionController.Base_ActionParam = 2; }

            //Left, right
            if (mActionController.animX < -0.5f && mActionController.animZ < 0.5f && mActionController.animZ > -0.5f)   { mActionController.Base_ActionParam = 3; }
            if (mActionController.animX >= 0.5f && mActionController.animZ < 0.5f && mActionController.animZ > -0.5f)   { mActionController.Base_ActionParam = 4; }

            // Back-right, back, back-left.
            if (mActionController.animX > 0.5f && mActionController.animZ < -0.5f)                                       { mActionController.Base_ActionParam = 5; }
            if (mActionController.animX <= 0.5f && mActionController.animX > -0.5f && mActionController.animZ < -0.5f)   { mActionController.Base_ActionParam = 6; }
            if (mActionController.animX <= -0.5f && mActionController.animZ < -0.5f)                                     { mActionController.Base_ActionParam = 7; }
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

            //If we've only just started moving OR we've not in the attacking movement SSM, we use a SSM to show the character kick off into a run or strafe while attacking.
            if (mActionController.mAnim.GetCurrentAnimatorStateInfo((int)AnimatorLayer).tagHash == Animator.StringToHash("RUNNING") && !mActionController.mCombatant.isAttacking) { mActionController.Base_ActionParam = 10; }

            if (mActionController.mCombatant.isAttacking) { mActionController.Base_ActionParam = 11; }

            if (!mActionController.mCombatant.isAttacking && mActionController.Base_ActionParam == 11) { Deactivate(this); }

            base.Tick();
        }


        /// <summary>
        /// Tests if we should be activating the motion. Returns FALSE by default.
        /// </summary>
        /// <returns></returns>
        public override bool TestActivate()
        {
            //Account for controller and navmesh innacuracies.
            if (Mathf.Abs(mActionController.animX) <= 0.01f && Mathf.Abs(mActionController.animZ) <= 0.01f) { return false; }
            
            //We can only run on the ground.
            if (mActionController.isGrounded && base.TestActivate())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests if we should be updating the motion. Returns FALSE by default.
        /// </summary>
        public override bool TestTick()
        {
            return base.TestTick();
        }

        /// <summary>
        /// Tests if we should end the motion. Returns TRUE by default.
        /// </summary>
        public override bool TestDeactivate()
        {
            //Account for controller and navmesh inaccuracies.
            if (Mathf.Abs(mActionController.animX) <= 0.01f && Mathf.Abs(mActionController.animZ) <= 0.01f) { return true; }
            return base.TestDeactivate();
        }

        public override void OnAnimationMessage(string Event)
        {

        }

        public override void OnAnimatorMove(Rigidbody rb)
        {
            base.OnAnimatorMove(rb);
        }
        #endregion
    }
}
