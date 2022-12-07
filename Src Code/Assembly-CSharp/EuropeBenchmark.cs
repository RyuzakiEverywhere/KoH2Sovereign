using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class EuropeBenchmark : MonoBehaviour
{
	// Token: 0x06000196 RID: 406 RVA: 0x0000FCE2 File Offset: 0x0000DEE2
	private void Start()
	{
		EuropeBenchmark.Instance = this;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000FCEA File Offset: 0x0000DEEA
	private void OnDestroy()
	{
		if (EuropeBenchmark.Instance == this)
		{
			EuropeBenchmark.Instance = null;
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000FCFF File Offset: 0x0000DEFF
	public void Benchmark(Action<List<float>> on_complete)
	{
		CameraController.GameCamera.gameObject.AddComponent<EuropeBenchmark.EuropeBenchmarkPerfomer>().Benchmark(this.camera_presets, on_complete);
	}

	// Token: 0x040002B2 RID: 690
	public static EuropeBenchmark Instance;

	// Token: 0x040002B3 RID: 691
	[SerializeField]
	private CameraPresets camera_presets;

	// Token: 0x02000502 RID: 1282
	public class EuropeBenchmarkPerfomer : MonoBehaviour
	{
		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x0600426B RID: 17003 RVA: 0x001F8CB4 File Offset: 0x001F6EB4
		public IReadOnlyList<float> Measures
		{
			get
			{
				return this.measures;
			}
		}

		// Token: 0x0600426C RID: 17004 RVA: 0x001C6F42 File Offset: 0x001C5142
		private void Awake()
		{
			base.enabled = false;
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x001F8CBC File Offset: 0x001F6EBC
		public void Benchmark(CameraPresets camera_presets, Action<List<float>> on_complete)
		{
			this.camera_presets = camera_presets;
			this.on_complete = on_complete;
			this.measures.Clear();
			base.enabled = true;
			this.viewIndex = 0;
			this.viewTestIndex = 0;
			this.stopwatch = Stopwatch.StartNew();
		}

		// Token: 0x0600426E RID: 17006 RVA: 0x001F8CF8 File Offset: 0x001F6EF8
		private void OnPreCull()
		{
			this.camera_presets.ApplyCameraPreset(this.viewIndex);
			this.stopwatch.Restart();
			this.viewTestIndex++;
			if (this.viewTestIndex >= 300)
			{
				this.viewTestIndex = 0;
				this.viewIndex++;
			}
			if (this.viewIndex >= this.camera_presets.presets.Count)
			{
				Action<List<float>> action = this.on_complete;
				if (action != null)
				{
					action(this.measures);
				}
				Object.Destroy(this);
			}
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x001F8D88 File Offset: 0x001F6F88
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.measures.Add((float)this.stopwatch.Elapsed.TotalMilliseconds);
			Graphics.Blit(source, destination);
		}

		// Token: 0x04002E81 RID: 11905
		private List<float> measures = new List<float>();

		// Token: 0x04002E82 RID: 11906
		private Action<List<float>> on_complete;

		// Token: 0x04002E83 RID: 11907
		private CameraPresets camera_presets;

		// Token: 0x04002E84 RID: 11908
		private Stopwatch stopwatch;

		// Token: 0x04002E85 RID: 11909
		private int viewTestIndex;

		// Token: 0x04002E86 RID: 11910
		private int viewIndex;

		// Token: 0x04002E87 RID: 11911
		private const int SINGLE_VIEW_TESTS_COUNT = 300;
	}
}
