// /* --------------------
// -----------------------
// Creation date: 07/11/2022
// Author: Alex
// Description: Destroys the gameobject this script is attached to after certain amount of time.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    public float Lifetime = 1f;

    private void Start()
    {
        this.Invoke(() => Destroy(gameObject), Lifetime);
    }
}
