using UnityEngine;
using System.Collections.Generic;

//[RequireComponent(typeof(CharacterController))]

public class Player : MyMonoBehaviour
{
	private SoundCollector soundCollector = null;

	[SerializeField]
	private GameObject explosionPrefab = null;

	private Animator anim = null;
	[SerializeField]
	private float speed = 3.0f; // 移動速度
	private int hashSpeed;
	private CharacterController characterController = null;
	private EraseLaser eraseLaser = null;
	//private ColorState currentLaser = ColorState.RED;

	/// <summary>
	/// レーザーの色と状態
	/// </summary>
	private struct PlayerLaser
	{
		public ColorState color; // 色
		public bool isPossible; // 使用可能か
	}

	private static readonly int laserMax = 3; // プレイヤーが使用可能なレーザーの種類
	private PlayerLaser[] lasers = new PlayerLaser[laserMax]; // プレイヤーが使用可能なレーザー
	[SerializeField]
	private bool isRedLaser = true;
	[SerializeField]
	private bool isGreenLaser = false;
	[SerializeField]
	private bool isBlueLaser = false;

	private int currentLaserIndex; // 現在使用中のレーザー

	private bool _isLaserPossible = true;
	/// <summary>
	/// レーザーが使用可能か
	/// </summary>
	public bool isLaserPossible
	{
		get { return _isLaserPossible; }
		set
		{
			_isLaserPossible = value;
			if (value == false)
			{
				if (eraseLaser != null)
				{
					Destroy(eraseLaser.gameObject);
				}
			}
		}
	}

	public bool isIrradiation
	{
		get { return eraseLaser != null; }
	}

	private bool _isOperationPossible = true;
	/// <summary>
	/// 操作可能か
	/// </summary>
	public bool isOperationPossible
	{
		get { return _isOperationPossible; }
		set { _isOperationPossible = value; }
	}

	public Scaffolds rideScaffolds
	{
		get { return _rideScaffolds; }
		set { _rideScaffolds = value; }
	}

	public bool isRide
	{
		get
		{
			return _isRide;
		}

		set
		{
			_isRide = value;
		}
	}


	//private bool isJump = false;
	private float jumpVY = 0.0f;
	private float jumpPower = 2f;
	private Vector3 moveVec = Vector3.zero;

	[SerializeField]
	private GameObject redLightPrefab = null;
	//private GameObject redLight = null;
	private IrradiationPort lightPosition = null;

	private Transform cameraTransform = null;   // プレイヤーカメラのトランスフォーム
	private float rotateSpeed = 1.5f;   // 視点回転の速度
	private float facingUpLimit = 60.0f; // 視点移動の上方向制限
	private float facingDownLimit = 70.0f;  // 視点移動の下方向制限
	//private Vector3 defaultCameraDirection = Vector3.zero;	 // 開始時の視点方向

	private static readonly int itemPossessionMax = 10;
	private List<Items> items = new List<Items>(itemPossessionMax); // プレイヤーのアイテムリスト
	[SerializeField]
	private Examine examine = null;
	[SerializeField]
	private GameObject raygun = null;
	//private Vector3 initExamineOffset;

	private bool isRightClick = false;
	private float rightClickWindowTime = 0.3f;
	private float rightClickWindowTimeCount = 0.0f;

	//	private MovableScaffoldsController;
	private Scaffolds _rideScaffolds; // 現在乗っている足場
	private bool _isRide = false;

	[SerializeField]
	private GameObject rayGunIconPrefab = null; // 現在のレイガンの色を表すアイコン
	private MovableObjectController[] rayGunIconControllers = null; // 現在のレイガンの色を表すアイコンを制御する
	private int rollCount = 0; // レイガンの色アイコンを回転させる回数

	protected override void Awake()
	{
		base.Awake();
		soundCollector = FindObjectOfType<SoundCollector>();

		anim = GetComponent<Animator>();
		hashSpeed = Animator.StringToHash("Speed");
		characterController = GetComponent<CharacterController>();
		characterController.detectCollisions = false;
		lightPosition = transform.FindChild("LightPosition").GetComponent<IrradiationPort>();
		lightPosition.transform.parent = Camera.main.transform;
		isLaserPossible = true;
		lasers[0].color = ColorState.RED;
		lasers[0].isPossible = isRedLaser;
		lasers[1].color = ColorState.GREEN;
		lasers[1].isPossible = isGreenLaser;
		lasers[2].color = ColorState.BLUE;
		lasers[2].isPossible = isBlueLaser;

		if (!lasers[0].isPossible)
		{
			raygun.SetActive(false);
		}
		else
		{
			rayGunIconControllers = (Instantiate(rayGunIconPrefab) as GameObject).GetComponentsInChildren<MovableObjectController>();
		}

		Cursor.visible = false;
	}

