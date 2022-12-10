using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004D3 RID: 1235
	[Serializable]
	public class TransformModule
	{
		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600419D RID: 16797 RVA: 0x001F317E File Offset: 0x001F137E
		// (set) Token: 0x0600419E RID: 16798 RVA: 0x001F3186 File Offset: 0x001F1386
		public Vector2 offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				if (value != this._offset)
				{
					this._offset = value;
					if (this.targetUser != null)
					{
						this.targetUser.Rebuild();
					}
				}
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x0600419F RID: 16799 RVA: 0x001F31B6 File Offset: 0x001F13B6
		// (set) Token: 0x060041A0 RID: 16800 RVA: 0x001F31BE File Offset: 0x001F13BE
		public Vector3 rotationOffset
		{
			get
			{
				return this._rotationOffset;
			}
			set
			{
				if (value != this._rotationOffset)
				{
					this._rotationOffset = value;
					if (this.targetUser != null)
					{
						this.targetUser.Rebuild();
					}
				}
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060041A1 RID: 16801 RVA: 0x001F31EE File Offset: 0x001F13EE
		// (set) Token: 0x060041A2 RID: 16802 RVA: 0x001F31F6 File Offset: 0x001F13F6
		public Vector3 baseScale
		{
			get
			{
				return this._baseScale;
			}
			set
			{
				if (value != this._baseScale)
				{
					this._baseScale = value;
					if (this.targetUser != null)
					{
						this.targetUser.Rebuild();
					}
				}
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060041A3 RID: 16803 RVA: 0x001F3226 File Offset: 0x001F1426
		// (set) Token: 0x060041A4 RID: 16804 RVA: 0x001F3241 File Offset: 0x001F1441
		public SplineSample splineResult
		{
			get
			{
				if (this._splineResult == null)
				{
					this._splineResult = new SplineSample();
				}
				return this._splineResult;
			}
			set
			{
				if (this._splineResult == null)
				{
					this._splineResult = new SplineSample(value);
					return;
				}
				this._splineResult.CopyFrom(value);
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x060041A5 RID: 16805 RVA: 0x001F3264 File Offset: 0x001F1464
		// (set) Token: 0x060041A6 RID: 16806 RVA: 0x001F3280 File Offset: 0x001F1480
		public bool applyPosition
		{
			get
			{
				return this.applyPositionX || this.applyPositionY || this.applyPositionZ;
			}
			set
			{
				this.applyPositionZ = value;
				this.applyPositionY = value;
				this.applyPositionX = value;
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060041A7 RID: 16807 RVA: 0x001F32A6 File Offset: 0x001F14A6
		// (set) Token: 0x060041A8 RID: 16808 RVA: 0x001F32C0 File Offset: 0x001F14C0
		public bool applyRotation
		{
			get
			{
				return this.applyRotationX || this.applyRotationY || this.applyRotationZ;
			}
			set
			{
				this.applyRotationZ = value;
				this.applyRotationY = value;
				this.applyRotationX = value;
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x060041A9 RID: 16809 RVA: 0x001F32E6 File Offset: 0x001F14E6
		// (set) Token: 0x060041AA RID: 16810 RVA: 0x001F3300 File Offset: 0x001F1500
		public bool applyScale
		{
			get
			{
				return this.applyScaleX || this.applyScaleY || this.applyScaleZ;
			}
			set
			{
				this.applyScaleZ = value;
				this.applyScaleY = value;
				this.applyScaleX = value;
			}
		}

		// Token: 0x060041AB RID: 16811 RVA: 0x001F3326 File Offset: 0x001F1526
		public void ApplyTransform(Transform input)
		{
			input.position = this.GetPosition(input.position);
			input.rotation = this.GetRotation(input.rotation);
			input.localScale = this.GetScale(input.localScale);
		}

		// Token: 0x060041AC RID: 16812 RVA: 0x001F3360 File Offset: 0x001F1560
		public void ApplyRigidbody(Rigidbody input)
		{
			input.transform.localScale = this.GetScale(input.transform.localScale);
			input.MovePosition(this.GetPosition(input.position));
			input.velocity = this.HandleVelocity(input.velocity);
			Vector3 vector = input.velocity;
			input.velocity = vector;
			input.MoveRotation(this.GetRotation(input.rotation));
			vector = input.angularVelocity;
			if (this.applyRotationX)
			{
				vector.x = 0f;
			}
			if (this.applyRotationY)
			{
				vector.y = 0f;
			}
			if (this.applyRotationZ)
			{
				vector.z = 0f;
			}
			input.angularVelocity = vector;
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x001F3418 File Offset: 0x001F1618
		public void ApplyRigidbody2D(Rigidbody2D input)
		{
			input.transform.localScale = this.GetScale(input.transform.localScale);
			input.position = this.GetPosition(input.position);
			input.velocity = this.HandleVelocity(input.velocity);
			input.rotation = -this.GetRotation(Quaternion.Euler(0f, 0f, input.rotation)).eulerAngles.z;
			if (this.applyRotationX)
			{
				input.angularVelocity = 0f;
			}
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x001F34BC File Offset: 0x001F16BC
		private Vector3 HandleVelocity(Vector3 velocity)
		{
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.right;
			switch (this.velocityHandleMode)
			{
			case TransformModule.VelocityHandleMode.Preserve:
				vector = velocity;
				break;
			case TransformModule.VelocityHandleMode.Align:
				vector2 = this._splineResult.forward;
				if (Vector3.Dot(velocity, vector2) < 0f)
				{
					vector2 *= -1f;
				}
				vector = vector2 * velocity.magnitude;
				break;
			case TransformModule.VelocityHandleMode.AlignRealistic:
				vector2 = this._splineResult.forward;
				if (Vector3.Dot(velocity, vector2) < 0f)
				{
					vector2 *= -1f;
				}
				vector = vector2 * velocity.magnitude * Vector3.Dot(velocity.normalized, vector2);
				break;
			}
			if (this.applyPositionX)
			{
				velocity.x = vector.x;
			}
			if (this.applyPositionY)
			{
				velocity.y = vector.y;
			}
			if (this.applyPositionZ)
			{
				velocity.z = vector.z;
			}
			return velocity;
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x001F35B8 File Offset: 0x001F17B8
		private Vector3 GetPosition(Vector3 inputPosition)
		{
			TransformModule.position = this._splineResult.position;
			Vector2 offset = this._offset;
			if (offset != Vector2.zero)
			{
				TransformModule.position += this._splineResult.right * offset.x * this._splineResult.size + this._splineResult.up * offset.y * this._splineResult.size;
			}
			if (this.applyPositionX)
			{
				inputPosition.x = TransformModule.position.x;
			}
			if (this.applyPositionY)
			{
				inputPosition.y = TransformModule.position.y;
			}
			if (this.applyPositionZ)
			{
				inputPosition.z = TransformModule.position.z;
			}
			return inputPosition;
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x001F3698 File Offset: 0x001F1898
		private Quaternion GetRotation(Quaternion inputRotation)
		{
			TransformModule.rotation = Quaternion.LookRotation(this._splineResult.forward * ((this.direction == Spline.Direction.Forward) ? 1f : -1f), this._splineResult.up);
			if (this._rotationOffset != Vector3.zero)
			{
				TransformModule.rotation *= Quaternion.Euler(this._rotationOffset);
			}
			if (!this.applyRotationX || !this.applyRotationY)
			{
				Vector3 eulerAngles = TransformModule.rotation.eulerAngles;
				if (!this.applyRotationX)
				{
					eulerAngles.x = inputRotation.eulerAngles.x;
				}
				if (!this.applyRotationY)
				{
					eulerAngles.y = inputRotation.eulerAngles.y;
				}
				if (!this.applyRotationZ)
				{
					eulerAngles.z = inputRotation.eulerAngles.z;
				}
				inputRotation.eulerAngles = eulerAngles;
			}
			else
			{
				inputRotation = TransformModule.rotation;
			}
			return inputRotation;
		}

		// Token: 0x060041B1 RID: 16817 RVA: 0x001F378C File Offset: 0x001F198C
		private Vector3 GetScale(Vector3 inputScale)
		{
			if (this.applyScaleX)
			{
				inputScale.x = this._baseScale.x * this._splineResult.size;
			}
			if (this.applyScaleY)
			{
				inputScale.y = this._baseScale.y * this._splineResult.size;
			}
			if (this.applyScaleZ)
			{
				inputScale.z = this._baseScale.z * this._splineResult.size;
			}
			return inputScale;
		}

		// Token: 0x04002DB1 RID: 11697
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("offset")]
		private Vector2 _offset;

		// Token: 0x04002DB2 RID: 11698
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("rotationOffset")]
		private Vector3 _rotationOffset = Vector3.zero;

		// Token: 0x04002DB3 RID: 11699
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("baseScale")]
		private Vector3 _baseScale = Vector3.one;

		// Token: 0x04002DB4 RID: 11700
		public TransformModule.VelocityHandleMode velocityHandleMode;

		// Token: 0x04002DB5 RID: 11701
		private SplineSample _splineResult;

		// Token: 0x04002DB6 RID: 11702
		public bool applyPositionX = true;

		// Token: 0x04002DB7 RID: 11703
		public bool applyPositionY = true;

		// Token: 0x04002DB8 RID: 11704
		public bool applyPositionZ = true;

		// Token: 0x04002DB9 RID: 11705
		public Spline.Direction direction = Spline.Direction.Forward;

		// Token: 0x04002DBA RID: 11706
		public bool applyRotationX = true;

		// Token: 0x04002DBB RID: 11707
		public bool applyRotationY = true;

		// Token: 0x04002DBC RID: 11708
		public bool applyRotationZ = true;

		// Token: 0x04002DBD RID: 11709
		public bool applyScaleX;

		// Token: 0x04002DBE RID: 11710
		public bool applyScaleY;

		// Token: 0x04002DBF RID: 11711
		public bool applyScaleZ;

		// Token: 0x04002DC0 RID: 11712
		[HideInInspector]
		public SplineUser targetUser;

		// Token: 0x04002DC1 RID: 11713
		private static Vector3 position = Vector3.zero;

		// Token: 0x04002DC2 RID: 11714
		private static Quaternion rotation = Quaternion.identity;

		// Token: 0x020009B5 RID: 2485
		public enum VelocityHandleMode
		{
			// Token: 0x0400452E RID: 17710
			Zero,
			// Token: 0x0400452F RID: 17711
			Preserve,
			// Token: 0x04004530 RID: 17712
			Align,
			// Token: 0x04004531 RID: 17713
			AlignRealistic
		}
	}
}
