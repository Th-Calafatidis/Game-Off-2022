// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: This action will deal damage to multiple tiles, creating a single damage action for each tile.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageAction : ICombatAction
{
    public List<SingleDamageAction> Actions { get; private set; }


    public IEnumerator Execute()
    {
        foreach (SingleDamageAction action in Actions)
        {
            action.Execute();
        }
        yield return 0;
    }

    public AreaDamageAction(List<SingleDamageAction> actions)
    {
        Actions = actions;
    }
}
