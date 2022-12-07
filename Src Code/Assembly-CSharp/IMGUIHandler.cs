using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class IMGUIHandler : MonoBehaviour
{
	// Token: 0x06000477 RID: 1143 RVA: 0x00034DF4 File Offset: 0x00032FF4
	public static IMGUIHandler Get()
	{
		if (IMGUIHandler.instance == null)
		{
			GameObject gameObject = GameObject.Find("IMGUIHandler");
			if (gameObject == null)
			{
				gameObject = new GameObject("IMGUIHandler");
			}
			gameObject.hideFlags = HideFlags.DontSave;
			IMGUIHandler.instance = gameObject.GetComponent<IMGUIHandler>();
			if (IMGUIHandler.instance == null)
			{
				IMGUIHandler.instance = gameObject.AddComponent<IMGUIHandler>();
			}
			return IMGUIHandler.instance;
		}
		return IMGUIHandler.instance;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00034E63 File Offset: 0x00033063
	public static void AddRect(Rect t)
	{
		if (IMGUIHandler.instance != null)
		{
			IMGUIHandler.instance.rects.Add(t);
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00034E84 File Offset: 0x00033084
	public static bool IsPointerOverIMGUI()
	{
		if (IMGUIHandler.instance == null)
		{
			return false;
		}
		Camera mainCamera = CameraController.MainCamera;
		if (mainCamera == null)
		{
			return false;
		}
		Vector3 point = new Vector3(Input.mousePosition.x, (float)mainCamera.pixelHeight - Input.mousePosition.y, 0f);
		foreach (Rect rect in IMGUIHandler.instance.rects)
		{
			if (rect.Contains(point))
			{
				return true;
			}
		}
		IMGUIHandler imguihandler = IMGUIHandler.instance;
		if (imguihandler != null)
		{
			imguihandler.rects.Clear();
		}
		return false;
	}

	// Token: 0x04000475 RID: 1141
	private static IMGUIHandler instance;

	// Token: 0x04000476 RID: 1142
	public List<Rect> rects = new List<Rect>();
}
