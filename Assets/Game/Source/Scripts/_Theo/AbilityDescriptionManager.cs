using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDescriptionManager : MonoBehaviour
{
    [SerializeField] private GameObject m_descriptionBox;
    [SerializeField] private TMP_Text m_text;


    public void HideAbilityBox()
    {
        m_descriptionBox.SetActive(false);
    }

    public void ShowAbilityInfo(string abilityInfo)
    {
        m_descriptionBox.SetActive(true);
        m_text.text = abilityInfo;
    }
}
