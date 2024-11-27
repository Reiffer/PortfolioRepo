using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Animations;
using UnityEngine;
using Controllers.Characters;

using Controllers.Inputs;


namespace Controllers.Animation
{

    public interface IActionController
    {
        bool DebugActive { get; }

        Animator mAnim { get; }

        Characters.CharacterManager mCharacterManager { get; }
        Combatant mCombatant { get; }
        float animX { get; set; }
        float animY { get; set; }
        float animZ { get; set; }
        int Base_ActionIdent { get; set; }
        int Base_ActionParam { get; set; }
        int UB_ActionIdent { get; set; }
        int UB_ActionParam { get; set; }
        bool isGrounded { get; set; }
        bool isActive { get; }
        bool isAerial { get; set; }
        bool isLockedOn { get; set; }
        bool isDoubleJumped { get; set; }
        bool isWallRunned { get; set; }
        string HitReactTrigger { get; }


        IAction[] AvailableActions { get; }
        IAction Base_ActiveAction { get; set; }
        IAction UB_ActiveAction { get; set; }
        IAction Base_QueuedAction { get; }
        ContactPoint cPoint { get; set; }
        Collision ActiveCollision { get; set; }
        Rigidbody rb { get; set; }

        void Construct(ICharacterManager CC, Animator anim);
        void UpdateAnimatorController();
        void UpdateAnimatorAxes(Rigidbody rb);
        void ValidateActionList();
        void TestActionActivation();
        void TestActionTick();
        void SendAnimationMessage(string Event);
        void FireAnimationTrigger(string TriggerName);


        bool CommandActionActivationOfType<T>(ICommand command, out T action) where T : ICommandedAction;
        T GetAction<T>();
    }
}
