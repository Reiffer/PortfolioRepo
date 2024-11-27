using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;
using Unity.AI.Navigation;
using UnityEngine.AI;


namespace Controllers.Animation
{
    /// <summary>
    /// Activated when a character is using a LEAP NavMeshLink. LEAPs scale horizontal gaps.
    /// </summary>
    public class LeapLink : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs
        bool isOnOffmeshLink { get { return mActionController.mCharacterManager.isOnOffMeshLink; } }
        NavMeshAgent Agent { get { return mActionController.mCharacterManager.mNavMeshAgent; } }

        Vector3 cPointRelativeToCharacter;

        Rigidbody rb { get { return mActionController.rb; } }

        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {
            //The navmeshlinks are aligned to root motion, and gameplay design demands that this action is uninterrupted. Kinematic it is.
            mActionController.rb.isKinematic = true;

            //To orient the character to face the gap they are traversing:
            Vector3 lookAtPosition = Agent.currentOffMeshLinkData.endPos;
            lookAtPosition.y = rb.position.y;
            rb.transform.LookAt(lookAtPosition);

            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            //This puts the NavMeshAgent on the other end of the link.
            Agent.CompleteOffMeshLink();

            mActionController.rb.isKinematic = false;

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
            base.Tick();
        }


        /// <summary>
        /// Tests if we should be activating the motion. Returns TRUE by default.
        /// </summary>
        /// <returns></returns>
        public override bool TestActivate()
        {
            if (!isOnOffmeshLink) { return false; }

            NavMeshLink link = (NavMeshLink)Agent.navMeshOwner;

            //This area is used to designate leaps.
            if (link.area != 6) { return false; }

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
        /// By default applies default root motion.
        /// </summary>
        /// <param name="rb"></param>
        public override void OnAnimatorMove(Rigidbody rb)
        {
            base.OnAnimatorMove(rb);
        }
        #endregion
    }
}
