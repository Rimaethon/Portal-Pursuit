using System.Collections.Generic;
using System.Linq;
using Managers;
using Scripts.Utility;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameObject audioSourcePrefab;
    private List<AudioSource> sfxSources;
    private AudioClip[] musicClips;
    private AudioClip[] sfxClips;
    private const int initial_sfx_count = 5;

    private void OnEnable()
    {
        EventManager.Subscribe<MusicToggleEventArgs>( HandleMusicToggle);
    }


    private void OnDisable()
    {
        EventManager.UnSubscribe<MusicToggleEventArgs>( HandleMusicToggle);
    }

    protected override void Awake()
    {
        base.Awake();
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < initial_sfx_count; i++)
        {
            GameObject newAudioSourceObject = Instantiate(audioSourcePrefab, transform);
            sfxSources.Add(newAudioSourceObject.GetComponent<AudioSource>());
        }
        musicClips = audioLibrary.MusicClips;
        sfxClips = audioLibrary.SFXClips;
        PlayMusic(MusicClips.Menu);
    }
    private void HandleMusicToggle(MusicToggleEventArgs obj)
    {
        if (SaveManager.Instance.GetUserData().IsMusicOn)
        {
            PlayMusic(MusicClips.Menu);
        }
        else
        {
            musicSource.Stop();
        }
    }

    private void PlayMusic(MusicClips clipEnum)
    {
        if (!SaveManager.Instance.GetUserData().IsMusicOn) return;
        if (musicSource.isPlaying) musicSource.Stop();
        musicSource.clip = musicClips[(int)clipEnum];
        musicSource.Play();
    }

    public AudioSource PlaySFX(SFXClips clipEnum, bool isLooping = false)
    {
        if (!SaveManager.Instance.GetUserData().IsSfxOn) return null;

        AudioSource playingSource = sfxSources.FirstOrDefault(source => source.isPlaying && source.clip == sfxClips[(int)clipEnum]);
        if (playingSource != null)
        {
            return playingSource;
        }
        AudioSource availableSource = sfxSources.FirstOrDefault(source => !source.isPlaying);

        // If there is no available AudioSource, create a new one
        if (availableSource == null)
        {
            GameObject newAudioSourceObject = Instantiate(audioSourcePrefab, transform);
            availableSource = newAudioSourceObject.GetComponent<AudioSource>();
            sfxSources.Add(availableSource);
        }
        availableSource.clip = sfxClips[(int)clipEnum];
        availableSource.loop = isLooping;
        if(isLooping)
        {
            availableSource.Play();
        }
        else
        {
            availableSource.PlayOneShot(availableSource.clip);
        }
        return availableSource;
    }

}
