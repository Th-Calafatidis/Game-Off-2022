// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: Sets the text at the top of the screen to the given string.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnnouncementText : MonoBehaviour
{
    private TMP_Text m_text;

    private void Awake()
    {
        m_text = GetComponentInChildren<TMP_Text>();
    }

    public void Announce(string text, float duration)
    {
        m_text.text = text;
        StartCoroutine(AnnounceRoutine(duration));
    }

    private IEnumerator AnnounceRoutine(float duration)
    {
        m_text.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        m_text.gameObject.SetActive(false);
    }
}
