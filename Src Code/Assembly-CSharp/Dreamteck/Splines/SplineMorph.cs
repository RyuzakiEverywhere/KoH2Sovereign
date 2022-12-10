using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004C2 RID: 1218
	[AddComponentMenu("Dreamteck/Splines/Morph")]
	public class SplineMorph : MonoBehaviour
	{
		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06004051 RID: 16465 RVA: 0x001EA468 File Offset: 0x001E8668
		// (set) Token: 0x06004052 RID: 16466 RVA: 0x001EA470 File Offset: 0x001E8670
		public SplineComputer spline
		{
			get
			{
				return this._spline;
			}
			set
			{
				if (Application.isPlaying && this.channels.Length != 0 && value.pointCount != this.channels[0].points.Length)
				{
					value.SetPoints(this.channels[0].points, this.space);
				}
				this._spline = value;
			}
		}

		// Token: 0x06004053 RID: 16467 RVA: 0x001EA4C4 File Offset: 0x001E86C4
		private void Reset()
		{
			this.spline = base.GetComponent<SplineComputer>();
		}

		// Token: 0x06004054 RID: 16468 RVA: 0x001EA4D2 File Offset: 0x001E86D2
		private void Update()
		{
			if (this.cycleUpdateMode == SplineMorph.UpdateMode.Update)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06004055 RID: 16469 RVA: 0x001EA4E2 File Offset: 0x001E86E2
		private void FixedUpdate()
		{
			if (this.cycleUpdateMode == SplineMorph.UpdateMode.FixedUpdate)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x001EA4F3 File Offset: 0x001E86F3
		private void LateUpdate()
		{
			if (this.cycleUpdateMode == SplineMorph.UpdateMode.LateUpdate)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06004057 RID: 16471 RVA: 0x001EA504 File Offset: 0x001E8704
		private void RunUpdate()
		{
			if (!this.cycle)
			{
				return;
			}
			if (this.cycleMode != SplineMorph.CycleMode.PingPong)
			{
				this.cycleDirection = 1;
			}
			this.cycleValue += Time.deltaTime / this.cycleDuration * (float)this.cycleDirection;
			switch (this.cycleMode)
			{
			case SplineMorph.CycleMode.Default:
				if (this.cycleValue > 1f)
				{
					this.cycleValue = 1f;
				}
				break;
			case SplineMorph.CycleMode.Loop:
				if (this.cycleValue > 1f)
				{
					this.cycleValue -= Mathf.Floor(this.cycleValue);
				}
				break;
			case SplineMorph.CycleMode.PingPong:
				if (this.cycleValue > 1f)
				{
					this.cycleValue = 1f - (this.cycleValue - Mathf.Floor(this.cycleValue));
					this.cycleDirection = -1;
				}
				else if (this.cycleValue < 0f)
				{
					this.cycleValue = -this.cycleValue - Mathf.Floor(-this.cycleValue);
					this.cycleDirection = 1;
				}
				break;
			}
			this.SetWeight(this.cycleValue, this.cycleMode == SplineMorph.CycleMode.Loop);
		}

		// Token: 0x06004058 RID: 16472 RVA: 0x001EA627 File Offset: 0x001E8827
		public void SetCycle(float value)
		{
			this.cycleValue = Mathf.Clamp01(value);
		}

		// Token: 0x06004059 RID: 16473 RVA: 0x001EA635 File Offset: 0x001E8835
		public void SetWeight(int index, float weight)
		{
			this.channels[index].percent = Mathf.Clamp01(weight);
			this.UpdateMorph();
		}

		// Token: 0x0600405A RID: 16474 RVA: 0x001EA650 File Offset: 0x001E8850
		public void SetWeight(string name, float weight)
		{
			int channelIndex = this.GetChannelIndex(name);
			this.channels[channelIndex].percent = Mathf.Clamp01(weight);
			this.UpdateMorph();
		}

		// Token: 0x0600405B RID: 16475 RVA: 0x001EA680 File Offset: 0x001E8880
		public void SetWeight(float percent, bool loop = false)
		{
			float num = percent * (float)(loop ? this.channels.Length : (this.channels.Length - 1));
			for (int i = 0; i < this.channels.Length; i++)
			{
				if (Mathf.Abs((float)i - num) > 1f)
				{
					this.SetWeight(i, 0f);
				}
				else if (num <= (float)i)
				{
					this.SetWeight(i, 1f - ((float)i - num));
				}
				else
				{
					this.SetWeight(i, 1f - (num - (float)i));
				}
			}
			if (loop && num >= (float)(this.channels.Length - 1))
			{
				this.SetWeight(0, num - (float)(this.channels.Length - 1));
			}
		}

		// Token: 0x0600405C RID: 16476 RVA: 0x001EA727 File Offset: 0x001E8927
		public void CaptureSnapshot(string name)
		{
			this.CaptureSnapshot(this.GetChannelIndex(name));
		}

		// Token: 0x0600405D RID: 16477 RVA: 0x001EA738 File Offset: 0x001E8938
		public void CaptureSnapshot(int index)
		{
			if (this._spline == null)
			{
				return;
			}
			if (this.channels.Length != 0 && this._spline.pointCount != this.channels[0].points.Length && index != 0)
			{
				Debug.LogError("Point count must be the same as " + this._spline.pointCount);
				return;
			}
			this.channels[index].points = this._spline.GetPoints(this.space);
			this.UpdateMorph();
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x001EA7C0 File Offset: 0x001E89C0
		public void Clear()
		{
			this.channels = new SplineMorph.Channel[0];
		}

		// Token: 0x0600405F RID: 16479 RVA: 0x001EA7CE File Offset: 0x001E89CE
		public SplinePoint[] GetSnapshot(int index)
		{
			return this.channels[index].points;
		}

		// Token: 0x06004060 RID: 16480 RVA: 0x001EA7E0 File Offset: 0x001E89E0
		public SplinePoint[] GetSnapshot(string name)
		{
			int channelIndex = this.GetChannelIndex(name);
			return this.channels[channelIndex].points;
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x001EA802 File Offset: 0x001E8A02
		public float GetWeight(int index)
		{
			return this.channels[index].percent;
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x001EA814 File Offset: 0x001E8A14
		public float GetWeight(string name)
		{
			int channelIndex = this.GetChannelIndex(name);
			return this.channels[channelIndex].percent;
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x001EA838 File Offset: 0x001E8A38
		public void AddChannel(string name)
		{
			if (this._spline == null)
			{
				return;
			}
			if (this.channels.Length != 0 && this._spline.pointCount != this.channels[0].points.Length)
			{
				Debug.LogError("Point count must be the same as " + this.channels[0].points.Length);
				return;
			}
			SplineMorph.Channel channel = new SplineMorph.Channel();
			channel.points = this._spline.GetPoints(this.space);
			channel.name = name;
			channel.curve = new AnimationCurve();
			channel.curve.AddKey(new Keyframe(0f, 0f, 0f, 1f));
			channel.curve.AddKey(new Keyframe(1f, 1f, 1f, 0f));
			ArrayUtility.Add<SplineMorph.Channel>(ref this.channels, channel);
			this.UpdateMorph();
		}

		// Token: 0x06004064 RID: 16484 RVA: 0x001EA928 File Offset: 0x001E8B28
		public void RemoveChannel(string name)
		{
			int channelIndex = this.GetChannelIndex(name);
			this.RemoveChannel(channelIndex);
		}

		// Token: 0x06004065 RID: 16485 RVA: 0x001EA944 File Offset: 0x001E8B44
		public void RemoveChannel(int index)
		{
			if (index < 0 || index >= this.channels.Length)
			{
				return;
			}
			SplineMorph.Channel[] array = new SplineMorph.Channel[this.channels.Length - 1];
			for (int i = 0; i < this.channels.Length; i++)
			{
				if (i != index)
				{
					if (i < index)
					{
						array[i] = this.channels[i];
					}
					else if (i >= index)
					{
						array[i - 1] = this.channels[i];
					}
				}
			}
			this.channels = array;
			this.UpdateMorph();
		}

		// Token: 0x06004066 RID: 16486 RVA: 0x001EA9B8 File Offset: 0x001E8BB8
		private int GetChannelIndex(string name)
		{
			for (int i = 0; i < this.channels.Length; i++)
			{
				if (this.channels[i].name == name)
				{
					return i;
				}
			}
			Debug.Log("Channel not found " + name);
			return 0;
		}

		// Token: 0x06004067 RID: 16487 RVA: 0x001EAA00 File Offset: 0x001E8C00
		public int GetChannelCount()
		{
			if (this.channels == null)
			{
				return 0;
			}
			return this.channels.Length;
		}

		// Token: 0x06004068 RID: 16488 RVA: 0x001EAA14 File Offset: 0x001E8C14
		public SplineMorph.Channel GetChannel(int index)
		{
			return this.channels[index];
		}

		// Token: 0x06004069 RID: 16489 RVA: 0x001EAA1E File Offset: 0x001E8C1E
		public SplineMorph.Channel GetChannel(string name)
		{
			return this.channels[this.GetChannelIndex(name)];
		}

		// Token: 0x0600406A RID: 16490 RVA: 0x001EAA30 File Offset: 0x001E8C30
		public void UpdateMorph()
		{
			if (this._spline == null)
			{
				return;
			}
			if (this.channels.Length == 0)
			{
				return;
			}
			if (this.points.Length != this.channels[0].points.Length)
			{
				this.points = new SplinePoint[this.channels[0].points.Length];
			}
			for (int i = 0; i < this.channels.Length; i++)
			{
				for (int j = 0; j < this.points.Length; j++)
				{
					if (i == 0)
					{
						this.points[j] = this.channels[0].points[j];
					}
					else
					{
						float num = this.channels[i].curve.Evaluate(this.channels[i].percent);
						if (this.channels[i].interpolation == SplineMorph.Channel.Interpolation.Linear)
						{
							SplinePoint[] array = this.points;
							int num2 = j;
							array[num2].position = array[num2].position + (this.channels[i].points[j].position - this.channels[0].points[j].position) * num;
							SplinePoint[] array2 = this.points;
							int num3 = j;
							array2[num3].tangent = array2[num3].tangent + (this.channels[i].points[j].tangent - this.channels[0].points[j].tangent) * num;
							SplinePoint[] array3 = this.points;
							int num4 = j;
							array3[num4].tangent2 = array3[num4].tangent2 + (this.channels[i].points[j].tangent2 - this.channels[0].points[j].tangent2) * num;
							SplinePoint[] array4 = this.points;
							int num5 = j;
							array4[num5].normal = array4[num5].normal + (this.channels[i].points[j].normal - this.channels[0].points[j].normal) * num;
						}
						else
						{
							this.points[j].position = Vector3.Slerp(this.points[j].position, this.points[j].position + (this.channels[i].points[j].position - this.channels[0].points[j].position), num);
							this.points[j].tangent = Vector3.Slerp(this.points[j].tangent, this.points[j].tangent + (this.channels[i].points[j].tangent - this.channels[0].points[j].tangent), num);
							this.points[j].tangent2 = Vector3.Slerp(this.points[j].tangent2, this.points[j].tangent2 + (this.channels[i].points[j].tangent2 - this.channels[0].points[j].tangent2), num);
							this.points[j].normal = Vector3.Slerp(this.points[j].normal, this.points[j].normal + (this.channels[i].points[j].normal - this.channels[0].points[j].normal), num);
						}
						SplinePoint[] array5 = this.points;
						int num6 = j;
						array5[num6].color = array5[num6].color + (this.channels[i].points[j].color - this.channels[0].points[j].color) * num;
						SplinePoint[] array6 = this.points;
						int num7 = j;
						array6[num7].size = array6[num7].size + (this.channels[i].points[j].size - this.channels[0].points[j].size) * num;
						if (this.points[j].type == SplinePoint.Type.SmoothMirrored)
						{
							this.points[j].type = this.channels[i].points[j].type;
						}
						else if (this.points[j].type == SplinePoint.Type.SmoothFree && this.channels[i].points[j].type == SplinePoint.Type.Broken)
						{
							this.points[j].type = SplinePoint.Type.Broken;
						}
					}
				}
			}
			for (int k = 0; k < this.points.Length; k++)
			{
				this.points[k].normal.Normalize();
			}
			this._spline.SetPoints(this.points, this.space);
		}

		// Token: 0x04002D1C RID: 11548
		[HideInInspector]
		public SplineComputer.Space space = SplineComputer.Space.Local;

		// Token: 0x04002D1D RID: 11549
		[HideInInspector]
		public bool cycle;

		// Token: 0x04002D1E RID: 11550
		[HideInInspector]
		public SplineMorph.CycleMode cycleMode;

		// Token: 0x04002D1F RID: 11551
		[HideInInspector]
		public SplineMorph.UpdateMode cycleUpdateMode;

		// Token: 0x04002D20 RID: 11552
		[HideInInspector]
		public float cycleDuration = 1f;

		// Token: 0x04002D21 RID: 11553
		[SerializeField]
		[HideInInspector]
		private SplineComputer _spline;

		// Token: 0x04002D22 RID: 11554
		private SplinePoint[] points = new SplinePoint[0];

		// Token: 0x04002D23 RID: 11555
		private float cycleValue;

		// Token: 0x04002D24 RID: 11556
		private short cycleDirection = 1;

		// Token: 0x04002D25 RID: 11557
		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("morphStates")]
		private SplineMorph.Channel[] channels = new SplineMorph.Channel[0];

		// Token: 0x020009A1 RID: 2465
		public enum CycleMode
		{
			// Token: 0x040044EB RID: 17643
			Default,
			// Token: 0x040044EC RID: 17644
			Loop,
			// Token: 0x040044ED RID: 17645
			PingPong
		}

		// Token: 0x020009A2 RID: 2466
		public enum UpdateMode
		{
			// Token: 0x040044EF RID: 17647
			Update,
			// Token: 0x040044F0 RID: 17648
			FixedUpdate,
			// Token: 0x040044F1 RID: 17649
			LateUpdate
		}

		// Token: 0x020009A3 RID: 2467
		[Serializable]
		public class Channel
		{
			// Token: 0x040044F2 RID: 17650
			[SerializeField]
			internal SplinePoint[] points = new SplinePoint[0];

			// Token: 0x040044F3 RID: 17651
			[SerializeField]
			internal float percent = 1f;

			// Token: 0x040044F4 RID: 17652
			public string name = "";

			// Token: 0x040044F5 RID: 17653
			public AnimationCurve curve;

			// Token: 0x040044F6 RID: 17654
			public SplineMorph.Channel.Interpolation interpolation;

			// Token: 0x02000A4A RID: 2634
			public enum Interpolation
			{
				// Token: 0x04004743 RID: 18243
				Linear,
				// Token: 0x04004744 RID: 18244
				Spherical
			}
		}
	}
}
