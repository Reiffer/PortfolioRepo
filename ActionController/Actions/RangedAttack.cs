using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Collections.Specialized;

namespace Controllers.Animation
{

    /// <summary>
    /// Action that aims ranged weapons at a target. All combat logic is handled by the Combatant class. 
    /// </summary>
    public class RangedAttack: Action, IIKAction
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs

        Combatant combatant { get { return mActionController.mCombatant; } }


        NavMeshAgent agent { get { return mActionController.mCharacterManager.mNavMeshAgent; } }

        int attackspeed = 0;

        bool aimed = false;

        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        ///<summary>
        ///Logic called upon entry to the action.
        ///</summary>summary>
        public override void Activate()
        {
            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            mActionController.mAnim.SetLookAtWeight(0);
            base.Deactivate();
        }

        public override void Deactivate(IAction queuedMotion)
        {
            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            mActionController.mAnim.SetLookAtWeight(0);
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
            if (!combatant.isAttacking) { return false; }
            if (agent.isOnOffMeshLink) { return false; }
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
            if (mActionController.mCombatant.Target == null) { return true; }
            if (combatant.isAttacking == false) { return true; }
            if (agent.isOnOffMeshLink) { return true; }
            return base.TestDeactivate();
        }

        /// <summary>
        /// If we want to do funny stuff with the root motion, we do it here. By default applies default root motion.
        /// </summary>
        /// <param name="rb"></param>
        public override void OnAnimatorMove(Rigidbody rb)
        {
            base.OnAnimatorMove(rb);
        }

        public override void OnAnimationMessage(string Event)
        {
            base.OnAnimationMessage(Event);
        }

        public void OnAnimatorIK(int layerIndex)
        {
            if (combatant.Target == null) { return; }
            Vector3 targetPos = combatant.Target.transform.position + (Vector3.up * 1.5f);
            Quaternion targetRot = Quaternion.LookRotation(targetPos - mActionController.rb.transform.position);

            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.RightHand,1);

            mActionController.mAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            mActionController.mAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            mActionController.mAnim.SetIKPosition(AvatarIKGoal.RightHand,targetPos);
            mActionController.mAnim.SetIKPosition(AvatarIKGoal.LeftHand, targetPos);

            mActionController.mAnim.SetIKRotation(AvatarIKGoal.RightHand, targetRot);
            mActionController.mAnim.SetIKRotation(AvatarIKGoal.LeftHand, targetRot);
        }
        #endregion
    }
}
