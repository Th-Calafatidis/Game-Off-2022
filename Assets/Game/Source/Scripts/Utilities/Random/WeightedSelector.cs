// /* --------------------
// -----------------------
// Creation date: 09/11/2022
// Author: Alex
// Description: A class for performing weighted selection on a list of weighted items.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedSelector<T>
{
    private List<WeightedItem<T>> m_weightedItems;
    

    public WeightedSelector(List<WeightedItem<T>> weightedItems)
    {
        m_weightedItems = weightedItems;
    }

    public T Select()
    {
        int totalWeight = 0;
        foreach (WeightedItem<T> weightedItem in m_weightedItems)
        {
            totalWeight += weightedItem.Weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (WeightedItem<T> weightedItem in m_weightedItems)
        {
            currentWeight += weightedItem.Weight;
            if (randomWeight < currentWeight)
            {
                return weightedItem.Item;
            }
        }

        return default(T);
    }
}
