// /* --------------------
// -----------------------
// Creation date: 09/11/2022
// Author: Alex
// Description: An item in a weighted list used for weighted random selection.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedItem<T>
{
    public T Item;
    public int Weight;

    public WeightedItem(T item, int weight)
    {
        Item = item;
        Weight = weight;
    }
}
