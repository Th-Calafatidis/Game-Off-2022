using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_menuSounds;
    [SerializeField] private List<AudioClip> m_battleSounds;
    [SerializeField] private AudioClip m_winSound;
    [SerializeField] private AudioClip m_lossSound;

    

    [Header("Button Effect Clips")]
    [SerializeField] private AudioClip m_buttonBack;
    [SerializeField] private AudioClip m_buttonSelected;
    [SerializeField] private AudioClip m_buttonDenied;
    [SerializeField] private AudioClip m_buttonSelection;

    [SerializeField] private float m_delayBetweenTransitions;

    private AudioSource m_audioScource;

    public AudioClip WinSound { get { return m_winSound; } }
    public AudioClip LossSound { get { return m_lossSound; } }

    public static AudioManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

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

        Debug.Log(clips[0].name);

        yield return new WaitForSeconds(clips[0].length);

        source.clip = clips[1];

        yield return new WaitForSeconds(m_delayBetweenTransitions);        

        source.Play();

        Debug.Log(clips[1].name);

    }
}
