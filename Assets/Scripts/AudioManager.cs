using System.Collections;
using System.IO;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public EventInstance musicEventInstance;

    [Header("Music")]
    public EventReference gameplayMusic;
    public EventReference titleScreenMusic;

    [Header("SFX")]
    public EventReference moo;
    public EventReference uiClick;
    public EventReference uiHover;
    public EventReference collect;
    public EventReference metal;
    public EventReference rock;
    public EventReference grapple;
    public EventReference shipHit;

    [Header("Volume")]
    [Range(0, 1)] public float masterVolume { get; set; }
    [Range(0, 1)] public float sfxVolume { get; set; }
    [Range(0, 1)] public float musicVolume { get; set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    [HideInInspector] public bool areBussesInitialized = false;
    private bool isMusicPlaying = false;

    private const float DEFAULT_VOLUME = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadBusses());
    }

    void Update()
    {
        if (areBussesInitialized)
        {
            masterBus.setVolume(masterVolume);
            sfxBus.setVolume(sfxVolume);
            musicBus.setVolume(musicVolume);
        }
    }

    public IEnumerator LoadBusses()
    {
        yield return new WaitUntil(() => RuntimeManager.HaveAllBanksLoaded);

        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");

        if (!areBussesInitialized)
        {
            masterVolume = DEFAULT_VOLUME;
            sfxVolume = DEFAULT_VOLUME;
            musicVolume = DEFAULT_VOLUME;
        }

        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);
        musicBus.setVolume(musicVolume);

        areBussesInitialized = true;
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(eventReference);
        return newInstance;
    }

    public void StopMusic()
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isMusicPlaying = false;
    }

    public void PauseUnpause()
    {
        musicEventInstance.getPaused(out bool isMusicPaused);
        musicEventInstance.setPaused(!isMusicPaused);
    }

    public void PlayMusic(EventReference music)
    {
        StartCoroutine(InitializeMusic(music));
    }

    private IEnumerator InitializeMusic(EventReference music)
    {
        yield return new WaitUntil(() => areBussesInitialized);

        if (isMusicPlaying) StopMusic();
        musicEventInstance = CreateEventInstance(music);
        musicEventInstance.start();
        isMusicPlaying = true;
    }
}
