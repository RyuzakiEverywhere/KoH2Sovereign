using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001BD RID: 445
public class GameSpeedUI : MonoBehaviour
{
	// Token: 0x06001A5C RID: 6748 RVA: 0x000FF028 File Offset: 0x000FD228
	private void Start()
	{
		GameSpeed.OnPaused += this.HandleOnPaused;
		GameSpeed.OnSpeedChange += this.HandeOnSpeedChange;
		this.Button_Paused.onClick.AddListener(new UnityAction(this.OnStopHandler));
		this.Button_Play.onClick.AddListener(new UnityAction(this.OnPlayHandler));
		this.Button_SpeedDown.onClick.AddListener(new UnityAction(this.OnSpeedDownHandler));
		this.Button_SpeedUp.onClick.AddListener(new UnityAction(this.OnSpeedUpHandler));
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x000FF0C8 File Offset: 0x000FD2C8
	private void Update()
	{
		if (this.Value_FramesPerSec != null)
		{
			this.Value_FramesPerSec.text = "FPS: " + this.SampleFPS().ToString("F1");
		}
		if (UnityEngine.Time.timeScale == 0f)
		{
			this.Value_GameSpeed.text = string.Format("Paused", UnityEngine.Time.timeScale);
			return;
		}
		this.Value_GameSpeed.text = string.Format("x{0,5:N2}", UnityEngine.Time.timeScale);
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x000FF158 File Offset: 0x000FD358
	private float SampleFPS()
	{
		if (this.samples.Count >= 25)
		{
			this.sum -= this.samples[0];
			this.samples.RemoveAt(0);
		}
		float num = 1f / UnityEngine.Time.unscaledDeltaTime;
		this.sum += num;
		this.samples.Add(num);
		return this.sum / (float)this.samples.Count;
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x000FF1D4 File Offset: 0x000FD3D4
	private void Refresh()
	{
		Game game = GameLogic.Get(true);
		if (game.IsPaused())
		{
			this.Value_GameSpeed.text = string.Format("Paused", game.GetSpeed());
			return;
		}
		this.Value_GameSpeed.text = string.Format("x{0,5:N2}", game.GetSpeed());
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000FF231 File Offset: 0x000FD431
	private void HandeOnSpeedChange(float obj)
	{
		this.Refresh();
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000FF231 File Offset: 0x000FD431
	private void HandleOnPaused(bool obj)
	{
		this.Refresh();
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000FF239 File Offset: 0x000FD439
	private void OnPlayHandler()
	{
		GameLogic.Get(true).pause.DelRequest("ManualPause", -2);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x000FF252 File Offset: 0x000FD452
	private void OnStopHandler()
	{
		GameLogic.Get(true).pause.AddRequest("ManualPause", -2);
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x000FF26C File Offset: 0x000FD46C
	private void OnSpeedDownHandler()
	{
		Game game = GameLogic.Get(true);
		float nextSpeedDown = GameSpeed.GetNextSpeedDown(game);
		game.SetSpeed(nextSpeedDown, -1);
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x000FF290 File Offset: 0x000FD490
	private void OnSpeedUpHandler()
	{
		Game game = GameLogic.Get(true);
		float nextSpeedUp = GameSpeed.GetNextSpeedUp(game);
		game.SetSpeed(nextSpeedUp, -1);
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000FF2B4 File Offset: 0x000FD4B4
	private void OnDestroy()
	{
		GameSpeed.OnPaused -= this.HandleOnPaused;
		GameSpeed.OnSpeedChange -= this.HandeOnSpeedChange;
		this.Button_Paused.onClick.RemoveListener(new UnityAction(this.OnStopHandler));
		this.Button_Play.onClick.RemoveListener(new UnityAction(this.OnPlayHandler));
		this.Button_SpeedDown.onClick.RemoveListener(new UnityAction(this.OnSpeedDownHandler));
		this.Button_SpeedUp.onClick.RemoveListener(new UnityAction(this.OnSpeedUpHandler));
	}

	// Token: 0x040010E2 RID: 4322
	private List<float> samples = new List<float>();

	// Token: 0x040010E3 RID: 4323
	private float sum;

	// Token: 0x040010E4 RID: 4324
	[SerializeField]
	public Button Button_Paused;

	// Token: 0x040010E5 RID: 4325
	[SerializeField]
	public Button Button_Play;

	// Token: 0x040010E6 RID: 4326
	[SerializeField]
	public Button Button_SpeedDown;

	// Token: 0x040010E7 RID: 4327
	[SerializeField]
	public Button Button_SpeedUp;

	// Token: 0x040010E8 RID: 4328
	[SerializeField]
	public Text Value_GameSpeed;

	// Token: 0x040010E9 RID: 4329
	[SerializeField]
	public Text Value_FramesPerSec;
}
