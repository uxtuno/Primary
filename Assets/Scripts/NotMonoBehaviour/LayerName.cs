public class LayerName
{

	public struct Para{ 
		public readonly int id;
		public readonly int maskValue;
		public readonly string name;

		public Para(int _id, int _maskValue, string _name)
		{
			id = _id;
			maskValue = _maskValue;
			name = _name;
		}
	}

	public static Para Default = new Para(0, 1, "Default");
	public static Para TransparentFX = new Para(1, 2, "TransparentFX");
	public static Para IgnoreRaycast = new Para(2, 4, "IgnoreRaycast");
	public static Para Water = new Para(4, 16, "Water");
	public static Para UI = new Para(5, 32, "UI");
	public static Para Laser = new Para(8, 256, "Laser");
	public static Para Gound = new Para(9, 512, "Gound");
	public static Para Title = new Para(10, 1024, "Title");
	public static Para Menu = new Para(11, 2048, "Menu");
	public static Para SelectStage = new Para(12, 4096, "SelectStage");
	public static Para NoShadow = new Para(13, 8192, "NoShadow");
	public static Para MiniMapIcon = new Para(14, 16384, "MiniMapIcon");
	public static Para Wall = new Para(15, 32768, "Wall");
	public static Para NoCamera = new Para(16, 65536, "NoCamera");
	public static Para NoLight = new Para(17, 131072, "NoLight");

	/// <summary>
	/// <para>0. "Default"</para>
	/// <para>1. "TransparentFX"</para>
	/// <para>2. "Ignore Raycast"</para>
	/// <para>3. ""</para>
	/// <para>4. "Water"</para>
	/// <para>5. "UI"</para>
	/// <para>6. ""</para>
	/// <para>7. ""</para>
	/// <para>8. "Laser"</para>
	/// <para>9. "Gound"</para>
	/// <para>10. "Title"</para>
	/// <para>11. "Menu"</para>
	/// <para>12. "SelectStage"</para>
	/// <para>13. "NoShadow"</para>
	/// <para>14. "MiniMapIcon"</para>
	/// <para>15. "Wall"</para>
	/// <para>16. "NoCamera"</para>
	/// <para>17. "NoLight"</para>
	/// <para>18. ""</para>
	/// <para>19. ""</para>
	/// <para>20. ""</para>
	/// <para>21. ""</para>
	/// <para>22. ""</para>
	/// <para>23. ""</para>
	/// <para>24. ""</para>
	/// <para>25. ""</para>
	/// <para>26. ""</para>
	/// <para>27. ""</para>
	/// <para>28. ""</para>
	/// <para>29. ""</para>
	/// <para>30. ""</para>
	/// <para>31. ""</para>
	/// </summary>
	public static readonly string[] names = new string[]{"Default","TransparentFX","Ignore Raycast","","Water","UI","","","Laser","Gound","Title","Menu","SelectStage","NoShadow","MiniMapIcon","Wall","NoCamera","NoLight","","","","","","","","","","","","","",""};
}
