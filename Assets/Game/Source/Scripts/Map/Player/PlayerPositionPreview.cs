// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This is a script to preview the player's position when selecting a position to move to.
// -----------------------
// ------------------- */

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionPreview : MonoBehaviour
{
    private void Awake()
    {
        // Disable all children, so that we can enable them when we need to.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void Show(Vector3 position)
    {
        transform.position = position + Vector3.down * 0.5f;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
