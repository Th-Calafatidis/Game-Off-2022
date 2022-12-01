// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This is a script to preview the player's position when selecting a position to move to.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionPreview : MonoBehaviour
{
    private Player m_player;
    private TMP_Text m_apText;

    private void Awake()
    {
        m_apText = transform.Find("APDisplay").GetComponentInChildren<TMP_Text>();

        // Disable all children, so that we can enable them when we need to.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateApText();
    }

    private void Start()
    {
        m_player = GetComponentInParent<Player>();
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

    public void UpdateApText()
    {
        Vector2Int playerPosition = Grid.Instance.GetGridPosition(m_player.transform.position);

        Vector2Int mousePosition = m_player.GetCursorGridPosition();

        List<Vector2Int> currentPath = new List<Vector2Int>();

        currentPath = Grid.Instance.GetPath(playerPosition, mousePosition);

        if (currentPath == null)
            return;

        m_apText.text = currentPath.Count.ToString() + " AP";
    }
}
