using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000239 RID: 569
public class MiniMap : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IScrollHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
{
	// Token: 0x060022CD RID: 8909 RVA: 0x0013B9E0 File Offset: 0x00139BE0
	private void LoadDefs()
	{
		this.def_field = global::Defs.GetDefField("Minimap", null);
		this.zoom_step = this.def_field.GetValue("zoom_step", null, true, true, true, '.').Float(this.zoom_step);
		this.visible_area_max_coef = this.def_field.GetValue("visible_area_max_coef", null, true, true, true, '.').Float(this.visible_area_max_coef);
		this.scroll_edge_offset = this.def_field.GetValue("scroll_edge_offset", null, true, true, true, '.').Float(this.scroll_edge_offset);
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x0013BA7C File Offset: 0x00139C7C
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.cam = global::Common.GetComponent<Camera>(base.transform, "Camera");
		this.m_MapImage = global::Common.FindChildComponent<RawImage>(base.gameObject, "id_MapImage");
		this.rect = base.GetComponent<RectTransform>();
		this.icons_container = global::Common.FindChildComponent<RectTransform>(base.gameObject, "id_IconsContainer");
		this.img = base.GetComponentInChildren<RawImage>();
		this.initialized = true;
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x0013BAF4 File Offset: 0x00139CF4
	private void Start()
	{
		this.LoadDefs();
		this.Init();
		this.state = MiniMap.State.View;
		this.ui = BaseUI.Get();
		this.terrainSize = Terrain.activeTerrain.terrainData.size;
		if (BattleMap.Get() == null || !SettlementBV.generating)
		{
			if (this.img != null && this.img.texture == null)
			{
				this.Render();
			}
		}
		else
		{
			SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.Render));
			SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.Render));
		}
		this.UpdateMiniMapImage();
		RectTransform rectTransform = this.screen_border;
		MinimapBVScreenBorder minimapBVScreenBorder = (rectTransform != null) ? rectTransform.GetComponent<MinimapBVScreenBorder>() : null;
		if (minimapBVScreenBorder != null)
		{
			this.on_zoom_changed = (MiniMap.OnZoomChanged)Delegate.Combine(this.on_zoom_changed, new MiniMap.OnZoomChanged(minimapBVScreenBorder.Minimap_OnZoomChanged));
		}
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x0013BBFC File Offset: 0x00139DFC
	private void Render()
	{
		if (this.cam == null)
		{
			this.cam = this.CreateCamera();
		}
		this.cam.enabled = true;
		if (this.fake_ocean != null)
		{
			this.fake_ocean.SetActive(true);
		}
		if (this.fake_trees != null)
		{
			for (int i = 0; i < this.fake_trees.Count; i++)
			{
				this.fake_trees[i].SetActive(true);
			}
		}
		this.cam.targetTexture = this.mm_rr;
		float shadowDistance = QualitySettings.shadowDistance;
		QualitySettings.shadowDistance = 10000f;
		BSGTerrain component = Terrain.activeTerrain.GetComponent<BSGTerrain>();
		bool flag = false;
		if (component != null)
		{
			flag = component.enabled;
			component.enabled = false;
		}
		this.cam.Render();
		if (flag)
		{
			component.enabled = true;
		}
		if (this.render_mat != null)
		{
			RenderTexture renderTexture = new RenderTexture(this.cam.targetTexture);
			Graphics.Blit(this.cam.targetTexture, renderTexture, this.render_mat);
			this.cam.targetTexture = renderTexture;
		}
		this.img.texture = this.cam.targetTexture;
		this.RenderTrees();
		QualitySettings.shadowDistance = shadowDistance;
		if (this.fake_ocean != null)
		{
			this.fake_ocean.SetActive(false);
		}
		if (this.fake_trees != null)
		{
			for (int j = 0; j < this.fake_trees.Count; j++)
			{
				global::Common.DestroyObj(this.fake_trees[j]);
			}
			this.fake_trees = null;
		}
		this.cam.enabled = false;
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x0013BD98 File Offset: 0x00139F98
	private void UpdateMiniMapImage()
	{
		if (this.m_MapImage == null)
		{
			return;
		}
		Scene activeScene = SceneManager.GetActiveScene();
		if (!activeScene.IsValid())
		{
			return;
		}
		string name = activeScene.name;
		Texture2D obj = global::Defs.GetObj<Texture2D>("MinimapSettings", name, null);
		if (obj != null)
		{
			this.m_MapImage.texture = obj;
		}
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x0013BDEE File Offset: 0x00139FEE
	private void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, Texture2D.whiteTexture);
		GUI.color = Color.white;
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x0013BE0C File Offset: 0x0013A00C
	private void DrawScreenLine(Vector2 pt1, Vector2 pt2, Color color, float thickness = 1f)
	{
		Vector2 vector = pt2 - pt1;
		float magnitude = vector.magnitude;
		float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(angle, pt1);
		this.DrawScreenRect(new Rect(pt1.x, pt1.y, magnitude, thickness), color);
		GUI.matrix = matrix;
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x0013BE70 File Offset: 0x0013A070
	public void AddObj(MapObject target)
	{
		if (target == null)
		{
			return;
		}
		this.Init();
		MinimapIcon minimapIcon;
		if (this.icons.TryGetValue(target, out minimapIcon))
		{
			return;
		}
		MinimapIcon iconByType = MinimapIcon.GetIconByType(target, this);
		if (iconByType == null)
		{
			return;
		}
		this.icons.Add(target, iconByType);
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x0013BEB4 File Offset: 0x0013A0B4
	public void DelObj(Logic.Object obj)
	{
		if (obj == null)
		{
			return;
		}
		MinimapIcon minimapIcon;
		if (!this.icons.TryGetValue(obj, out minimapIcon))
		{
			return;
		}
		minimapIcon.Destroy();
		this.icons.Remove(obj);
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x0013BEEC File Offset: 0x0013A0EC
	private void UpdateIcon(MinimapIcon icon)
	{
		if (icon == null)
		{
			return;
		}
		icon.image.rectTransform.localScale = Vector3.one * Mathf.Lerp(icon.GetMinScale(), icon.GetMaxScale(), this.GetNormalizedZoom());
		icon.image.rectTransform.position = this.WorldPosToScreenPos(icon.GetPosition());
		icon.image.rectTransform.rotation = icon.GetRotation();
		if (icon.IsSelected())
		{
			icon.image.rectTransform.SetAsLastSibling();
		}
		icon.UpdateSprite(false);
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x0013BF84 File Offset: 0x0013A184
	private Vector2 WorldPosToScreenPos(Vector3 pos)
	{
		Vector3 vector = this.ui.GetTerrainSize();
		float x = vector.x;
		float z = vector.z;
		Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
		Vector3 vector2 = pos - vector / 2f;
		Vector2 b = new Vector2(vector2.x / x * rect.width, vector2.z / z * rect.height);
		Vector2 vector3 = rect.center + b;
		vector3.Set((float)((int)vector3.x), (float)((int)vector3.y));
		return rect.center + b;
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x0013C030 File Offset: 0x0013A230
	private static Rect ToScreenRect(RectTransform rectTransform)
	{
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		float num = Mathf.Min(new float[]
		{
			array[0].x,
			array[1].x,
			array[2].x,
			array[3].x
		});
		float num2 = Mathf.Min(new float[]
		{
			array[0].y,
			array[1].y,
			array[2].y,
			array[3].y
		});
		float num3 = Mathf.Max(new float[]
		{
			array[0].x,
			array[1].x,
			array[2].x,
			array[3].x
		});
		float num4 = Mathf.Max(new float[]
		{
			array[0].y,
			array[1].y,
			array[2].y,
			array[3].y
		});
		float width = num3 - num;
		float height = num4 - num2;
		return new Rect(num, num2, width, height);
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x0013C180 File Offset: 0x0013A380
	private void LookAtFocusRectLocation()
	{
		this.LookAtPointFromMinimapImage(MiniMap.ToScreenRect(this.screen_border).center);
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x0013C1A8 File Offset: 0x0013A3A8
	private void LookAtPointFromMinimapImage(Vector2 pos)
	{
		Vector3 pickedPoint = this.GetPickedPoint(pos);
		if (pickedPoint != Vector3.zero)
		{
			this.ui.LookAt(pickedPoint, false);
		}
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x0013C1D7 File Offset: 0x0013A3D7
	public void Save(DT.Field f)
	{
		f.SetValue("minimap_zoom", this.GetZoom());
		string key = "minimap_view_mode";
		ViewMode secondary = ViewMode.secondary;
		f.SetValue(key, DT.Enquote((secondary != null) ? secondary.name : null), null);
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x0013C214 File Offset: 0x0013A414
	public void Load(DT.Field f)
	{
		if (!this.started)
		{
			this.load_from = f;
			return;
		}
		float @float = f.GetFloat("minimap_zoom", null, 1f, true, true, true, '.');
		this.SetZoom(@float);
		this.MoveFocusRectToLookAtPoint();
		this.ScrollMapImageToFocusRect(this.scroll_edge_offset);
		string @string = f.GetString("minimap_view_mode", null, "", true, true, true, '.');
		for (int i = 0; i < ViewMode.all.Count; i++)
		{
			ViewMode viewMode = ViewMode.all[i];
			if (((viewMode != null) ? viewMode.name : null) == @string)
			{
				ViewMode viewMode2 = ViewMode.all[i];
				if (viewMode2 != null)
				{
					viewMode2.ApplySecondary();
				}
				WorldUI.Get().ViewModeChanged();
				return;
			}
		}
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x0013C2D0 File Offset: 0x0013A4D0
	private void OnDestroy()
	{
		RectTransform rectTransform = this.screen_border;
		MinimapBVScreenBorder minimapBVScreenBorder = (rectTransform != null) ? rectTransform.GetComponent<MinimapBVScreenBorder>() : null;
		if (minimapBVScreenBorder != null)
		{
			this.on_zoom_changed = (MiniMap.OnZoomChanged)Delegate.Remove(this.on_zoom_changed, new MiniMap.OnZoomChanged(minimapBVScreenBorder.Minimap_OnZoomChanged));
		}
		if (this.mm_rr == null)
		{
			return;
		}
		RenderTexture.ReleaseTemporary(this.mm_rr);
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x0013C338 File Offset: 0x0013A538
	private void LateUpdate()
	{
		if (this.img == null || this.img.texture == null)
		{
			return;
		}
		if (!this.started)
		{
			this.started = true;
			if (this.load_from != null)
			{
				this.started = true;
				this.Load(this.load_from);
				this.load_from = null;
			}
			else
			{
				this.SetZoom(1f);
			}
		}
		Vector3 lookAtPoint = CameraController.GameCamera.GetLookAtPoint();
		if (lookAtPoint.x == 0f && lookAtPoint.z == 0f && this.last_look_at_point == Vector3.zero)
		{
			return;
		}
		if (Mathf.Abs((this.last_look_at_point - lookAtPoint).sqrMagnitude) > Mathf.Epsilon)
		{
			this.force_visible_screen_border = true;
		}
		this.last_look_at_point = lookAtPoint;
		MiniMap.State state = this.state;
		if (state != MiniMap.State.DragRect)
		{
			if (state != MiniMap.State.DragMap)
			{
				this.MoveFocusRectToLookAtPoint();
				if (this.force_visible_screen_border)
				{
					this.ScrollMapImageToFocusRect(0f);
					this.EnsureFocusRectInsideMinimapArea();
				}
			}
			else
			{
				this.PanMapImageWithMouseMovement();
				this.MoveFocusRectToLookAtPoint();
			}
		}
		else
		{
			this.MoveFocusRectToMousePosition();
			if (this.force_visible_screen_border)
			{
				this.EnsureFocusRectInsideMinimapArea();
				this.ScrollMapImageToFocusRect(this.scroll_edge_offset);
			}
			this.LookAtFocusRectLocation();
		}
		if (this.icons_container != null && !this.icons_container.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (KeyValuePair<Logic.Object, MinimapIcon> keyValuePair in this.icons)
		{
			this.UpdateIcon(keyValuePair.Value);
			keyValuePair.Value.Update();
		}
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x0013C4EC File Offset: 0x0013A6EC
	private void PanMapImageWithMouseMovement()
	{
		Vector3 mousePosition = Input.mousePosition;
		this.img.rectTransform.position = new Vector3(mousePosition.x + this.mouse_to_map.x, mousePosition.y + this.mouse_to_map.y, 0f);
		this.EnsureValidImgPosition();
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x0013C544 File Offset: 0x0013A744
	private Vector3 GetPickedPoint(Vector2 spt)
	{
		Vector3 vector = this.ui.GetTerrainSize();
		int height = Screen.height;
		Vector3[] array = new Vector3[4];
		this.img.rectTransform.GetWorldCorners(array);
		float x = array[0].x;
		float num = (float)height - array[2].y;
		float num2 = array[2].x - array[0].x;
		float num3 = array[2].y - array[0].y;
		float x2 = global::Common.map(spt.x, x, x + num2, 0f, vector.x, false);
		float lookAtHeight = CameraController.GameCamera.Settings.lookAtHeight;
		float z = global::Common.map((float)height - spt.y, num + num3, num, 0f, vector.z, false);
		return new Vector3(x2, lookAtHeight, z);
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x0013C628 File Offset: 0x0013A828
	public void OnPointerDown(PointerEventData e)
	{
		this.ptMouseDown = this.GetPickedPoint(e.position);
		if (this.ptMouseDown == Vector3.zero)
		{
			return;
		}
		switch (e.button)
		{
		case PointerEventData.InputButton.Left:
			this.ui.LookAt(this.ptMouseDown, false);
			return;
		case PointerEventData.InputButton.Right:
		{
			BaseUI baseUI = BaseUI.Get();
			global::Army army;
			if (baseUI == null)
			{
				army = null;
			}
			else
			{
				GameObject selected_obj = baseUI.selected_obj;
				army = ((selected_obj != null) ? selected_obj.GetComponent<global::Army>() : null);
			}
			global::Army army2 = army;
			if (army2 != null)
			{
				army2.MoveTo(this.AvoidSettlementsUsingPositions(this.ptMouseDown), 0, true, true);
				return;
			}
			BattleViewUI battleViewUI = BattleViewUI.Get();
			List<Logic.Squad> list = (battleViewUI != null) ? battleViewUI.GetSelectedSquads() : null;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Logic.Squad squad = list[i];
					if (!squad.is_fleeing)
					{
						(squad.visuals as global::Squad).MoveTo(this.ptMouseDown, 0, true);
					}
				}
			}
			return;
		}
		case PointerEventData.InputButton.Middle:
		{
			this.force_visible_screen_border = false;
			Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
			Vector2 position = e.position;
			this.mouse_to_map = rect.center - position;
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x0013C754 File Offset: 0x0013A954
	private Vector3 AvoidSettlementsUsingPositions(Vector3 point)
	{
		Game game = GameLogic.Get(false);
		Logic.Realm realm = (game != null) ? game.GetRealm(this.ptMouseDown) : null;
		if (realm == null)
		{
			return point;
		}
		List<Logic.Settlement> settlements = realm.settlements;
		for (int i = 0; i < settlements.Count; i++)
		{
			global::Settlement settlement = settlements[i].visuals as global::Settlement;
			if (!(settlement == null))
			{
				Vector3 center = settlement.GetCenter();
				center.y = point.y;
				float radius = settlement.GetRadius();
				if ((center - point).magnitude < radius)
				{
					if (point.x <= center.x)
					{
						point.x -= radius;
					}
					else
					{
						point.x += radius;
					}
					if (point.z <= center.z)
					{
						point.z -= radius;
					}
					else
					{
						point.z += radius;
					}
				}
			}
		}
		return point;
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x0013C84C File Offset: 0x0013AA4C
	public void OnPointerUp(PointerEventData e)
	{
		MiniMap.State state = this.state;
		if (state - MiniMap.State.DragRect <= 1)
		{
			this.state = MiniMap.State.View;
		}
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x0013C870 File Offset: 0x0013AA70
	public void OnBeginDrag(PointerEventData e)
	{
		if (this.state == MiniMap.State.View)
		{
			if (e.button == PointerEventData.InputButton.Left)
			{
				this.state = MiniMap.State.DragRect;
				return;
			}
			if (e.button == PointerEventData.InputButton.Middle)
			{
				this.state = MiniMap.State.DragMap;
			}
		}
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnDrag(PointerEventData e)
	{
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x0013C8A8 File Offset: 0x0013AAA8
	private Vector2 CalculateVisibleAreaRectSizeOnMinimap()
	{
		GameCamera gameCamera = CameraController.GameCamera;
		if (gameCamera == null)
		{
			return Vector2.zero;
		}
		Vector3 a = gameCamera.Camera.ScreenToWorldPoint(new Vector3(0f, (float)Screen.width, gameCamera.Settings.dist.magnitude));
		Vector3 lookAtPoint = gameCamera.GetLookAtPoint();
		float num = (a - lookAtPoint).magnitude * 2f;
		float screenAspectRatio = UICommon.GetScreenAspectRatio();
		float num2 = Mathf.Sqrt(num * num / (1f + screenAspectRatio * screenAspectRatio));
		float x = this.ui.GetTerrainSize().x;
		float num3 = Mathf.Round(num2 / x * (this.img.rectTransform.sizeDelta.y * this.img.rectTransform.localScale.y));
		return new Vector2(Mathf.Round(num3 * screenAspectRatio), num3);
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x0013C980 File Offset: 0x0013AB80
	private void SetZoom(float zoom)
	{
		this.img.transform.localScale = zoom * Vector3.one;
		this.screen_border.sizeDelta = this.CalculateVisibleAreaRectSizeOnMinimap();
		if (this.on_zoom_changed != null)
		{
			this.on_zoom_changed();
		}
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x0013C9CC File Offset: 0x0013ABCC
	private void EnsureValidImgPosition()
	{
		Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
		Rect rect2 = MiniMap.ToScreenRect(this.rect);
		Vector3 zero = Vector3.zero;
		if (rect2.width < rect.width)
		{
			if (rect2.xMin < rect.xMin)
			{
				zero.x = rect2.xMin - rect.xMin;
			}
			else if (rect2.xMax > rect.xMax)
			{
				zero.x = rect2.xMax - rect.xMax;
			}
		}
		else
		{
			zero.x = rect2.center.x - rect.center.x;
		}
		if (rect2.height < rect.height)
		{
			if (rect2.yMin < rect.yMin)
			{
				zero.y = rect2.yMin - rect.yMin;
			}
			else if (rect2.yMax > rect.yMax)
			{
				zero.y = rect2.yMax - rect.yMax;
			}
		}
		else
		{
			zero.y = rect2.center.y - rect.center.y;
		}
		this.img.rectTransform.Translate(zero);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x0013CB10 File Offset: 0x0013AD10
	private void MoveImageBy(Vector2 img_offset)
	{
		this.img.transform.position = base.gameObject.transform.position;
		this.img.rectTransform.Translate(img_offset.x, img_offset.y, 0f);
		this.EnsureValidImgPosition();
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x0013CB64 File Offset: 0x0013AD64
	private void CenterLookAtPointOnMinimap()
	{
		Vector3 lookAtPoint = CameraController.GameCamera.GetLookAtPoint();
		Vector3 vector = this.ui.GetTerrainSize();
		float x = vector.x;
		float z = vector.z;
		Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
		Vector3 vector2 = lookAtPoint - vector / 2f;
		Vector2 a = new Vector2(vector2.x / x * rect.width, vector2.z / z * rect.height);
		this.MoveImageBy(-a);
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x0013CBF0 File Offset: 0x0013ADF0
	private void MoveFocusRectToMousePosition()
	{
		Vector3 mousePosition = Input.mousePosition;
		MiniMap.ToScreenRect(this.screen_border);
		this.screen_border.position = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x0013CC3C File Offset: 0x0013AE3C
	private void EnsureFocusRectInsideMinimapArea()
	{
		Rect rect = MiniMap.ToScreenRect(this.screen_border);
		Rect rect2 = MiniMap.ToScreenRect(this.rect);
		if (rect.xMin < rect2.xMin)
		{
			rect.x = rect2.xMin;
		}
		else if (rect.xMax > rect2.xMax)
		{
			rect.x = rect2.xMax - rect.width;
		}
		if (rect.yMin < rect2.yMin)
		{
			rect.y = rect2.yMin;
		}
		else if (rect.yMax > rect2.yMax)
		{
			rect.y = rect2.yMax - rect.height;
		}
		this.screen_border.position = new Vector3(Mathf.Round(rect.xMin + rect.width / 2f), Mathf.Round(rect.yMin + rect.height / 2f), 0f);
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x0013CD38 File Offset: 0x0013AF38
	private void MoveFocusRectToLookAtPoint()
	{
		Vector3 lookAtPoint = CameraController.GameCamera.GetLookAtPoint();
		Vector3 vector = this.ui.GetTerrainSize();
		float x = vector.x;
		float z = vector.z;
		Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
		Vector3 vector2 = lookAtPoint - vector / 2f;
		Vector2 vector3 = new Vector2(vector2.x / x * rect.width, vector2.z / z * rect.height);
		Vector3 position = new Vector3(Mathf.Round(rect.center.x + vector3.x), Mathf.Round(rect.center.y + vector3.y), 0f);
		this.screen_border.position = position;
		this.screen_border.rotation = Quaternion.Euler(0f, 0f, -CameraController.GameCamera.transform.rotation.eulerAngles.y);
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x0013CE38 File Offset: 0x0013B038
	private void ScrollMapImageToFocusRect(float max_edge_offset = 0f)
	{
		Rect rect = MiniMap.ToScreenRect(this.screen_border);
		Rect rect2 = new Rect(rect.x - max_edge_offset, rect.y - max_edge_offset, rect.width + 2f * max_edge_offset, rect.height + 2f * max_edge_offset);
		Rect rect3 = MiniMap.ToScreenRect(this.rect);
		Rect rect4 = MiniMap.ToScreenRect(this.img.rectTransform);
		Rect rect5 = new Rect(rect4.x - rect.width / 2f, rect4.y - rect.height / 2f, rect4.width + rect.width, rect4.height + rect.height);
		Vector3 zero = Vector3.zero;
		if (rect2.xMin < rect3.xMin && rect5.xMin < rect3.xMin)
		{
			zero.x = Mathf.Min(rect3.xMin - rect2.xMin, rect3.xMin - rect5.xMin);
		}
		else if (rect2.xMax > rect3.xMax && rect5.xMax > rect3.xMax)
		{
			zero.x = Mathf.Max(rect3.xMax - rect2.xMax, rect3.xMax - rect5.xMax);
		}
		if (rect2.yMin < rect3.yMin && rect5.yMin < rect3.yMin)
		{
			zero.y = Mathf.Min(rect3.yMin - rect2.yMin, rect3.yMin - rect5.yMin);
		}
		else if (rect2.yMax > rect3.yMax && rect5.yMax > rect3.yMax)
		{
			zero.y = Mathf.Max(rect3.yMax - rect2.yMax, rect3.yMax - rect5.yMax);
		}
		this.img.rectTransform.Translate(zero);
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x0013D03C File Offset: 0x0013B23C
	public float GetZoom()
	{
		RawImage rawImage = this.img;
		float? num;
		if (rawImage == null)
		{
			num = null;
		}
		else
		{
			Transform transform = rawImage.transform;
			num = ((transform != null) ? new float?(transform.localScale.x) : null);
		}
		float? num2 = num;
		if (num2 == null)
		{
			return 1f;
		}
		return num2.GetValueOrDefault();
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x0013D098 File Offset: 0x0013B298
	public float GetNormalizedZoom()
	{
		if (this.img == null)
		{
			return 1f;
		}
		float x = this.img.transform.localScale.x;
		return Mathf.InverseLerp(1f, this.GetMaxZoom(), x);
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x0013D0E0 File Offset: 0x0013B2E0
	private float GetMaxZoom()
	{
		return 10f;
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x0013D0E8 File Offset: 0x0013B2E8
	void IScrollHandler.OnScroll(PointerEventData eventData)
	{
		if (this.state == MiniMap.State.View)
		{
			RawImage rawImage = this.img;
			if (!(((rawImage != null) ? rawImage.rectTransform : null) == null))
			{
				float x = this.img.transform.localScale.x;
				float num = x + eventData.scrollDelta.y * this.zoom_step;
				if (num < 1f)
				{
					num = 1f;
				}
				if (Mathf.Abs(num - x) <= Mathf.Epsilon)
				{
					return;
				}
				float num2 = num / x;
				if (this.screen_border.rect.width * num2 > this.rect.rect.width * this.visible_area_max_coef || this.screen_border.rect.height * num2 > this.rect.rect.height * this.visible_area_max_coef)
				{
					return;
				}
				this.force_visible_screen_border = false;
				Vector3 mousePosition = Input.mousePosition;
				Rect rect = MiniMap.ToScreenRect(this.img.rectTransform);
				Vector2 vector = new Vector2((mousePosition.x - rect.xMin) / rect.width, (mousePosition.y - rect.yMin) / rect.height);
				this.SetZoom(num);
				rect = MiniMap.ToScreenRect(this.img.rectTransform);
				this.img.rectTransform.position = new Vector3(mousePosition.x + rect.width / 2f - vector.x * rect.width, mousePosition.y + rect.height / 2f - vector.y * rect.height, 0f);
				this.EnsureValidImgPosition();
				this.MoveFocusRectToLookAtPoint();
				return;
			}
		}
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x0013D2A8 File Offset: 0x0013B4A8
	private void RenderTrees()
	{
		if (this.fake_trees != null)
		{
			return;
		}
		Vector3 vector = this.ui.GetTerrainSize();
		int num = LayerMask.NameToLayer("UI");
		int cullingMask = 1 << num;
		SquadTreesBuff.Def @base = GameLogic.Get(true).defs.GetBase<SquadTreesBuff.Def>();
		if (@base != null)
		{
			this.fake_trees = new List<GameObject>();
			this.tree_materials = new List<Material>();
			DT.Field field = @base.field.FindChild("minimap_sprites", null, true, true, true, '.');
			for (int i = 0; i < field.NumValues(); i++)
			{
				Texture2D texture2D = field.Value(i, null, true, true).Get<Texture2D>();
				if (!(texture2D == null))
				{
					Material material = new Material(Shader.Find("Unlit/Transparent Cutout"));
					material.SetFloat("_Cutoff", 0.1f);
					material.SetTexture("_MainTex", texture2D);
					this.tree_materials.Add(material);
				}
			}
			int min_trees_count = @base.min_trees_count;
			Logic.Battle battle = BattleMap.battle;
			byte[,] tree_count_grid = battle.tree_count_grid;
			int tree_count_grid_width = battle.tree_count_grid_width;
			int tree_count_grid_height = battle.tree_count_grid_height;
			int tree_grid_size = battle.tree_grid_size;
			float x = vector.x;
			float z = vector.z;
			float num2 = x / 40f;
			float num3 = z / 40f;
			Mesh sharedMesh = MeshUtils.CreateGridMesh(new Vector3(-num2 / 2f, 0.5f, -num3 / 2f), 1, 1, Vector3.right * num2, Vector3.forward * num3, false, false);
			for (int j = 0; j < tree_count_grid_width; j++)
			{
				for (int k = tree_count_grid_height - 1; k >= 0; k--)
				{
					Vector3 vector2 = new Vector3(((float)j - 0.5f) * (float)tree_grid_size, vector.y, ((float)k - 0.5f) * (float)tree_grid_size);
					int treeCount = battle.GetTreeCount(vector2);
					if (treeCount != 0 && treeCount >= min_trees_count)
					{
						GameObject gameObject = global::Common.SpawnTemplate("QuadRenderer", "FakeTree", null, true, new Type[]
						{
							typeof(MeshFilter),
							typeof(MeshRenderer)
						});
						gameObject.transform.position = vector2;
						gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
						gameObject.GetComponent<MeshRenderer>().sharedMaterial = this.tree_materials[Random.Range(0, this.tree_materials.Count)];
						gameObject.SetLayer(num, true);
						gameObject.hideFlags = HideFlags.HideAndDontSave;
						this.fake_trees.Add(gameObject);
					}
				}
			}
		}
		this.cam.cullingMask = cullingMask;
		this.cam.clearFlags = CameraClearFlags.Depth;
		this.cam.Render();
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x0013D578 File Offset: 0x0013B778
	private Camera CreateCamera()
	{
		GameObject gameObject = new GameObject("MinmapCamera");
		Camera camera = gameObject.AddComponent<Camera>();
		camera.orthographic = true;
		camera.renderingPath = RenderingPath.Forward;
		int num = LayerMask.NameToLayer("Terrain");
		int num2 = LayerMask.NameToLayer("Roads");
		camera.cullingMask = (1 << num | 1 << LayerMask.NameToLayer("Settlements") | 1 << num2);
		gameObject.transform.SetParent(base.transform, true);
		camera.useOcclusionCulling = false;
		Vector3 vector = this.ui.GetTerrainSize();
		gameObject.transform.forward = Vector3.down;
		gameObject.transform.position = new Vector3(vector.x / 2f, vector.y + camera.nearClipPlane * 2f + 10f, vector.z / 2f);
		camera.orthographicSize = Mathf.Max(vector.x, vector.z) / 2f;
		if (!camera.targetTexture)
		{
			this.mm_rr = RenderTexture.GetTemporary(new RenderTextureDescriptor(1024, 1024, RenderTextureFormat.ARGB32, 0)
			{
				autoGenerateMips = false,
				sRGB = true,
				vrUsage = VRTextureUsage.None
			});
		}
		if (this.ocean_material && this.fake_ocean == null)
		{
			float x = vector.x;
			float z = vector.z;
			Mesh sharedMesh = MeshUtils.CreateGridMesh(new Vector3(-x / 2f, 0.5f, -z / 2f), 1, 1, Vector3.right * x, Vector3.forward * z, false, false);
			GameObject gameObject2 = global::Common.SpawnTemplate("QuadRenderer", "FakeOcean", null, true, new Type[]
			{
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			gameObject2.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
			gameObject2.GetComponent<MeshRenderer>().sharedMaterial = this.ocean_material;
			gameObject2.transform.position = new Vector3(x / 2f, MapData.GetWaterLevel(), z / 2f);
			gameObject2.gameObject.SetLayer(num, true);
			this.fake_ocean = gameObject2;
		}
		return camera;
	}

	// Token: 0x04001753 RID: 5971
	public float ground_height;

	// Token: 0x04001754 RID: 5972
	public float zoom_step = 0.1f;

	// Token: 0x04001755 RID: 5973
	public float scroll_edge_offset;

	// Token: 0x04001756 RID: 5974
	public float visible_area_max_coef = 0.5f;

	// Token: 0x04001757 RID: 5975
	public Material ocean_material;

	// Token: 0x04001758 RID: 5976
	public List<Material> tree_materials;

	// Token: 0x04001759 RID: 5977
	private bool started;

	// Token: 0x0400175A RID: 5978
	private DT.Field load_from;

	// Token: 0x0400175B RID: 5979
	[HideInInspector]
	public DT.Field def_field;

	// Token: 0x0400175C RID: 5980
	private Dictionary<Logic.Object, MinimapIcon> icons = new Dictionary<Logic.Object, MinimapIcon>();

	// Token: 0x0400175D RID: 5981
	private BaseUI ui;

	// Token: 0x0400175E RID: 5982
	private Camera cam;

	// Token: 0x0400175F RID: 5983
	private Point terrainSize;

	// Token: 0x04001760 RID: 5984
	private RawImage img;

	// Token: 0x04001761 RID: 5985
	private RectTransform rect;

	// Token: 0x04001762 RID: 5986
	private GameObject fake_ocean;

	// Token: 0x04001763 RID: 5987
	private List<GameObject> fake_trees;

	// Token: 0x04001764 RID: 5988
	public RectTransform icons_container;

	// Token: 0x04001765 RID: 5989
	private RenderTexture mm_rr;

	// Token: 0x04001766 RID: 5990
	public RectTransform screen_border;

	// Token: 0x04001767 RID: 5991
	public Material render_mat;

	// Token: 0x04001768 RID: 5992
	private RawImage m_MapImage;

	// Token: 0x04001769 RID: 5993
	private Vector3 ptMouseDown;

	// Token: 0x0400176A RID: 5994
	private Vector2 mouse_to_map = Vector2.zero;

	// Token: 0x0400176B RID: 5995
	private Vector3 last_look_at_point = Vector3.zero;

	// Token: 0x0400176C RID: 5996
	private bool force_visible_screen_border = true;

	// Token: 0x0400176D RID: 5997
	public MiniMap.OnZoomChanged on_zoom_changed;

	// Token: 0x0400176E RID: 5998
	private MiniMap.State state;

	// Token: 0x0400176F RID: 5999
	private bool initialized;

	// Token: 0x02000797 RID: 1943
	// (Invoke) Token: 0x06004CB4 RID: 19636
	public delegate void OnZoomChanged();

	// Token: 0x02000798 RID: 1944
	private enum State
	{
		// Token: 0x04003B3C RID: 15164
		View,
		// Token: 0x04003B3D RID: 15165
		DragRect,
		// Token: 0x04003B3E RID: 15166
		DragMap
	}
}
