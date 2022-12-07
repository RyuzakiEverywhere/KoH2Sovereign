using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000B1 RID: 177
public static class UICommon
{
	// Token: 0x0600063A RID: 1594
	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);

	// Token: 0x0600063B RID: 1595 RVA: 0x00042F1D File Offset: 0x0004111D
	public static bool IsWinKeyDown(UICommon.WinKeyCode key)
	{
		return ((int)UICommon.GetAsyncKeyState((int)key) & 32768) != 0;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00042F30 File Offset: 0x00041130
	public static bool IsInInput()
	{
		EventSystem current = EventSystem.current;
		GameObject gameObject = (current != null) ? current.currentSelectedGameObject : null;
		if (gameObject != null)
		{
			InputField component = gameObject.GetComponent<InputField>();
			if (component != null && component.isFocused)
			{
				return true;
			}
			TMP_InputField component2 = gameObject.GetComponent<TMP_InputField>();
			if (component2 != null && component2.isFocused)
			{
				return true;
			}
		}
		return UIPreferences.KeyBindPreference.disableOtherInput || UICommon.ConsoleVisible() || UIBugReportWindow.IsActive();
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00042FA8 File Offset: 0x000411A8
	public static bool IsInMenuOrBugReport()
	{
		BaseUI baseUI = BaseUI.Get();
		return !(baseUI == null) && (baseUI.IsMainMenuOpen() || UIBugReportWindow.IsActive());
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00042FDC File Offset: 0x000411DC
	public static bool ConsoleVisible()
	{
		AttributeConsoleManager instance = AttributeConsoleManager.instance;
		if (instance == null)
		{
			return false;
		}
		AttributeConsoleGUI component = instance.gameObject.GetComponent<AttributeConsoleGUI>();
		return !(component == null) && component.display;
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x0004301C File Offset: 0x0004121C
	public static bool GetModifierKey(UICommon.ModifierKey mk)
	{
		if (mk <= UICommon.ModifierKey.LeftAlt)
		{
			switch (mk)
			{
			case UICommon.ModifierKey.Ctrl:
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_CONTROL);
			case UICommon.ModifierKey.Alt:
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_MENU);
			case UICommon.ModifierKey.Ctrl | UICommon.ModifierKey.Alt:
				break;
			case UICommon.ModifierKey.Shift:
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_SHIFT);
			default:
				if (mk == UICommon.ModifierKey.LeftCtrl)
				{
					return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LCONTROL);
				}
				if (mk == UICommon.ModifierKey.LeftAlt)
				{
					return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LMENU);
				}
				break;
			}
		}
		else if (mk <= UICommon.ModifierKey.RightCtrl)
		{
			if (mk == UICommon.ModifierKey.LeftShift)
			{
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LSHIFT);
			}
			if (mk == UICommon.ModifierKey.RightCtrl)
			{
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RCONTROL);
			}
		}
		else
		{
			if (mk == UICommon.ModifierKey.RightAlt)
			{
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RMEMU);
			}
			if (mk == UICommon.ModifierKey.RightShift)
			{
				return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RSHIFT);
			}
		}
		Debug.LogError(string.Format("Unknown modifier key: {0}", mk));
		return false;
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x000430E4 File Offset: 0x000412E4
	public static UICommon.ModifierKey GetModifierKeys()
	{
		UICommon.ModifierKey modifierKey = UICommon.ModifierKey.None;
		if (UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl))
		{
			modifierKey |= UICommon.ModifierKey.Ctrl;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.Alt))
		{
			modifierKey |= UICommon.ModifierKey.Alt;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
		{
			modifierKey |= UICommon.ModifierKey.Shift;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.LeftCtrl))
		{
			modifierKey |= UICommon.ModifierKey.LeftCtrl;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.LeftAlt))
		{
			modifierKey |= UICommon.ModifierKey.LeftAlt;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.LeftShift))
		{
			modifierKey |= UICommon.ModifierKey.LeftShift;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.RightCtrl))
		{
			modifierKey |= UICommon.ModifierKey.RightCtrl;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.RightAlt))
		{
			modifierKey |= UICommon.ModifierKey.RightAlt;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.RightShift))
		{
			modifierKey |= UICommon.ModifierKey.RightShift;
		}
		return modifierKey;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00043178 File Offset: 0x00041378
	public static void DockTo(RectTransform source, RectTransform target, TextAnchor dockPoint, Vector2 offset)
	{
		Vector3 a = target.transform.position;
		Vector3[] array = new Vector3[4];
		target.GetWorldCorners(array);
		switch (dockPoint)
		{
		case TextAnchor.UpperLeft:
			a = array[1];
			break;
		case TextAnchor.UpperCenter:
			a = Vector3.Lerp(array[1], array[2], 0.5f);
			break;
		case TextAnchor.UpperRight:
			a = array[2];
			break;
		case TextAnchor.MiddleLeft:
			a = Vector3.Lerp(array[0], array[1], 0.5f);
			break;
		case TextAnchor.MiddleCenter:
			a = Vector3.Lerp(array[0], array[2], 0.5f);
			break;
		case TextAnchor.MiddleRight:
			a = Vector3.Lerp(array[2], array[3], 0.5f);
			break;
		case TextAnchor.LowerLeft:
			a = array[0];
			break;
		case TextAnchor.LowerCenter:
			a = Vector3.Lerp(array[0], array[3], 0.5f);
			break;
		case TextAnchor.LowerRight:
			a = array[3];
			break;
		}
		source.transform.position = a + new Vector3(offset.x, offset.y, 0f);
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000432A8 File Offset: 0x000414A8
	public static UICommon.ModifierKey AdjustAllowed(UICommon.ModifierKey allowed)
	{
		UICommon.ModifierKey modifierKey = allowed;
		if ((allowed & UICommon.ModifierKey.Ctrl) == UICommon.ModifierKey.Ctrl)
		{
			modifierKey |= UICommon.ModifierKey.LeftCtrl;
			modifierKey |= UICommon.ModifierKey.RightCtrl;
		}
		else
		{
			if ((allowed & UICommon.ModifierKey.LeftCtrl) == UICommon.ModifierKey.LeftCtrl)
			{
				modifierKey |= UICommon.ModifierKey.Ctrl;
			}
			if ((allowed & UICommon.ModifierKey.RightCtrl) == UICommon.ModifierKey.RightCtrl)
			{
				modifierKey |= UICommon.ModifierKey.Ctrl;
			}
		}
		if ((allowed & UICommon.ModifierKey.Alt) == UICommon.ModifierKey.Alt)
		{
			modifierKey |= UICommon.ModifierKey.LeftAlt;
			modifierKey |= UICommon.ModifierKey.RightAlt;
		}
		else
		{
			if ((allowed & UICommon.ModifierKey.LeftAlt) == UICommon.ModifierKey.LeftAlt)
			{
				modifierKey |= UICommon.ModifierKey.Alt;
			}
			if ((allowed & UICommon.ModifierKey.RightAlt) == UICommon.ModifierKey.RightAlt)
			{
				modifierKey |= UICommon.ModifierKey.Alt;
			}
		}
		if ((allowed & UICommon.ModifierKey.Shift) == UICommon.ModifierKey.Shift)
		{
			modifierKey |= UICommon.ModifierKey.LeftShift;
			modifierKey |= UICommon.ModifierKey.RightShift;
		}
		else
		{
			if ((allowed & UICommon.ModifierKey.LeftShift) == UICommon.ModifierKey.LeftShift)
			{
				modifierKey |= UICommon.ModifierKey.Shift;
			}
			if ((allowed & UICommon.ModifierKey.RightShift) == UICommon.ModifierKey.RightShift)
			{
				modifierKey |= UICommon.ModifierKey.Shift;
			}
		}
		return modifierKey;
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00043348 File Offset: 0x00041548
	public static bool CheckModifierKeys(UICommon.ModifierKey current, UICommon.ModifierKey required, UICommon.ModifierKey allowed)
	{
		if ((current & required) != required)
		{
			return false;
		}
		allowed |= required;
		UICommon.ModifierKey modifierKey = UICommon.AdjustAllowed(allowed);
		return (current & modifierKey) == current;
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00043372 File Offset: 0x00041572
	public static bool CheckModifierKeys(UICommon.ModifierKey required, UICommon.ModifierKey allowed)
	{
		return UICommon.CheckModifierKeys(UICommon.GetModifierKeys(), required, allowed);
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00043380 File Offset: 0x00041580
	public static bool GetKey(KeyCode key, bool ignoreInputFocused = false)
	{
		if (!ignoreInputFocused && UICommon.IsInInput())
		{
			return false;
		}
		switch (key)
		{
		case KeyCode.RightShift:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RSHIFT);
		case KeyCode.LeftShift:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LSHIFT);
		case KeyCode.RightControl:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RCONTROL);
		case KeyCode.LeftControl:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LCONTROL);
		case KeyCode.RightAlt:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_RMEMU);
		case KeyCode.LeftAlt:
			return UICommon.IsWinKeyDown(UICommon.WinKeyCode.VK_LMENU);
		default:
			return Input.GetKey(key);
		}
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00043408 File Offset: 0x00041608
	public static bool GetKeyDown(KeyCode key, UICommon.ModifierKey required = UICommon.ModifierKey.None, UICommon.ModifierKey allowed = UICommon.ModifierKey.None)
	{
		if (!Input.GetKeyDown(key))
		{
			return false;
		}
		if (UICommon.IsInInput())
		{
			return false;
		}
		if (key != KeyCode.Escape)
		{
			EventSystem current = EventSystem.current;
			if (((current != null) ? current.currentSelectedGameObject : null) != null)
			{
				return false;
			}
		}
		return UICommon.CheckModifierKeys(required, allowed);
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00043454 File Offset: 0x00041654
	public static bool GetKeyUp(KeyCode key, UICommon.ModifierKey required = UICommon.ModifierKey.None, UICommon.ModifierKey allowed = UICommon.ModifierKey.None)
	{
		if (!Input.GetKeyUp(key))
		{
			return false;
		}
		if (UICommon.IsInInput())
		{
			return false;
		}
		if (key != KeyCode.Escape)
		{
			EventSystem current = EventSystem.current;
			if (((current != null) ? current.currentSelectedGameObject : null) != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
			{
				return false;
			}
		}
		return UICommon.CheckModifierKeys(required, allowed);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x000434B8 File Offset: 0x000416B8
	public static void FindComponents(UnityEngine.Component obj, bool incleBaseTypes = false)
	{
		Type type = obj.GetType();
		FieldInfo[] array;
		if (incleBaseTypes)
		{
			UICommon.reusableFieldInfoList.Clear();
			UICommon.reusableFieldInfoList.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			while (type.BaseType != null && type.BaseType.IsSubclassOf(typeof(UnityEngine.Component)))
			{
				UICommon.reusableFieldInfoList.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				type = type.BaseType;
			}
			array = UICommon.reusableFieldInfoList.ToArray();
			UICommon.reusableFieldInfoList.Clear();
		}
		else
		{
			array = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.IsDefined(typeof(UIFieldTarget), false))
			{
				object value = fieldInfo.GetValue(obj);
				if (value == null || value.ToString() == "null" || fieldInfo.FieldType.IsArray || (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
				{
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(UIFieldTarget), false);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						string objectName = (customAttributes[j] as UIFieldTarget).objectName;
						if (fieldInfo.FieldType.IsArray)
						{
							UICommon.reusableList.Clear();
							global::Common.FindChildrenByName(UICommon.reusableList, obj.gameObject, objectName, true, false);
							Array array2 = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), UICommon.reusableList.Count);
							Type elementType = array2.GetType().GetElementType();
							for (int k = 0; k < UICommon.reusableList.Count; k++)
							{
								if (fieldInfo.FieldType.GetElementType().Equals(typeof(GameObject)))
								{
									array2.SetValue(UICommon.reusableList[k], k);
								}
								else
								{
									UnityEngine.Component component = UICommon.reusableList[k].GetComponent(elementType);
									array2.SetValue(component, k);
								}
							}
							fieldInfo.SetValue(obj, array2);
						}
						else if (!fieldInfo.FieldType.IsGenericType || !(fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
						{
							GameObject gameObject = global::Common.FindChildByName(obj.gameObject, objectName, true, true);
							if (gameObject != null)
							{
								if (fieldInfo.FieldType.Equals(typeof(GameObject)))
								{
									fieldInfo.SetValue(obj, gameObject);
								}
								else
								{
									UnityEngine.Component component2 = gameObject.GetComponent(fieldInfo.FieldType);
									fieldInfo.SetValue(obj, component2);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00043760 File Offset: 0x00041960
	public static bool HasActiveChildren(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		if (go.transform.childCount == 0)
		{
			return false;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			if (!UICommon.IsFakeChild(go) && child.gameObject.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000437C4 File Offset: 0x000419C4
	public static Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
		return new Rect(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0004381E File Offset: 0x00041A1E
	public static bool IsFakeChild(GameObject go)
	{
		return go != null && go.name.StartsWith("UIP_TutorialHighlight", StringComparison.Ordinal);
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0004383C File Offset: 0x00041A3C
	public static void DeleteChildren(Transform t)
	{
		if (t == null)
		{
			return;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			GameObject gameObject = t.GetChild(i).gameObject;
			if (!UICommon.IsFakeChild(gameObject))
			{
				ObjectPool.DestroyObj(gameObject);
			}
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00043884 File Offset: 0x00041A84
	public static void DeleteActiveChildren(Transform t)
	{
		if (t == null)
		{
			return;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			GameObject gameObject = t.GetChild(i).gameObject;
			if (gameObject.activeSelf && !UICommon.IsFakeChild(gameObject))
			{
				ObjectPool.DestroyObj(gameObject);
			}
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x000438D4 File Offset: 0x00041AD4
	public static bool DeleteChildren(Transform t, Type componentType)
	{
		bool result = false;
		if (t == null)
		{
			return result;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			GameObject gameObject = t.GetChild(i).gameObject;
			if (gameObject.GetComponent(componentType) != null)
			{
				ObjectPool.DestroyObj(gameObject);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00043928 File Offset: 0x00041B28
	public static void SetActiveChildren(Transform t, bool active)
	{
		if (t == null)
		{
			return;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			t.GetChild(i).gameObject.SetActive(active);
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00043964 File Offset: 0x00041B64
	public static GameObject LoadPrefab(string componentId, Transform parent)
	{
		GameObject prefab = UICommon.GetPrefab(componentId, null);
		if (prefab == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0004398C File Offset: 0x00041B8C
	public static GameObject GetPrefab(string componentId, string varinat = null)
	{
		if (string.IsNullOrEmpty(componentId))
		{
			return null;
		}
		DT.Field defField = global::Defs.GetDefField(componentId, null);
		if (defField == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(componentId))
		{
			GameObject obj = global::Defs.GetObj<GameObject>(defField, "window_prefab." + varinat, null);
			if (obj != null)
			{
				return obj;
			}
		}
		return global::Defs.GetObj<GameObject>(defField, "window_prefab", null);
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000439E4 File Offset: 0x00041BE4
	public static Rect GetWorldRect(RectTransform rt)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		return new Rect(array[0].x, array[0].y, array[2].x - array[0].x, array[2].y - array[0].y);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00043A4E File Offset: 0x00041C4E
	public static float GetScreenAspectRatio()
	{
		if ((float)Screen.height == 0f)
		{
			return 1f;
		}
		return (float)Screen.width / (float)Screen.height;
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00043A70 File Offset: 0x00041C70
	public static void SetAligment(RectTransform rect, TextAnchor anchor)
	{
		if (rect == null)
		{
			return;
		}
		float num = rect.rect.width * 0.5f;
		float num2 = rect.rect.height * 0.5f;
		switch (anchor)
		{
		case TextAnchor.UpperLeft:
			rect.anchorMin = new Vector2(0f, 1f);
			rect.anchorMax = new Vector2(0f, 1f);
			rect.anchoredPosition = new Vector3(num, -num2);
			return;
		case TextAnchor.UpperCenter:
			rect.anchorMin = new Vector2(0.5f, 1f);
			rect.anchorMax = new Vector2(0.5f, 1f);
			rect.anchoredPosition = new Vector3(0f, -num2);
			return;
		case TextAnchor.UpperRight:
			rect.anchorMin = new Vector2(1f, 1f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.anchoredPosition = new Vector3(-num, -num2);
			return;
		case TextAnchor.MiddleLeft:
			rect.anchorMin = new Vector2(0f, 0.5f);
			rect.anchorMax = new Vector2(0f, 0.5f);
			rect.anchoredPosition = new Vector3(num, 0f);
			return;
		case TextAnchor.MiddleCenter:
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector3(0f, 0f);
			return;
		case TextAnchor.MiddleRight:
			rect.anchorMin = new Vector2(1f, 0.5f);
			rect.anchorMax = new Vector2(1f, 0.5f);
			rect.anchoredPosition = new Vector3(-num, 0f);
			return;
		case TextAnchor.LowerLeft:
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMax = new Vector2(0f, 0f);
			rect.anchoredPosition = new Vector3(num, num2);
			return;
		case TextAnchor.LowerCenter:
			rect.anchorMin = new Vector2(0.5f, 0f);
			rect.anchorMax = new Vector2(0.5f, 0f);
			rect.anchoredPosition = new Vector3(0f, num2);
			return;
		case TextAnchor.LowerRight:
			rect.anchorMin = new Vector2(1f, 0f);
			rect.anchorMax = new Vector2(1f, 0f);
			rect.anchoredPosition = new Vector3(-num, num2);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00043D20 File Offset: 0x00041F20
	public static void FitInParent(RectTransform rectTransform)
	{
		if (rectTransform == null)
		{
			return;
		}
		RectTransform rectTransform2 = rectTransform.parent as RectTransform;
		if (rectTransform2 == null)
		{
			return;
		}
		Vector2 size = rectTransform2.rect.size;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		UICommon.SetAligment(rectTransform, TextAnchor.MiddleCenter);
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00043D80 File Offset: 0x00041F80
	public static void FillParent(RectTransform rectTransform)
	{
		if (rectTransform == null)
		{
			return;
		}
		rectTransform.localPosition = new Vector3(0f, 0f, 0f);
		rectTransform.anchorMin = new Vector2(0f, 0f);
		rectTransform.anchorMax = new Vector2(1f, 1f);
		rectTransform.anchoredPosition = new Vector2(0f, 0f);
		rectTransform.sizeDelta = new Vector2(0f, 0f);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00043E08 File Offset: 0x00042008
	public static void AddAspectRatioFitter(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		RectTransform rectTransform = obj.transform as RectTransform;
		if (rectTransform == null)
		{
			return;
		}
		LayoutElement component = obj.GetComponent<LayoutElement>();
		float aspectRatio;
		if (component != null)
		{
			aspectRatio = component.preferredWidth / component.preferredHeight;
		}
		else
		{
			aspectRatio = rectTransform.rect.width / rectTransform.rect.height;
		}
		AspectRatioFitter orAddComponent = obj.GetOrAddComponent<AspectRatioFitter>();
		orAddComponent.aspectRatio = aspectRatio;
		orAddComponent.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00043E8C File Offset: 0x0004208C
	public static void EnsureInScreen(GameObject gameObject, float keepOnScreen = 0.85f)
	{
		RectTransform rectTransform = (gameObject != null) ? gameObject.GetComponent<RectTransform>() : null;
		if (rectTransform == null)
		{
			return;
		}
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		float x = array[0].x;
		float y = array[0].y;
		float num = array[2].x - array[1].x;
		float num2 = array[1].y - array[0].y;
		float num3 = num * (1f - keepOnScreen);
		float num4 = num2 * (1f - keepOnScreen);
		float num5 = (float)Screen.height;
		float num6 = (float)Screen.width;
		Vector3 position = rectTransform.position;
		float num7 = position.x - (x + num / 2f);
		float num8 = position.y - (y + num2 / 2f);
		if (x + num3 < 0f)
		{
			position.x = -num3 + num / 2f + num7;
		}
		else if (x + num - num3 > num6)
		{
			position.x = num6 + num3 - num / 2f + num7;
		}
		if (y + num4 < 0f)
		{
			position.y = -num4 + num2 / 2f + num8;
		}
		else if (y + num2 - num4 > num5)
		{
			position.y = num5 + num4 - num2 / 2f + num8;
		}
		rectTransform.position = position;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00043FFC File Offset: 0x000421FC
	public static Color GetRelationColor(float relation)
	{
		float t = (relation / RelationUtils.Def.maxRelationship + 1f) / 2f;
		return Color.Lerp(Color.red, Color.green, t);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00044030 File Offset: 0x00042230
	public static Vector3 WorldToScreen(Vector3 pos, Camera cam)
	{
		BaseUI baseUI = BaseUI.Get();
		if (((baseUI != null) ? baseUI.m_statusBar : null) == null)
		{
			return Vector3.zero;
		}
		int frameCount = UnityEngine.Time.frameCount;
		if ((long)frameCount != UICommon.last_frame || cam != UICommon.last_w2s_cam || UICommon.last_w2s_func != "WorldToScreen")
		{
			using (Game.Profile("WorldToScreen.MatrixCalculation", false, 0f, null))
			{
				UICommon.world2clip = cam.nonJitteredProjectionMatrix * cam.worldToCameraMatrix;
				UICommon.clip2world = UICommon.world2clip.inverse;
				UICommon.last_frame = (long)frameCount;
				UICommon.last_w2s_cam = cam;
				UICommon.last_w2s_func = "WorldToScreen";
			}
		}
		Vector2 screenPoint;
		using (Game.Profile("WorldToScreen.Calc", false, 0f, null))
		{
			Vector3 vector = UICommon.world2clip.MultiplyPoint(pos);
			Rect pixelRect = cam.pixelRect;
			screenPoint = new Vector2(pixelRect.x + (1f + vector.x) * pixelRect.width * 0.5f, pixelRect.y + (1f + vector.y) * pixelRect.height * 0.5f);
		}
		Vector2 vector2;
		using (Game.Profile("WorldToScreen.ScreenPointToLocalPointInRectangle", false, 0f, null))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(baseUI.m_statusBar, screenPoint, null, out vector2);
		}
		vector2 = new Vector2(Mathf.Round(vector2.x), Mathf.Round(vector2.y));
		return vector2;
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x000441EC File Offset: 0x000423EC
	public static Vector3 Screen2World(float sx, float sy, Camera cam)
	{
		int frameCount = UnityEngine.Time.frameCount;
		if ((long)frameCount != UICommon.last_frame || cam != UICommon.last_w2s_cam || UICommon.last_w2s_func != "Screen2World")
		{
			UICommon.world2clip = cam.nonJitteredProjectionMatrix * cam.worldToCameraMatrix;
			UICommon.clip2world = UICommon.world2clip.inverse;
			UICommon.last_frame = (long)frameCount;
			UICommon.last_w2s_cam = cam;
			UICommon.last_w2s_func = "Screen2World";
		}
		int pixelHeight = cam.pixelHeight;
		int pixelWidth = cam.pixelWidth;
		Vector3 point = new Vector3(2f * sx / (float)pixelWidth - 1f, 2f * sy / (float)pixelHeight - 1f, -cam.nearClipPlane);
		return UICommon.clip2world.MultiplyPoint(point);
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000442AC File Offset: 0x000424AC
	public static string IntToRomanNumber(int number)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int[] array = new int[]
		{
			1,
			4,
			5,
			9,
			10,
			40,
			50,
			90,
			100,
			400,
			500,
			900,
			1000
		};
		string[] array2 = new string[]
		{
			"I",
			"IV",
			"V",
			"IX",
			"X",
			"XL",
			"L",
			"XC",
			"C",
			"CD",
			"D",
			"CM",
			"M"
		};
		while (number > 0)
		{
			for (int i = array.Length - 1; i >= 0; i--)
			{
				if (number / array[i] >= 1)
				{
					number -= array[i];
					stringBuilder.Append(array2[i]);
					break;
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040005D2 RID: 1490
	private static List<GameObject> reusableList = new List<GameObject>(1000);

	// Token: 0x040005D3 RID: 1491
	private static List<FieldInfo> reusableFieldInfoList = new List<FieldInfo>(1000);

	// Token: 0x040005D4 RID: 1492
	private static long last_frame = -1L;

	// Token: 0x040005D5 RID: 1493
	public static Matrix4x4 world2clip;

	// Token: 0x040005D6 RID: 1494
	public static Matrix4x4 clip2world;

	// Token: 0x040005D7 RID: 1495
	private static Camera last_w2s_cam;

	// Token: 0x040005D8 RID: 1496
	private static string last_w2s_func;

	// Token: 0x02000579 RID: 1401
	[Flags]
	public enum ModifierKey
	{
		// Token: 0x04003067 RID: 12391
		None = 0,
		// Token: 0x04003068 RID: 12392
		Ctrl = 1,
		// Token: 0x04003069 RID: 12393
		Alt = 2,
		// Token: 0x0400306A RID: 12394
		Shift = 4,
		// Token: 0x0400306B RID: 12395
		LeftCtrl = 8,
		// Token: 0x0400306C RID: 12396
		LeftAlt = 16,
		// Token: 0x0400306D RID: 12397
		LeftShift = 32,
		// Token: 0x0400306E RID: 12398
		RightCtrl = 64,
		// Token: 0x0400306F RID: 12399
		RightAlt = 128,
		// Token: 0x04003070 RID: 12400
		RightShift = 256,
		// Token: 0x04003071 RID: 12401
		All = 511
	}

	// Token: 0x0200057A RID: 1402
	public enum WinKeyCode
	{
		// Token: 0x04003073 RID: 12403
		VK_LBUTTON = 1,
		// Token: 0x04003074 RID: 12404
		VK_SHIFT = 16,
		// Token: 0x04003075 RID: 12405
		VK_LSHIFT = 160,
		// Token: 0x04003076 RID: 12406
		VK_RSHIFT,
		// Token: 0x04003077 RID: 12407
		VK_CONTROL = 17,
		// Token: 0x04003078 RID: 12408
		VK_LCONTROL = 162,
		// Token: 0x04003079 RID: 12409
		VK_RCONTROL,
		// Token: 0x0400307A RID: 12410
		VK_MENU = 18,
		// Token: 0x0400307B RID: 12411
		VK_LMENU = 164,
		// Token: 0x0400307C RID: 12412
		VK_RMEMU
	}
}
