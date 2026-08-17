using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _systemSource;
    private List<AudioSource> _activeSources = new List<AudioSource>();
    
    [SerializeField] private float defaultMinDistance = 1f;
    [SerializeField] private float defaultMaxDistance = 500f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _systemSource = GetComponent<AudioSource>();
        if (_systemSource == null)
        {
            _systemSource = gameObject.AddComponent<AudioSource>();
            _systemSource.playOnAwake = false;
            _systemSource.spatialBlend = 0f; // 2D
        }
    }

    #region 2D Sound Controls (systemSource)

    // Toca um clip em loop (ou não, dependendo do parâmetro)
    public void Play2D(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;
        _systemSource.Stop();
        _systemSource.clip = clip;
        _systemSource.volume = volume;
        _systemSource.loop = loop;
        _systemSource.spatialBlend = 0f;
        _systemSource.Play();
    }

    public void Pause2D()
    {
        if (_systemSource.isPlaying)
            _systemSource.Pause();
    }

    public void Resume2D()
    {
        if (_systemSource.clip != null && !_systemSource.isPlaying)
            _systemSource.UnPause();
    }

    public void Stop2D()
    {
        _systemSource.Stop();
        _systemSource.clip = null;
    }

    // Versão one-shot para 2D (não altera o clip principal)
    public void Play2DOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _systemSource.spatialBlend = 0f;
        _systemSource.PlayOneShot(clip, volume);
    }

    #endregion

    #region 3D Sound Controls (activeSources)
    public AudioSource Play3D(AudioClip clip, Vector3 position, float volume = 1f, bool loop = true)
    {
        if (clip == null) return null;

        GameObject go = new GameObject("3DSound_" + clip.name);
        go.transform.position = position;
        go.transform.parent = this.transform;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;
        src.volume = volume;
        src.loop = loop;
        src.minDistance = defaultMinDistance;
        src.maxDistance = defaultMaxDistance;
        src.playOnAwake = false;
        src.Play();

        _activeSources.Add(src);

        if (!loop)
        {
            StartCoroutine(CleanupAfterPlayback(src));
        }

        return src;
    }
    
        public void Pause3D(AudioSource source)
    {
        if (source == null) return;
        if (source.isPlaying) source.Pause();
    }

    public void Resume3D(AudioSource source)
    {
        if (source == null) return;
        if (source.clip != null && !source.isPlaying) source.UnPause();
    }

        public void Stop3D(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        _activeSources.Remove(source);
        if (source.gameObject != null)
            Destroy(source.gameObject);
    }
        
    public void Play3DOneShot(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    
        private IEnumerator CleanupAfterPlayback(AudioSource src)
    {
        if (src == null) yield break;
        yield return new WaitForSecondsRealtime(src.clip != null ? src.clip.length : 0f);
        if (src == null) yield break;
        _activeSources.Remove(src);
        if (src.gameObject != null)
            Destroy(src.gameObject);
    }
        
        public void StopAll3D()
    {
        foreach (var src in new List<AudioSource>(_activeSources))
        {
            if (src == null) continue;
            src.Stop();
            if (src.gameObject != null) Destroy(src.gameObject);
        }
        _activeSources.Clear();
    }

    #endregion
}
