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
    /// Activated when a character is using a VAULT OffMeshLink. VAULTs move the character frome one side of a low-height obstacle to their other, over the top of said obstacle.
    /// </summary>
    public class VaultLink : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs
        bool isOnOffmeshLink { get { return mActionController.mCharacterManager.isOnOffMeshLink; } }
        NavMeshAgent Agent { get { return mActionController.mCharacterManager.mNavMeshAgent; } }
        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {
            //Physics collisions should not interrupt the vault.
            mActionController.rb.isKinematic = true;
            mActionController.rb.transform.LookAt(Agent.steeringTarget);

            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            //Needed to re-sync the agent and the gameobject.
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

            //Int used to identify the OffMeshLink's type. 4 is VAULT.
            if (link.area != 4) { return false; }

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
