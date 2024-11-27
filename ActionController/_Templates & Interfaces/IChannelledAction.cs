using Combat.Status;
using System.Collections;
using UnityEngine;

namespace Controllers.Animation
{
    public interface IChannelledAction
    {
        public void AssignChannellingStatus(Status_Channelling channel);
    }
}