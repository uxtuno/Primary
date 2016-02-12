using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 特定の名前から
/// 対応するAudioClipを取得する
/// </summary>
public class SoundCollector : BaseSoundCollector<SoundCollector.SoundName>
{
	public enum SoundName
	{
		// Other
		NONE,

		// Title
		Cancel,             // キャンセル音
		CursorSelect,       // カーソルの移動音
		Decision,           // 決定音

		// Main
		CardKeyFalse,       // カードキー認識失敗音
		CardKeyTrue,        // カードキー認識成功音
		CubeElase,          // 消えるオブジェクトの消失音
		CubeRestoration,    // 消えるオブジェクトの復元音
		Door,               // ドアの駆動音
		Driving1,           // ギミックの駆動音
		Driving2,           // ギミックの駆動音
		Driving3,           // ギミックの駆動音
		Driving4,           // ギミックの駆動音
		Driving5,           // ギミックの駆動音
		Fall1,              // 衝突・落下音1
		Fall2,              // 衝突・落下音2
		Fall3,              // 衝突・落下音3
		ItemGet,            // アイテム拾得音
		Jump,               // ジャンプ音
		LaserClarity,       // レーザー照射音
		LaserHigh,          // レーザー照射音
		LaserLow,           // レーザー照射音
		LensReplacement,    // レンズ切り替え音
		Pulley,             // 滑車の駆動音
		StepNomal,          // 足音(通常時)
		StepMetal,          // 足音(金属の上歩行時)
		Switch,             // スイッチの駆動音
		WarpIn,             // ワープ開始音
		WarpOut             // ワープ終了音
	}

	public override void Awake()
	{
		// Title
		sounds.Add(SoundName.Cancel, Resources.Load<AudioClip>("Sounds/Cancel"));
		sounds.Add(SoundName.CursorSelect, Resources.Load<AudioClip>("Sounds/CursorSelect2"));
		sounds.Add(SoundName.Decision, Resources.Load<AudioClip>("Sounds/Decision"));

		// Main
		sounds.Add(SoundName.CardKeyFalse, Resources.Load<AudioClip>("Sounds/CardKeyFalse"));
		sounds.Add(SoundName.CardKeyTrue, Resources.Load<AudioClip>("Sounds/CardKeyTrue"));
		sounds.Add(SoundName.CubeElase, Resources.Load<AudioClip>("Sounds/CubeElase"));
		sounds.Add(SoundName.CubeRestoration, Resources.Load<AudioClip>("Sounds/CubeRestoration"));
		sounds.Add(SoundName.Door, Resources.Load<AudioClip>("Sounds/Door"));
		sounds.Add(SoundName.Driving1, Resources.Load<AudioClip>("Sounds/Driving1"));
		sounds.Add(SoundName.Driving2, Resources.Load<AudioClip>("Sounds/Driving2"));
		sounds.Add(SoundName.Driving3, Resources.Load<AudioClip>("Sounds/Driving3"));
		sounds.Add(SoundName.Driving4, Resources.Load<AudioClip>("Sounds/Driving4"));
		sounds.Add(SoundName.Driving5, Resources.Load<AudioClip>("Sounds/Driving5"));
		sounds.Add(SoundName.Fall1, Resources.Load<AudioClip>("Sounds/Fall(Light)"));
		sounds.Add(SoundName.Fall2, Resources.Load<AudioClip>("Sounds/Fall(Medium)"));
		sounds.Add(SoundName.Fall3, Resources.Load<AudioClip>("Sounds/Fall(Medium)2"));
		sounds.Add(SoundName.ItemGet, Resources.Load<AudioClip>("Sounds/ItemGet"));
		sounds.Add(SoundName.Jump, Resources.Load<AudioClip>("Sounds/Jump"));
		sounds.Add(SoundName.LaserClarity, Resources.Load<AudioClip>("Sounds/Laser(Clarity)"));
		sounds.Add(SoundName.LaserHigh, Resources.Load<AudioClip>("Sounds/Laser(High)"));
		sounds.Add(SoundName.LaserLow, Resources.Load<AudioClip>("Sounds/Laser(Low)"));
		sounds.Add(SoundName.LensReplacement, Resources.Load<AudioClip>("Sounds/LensReplacement"));
		sounds.Add(SoundName.Pulley, Resources.Load<AudioClip>("Sounds/Pulley"));
		sounds.Add(SoundName.StepNomal, Resources.Load<AudioClip>("Sounds/Step1"));
		sounds.Add(SoundName.StepMetal, Resources.Load<AudioClip>("Sounds/Step(Metal)"));
		sounds.Add(SoundName.Switch, Resources.Load<AudioClip>("Sounds/Switch"));
		sounds.Add(SoundName.WarpIn, Resources.Load<AudioClip>("Sounds/WarpIn"));
		sounds.Add(SoundName.WarpOut, Resources.Load<AudioClip>("Sounds/WarpOut"));
	}
}
