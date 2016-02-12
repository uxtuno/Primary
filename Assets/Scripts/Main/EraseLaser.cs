using UnityEngine;
using System.Collections;

public class EraseLaser : MyMonoBehaviour
{
	public GameObject hitPointLightPrefab = null;
	private GameObject hitPointLight = null; // レーザーの当たった地点に表示するライト
	[SerializeField]
	private float length = 20.0f;   // レーザーの長さ
	[SerializeField]
	private ObjectColor _color = null; // レーザーの色

	/// <summary>
	/// レーザーの色
	/// </summary>
	public ObjectColor color
	{
		get
		{
			return _color;
		}

		set
		{
			// 反射レーザーも一度に変更する
			_color = value;
			foreach (Renderer item in transform.GetComponentsInChildren<Renderer>())
			{
				item.material.color = (Color)value;
			}

			if (hitPointLight != null)
			{
				hitPointLight.GetComponent<Light>().color = (Color)value;
			}

			if (reflectedLaser != null)
			{
				reflectedLaser.color = value;
			}
		}
	}

	[SerializeField]
	private GameObject reflectedLaserPrefab = null;
	private EraseLaser reflectedLaser = null; // 反射レーザー
	private static readonly string originalName = "EraseLaser";
	[SerializeField]
	private int reflectCount = 10; // あと何かい反射できるか

	protected override void Awake()
	{
		base.Awake();

		color = _color; // setterを呼ぶためにあえてこの書き方。設計を見直した方が良いかも知れない

		name = originalName;
	}

	/// <summary>
	/// 対象地点へ照射
	/// 目標地点との間に障害物があれば遮られる
	/// </summary>
	/// <param name="point">目標地点</param>
	public bool IrradiationToTarget(Vector3 point)
	{
		bool isHit = false;
		RaycastHit hit;
		transform.LookAt(point);
		int layerMask = ~((1 << Layers.Leaser.layer) | (1 << Layers.IgnoreRaycast.layer));
		if (Physics.Raycast(transform.position, transform.forward, out hit, length, layerMask))
		{
			isHit = true;

			if (Application.isPlaying)
			{
				if (hitPointLight == null)
				{
					hitPointLight = (GameObject)Instantiate(hitPointLightPrefab, Vector3.zero, Quaternion.identity);
					hitPointLight.transform.parent = transform.parent;
					hitPointLight.GetComponent<Light>().color = (Color)color;
				}

				// 命中点に明かりを表示する
				hitPointLight.transform.position = hit.point - transform.forward * 0.2f;
				hitPointLight.transform.forward = transform.forward;
			}

			// 命中した地点に合わせてレーザーの形状を変更
			transform.localScale = new Vector3(1.0f, 1.0f, hit.distance + 0.001f);

			ColorObjectBase colorObject = hit.transform.GetComponent<ColorObjectBase>();

			// 命中した地点がカラーオブジェクトなら
			if (colorObject != null)
			{
				// 照射メッセージを送る
				colorObject.Irradiated(color);

				// 白い箱なら反射
				if (colorObject.objectColor.state == ColorState.WHITE && reflectCount > 0)
				{
					if (reflectedLaser == null)
					{
						Reflected();
						reflectedLaser.reflectCount--;
					}

					Vector3 r = Vector3.Reflect(transform.forward, hit.normal);
					reflectedLaser.transform.position = hit.point;
					reflectedLaser.transform.forward = r;
					reflectedLaser.length = length - hit.distance;
					if (reflectedLaser.reflectCount <= 0)
					{
						reflectedLaser.reflectCount = 0;
					}
					reflectedLaser.IrradiationToTarget(hit.point + r * length);
				}
				else
				{
					// 反射しないので反射レーザーが生成されていれば削除
					if (reflectedLaser != null)
					{
						Destroy(reflectedLaser.gameObject);
					}
				}
			}
			else
			{
				// 反射しないので反射レーザーが生成されていれば削除
				if (reflectedLaser != null)
				{
					Destroy(reflectedLaser.gameObject);
				}
			}
		}
		else
		{
			transform.localScale = new Vector3(1.0f, 1.0f, length);

			// そもそも当たってないので色々削除
			if (Application.isPlaying)
			{
				Destroy(hitPointLight);
			}

			if (reflectedLaser != null)
			{
				Destroy(reflectedLaser.gameObject);
			}
		}

		return isHit;
	}

	/// <summary>
	/// 現在の角度で照射
	/// </summary>
	public void Irradiation()
	{
		IrradiationToTarget(transform.forward * length + transform.position);
	}

	/// <summary>
	/// 反射レーザーを生成
	/// </summary>
	private void Reflected()
	{
		GameObject go = Instantiate(reflectedLaserPrefab);
		go.transform.parent = transform.parent;
		reflectedLaser = go.GetComponent<EraseLaser>();
	}

	/// <summary>
	/// 反射先レーザーを全て削除
	/// </summary>
	void OnDestroy()
	{
		if (hitPointLight != null)
		{
			Destroy(hitPointLight.gameObject);
		}

		if (reflectedLaser != null)
		{
			Destroy(reflectedLaser.gameObject);
		}
	}

	/// <summary>
	/// 表示状態
	/// </summary>
	public override bool IsShow
	{
		get
		{
			return base.IsShow;
		}

		set
		{
			base.IsShow = value;

			if (hitPointLight != null)
			{
				hitPointLight.GetComponent<Light>().enabled = value;
			}

			if (reflectedLaser != null)
			{
				reflectedLaser.IsShow = value;
			}
		}
	}
}
