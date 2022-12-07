using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class WorldToScreenObject : Hotspot
{
	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x060025BD RID: 9661 RVA: 0x0014D906 File Offset: 0x0014BB06
	// (set) Token: 0x060025BE RID: 9662 RVA: 0x0014D90E File Offset: 0x0014BB0E
	public float base_sort_order { get; private set; }

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x060025BF RID: 9663 RVA: 0x0014D917 File Offset: 0x0014BB17
	// (set) Token: 0x060025C0 RID: 9664 RVA: 0x0014D91F File Offset: 0x0014BB1F
	public float sort_order { get; private set; }

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x060025C1 RID: 9665 RVA: 0x0014D928 File Offset: 0x0014BB28
	// (set) Token: 0x060025C2 RID: 9666 RVA: 0x0014D930 File Offset: 0x0014BB30
	public float sort_order_tie_breaker { get; private set; }

	// Token: 0x060025C3 RID: 9667 RVA: 0x0014D939 File Offset: 0x0014BB39
	public void SetParent(WorldToScreenObject parent)
	{
		this.parent = parent;
		if (parent != null)
		{
			this._was_visible = -1;
		}
		this.UpdateVisibility();
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x0014D958 File Offset: 0x0014BB58
	protected bool InitDef()
	{
		WorldToScreenObject.WorldToScreenScaleParams value;
		if (!WorldToScreenObject.def_params.TryGetValue(this.DefKey(true), out value))
		{
			value = new WorldToScreenObject.WorldToScreenScaleParams();
			WorldToScreenObject.def_params[this.DefKey(false)] = value;
			return true;
		}
		return false;
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x0014D998 File Offset: 0x0014BB98
	protected void LoadFigureScale(DT.Field cameraScale)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.defScaleCameraZoomedIn = 1f;
		worldToScreenScaleParams.defScaleOnCameraZoomedOut = 1f;
		if (cameraScale != null)
		{
			Value value = (cameraScale != null) ? cameraScale.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (cameraScale != null) ? cameraScale.Value(1, null, true, true) : Value.Unknown;
			if (value != Value.Unknown)
			{
				worldToScreenScaleParams.defScaleCameraZoomedIn = value;
			}
			if (value2 != Value.Unknown)
			{
				worldToScreenScaleParams.defScaleOnCameraZoomedOut = value2;
			}
		}
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x0014DA2C File Offset: 0x0014BC2C
	protected void LoadColorDef(DT.Field colorField)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.color = Color.black;
		if (colorField != null)
		{
			worldToScreenScaleParams.color = global::Defs.GetColor(colorField, worldToScreenScaleParams.color, null);
		}
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x0014DA6C File Offset: 0x0014BC6C
	protected void LoadSecondaryColorDef(DT.Field colorField)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.secondary_color = Color.black;
		if (colorField != null)
		{
			worldToScreenScaleParams.secondary_color = global::Defs.GetColor(colorField, worldToScreenScaleParams.secondary_color, null);
		}
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x0014DAAC File Offset: 0x0014BCAC
	protected void LoadPVDef(DT.Field cameraScale)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.defScalePVOnCameraZoomedOut = worldToScreenScaleParams.defScaleOnCameraZoomedOut;
		worldToScreenScaleParams.defScalePVCameraZoomedIn = worldToScreenScaleParams.defScaleCameraZoomedIn;
		if (cameraScale != null)
		{
			Value value = (cameraScale != null) ? cameraScale.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (cameraScale != null) ? cameraScale.Value(1, null, true, true) : Value.Unknown;
			if (value != Value.Unknown)
			{
				worldToScreenScaleParams.defScalePVCameraZoomedIn = value;
			}
			if (value2 != Value.Unknown)
			{
				worldToScreenScaleParams.defScalePVOnCameraZoomedOut = value2;
			}
		}
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x0014DB44 File Offset: 0x0014BD44
	protected void LoadOffset3d(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.offset_3d = Vector3.zero;
		if (field != null)
		{
			Value value = (field != null) ? field.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (field != null) ? field.Value(1, null, true, true) : Value.Unknown;
			Vector3 offset_3d = default(Vector3);
			if (value != Value.Unknown)
			{
				offset_3d.x = value;
			}
			if (value2 != Value.Unknown)
			{
				offset_3d.y = value2;
			}
			worldToScreenScaleParams.offset_3d = offset_3d;
		}
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x0014DBE0 File Offset: 0x0014BDE0
	protected void LoadOffset3dAlternative(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.offset_3d_alternative = worldToScreenScaleParams.offset_3d;
		if (field != null)
		{
			Value value = (field != null) ? field.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (field != null) ? field.Value(1, null, true, true) : Value.Unknown;
			Vector3 offset_3d_alternative = default(Vector3);
			if (value != Value.Unknown)
			{
				offset_3d_alternative.x = value;
			}
			if (value2 != Value.Unknown)
			{
				offset_3d_alternative.y = value2;
			}
			worldToScreenScaleParams.offset_3d_alternative = offset_3d_alternative;
		}
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x0014DC7C File Offset: 0x0014BE7C
	protected void LoadOffset2d(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.offset_2d = Vector3.zero;
		if (field != null)
		{
			Value value = (field != null) ? field.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (field != null) ? field.Value(1, null, true, true) : Value.Unknown;
			Vector3 offset_2d = default(Vector3);
			if (value != Value.Unknown)
			{
				offset_2d.x = value;
			}
			if (value2 != Value.Unknown)
			{
				offset_2d.y = value2;
			}
			worldToScreenScaleParams.offset_2d = offset_2d;
		}
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x0014DD18 File Offset: 0x0014BF18
	protected void LoadOffset2dAlternative(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.offset_2d_alternative = worldToScreenScaleParams.offset_2d;
		if (field != null)
		{
			Value value = (field != null) ? field.Value(0, null, true, true) : Value.Unknown;
			Value value2 = (field != null) ? field.Value(1, null, true, true) : Value.Unknown;
			Vector3 offset_2d_alternative = default(Vector3);
			if (value != Value.Unknown)
			{
				offset_2d_alternative.x = value;
			}
			if (value2 != Value.Unknown)
			{
				offset_2d_alternative.y = value2;
			}
			worldToScreenScaleParams.offset_2d_alternative = offset_2d_alternative;
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x0014DDB4 File Offset: 0x0014BFB4
	protected void LoadClampToScreen(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.clamp_to_screen = false;
		if (field != null)
		{
			worldToScreenScaleParams.clamp_to_screen = field.Bool(null, false);
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x0014DDEC File Offset: 0x0014BFEC
	protected void LoadClampToScreenStance(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.clamp_stance = RelationUtils.Stance.All;
		if (field != null)
		{
			worldToScreenScaleParams.clamp_stance = RelationUtils.Stance.None;
			for (int i = 0; i < field.NumValues(); i++)
			{
				string value = field.String(i, null, "");
				RelationUtils.Stance stance;
				if (!string.IsNullOrEmpty(value) && Enum.TryParse<RelationUtils.Stance>(value, out stance))
				{
					worldToScreenScaleParams.clamp_stance |= stance;
				}
			}
		}
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x0014DE60 File Offset: 0x0014C060
	protected void LoadClampPadding(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.padding_down = 21.6f;
		worldToScreenScaleParams.padding_left = 38.4f;
		worldToScreenScaleParams.padding_up = 21.6f;
		worldToScreenScaleParams.padding_right = 38.4f;
		if (field != null && field.NumValues() == 4)
		{
			worldToScreenScaleParams.padding_left = field.Float(0, null, 0f);
			worldToScreenScaleParams.padding_right = field.Float(1, null, 0f);
			worldToScreenScaleParams.padding_up = field.Float(2, null, 0f);
			worldToScreenScaleParams.padding_down = field.Float(3, null, 0f);
		}
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x0014DF04 File Offset: 0x0014C104
	protected virtual void LoadDefs(DT.Field def)
	{
		this.LoadFigureScale((def != null) ? def.FindChild("figure_scale_to_camera_zoom", null, true, true, true, '.') : null);
		this.LoadPVDef((def != null) ? def.FindChild("figure_scale_to_camera_pv_zoom", null, true, true, true, '.') : null);
		this.LoadOffset3d((def != null) ? def.FindChild("offset_3d", null, true, true, true, '.') : null);
		this.LoadOffset2d((def != null) ? def.FindChild("offset_2d", null, true, true, true, '.') : null);
		this.LoadOffset3dAlternative((def != null) ? def.FindChild("offset_3d_alternative", null, true, true, true, '.') : null);
		this.LoadOffset2dAlternative((def != null) ? def.FindChild("offset_2d_alternative", null, true, true, true, '.') : null);
		this.LoadColorDef((def != null) ? def.FindChild("health", null, true, true, true, '.') : null);
		this.LoadSecondaryColorDef((def != null) ? def.FindChild("health_secondary", null, true, true, true, '.') : null);
		this.LoadClampToScreen((def != null) ? def.FindChild("clamp_to_screen", null, true, true, true, '.') : null);
		this.LoadClampToScreenStance((def != null) ? def.FindChild("clamp_stance", null, true, true, true, '.') : null);
		this.LoadClampPadding((def != null) ? def.FindChild("clamp_padding", null, true, true, true, '.') : null);
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x0014E050 File Offset: 0x0014C250
	public override void Awake()
	{
		base.Awake();
		if (this.AddToGlobalListOnAwake())
		{
			this.AddToWTS();
		}
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x0014E066 File Offset: 0x0014C266
	protected void AddToWTS()
	{
		WorldToScreenObject.worldToScreenObjects.Add(this);
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x0014E073 File Offset: 0x0014C273
	protected void DelFromWTS()
	{
		WorldToScreenObject.worldToScreenObjects.Remove(this);
		WorldToScreenObject.active_objects.Remove(this);
		this._was_visible = -1;
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x0002C53B File Offset: 0x0002A73B
	protected virtual bool AddToGlobalListOnAwake()
	{
		return true;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x0014E094 File Offset: 0x0014C294
	protected virtual void OnDestroy()
	{
		if (this.AddToGlobalListOnAwake())
		{
			this.DelFromWTS();
		}
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void RefreshDefField()
	{
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x0014E0A4 File Offset: 0x0014C2A4
	protected virtual string DefKey(bool refresh = false)
	{
		return this.def_key;
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x060025D8 RID: 9688 RVA: 0x0014E0AC File Offset: 0x0014C2AC
	// (set) Token: 0x060025D9 RID: 9689 RVA: 0x0014E0B4 File Offset: 0x0014C2B4
	public bool Clamped
	{
		get
		{
			return this._clamped;
		}
		set
		{
			if (this._clamped == value)
			{
				return;
			}
			this._clamped = value;
			this.OnClampChange();
		}
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000023FD File Offset: 0x000005FD
	protected virtual void OnClampChange()
	{
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x0014E0CD File Offset: 0x0014C2CD
	public virtual RelationUtils.Stance Stance()
	{
		return RelationUtils.Stance.All;
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x0014E0D4 File Offset: 0x0014C2D4
	public static void UpdateTransforms()
	{
		using (Game.Profile("WorldToScreenObjects.UpdateAllTransforms", false, 0f, null))
		{
			GameCamera gameCamera = CameraController.GameCamera;
			Camera camera = (gameCamera != null) ? gameCamera.Camera : null;
			if (!(camera == null))
			{
				BaseUI baseUI = BaseUI.Get();
				if (!(((baseUI != null) ? baseUI.m_statusBar : null) == null))
				{
					baseUI.m_statusBar.pivot = Vector2.zero;
					Matrix4x4 matrix4x = camera.nonJitteredProjectionMatrix * camera.worldToCameraMatrix;
					Rect pixelRect = camera.pixelRect;
					float num = Vector3.Distance(BaseUI.Get().ptLookAt, gameCamera.transform.position);
					float x = gameCamera.Settings.dist.x;
					float y = gameCamera.Settings.dist.y;
					bool flag = ViewMode.IsPoliticalView();
					ScreenEdgeBlock.CacheEdges();
					foreach (KeyValuePair<string, WorldToScreenObject.WorldToScreenScaleParams> keyValuePair in WorldToScreenObject.def_params)
					{
						if (flag)
						{
							keyValuePair.Value.calculated_scale = Vector3.one * Mathf.Lerp(keyValuePair.Value.defScalePVCameraZoomedIn, keyValuePair.Value.defScalePVOnCameraZoomedOut, (num - x) / (y - x));
						}
						else
						{
							keyValuePair.Value.calculated_scale = Vector3.one * Mathf.Lerp(keyValuePair.Value.defScaleCameraZoomedIn, keyValuePair.Value.defScaleOnCameraZoomedOut, (num - x) / (y - x));
						}
					}
					Vector3 position = camera.transform.position;
					for (int i = 0; i < WorldToScreenObject.worldToScreenObjects.Count; i++)
					{
						WorldToScreenObject worldToScreenObject = WorldToScreenObject.worldToScreenObjects[i];
						if (!(worldToScreenObject.parent != null) && worldToScreenObject.IsVisible())
						{
							WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[worldToScreenObject.DefKey(false)];
							Vector3 forward = camera.transform.forward;
							Vector3 vector = worldToScreenObject.GetDesiredPosition(flag);
							Vector3 vector2 = vector - camera.transform.position;
							float num2 = Vector3.Dot(forward, vector2);
							if (num2 <= 0f)
							{
								Vector3 b = forward * num2 * 2f;
								vector = camera.transform.position + (vector2 - b);
							}
							Vector3 vector3 = matrix4x.MultiplyPoint(vector);
							Vector2 vector4 = new Vector2(pixelRect.x + (1f + vector3.x) * 0.5f, pixelRect.y + (1f + vector3.y) * 0.5f);
							Vector3 vector5 = flag ? worldToScreenScaleParams.offset_2d_alternative : worldToScreenScaleParams.offset_2d;
							float num3 = pixelRect.width * vector4.x + vector5.x;
							float num4 = pixelRect.height * vector4.y + vector5.y;
							if (worldToScreenScaleParams.clamp_to_screen)
							{
								using (Game.Profile("WorldToScreenObjects.Clamp", false, 0f, null))
								{
									RelationUtils.Stance stance = worldToScreenObject.Stance();
									if ((worldToScreenScaleParams.clamp_stance & stance) != RelationUtils.Stance.None)
									{
										float num5 = pixelRect.width / worldToScreenScaleParams.padding_left;
										float num6 = pixelRect.width / worldToScreenScaleParams.padding_right;
										float num7 = pixelRect.height / worldToScreenScaleParams.padding_down;
										float num8 = pixelRect.height / worldToScreenScaleParams.padding_up;
										bool clamped = false;
										if (ScreenEdgeBlock.screenEdgeBlocks.Count > 0)
										{
											using (Game.Profile("WorldToScreenObjects.Trace", false, 0f, null))
											{
												Point point = new Point(num3, num4);
												float num9 = 0f;
												float num10 = 0f;
												if (num3 < num5)
												{
													num9 = Math.Max(num9, Math.Abs(num3 - num5));
												}
												if (num3 > pixelRect.width - num6)
												{
													num9 = Math.Max(num9, Math.Abs(num3 - (pixelRect.width - num6)));
												}
												if (num4 < num7)
												{
													num10 = Math.Max(num10, Math.Abs(num4 - num7));
												}
												if (num4 > pixelRect.height - num8)
												{
													num10 = Math.Max(num10, Math.Abs(num4 - (pixelRect.height - num7)));
												}
												float f = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
												Point pt = new Point(pixelRect.width / 2f, pixelRect.height / 2f);
												Point pt2 = pt - point;
												float num11 = pt2.Length();
												if (num11 > 0.1f)
												{
													point += pt2 * f / num11;
													pt2 = pt - point;
													num11 = pt2.Length();
													pt2 /= num11;
													Point point2 = point;
													for (float num12 = 0f; num12 < num11; num12 += 1f)
													{
														bool flag2 = false;
														if (point2.x < num5)
														{
															flag2 = true;
														}
														else if (point2.x > pixelRect.width - num6)
														{
															flag2 = true;
														}
														else if (point2.y < num7)
														{
															flag2 = true;
														}
														else if (point2.y > pixelRect.height - num8)
														{
															flag2 = true;
														}
														if (!flag2)
														{
															for (int j = 0; j < ScreenEdgeBlock.screenEdgeBlocks.Count; j++)
															{
																Vector3[] v = ScreenEdgeBlock.screenEdgeBlocks[j].v;
																Vector3 vector6 = v[0];
																vector6.x -= num6;
																vector6.y -= num8;
																Vector3 vector7 = v[2];
																vector7.x += num5;
																vector7.y += num7;
																if ((point2.x > vector6.x && point2.x < vector7.x && point2.y > vector6.y && point2.y < vector7.y) || (point2.y > vector6.y && point2.y < vector7.y && point2.x > vector6.x && point2.x < vector7.x))
																{
																	flag2 = true;
																	break;
																}
															}
														}
														if (!flag2)
														{
															break;
														}
														point2 = point + pt2 * num12;
													}
													if (point2.x != num3 || point2.y != num4)
													{
														num3 = point2.x;
														num4 = point2.y;
														if (worldToScreenObject.Clamped && point2.Dist(new Point(worldToScreenObject.transform.position.x, worldToScreenObject.transform.position.y)) < 5f)
														{
															num3 = Mathf.Lerp(worldToScreenObject.transform.position.x, num3, 0.3f);
															num4 = Mathf.Lerp(worldToScreenObject.transform.position.y, num4, 0.3f);
														}
														clamped = true;
													}
												}
											}
										}
										worldToScreenObject.Clamped = clamped;
									}
								}
							}
							worldToScreenObject.transform.position = new Vector3(num3, num4, 0f);
							worldToScreenObject.transform.localScale = worldToScreenScaleParams.calculated_scale;
							worldToScreenObject.base_sort_order = worldToScreenObject.BaseSortOrder();
							worldToScreenObject.sort_order = Vector3.SqrMagnitude(position - vector);
							worldToScreenObject.sort_order_tie_breaker = (float)worldToScreenObject.SortOrderTieBreaker();
						}
					}
					using (Game.Profile("WorldToScreenObjects.SortByY", false, 0f, null))
					{
						WorldToScreenObject.active_objects.Sort(new Comparison<WorldToScreenObject>(WorldToScreenObject.OrderBy));
					}
					using (Game.Profile("WorldToScreenObjects.SetSiblingIndex", false, 0f, null))
					{
						for (int k = 0; k < WorldToScreenObject.active_objects.Count; k++)
						{
							WorldToScreenObject.active_objects[k].transform.SetSiblingIndex(k);
						}
					}
				}
			}
		}
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x0014E988 File Offset: 0x0014CB88
	public static Point WorldToScreen(Vector3 pos, Camera cam)
	{
		Matrix4x4 matrix4x = cam.nonJitteredProjectionMatrix * cam.worldToCameraMatrix;
		Vector3 forward = cam.transform.forward;
		Vector3 vector = pos - cam.transform.position;
		float num = Vector3.Dot(forward, vector);
		if (num <= 0f)
		{
			Vector3 b = forward * num * 2f;
			pos = cam.transform.position + (vector - b);
		}
		Vector3 vector2 = matrix4x.MultiplyPoint(pos);
		Rect pixelRect = cam.pixelRect;
		Vector2 vector3 = new Vector2(pixelRect.x + (1f + vector2.x) * 0.5f, pixelRect.y + (1f + vector2.y) * 0.5f);
		float x = pixelRect.width * vector3.x;
		float y = pixelRect.height * vector3.y;
		return new Point(x, y);
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x0014EA78 File Offset: 0x0014CC78
	private static int OrderBy(WorldToScreenObject x, WorldToScreenObject y)
	{
		float num = x.base_sort_order;
		float num2 = y.base_sort_order;
		if (num > num2)
		{
			return -1;
		}
		if (num < num2)
		{
			return 1;
		}
		num = x.sort_order;
		num2 = y.sort_order;
		if (num > num2)
		{
			return -1;
		}
		if (num < num2)
		{
			return 1;
		}
		num = x.sort_order_tie_breaker;
		num2 = y.sort_order_tie_breaker;
		if (num > num2)
		{
			return -1;
		}
		if (num < num2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x0014EAD4 File Offset: 0x0014CCD4
	public virtual GameObject GetVisuals()
	{
		return this._g;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x0014EADC File Offset: 0x0014CCDC
	public virtual Vector3 GetDesiredPosition(bool is_pv)
	{
		GameObject visuals = this.GetVisuals();
		if (visuals == null)
		{
			return Vector3.zero;
		}
		Transform transform = visuals.transform;
		if (is_pv)
		{
			return transform.position + WorldToScreenObject.def_params[this.DefKey(false)].offset_3d_alternative;
		}
		return transform.position + WorldToScreenObject.def_params[this.DefKey(false)].offset_3d;
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual int SortOrderTieBreaker()
	{
		return 0;
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x0007EB68 File Offset: 0x0007CD68
	public virtual float BaseSortOrder()
	{
		return 0f;
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x0014EB4C File Offset: 0x0014CD4C
	public virtual bool IsVisible()
	{
		return this.visibility_from_object && this.visibility_from_view && this.visibility_from_filter;
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x0014EB68 File Offset: 0x0014CD68
	public void UpdateVisibility()
	{
		if (base.gameObject == null)
		{
			return;
		}
		bool flag = this.IsVisible();
		bool flag2 = this._was_visible != -1;
		bool flag3 = this._was_visible == 1;
		if (flag != flag3 || !flag2)
		{
			this._was_visible = (flag ? 1 : 0);
			base.gameObject.SetActive(flag);
			if (this.parent == null)
			{
				if (flag)
				{
					WorldToScreenObject.active_objects.Add(this);
					return;
				}
				WorldToScreenObject.active_objects.Remove(this);
			}
		}
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x0014EBEB File Offset: 0x0014CDEB
	public virtual void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFiguresFromViewMode)
	{
		this.visibility_from_view = ((allowedFiguresFromViewMode & this.allowedType) > ViewMode.AllowedFigures.None);
		this.UpdateVisibility();
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x0014EC04 File Offset: 0x0014CE04
	public virtual void UpdateVisibilityFromObject(bool visible_from_object)
	{
		this.visibility_from_object = visible_from_object;
		this.UpdateVisibility();
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x0014EC13 File Offset: 0x0014CE13
	public virtual void UpdateVisibilityFilter()
	{
		this.visibility_from_filter = ((ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		this.UpdateVisibility();
	}

	// Token: 0x040019A2 RID: 6562
	protected bool visibility_from_filter = true;

	// Token: 0x040019A3 RID: 6563
	protected bool visibility_from_object;

	// Token: 0x040019A4 RID: 6564
	protected bool visibility_from_view;

	// Token: 0x040019A5 RID: 6565
	protected ViewMode.AllowedFigures allowedType;

	// Token: 0x040019A6 RID: 6566
	protected WorldToScreenObject parent;

	// Token: 0x040019AA RID: 6570
	protected GameObject _g;

	// Token: 0x040019AB RID: 6571
	public static List<WorldToScreenObject> worldToScreenObjects = new List<WorldToScreenObject>();

	// Token: 0x040019AC RID: 6572
	public static List<WorldToScreenObject> active_objects = new List<WorldToScreenObject>();

	// Token: 0x040019AD RID: 6573
	public static Dictionary<string, WorldToScreenObject.WorldToScreenScaleParams> def_params = new Dictionary<string, WorldToScreenObject.WorldToScreenScaleParams>();

	// Token: 0x040019AE RID: 6574
	protected string def_key;

	// Token: 0x040019AF RID: 6575
	private bool _clamped;

	// Token: 0x040019B0 RID: 6576
	private int _was_visible = -1;

	// Token: 0x020007BF RID: 1983
	public class WorldToScreenScaleParams
	{
		// Token: 0x04003C35 RID: 15413
		public float defScaleCameraZoomedIn;

		// Token: 0x04003C36 RID: 15414
		public float defScaleOnCameraZoomedOut;

		// Token: 0x04003C37 RID: 15415
		public float defScalePVCameraZoomedIn;

		// Token: 0x04003C38 RID: 15416
		public float defScalePVOnCameraZoomedOut;

		// Token: 0x04003C39 RID: 15417
		public Vector3 offset_3d;

		// Token: 0x04003C3A RID: 15418
		public Vector3 offset_3d_alternative;

		// Token: 0x04003C3B RID: 15419
		public Vector3 offset_2d;

		// Token: 0x04003C3C RID: 15420
		public Vector3 offset_2d_alternative;

		// Token: 0x04003C3D RID: 15421
		public bool clamp_to_screen;

		// Token: 0x04003C3E RID: 15422
		public RelationUtils.Stance clamp_stance = RelationUtils.Stance.All;

		// Token: 0x04003C3F RID: 15423
		public float padding_left;

		// Token: 0x04003C40 RID: 15424
		public float padding_right;

		// Token: 0x04003C41 RID: 15425
		public float padding_up;

		// Token: 0x04003C42 RID: 15426
		public float padding_down;

		// Token: 0x04003C43 RID: 15427
		public float raycast_target_min_x;

		// Token: 0x04003C44 RID: 15428
		public float raycast_target_min_y;

		// Token: 0x04003C45 RID: 15429
		public float raycast_target_max_x;

		// Token: 0x04003C46 RID: 15430
		public float raycast_target_max_y;

		// Token: 0x04003C47 RID: 15431
		public Vector3 calculated_scale;

		// Token: 0x04003C48 RID: 15432
		public Color color;

		// Token: 0x04003C49 RID: 15433
		public Color secondary_color;
	}
}
