using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Animation;
using System;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace Controllers.Animation
{

    /// <summary>
    /// Activated when a character is using a MOUNT NavMeshLink. MOUNTs put the character at the top or bottom of a scalable wall.
    /// </summary>
    public class MountUnmountLink : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs

        bool isOnOffmeshLink { get { return mActionController.mCharacterManager.isOnOffMeshLink; } }

        Vector3 startPos;
        Vector3 endPos;

        NavMeshAgent Agent { get { return mActionController.mCharacterManager.mNavMeshAgent; } }


        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {
            mActionController.rb.transform.LookAt(Agent.steeringTarget, Vector3.up);
            startPos = mActionController.mCharacterManager.mNavMeshAgent.currentOffMeshLinkData.startPos;
            endPos = mActionController.mCharacterManager.mNavMeshAgent.currentOffMeshLinkData.endPos;
            mActionController.rb.isKinematic = true;


            //Detects if we're going up or down a wall.
            if (endPos.y < startPos.y) 
            {
                mActionController.Base_ActionParam = 1; 
            }

            base.Activate();
        }

        /// <summary>
        /// Ends the current motion and resets animator parameters.
        /// </summary>
        public override void Deactivate()
        {
            mActionController.rb.isKinematic = false;
            Agent.CompleteOffMeshLink();
            mActionController.mCharacterManager.transform.position = new Vector3(mActionController.mCharacterManager.transform.position.x, endPos.y, mActionController.mCharacterManager.transform.position.z);
            
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

            if (link.area != 3) { return false;}

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
        /// Tests if we should end the motion. Returns TRUE by default.
        /// </summary>
        public override bool TestDeactivate()
        {

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
        #endregion
    }
}
