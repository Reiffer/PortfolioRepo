using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using Controllers.Animation;
using UnityEngine;
using Controllers.Characters;
using Controllers.Inputs;

public enum AnimLayer { Base = 0, UB = 1, LB = 2 }

public interface IAction
{
    IActionController mActionController { get; set; }

    AnimLayer AnimatorLayer { get; }
    int Priority { get; }
    int MotionIdent { get; }
    bool isActive { get; set; }
    bool DefaultRootMotion { get; }

    bool DebugActivate { get; set; }

    bool isDisabled { get; set; }

    bool TestActivate();
    bool TestDeactivate();
    bool TestTick();
    void Activate();
    void Tick();
    void Deactivate();
    void Deactivate(IAction queuedMotion);
 
    void OnAnimationMessage(string Event);
    void OnAnimatorMove(Rigidbody rb);

}
