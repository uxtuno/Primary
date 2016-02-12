using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// プレイヤーが持ち運べるものにアタッチする
/// </summary>
public class GraspItem : Gimmick, ICheckEvent
{
    private RigidbodyConstraints defaultConstraints = RigidbodyConstraints.None;
    private GameObject owner; // 掴んでいる者。この角度を元に自身の角度を調整
    private bool _isGrasp; // 掴まれているかどうか
	private Transform rideScaffolds; // 現在乗っている足場
	private BlockRespawnPoint blockRespawnPoint;
	private Vector3 respawnPoint;

    /// <summary>
    /// 掴まれているかどうか
    /// </summary>
    public bool isGrasp
    {
        get
        {
            return _isGrasp;
        }

        private set
        {
            _isGrasp = value;
        }
    }

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

		if (!isGrasp)
        {
            if (other.transform.GetComponentInChildren<Scaffolds>() == null)
            {
				InitParent();
            }

			if (other.transform.GetComponent<GraspItem>() != null)
			{
				
			}
        }
    }

	void OnCollisionExit(Collision other)
	{
		if (!isGrasp)
		{
			if (other.transform.GetComponent<GraspItem>() != null)
			{
				
			}

			if(rideScaffolds == other.transform.parent)
			{
				rideScaffolds = null;
			}
		}
	}

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
	public void ShowIcon()
	{
		if(isPossible)
		{
			ExamineIconManager.ShowIcon(ExamineIconManager.IconType.Hold);
		}
	}
}
