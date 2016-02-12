using UnityEngine;
using System.Collections;

/// <summary>
/// 調べることが可能な
/// </summary>
public interface ICanExamine{

	/// <summary>
	/// 調べる
	/// </summary>
	void Check();

	/// <summary>
	/// 調べることが可能になった瞬間に呼ばれる
	/// </summary>
	void ViewIcon();

	/// <summary>
	/// 調べることが可能なエリアから離れた瞬間に呼ばれる
	/// </summary>
	void HideIcon();
}
