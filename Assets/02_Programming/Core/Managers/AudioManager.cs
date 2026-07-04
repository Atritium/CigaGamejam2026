using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    internal static AudioLibrary PendingLibrary;

    private AudioLibrary _library;
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    protected override void OnSingletonAwake()
    {
        _library = PendingLibrary;
        PendingLibrary = null;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
    }

    public void PlayMusic(BGMType type, bool loop = true)
    {
        _musicSource.clip = _library.GetBGM(type);
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void PlaySFX(SFXType type)
    {
        _sfxSource.PlayOneShot(_library.GetSFX(type));
    }
}
