using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDebugging : Unit
{
    #region Debugging

    [Button("Poison")]
    public void AddPoison()
    {
        AddStatusEffect(new PoisonStatusEffect(1, 3));
    }

    [Button("Fire")]
    public void AddFire()
    {
        AddStatusEffect(new FireStatusEffect(1, 3));
    }

    [Button("Slow")]
    public void AddSlow()
    {
        AddStatusEffect(new SlowStatusEffect(3));
    }

    [Button("Remove Effects")]
    public void RemoveEffects()
    {
        StatusEffects.Clear();

    }

    #endregion
}
