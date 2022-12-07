using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000202 RID: 514
public class FPSCounterIMGUI : MonoBehaviour
{
	// Token: 0x06001F51 RID: 8017 RVA: 0x0012283C File Offset: 0x00120A3C
	private void Update()
	{
		this.fps.Add(1f / Time.unscaledDeltaTime);
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x00122854 File Offset: 0x00120A54
	public void NextMode()
	{
		if (!base.enabled)
		{
			base.enabled = true;
			this.verbose = false;
			return;
		}
		if (this.verbose)
		{
			base.enabled = false;
			return;
		}
		this.verbose = true;
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x00122884 File Offset: 0x00120A84
	private void OnGUI()
	{
		ulong num = 1048576UL;
		Rect position = new Rect((float)(Screen.width - 310), 70f, 300f, 300f);
		this.sb.Clear();
		if (!Mathf.Approximately(Time.timeScale, 1f))
		{
			if (Time.timeScale == 0f)
			{
				this.sb.Append("Paused ");
			}
			else
			{
				this.sb.Append(string.Format("Speed: {0:F1} ", Time.timeScale));
			}
		}
		this.sb.Append(string.Format("FPS: {0:F1} ({1:F1} - {2:F1})", this.fps.avg, this.fps.min, this.fps.max));
		if (this.verbose)
		{
			this.sb.Append("\nGPU: " + SystemInfo.graphicsDeviceName);
			this.sb.Append(string.Format("\nResolution: {0}x{1}", Screen.width, Screen.height));
			this.sb.Append(string.Format("\nGPU Memory: {0} MB", SystemInfo.graphicsMemorySize));
			this.sb.Append(string.Format("\nTextures: {0}, {1} MB", Texture.nonStreamingTextureCount, Texture.currentTextureMemory / num));
			this.sb.Append(string.Format("\nCPU Mem: {0} MB", Profiler.GetTotalAllocatedMemoryLong() / (long)num));
		}
		if (this.txt_style == null)
		{
			this.txt_style = new GUIStyle(GUI.skin.label);
			this.txt_style.alignment = TextAnchor.UpperRight;
		}
		GUI.Label(position, this.sb.ToString(), this.txt_style);
	}

	// Token: 0x040014CF RID: 5327
	private global::Sampler fps = new global::Sampler(25);

	// Token: 0x040014D0 RID: 5328
	private StringBuilder sb = new StringBuilder(16384);

	// Token: 0x040014D1 RID: 5329
	private bool verbose;

	// Token: 0x040014D2 RID: 5330
	private GUIStyle txt_style;
}
