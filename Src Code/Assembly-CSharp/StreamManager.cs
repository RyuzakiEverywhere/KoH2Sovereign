using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class StreamManager : MonoBehaviour
{
	// Token: 0x06000236 RID: 566 RVA: 0x0002090C File Offset: 0x0001EB0C
	private static Vector2 WorldToTerrain(Terrain ter, Vector3 point, Texture splatControl)
	{
		point = ter.transform.worldToLocalMatrix.MultiplyPoint(point);
		float x = point.x / ter.terrainData.size.x * (float)splatControl.width;
		float y = point.z / ter.terrainData.size.z * (float)splatControl.height;
		return new Vector2(x, y);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00020974 File Offset: 0x0001EB74
	public void Register(StreamEmitter e)
	{
		this.emitters.Add(e);
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00020982 File Offset: 0x0001EB82
	public void Unregister(StreamEmitter e)
	{
		this.emitters.Remove(e);
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00020991 File Offset: 0x0001EB91
	public void Register(StreamCollider e)
	{
		this.colliders.Add(e);
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0002099F File Offset: 0x0001EB9F
	public void Unregister(StreamCollider e)
	{
		this.colliders.Remove(e);
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000209AE File Offset: 0x0001EBAE
	private void Awake()
	{
		this.msTerrain = base.GetComponent<MicroSplatTerrain>();
	}

	// Token: 0x0600023C RID: 572 RVA: 0x000209BC File Offset: 0x0001EBBC
	private void OnEnable()
	{
		this.terrainDesc = this.msTerrain.terrainDesc;
		int width = this.terrainDesc.width;
		int height = this.terrainDesc.height;
		this.buffer0 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		this.buffer1 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		Graphics.Blit(Texture2D.blackTexture, this.buffer0);
		Graphics.Blit(Texture2D.blackTexture, this.buffer1);
		this.updateMat = new Material(this.updateShader);
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00020A44 File Offset: 0x0001EC44
	private void OnDisable()
	{
		this.buffer0.Release();
		this.buffer1.Release();
		Object.DestroyImmediate(this.buffer0);
		Object.DestroyImmediate(this.buffer1);
		this.onBuffer0 = false;
		Object.DestroyImmediate(this.updateMat);
		this.buffer0 = null;
		this.buffer1 = null;
		this.updateMat = null;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00020AA4 File Offset: 0x0001ECA4
	private void Update()
	{
		int count = this.emitters.Count;
		int count2 = this.colliders.Count;
		int num = 0;
		for (int i = 0; i < this.emitters.Count; i++)
		{
			StreamEmitter streamEmitter = this.emitters[i];
			Vector2 vector = StreamManager.WorldToTerrain(this.msTerrain.terrain, streamEmitter.transform.position, this.buffer0);
			if (vector.x >= 0f && vector.x < (float)this.buffer0.width && vector.y >= 0f && vector.y < (float)this.buffer0.width)
			{
				Vector3 point = streamEmitter.transform.position + Vector3.left * streamEmitter.transform.lossyScale.x;
				Vector2 b = StreamManager.WorldToTerrain(this.msTerrain.terrain, point, this.buffer0);
				float num2 = Vector2.Distance(vector, b);
				if (num2 < 1f)
				{
					num2 = 1f;
				}
				num2 *= streamEmitter.strength;
				Vector4 vector2 = new Vector4(vector.x, vector.y, 0f, 0f);
				if (streamEmitter.emitterType == StreamEmitter.EmitterType.Water)
				{
					vector2.z = num2;
				}
				else
				{
					vector2.w = num2;
				}
				this.spawnBuffer[num] = vector2;
				num++;
			}
		}
		int num3 = 0;
		for (int j = 0; j < this.colliders.Count; j++)
		{
			StreamCollider streamCollider = this.colliders[j];
			Vector2 vector3 = StreamManager.WorldToTerrain(this.msTerrain.terrain, streamCollider.transform.position, this.buffer0);
			if (vector3.x >= 0f && vector3.x < (float)this.buffer0.width && vector3.y >= 0f && vector3.y < (float)this.buffer0.width)
			{
				Vector3 point2 = streamCollider.transform.position + Vector3.left * streamCollider.transform.lossyScale.x;
				Vector2 b2 = StreamManager.WorldToTerrain(this.msTerrain.terrain, point2, this.buffer0);
				float num4 = Vector2.Distance(vector3, b2);
				Vector4 vector4 = new Vector4(vector3.x, vector3.y, 0f, 0f);
				if (streamCollider.colliderType != StreamCollider.ColliderType.Lava)
				{
					vector4.z = num4;
				}
				if (streamCollider.colliderType != StreamCollider.ColliderType.Water)
				{
					vector4.w = num4;
				}
				this.colliderBuffer[num3] = vector4;
				num3++;
			}
		}
		this.updateMat.SetVectorArray("_Positions", this.spawnBuffer);
		this.updateMat.SetVectorArray("_Colliders", this.colliderBuffer);
		this.updateMat.SetInt("_PositionsCount", num);
		this.updateMat.SetInt("_CollidersCount", num3);
		this.updateMat.SetVector("_SpawnStrength", this.strength);
		this.updateMat.SetTexture("_TerrainDesc", this.terrainDesc);
		this.updateMat.SetFloat("_DeltaTime", Time.smoothDeltaTime);
		this.updateMat.SetVector("_Speed", this.speed);
		this.updateMat.SetVector("_Resistance", this.resistance);
		if (this.onBuffer0)
		{
			if (this.evaporation.x > 0f)
			{
				float num5 = 1f / this.evaporation.x / 255f;
				if (this.timeSinceEvapX > (double)num5)
				{
					this.timeSinceEvapX = 0.0;
					this.evapAmount.x = 0.004f;
				}
				else
				{
					this.evapAmount.x = 0f;
				}
			}
			if (this.evaporation.y > 0f)
			{
				float num6 = 1f / this.evaporation.y / 255f;
				if (this.timeSinceEvapY > (double)num6)
				{
					this.timeSinceEvapY = 0.0;
					this.evapAmount.y = 0.004f;
				}
				else
				{
					this.evapAmount.y = 0f;
				}
			}
			this.updateMat.SetVector("_Evaporation", this.evapAmount);
			if (this.wetnessEvaporation > 0f)
			{
				float num7 = 1f / this.wetnessEvaporation / 255f;
				if (this.timeSinceWetnessEvap > (double)num7)
				{
					this.updateMat.SetFloat("_WetnessEvaporation", 0.004f);
					this.timeSinceWetnessEvap = 0.0;
				}
				else
				{
					this.updateMat.SetFloat("_WetnessEvaporation", 0f);
				}
			}
			if (this.burnEvaporation > 0f)
			{
				float num8 = 1f * this.burnEvaporation / 255f;
				if (this.timeSinceBurnEvap > (double)num8)
				{
					this.updateMat.SetFloat("_BurnEvaporation", 0.004f);
					this.timeSinceBurnEvap = 0.0;
				}
				else
				{
					this.updateMat.SetFloat("_BurnEvaporation", 0f);
				}
			}
			Graphics.Blit(this.buffer0, this.buffer1, this.updateMat);
			this.currentBuffer = this.buffer1;
		}
		else
		{
			this.updateMat.SetInt("_PositionsCount", 0);
			this.updateMat.SetVector("_Evaporation", Vector2.zero);
			this.updateMat.SetFloat("_WetnessEvaporation", 0f);
			this.updateMat.SetFloat("_BurnEvaporation", 0f);
			Graphics.Blit(this.buffer1, this.buffer0, this.updateMat);
			this.currentBuffer = this.buffer0;
		}
		this.onBuffer0 = !this.onBuffer0;
		float deltaTime = Time.deltaTime;
		this.timeSinceEvapX += (double)deltaTime;
		this.timeSinceEvapY += (double)deltaTime;
		this.timeSinceWetnessEvap += (double)deltaTime;
		this.timeSinceBurnEvap += (double)deltaTime;
		this.msTerrain.matInstance.SetTexture("_DynamicStreamControl", this.currentBuffer);
	}

	// Token: 0x04000345 RID: 837
	private MicroSplatTerrain msTerrain;

	// Token: 0x04000346 RID: 838
	private RenderTexture buffer0;

	// Token: 0x04000347 RID: 839
	private RenderTexture buffer1;

	// Token: 0x04000348 RID: 840
	[HideInInspector]
	public RenderTexture currentBuffer;

	// Token: 0x04000349 RID: 841
	private bool onBuffer0 = true;

	// Token: 0x0400034A RID: 842
	private Material updateMat;

	// Token: 0x0400034B RID: 843
	[HideInInspector]
	public Shader updateShader;

	// Token: 0x0400034C RID: 844
	private Vector4[] spawnBuffer = new Vector4[64];

	// Token: 0x0400034D RID: 845
	private Vector4[] colliderBuffer = new Vector4[64];

	// Token: 0x0400034E RID: 846
	private Texture2D terrainDesc;

	// Token: 0x0400034F RID: 847
	public Vector2 evaporation = new Vector2(0.01f, 0.01f);

	// Token: 0x04000350 RID: 848
	public Vector2 strength = new Vector2(1f, 1f);

	// Token: 0x04000351 RID: 849
	public Vector2 speed = new Vector2(1f, 1f);

	// Token: 0x04000352 RID: 850
	public Vector2 resistance = new Vector2(0.1f, 0.1f);

	// Token: 0x04000353 RID: 851
	public float wetnessEvaporation = 0.01f;

	// Token: 0x04000354 RID: 852
	public float burnEvaporation = 0.01f;

	// Token: 0x04000355 RID: 853
	private List<StreamEmitter> emitters = new List<StreamEmitter>(16);

	// Token: 0x04000356 RID: 854
	private List<StreamCollider> colliders = new List<StreamCollider>(16);

	// Token: 0x04000357 RID: 855
	private double timeSinceWetnessEvap;

	// Token: 0x04000358 RID: 856
	private double timeSinceBurnEvap;

	// Token: 0x04000359 RID: 857
	private double timeSinceEvapX;

	// Token: 0x0400035A RID: 858
	private double timeSinceEvapY;

	// Token: 0x0400035B RID: 859
	private Vector2 evapAmount = new Vector2(0f, 0f);
}
