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
    public float ButtonSoundVolume;

    private AudioSource m_audioScource;

    public AudioClip WinSound { get { return m_winSound; } }
    public AudioClip LossSound { get { return m_lossSound; } }

    public AudioClip ButtonSelected { get { return m_buttonSelected; } }

    public AudioClip ButtonDenied { get { return m_buttonDenied; } }

    public AudioClip ButtonBack { get { return m_buttonBack; } }

    public AudioClip ButtonSelection { get { return m_buttonSelection; } }


    public static AudioManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        if (AbilityButtonManager.Instance != null)
        {
            AbilityButtonManager.Instance.OnEndTurnButtonPress += PlayButtonPressSound;
        }

        m_audioScource = GetComponent<AudioSource>();

        m_audioScource.Stop();

        m_audioScource.loop = true;

        m_audioScource.ignoreListenerPause = true;

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

        yield return new WaitForSecondsRealtime(clips[0].length);

        source.clip = clips[1];

        source.Play();

    }

    public void PlayButtonBackSound()
    {
        m_audioScource.PlayOneShot(m_buttonBack, ButtonSoundVolume);
    }

    public void PlayButtonPressSound()
    {
        m_audioScource.PlayOneShot(m_buttonSelected, ButtonSoundVolume);
    }

    public void PlayButtonHighlightSound()
    {
        m_audioScource.PlayOneShot(m_buttonSelection, ButtonSoundVolume - 0.1f);
    }

    public void PlayButtonDeniedSound()
    {
        m_audioScource.PlayOneShot(m_buttonDenied, ButtonSoundVolume + 0.2f);
    }
}
