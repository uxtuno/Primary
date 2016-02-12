using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AdvancedWriteMessageSingleton : MyMonoBehaviour
{
	/// <summary>
	/// 唯一のインスタンス
	/// </summary>
	private static AdvancedWriteMessageSingleton _instance;

	private AdvancedWriteMessageSingleton()
	{
	}

	public static AdvancedWriteMessageSingleton instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject("AdvancedWriteMessageSingleton");
				_instance = go.AddComponent<AdvancedWriteMessageSingleton>();
			}
			return _instance;
		}
	}

	private struct CharAndColor
	{
		public char character;
		public ObjectColor objColor;
	}

	private const float intervalForCharacterDisplay = 0.075f;     // 1文字の表示にかかる時間
	private GameObject canvas { get; set; }                     // シーンに存在するキャンバス
	private Text uiText { get; set; }                           // シーンに存在するTextクラス
	private TextAsset stageText { get; set; }                   // 現在のシーンに対応したTextAsset
	private List<string> sentence { get; set; }                 // 読み込んだStageTextから分割したmessagesのリスト
	private List<string> messages { get; set; }                 // 読み込んだsentenceから分割したmessageのリスト
	private string message { get; set; }                        // 読み込んだmessagesから分割した実際に使用される文字列
	private List<CharAndColor> messageAndColor { get; set; }    // 読み込んだmessageから分割した使用するmessageと色情報のリスト
	private int messageLength { get; set; }                     // 表示中の文字数
	private string currentDisplayCharacter { get; set; }        // 表示中の文字列
	private int currentLine { get; set; }                       // 表示中の行数
	private float timeElapsed { get; set; }                     // 文字列の表示を開始した時間
	private bool isOnce { get; set; }                           // 一度だけ実行したいものに使う
	private bool isFullView { get; set; }                       // 全文表示を実行するかどうか
	private bool isNextLine { get; set; }                       // 次の行の読み込みをするかどうか
	private bool _isWrite;                                      // isWriteの実体
	public bool isWrite                                         // 描画中かどうか
	{
		private set
		{
			if (canvas != null)
			{
				canvas.GetComponent<Canvas>().enabled = value;
			}
			_isWrite = value;
		}

		get
		{
			return _isWrite;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		canvas = GameObject.Find("Canvas");
		uiText = FindObjectOfType<Text>();
		stageText = (TextAsset)Resources.Load("Texts/" + Application.loadedLevelName + "Message");
		if (stageText == null)
		{
			Debug.LogWarning("Textが存在しません\nWriteMessageを終了します。");
			enabled = false;
			return;
		}

		sentence = new List<string>();
		sentence.AddRange(stageText.text.Replace("[Message]\r\n", "[Message]").Replace("[Message]", "$").Split('$'));
		IndexTagCheck();
		Initialize();
	}

	private void Initialize()
	{
		messages = new List<string>();
		messageAndColor = new List<CharAndColor>();
		message = string.Empty;
		messageLength = 0;
		currentDisplayCharacter = string.Empty;
		currentLine = 0;
		timeElapsed = 0.0f;
		isOnce = false;
		isFullView = false;
		isNextLine = false;
		isWrite = false;
	}

	public void Write(int messageNumber)
	{
		isWrite = true;
		timeElapsed = Time.time;
		StartCoroutine(Move(messageNumber));
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetMouseButtonDown(0) && isWrite)
		{
			// 文字列の表示を開始してから0.2秒間次の文字列を再生しない
			if ((Time.time - timeElapsed) < 0.15f)
			{
				return;
			}

			// 文字列が最後まで再生されていない場合は最後まで表示する
			if (messageLength < messageAndColor.Count)
			{
				isFullView = true;
			}
			else
			{
				isFullView = false;

				// 次の行がない場合は初期値を与えて終了
				if (currentLine >= messages.Count - 1)
				{
					Initialize();
				}
				else
				{
					currentLine++;
					isNextLine = true;
				}
			}
		}
	}

	private IEnumerator Move(int messageNumber)
	{
		while (true)
		{
			// 描画が終了していた場合コルーチンを停止する
			if (!isWrite)
			{
				Initialize();
				yield break;
			}

			// Move関数が実行されたとき、1度だけ処理を行う
			if (!isOnce)
			{
				messages.AddRange(sentence[messageNumber].Replace("{Next}\r\n", "{Next}").Replace("{Next}", "$").Split('$'));
				SetNextLine(currentLine);
				ColorTagCheck(message);
				isOnce = true;
			}

			// 全文表示
			if (isFullView)
			{
				if (isNextLine)
				{
					SetNextLine(currentLine);
					ColorTagCheck(message);
					isFullView = false;
					isNextLine = false;
				}
				else
				{
					currentDisplayCharacter = ConvertToStringRecursion();
				}
			}


			messageLength++;
			if (messageLength >= messageAndColor.Count)
			{
				messageLength = messageAndColor.Count;
				isFullView = true;
			}

			currentDisplayCharacter = ConvertToString();
			uiText.text = currentDisplayCharacter;

			// 指定秒数分待機する
			yield return new WaitForSeconds(intervalForCharacterDisplay);
		}
	}

	/// <summary>
	/// インデックスタグを元にコレクションの要素を並び替える
	/// インデックスタグは最初にヒットしたもののみを読み取る
	/// </summary>
	private void IndexTagCheck()
	{
		const int numberOfMessages = 20;									// sortingSentenceの初期要素数
		List<string> sortingSentence = new List<string>(numberOfMessages);	//並び替えたコレクションを一時的に格納しておくリスト

		// sortingSentenceの要素を空文字で初期化
		for (int i = 0; i < numberOfMessages; i++)
		{
			sortingSentence.Add("");
		}

		foreach (string str in sentence)
		{
			int index = 0;                              // 格納先の要素番号
			const string regexTag = @"\[index\d+\]";    // 正規表現パターンで記した検索対象のタグ

			if (Regex.IsMatch(str, regexTag))			// タグの存在確認
			{
				// 正規表現でindexを取得
				Match mc = Regex.Match(str, regexTag);
				index = int.Parse(mc.Value.Replace("[index", "").Replace("]", ""));
			}
			else
			{
				if (sortingSentence.IndexOf("") == -1)
				{
					sortingSentence.Add("");
				}

				// 一番初めにヒットした空文字が格納されている要素番号を取得
				index = sortingSentence.IndexOf("");
			}

			// 上書き予定の要素にすでに文字列が存在した場合は
			// 一番初めにヒットした空文字が格納されている要素に上書き
			if (sortingSentence[index] != "")
			{
				if (sortingSentence.IndexOf("") == -1)
				{
					sortingSentence.Add("");
				}

				sortingSentence[sortingSentence.IndexOf("")] = str.Replace("[index" + index + "]\r\n", "[index" + index + "]").Replace("[index" + index + "]", "");
			}
			else    
			{
				// index番目の要素に上書き
				sortingSentence[index] = str.Replace("[index" + index + "]\r\n", "[index" + index + "]").Replace("[index" + index + "]", "");
			}
		}

		// sortingSentenceの要素数が最小限になるように調整
		sortingSentence.Reverse();

		int removeCounter = 0;

		foreach (string target in sortingSentence)
		{
			if (target == "")
			{
				removeCounter++;
			}
			else
			{
				break;
			}
		}

		sortingSentence.RemoveRange(0, removeCounter);
		sortingSentence.Reverse();

		// sentenceを更新
		sentence = sortingSentence;
	}

	/// <summary>
	/// 次の行をセットする
	/// </summary>
	private void SetNextLine(int index)
	{
		if (!isWrite)
		{
			isWrite = true;
		}

		// 行セット
		message = messages[index];
		// 時間セット
		timeElapsed = Time.time;

		// 文字カウントを初期化
		messageLength = 0;
	}

	/// <summary>
	/// 文字列内に存在する独自のタグを解析する
	/// </summary>
	private void ColorTagCheck(string str)
	{
		messageAndColor = new List<CharAndColor>();

		CharAndColor charAndColor = new CharAndColor();
		int state = 0;
		ColorState currentColor = ColorState.NONE;		// 文字色
		ColorState candidateColor = ColorState.NONE;	// タグから解析している途中で候補に挙がった色
		bool isTagClosed = false;

		for (int i = 0; i < str.Length; i++)
		{
			switch (state)
			{
				case 0:
					if (str[i] == '<')
					{
						state = 1;
						isTagClosed = false;
						continue;
					}
					break;

				case 1:
					if (str[i] == 'r')
					{
						state = 2;
						candidateColor = ColorState.RED;
					}
					else if (str[i] == 'g')
					{
						state = 2;
						candidateColor = ColorState.GREEN;
					}
					else if (str[i] == 'b')
					{
						state = 2;
						candidateColor = ColorState.BLUE;
					}
					else if (str[i] == 'c')
					{
						state = 2;
						candidateColor = ColorState.CYAN;
					}
					else if (str[i] == 'm')
					{
						state = 2;
						candidateColor = ColorState.MAGENTA;
					}
					else if (str[i] == 'y')
					{
						state = 2;
						candidateColor = ColorState.YELLOW;
					}
					else if (str[i] == 'w')
					{
						state = 2;
						candidateColor = ColorState.WHITE;
					}
					else if (str[i] == '/')
					{
						state = 2;
						isTagClosed = true;
					}
					else
					{
						state = 0;
						break;
					}
					continue;

				case 2:
					if (str[i] == '>')
					{
						state = 0;
						currentColor = candidateColor;
						continue;
					}
					state = 1;
					isTagClosed = false;
					break;
			}

			// </>が使われた次の文字からはデフォルトに戻す
			if (isTagClosed)
			{
				currentColor = ColorState.NONE;
				isTagClosed = false;
			}
			charAndColor.character = str[i];
			charAndColor.objColor = currentColor;
			messageAndColor.Add(charAndColor);

		}
	}

	/// <summary>
	/// 色と文字の情報を適切な文字列に変換する
	/// </summary>
	/// <param name="str">格納先</param>
	/// <returns></returns>
	private string ConvertToString(string str = "")
	{
		for (int i = 0; i < messageLength; i++)
		{
			if (messageAndColor[i].objColor.state != ColorState.NONE)
			{
				str += "<color=#" + messageAndColor[i].objColor.ToString() + ">" + messageAndColor[i].character + "</color>";

			}
			else
			{
				str += messageAndColor[i].character;
			}
		}

		return str;
	}
	/// <summary>
	/// 色と文字の情報を適切な文字列に変換する(再帰版)
	/// </summary>
	/// <param name="str">格納先</param>
	/// <returns></returns>
	private string ConvertToStringRecursion(string str = "")
	{
		if (messageLength <= messageAndColor.Count)
		{
			for (int i = 0; i < messageLength; i++)
			{
				if (messageAndColor[i].objColor.state != ColorState.NONE)
				{
					str += "<color=#" + messageAndColor[i].objColor.ToString() + ">" + messageAndColor[i].character + "</color>";
				}
				else
				{
					str += messageAndColor[i].character;
				}
			}
			messageLength++;
			return ConvertToStringRecursion(str);
		}
		else
		{
			return str;
		}
	}

	//private string Trim(string str)
	//{
	//	MatchCollection mc = Regex.Matches(str, @"<color=#......>");
	//	while (mc.Count > 1)
	//	{
	//		for(int i = 0; i < mc.Count - 2; i++)
	//		{
	//			if(mc[i] == mc[i+1])
	//			{

	//			}
	//		}
	//		if (Regex.IsMatch(str, @"</color><color=#\w\w\w\w\w\w>"))
	//		{
	//			Match match = Regex.Match(str, @"</color><color=#\w\w\w\w\w\w>");
	//			str = str.Remove(match.Index, match.Length);
	//		}
	//		mc = Regex.Matches(str, @"<color=#......>");
	//	}
	//	return str;
	//}
}