	void Start()
	{
		cameraTransform = Camera.main.transform;
		//defaultCameraDirection = cameraTransform.forward;
	}

	protected override void Update()
	{
		if (FadeManager.instance.IsFading)
		{
			return;
		}

		Cursor.lockState = CursorLockMode.Locked;

		Move(); // プレイヤーの移動など

		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		CameraMove(-mouseY, mouseX); // カメラの操作

		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.B))
		{
			RaycastHit hit;
			Physics.Raycast(ray, out hit);
			Instantiate(explosionPrefab, hit.point, Quaternion.identity);
		}
#endif

		// レイガンを照射
		if (Input.GetMouseButton(0) && isLaserPossible && !AdvancedWriteMessageSingleton.instance.isWrite && lasers[0].isPossible)
		{
			if (eraseLaser == null)
			{
				eraseLaser = ((GameObject)Instantiate(redLightPrefab, lightPosition.transform.position, Quaternion.identity)).GetComponent<EraseLaser>();
				eraseLaser.transform.parent = Camera.main.transform;
				eraseLaser.color = lasers[currentLaserIndex].color;

				if (examine.grabbedObject != null)
				{
					examine.LetGo();
				}

				//SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[SoundCollector.SoundName.LaserLow], true, true, 1f, 0.0f, true);
			}

			eraseLaser.IrradiationToTarget(ray.direction * 100.0f + transform.position);
		}

		// レーザーを停止
		if (!Input.GetMouseButton(0) && eraseLaser != null)
		{
			StopLaser();
		}

		bool isLaserCange = false;
		if (Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
		{
			isLaserCange = true;
		}

		if (Input.GetMouseButtonDown(1) && !isRightClick)
		{
			isRightClick = true;
		}
		else if (Input.GetMouseButtonDown(1) && isRightClick)
		{
			isLaserCange = true;
			isRightClick = false;
			rightClickWindowTimeCount = 0.0f;
		}

		if (isRightClick && rightClickWindowTimeCount < rightClickWindowTime)
		{
			rightClickWindowTimeCount += Time.deltaTime;
		}
		else if (isRightClick)
		{
			rightClickWindowTimeCount = 0.0f;
			isRightClick = false;
		}

		if (Input.GetKeyDown(KeyCode.Q) && lasers[currentLaserIndex].isPossible)
		{
			isLaserCange = true;
		}

		if (isLaserCange)
		{
			ChangeLaser();

			if (eraseLaser != null)
			{
				eraseLaser.color = lasers[currentLaserIndex].color;
			}
		}

		if (rayGunIconControllers == null)
		{
			return;
		}

		foreach (MovableObjectController movableObjectController in rayGunIconControllers)
		{
			if (movableObjectController.currentState == MovableObjectController.State.Move)
			{
				return;
			}
		}

		if (rollCount > 0)
		{
			rollCount--;
			foreach (MovableObjectController movableObjectController in rayGunIconControllers)
			{
				movableObjectController.Action();
			}
		}
	}

	/// <summary>
	/// プレイヤーの所持するアイテムリストを返す
	/// </summary>
	/// <returns></returns>
	public List<Items> GetItemList()
	{
		return items;
	}

	public void StopLaser()
	{
		if (eraseLaser != null)
		{
			SoundPlayerSingleton.instance.StopSE(gameObject, 1);
			Destroy(eraseLaser.gameObject);
		}
	}

	/// <summary>
	/// 視点を移動する
	/// </summary>
	/// <param name="vx">X方向の移動量</param>
	/// <param name="vy">Y方向の移動量</param>
	private void CameraMove(float vx, float vy)
	{
		if (AdvancedWriteMessageSingleton.instance.isWrite)
		{
			return;
		}

		transform.Rotate(0.0f, vy * rotateSpeed, 0.0f);

		float cameraRotX = vx * rotateSpeed;
		cameraTransform.Rotate(cameraRotX, 0.0f, 0.0f);

		// カメラの見ている方向
		Vector3 direction = cameraTransform.forward;

		float fFront;
		// カメラの前方方向値
		Vector3 front = direction;
		front.y = 0;     // XZ平面での距離なのでYはいらない
		fFront = front.magnitude;

		// Y軸とXZ平面の前方方向との角度を求める
		float deg = Mathf.Atan2(-direction.y, fFront) * Mathf.Rad2Deg;

		// 可動範囲を制限
		if (deg > facingDownLimit)
		{
			cameraTransform.Rotate(-cameraRotX, 0.0f, 0.0f);
		}
		if (deg < -facingUpLimit)
		{
			cameraTransform.Rotate(-cameraRotX, 0.0f, 0.0f);
		}
	}

	void Move()
	{
		if (AdvancedWriteMessageSingleton.instance.isWrite)
		{
			return;
		}

		float dx = Input.GetAxisRaw("Horizontal");
		float dy = Input.GetAxisRaw("Vertical");

		moveVec = Vector3.zero; // 今回の移動量計算用
		if (dy != 0.0f || dx != 0.0f)
		{
			moveVec = transform.rotation * new Vector3(dx, 0.0f, dy).normalized;
			if (anim != null)
			{
				anim.SetFloat(hashSpeed, speed);
			}
		}
		else if (anim != null)
		{
			anim.SetFloat(hashSpeed, 0.0f);
		}

		if (!characterController.isGrounded)
		{
			// 空中では重力により落下速度を加算する
			jumpVY += Physics.gravity.y * Time.deltaTime;
		}
		else // 地面についている
		{
			// ジャンプさせる
			if (Input.GetButtonDown("Jump"))
			{
				SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[SoundCollector.SoundName.Jump]);
				jumpVY = jumpPower;
				//isJump = true;
			}
			else
			{
				//isJump = false;
				jumpVY = Physics.gravity.y * Time.deltaTime;
			}
		}

		moveVec.y += jumpVY;

		if (moveVec != Vector3.zero)
		{
			characterController.Move(moveVec * speed * Time.deltaTime);
		}
	}


	/// <summary>
	/// 使用するレーザーを変更する
	/// </summary>
	private void ChangeLaser()
	{
		if (rayGunIconControllers == null)
		{
			return;
		}

		bool isLaserChanged = true;
		foreach (MovableObjectController movableObjectController in rayGunIconControllers)
		{
			if (movableObjectController.currentState == MovableObjectController.State.Move)
			{
				isLaserChanged = false;
				return;
			}
		}

		int laserIndex = currentLaserIndex;
		do
		{
			rollCount++;
			if (++laserIndex >= laserMax)
			{
				laserIndex = 0;
			}

			if (currentLaserIndex == laserIndex)
			{
				isLaserChanged = false;
				break;
			}
		} while (!lasers[laserIndex].isPossible);

		if (isLaserChanged)
		{
			currentLaserIndex = laserIndex;
			SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[SoundCollector.SoundName.LensReplacement]);
		}
	}

	/// <summary>
	/// アイテム欄にアイテムを追加する
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool AddItem(Items item)
	{
		switch (item)
		{
			case Items.RedRaygun:
				lasers[0].isPossible = true;
				break;
			case Items.GreenRaygun:
				lasers[1].isPossible = true;
				break;
			case Items.BlueRaygun:
				lasers[2].isPossible = true;
				break;
		}

		switch (item)
		{
			case Items.RedRaygun:
			case Items.GreenRaygun:
			case Items.BlueRaygun:
				raygun.SetActive(true);
				if (rayGunIconControllers == null)
				{
					rayGunIconControllers = (Instantiate(rayGunIconPrefab) as GameObject).GetComponentsInChildren<MovableObjectController>();
				}
				ShowIcon(true);
				break;
		}

		if (items.Count < itemPossessionMax)
		{
			items.Add(item);
			SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[SoundCollector.SoundName.ItemGet]);
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 指定のアイテムをプレイヤーは持っているか
	/// </summary>
	/// <param name="item">探したいアイテム</param>
	/// <returns></returns>
	public bool ContainsItem(Items item)
	{
		return items.Contains(item);
	}

	/// <summary>
	/// アイテムを削除
	/// </summary>
	/// <param name="item"></param>
	public void DeleteItem(Items item)
	{
		items.Remove(item);
	}

	///// <summary>
	///// 指定のアイテムを持っているか
	///// </summary>
	///// <typeparam name="T">アイテムを表す型</typeparam>
	///// <returns></returns>
	//public bool ContainsItem<T>() where T : Item
	//{
	//	foreach (T item in items)
	//	{
	//		if (item is T)
	//		{
	//			return true;
	//		}
	//	}

	//	return false;
	//}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null)
			return;
	}

	public void Respawn()
	{
		examine.LetGo();
	}

	/// <summary>
	/// カラーアイコンの標示状態を切り替え
	/// </summary>
	public void ShowIcon(bool flag)
	{
		rayGunIconControllers[0].transform.parent.gameObject.SetActive(flag);
	}

	/// <summary>
	/// minimappu標示状態を切り替え
	/// </summary>
	public void ShowMiniMap(bool flag)
	{
		GameObject.FindGameObjectWithTag(Tags.MiniMapCamera).GetComponent<Camera>().enabled = flag;
	}
}
