using UnityEngine;
using System.Collections;

public class FlashingCursor : MonoBehaviour
{
	private enum FadeState
	{
		FadeIn,
		FadeOut
	}

	private struct ColorConcentration
	{
		public const float max = 1.0f;
		public const float min = 0.3f;
		public const float centor = max + min / 2.0f;
	}

	private Color color { get; set; }
	private float alpha { get; set; }
	private FadeState fadeState { get; set; }
	private const float speed = 1f;

	private void Awake()
	{
		if(GetComponent<Renderer>().material.color != null)
		{
			color = GetComponent<Renderer>().material.color;
			if(color.a > ColorConcentration.centor)
			{
				alpha = ColorConcentration.max;
				fadeState = FadeState.FadeOut;
			}
			else
			{
				alpha = ColorConcentration.min;
				fadeState = FadeState.FadeIn;
			}
			color = new Color(color.r, color.g, color.b, alpha);
		}
		else
		{
			enabled = false;
		}
	}

	private void Update()
	{
		switch(fadeState)
		{
			case FadeState.FadeIn:
				if (alpha < ColorConcentration.max)
				{
					alpha += speed * Time.deltaTime;
				}
				else
				{
					alpha = ColorConcentration.max;
					fadeState = FadeState.FadeOut;
				}
				break;

			case FadeState.FadeOut:
				if (alpha > ColorConcentration.min)
				{
					alpha -= speed * Time.deltaTime;
				}
				else
				{
					alpha = ColorConcentration.min;
					fadeState = FadeState.FadeIn;
				}
				break;
		}

		color = new Color(color.r, color.g, color.b, alpha);
		GetComponent<Renderer>().material.color = color;
	}
}
