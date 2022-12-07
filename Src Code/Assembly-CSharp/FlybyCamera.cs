using System;
using BezierSolution;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class FlybyCamera : MonoBehaviour
{
	// Token: 0x06000805 RID: 2053 RVA: 0x0005542C File Offset: 0x0005362C
	public void Begin(BezierSpline path, AnimationCurve zoomCurve, AnimationCurve yawCurve, AnimationCurve speedCurve)
	{
		if (zoomCurve != null)
		{
			this.m_ZoomCurve = zoomCurve;
		}
		if (yawCurve != null)
		{
			this.m_YawCurve = yawCurve;
		}
		if (speedCurve != null)
		{
			this.m_SpeedCurve = speedCurve;
		}
		this.Spline = path;
		base.enabled = true;
		this.inProgress = true;
		this.progress = 0f;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00055479 File Offset: 0x00053679
	public void Stop()
	{
		this.inProgress = false;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00055482 File Offset: 0x00053682
	public void SetFocusPoint(Vector3 focusPoint)
	{
		this.currenFocusPoint = new Vector3?(focusPoint);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00055490 File Offset: 0x00053690
	private void Complete()
	{
		this.inProgress = false;
		base.enabled = false;
		if (this.OnComplete != null)
		{
			this.OnComplete();
		}
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x000554B4 File Offset: 0x000536B4
	public void Update()
	{
		if (!this.inProgress)
		{
			base.enabled = false;
			return;
		}
		if (this.Spline != null)
		{
			float num = this.speed_mod;
			CameraController.LookAt(this.Spline.MoveAlongSpline(ref this.progress, this.targetSpeed * num * Time.unscaledDeltaTime, 3));
			if (this.progress >= 1f)
			{
				this.Complete();
				return;
			}
		}
		if (this.currenFocusPoint != null)
		{
			base.transform.LookAt(this.currenFocusPoint.Value);
		}
	}

	// Token: 0x0400065C RID: 1628
	public Action OnComplete;

	// Token: 0x0400065D RID: 1629
	private Vector3? currenFocusPoint;

	// Token: 0x0400065E RID: 1630
	public BezierSpline Spline;

	// Token: 0x0400065F RID: 1631
	private float progress;

	// Token: 0x04000660 RID: 1632
	public float travelTime = 10f;

	// Token: 0x04000661 RID: 1633
	public float targetSpeed = 60f;

	// Token: 0x04000662 RID: 1634
	public float speed_mod = 1f;

	// Token: 0x04000663 RID: 1635
	[SerializeField]
	private AnimationCurve m_ZoomCurve;

	// Token: 0x04000664 RID: 1636
	[SerializeField]
	private AnimationCurve m_YawCurve;

	// Token: 0x04000665 RID: 1637
	[SerializeField]
	private AnimationCurve m_SpeedCurve;

	// Token: 0x04000666 RID: 1638
	private bool inProgress;
}
