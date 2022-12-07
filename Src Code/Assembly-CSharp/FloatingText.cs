using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class FloatingText : MonoBehaviour
{
	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06001029 RID: 4137 RVA: 0x000AC783 File Offset: 0x000AA983
	// (set) Token: 0x0600102A RID: 4138 RVA: 0x000AC78B File Offset: 0x000AA98B
	public float alpha { get; set; }

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x0600102B RID: 4139 RVA: 0x000AC794 File Offset: 0x000AA994
	// (set) Token: 0x0600102C RID: 4140 RVA: 0x000AC79C File Offset: 0x000AA99C
	public float fadeSpeed { get; set; }

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x0600102D RID: 4141 RVA: 0x000AC7A5 File Offset: 0x000AA9A5
	// (set) Token: 0x0600102E RID: 4142 RVA: 0x000AC7AD File Offset: 0x000AA9AD
	public float fadeDelay { get; set; }

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x0600102F RID: 4143 RVA: 0x000AC7B6 File Offset: 0x000AA9B6
	// (set) Token: 0x06001030 RID: 4144 RVA: 0x000AC7BE File Offset: 0x000AA9BE
	public float heightChangeSpeed { get; set; }

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06001031 RID: 4145 RVA: 0x000AC7C7 File Offset: 0x000AA9C7
	// (set) Token: 0x06001032 RID: 4146 RVA: 0x000AC7CF File Offset: 0x000AA9CF
	public float startingHeight { get; set; }

	// Token: 0x06001033 RID: 4147 RVA: 0x000AC7D8 File Offset: 0x000AA9D8
	private void Update()
	{
		this.previous_local_to_world = base.transform.localToWorldMatrix;
		if (this.alpha > 0f)
		{
			float unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
			this.fadeDelayCurrent += unscaledDeltaTime;
			if (this.fadeDelayCurrent >= this.fadeDelay)
			{
				this.alpha -= unscaledDeltaTime * this.fadeSpeed;
			}
			this.color.a = this.alpha;
			this.textMesh.color = this.color;
			base.transform.Translate(new Vector3(0f, unscaledDeltaTime * this.heightChangeSpeed, 0f));
			CameraController cameraController = CameraController.Get();
			Camera camera;
			if (cameraController == null)
			{
				camera = null;
			}
			else
			{
				GameCamera currentGameCamera = cameraController.CurrentGameCamera;
				camera = ((currentGameCamera != null) ? currentGameCamera.Camera : null);
			}
			Camera camera2 = camera;
			if (camera2 != null && camera2.depthTextureMode.HasFlag(DepthTextureMode.MotionVectors))
			{
				if (this.motion_vectors_material == null)
				{
					this.motion_vectors_material = new Material(this.textMesh.fontSharedMaterial);
					this.motion_vectors_material.shader = Shader.Find("Hidden/TextMeshPro/Distance Field MotionVectors");
				}
				CustomMotionVectors.DrawRenderer(this.textMesh.renderer, this.previous_local_to_world, this.motion_vectors_material, 0, 0);
			}
			return;
		}
		if (this.useSharedInstance)
		{
			base.gameObject.SetActive(false);
			return;
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000AC935 File Offset: 0x000AAB35
	private void OnDestroy()
	{
		if (this.motion_vectors_material != null)
		{
			global::Common.DestroyObj(this.motion_vectors_material);
		}
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x000AC950 File Offset: 0x000AAB50
	private static FloatingText CreateInstance(DT.Field def, string instanceName = "FloatingText")
	{
		GameObject prefabLocalzied = FloatingText.GetPrefabLocalzied(def);
		if (prefabLocalzied == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prefabLocalzied, GameLogic.instance.transform, false, "TemploaryObjs");
		gameObject.name = instanceName;
		return gameObject.GetComponent<FloatingText>();
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000AC994 File Offset: 0x000AAB94
	private static GameObject GetPrefabLocalzied(DT.Field field)
	{
		GameObject gameObject = null;
		if (!string.IsNullOrEmpty(global::Defs.set_language))
		{
			gameObject = global::Defs.GetObj<GameObject>(field, "textPrefab." + global::Defs.set_language, null);
		}
		else if (!string.IsNullOrEmpty(global::Defs.fake_translation_to))
		{
			gameObject = global::Defs.GetObj<GameObject>(field, "textPrefab." + global::Defs.fake_translation_to, null);
		}
		if (gameObject == null)
		{
			gameObject = global::Defs.GetObj<GameObject>(field, "textPrefab", null);
		}
		return gameObject;
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000ACA02 File Offset: 0x000AAC02
	public static FloatingText Create(GameObject obj, string prefabName, string text, Vars vars = null, bool use_shared_instance = false)
	{
		if (obj.gameObject.activeSelf)
		{
			return FloatingText.Create(obj.transform.position, prefabName, text, vars, false);
		}
		return null;
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000ACA28 File Offset: 0x000AAC28
	public static FloatingText Create(Vector3 position, string prefabName, string text, Vars vars = null, bool useSharedInstance = true)
	{
		DT.Field def = global::Defs.GetDefField(prefabName, null);
		FloatingText floatingText;
		if (useSharedInstance)
		{
			GameObject gameObject = global::Common.FindChildByName(global::Common.FindChildByName(GameLogic.instance.gameObject, "TemploaryObjs", false, false), "FloatingTextShared", false, false);
			if (gameObject == null)
			{
				floatingText = FloatingText.CreateInstance(def, "FloatingTextShared");
				floatingText.useSharedInstance = useSharedInstance;
			}
			else
			{
				floatingText = gameObject.GetComponent<FloatingText>();
			}
		}
		else
		{
			floatingText = FloatingText.CreateInstance(def, "FloatingText");
		}
		floatingText.Refresh(position, def, text, vars);
		return floatingText;
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x000ACAA4 File Offset: 0x000AACA4
	public void Refresh(Vector3 position, DT.Field def, string text, Vars vars)
	{
		if (def == null)
		{
			Debug.Log("FloatingText dt field missing.");
			return;
		}
		if (this.defField != def)
		{
			this.defField = def;
			this.startingHeight = this.defField.GetFloat("startingHeight", null, 0f, true, true, true, '.');
			this.heightChangeSpeed = this.defField.GetFloat("heightChangeSpeed", null, 0f, true, true, true, '.');
			this.fadeDelay = this.defField.GetFloat("fadeDelay", null, 0f, true, true, true, '.');
			this.fadeSpeed = this.defField.GetFloat("fadeSpeed", null, 0f, true, true, true, '.');
			this.color = global::Defs.GetColor(this.defField, "text_color", Color.white, null);
			this.fontSize = this.defField.GetInt("text_font_size", null, 0, true, true, true, '.');
		}
		if (this.textMesh == null)
		{
			this.textMesh = base.GetComponent<TextMeshPro>();
			this.textMesh.alignment = TextAlignmentOptions.Top;
		}
		UIText.SetText(this.textMesh, this.defField, text, vars, null);
		this.alpha = 1f;
		this.fadeDelayCurrent = 0f;
		base.transform.position = position;
		base.transform.Translate(new Vector3(0f, this.startingHeight, 0f));
		base.gameObject.SetActive(true);
	}

	// Token: 0x04000AA2 RID: 2722
	private float fadeDelayCurrent;

	// Token: 0x04000AA5 RID: 2725
	public Color color;

	// Token: 0x04000AA6 RID: 2726
	public int fontSize;

	// Token: 0x04000AA7 RID: 2727
	private bool useSharedInstance;

	// Token: 0x04000AA8 RID: 2728
	private TextMeshPro textMesh;

	// Token: 0x04000AA9 RID: 2729
	private DT.Field defField;

	// Token: 0x04000AAA RID: 2730
	private Matrix4x4 previous_local_to_world;

	// Token: 0x04000AAB RID: 2731
	private Material motion_vectors_material;
}
