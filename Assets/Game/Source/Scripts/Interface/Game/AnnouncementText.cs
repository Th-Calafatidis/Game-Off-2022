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
using UnityEngine.UI;

public class AnnouncementText : MonoBehaviour
{
    private TMP_Text m_text;
    private GameObject m_announcement;
    public GameObject StageCleared { get; set; }
    public GameObject GameFinished { get; set; }

    private void Awake()
    {
        m_text = GetComponentInChildren<TMP_Text>();
        m_announcement = GetComponentInChildren<Image>().gameObject;

        StageCleared = GameObject.Find("StageCleared");
        GameFinished = GameObject.Find("FinalScreen");
    }

    public void Announce(string text, float duration)
    {
        m_text.text = text;
        StartCoroutine(AnnounceRoutine(duration));
    }

    public void AnnounceStageCleared(float duration)
    {
        StartCoroutine(StageClearedText(duration));
    }

    private IEnumerator AnnounceRoutine(float duration)
    {
        m_announcement.SetActive(true);
        yield return new WaitForSeconds(duration);
        m_announcement.SetActive(false);
    }

    private IEnumerator StageClearedText(float duration)
    {
        StageCleared.SetActive(true);
        yield return new WaitForSeconds(duration);
        StageCleared.SetActive(false);
    }
}
