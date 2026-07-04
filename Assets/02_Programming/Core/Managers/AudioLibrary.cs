using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    private struct ClipEntry
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private List<ClipEntry> _bgmClips = new();
    [SerializeField] private List<ClipEntry> _sfxClips = new();

    private Dictionary<string, AudioClip> _bgmMap;
    private Dictionary<string, AudioClip> _sfxMap;

    public AudioClip GetBGM(BGMType type) => GetClip(ref _bgmMap, _bgmClips, type.ToString());

    public AudioClip GetSFX(SFXType type) => GetClip(ref _sfxMap, _sfxClips, type.ToString());

    private static AudioClip GetClip(ref Dictionary<string, AudioClip> map, List<ClipEntry> entries, string name)
    {
        map ??= BuildMap(entries);
        return map.TryGetValue(name, out var clip) ? clip : null;
    }

    private static Dictionary<string, AudioClip> BuildMap(List<ClipEntry> entries)
    {
        var map = new Dictionary<string, AudioClip>();
        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.name) && entry.clip != null)
                map[entry.name] = entry.clip;
        }

        return map;
    }
}
