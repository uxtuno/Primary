using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// シーンごとに使用する音楽を管理するクラスは
/// このクラスを継承する
/// </summary>
public class BaseSoundCollector<T> : MonoBehaviour
{
    // 派生クラス側で定義
    //public enum SoundName
    //{
    //    // BGM
    //    // SE
    //}

    public virtual void Awake()
    {
        // ここで読み込み
        //sounds.Add(SoundName.Title, (AudioClip)Resources.Load("Sounds/道中"));
        //sounds.Add(SoundName.Explosion, (AudioClip)Resources.Load("Sounds/爆発音1"));
    }

    /// <summary>
    /// サウンド名からAudioClipを取得
    /// </summary>
    public AudioClip this[T index]
    {
        get
        {
            if (!sounds.ContainsKey(index))
            {
                return null;
            }

            return sounds[index];
        }
    }

    protected Dictionary<T, AudioClip> sounds = new Dictionary<T, AudioClip>();
}
