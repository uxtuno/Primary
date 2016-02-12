using UnityEngine;
using System.Collections;

/// <summary>
/// ギミックが外部に公開する情報
/// 独自の情報を持たせたければこれを継承して作成
/// </summary>
public abstract class GimmickState
{
	public bool frag;
}

/// <summary>
/// 全てのギミックはこれを継承する
/// </summary>
public abstract class Gimmick : EventObject
{

}
