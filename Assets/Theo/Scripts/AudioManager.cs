using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_menuSounds;
    [SerializeField] private List<AudioClip> m_battleSounds;

    [Header("Button Effect Clips")]
    [SerializeField] private AudioClip m_buttonBack;
    [SerializeField] private AudioClip m_buttonSelected;
    [SerializeField] private AudioClip m_buttonDenied;
    [SerializeField] private AudioClip m_buttonSelection;

    private AudioSource m_audioScource;


    private void Start()
    {
        m_audioScource = GetComponent<AudioSource>();

        m_audioScource.Stop();

        m_audioScource.loop = true;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(PlaySounds(m_audioScource, m_menuSounds));
        }
        else
        {
            StartCoroutine(PlaySounds(m_audioScource, m_battleSounds));
        }
    }

    private IEnumerator PlaySounds(AudioSource source, List<AudioClip> clips)
    {
        yield return new WaitForEndOfFrame();

        // This is not ideal but we know we only have two sound clips so it will work for now.
        source.PlayOneShot(clips[0]);

        var activeClip = clips[0];

        Debug.Log(activeClip.name);

        yield return new WaitForSeconds(clips[0].length);

        source.clip = clips[1];

        source.Play();

        activeClip = clips[1];

        Debug.Log(activeClip.name);
    }
}
