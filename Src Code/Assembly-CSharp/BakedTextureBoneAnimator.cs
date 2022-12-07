using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class BakedTextureBoneAnimator
{
	// Token: 0x06000C80 RID: 3200 RVA: 0x0008B250 File Offset: 0x00089450
	private static void ReadBoneData(ref Quaternion rotation, ref float3 translation, ref float objscale, float texturePosition, int boneId, Color[] animation_keyframe)
	{
		int num = boneId * 2;
		int num2 = boneId * 2 + 1;
		float4 @float = animation_keyframe[num];
		float4 float2 = animation_keyframe[num2];
		rotation = new Quaternion(float2.x, float2.y, float2.z, float2.w);
		translation = math.float3(@float.xyz);
		objscale = @float.w;
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0008B2CC File Offset: 0x000894CC
	private static Color[] GetAnimationKeyframeData(Texture2D animation_texture, float texture_position)
	{
		int x = (int)(texture_position * (float)animation_texture.width);
		return animation_texture.GetPixels(x, 0, 1, animation_texture.height, 0);
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0008B2F4 File Offset: 0x000894F4
	public static void SetupBonesFromAnimationTexture(SkinnedMeshRenderer skinned_mesh_renderer, float3 animation_coord, Texture2D animation_texture)
	{
		float x = animation_coord.x;
		BakedTextureBoneAnimator.<>c__DisplayClass2_0 CS$<>8__locals1;
		CS$<>8__locals1.bones = skinned_mesh_renderer.bones;
		Matrix4x4 localToWorldMatrix = skinned_mesh_renderer.localToWorldMatrix;
		Matrix4x4[] bindposes = skinned_mesh_renderer.sharedMesh.bindposes;
		CS$<>8__locals1.bones_count = math.min(CS$<>8__locals1.bones.Length, bindposes.Length);
		Color[] animationKeyframeData = BakedTextureBoneAnimator.GetAnimationKeyframeData(animation_texture, x);
		CS$<>8__locals1.bone_local_to_worlds = new Matrix4x4[CS$<>8__locals1.bones_count];
		for (int i = 0; i < CS$<>8__locals1.bones_count; i++)
		{
			Quaternion identity = Quaternion.identity;
			float3 v = 0;
			float d = 0f;
			BakedTextureBoneAnimator.ReadBoneData(ref identity, ref v, ref d, x, i, animationKeyframeData);
			Matrix4x4 rhs = Matrix4x4.TRS(v, identity, Vector3.one * d);
			CS$<>8__locals1.bone_local_to_worlds[i] = localToWorldMatrix * rhs * bindposes[i].inverse;
		}
		for (int j = 0; j < 2; j++)
		{
			BakedTextureBoneAnimator.<SetupBonesFromAnimationTexture>g__FixBones|2_0(ref CS$<>8__locals1);
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0008B3F0 File Offset: 0x000895F0
	[CompilerGenerated]
	internal static void <SetupBonesFromAnimationTexture>g__FixBones|2_0(ref BakedTextureBoneAnimator.<>c__DisplayClass2_0 A_0)
	{
		for (int i = 0; i < A_0.bones_count; i++)
		{
			Transform transform = A_0.bones[i];
			Matrix4x4 matrix4x = A_0.bone_local_to_worlds[i];
			if (transform != null)
			{
				transform.SetPositionAndRotation(matrix4x.GetColumn(3), matrix4x.rotation);
			}
		}
	}
}
