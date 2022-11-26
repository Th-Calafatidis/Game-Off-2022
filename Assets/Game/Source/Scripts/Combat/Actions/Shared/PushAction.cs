// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: This action will push a unit in a set number of tiles.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAction : ICombatAction
{
    private Unit m_unit;
    private int m_tilesToPush;

    public IEnumerator Execute()
    {
        throw new System.NotImplementedException();
    }

    public PushAction(Unit unit, int tilesToPush)
    {
        m_unit = unit;
        m_tilesToPush = tilesToPush;
    }
}
