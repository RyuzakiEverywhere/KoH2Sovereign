using System;
using System.IO;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class MovieRecorder : MonoBehaviour
{
	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060010AB RID: 4267 RVA: 0x000B150D File Offset: 0x000AF70D
	public bool is_recording
	{
		get
		{
			return this.recording_prefix != null;
		}
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x000B1518 File Offset: 0x000AF718
	private void OnEnable()
	{
		MovieRecorder.instance = this;
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x000B1520 File Offset: 0x000AF720
	private void OnDisable()
	{
		if (MovieRecorder.instance == this)
		{
			MovieRecorder.instance = null;
		}
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x000B1538 File Offset: 0x000AF738
	private void Update()
	{
		if (string.IsNullOrEmpty(this.SaveFolder))
		{
			return;
		}
		this.UpdateInput();
		if (string.IsNullOrEmpty(this.recording_prefix))
		{
			return;
		}
		this.recording_index++;
		ScreenCapture.CaptureScreenshot(this.recording_prefix + this.recording_index.ToString("D05") + ".png", this.SuperSize);
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x000B15A0 File Offset: 0x000AF7A0
	public void BeginRecording(string prefix = "")
	{
		DateTime now = DateTime.Now;
		string text = string.Format("{0}{1:D04}-{2:D02}-{3:D02}-{4:D02}-{5:D02}-{6:D02}", new object[]
		{
			prefix,
			now.Year,
			now.Month,
			now.Day,
			now.Hour,
			now.Minute,
			now.Second
		});
		Directory.CreateDirectory(this.SaveFolder + "/" + text);
		this.recording_prefix = this.SaveFolder + "/" + text + "/frame";
		this.recording_index = 0;
		Debug.Log("Start recording: " + this.SaveFolder + "/" + text);
		Time.captureFramerate = this.FPS;
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x000B1682 File Offset: 0x000AF882
	public void EndRecording()
	{
		Debug.Log("End recording: " + this.recording_prefix + this.recording_index.ToString("D05"));
		this.recording_prefix = null;
		this.recording_index = 0;
		Time.captureFramerate = 0;
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x000B16C0 File Offset: 0x000AF8C0
	private void UpdateInput()
	{
		if (!Input.GetKeyDown(KeyCode.R))
		{
			return;
		}
		if (!UICommon.GetKey(KeyCode.LeftAlt, false) || !UICommon.GetKey(KeyCode.LeftControl, false))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.recording_prefix))
		{
			this.EndRecording();
			return;
		}
		this.BeginRecording("");
	}

	// Token: 0x04000B13 RID: 2835
	public static MovieRecorder instance;

	// Token: 0x04000B14 RID: 2836
	public string SaveFolder = "E:/BSG/Movies";

	// Token: 0x04000B15 RID: 2837
	public int FPS = 30;

	// Token: 0x04000B16 RID: 2838
	public int SuperSize = 1;

	// Token: 0x04000B17 RID: 2839
	public bool AutoRecord;

	// Token: 0x04000B18 RID: 2840
	private string recording_prefix;

	// Token: 0x04000B19 RID: 2841
	private int recording_index;
}
