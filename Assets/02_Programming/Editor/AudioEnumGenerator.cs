using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AudioEnumGenerator
{
    private const string ArtBGMPath = "Assets/03_Art/Audio/BGM";
    private const string ArtSFXPath = "Assets/03_Art/Audio/SFX";
    private const string LibraryPath = "Assets/04_Shared/ScriptableObjects/AudioLibrary.asset";
    private const string GeneratedFilePath = "Assets/02_Programming/Core/Managers/AudioEnums.generated.cs";

    [MenuItem("Tools/Audio/Generate Audio Enums")]
    public static void Generate()
    {
        EnsureFolder("Assets/03_Art/Audio");
        EnsureFolder(ArtBGMPath);
        EnsureFolder(ArtSFXPath);
        EnsureFolder("Assets/04_Shared/ScriptableObjects");

        var bgmClips = CollectClips(ArtBGMPath);
        var sfxClips = CollectClips(ArtSFXPath);

        WriteGeneratedFile(
            bgmClips.Select(entry => entry.name).ToList(),
            sfxClips.Select(entry => entry.name).ToList());
        UpdateLibrary(bgmClips, sfxClips);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static List<(string name, AudioClip clip)> CollectClips(string artPath)
    {
        var results = new List<(string name, AudioClip clip)>();
        var guids = AssetDatabase.FindAssets("t:AudioClip", new[] { artPath });

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var enumName = ToEnumName(Path.GetFileNameWithoutExtension(assetPath));
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (string.IsNullOrEmpty(enumName) || clip == null)
                continue;

            results.Add((enumName, clip));
        }

        return results.OrderBy(entry => entry.name).ToList();
    }

    private static void UpdateLibrary(
        IReadOnlyList<(string name, AudioClip clip)> bgmClips,
        IReadOnlyList<(string name, AudioClip clip)> sfxClips)
    {
        var library = AssetDatabase.LoadAssetAtPath<AudioLibrary>(LibraryPath);
        if (library == null)
        {
            library = ScriptableObject.CreateInstance<AudioLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
        }

        var serializedLibrary = new SerializedObject(library);
        SetClipEntries(serializedLibrary.FindProperty("_bgmClips"), bgmClips);
        SetClipEntries(serializedLibrary.FindProperty("_sfxClips"), sfxClips);
        serializedLibrary.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(library);
    }

    private static void SetClipEntries(
        SerializedProperty entriesProperty,
        IReadOnlyList<(string name, AudioClip clip)> clips)
    {
        entriesProperty.ClearArray();

        for (var i = 0; i < clips.Count; i++)
        {
            entriesProperty.InsertArrayElementAtIndex(i);
            var element = entriesProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("name").stringValue = clips[i].name;
            element.FindPropertyRelative("clip").objectReferenceValue = clips[i].clip;
        }
    }

    private static string ToEnumName(string fileName)
    {
        var builder = new StringBuilder();

        foreach (var c in fileName)
        {
            if (char.IsLetterOrDigit(c))
                builder.Append(c);
            else if (c is ' ' or '-' or '_')
                builder.Append('_');
        }

        var result = builder.ToString();
        if (string.IsNullOrEmpty(result))
            return string.Empty;

        if (char.IsDigit(result[0]))
            result = "_" + result;

        return result;
    }

    private static void WriteGeneratedFile(IReadOnlyList<string> bgmNames, IReadOnlyList<string> sfxNames)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// 此文件由 Tools/Audio/Generate Audio Enums 自动生成，请勿手动编辑。");
        builder.AppendLine();
        builder.AppendLine(GenerateEnum("BGMType", bgmNames));
        builder.AppendLine();
        builder.AppendLine(GenerateEnum("SFXType", sfxNames));
        builder.AppendLine();

        File.WriteAllText(GeneratedFilePath, builder.ToString(), Encoding.UTF8);
    }

    private static string GenerateEnum(string enumName, IReadOnlyList<string> memberNames)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"public enum {enumName}");
        builder.AppendLine("{");

        if (memberNames.Count == 0)
        {
            builder.AppendLine("    None,");
        }
        else
        {
            foreach (var memberName in memberNames)
                builder.AppendLine($"    {memberName},");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        var folderName = Path.GetFileName(path);

        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(folderName))
            return;

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folderName);
    }
}
