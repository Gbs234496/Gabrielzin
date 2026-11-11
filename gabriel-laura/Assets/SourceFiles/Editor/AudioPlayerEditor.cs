using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
    private SerializedProperty clipsProp;
    private SerializedProperty selectedIndexProp;
    private SerializedProperty audioSourcesProp;
    private SerializedProperty selectedSourceIndexProp;
    private SerializedProperty useAudioSourceProp;

    private void OnEnable()
    {
        clipsProp = serializedObject.FindProperty("clips");
        selectedIndexProp = serializedObject.FindProperty("selectedIndex");
        audioSourcesProp = serializedObject.FindProperty("audioSources");
        selectedSourceIndexProp = serializedObject.FindProperty("selectedSourceIndex");
        useAudioSourceProp = serializedObject.FindProperty("useAudioSource");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(clipsProp, true);

        // Build popup options from clips
        int currentIndex = selectedIndexProp.intValue;
        string[] options;
        int clipsCount = clipsProp.arraySize;
        if (clipsCount == 0)
        {
            options = new string[] { "<no clips>" };
            currentIndex = 0;
        }
        else
        {
            options = new string[clipsCount];
            for (int i = 0; i < clipsCount; i++)
            {
                var clipProp = clipsProp.GetArrayElementAtIndex(i);
                var clip = clipProp.objectReferenceValue as AudioClip;
                options[i] = clip != null ? string.Format("{0}: {1}", i, clip.name) : string.Format("{0}: <null>", i);
            }
            currentIndex = Mathf.Clamp(currentIndex, 0, clipsCount - 1);
        }

        int newIndex = EditorGUILayout.Popup("Selected Clip Index", currentIndex, options);
        selectedIndexProp.intValue = newIndex;

        EditorGUILayout.Space();

        // AudioSource selection section
        EditorGUILayout.PropertyField(useAudioSourceProp, new GUIContent("Use AudioSource"));
        EditorGUILayout.PropertyField(audioSourcesProp, new GUIContent("AudioSources"), true);

        int sourceCount = audioSourcesProp.arraySize;
        int currentSourceIndex = selectedSourceIndexProp.intValue;
        string[] sourceOptions;
        if (sourceCount == 0)
        {
            sourceOptions = new string[] { "<no sources>" };
            currentSourceIndex = 0;
        }
        else
        {
            sourceOptions = new string[sourceCount];
            for (int i = 0; i < sourceCount; i++)
            {
                var srcProp = audioSourcesProp.GetArrayElementAtIndex(i);
                var src = srcProp.objectReferenceValue as AudioSource;
                sourceOptions[i] = src != null ? string.Format("{0}: {1}", i, src.gameObject.name) : string.Format("{0}: <null>", i);
            }
            currentSourceIndex = Mathf.Clamp(currentSourceIndex, 0, sourceCount - 1);
        }

        int newSourceIndex = EditorGUILayout.Popup("Selected Source Index", currentSourceIndex, sourceOptions);
        selectedSourceIndexProp.intValue = newSourceIndex;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            PlayOnTargets(newIndex);
        }
        if (GUILayout.Button("Stop"))
        {
            StopOnTargets();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Pause"))
        {
            PauseOnTargets();
        }
        if (GUILayout.Button("Resume"))
        {
            ResumeOnTargets();
        }
        EditorGUILayout.EndHorizontal();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play/Stop/Pause/Resume buttons are available only in Play Mode.", MessageType.Info);
        }

        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

    private void PlayOnTargets(int index)
    {
        foreach (var t in targets)
        {
            var ap = t as AudioPlayer;
            if (ap == null) continue;
            ap.Play(index);
        }
    }

    private void StopOnTargets()
    {
        foreach (var t in targets)
        {
            var ap = t as AudioPlayer;
            if (ap == null) continue;
            ap.Stop();
        }
    }

    private void PauseOnTargets()
    {
        foreach (var t in targets)
        {
            var ap = t as AudioPlayer;
            if (ap == null) continue;
            ap.Pause();
        }
    }

    private void ResumeOnTargets()
    {
        foreach (var t in targets)
        {
            var ap = t as AudioPlayer;
            if (ap == null) continue;
            ap.Resume();
        }
    }
}


