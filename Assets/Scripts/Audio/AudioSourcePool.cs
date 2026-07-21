using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioSource의 잦은 Instantiate/Destroy로 인한 GC 할당 및 성능 저하를 막기 위해
/// Object Pool 패턴을 적용하여 재사용합니다.
/// </summary>
public class AudioSourcePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField, Tooltip("초기에 생성해 둘 AudioSource의 개수")] 
    private int _initialSize = 15;
    
    private AudioMixerGroup _outputGroup;
    private Queue<AudioSource> _pool = new Queue<AudioSource>();

    /// <summary>
    /// 풀을 초기화합니다.
    /// </summary>
    public void Initialize(AudioMixerGroup outputGroup)
    {
        _outputGroup = outputGroup;
        
        for (int i = 0; i < _initialSize; i++)
        {
            CreateNewSource();
        }
    }

    /// <summary>
    /// 새로운 AudioSource 게임 오브젝트를 생성하여 풀에 추가합니다.
    /// </summary>
    private AudioSource CreateNewSource()
    {
        GameObject go = new GameObject("PooledAudioSource");
        go.transform.SetParent(this.transform);
        
        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = _outputGroup;
        
        _pool.Enqueue(source);
        return source;
    }

    /// <summary>
    /// 풀에서 사용 가능한 AudioSource를 가져옵니다. 부족하면 새로 생성합니다.
    /// </summary>
    public AudioSource GetSource()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }
        else
        {
            // 풀이 비어있으면 유연하게 확장
            Debug.LogWarning("AudioSourcePool: 풀 크기를 초과하여 새 AudioSource를 생성합니다. 초기 크기를 늘리는 것을 고려하세요.");
            CreateNewSource();
            return _pool.Dequeue();
        }
    }

    /// <summary>
    /// 사용이 끝난 AudioSource를 초기화하고 풀로 반환합니다.
    /// </summary>
    public void ReturnSource(AudioSource source)
    {
        if (source == null) return;
        
        source.Stop();
        source.clip = null;
        source.pitch = 1f;
        source.volume = 1f;
        source.priority = 128; // Unity 기본값
        
        _pool.Enqueue(source);
    }
}
