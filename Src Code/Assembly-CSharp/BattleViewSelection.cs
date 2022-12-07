using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001B9 RID: 441
public class BattleViewSelection
{
	// Token: 0x06001A1B RID: 6683 RVA: 0x000FCE6C File Offset: 0x000FB06C
	public BattleViewSelection(BaseUI baseUI)
	{
		this.ui = baseUI;
		this.FindSelectbales();
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000FCEA4 File Offset: 0x000FB0A4
	private void FindSelectbales()
	{
		MonoBehaviour[] array = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] is ISelectable)
			{
				this.Register(array[i] as ISelectable);
			}
		}
		if (BattleMap.battle != null)
		{
			for (int j = 0; j < BattleMap.battle.squads.Count; j++)
			{
				Logic.Squad squad = BattleMap.battle.squads[j];
				global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
				if (!(squad2 == null))
				{
					this.Register(squad2);
				}
			}
		}
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x000FCF30 File Offset: 0x000FB130
	public void Register(ISelectable selectable)
	{
		if (selectable != null)
		{
			this.selectabales.Add(selectable);
		}
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000FCF42 File Offset: 0x000FB142
	public void Unregister(ISelectable selectable)
	{
		this.selectabales.Remove(selectable);
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x000FCF54 File Offset: 0x000FB154
	public ISelectable[] GetSelected()
	{
		if (this.selectabales == null || this.selectabales.Count == 0)
		{
			return new ISelectable[0];
		}
		List<ISelectable> list = new List<ISelectable>();
		foreach (ISelectable selectable in this.selectabales)
		{
			if (selectable.Selected)
			{
				list.Add(selectable);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x000FCFD8 File Offset: 0x000FB1D8
	public ISelectable[] GetPreselected()
	{
		if (this.selectabales == null || this.selectabales.Count == 0)
		{
			return new ISelectable[0];
		}
		List<ISelectable> list = new List<ISelectable>();
		foreach (ISelectable selectable in this.selectabales)
		{
			if (selectable.PreSelected)
			{
				list.Add(selectable);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x000FD05C File Offset: 0x000FB25C
	public void Start()
	{
		this.Init();
		this.CreateBoxRect();
		this.ResetBoxRect();
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x000FD070 File Offset: 0x000FB270
	public void Update()
	{
		this.BeginSelection();
		this.DragSelection();
		this.EndSelection();
	}

	// Token: 0x06001A23 RID: 6691 RVA: 0x000FD084 File Offset: 0x000FB284
	private void Init()
	{
		if (this.selectionBoxContainer == null)
		{
			if (this.ui != null)
			{
				this.selectionBoxContainer = (this.ui.tCanvas.transform as RectTransform);
				return;
			}
			foreach (Canvas canvas in UnityEngine.Object.FindObjectsOfType<Canvas>())
			{
				if (canvas.isRootCanvas && canvas.isActiveAndEnabled)
				{
					this.selectionBoxContainer = (canvas.transform as RectTransform);
					return;
				}
			}
		}
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x000FD104 File Offset: 0x000FB304
	private void CreateBoxRect()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Selection Box";
		gameObject.transform.parent = this.selectionBoxContainer.transform;
		Image image = gameObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.color = this.color;
		this.art = global::Defs.GetRandomObj<Sprite>("BattleViewSelection", "BoxRect", null);
		image.sprite = this.art;
		image.type = Image.Type.Sliced;
		this.boxRect = (gameObject.transform as RectTransform);
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x000FD18C File Offset: 0x000FB38C
	private void ResetBoxRect()
	{
		this.boxRect.GetComponent<Image>();
		this.origin = Vector2.zero;
		this.boxRect.anchoredPosition = Vector2.zero;
		this.boxRect.sizeDelta = Vector2.zero;
		this.boxRect.anchorMax = Vector2.zero;
		this.boxRect.anchorMin = Vector2.zero;
		this.boxRect.localScale = Vector3.one;
		this.boxRect.pivot = Vector2.zero;
		this.boxRect.gameObject.SetActive(false);
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x000FD224 File Offset: 0x000FB424
	private void ResizeBox()
	{
		Vector2 point;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.selectionBoxContainer, this.origin, null, out point);
		Vector2 vector = Rect.PointToNormalized(this.selectionBoxContainer.rect, point);
		Vector2 screenPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector2 point2;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.boxRect.parent as RectTransform, screenPoint, null, out point2);
		Vector2 vector2 = Rect.PointToNormalized(this.selectionBoxContainer.rect, point2);
		Vector2 anchorMin = default(Vector2);
		anchorMin.x = ((vector.x < vector2.x) ? vector.x : vector2.x);
		anchorMin.y = ((vector.y < vector2.y) ? vector.y : vector2.y);
		Vector2 anchorMax = default(Vector2);
		anchorMax.x = ((vector.x > vector2.x) ? vector.x : vector2.x);
		anchorMax.y = ((vector.y > vector2.y) ? vector.y : vector2.y);
		this.boxRect.anchorMin = anchorMin;
		this.boxRect.anchorMax = anchorMax;
		this.boxRect.anchoredPosition = Vector2.zero;
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x000FD370 File Offset: 0x000FB570
	private void BeginSelection()
	{
		if (Input.GetMouseButton(1) || (KeyBindings.GetBind("alternate_drag_formation", false) && Input.GetMouseButton(0)) || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		if (BattleFieldOverview.InProgress())
		{
			return;
		}
		EventSystem current = EventSystem.current;
		if ((current != null) ? current.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI())
		{
			return;
		}
		this.boxRect.gameObject.SetActive(true);
		this.origin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		if (!this.PointIsValidAgainstSelectionMask(this.origin))
		{
			this.ResetBoxRect();
			return;
		}
		if (!UICommon.GetKey(KeyCode.LeftShift, false) && !UICommon.GetKey(KeyCode.RightShift, false))
		{
			foreach (ISelectable selectable in this.selectabales)
			{
				selectable.Selected = false;
				selectable.PreSelected = false;
			}
		}
		this.boxRect.anchoredPosition = this.origin;
		this.clickedBeforeDrag = this.GetSelectableAtMousePosition();
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x000FD48C File Offset: 0x000FB68C
	private bool PointIsValidAgainstSelectionMask(Vector2 screenPoint)
	{
		return !this.selectionMask || RectTransformUtility.RectangleContainsScreenPoint(this.selectionMask, screenPoint, CameraController.GameCamera.Camera);
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x000FD4B4 File Offset: 0x000FB6B4
	private ISelectable GetSelectableAtMousePosition()
	{
		if (!this.PointIsValidAgainstSelectionMask(Input.mousePosition))
		{
			return null;
		}
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI == null)
		{
			return null;
		}
		int num = BattleMap.BattleSide;
		if (num < 0 || num > 1)
		{
			num = 0;
		}
		if (battleViewUI.picked_squads[num] != null)
		{
			return battleViewUI.picked_squads[num];
		}
		if (battleViewUI.picked_squads[1 - num] != null)
		{
			return battleViewUI.picked_squads[1 - num];
		}
		return null;
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x000FD530 File Offset: 0x000FB730
	private void DragSelection()
	{
		if (!Input.GetMouseButton(0) || !this.boxRect.gameObject.activeSelf)
		{
			return;
		}
		this.ResizeBox();
		foreach (ISelectable selectable in this.selectabales)
		{
			Vector3 v = this.GetScreenPointOfSelectable(selectable.transform.gameObject);
			bool flag = RectTransformUtility.RectangleContainsScreenPoint(this.boxRect, v, null) && this.PointIsValidAgainstSelectionMask(v);
			if (!flag)
			{
				global::Squad squad = selectable as global::Squad;
				if (squad != null)
				{
					v = CameraController.GameCamera.Camera.WorldToScreenPoint(squad.StatusBarPos());
					flag = (RectTransformUtility.RectangleContainsScreenPoint(this.boxRect, v, null) && this.PointIsValidAgainstSelectionMask(v));
				}
			}
			selectable.PreSelected = flag;
		}
		if (this.clickedBeforeDrag != null)
		{
			this.clickedBeforeDrag.PreSelected = true;
		}
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x000FD64C File Offset: 0x000FB84C
	private void ApplySingleClickDeselection()
	{
		if (this.clickedBeforeDrag == null)
		{
			return;
		}
		if (this.clickedAfterDrag != null && this.clickedBeforeDrag.Selected && this.clickedBeforeDrag.transform == this.clickedAfterDrag.transform)
		{
			this.clickedBeforeDrag.Selected = false;
			this.clickedBeforeDrag.PreSelected = false;
		}
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000FD6AC File Offset: 0x000FB8AC
	private void ApplyPreSelections()
	{
		foreach (ISelectable selectable in this.selectabales)
		{
			if (selectable.PreSelected)
			{
				selectable.Selected = true;
				selectable.PreSelected = false;
			}
		}
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000FD710 File Offset: 0x000FB910
	private Vector2 GetScreenPointOfSelectable(GameObject selectable)
	{
		if (CameraController.GameCamera == null || CameraController.GameCamera.Camera == null)
		{
			return new Vector2(0f, 0f);
		}
		return CameraController.GameCamera.Camera.WorldToScreenPoint(selectable.transform.position);
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x000FD76C File Offset: 0x000FB96C
	private Camera GetScreenPointCamera(RectTransform rectTransform)
	{
		RectTransform rectTransform2 = rectTransform;
		Canvas canvas;
		do
		{
			canvas = rectTransform2.GetComponent<Canvas>();
			if (canvas && !canvas.isRootCanvas)
			{
				canvas = null;
			}
			rectTransform2 = (RectTransform)rectTransform2.parent;
		}
		while (canvas == null);
		switch (canvas.renderMode)
		{
		case RenderMode.ScreenSpaceOverlay:
			return null;
		case RenderMode.ScreenSpaceCamera:
			if (!canvas.worldCamera)
			{
				return CameraController.MainCamera;
			}
			return canvas.worldCamera;
		}
		return CameraController.MainCamera;
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x000FD7E8 File Offset: 0x000FB9E8
	private void EndSelection()
	{
		if (!Input.GetMouseButtonUp(0) || !this.boxRect.gameObject.activeSelf)
		{
			return;
		}
		this.clickedAfterDrag = this.GetSelectableAtMousePosition();
		this.ApplySingleClickDeselection();
		this.ApplyPreSelections();
		this.ResetBoxRect();
		this.onSelectionChange.Invoke(this.GetSelected());
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI != null && battleViewUI.dblclk)
		{
			for (int i = 0; i < Troops.squads.Length; i++)
			{
				global::Squad squad = Troops.squads[i];
				if (((squad != null) ? squad.logic : null) != null && !squad.logic.IsDefeated() && squad.logic.IsValid() && squad.Selected)
				{
					battleViewUI.LookAt(squad.logic, false);
					return;
				}
			}
		}
	}

	// Token: 0x040010C3 RID: 4291
	public Color color = Color.white;

	// Token: 0x040010C4 RID: 4292
	public Sprite art;

	// Token: 0x040010C5 RID: 4293
	public RectTransform selectionMask;

	// Token: 0x040010C6 RID: 4294
	public BattleViewSelection.SelectionEvent onSelectionChange = new BattleViewSelection.SelectionEvent();

	// Token: 0x040010C7 RID: 4295
	private Vector2 origin;

	// Token: 0x040010C8 RID: 4296
	private RectTransform boxRect;

	// Token: 0x040010C9 RID: 4297
	private HashSet<ISelectable> selectabales = new HashSet<ISelectable>();

	// Token: 0x040010CA RID: 4298
	private ISelectable clickedBeforeDrag;

	// Token: 0x040010CB RID: 4299
	private ISelectable clickedAfterDrag;

	// Token: 0x040010CC RID: 4300
	public RectTransform selectionBoxContainer;

	// Token: 0x040010CD RID: 4301
	private BaseUI ui;

	// Token: 0x02000718 RID: 1816
	public class SelectionEvent : UnityEvent<ISelectable[]>
	{
	}
}
