using System;
using UnityEngine;
using Controllers.Inputs;
using Controllers.Characters;
using Sirenix.OdinInspector;
using System.Diagnostics;

namespace Controllers.Animation
{
    //TODO 
    //  - Expand Motion vars to cover Upper/LowerBody anim layers.    

    /// <summary>
    /// Class that sits between other animation-dependent components and the Animator. Inherits from Odin's SerializedMonoBehaviour to display IActions as an array in-editor.
    /// </summary>
    public class ActionController : SerializedMonoBehaviour, IActionController
    {
        #region External Reference
        // ---------------------------------------------------
        public Rigidbody rb { get; set; }

        Animator _mAnim;
        /// <summary>
        /// The animator on the GameObject. Injected via the CharacterManager. See constructor.
        /// </summary>
        public Animator mAnim { get { return _mAnim; } }

        /// <summary>
        /// The character manager is the centre of each character. Contains generic game-state data and passes commands to the CommandQueue.
        /// </summary>
        CharacterManager _mCharacterManager;
        public CharacterManager mCharacterManager { get {  return _mCharacterManager; } }


        /// <summary>
        /// The combatant handles combat logic.
        /// </summary>
        Combatant _mCombatant;
        public Combatant mCombatant { get {  return _mCombatant; } }

        /// <summary>
        /// The input driving the action controller. Fetched in the constructor via GetComponent<IInput>.
        /// </summary>
        [ShowInInspector]
        IInput _mInput;
        public IInput mInput { get { return _mInput; } }

        #endregion

        #region Internal Action Refs

        [ShowInInspector]
        public bool DebugActive { get; set; }

        [SerializeField]
        private IAction[] _AvailableActions;
        public IAction[] AvailableActions { get { return _AvailableActions; } }

        [ShowInInspector]
        IAction _Base_ActiveAction;
        public IAction Base_ActiveAction
        {
            get { return _Base_ActiveAction; }
            set
            {
                if (value != null)
                {
                    _Base_ActiveAction = value;
                }
            }
        }

        public IAction UB_ActiveAction { get; set; }

        IAction _QueuedAction;
        public IAction Base_QueuedAction { get { return _QueuedAction; } }

        #endregion

        public ContactPoint cPoint { get; set; }
        public Collision ActiveCollision { get; set; }

        // ---------------------------------------------------

        #region Animator Variables

        // ---------------------------------------------------
        // ---------------Variables & Properties--------------
        // ---------------------------------------------------


        float _animX;
        /// <summary>
        /// Sets the AnimX of the animator referenced by this controller to value.
        /// </summary>
        public float animX
        {
            get { return _animX; }
            set
            {
                _animX = value;
                _mAnim.SetFloat("AnimX", _animX);
            }
        }

        // ---------------------------------------------------

        float _animY;
        /// <summary>
        /// Sets the AnimY of the animator referenced by this controller to value.
        /// </summary>
        public float animY
        {
            get { return _animY; }
            set
            {
                _animY = value;
                _mAnim.SetFloat("AnimY", _animY);
            }
        }

        // ---------------------------------------------------

        float _animZ;
        /// <summary>
        /// Sets the AnimZ of the animator referenced by this controller to value.
        /// </summary>
        public float animZ
        {
            get { return _animZ; }
            set
            {
                _animZ = value;
                _mAnim.SetFloat("AnimZ", _animZ);
            }
        }

        // ---------------------------------------------------

        bool _isGrounded;
        /// <summary>
        /// Sets the isGrounded of the animator referenced by this controller to value.
        /// </summary>
        public bool isGrounded
        {
            get { return _isGrounded; }
            set
            {
                _isGrounded = value;
                _mAnim.SetBool("isGrounded", _isGrounded);
            }
        }

        // ---------------------------------------------------

        bool _isAerial;
        /// <summary>
        /// Sets the isPivoting of the animator referenced by this controller to value.
        /// </summary>
        public bool isAerial
        {
            get { return _isAerial; }
            set
            {
                _isAerial = value;
                _mAnim.SetBool("isAerial", _isAerial);
            }
        }

        // ---------------------------------------------------

