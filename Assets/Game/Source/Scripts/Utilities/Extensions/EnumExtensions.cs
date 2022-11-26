// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: Extension methods for enums.
// -----------------------
// ------------------- */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
    /// <summary>
    /// Returns a random value from the enum.
    /// </summary>
    /// <param name="t">The type of enum</param>
    /// <returns>A random enum value</returns>
    public static Enum GetRandomValue(this Type t)
    {
        return Enum.GetValues(t)            // Get all the values from the enum
            .OfType<Enum>()                 // Cast them to an enum.
            .OrderBy(e => Guid.NewGuid())   // Order them randomly.
            .FirstOrDefault();              // Take the first item.
    }
}