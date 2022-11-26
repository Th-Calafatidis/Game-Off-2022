// /* --------------------
// -----------------------
// Creation date: 13/11/2022
// Author: Alex
// Description: A potential line that an enemy can say when performing an action.
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionLine
{
    public string Tag;
    [TextArea]
    public string Line;

    public void SetVariable(string varName, int value)
    {
        Line = Line.Replace("{" + varName + "}", value.ToString());
    }
}