        int _Base_MotionIdent;
        /// <summary>
        /// Sets the Base_MotionIdent of the animator referenced by this controller to value.
        /// </summary>
        public int Base_ActionIdent
        {
            get { return _Base_MotionIdent; }
            set
            {
                //Debug.Log("Request to change MotionIdent to: " + value);
                _Base_MotionIdent = value;
                _mAnim.SetInteger("Base_MotionIdent", _Base_MotionIdent);
                if (DebugActive) { Debug.Log("Setting ActionIdent to: " + Base_ActiveAction + " " + value); }
            }
        }

        // ---------------------------------------------------

        int _Base_MotionParam;
        /// <summary>
        /// Sets the Base_MotionParam of the animator referenced by this controller to value.
        /// </summary>
        public int Base_ActionParam
        {
            get
            {
                return _Base_MotionParam;
            }

            set
            {
                _Base_MotionParam = value;
                _mAnim.SetInteger("Base_MotionParam", value);
                if (DebugActive) { Debug.Log("Setting ActionParam to: " + Base_ActiveAction + " " + value); }
            }
        }

        int _UB_MotionIdent;
        /// <summary>
        /// Sets the Base_MotionIdent of the animator referenced by this controller to value.
        /// </summary>
        public int UB_ActionIdent
        {
            get { return _UB_MotionIdent; }
            set
            {
                //Debug.Log("Request to change MotionIdent to: " + value);
                _UB_MotionIdent = value;
                _mAnim.SetInteger("UB_MotionIdent", value);
                if (DebugActive) { Debug.Log("Setting UB_ActionIdent to: " + UB_ActiveAction + " " + value); }
            }
        }

        // ---------------------------------------------------

        int _UB_MotionParam;
        /// <summary>
        /// Sets the Base_MotionParam of the animator referenced by this controller to value.
        /// </summary>
        public int UB_ActionParam
        {
            get
            {
                return _UB_MotionParam;
            }

            set
            {
                _UB_MotionParam = value;
                _mAnim.SetInteger("UB_MotionParam", value) ;
                if (DebugActive) { Debug.Log("Setting UB_ActionParam to: " + UB_ActiveAction + " " + value); }
            }
        }



        bool _isLocked;
        /// <summary>
        /// Is the animator currently updating?
        /// </summary>
        public bool isActive
        {
            get { return _isLocked; }
        }

        bool _isLockedOn;
        /// <summary>
        /// Is the combatant that this animator is associated with locked onto another combatant?
        /// </summary>
        public bool isLockedOn
        {
            get { return _isLockedOn; }
            set { _isLockedOn = value; mAnim.SetBool("isLockedOn", value); /* Debug.Log("AC: Locking value is " + value); */ }
        }

        public bool isDoubleJumped { get; set; }

        public bool isWallRunned { get; set; }

        public string HitReactTrigger { get { return "HitReactTrigger"; } }



        // ---------------------------------------------------
        // ----------------End of variables-------------------
        // ---------------------------------------------------

        #endregion

        #region Methods
        // ---------------------------------------------------
        // ----------------Methods----------------------------
        // ---------------------------------------------------

        public void UpdateAnimatorController()
        {
            //Debugging for Base and Upper Body actions.
            if (DebugActive) { Debug.Log("Base action on " + gameObject + " is " + Base_ActiveAction); }
            if (DebugActive) { Debug.Log("UB action on " + gameObject + " is " + UB_ActiveAction); }

            if (mCharacterManager.mCharacterDataOject.isDirty) { UpdateAnimatorStatParams(); }

            TestActionTick();
            TestActionActivation();
        }

        /*
        /// <summary>
        /// DEPRECATED - Directly interprets virtual stick inputs from a controller or NPC. No longer used for current PC RTS project.
        /// </summary>
        /// <param name="rb"></param>
        public void UpdateAnimatorAxes(Rigidbody rb)
        {
            
        }
        */

        //Whenever an RPG stat is changed that impacts the way the animations should work, we update mecanim's params.
        public void UpdateAnimatorStatParams() 
        {
            mAnim.SetFloat("Speed", mCharacterManager.mCharacterDataOject.Speed);
        }

