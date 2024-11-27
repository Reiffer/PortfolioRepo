using System.Collections;
using UnityEngine;

namespace Controllers.Animation
{
    public interface ICommandedAction :IAction
    {
        public void AssignTriggeringCommand<T>(T triggeringCommand)
            where T : ICommand;

        
    }
}