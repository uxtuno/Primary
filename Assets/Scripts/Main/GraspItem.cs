using UnityEngine;

/// <summary>
/// プレイヤーが持ち運べるものにアタッチする
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GraspItem : Gimmick, ICheckEvent
{
    private RigidbodyConstraints defaultConstraints = RigidbodyConstraints.None;
    private GameObject owner; // 掴んでいる者。この角度を元に自身の角度を調整
	private Transform rideScaffolds; // 現在乗っている足場
	private BlockRespawnPoint blockRespawnPoint;
	private Vector3 respawnPoint;

    /// <summary>
    /// 掴まれているかどうか
    /// </summary>
    public bool isGrasp { get; private set; }

	/// <summary>
	/// 利用可能フラグ(常に利用可能)
	/// </summary>
	public bool isPossible
	{
		get
		{
			return enabled;
		}
	}

	protected override void Awake()
    {
        base.Awake();
		// RigidBodyの初期設定を記録
        defaultConstraints = rigidbody.constraints;
		blockRespawnPoint = GetComponentInParent<BlockRespawnPoint>();
    }

	void Start()
	{
		respawnPoint = transform.position;
	}

    protected override void Update()
    {
        if (owner == null)
        {
            return;
        }
        Vector3 direction = owner.transform.eulerAngles;
        direction.x = transform.eulerAngles.x;
        direction.z = transform.eulerAngles.z;
        transform.eulerAngles = direction;
    }

    /// <summary>
    /// 掴む
    /// </summary>
    public void Grab(GameObject owner)
    {
        this.owner = owner;
        isGrasp = true;
        //transform.parent = owner.transform;
        rigidbody.useGravity = false;
        rigidbody.freezeRotation = true;
		gameController.HideIcon();
    }

    /// <summary>
    /// 手を放す
    /// </summary>
    public void LetGo()
    {
        owner = null;
        isGrasp = false;
        //transform.parent = null;
        rigidbody.useGravity = true;
        rigidbody.constraints = defaultConstraints;
		rigidbody.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision other)
    {
		if (other.transform.GetComponentInChildren<Scaffolds>() != null)
		{
			rideScaffolds = other.transform;
		}

	    if (isGrasp)
			return;

	    if (other.transform.GetComponentInChildren<Scaffolds>() == null)
	    {
		    InitParent();
	    }
    }

	void OnCollisionExit(Collision other)
	{
		if (isGrasp)
			return;

		// 載ってた足場から離れた
		if(rideScaffolds == other.transform.parent)
		{
			rideScaffolds = null;
		}
	}

	/// <summary>
	/// 再生成
	/// </summary>
	public void Respawn()
	{
		if (blockRespawnPoint != null)
		{
			blockRespawnPoint.Action();
		}
		else
		{
			transform.position = respawnPoint;
			InitParent();
		}
	}

	/// <summary>
	/// 実装しない
	/// </summary>
	public void Check()
	{
	}

	/// <summary>
	/// 右クリックで掴むアイコンを表示
	/// </summary>
	public void GetIconSprite()
	{
		if(isPossible)
		{
			ExamineIconManager.ShowIcon(ExamineIconManager.IconType.Hold);
		}
	}
}
