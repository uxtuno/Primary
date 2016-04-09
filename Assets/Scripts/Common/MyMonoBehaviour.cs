using System.Linq;
using UnityEngine;

/// <summary>
///     MonoBehaviourに独自の機能を追加する
/// </summary>
public class MyMonoBehaviour : MonoBehaviour
{
	private Collider _collider; // colliderプロパティの実態

	private GameManager _gameManager = null;
	private Player _player = null; // playerプロパティの実態
	private Renderer _renderer; // rendererプロパティの実態
	private Rigidbody _rigidbody; // rigidbodyプロパティの実態
	private Transform defaultParent; // 初期状態の親オブジェクト

	/// <summary>
	///     Playerを取得
	/// </summary>
	protected Player player
	{
		get
		{
			if (_player == null)
			{
				var go = GameObject.FindGameObjectWithTag(Tags.Player);
				if (go != null)
				{
					_player = go.GetComponentInParent<Player>();
				}
			}
			return _player;
		}
	}

	/// <summary>
	///     GameControllerを取得
	/// </summary>
	protected GameManager gameController
	{
		get
		{
			if (_gameManager == null)
			{
				var go = GameObject.FindGameObjectWithTag(Tags.GameController);
				if (go != null)
				{
					_gameManager = go.GetComponent<GameManager>();
				}
			}

			return _gameManager;
		}
	}

	/// <summary>
	///     自分自身のRendererを取得する
	/// </summary>
	public new virtual Renderer renderer
	{
		get
		{
			if (_renderer == null)
			{
				_renderer = GetComponent<Renderer>();
			}

			return _renderer;
		}
	}

	/// <summary>
	///     自分自身のcollideを取得する
	/// </summary>
	public new virtual Collider collider
	{
		get
		{
			if (_collider == null)
			{
				_collider = GetComponent<Collider>();
			}

			return _collider;
		}
	}

	/// <summary>
	///     自分自身のrigidbodyを取得する
	/// </summary>
	public new Rigidbody rigidbody
	{
		get
		{
			if (_rigidbody != null)
				return _rigidbody;

			_rigidbody = GetComponent<Rigidbody>();
			return _rigidbody;
		}
	}

	/// <summary>
	///     表示状態
	/// </summary>
	public virtual bool IsShow
	{
		// 子も含めて初めに見つけたRendererの状態を返す
		get
		{
			return GetComponentsInChildren<Renderer>()
				.Select(renderer => renderer.enabled).FirstOrDefault();
		}

		// 子も含めて全てのRendererの表示状態を変更する
		set
		{
			foreach (var renderer in GetComponentsInChildren<Renderer>())
			{
				renderer.enabled = value;
			}

			foreach (var renderer in GetComponentsInChildren<Light>())
			{
				renderer.enabled = value;
			}
		}
	}

	protected virtual void Awake()
	{
		var gameObject = GameObject.FindGameObjectWithTag(Tags.GameController);
		if (gameObject != null)
		{
			_gameManager = gameObject.GetComponent<GameManager>();
		}

		defaultParent = transform.parent;
	}

	protected virtual void Update()
	{
	}

	protected virtual void LateUpdate()
	{
	}

	public T GetSafeComponent<T>() where T : MonoBehaviour
	{
		var component = GetComponent<T>();

		if (component == null)
		{
			Debug.LogError("Expected to find component of type "
						   + typeof(T) + " but found none", this);
		}

		return component;
	}

	/// <summary>
	///     親を初期状態にする
	/// </summary>
	public void InitParent()
	{
		transform.parent = defaultParent;
	}
}