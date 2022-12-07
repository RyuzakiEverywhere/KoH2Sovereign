using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class AnalyticsVisualClues : MonoBehaviour
{
	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600028D RID: 653 RVA: 0x00024762 File Offset: 0x00022962
	public int IMG_WIDTH
	{
		get
		{
			return this.TilesX * this.TileSize;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600028E RID: 654 RVA: 0x00024771 File Offset: 0x00022971
	public int IMG_HEIGHT
	{
		get
		{
			return this.TilesY * this.TileSize;
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00024780 File Offset: 0x00022980
	public static void Init()
	{
		if (AnalyticsVisualClues.instance != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("AnalyticsVisualClues");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		AnalyticsVisualClues.instance = gameObject.AddComponent<AnalyticsVisualClues>();
		AnalyticsVisualClues.LoadSettings();
		global::Defs.OnDefsProcessedEvent = (Action)Delegate.Combine(global::Defs.OnDefsProcessedEvent, new Action(AnalyticsVisualClues.LoadSettings));
	}

	// Token: 0x06000290 RID: 656 RVA: 0x000247DC File Offset: 0x000229DC
	private void OnGUI()
	{
		if (this.timeout > 0f && UnityEngine.Time.unscaledTime >= this.timeout)
		{
			if (this.queue.Count > 0)
			{
				this.Value = this.queue[0];
				this.queue.RemoveAt(0);
				this.timeout = UnityEngine.Time.unscaledTime + this.Duration;
			}
			else
			{
				this.Value = -1;
				this.timeout = -1f;
			}
		}
		int num = this.Value;
		if (num == -1)
		{
			return;
		}
		if (num == -2)
		{
			num = (int)UnityEngine.Time.unscaledTime;
		}
		if (num != this.tex_val || this.tex == null || this.tex.width != this.IMG_WIDTH || this.tex.height != this.IMG_HEIGHT)
		{
			this.FillTexture(num);
		}
		float num2 = (float)Screen.height / 1080f;
		float num3 = (float)this.IMG_WIDTH * num2;
		float height = (float)this.IMG_HEIGHT * num2;
		GUI.DrawTexture(new Rect((float)Screen.width - num3 - 3f, 1f, num3, height), this.tex);
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000248F8 File Offset: 0x00022AF8
	private void FillTexture(int value)
	{
		this.tex_val = value;
		if (this.tex == null || this.tex.width != this.IMG_WIDTH || this.tex.height != this.IMG_HEIGHT)
		{
			this.tex = new Texture2D(this.IMG_WIDTH, this.IMG_HEIGHT, TextureFormat.RGB24, false);
		}
		if (this.colors == null || this.colors.Length != this.IMG_WIDTH * this.IMG_HEIGHT)
		{
			this.colors = new Color32[this.IMG_WIDTH * this.IMG_HEIGHT];
		}
		for (int i = 0; i < this.colors.Length; i++)
		{
			int num = i % this.IMG_WIDTH;
			int num2 = i / this.IMG_WIDTH;
			int num3 = this.TilesX - 1 - num / this.TileSize;
			int num4 = num2 / this.TileSize;
			int num5 = this.TilesX * num4 + num3;
			int num6 = 1 << num5;
			Color32 color = ((value & num6) != 0) ? this.clr1 : this.clr0;
			this.colors[i] = color;
		}
		this.tex.SetPixels32(this.colors);
		this.tex.Apply();
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00024A26 File Offset: 0x00022C26
	public static void Show(int value)
	{
		if (AnalyticsVisualClues.instance == null)
		{
			return;
		}
		AnalyticsVisualClues.instance.AddValue(value);
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00024A41 File Offset: 0x00022C41
	private void AddValue(int value)
	{
		if (this.timeout >= 0f)
		{
			this.queue.Add(value);
			return;
		}
		this.Value = value;
		this.timeout = UnityEngine.Time.unscaledTime + this.Duration;
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00024A78 File Offset: 0x00022C78
	public static void LoadSettings()
	{
		if (AnalyticsVisualClues.instance == null)
		{
			return;
		}
		DT.Field f = DT.Parser.LoadFieldFromFile(Game.defs_path + "AVCSettings.def", "AVCSettings");
		AnalyticsVisualClues.instance.LoadSettings(f);
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00024AB8 File Offset: 0x00022CB8
	private void LoadSettings(DT.Field f)
	{
		if (f == null)
		{
			return;
		}
		this.TileSize = f.GetInt("TileSize", null, this.TileSize, true, true, true, '.');
		this.TilesX = f.GetInt("TilesX", null, this.TilesX, true, true, true, '.');
		this.TilesY = f.GetInt("TilesY", null, this.TilesY, true, true, true, '.');
		this.Duration = f.GetFloat("Duration", null, this.Duration, true, true, true, '.');
		this.clr0 = global::Defs.GetColor(f, "clr0", this.clr0, null);
		this.clr1 = global::Defs.GetColor(f, "clr1", this.clr1, null);
	}

	// Token: 0x040003B7 RID: 951
	public int Value = -1;

	// Token: 0x040003B8 RID: 952
	public int TileSize = 8;

	// Token: 0x040003B9 RID: 953
	public int TilesX = 4;

	// Token: 0x040003BA RID: 954
	public int TilesY = 2;

	// Token: 0x040003BB RID: 955
	public float Duration = 0.5f;

	// Token: 0x040003BC RID: 956
	public Color32 clr0 = Color.magenta;

	// Token: 0x040003BD RID: 957
	public Color32 clr1 = Color.green;

	// Token: 0x040003BE RID: 958
	private Texture2D tex;

	// Token: 0x040003BF RID: 959
	private Color32[] colors;

	// Token: 0x040003C0 RID: 960
	private int tex_val;

	// Token: 0x040003C1 RID: 961
	private List<int> queue = new List<int>(32);

	// Token: 0x040003C2 RID: 962
	private float timeout = -1f;

	// Token: 0x040003C3 RID: 963
	public static AnalyticsVisualClues instance;
}
