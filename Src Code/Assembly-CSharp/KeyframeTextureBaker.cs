using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000113 RID: 275
public static class KeyframeTextureBaker
{
	// Token: 0x06000C8F RID: 3215 RVA: 0x0008B724 File Offset: 0x00089924
	private static float4 MultiplyQuaternion(float4 q1, float4 q2)
	{
		float w = q1.w * q2.w - math.dot(q1.xyz, q2.xyz);
		q1.xyz = q2.xyz * q1.w + q1.xyz * q2.w + math.cross(q1.xyz, q2.xyz);
		q1.w = w;
		return q1;
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0008B7A4 File Offset: 0x000899A4
	public static bool Skip(UnitAnimation.Index index)
	{
		for (int i = 0; i < UnitAnimation.no_bake.Length; i++)
		{
			if (UnitAnimation.no_bake[i] == index)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0008B7D0 File Offset: 0x000899D0
	public static string ClipIndexToName(int index, Animation anim)
	{
		AnimationClip clipByIndex = KeyframeTextureBaker.GetClipByIndex(index, anim);
		if (clipByIndex == null)
		{
			return null;
		}
		return clipByIndex.name;
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0008B7F8 File Offset: 0x000899F8
	public static AnimationClip GetClipByIndex(int index, Animation anim)
	{
		int num = 0;
		foreach (object obj in anim)
		{
			AnimationState animationState = (AnimationState)obj;
			if (num == index)
			{
				return animationState.clip;
			}
			num++;
		}
		return null;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0008B860 File Offset: 0x00089A60
	public static KeyframeTextureBaker.BakedData BakeClips(SkinnedMeshRenderer originalRenderer, Animation anim, ref KeyframeTextureBaker.BakedData first_data)
	{
		Profile.BeginSection("Baking animations");
		KeyframeTextureBaker.BakedData bakedData = new KeyframeTextureBaker.BakedData();
		bakedData.Mat = originalRenderer.sharedMaterial;
		bakedData.NewMesh = KeyframeTextureBaker.CreateMesh(originalRenderer, null);
		bakedData.Framerate = 30f;
		List<Matrix4x4[,]> list = new List<Matrix4x4[,]>();
		List<string> list2 = new List<string>();
		int num = 0;
		List<AnimationClip> list3 = new List<AnimationClip>();
		foreach (object obj in anim)
		{
			AnimationState animationState = (AnimationState)obj;
			Matrix4x4[,] array = KeyframeTextureBaker.SampleAnimationClip(animationState.clip, originalRenderer, anim, bakedData.Framerate);
			list3.Add(animationState.clip);
			list.Add(array);
			num += array.GetLength(0);
			list2.Add(animationState.name);
		}
		int length = list[0].GetLength(1);
		bakedData.Texture = new Texture2D(num, length * 2, TextureFormat.RGBAFloat, false);
		bakedData.Texture.wrapMode = TextureWrapMode.Clamp;
		bakedData.Texture.filterMode = FilterMode.Bilinear;
		bakedData.Texture.anisoLevel = 0;
		int num2 = 0;
		Vector4 v = Vector4.zero;
		for (int i = 0; i < list.Count; i++)
		{
			AnimationClip animationClip = list3[i];
			if (!(animationClip == null))
			{
				for (int j = 0; j < list[i].GetLength(1); j++)
				{
					for (int k = 0; k < list[i].GetLength(0); k++)
					{
						Vector4 v2;
						Vector4 vector;
						KeyframeTextureBaker.SampleMatrix(list[i][k, j], out v2, out vector);
						if (k > 0 && math.dot(v, vector) < 0f)
						{
							vector *= -1f;
						}
						v = vector;
						bakedData.Texture.SetPixel(num2 + k, j * 2, v2);
						bakedData.Texture.SetPixel(num2 + k, j * 2 + 1, Vector4.Normalize(vector));
					}
				}
				KeyframeTextureBaker.AnimationClipDataBaked animationClipDataBaked = default(KeyframeTextureBaker.AnimationClipDataBaked);
				animationClipDataBaked.BlendTime = 0.25f;
				animationClipDataBaked.AnimationLength = animationClip.length;
				float num3 = (float)(num2 + 1);
				float num4 = (float)(num2 + list[i].GetLength(0) - 2);
				float num5 = 1f / (float)num;
				animationClipDataBaked.TextureStart = num3 / (float)num + num5 * 0.5f;
				animationClipDataBaked.TextureEnd = num4 / (float)num + num5 * 0.5f;
				animationClipDataBaked.TextureRange = animationClipDataBaked.TextureEnd - animationClipDataBaked.TextureStart;
				List<KeyframeTextureBaker.ActionFrame> list4 = new List<KeyframeTextureBaker.ActionFrame>();
				for (int l = 0; l < animationClip.events.Length; l++)
				{
					try
					{
						list4.Add(new KeyframeTextureBaker.ActionFrame
						{
							type = (KeyframeTextureBaker.ActionFrame.ActionType)Enum.Parse(typeof(KeyframeTextureBaker.ActionFrame.ActionType), animationClip.events[l].stringParameter, true),
							time = animationClip.events[l].time
						});
					}
					catch
					{
					}
				}
				if (list4.Count > 0)
				{
					animationClipDataBaked.event0 = list4[0];
				}
				if (list4.Count > 1)
				{
					animationClipDataBaked.event1 = list4[1];
				}
				if (list4.Count > 2)
				{
					animationClipDataBaked.event2 = list4[2];
				}
				if (list4.Count > 3)
				{
					animationClipDataBaked.event3 = list4[3];
				}
				bakedData.Animations.Add(animationClipDataBaked);
				num2 += list[i].GetLength(0);
			}
		}
		bakedData.state_names = list2.ToArray();
		bakedData.Texture.Apply(false, false);
		Profile.EndSection("Baking animations");
		return bakedData;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0008BC48 File Offset: 0x00089E48
	private static Mesh CreateMesh(SkinnedMeshRenderer originalRenderer, Mesh mesh = null)
	{
		Mesh mesh2 = new Mesh();
		Mesh mesh3 = (mesh == null) ? originalRenderer.sharedMesh : mesh;
		BoneWeight[] boneWeights = mesh3.boneWeights;
		mesh3.CopyMeshData(mesh2);
		Vector3[] vertices = mesh3.vertices;
		Vector2[] array = new Vector2[mesh3.vertexCount];
		Vector2[] array2 = new Vector2[mesh3.vertexCount];
		Vector2[] array3 = new Vector2[mesh3.vertexCount];
		Vector2[] array4 = new Vector2[mesh3.vertexCount];
		int[] array5 = null;
		if (mesh != null)
		{
			Matrix4x4[] bindposes = originalRenderer.sharedMesh.bindposes;
			Matrix4x4[] newBindPoseMatrices = mesh.bindposes;
			if (newBindPoseMatrices.Length == bindposes.Length)
			{
				array5 = new int[bindposes.Length];
				int k;
				int i;
				for (i = 0; i < array5.Length; i = k + 1)
				{
					array5[i] = Array.FindIndex<Matrix4x4>(bindposes, (Matrix4x4 x) => x == newBindPoseMatrices[i]);
					k = i;
				}
			}
		}
		Vector3[] array6 = new Vector3[mesh3.vertexCount];
		for (int j = 0; j < mesh3.vertexCount; j++)
		{
			array6[j] = vertices[j];
			int num = boneWeights[j].boneIndex0;
			int num2 = boneWeights[j].boneIndex1;
			int num3 = boneWeights[j].boneIndex2;
			int num4 = boneWeights[j].boneIndex3;
			if (array5 != null)
			{
				num = array5[num];
				num2 = array5[num2];
				num3 = array5[num3];
				num4 = array5[num4];
			}
			array[j] = new Vector2((float)num, (float)num2);
			array2[j] = new Vector2((float)num3, (float)num4);
			float num5 = boneWeights[j].weight0 + boneWeights[j].weight1 + boneWeights[j].weight2 + boneWeights[j].weight3;
			array3[j] = new Vector2(boneWeights[j].weight0 / num5, boneWeights[j].weight1 / num5);
			array4[j] = new Vector2(boneWeights[j].weight2 / num5, boneWeights[j].weight3 / num5);
		}
		mesh2.vertices = array6;
		mesh2.uv2 = array;
		mesh2.uv4 = array2;
		mesh2.uv3 = array3;
		mesh2.uv5 = array4;
		return mesh2;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0008BED8 File Offset: 0x0008A0D8
	private static Matrix4x4[,] SampleAnimationClip(UnitAnimation.Index state, SkinnedMeshRenderer renderer, Animator anim, float framerate)
	{
		if (state == UnitAnimation.Index.None)
		{
			state = UnitAnimation.Index.Idle;
		}
		anim.Play(state.ToString());
		anim.Update(0f);
		Transform[] bones = renderer.bones;
		Matrix4x4[,] array = new Matrix4x4[Mathf.CeilToInt(framerate * anim.GetCurrentAnimatorStateInfo(0).length) + 3, bones.Length];
		Matrix4x4 inverse = renderer.localToWorldMatrix.inverse;
		Matrix4x4[] bindposes = renderer.sharedMesh.bindposes;
		for (int i = 1; i < array.GetLength(0) - 1; i++)
		{
			float normalizedTime = (float)(i - 1) / (float)(array.GetLength(0) - 3);
			anim.Play(state.ToString(), 0, normalizedTime);
			anim.Update(0f);
			for (int j = 0; j < bones.Length; j++)
			{
				array[i, j] = inverse * bones[j].localToWorldMatrix * bindposes[j];
			}
		}
		for (int k = 0; k < bones.Length; k++)
		{
			array[0, k] = array[array.GetLength(0) - 2, k];
			array[array.GetLength(0) - 1, k] = array[1, k];
		}
		return array;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0008C018 File Offset: 0x0008A218
	private static Matrix4x4[,] SampleAnimationClip(string state, SkinnedMeshRenderer renderer, Animator anim, float framerate)
	{
		anim.Play(state);
		anim.Update(0f);
		Transform[] bones = renderer.bones;
		Matrix4x4[,] array = new Matrix4x4[Mathf.CeilToInt(framerate * anim.GetCurrentAnimatorStateInfo(0).length) + 3, bones.Length];
		Matrix4x4 inverse = renderer.localToWorldMatrix.inverse;
		Matrix4x4[] bindposes = renderer.sharedMesh.bindposes;
		for (int i = 1; i < array.GetLength(0) - 1; i++)
		{
			float normalizedTime = (float)(i - 1) / (float)(array.GetLength(0) - 3);
			anim.Play(state.ToString(), 0, normalizedTime);
			anim.Update(0f);
			for (int j = 0; j < bones.Length; j++)
			{
				array[i, j] = inverse * bones[j].localToWorldMatrix * bindposes[j];
			}
		}
		for (int k = 0; k < bones.Length; k++)
		{
			array[0, k] = array[array.GetLength(0) - 2, k];
			array[array.GetLength(0) - 1, k] = array[1, k];
		}
		return array;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0008C140 File Offset: 0x0008A340
	private static Matrix4x4[,] SampleAnimationClip(AnimationClip clip, SkinnedMeshRenderer renderer, Animation anim, float framerate)
	{
		Transform[] bones = renderer.bones;
		Matrix4x4 inverse = renderer.localToWorldMatrix.inverse;
		Matrix4x4[] bindposes = renderer.sharedMesh.bindposes;
		Matrix4x4[,] array = new Matrix4x4[Mathf.CeilToInt(framerate * clip.length) + 3, bones.Length];
		AnimationState animationState = anim[clip.name];
		animationState.enabled = true;
		animationState.weight = 1f;
		for (int i = 1; i < array.GetLength(0) - 1; i++)
		{
			float normalizedTime = (float)(i - 1) / (float)(array.GetLength(0) - 3);
			animationState.normalizedTime = normalizedTime;
			anim.Sample();
			for (int j = 0; j < bones.Length; j++)
			{
				array[i, j] = inverse * bones[j].localToWorldMatrix * bindposes[j];
			}
		}
		for (int k = 0; k < bones.Length; k++)
		{
			array[0, k] = array[array.GetLength(0) - 2, k];
			array[array.GetLength(0) - 1, k] = array[1, k];
		}
		animationState.enabled = false;
		animationState.weight = 0f;
		return array;
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0008C278 File Offset: 0x0008A478
	public static void CopyMeshData(this Mesh originalMesh, Mesh newMesh)
	{
		Vector3[] vertices = originalMesh.vertices;
		newMesh.Clear();
		newMesh.vertices = vertices;
		newMesh.triangles = originalMesh.triangles;
		newMesh.normals = originalMesh.normals;
		newMesh.uv = originalMesh.uv;
		newMesh.uv2 = originalMesh.uv2;
		newMesh.uv3 = originalMesh.uv3;
		newMesh.uv4 = originalMesh.uv4;
		newMesh.uv5 = originalMesh.uv5;
		newMesh.uv6 = originalMesh.uv6;
		newMesh.uv7 = originalMesh.uv7;
		newMesh.uv8 = originalMesh.uv8;
		newMesh.tangents = originalMesh.tangents;
		newMesh.name = originalMesh.name;
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0008C32C File Offset: 0x0008A52C
	private static void SampleMatrix(Matrix4x4 m, out Vector4 position, out Vector4 rotation)
	{
		float w = (m.GetColumn(0).magnitude + m.GetColumn(1).magnitude + m.GetColumn(2).magnitude) / 3f;
		Vector3 vector = m.GetColumn(3);
		position = new Vector4(vector.x, vector.y, vector.z, w);
		Quaternion quaternion = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		rotation = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
	}

	// Token: 0x02000611 RID: 1553
	[Serializable]
	public class BakedData
	{
		// Token: 0x040033B1 RID: 13233
		public Quaternion forwardRot;

		// Token: 0x040033B2 RID: 13234
		public Texture2D Texture;

		// Token: 0x040033B3 RID: 13235
		public Mesh NewMesh;

		// Token: 0x040033B4 RID: 13236
		public float Framerate;

		// Token: 0x040033B5 RID: 13237
		public Material Mat;

		// Token: 0x040033B6 RID: 13238
		public float scale = 1f;

		// Token: 0x040033B7 RID: 13239
		public List<KeyframeTextureBaker.AnimationClipDataBaked> Animations = new List<KeyframeTextureBaker.AnimationClipDataBaked>();

		// Token: 0x040033B8 RID: 13240
		public string[] state_names;
	}

	// Token: 0x02000612 RID: 1554
	[Serializable]
	public struct AnimationClipDataBaked
	{
		// Token: 0x060046BB RID: 18107 RVA: 0x0020FBF8 File Offset: 0x0020DDF8
		public bool HasEvent(KeyframeTextureBaker.ActionFrame.ActionType key)
		{
			return this.event0.type == key || this.event1.type == key || this.event2.type == key || this.event3.type == key;
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x0020FC46 File Offset: 0x0020DE46
		public int FreeEventID()
		{
			if (this.event0.type == KeyframeTextureBaker.ActionFrame.ActionType.None)
			{
				return 0;
			}
			if (this.event1.type == KeyframeTextureBaker.ActionFrame.ActionType.None)
			{
				return 1;
			}
			if (this.event2.type == KeyframeTextureBaker.ActionFrame.ActionType.None)
			{
				return 2;
			}
			if (this.event3.type == KeyframeTextureBaker.ActionFrame.ActionType.None)
			{
				return 3;
			}
			return -1;
		}

		// Token: 0x060046BD RID: 18109 RVA: 0x0020FC85 File Offset: 0x0020DE85
		public void SetEvent(KeyframeTextureBaker.ActionFrame temp_event, int free_id)
		{
			switch (free_id)
			{
			case 0:
				this.event0 = temp_event;
				return;
			case 1:
				this.event1 = temp_event;
				return;
			case 2:
				this.event2 = temp_event;
				return;
			case 3:
				this.event3 = temp_event;
				return;
			default:
				return;
			}
		}

		// Token: 0x060046BE RID: 18110 RVA: 0x0020FCC0 File Offset: 0x0020DEC0
		public float GetAction(KeyframeTextureBaker.ActionFrame.ActionType key)
		{
			if (this.event0.type == key)
			{
				return this.event0.time;
			}
			if (this.event1.type == key)
			{
				return this.event1.time;
			}
			if (this.event2.type == key)
			{
				return this.event2.time;
			}
			if (this.event3.type == key)
			{
				return this.event3.time;
			}
			return -1f;
		}

		// Token: 0x040033B9 RID: 13241
		public float TextureStart;

		// Token: 0x040033BA RID: 13242
		public float TextureEnd;

		// Token: 0x040033BB RID: 13243
		public float TextureRange;

		// Token: 0x040033BC RID: 13244
		public float AnimationLength;

		// Token: 0x040033BD RID: 13245
		public float BlendTime;

		// Token: 0x040033BE RID: 13246
		public bool valid;

		// Token: 0x040033BF RID: 13247
		public KeyframeTextureBaker.ActionFrame event0;

		// Token: 0x040033C0 RID: 13248
		public KeyframeTextureBaker.ActionFrame event1;

		// Token: 0x040033C1 RID: 13249
		public KeyframeTextureBaker.ActionFrame event2;

		// Token: 0x040033C2 RID: 13250
		public KeyframeTextureBaker.ActionFrame event3;
	}

	// Token: 0x02000613 RID: 1555
	[Serializable]
	public struct ActionFrame
	{
		// Token: 0x040033C3 RID: 13251
		public KeyframeTextureBaker.ActionFrame.ActionType type;

		// Token: 0x040033C4 RID: 13252
		public float time;

		// Token: 0x020009EB RID: 2539
		public enum ActionType
		{
			// Token: 0x040045DC RID: 17884
			None,
			// Token: 0x040045DD RID: 17885
			Attack,
			// Token: 0x040045DE RID: 17886
			LeftFoot,
			// Token: 0x040045DF RID: 17887
			RightFoot
		}
	}
}
