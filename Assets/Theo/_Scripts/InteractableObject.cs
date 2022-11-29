// /* --------------------
// -----------------------
// Creation date: 26/11/2022
// Author: Theodore
// Description: A script to apply on objects to make them interactables, in order for them to be able to trigger an
// effect on a target object when player presses the interact button while in their trigger range.
// -----------------------
// ------------------- */


using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private GameObject m_target;

    private bool m_triggerInput;
    private float m_triggerInputCooldown = 0.3f;
    private bool m_isPlayerColliding;

    public enum Interactions
    {
        Explode,
        Activate,
        Deactivate
    }

    public Interactions m_interaction;

    private delegate void OnInteract(GameObject target);
    private OnInteract Interaction;

    private void OnEnable()
    {
        switch (m_interaction)
        {
            default:
                Debug.Log("No default interaction set"); break;

            case Interactions.Explode:
                Interaction += ExplodeTarget; break;

            case Interactions.Activate:
                Interaction += ActivateTarget; break;

            case Interactions.Deactivate:
                Interaction += DeactivateTarget; break;
        }
    }



    private void Update()
    {
        if (m_target == null)
            return;

        if (!m_isPlayerColliding)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_triggerInput = true;

            m_triggerInputCooldown -= Time.deltaTime;

            if (m_triggerInputCooldown <= 0)
            {
                m_triggerInput = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerColliding = true;

            if (m_triggerInput)
            {
                m_triggerInput = false;
                //ExplodeTarget(m_target);

                Interaction?.Invoke(m_target);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_isPlayerColliding = false;
    }

    private void ExplodeTarget(GameObject target)
    {
        DestructableObject tmp = target.GetComponent<DestructableObject>();

        if (tmp == null)
        {
            Debug.LogWarning("Target needs the DestructableObject script to trigger");
            return;
        }

        tmp.OnDeath();

        Grid.Instance.BakeNavMesh();
    }

    private void ActivateTarget(GameObject target)
    {
        target.SetActive(true);

        Grid.Instance.BakeNavMesh();
    }

    private void DeactivateTarget(GameObject target)
    {
        target.SetActive(false);

        Grid.Instance.BakeNavMesh();
    }
}
