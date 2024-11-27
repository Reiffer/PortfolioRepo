using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Controllers.Inputs;

namespace Controllers.Animation
{

    /// <summary>
    /// Base class that manages a mecanim sub-state machine and gameplay logic. 
    /// Each Action is resposible for its own activation, deactivation and maintenance.
    /// </summary>
    public class Action : IAction
    {

        // ----------------------Vars & Refs--------------------------------------
        #region Boilerplate Vars&Refs

        [SerializeField]
        private bool _DebugActivate;
        public bool DebugActivate 
        {
            get { return _DebugActivate; }
            set { _DebugActivate = value; }
        }

        [SerializeField]
        private bool _isDisabled;
        public bool isDisabled { get { return _isDisabled; } set { _isDisabled = value; } }

        [SerializeField]
        private AnimLayer _AnimatorLayer;
        public virtual AnimLayer AnimatorLayer { get { return _AnimatorLayer; } }

        [SerializeField]
        protected int _Priority = -1;
        public int Priority
        {
            get
            {
                if (DebugActivate) { return 99; }
                if (_Priority == -1)
                {
                    //Debug.Log("WARNING: Attempting to access an EMPTY Priority!");
                }
                return _Priority;
            }
        }

        [SerializeField]
        int _MotionIdent = -1;
        public int MotionIdent
        {
            get
            {
                if (_MotionIdent == -1)
                {
                    //Debug.Log("WARNING: Attempting to access an EMPTY MotionIdent!");
                }
                return _MotionIdent;
            }
        }

        /// <summary>
        /// Virtual buttons are one of my solutions for managing multiple inputs. Both PC and NPC inputs can be interpreted as button presses.
        /// </summary>
        IVirtualButton _VirtualButtonReference = null;
        public IVirtualButton VirtualButtonReference
        {
            get { return _VirtualButtonReference; }
            set { _VirtualButtonReference = value; }
        }

        bool _isFirstTick = false;
        public bool isFirstTick { get { return _isFirstTick; } set { _isFirstTick = value; } }

        IActionController _mActionController;
        public IActionController mActionController
        {
            get { return _mActionController; }
            set { _mActionController = value; }
        }

        bool _isActive = false;
        public bool isActive
        {
            get { return _isActive; }
            set {_isActive  = value; }
        }

        protected bool _DefaultRootMotion;
        public bool DefaultRootMotion { get {return _DefaultRootMotion; } set { _DefaultRootMotion = value; } }


        #endregion
        // ----------------------Methods----------------------------------------
        #region Methods

        public virtual void Activate()
        {
            if (mActionController.DebugActive) { UnityEngine.Debug.Log("activating " + this); }

            if (AnimatorLayer == AnimLayer.Base) 
            {
                mActionController.Base_ActionIdent = MotionIdent;
                mActionController.Base_ActiveAction = this;
            }

            if (AnimatorLayer == AnimLayer.UB) 
            {
                mActionController.UB_ActiveAction = this;
                mActionController.UB_ActionIdent = MotionIdent;
            }

            isFirstTick = true;
            
            isActive = true;
        }

        public virtual void Deactivate()
        {
            if (mActionController.DebugActive) { UnityEngine.Debug.Log("Deactivating " + this); }
            _isActive = false;
            if (AnimatorLayer == AnimLayer.Base)
            {
                mActionController.Base_ActionIdent = 0;
                mActionController.Base_ActionParam = 0;

            }
            if (AnimatorLayer == AnimLayer.UB)
            {
                mActionController.UB_ActionParam = 0;
                mActionController.UB_ActionIdent = 0;
            }
            //Debug.Log(_isActive);

        }

        public virtual void Deactivate(IAction queuedAction)
        {
            if (mActionController.DebugActive) { UnityEngine.Debug.Log("Deactivating " + this + " with queued motion " + queuedAction); }
            if (AnimatorLayer == AnimLayer.Base)
            {
                mActionController.Base_ActionParam = 0;
                mActionController.Base_ActionIdent = 0;
            }
            if (AnimatorLayer == AnimLayer.UB)
            {
                mActionController.UB_ActionParam = 0;
                mActionController.UB_ActionIdent = 0;
            }
            _isActive = false;
            mActionController.Base_ActiveAction = queuedAction;
            queuedAction.Activate();
        }

        /// <summary>
        /// Anything that needs to be maintained every frame goes here.
        /// </summary>
        public virtual void Tick()
        {            
            if (TestDeactivate())
            {
                Deactivate();
            }
            if (isFirstTick) { isFirstTick = false; }
        }


        /// <summary>
        /// Tests if we should be activating the motion. Base version returns TRUE by default.
        /// </summary>
        /// <returns></returns>
        public virtual bool TestActivate()
        {
            if (DebugActivate) { return true; }
            if (AnimatorLayer == AnimLayer.Base) 
            {
                if (mActionController.Base_ActiveAction.Priority > Priority && mActionController.Base_ActiveAction.isActive)
                {
                    if (mActionController.DebugActive) { Debug.Log("False due to lower BASE prio"); }
                    return false;
                }
            }

            if (AnimatorLayer == AnimLayer.UB && mActionController.UB_ActiveAction != null)
            {
                if (mActionController.UB_ActiveAction.Priority > Priority && mActionController.UB_ActiveAction.isActive)
                {
                    if (mActionController.DebugActive) { Debug.Log("False due to lower UB prio"); }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests if we should be updating the motion. Returns TRUE by default.
        /// </summary>
        public virtual bool TestTick()
        {
            if (AnimatorLayer == AnimLayer.Base && mActionController.Base_ActiveAction != this) { return false; }
            if (AnimatorLayer == AnimLayer.UB && mActionController.UB_ActiveAction != this) { return false; }
            if (!isActive) { return false; }
            return true;
        }

        /// <summary>
        /// Tests if we should end the motion. Returns FALSE by default.
        /// </summary>
        public virtual bool TestDeactivate()
        {
            //Test if another action is now active and this action has not been deactivated on both anim layers.
            if (AnimatorLayer == AnimLayer.Base && mActionController.Base_ActiveAction != this && isActive == true) { return true; }
            if (AnimatorLayer == AnimLayer.UB && mActionController.UB_ActiveAction != this && isActive == true) { return true; }

            //Each mecanim SSM has a node at the end tagged with "EXIT", if the state can end by itself.
            if (mActionController.mAnim.GetCurrentAnimatorStateInfo((int)AnimatorLayer).tagHash == Animator.StringToHash("EXIT"))
            {
                if (mActionController.DebugActive) { UnityEngine.Debug.Log("EXIT state hit in " + this); }
                return true;
            }

            //If we fall through without being told to keep the motion, don't deactivate just in case.
            return false;
        }

        /// <summary>
        /// Reads messaged placed at specific frames in the playing animation. Used for activating VFX, SFX, and other timing-specific things.
        /// </summary>
        /// <param name="Event"></param>
        public virtual void OnAnimationMessage(string Event)
        {

        }

        /// <summary>
        /// Any movement or movement-related logic should happen here.
        /// Any gameplay logic should happen in Tick().
        /// </summary>
        /// <param name="rb"></param>
        public virtual void OnAnimatorMove(Rigidbody rb)
        {
            mActionController.mAnim.ApplyBuiltinRootMotion();
        }

        #endregion
    }
}
