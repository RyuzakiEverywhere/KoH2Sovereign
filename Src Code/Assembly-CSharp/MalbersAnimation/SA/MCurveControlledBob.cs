using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000455 RID: 1109
	[Serializable]
	public class MCurveControlledBob
	{
		// Token: 0x06003AD1 RID: 15057 RVA: 0x001C4380 File Offset: 0x001C2580
		public void Setup(Camera camera, float bobBaseInterval)
		{
			this.m_BobBaseInterval = bobBaseInterval;
			this.m_OriginalCameraPosition = camera.transform.localPosition;
			this.m_Time = this.Bobcurve[this.Bobcurve.length - 1].time;
		}

		// Token: 0x06003AD2 RID: 15058 RVA: 0x001C43CC File Offset: 0x001C25CC
		public Vector3 DoHeadBob(float speed)
		{
			float x = this.m_OriginalCameraPosition.x + this.Bobcurve.Evaluate(this.m_CyclePositionX) * this.HorizontalBobRange;
			float y = this.m_OriginalCameraPosition.y + this.Bobcurve.Evaluate(this.m_CyclePositionY) * this.VerticalBobRange;
			this.m_CyclePositionX += speed * Time.deltaTime / this.m_BobBaseInterval;
			this.m_CyclePositionY += speed * Time.deltaTime / this.m_BobBaseInterval * this.VerticaltoHorizontalRatio;
			if (this.m_CyclePositionX > this.m_Time)
			{
				this.m_CyclePositionX -= this.m_Time;
			}
			if (this.m_CyclePositionY > this.m_Time)
			{
				this.m_CyclePositionY -= this.m_Time;
			}
			return new Vector3(x, y, 0f);
		}

		// Token: 0x04002A94 RID: 10900
		public float HorizontalBobRange = 0.33f;

		// Token: 0x04002A95 RID: 10901
		public float VerticalBobRange = 0.33f;

		// Token: 0x04002A96 RID: 10902
		public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.5f, 1f),
			new Keyframe(1f, 0f),
			new Keyframe(1.5f, -1f),
			new Keyframe(2f, 0f)
		});

		// Token: 0x04002A97 RID: 10903
		public float VerticaltoHorizontalRatio = 1f;

		// Token: 0x04002A98 RID: 10904
		private float m_CyclePositionX;

		// Token: 0x04002A99 RID: 10905
		private float m_CyclePositionY;

		// Token: 0x04002A9A RID: 10906
		private float m_BobBaseInterval;

		// Token: 0x04002A9B RID: 10907
		private Vector3 m_OriginalCameraPosition;

		// Token: 0x04002A9C RID: 10908
		private float m_Time;
	}
}
