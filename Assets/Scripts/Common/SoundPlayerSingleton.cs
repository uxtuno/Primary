using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundPlayerSingleton : MyMonoBehaviour
{
	private static bool isCreated = false; // 複数生成防止

	/// <summary>
	/// 唯一のインスタンスを取得
	/// </summary>
	public static SoundPlayerSingleton instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject soundPlayer = new GameObject("SoundPlayerSingleton");
				_instance = soundPlayer.AddComponent<SoundPlayerSingleton>();
				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;
		}
	}
	private static SoundPlayerSingleton _instance;
	// 実質外部からのコンストラクタ呼び出しは不可
	private SoundPlayerSingleton() { }

	private const float maxVolume = 1f;                         // 最大音量
	private const float minVolume = 0f;                         // 最小音量
	private GameObject bgmPlayer;                               // BGMを鳴らすためのオブジェクト
	private bool isBGMFade = false;                             // BGMがフェード中かどうか
	private float bgmVolume = 1f;                               // 現在再生中のBGMの音量
	private float bgmAttenuation = 0f;                          // BGMのフェードアウト時の減衰量(実際はTime.deltaTimeを乗算する。マイナスの数値を入れることでフェードインにも使用)

	//private GameObject sePlayer;                                // SEを鳴らすためのオブジェクト
	private AudioSource seAudio;                                // SEPlayerのAudioSouceリスト
	private AudioSource fadeSEAudio = null;                     // フェード中のSE
	private float initSEVolume = 1.0f;                          // SEの初期音量
	private bool isSEFade = false;                              // SEがフェード中かどうか
	private float seAttenuation = 0f;                           // SEのフェードアウト時の減衰量(実際はTime.deltaTimeを乗算する。マイナスの数値を入れることでフェードインにも使用)
	private List<float> seTimeLeft = new List<float>();         // SEの再生が終わるまでの時間。再生が終われば要素を削除
	private const float threshold = 0.2f;                       // SEの同時再生カウント用閾値
	private float multipleCount = 0f;
	private const float volumePolyphony = 2.8f;                 // SE同時再生時の音量減算割合計算用

	protected override void Awake()
	{
		base.Awake();

		if(isCreated)
		{
			Destroy(gameObject);
			return;
		}

		isCreated = true;
	}
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (isBGMFade)
		{
			bgmVolume -= bgmAttenuation * Time.deltaTime;

			if (bgmVolume < minVolume)
			{
				isBGMFade = false;
				bgmVolume = minVolume;
				Destroy(bgmPlayer);
				return;
			}
			else if (bgmVolume > maxVolume)
			{
				isBGMFade = false;
				bgmVolume = maxVolume;
			}

			bgmPlayer.GetComponent<AudioSource>().volume = bgmVolume;
		}

		if (isSEFade && fadeSEAudio != null)
		{
			fadeSEAudio.volume -= seAttenuation * Time.deltaTime;

			if (fadeSEAudio.volume <= minVolume)
			{
				isSEFade = false;
				seAttenuation = 0.0f;
				fadeSEAudio.Stop();
				fadeSEAudio.volume = initSEVolume;
				return;
			}
			else if (fadeSEAudio.volume >= maxVolume)
			{
				isSEFade = false;
				fadeSEAudio.volume = maxVolume;
			}
		}

		if (multipleCount <= 0f)
		{
			multipleCount = 0f;
		}
		else
		{
			multipleCount -= Time.deltaTime;
		}

		List<float> LeaveItem = new List<float>();
		for (int i = 0; i < seTimeLeft.Count; i++)
		{
			seTimeLeft[i] -= Time.deltaTime;
			if (seTimeLeft[i] > threshold)
			{
				// 残す要素を記録
				LeaveItem.Add(seTimeLeft[i]);
			}
		}

		// 残った要素をそのまま代入
		seTimeLeft = LeaveItem;
	}

	public void PlayBGM(AudioClip clip)
	{
		PlayBGM(clip, 0.0f);
	}

	/// <summary>
	/// BGMを再生する
	/// 複数のBGMの同時再生には未対応
	/// </summary>
	/// <param name="clip">音楽データ</param>
	/// <param name="fadeTime">フェードインで最大音量に達するまでの時間。0 = そのまま再生</param>
	public void PlayBGM(AudioClip clip, float fadeTime)
	{
		// 再生中ならなにもしない
		if (bgmPlayer != null)
		{
			return;
		}

		if (clip == null)
		{
			return;
		}


		bgmPlayer = new GameObject("BGMPlayer");
		AudioSource audio = bgmPlayer.AddComponent<AudioSource>();
		audio.clip = clip;
		audio.loop = true;
		bgmPlayer.transform.parent = transform;

		if (fadeTime > 0.0f)
		{
			// フェードインが有効
			isBGMFade = true;
			bgmAttenuation = -(1.0f / fadeTime);
			audio.volume = minVolume;
			bgmVolume = audio.volume;
		}
		else
		{
			isBGMFade = false;
			bgmAttenuation = 0.0f;
			audio.volume = maxVolume;
		}

		bgmVolume = audio.volume;
		audio.Play();
	}

	public void StopBGM(float fadeTime)
	{
		if (bgmPlayer == null || !bgmPlayer.GetComponent<AudioSource>().isPlaying)
		{
			return;
		}

		// 補間なしなので即Destroy
		if (fadeTime == 0.0f)
		{
			Destroy(bgmPlayer);
			return;
		}

		isBGMFade = true;
		bgmAttenuation = 1.0f / fadeTime;
	}

	/// <summary>
	/// SEを再生する
	/// </summary>　
	/// <param name="target">参照するGameObject</param>
	/// <param name="clip">サウンドデータ</param>
	/// <param name="isLoop">ループ再生するかどうか</param>
	/// <param name="isUse3DSound">3DSoundを使用するかどうか</param>
	/// <param name="volume">音量(0.0f～1.0f)</param>
	/// <param name="fadeTime">フェードインで最大音量に達するまでの時間。0 = そのまま再生</param>
	/// <param name="isAnythingVolume">指定したままの音で再生</param>
	public void PlaySE(GameObject target, AudioClip clip, bool isLoop = false, bool isUse3DSound = true, float volume = maxVolume, float fadeTime = 0.0f, bool isAnythingVolume = false)
	{
		if (clip == null)
		{
			return;
		}

		// 前回再生時のフェード中の場合フェードを中止する
		if (fadeSEAudio != null && isSEFade)
		{
			isSEFade = false;
			seAttenuation = 0.0f;
			fadeSEAudio.volume = initSEVolume;
		}

		// 使用していないAudioSouceを削除する
		foreach (AudioSource audio in target.GetComponents<AudioSource>())
		{
			if (audio.clip == null)
			{
				Destroy(audio);
			}
		}

		initSEVolume = volume;
		if (!isAnythingVolume)
		{
			initSEVolume = volume - (volume / volumePolyphony);
			if (multipleCount > 0f)
			{
				initSEVolume = initSEVolume - (volume / volumePolyphony);
			}
		}
		else
		{
			initSEVolume = volume;
		}

		// 対象にAudioSourceがアタッチされていなければAdd、アタッチされていればGetする
		if (target.GetComponent<AudioSource>() == null || !SearchAudioSource(target.GetComponents<AudioSource>(), clip))
		{
			seAudio = target.AddComponent<AudioSource>();
		}

		if (isUse3DSound)
		{
			seAudio.spatialBlend = 1.0f;
			seAudio.minDistance = 2.5f;
		}

		// ループが有効な場合はPlay()そうでない場合はPlayOneShot()を使用する
		if (isLoop)
		{
			seAudio.clip = clip;
			seAudio.volume = initSEVolume;
			seAudio.loop = true;
			seAudio.Play();
		}
		else
		{
			seAudio.PlayOneShot(volumeScale: initSEVolume, clip: clip);
			seAudio.PlayOneShot(clip, initSEVolume);
		}

		// フェードイン
		if (fadeTime > 0.0f)
		{
			seAudio.volume = minVolume;
			fadeSEAudio = seAudio;
			isSEFade = true;
			seAttenuation = -(1.0f / fadeTime);
		}

		seTimeLeft.Add(clip.length);
		multipleCount = threshold;
	}

	/// <summary>
	/// SEを停止する
	/// ループ再生していないSEの停止には未対応
	/// </summary>
	/// <param name="target">参照するGameObject</param>
	/// <param name="fadeTime">フェードアウトで停止するまでの時間。0 = 即停止</param>
	public void StopSE(GameObject target, float fadeTime)
	{
		if (target.GetComponent<AudioSource>() == null)
		{
			return;
		}

		fadeSEAudio = target.GetComponent<AudioSource>();
		if (target == null || !fadeSEAudio.isPlaying)
		{
			return;
		}


		// 補間無しならStop
		if (fadeTime <= 0.0f)
		{
			fadeSEAudio.Stop();
		}
		else
		{
			isSEFade = true;
			seAttenuation = 1.0f / fadeTime;
		}
	}

	public void ChangeVolume(float volume = maxVolume)
	{
		if (bgmPlayer == null || !bgmPlayer.GetComponent<AudioSource>().isPlaying)
		{
			return;
		}

		bgmPlayer.GetComponent<AudioSource>().volume = volume;
	}


	void OnLevelWasLoaded()
	{
		if (bgmPlayer != null)
		{
			Destroy(bgmPlayer.gameObject);
		}
		bgmPlayer = null;
	}

	/// <summary>
	/// 対象となるAudioSourceの配列の中から、
	/// 目的のAudioClipが登録されているAudioSourceを検索し、seAudioに代入する
	/// </summary>
	/// <param name="audios">対象となるAudioSourceの配列</param>
	/// <param name="clip">検索するAudioClip</param>
	private bool SearchAudioSource(AudioSource[] audios, AudioClip clip)
	{
		bool result = false;
		seAudio = null;

		foreach (AudioSource audio in audios)
		{
			if (audio.clip == clip)
			{
				seAudio = audio;
				result = true;
				break;
			}
		}

		return result;
	}
}
