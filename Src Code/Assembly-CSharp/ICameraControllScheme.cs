using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public interface ICameraControllScheme
{
	// Token: 0x06000AAF RID: 2735
	void UpdateInput(Transform camTransform, CameraSettings cameraSettings);

	// Token: 0x06000AB0 RID: 2736
	void UpdateCamera(Transform camTransform, CameraSettings cameraSettings);

	// Token: 0x06000AB1 RID: 2737
	void LookAt(Vector3 point);

	// Token: 0x06000AB2 RID: 2738
	void Zoom(float zoom);

	// Token: 0x06000AB3 RID: 2739
	void Yaw(float yaw);

	// Token: 0x06000AB4 RID: 2740
	void Set(Vector3 pos, Quaternion rot);

	// Token: 0x06000AB5 RID: 2741
	void RecalcCameraSettings();

	// Token: 0x06000AB6 RID: 2742
	float GetDistanceToAimPoint();

	// Token: 0x06000AB7 RID: 2743
	void Reset();

	// Token: 0x06000AB8 RID: 2744
	Vector3 GetLookAtPoint();

	// Token: 0x06000AB9 RID: 2745
	Vector3 GetScreenToWorldPoint(Vector2 screenPoint);

	// Token: 0x06000ABA RID: 2746
	KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom);

	// Token: 0x06000ABB RID: 2747
	ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation();
}
