using System;
using UnityEngine;
using UnityEngine.Rendering;
using VacuumShaders.TerrainToMesh;

// Token: 0x0200033B RID: 827
[AddComponentMenu("VacuumShaders/Terrain To Mesh/Example/Runtime Converter")]
public class RunTime_Terrain_Convertion : MonoBehaviour
{
	// Token: 0x06003271 RID: 12913 RVA: 0x00199648 File Offset: 0x00197848
	private void Start()
	{
		if (this.sourceTerrain != null)
		{
			Mesh[] array = TerrainToMeshConverter.Convert(this.sourceTerrain, this.convertInfo, false, null);
			if (array != null)
			{
				Material sharedMaterial;
				if (this.generateBasemap)
				{
					sharedMaterial = this.GenerateMaterial_Basemap();
				}
				else
				{
					sharedMaterial = this.GenerateMaterial_Splatmap();
				}
				if (array.Length == 1)
				{
					MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
					if (meshFilter == null)
					{
						meshFilter = base.gameObject.AddComponent<MeshFilter>();
					}
					meshFilter.sharedMesh = array[0];
					MeshRenderer meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer == null)
					{
						meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
					}
					meshRenderer.sharedMaterial = sharedMaterial;
					if (this.attachMeshCollider)
					{
						base.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
						return;
					}
				}
				else
				{
					for (int i = 0; i < array.Length; i++)
					{
						GameObject gameObject = new GameObject(array[i].name);
						gameObject.transform.parent = base.gameObject.transform;
						gameObject.transform.localPosition = Vector3.zero;
						MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
						meshFilter2.sharedMesh = array[i];
						gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
						if (this.attachMeshCollider)
						{
							gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter2.sharedMesh;
						}
					}
				}
			}
		}
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x001997A0 File Offset: 0x001979A0
	private Material GenerateMaterial_Basemap()
	{
		Texture2D value = null;
		Texture2D texture2D = null;
		bool sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		TerrainToMeshConverter.ExtractBasemap(this.sourceTerrain, out value, out texture2D, 1024, 1024, sRGB);
		Material material;
		if (GraphicsSettings.renderPipelineAsset == null)
		{
			material = new Material(Shader.Find((texture2D != null) ? "Legacy Shaders/Bumped Diffuse" : "Legacy Shaders/Diffuse"));
			material.SetTexture("_MainTex", value);
			material.SetColor("_Color", Color.white);
			if (texture2D != null)
			{
				material.SetTexture("_BumpMap", texture2D);
			}
		}
		else
		{
			Shader shader = Shader.Find("Lightweight Render Pipeline/Lit");
			if (shader == null)
			{
				shader = Shader.Find("VacuumShaders/Terrain To Mesh/SRP Default");
			}
			material = new Material(shader);
			if (material.HasProperty("_MainTex"))
			{
				material.SetTexture("_MainTex", value);
			}
			if (material.HasProperty("_BaseMap"))
			{
				material.SetTexture("_BaseMap", value);
			}
			material.SetColor("_Color", Color.white);
			if (texture2D != null)
			{
				material.SetTexture("_BumpMap", texture2D);
			}
		}
		return material;
	}

	// Token: 0x06003273 RID: 12915 RVA: 0x001998C0 File Offset: 0x00197AC0
	private Material GenerateMaterial_Splatmap()
	{
		Material material = null;
		Texture2D[] array = TerrainToMeshConverter.ExtractSplatmaps(this.sourceTerrain);
		if (array == null || array.Length == 0)
		{
			return material;
		}
		Texture2D[] array2;
		Texture2D[] array3;
		Vector2[] array4;
		Vector2[] array5;
		float[] array6;
		float[] array7;
		int num = TerrainToMeshConverter.ExtractTexturesInfo(this.sourceTerrain, out array2, out array3, out array4, out array5, out array6, out array7);
		if (num == 0 || array2 == null)
		{
			Debug.LogWarning("usedTexturesCount == 0");
			return material;
		}
		if (num == 1)
		{
			if (GraphicsSettings.renderPipelineAsset == null)
			{
				Shader shader = Shader.Find("Legacy Shaders/Diffuse");
				if (shader != null)
				{
					material = new Material(shader);
					material.SetTexture("_MainTex", array2[0]);
					material.SetTextureScale("_MainTex", array4[0]);
					material.SetTextureOffset("_MainTex", array5[0]);
				}
			}
			else
			{
				Shader shader = Shader.Find("Lightweight Render Pipeline/Lit");
				if (shader != null)
				{
					material = new Material(shader);
					material.SetTexture("_BaseMap", array2[0]);
					material.SetTextureScale("_BaseMap", array4[0]);
					material.SetTextureOffset("_BaseMap", array5[0]);
				}
			}
			return material;
		}
		num = Mathf.Clamp(num, 2, 8);
		bool flag = false;
		if (array3 != null && num < 5)
		{
			flag = true;
		}
		string text;
		if (GraphicsSettings.renderPipelineAsset == null)
		{
			text = string.Format("VacuumShaders/Terrain To Mesh/Standard/" + (flag ? "Bumped" : "Diffuse") + "/{0} Textures", num);
		}
		else
		{
			text = string.Format("VacuumShaders/Terrain To Mesh/Lightweight Render Pipeline/Lit/{0} Textures", num);
		}
		Shader shader2 = Shader.Find(text);
		if (shader2 == null)
		{
			if (GraphicsSettings.renderPipelineAsset == null)
			{
				Debug.LogWarning("Shader not found: " + text);
			}
			else
			{
				Debug.LogWarning("Shader not found: '" + text + "'.\nLightweight Render Pipeline shaders (http://u3d.as/1jFw) are not installed.\n");
			}
			return material;
		}
		material = new Material(shader2);
		if (array.Length == 1)
		{
			material.SetTexture("_V_T2M_Control", array[0]);
		}
		else
		{
			if (array.Length > 2)
			{
				Debug.Log("TerrainToMesh shaders support max 2 control textures. Current terrain uses " + array.Length);
			}
			material.SetTexture("_V_T2M_Control", array[0]);
			material.SetTexture("_V_T2M_Control2", array[1]);
		}
		for (int i = 0; i < num; i++)
		{
			material.SetTexture(string.Format("_V_T2M_Splat{0}", i + 1), array2[i]);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_uvScale", i + 1), array4[i].x);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_Metallic", i + 1), array6[i]);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_Glossiness", i + 1), array7[i]);
			if (flag)
			{
				material.SetTexture(string.Format("_V_T2M_Splat{0}_bumpMap", i + 1), array3[i]);
			}
		}
		return material;
	}

	// Token: 0x040021EA RID: 8682
	public Terrain sourceTerrain;

	// Token: 0x040021EB RID: 8683
	public TerrainConvertInfo convertInfo;

	// Token: 0x040021EC RID: 8684
	public bool generateBasemap;

	// Token: 0x040021ED RID: 8685
	public bool attachMeshCollider;
}
