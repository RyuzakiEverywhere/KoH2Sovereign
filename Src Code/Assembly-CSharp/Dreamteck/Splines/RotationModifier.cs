using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004BA RID: 1210
	[Serializable]
	public class RotationModifier : SplineSampleModifier
	{
		// Token: 0x06003F9A RID: 16282 RVA: 0x001E5FFF File Offset: 0x001E41FF
		public RotationModifier()
		{
			this.keys = new List<RotationModifier.RotationKey>();
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x001E6020 File Offset: 0x001E4220
		public override List<SplineSampleModifier.Key> GetKeys()
		{
			List<SplineSampleModifier.Key> list = new List<SplineSampleModifier.Key>();
			for (int i = 0; i < this.keys.Count; i++)
			{
				list.Add(this.keys[i]);
			}
			return list;
		}

		// Token: 0x06003F9C RID: 16284 RVA: 0x001E605C File Offset: 0x001E425C
		public override void SetKeys(List<SplineSampleModifier.Key> input)
		{
			this.keys = new List<RotationModifier.RotationKey>();
			for (int i = 0; i < input.Count; i++)
			{
				this.keys.Add((RotationModifier.RotationKey)input[i]);
			}
			base.SetKeys(input);
		}

		// Token: 0x06003F9D RID: 16285 RVA: 0x001E60A3 File Offset: 0x001E42A3
		public void AddKey(Vector3 rotation, double f, double t)
		{
			this.keys.Add(new RotationModifier.RotationKey(rotation, f, t, this));
		}

		// Token: 0x06003F9E RID: 16286 RVA: 0x001E60BC File Offset: 0x001E42BC
		public override void Apply(SplineSample result)
		{
			if (this.keys.Count == 0)
			{
				return;
			}
			base.Apply(result);
			Quaternion quaternion = Quaternion.identity;
			Quaternion quaternion2 = result.rotation;
			for (int i = 0; i < this.keys.Count; i++)
			{
				if (this.keys[i].useLookTarget && this.keys[i].target != null)
				{
					Quaternion b = Quaternion.LookRotation(this.keys[i].target.position - result.position);
					quaternion2 = Quaternion.Slerp(quaternion2, b, this.keys[i].Evaluate(result.percent));
				}
				else
				{
					Quaternion rhs = Quaternion.Euler(this.keys[i].rotation.x, this.keys[i].rotation.y, this.keys[i].rotation.z);
					quaternion = Quaternion.Slerp(quaternion, quaternion * rhs, this.keys[i].Evaluate(result.percent));
				}
			}
			Quaternion rotation = quaternion2 * quaternion;
			Vector3 point = Quaternion.Inverse(result.rotation) * result.up;
			result.forward = rotation * Vector3.forward;
			result.up = rotation * point;
		}

		// Token: 0x04002CE7 RID: 11495
		public List<RotationModifier.RotationKey> keys = new List<RotationModifier.RotationKey>();

		// Token: 0x02000996 RID: 2454
		[Serializable]
		public class RotationKey : SplineSampleModifier.Key
		{
			// Token: 0x0600543F RID: 21567 RVA: 0x002460ED File Offset: 0x002442ED
			public RotationKey(Vector3 rotation, double f, double t, RotationModifier modifier) : base(f, t, modifier)
			{
				this.rotation = rotation;
			}

			// Token: 0x0400449D RID: 17565
			public bool useLookTarget;

			// Token: 0x0400449E RID: 17566
			public Transform target;

			// Token: 0x0400449F RID: 17567
			public Vector3 rotation = Vector3.zero;
		}
	}
}
