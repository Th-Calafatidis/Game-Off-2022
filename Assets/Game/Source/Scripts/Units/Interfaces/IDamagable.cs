// Creation date: 04/11/2022
// Author: Alex
// Description: Interface for any unit that can be damaged.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    /// <summary>
    /// Signals to the unit that it is recieving damage.
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    public void TakeDamage(int damage);
}
