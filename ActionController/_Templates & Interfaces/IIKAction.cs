using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using Controllers.Animation;
using UnityEngine;
using Controllers.Characters;
using Controllers.Inputs;

/// <summary>
/// Interface to add to an action if we want to call mecanim's IK system.
/// </summary>
public interface IIKAction : IAction
{
    void OnAnimatorIK(int layerIndex);
}
