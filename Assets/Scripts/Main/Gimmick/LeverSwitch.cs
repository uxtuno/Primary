using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LeverSwitch : MonoBehaviour
{
	/// <summary>
	/// 回転方向
	/// </summary>
	private enum RotationDirection
	{
		right, left
	}

	private float initRotationAngle { get; set; }
	private float currentRotationAngle { get; set; }
	private Range<float> rotationRange { get; set; }
	private bool isSwitchable { get; set; }
	private bool isRuntime { get; set; }
	private bool isState;
	private bool IsState
	{
		get
		{
			if (isRuntime)
			{
				return false;
			}
			else
			{
				return isState;
			}
		}
		set
		{
			isState = value;
		}
	}

	void Awake()
	{
		initRotationAngle = transform.eulerAngles.z;
		currentRotationAngle = initRotationAngle;
		rotationRange = new Range<float>(0.0f + initRotationAngle, 90.0f + initRotationAngle);
		isRuntime = false;
		IsState = false;
		isSwitchable = false;
	}

	void Start()
	{
	}

	void FixedUpdate()
	{
	}

	void Update()
	{
		if (isSwitchable)
		{
			if (Input.GetKeyDown(KeyCode.E) && !isRuntime)
			{
				IsState = !IsState;
				isRuntime = true;
			}
		}

		if (isState)
		{
			Rotation(RotationDirection.left);
		}
		else
		{
			Rotation(RotationDirection.right);
		}
	}

	private void Rotation(RotationDirection direction)
	{
		const float dAngle = 150.0f;
		switch (direction)
		{
			case RotationDirection.left:
				currentRotationAngle += dAngle * Time.deltaTime;
				if (currentRotationAngle > rotationRange.max)
				{
					currentRotationAngle = rotationRange.max;
					isRuntime = false;
				}
				break;
			case RotationDirection.right:
				currentRotationAngle -= dAngle * Time.deltaTime;
				if (currentRotationAngle < rotationRange.min)
				{
					currentRotationAngle = rotationRange.min;
					isRuntime = false;
				}
				break;
		}
		transform.eulerAngles = new Vector3(0.0f, 0.0f, currentRotationAngle);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			isSwitchable = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			isSwitchable = false;
		}
	}
}
