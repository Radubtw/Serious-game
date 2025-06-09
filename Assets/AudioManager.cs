using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }
    
    public static AudioManager Instance;
    
    [Header("Sound Effects")]
    public SoundEffect[] soundEffects;
    
    [Header("Music")]
    public AudioClip[] backgroundMusic;
    public float musicVolume = 0.5f;
    public bool playMusicOnStart = true;
    
    private AudioSource musicSource;
    private int currentMusicIndex = 0;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Create audio sources for each sound effect
        foreach (SoundEffect sound in soundEffects)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        
        // Create music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        
        if (playMusicOnStart && backgroundMusic.Length > 0)
            PlayMusic(0);
    }
    
    // Play a sound effect by name
    public void PlaySound(string name)
    {
        SoundEffect sound = System.Array.Find(soundEffects, s => s.name == name);
        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound effect " + name + " not found!");
        }
    }
    
    // Play a sound effect with position
    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        SoundEffect sound = System.Array.Find(soundEffects, s => s.name == name);
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
        }
        else
        {
            Debug.LogWarning("Sound effect " + name + " not found!");
        }
    }
    
    // Stop a sound effect
    public void StopSound(string name)
    {
        SoundEffect sound = System.Array.Find(soundEffects, s => s.name == name);
        if (sound != null)
        {
            sound.source.Stop();
        }
    }
    
    // Play background music by index
    public void PlayMusic(int index)
    {
        if (index < 0 || index >= backgroundMusic.Length)
            return;
            
        currentMusicIndex = index;
        
        if (musicSource.isPlaying)
            musicSource.Stop();
            
        musicSource.clip = backgroundMusic[index];
        musicSource.Play();
    }
    
    // Play next music track
    public void PlayNextMusic()
    {
        currentMusicIndex = (currentMusicIndex + 1) % backgroundMusic.Length;
        PlayMusic(currentMusicIndex);
    }
    
    // Stop music
    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
    
    // Set music volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    // Set sound effects volume (global multiplier)
    public void SetSoundEffectsVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        foreach (SoundEffect sound in soundEffects)
        {
            sound.source.volume = sound.volume * volume;
        }
    }
    
    // Fade in music
    public void FadeInMusic(float duration = 1f)
    {
        StartCoroutine(FadeMusicCoroutine(0f, musicVolume, duration));
    }
    
    // Fade out music
    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeMusicCoroutine(musicVolume, 0f, duration));
    }
    
    // Coroutine for fading music
    private IEnumerator FadeMusicCoroutine(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        
        musicSource.volume = targetVolume;
        
        if (targetVolume <= 0)
            musicSource.Stop();
    }
}