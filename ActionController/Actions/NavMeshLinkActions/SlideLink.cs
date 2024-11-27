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
    /// Activated when a character is using a SLIDE NavMeshLink. SLIDEs move the character along a horizontal(ish) surface and shrink the collider towards the floor for the duration.
    /// Slides can be used to move quickly, and provide cover if sliding behind/under occluding terrain.
    /// </summary>
    public class SlideLink : Action
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Vars&Refs

        bool isOnOffmeshLink { get { return mActionController.mCharacterManager.isOnOffMeshLink; } }
        NavMeshAgent Agent { get { return mActionController.mCharacterManager.mNavMeshAgent; } }

        CapsuleCollider col;
        float defaultColliderHeight;
        float defaultColliderCenterY;

        #endregion
        // ----------------------Functions----------------------------------------
        #region Functions

        public override void Activate()
        {

            //Physics collisions should not interrupt the slide.
            mActionController.rb.isKinematic = true;

            //Align orientation for slide.
            mActionController.rb.transform.LookAt(Agent.steeringTarget);

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
            //Moves the agent to the other end of the link.
            Agent.CompleteOffMeshLink();
            mActionController.rb.isKinematic = false;

            //Used cached dimensions to return collider to normal on exit.
            col.height = defaultColliderHeight;
            col.center = new Vector3(0, defaultColliderCenterY, 0);

            base.Deactivate();
        }

        public override void Deactivate(IAction queuedMotion)
        {
            //Moves the agent to the other end of the link.
            Agent.CompleteOffMeshLink();
            mActionController.rb.isKinematic = false;

            //Used cached dimensions to return collider to normal on exit.
            col.height = defaultColliderHeight;
            col.center = new Vector3(0, defaultColliderCenterY, 0);

            base.Deactivate(queuedMotion);
        }

        /// <summary>
        /// Anything that needs to be maintained every frame goes here.
        /// </summary>
        public override void Tick()
        {
            //After we've initiated the slide, go to a looping anim until EXIT.
            if (mActionController.mAnim.GetCurrentAnimatorStateInfo((int)AnimatorLayer).tagHash == Animator.StringToHash("SLIDING")) { mActionController.Base_ActionParam = 1; }
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

            if (link.area != 5) { return false; }
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
