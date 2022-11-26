using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatAction
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    public IEnumerator Execute();
}