        public virtual void TestActionActivation()
        {
            foreach (IAction action in AvailableActions)
            {
                if (!action.isActive)
                {
                    if (action.TestActivate() && !action.isDisabled)
                    {
                        if (DebugActive) { Debug.Log("Activating " + action + " on " + action.AnimatorLayer); }
                        if (action.AnimatorLayer == AnimLayer.Base)
                        {
                            _Base_ActiveAction.Deactivate();
                            _Base_ActiveAction = action;
                            _Base_ActiveAction.Activate();
                        }
                        if (action.AnimatorLayer == AnimLayer.UB)
                        {
                            if (UB_ActiveAction != null)
                            {
                                UB_ActiveAction.Deactivate();
                            }
                            UB_ActiveAction = action;
                            UB_ActiveAction.Activate();
                        }
                    }
                }
            }

        }

        public virtual void TestActionTick()
        {
            foreach (IAction action in AvailableActions)
            {
                if (action.isActive)
                {
                    if (action.TestTick()) { action.Tick(); }
                }
            }
        }

        /// <summary>
        /// Called when mecanim detects an authored animation message on the current anim frame. Each action then checks the passed string.
        /// </summary>
        /// <param name="Event"></param>
        public void SendAnimationMessage(string Event)
        {
            foreach (IAction action in AvailableActions)
            {
                action.OnAnimationMessage(Event);
            }
        }

        /// <summary>
        /// Unity Message for hooking into mecanim's updates. Typically used in the AC for altering root motion or working with physics.
        /// </summary>
        private void OnAnimatorMove()
        {
            if (rb == null) { rb = gameObject.GetComponent<Rigidbody>(); }

            Base_ActiveAction.OnAnimatorMove(rb);
            if (UB_ActiveAction != null)
            {
                if (UB_ActiveAction.isActive)
                UB_ActiveAction.OnAnimatorMove(rb);
            }
        }

        /// <summary>
        /// Unity Message for hooking into mecanim's IK system.
        /// </summary>
        private void OnAnimatorIK(int layerIndex)
        {
            if (UB_ActiveAction != null)
            {
                if (UB_ActiveAction.isActive && UB_ActiveAction is IIKAction)
                {
                    IIKAction IKAction = UB_ActiveAction as IIKAction;
                    IKAction.OnAnimatorIK(layerIndex);
                }
            }
        }

        /// <summary>
        /// Fire a mecanim trigger by name (bools that unset after a frame).
        /// </summary>
        /// <param name="TriggerName"></param>
        public void FireAnimationTrigger(string TriggerName) 
        {
            mAnim.SetTrigger(TriggerName);
        }

        // ---------------------------------------------------
        // ----------------End of Methods---------------------
        // ---------------------------------------------------
        #endregion

        #region Constructors & Gets
        public virtual void Construct(ICharacterManager CC, Animator anim)
        {
            _mCombatant = gameObject.GetComponent<Combatant>();
            _mCharacterManager = CC as CharacterManager;
            _mAnim = anim;
            rb = gameObject.GetComponent<Rigidbody>();
            _mInput = GetComponent<IInput>();
            ValidateActionList();
            UpdateAnimatorStatParams();
        }

        /// <summary>
        /// Returns, if available, action of type T from this AC's list of available actions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAction <T> ()
        {
            foreach (IAction action in AvailableActions)
            {
                if (action.GetType() == typeof (T)) { return (T)Convert.ChangeType(action, typeof(T)); }
            }

            //Debug.Log("Action of type " + typeof(T) + " does not exist in " + gameObject.name + "'s available actions.");

            return default(T);
        }

        /// <summary>
        /// Injects relevant dependencies into the action list. Currently only the AC to be passed in - might expand.
        /// </summary>
        public virtual void ValidateActionList()
        {
            for (int i = 0; i < AvailableActions.Length; i++)
            {
                IAction action = AvailableActions[i];
                action.mActionController = this;
            }
            _Base_ActiveAction = _AvailableActions[0];
        }

        /// <summary>
        /// When the command queue requests a specific action (e.g. for a character ability) it uses this this method to get by type while 
        /// also passing itself so that the action has access to all of its data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool CommandActionActivationOfType<T>(ICommand command, out T action) 
            where T : ICommandedAction
        {
            action = GetAction<T>();
            if (action == null) { return false; }
            action.AssignTriggeringCommand(command);
            return true;
        }
        #endregion
    }
}
