using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C1 RID: 1217
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Spline Mesh")]
	public class SplineMesh : MeshGenerator
	{
		// Token: 0x06004044 RID: 16452 RVA: 0x001E97E2 File Offset: 0x001E79E2
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "Extruded Mesh";
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x001E97FA File Offset: 0x001E79FA
		protected override void Reset()
		{
			base.Reset();
			this.AddChannel("Channel 1");
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x001E980E File Offset: 0x001E7A0E
		public void RemoveChannel(int index)
		{
			this.channels.RemoveAt(index);
			this.Rebuild();
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x001E9824 File Offset: 0x001E7A24
		public void SwapChannels(int a, int b)
		{
			if (a < 0 || a >= this.channels.Count || b < 0 || b >= this.channels.Count)
			{
				return;
			}
			SplineMesh.Channel value = this.channels[b];
			this.channels[b] = this.channels[a];
			this.channels[a] = value;
			this.Rebuild();
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x001E9890 File Offset: 0x001E7A90
		public SplineMesh.Channel AddChannel(Mesh inputMesh, string name)
		{
			SplineMesh.Channel channel = new SplineMesh.Channel(name, inputMesh, this);
			this.channels.Add(channel);
			return channel;
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x001E98B4 File Offset: 0x001E7AB4
		public SplineMesh.Channel AddChannel(string name)
		{
			SplineMesh.Channel channel = new SplineMesh.Channel(name, this);
			this.channels.Add(channel);
			return channel;
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x001E98D6 File Offset: 0x001E7AD6
		public int GetChannelCount()
		{
			return this.channels.Count;
		}

		// Token: 0x0600404B RID: 16459 RVA: 0x001E98E3 File Offset: 0x001E7AE3
		public SplineMesh.Channel GetChannel(int index)
		{
			return this.channels[index];
		}

		// Token: 0x0600404C RID: 16460 RVA: 0x001E98F1 File Offset: 0x001E7AF1
		protected override void BuildMesh()
		{
			if (base.sampleCount == 0)
			{
				return;
			}
			base.BuildMesh();
			this.Generate();
		}

		// Token: 0x0600404D RID: 16461 RVA: 0x001E9908 File Offset: 0x001E7B08
		private void Generate()
		{
			this.meshCount = 0;
			for (int i = 0; i < this.channels.Count; i++)
			{
				if (this.channels[i].GetMeshCount() != 0)
				{
					this.meshCount += this.channels[i].count;
				}
			}
			if (this.meshCount == 0)
			{
				this.tsMesh.Clear();
				return;
			}
			if (this.combineMeshes.Count < this.meshCount)
			{
				this.combineMeshes.AddRange(new TS_Mesh[this.meshCount - this.combineMeshes.Count]);
			}
			else if (this.combineMeshes.Count > this.meshCount)
			{
				this.combineMeshes.RemoveRange(this.combineMeshes.Count - 1 - (this.combineMeshes.Count - this.meshCount), this.combineMeshes.Count - this.meshCount);
			}
			int num = 0;
			for (int j = 0; j < this.channels.Count; j++)
			{
				if (this.channels[j].GetMeshCount() != 0)
				{
					if (this.channels[j].autoCount)
					{
						float num2 = 0f;
						for (int k = 0; k < this.channels[j].GetMeshCount(); k++)
						{
							num2 += this.channels[j].GetMesh(k).bounds.size.z;
						}
						if (this.channels[j].GetMeshCount() > 1)
						{
							num2 /= (float)this.channels[j].GetMeshCount();
						}
						if (num2 > 0f)
						{
							float num3 = base.CalculateLength(this.channels[j].clipFrom, this.channels[j].clipTo);
							this.channels[j].count = Mathf.RoundToInt(num3 / num2);
							if (this.channels[j].count < 1)
							{
								this.channels[j].count = 1;
							}
						}
					}
					this.channels[j].ResetIteration();
					this.useLastResult = false;
					double num4 = 1.0 / (double)this.channels[j].count;
					double num5 = num4 * this.channels[j].spacing * 0.5;
					SplineMesh.Channel.Type type = this.channels[j].type;
					if (type != SplineMesh.Channel.Type.Extrude)
					{
						if (type == SplineMesh.Channel.Type.Place)
						{
							for (int l = 0; l < this.channels[j].count; l++)
							{
								if (this.combineMeshes[num] == null)
								{
									this.combineMeshes[num] = new TS_Mesh();
								}
								this.Place(this.channels[j], this.combineMeshes[num], DMath.Lerp(this.channels[j].clipFrom, this.channels[j].clipTo, (double)l / (double)Mathf.Max(this.channels[j].count - 1, 1)));
								num++;
							}
						}
					}
					else
					{
						for (int m = 0; m < this.channels[j].count; m++)
						{
							double from = DMath.Lerp(this.channels[j].clipFrom, this.channels[j].clipTo, (double)m * num4 + num5);
							double to = DMath.Lerp(this.channels[j].clipFrom, this.channels[j].clipTo, (double)m * num4 + num4 - num5);
							if (this.combineMeshes[num] == null)
							{
								this.combineMeshes[num] = new TS_Mesh();
							}
							this.Stretch(this.channels[j], this.combineMeshes[num], from, to);
							num++;
						}
						if (num5 == 0.0)
						{
							this.useLastResult = true;
						}
					}
				}
			}
			if (this.tsMesh == null)
			{
				this.tsMesh = new TS_Mesh();
			}
			else
			{
				this.tsMesh.Clear();
			}
			this.tsMesh.Combine(this.combineMeshes, false);
		}

		// Token: 0x0600404E RID: 16462 RVA: 0x001E9D78 File Offset: 0x001E7F78
		private void Place(SplineMesh.Channel channel, TS_Mesh target, double percent)
		{
			SplineMesh.Channel.MeshDefinition meshDefinition = channel.NextMesh();
			if (target == null)
			{
				target = new TS_Mesh();
			}
			meshDefinition.Write(target, channel.overrideMaterialID ? channel.targetMaterialID : -1);
			Vector2 vector = channel.NextOffset(percent);
			base.Evaluate(percent, this.evalResult);
			base.ModifySample(this.evalResult);
			Vector3 up = this.evalResult.up;
			Vector3 right = this.evalResult.right;
			Vector3 forward = this.evalResult.forward;
			if (channel.overrideNormal)
			{
				this.evalResult.forward = Vector3.Cross(this.evalResult.right, channel.customNormal);
				this.evalResult.up = channel.customNormal;
			}
			Quaternion identity = Quaternion.identity;
			this.vertexMatrix.SetTRS(this.evalResult.position + right * (base.offset.x + vector.x) * this.evalResult.size + up * (base.offset.y + vector.y) * this.evalResult.size + forward * base.offset.z, this.evalResult.rotation * channel.NextPlaceRotation(percent) * Quaternion.AngleAxis(base.rotation, Vector3.forward), channel.NextPlaceScale() * this.evalResult.size);
			this.normalMatrix = this.vertexMatrix.inverse.transpose;
			for (int i = 0; i < target.vertexCount; i++)
			{
				target.vertices[i] = this.vertexMatrix.MultiplyPoint3x4(meshDefinition.vertices[i]);
				target.normals[i] = this.normalMatrix.MultiplyVector(meshDefinition.normals[i]);
			}
			for (int j = 0; j < Mathf.Min(target.colors.Length, meshDefinition.colors.Length); j++)
			{
				target.colors[j] = meshDefinition.colors[j] * this.evalResult.color;
			}
		}

		// Token: 0x0600404F RID: 16463 RVA: 0x001E9FC4 File Offset: 0x001E81C4
		private void Stretch(SplineMesh.Channel channel, TS_Mesh target, double from, double to)
		{
			SplineMesh.Channel.MeshDefinition meshDefinition = channel.NextMesh();
			if (target == null)
			{
				target = new TS_Mesh();
			}
			meshDefinition.Write(target, channel.overrideMaterialID ? channel.targetMaterialID : -1);
			Vector2 vector = Vector2.zero;
			Vector3 vector2 = Vector3.zero;
			for (int i = 0; i < meshDefinition.vertexGroups.Count; i++)
			{
				if (this.useLastResult && i == meshDefinition.vertexGroups.Count)
				{
					this.evalResult = this.lastResult;
				}
				else
				{
					base.Evaluate(DMath.Lerp(from, to, meshDefinition.vertexGroups[i].percent), this.evalResult);
				}
				base.ModifySample(this.evalResult, this.modifiedResult);
				Vector3 up = this.modifiedResult.up;
				Vector3 right = this.modifiedResult.right;
				Vector3 forward = this.modifiedResult.forward;
				if (channel.overrideNormal)
				{
					this.modifiedResult.forward = Vector3.Cross(this.modifiedResult.right, channel.customNormal);
					this.modifiedResult.up = channel.customNormal;
				}
				Vector2 vector3 = channel.NextOffset(this.modifiedResult.percent);
				Vector3 a = channel.NextExtrudeScale(this.modifiedResult.percent);
				Vector2 scale = channel.scaleModifier.GetScale(this.modifiedResult);
				a.x *= scale.x;
				a.y *= scale.y;
				float num = channel.NextExtrudeRotation(this.modifiedResult.percent);
				this.vertexMatrix.SetTRS(this.modifiedResult.position + right * (base.offset.x + vector3.x) * this.modifiedResult.size + up * (base.offset.y + vector3.y) * this.modifiedResult.size + forward * base.offset.z, this.modifiedResult.rotation * Quaternion.AngleAxis(base.rotation + num, Vector3.forward), a * this.modifiedResult.size);
				this.normalMatrix = this.vertexMatrix.inverse.transpose;
				if (i == 0)
				{
					this.lastResult.CopyFrom(this.evalResult);
				}
				for (int j = 0; j < meshDefinition.vertexGroups[i].ids.Length; j++)
				{
					int num2 = meshDefinition.vertexGroups[i].ids[j];
					vector2 = meshDefinition.vertices[num2];
					vector2.z = 0f;
					target.vertices[num2] = this.vertexMatrix.MultiplyPoint3x4(vector2);
					vector2 = meshDefinition.normals[num2];
					target.normals[num2] = this.normalMatrix.MultiplyVector(vector2);
					target.colors[num2] = target.colors[num2] * this.modifiedResult.color;
					if (target.uv.Length > num2)
					{
						vector = target.uv[num2];
						switch (channel.overrideUVs)
						{
						case SplineMesh.Channel.UVOverride.ClampU:
							vector.x = (float)this.modifiedResult.percent;
							break;
						case SplineMesh.Channel.UVOverride.ClampV:
							vector.y = (float)this.modifiedResult.percent;
							break;
						case SplineMesh.Channel.UVOverride.UniformU:
							vector.x = base.CalculateLength(0.0, base.ClipPercent(this.modifiedResult.percent));
							break;
						case SplineMesh.Channel.UVOverride.UniformV:
							vector.y = base.CalculateLength(0.0, base.ClipPercent(this.modifiedResult.percent));
							break;
						}
						target.uv[num2] = new Vector2(vector.x * base.uvScale.x * channel.uvScale.x, vector.y * base.uvScale.y * channel.uvScale.y);
						target.uv[num2] += base.uvOffset + channel.uvOffset;
					}
				}
			}
		}

		// Token: 0x04002D14 RID: 11540
		[SerializeField]
		[HideInInspector]
		private List<SplineMesh.Channel> channels = new List<SplineMesh.Channel>();

		// Token: 0x04002D15 RID: 11541
		private bool useLastResult;

		// Token: 0x04002D16 RID: 11542
		private List<TS_Mesh> combineMeshes = new List<TS_Mesh>();

		// Token: 0x04002D17 RID: 11543
		private int meshCount;

		// Token: 0x04002D18 RID: 11544
		private Matrix4x4 vertexMatrix;

		// Token: 0x04002D19 RID: 11545
		private Matrix4x4 normalMatrix;

		// Token: 0x04002D1A RID: 11546
		private SplineSample lastResult = new SplineSample();

		// Token: 0x04002D1B RID: 11547
		private SplineSample modifiedResult = new SplineSample();

		// Token: 0x020009A0 RID: 2464
		[Serializable]
		public class Channel
		{
			// Token: 0x17000701 RID: 1793
			// (get) Token: 0x06005456 RID: 21590 RVA: 0x00246587 File Offset: 0x00244787
			// (set) Token: 0x06005457 RID: 21591 RVA: 0x0024658F File Offset: 0x0024478F
			public double clipFrom
			{
				get
				{
					return this._clipFrom;
				}
				set
				{
					if (value != this._clipFrom)
					{
						this._clipFrom = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000702 RID: 1794
			// (get) Token: 0x06005458 RID: 21592 RVA: 0x002465A7 File Offset: 0x002447A7
			// (set) Token: 0x06005459 RID: 21593 RVA: 0x002465AF File Offset: 0x002447AF
			public double clipTo
			{
				get
				{
					return this._clipTo;
				}
				set
				{
					if (value != this._clipTo)
					{
						this._clipTo = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000703 RID: 1795
			// (get) Token: 0x0600545A RID: 21594 RVA: 0x002465C7 File Offset: 0x002447C7
			// (set) Token: 0x0600545B RID: 21595 RVA: 0x002465CF File Offset: 0x002447CF
			public bool randomOffset
			{
				get
				{
					return this._randomOffset;
				}
				set
				{
					if (value != this._randomOffset)
					{
						this._randomOffset = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000704 RID: 1796
			// (get) Token: 0x0600545C RID: 21596 RVA: 0x002465E7 File Offset: 0x002447E7
			// (set) Token: 0x0600545D RID: 21597 RVA: 0x002465EF File Offset: 0x002447EF
			public SplineMesh.Channel.Vector2Handler offsetHandler
			{
				get
				{
					return this._offsetHandler;
				}
				set
				{
					if (value != this._offsetHandler)
					{
						this._offsetHandler = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000705 RID: 1797
			// (get) Token: 0x0600545E RID: 21598 RVA: 0x0024660C File Offset: 0x0024480C
			// (set) Token: 0x0600545F RID: 21599 RVA: 0x00246614 File Offset: 0x00244814
			public bool overrideMaterialID
			{
				get
				{
					return this._overrideMaterialID;
				}
				set
				{
					if (value != this._overrideMaterialID)
					{
						this._overrideMaterialID = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000706 RID: 1798
			// (get) Token: 0x06005460 RID: 21600 RVA: 0x0024662C File Offset: 0x0024482C
			// (set) Token: 0x06005461 RID: 21601 RVA: 0x00246634 File Offset: 0x00244834
			public int targetMaterialID
			{
				get
				{
					return this._targetMaterialID;
				}
				set
				{
					if (value != this._targetMaterialID)
					{
						this._targetMaterialID = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000707 RID: 1799
			// (get) Token: 0x06005462 RID: 21602 RVA: 0x0024664C File Offset: 0x0024484C
			// (set) Token: 0x06005463 RID: 21603 RVA: 0x00246654 File Offset: 0x00244854
			public bool randomRotation
			{
				get
				{
					return this._randomRotation;
				}
				set
				{
					if (value != this._randomRotation)
					{
						this._randomRotation = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000708 RID: 1800
			// (get) Token: 0x06005464 RID: 21604 RVA: 0x0024666C File Offset: 0x0024486C
			// (set) Token: 0x06005465 RID: 21605 RVA: 0x00246674 File Offset: 0x00244874
			public SplineMesh.Channel.QuaternionHandler placeRotationHandler
			{
				get
				{
					return this._placeRotationHandler;
				}
				set
				{
					if (value != this._placeRotationHandler)
					{
						this._placeRotationHandler = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000709 RID: 1801
			// (get) Token: 0x06005466 RID: 21606 RVA: 0x00246691 File Offset: 0x00244891
			// (set) Token: 0x06005467 RID: 21607 RVA: 0x00246699 File Offset: 0x00244899
			public SplineMesh.Channel.FloatHandler extrudeRotationHandler
			{
				get
				{
					return this._extrudeRotationHandler;
				}
				set
				{
					if (value != this._extrudeRotationHandler)
					{
						this._extrudeRotationHandler = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070A RID: 1802
			// (get) Token: 0x06005468 RID: 21608 RVA: 0x002466B6 File Offset: 0x002448B6
			// (set) Token: 0x06005469 RID: 21609 RVA: 0x002466BE File Offset: 0x002448BE
			public bool randomScale
			{
				get
				{
					return this._randomScale;
				}
				set
				{
					if (value != this._randomScale)
					{
						this._randomScale = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070B RID: 1803
			// (get) Token: 0x0600546A RID: 21610 RVA: 0x002466D6 File Offset: 0x002448D6
			// (set) Token: 0x0600546B RID: 21611 RVA: 0x002466DE File Offset: 0x002448DE
			public SplineMesh.Channel.Vector3Handler scaleHandler
			{
				get
				{
					return this._scaleHandler;
				}
				set
				{
					if (value != this._scaleHandler)
					{
						this._scaleHandler = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070C RID: 1804
			// (get) Token: 0x0600546C RID: 21612 RVA: 0x002466FB File Offset: 0x002448FB
			// (set) Token: 0x0600546D RID: 21613 RVA: 0x00246703 File Offset: 0x00244903
			public bool uniformRandomScale
			{
				get
				{
					return this._uniformRandomScale;
				}
				set
				{
					if (value != this._uniformRandomScale)
					{
						this._uniformRandomScale = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070D RID: 1805
			// (get) Token: 0x0600546E RID: 21614 RVA: 0x0024671B File Offset: 0x0024491B
			// (set) Token: 0x0600546F RID: 21615 RVA: 0x00246723 File Offset: 0x00244923
			public int offsetSeed
			{
				get
				{
					return this._offsetSeed;
				}
				set
				{
					if (value != this._offsetSeed)
					{
						this._offsetSeed = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070E RID: 1806
			// (get) Token: 0x06005470 RID: 21616 RVA: 0x0024673B File Offset: 0x0024493B
			// (set) Token: 0x06005471 RID: 21617 RVA: 0x00246743 File Offset: 0x00244943
			public int rotationSeed
			{
				get
				{
					return this._rotationSeed;
				}
				set
				{
					if (value != this._rotationSeed)
					{
						this._rotationSeed = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700070F RID: 1807
			// (get) Token: 0x06005472 RID: 21618 RVA: 0x0024675B File Offset: 0x0024495B
			// (set) Token: 0x06005473 RID: 21619 RVA: 0x00246763 File Offset: 0x00244963
			public int scaleSeed
			{
				get
				{
					return this._scaleSeed;
				}
				set
				{
					if (value != this._scaleSeed)
					{
						this._scaleSeed = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000710 RID: 1808
			// (get) Token: 0x06005474 RID: 21620 RVA: 0x0024677B File Offset: 0x0024497B
			// (set) Token: 0x06005475 RID: 21621 RVA: 0x00246783 File Offset: 0x00244983
			public double spacing
			{
				get
				{
					return this._spacing;
				}
				set
				{
					if (value != this._spacing)
					{
						this._spacing = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000711 RID: 1809
			// (get) Token: 0x06005476 RID: 21622 RVA: 0x0024679B File Offset: 0x0024499B
			// (set) Token: 0x06005477 RID: 21623 RVA: 0x002467A3 File Offset: 0x002449A3
			public Vector2 minOffset
			{
				get
				{
					return this._minOffset;
				}
				set
				{
					if (value != this._minOffset)
					{
						this._minOffset = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000712 RID: 1810
			// (get) Token: 0x06005478 RID: 21624 RVA: 0x002467C0 File Offset: 0x002449C0
			// (set) Token: 0x06005479 RID: 21625 RVA: 0x002467C8 File Offset: 0x002449C8
			public Vector2 maxOffset
			{
				get
				{
					return this._maxOffset;
				}
				set
				{
					if (value != this._maxOffset)
					{
						this._maxOffset = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000713 RID: 1811
			// (get) Token: 0x0600547A RID: 21626 RVA: 0x002467E5 File Offset: 0x002449E5
			// (set) Token: 0x0600547B RID: 21627 RVA: 0x002467ED File Offset: 0x002449ED
			public Vector3 minRotation
			{
				get
				{
					return this._minRotation;
				}
				set
				{
					if (value != this._minRotation)
					{
						this._minRotation = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000714 RID: 1812
			// (get) Token: 0x0600547C RID: 21628 RVA: 0x0024680A File Offset: 0x00244A0A
			// (set) Token: 0x0600547D RID: 21629 RVA: 0x00246812 File Offset: 0x00244A12
			public Vector3 maxRotation
			{
				get
				{
					return this._maxRotation;
				}
				set
				{
					if (value != this._maxRotation)
					{
						this._maxRotation = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000715 RID: 1813
			// (get) Token: 0x0600547E RID: 21630 RVA: 0x0024682F File Offset: 0x00244A2F
			// (set) Token: 0x0600547F RID: 21631 RVA: 0x00246837 File Offset: 0x00244A37
			public Vector3 minScale
			{
				get
				{
					return this._minScale;
				}
				set
				{
					if (value != this._minScale)
					{
						this._minScale = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000716 RID: 1814
			// (get) Token: 0x06005480 RID: 21632 RVA: 0x00246854 File Offset: 0x00244A54
			// (set) Token: 0x06005481 RID: 21633 RVA: 0x0024685C File Offset: 0x00244A5C
			public Vector3 maxScale
			{
				get
				{
					return this._maxScale;
				}
				set
				{
					if (value != this._maxScale)
					{
						this._maxScale = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000717 RID: 1815
			// (get) Token: 0x06005482 RID: 21634 RVA: 0x00246879 File Offset: 0x00244A79
			// (set) Token: 0x06005483 RID: 21635 RVA: 0x00246881 File Offset: 0x00244A81
			public SplineMesh.Channel.Type type
			{
				get
				{
					return this._type;
				}
				set
				{
					if (value != this._type)
					{
						this._type = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000718 RID: 1816
			// (get) Token: 0x06005484 RID: 21636 RVA: 0x00246899 File Offset: 0x00244A99
			// (set) Token: 0x06005485 RID: 21637 RVA: 0x002468A1 File Offset: 0x00244AA1
			public bool randomOrder
			{
				get
				{
					return this._randomOrder;
				}
				set
				{
					if (value != this._randomOrder)
					{
						this._randomOrder = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000719 RID: 1817
			// (get) Token: 0x06005486 RID: 21638 RVA: 0x002468B9 File Offset: 0x00244AB9
			// (set) Token: 0x06005487 RID: 21639 RVA: 0x002468C1 File Offset: 0x00244AC1
			public int randomSeed
			{
				get
				{
					return this._iterationSeed;
				}
				set
				{
					if (value != this._iterationSeed)
					{
						this._iterationSeed = value;
						if (this._randomOrder)
						{
							this.Rebuild();
						}
					}
				}
			}

			// Token: 0x1700071A RID: 1818
			// (get) Token: 0x06005488 RID: 21640 RVA: 0x002468E1 File Offset: 0x00244AE1
			// (set) Token: 0x06005489 RID: 21641 RVA: 0x002468E9 File Offset: 0x00244AE9
			public int count
			{
				get
				{
					return this._count;
				}
				set
				{
					if (value != this._count)
					{
						this._count = value;
						if (this._count < 1)
						{
							this._count = 1;
						}
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700071B RID: 1819
			// (get) Token: 0x0600548A RID: 21642 RVA: 0x00246911 File Offset: 0x00244B11
			// (set) Token: 0x0600548B RID: 21643 RVA: 0x00246919 File Offset: 0x00244B19
			public bool autoCount
			{
				get
				{
					return this._autoCount;
				}
				set
				{
					if (value != this._autoCount)
					{
						this._autoCount = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700071C RID: 1820
			// (get) Token: 0x0600548C RID: 21644 RVA: 0x00246931 File Offset: 0x00244B31
			// (set) Token: 0x0600548D RID: 21645 RVA: 0x00246939 File Offset: 0x00244B39
			public SplineMesh.Channel.UVOverride overrideUVs
			{
				get
				{
					return this._overrideUVs;
				}
				set
				{
					if (value != this._overrideUVs)
					{
						this._overrideUVs = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700071D RID: 1821
			// (get) Token: 0x0600548E RID: 21646 RVA: 0x00246951 File Offset: 0x00244B51
			// (set) Token: 0x0600548F RID: 21647 RVA: 0x00246959 File Offset: 0x00244B59
			public Vector2 uvOffset
			{
				get
				{
					return this._uvOffset;
				}
				set
				{
					if (value != this._uvOffset)
					{
						this._uvOffset = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700071E RID: 1822
			// (get) Token: 0x06005490 RID: 21648 RVA: 0x00246976 File Offset: 0x00244B76
			// (set) Token: 0x06005491 RID: 21649 RVA: 0x0024697E File Offset: 0x00244B7E
			public Vector2 uvScale
			{
				get
				{
					return this._uvScale;
				}
				set
				{
					if (value != this._uvScale)
					{
						this._uvScale = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x1700071F RID: 1823
			// (get) Token: 0x06005492 RID: 21650 RVA: 0x0024699B File Offset: 0x00244B9B
			// (set) Token: 0x06005493 RID: 21651 RVA: 0x002469A3 File Offset: 0x00244BA3
			public bool overrideNormal
			{
				get
				{
					return this._overrideNormal;
				}
				set
				{
					if (value != this._overrideNormal)
					{
						this._overrideNormal = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000720 RID: 1824
			// (get) Token: 0x06005494 RID: 21652 RVA: 0x002469BB File Offset: 0x00244BBB
			// (set) Token: 0x06005495 RID: 21653 RVA: 0x002469C3 File Offset: 0x00244BC3
			public Vector3 customNormal
			{
				get
				{
					return this._customNormal;
				}
				set
				{
					if (value != this._customNormal)
					{
						this._customNormal = value;
						this.Rebuild();
					}
				}
			}

			// Token: 0x17000721 RID: 1825
			// (get) Token: 0x06005496 RID: 21654 RVA: 0x002469E0 File Offset: 0x00244BE0
			public MeshScaleModifier scaleModifier
			{
				get
				{
					return this._scaleModifier;
				}
			}

			// Token: 0x06005497 RID: 21655 RVA: 0x002469E8 File Offset: 0x00244BE8
			public Channel(string n, SplineMesh parent)
			{
				this.name = n;
				this.owner = parent;
				this.Init();
			}

			// Token: 0x06005498 RID: 21656 RVA: 0x00246AAC File Offset: 0x00244CAC
			public Channel(string n, Mesh inputMesh, SplineMesh parent)
			{
				this.name = n;
				this.owner = parent;
				this.meshes.Add(new SplineMesh.Channel.MeshDefinition(inputMesh));
				this.Init();
				this.Rebuild();
			}

			// Token: 0x06005499 RID: 21657 RVA: 0x00246B84 File Offset: 0x00244D84
			private void Init()
			{
				this._minScale = (this._maxScale = Vector3.one);
				this._minOffset = (this._maxOffset = Vector3.zero);
				this._minRotation = (this._maxRotation = Vector3.zero);
			}

			// Token: 0x0600549A RID: 21658 RVA: 0x00246BD4 File Offset: 0x00244DD4
			public void CopyTo(SplineMesh.Channel target)
			{
				target.meshes.Clear();
				for (int i = 0; i < this.meshes.Count; i++)
				{
					target.meshes.Add(this.meshes[i].Copy());
				}
				target._clipFrom = this._clipFrom;
				target._clipTo = this._clipTo;
				target._customNormal = this._customNormal;
				target._iterationSeed = this._iterationSeed;
				target._minOffset = this._minOffset;
				target._minRotation = this._minRotation;
				target._minScale = this._minScale;
				target._maxOffset = this._maxOffset;
				target._maxRotation = this._maxRotation;
				target._maxScale = this._maxScale;
				target._randomOffset = this._randomOffset;
				target._randomRotation = this._randomRotation;
				target._randomScale = this._randomScale;
				target._offsetSeed = this._offsetSeed;
				target._offsetHandler = this._offsetHandler;
				target._rotationSeed = this._rotationSeed;
				target._placeRotationHandler = this._placeRotationHandler;
				target._extrudeRotationHandler = this._extrudeRotationHandler;
				target._scaleSeed = this._scaleSeed;
				target._scaleHandler = this._scaleHandler;
				target._iterationSeed = this._iterationSeed;
				target._count = this._count;
				target._spacing = this._spacing;
				target._overrideUVs = this._overrideUVs;
				target._type = this._type;
				target._overrideMaterialID = this._overrideMaterialID;
				target._targetMaterialID = this._targetMaterialID;
				target._overrideNormal = this._overrideNormal;
			}

			// Token: 0x0600549B RID: 21659 RVA: 0x00246D6E File Offset: 0x00244F6E
			public int GetMeshCount()
			{
				return this.meshes.Count;
			}

			// Token: 0x0600549C RID: 21660 RVA: 0x00246D7C File Offset: 0x00244F7C
			public void SwapMeshes(int a, int b)
			{
				if (a < 0 || a >= this.meshes.Count || b < 0 || b >= this.meshes.Count)
				{
					return;
				}
				SplineMesh.Channel.MeshDefinition value = this.meshes[b];
				this.meshes[b] = this.meshes[a];
				this.meshes[a] = value;
				this.Rebuild();
			}

			// Token: 0x0600549D RID: 21661 RVA: 0x00246DE6 File Offset: 0x00244FE6
			public void DuplicateMesh(int index)
			{
				if (index < 0 || index >= this.meshes.Count)
				{
					return;
				}
				this.meshes.Add(this.meshes[index].Copy());
				this.Rebuild();
			}

			// Token: 0x0600549E RID: 21662 RVA: 0x00246E1D File Offset: 0x0024501D
			public SplineMesh.Channel.MeshDefinition GetMesh(int index)
			{
				return this.meshes[index];
			}

			// Token: 0x0600549F RID: 21663 RVA: 0x00246E2B File Offset: 0x0024502B
			public void AddMesh(Mesh input)
			{
				this.meshes.Add(new SplineMesh.Channel.MeshDefinition(input));
				this.Rebuild();
			}

			// Token: 0x060054A0 RID: 21664 RVA: 0x00246E44 File Offset: 0x00245044
			public void RemoveMesh(int index)
			{
				this.meshes.RemoveAt(index);
				this.Rebuild();
			}

			// Token: 0x060054A1 RID: 21665 RVA: 0x00246E58 File Offset: 0x00245058
			public void ResetIteration()
			{
				if (this._randomOrder)
				{
					this.iterationRandom = new Random(this._iterationSeed);
				}
				if (this._randomOffset)
				{
					this.offsetRandom = new Random(this._offsetSeed);
				}
				if (this._randomRotation)
				{
					this.rotationRandom = new Random(this._rotationSeed);
				}
				if (this._randomScale)
				{
					this.scaleRandom = new Random(this._scaleSeed);
				}
				this.iterator = 0;
			}

			// Token: 0x060054A2 RID: 21666 RVA: 0x00246ED0 File Offset: 0x002450D0
			public Vector2 NextOffset(double percent)
			{
				if (this._offsetHandler != null)
				{
					return this._offsetHandler(percent);
				}
				if (this._randomOffset)
				{
					return new Vector2(Mathf.Lerp(this._minOffset.x, this._maxOffset.x, (float)this.offsetRandom.NextDouble()), Mathf.Lerp(this._minOffset.y, this._maxOffset.y, (float)this.offsetRandom.NextDouble()));
				}
				return this._minOffset;
			}

			// Token: 0x060054A3 RID: 21667 RVA: 0x00246F54 File Offset: 0x00245154
			public Quaternion NextPlaceRotation(double percent)
			{
				if (this._placeRotationHandler != null)
				{
					return this._placeRotationHandler(percent);
				}
				if (this._randomRotation)
				{
					return Quaternion.Euler(new Vector3(Mathf.Lerp(this._minRotation.x, this._maxRotation.x, (float)this.rotationRandom.NextDouble()), Mathf.Lerp(this._minRotation.y, this._maxRotation.y, (float)this.rotationRandom.NextDouble()), Mathf.Lerp(this._minRotation.z, this._maxRotation.z, (float)this.rotationRandom.NextDouble())));
				}
				return Quaternion.Euler(this._minRotation);
			}

			// Token: 0x060054A4 RID: 21668 RVA: 0x0024700C File Offset: 0x0024520C
			public float NextExtrudeRotation(double percent)
			{
				if (this._extrudeRotationHandler != null)
				{
					return this._extrudeRotationHandler(percent);
				}
				if (this._randomRotation)
				{
					return Mathf.Lerp(this._minRotation.z, this._maxRotation.z, (float)this.rotationRandom.NextDouble());
				}
				return this._minRotation.z;
			}

			// Token: 0x060054A5 RID: 21669 RVA: 0x0024706C File Offset: 0x0024526C
			public Vector3 NextExtrudeScale(double percent)
			{
				if (this._scaleHandler != null)
				{
					return this._scaleHandler(percent);
				}
				if (!this._randomScale)
				{
					return new Vector3(this._minScale.x, this._minScale.y, 1f);
				}
				if (this._uniformRandomScale)
				{
					return Vector3.Lerp(new Vector3(this._minScale.x, this._minScale.y, 1f), new Vector3(this._maxScale.x, this._maxScale.y, 1f), (float)this.scaleRandom.NextDouble());
				}
				return new Vector3(Mathf.Lerp(this._minScale.x, this._maxScale.x, (float)this.scaleRandom.NextDouble()), Mathf.Lerp(this._minScale.y, this._maxScale.y, (float)this.scaleRandom.NextDouble()), 1f);
			}

			// Token: 0x060054A6 RID: 21670 RVA: 0x0024716C File Offset: 0x0024536C
			public Vector3 NextPlaceScale()
			{
				if (!this._randomScale)
				{
					return this._minScale;
				}
				if (this._uniformRandomScale)
				{
					return Vector3.Lerp(this._minScale, this._maxScale, (float)this.scaleRandom.NextDouble());
				}
				return new Vector3(Mathf.Lerp(this._minScale.x, this._maxScale.x, (float)this.scaleRandom.NextDouble()), Mathf.Lerp(this._minScale.y, this._maxScale.y, (float)this.scaleRandom.NextDouble()), Mathf.Lerp(this._minScale.z, this._maxScale.z, (float)this.scaleRandom.NextDouble()));
			}

			// Token: 0x060054A7 RID: 21671 RVA: 0x0024722C File Offset: 0x0024542C
			public SplineMesh.Channel.MeshDefinition NextMesh()
			{
				if (this._randomOrder)
				{
					return this.meshes[this.iterationRandom.Next(this.meshes.Count)];
				}
				if (this.iterator >= this.meshes.Count)
				{
					this.iterator = 0;
				}
				List<SplineMesh.Channel.MeshDefinition> list = this.meshes;
				int num = this.iterator;
				this.iterator = num + 1;
				return list[num];
			}

			// Token: 0x060054A8 RID: 21672 RVA: 0x00247299 File Offset: 0x00245499
			internal void Rebuild()
			{
				if (this.owner != null)
				{
					this.owner.Rebuild();
				}
			}

			// Token: 0x060054A9 RID: 21673 RVA: 0x002472B4 File Offset: 0x002454B4
			private void Refresh()
			{
				for (int i = 0; i < this.meshes.Count; i++)
				{
					this.meshes[i].Refresh();
				}
				this.Rebuild();
			}

			// Token: 0x040044C1 RID: 17601
			public string name = "Channel";

			// Token: 0x040044C2 RID: 17602
			private Random iterationRandom;

			// Token: 0x040044C3 RID: 17603
			[SerializeField]
			[HideInInspector]
			private int _iterationSeed;

			// Token: 0x040044C4 RID: 17604
			[SerializeField]
			[HideInInspector]
			private int _offsetSeed;

			// Token: 0x040044C5 RID: 17605
			private Random offsetRandom;

			// Token: 0x040044C6 RID: 17606
			private SplineMesh.Channel.Vector2Handler _offsetHandler;

			// Token: 0x040044C7 RID: 17607
			[SerializeField]
			[HideInInspector]
			private int _rotationSeed;

			// Token: 0x040044C8 RID: 17608
			private Random rotationRandom;

			// Token: 0x040044C9 RID: 17609
			private SplineMesh.Channel.QuaternionHandler _placeRotationHandler;

			// Token: 0x040044CA RID: 17610
			private SplineMesh.Channel.FloatHandler _extrudeRotationHandler;

			// Token: 0x040044CB RID: 17611
			[SerializeField]
			[HideInInspector]
			private int _scaleSeed;

			// Token: 0x040044CC RID: 17612
			private Random scaleRandom;

			// Token: 0x040044CD RID: 17613
			private SplineMesh.Channel.Vector3Handler _scaleHandler;

			// Token: 0x040044CE RID: 17614
			[SerializeField]
			internal SplineMesh owner;

			// Token: 0x040044CF RID: 17615
			[SerializeField]
			[HideInInspector]
			private List<SplineMesh.Channel.MeshDefinition> meshes = new List<SplineMesh.Channel.MeshDefinition>();

			// Token: 0x040044D0 RID: 17616
			[SerializeField]
			[HideInInspector]
			private double _clipFrom;

			// Token: 0x040044D1 RID: 17617
			[SerializeField]
			[HideInInspector]
			private double _clipTo = 1.0;

			// Token: 0x040044D2 RID: 17618
			[SerializeField]
			[HideInInspector]
			private bool _randomOrder;

			// Token: 0x040044D3 RID: 17619
			[SerializeField]
			[HideInInspector]
			private SplineMesh.Channel.UVOverride _overrideUVs;

			// Token: 0x040044D4 RID: 17620
			[SerializeField]
			[HideInInspector]
			private Vector2 _uvScale = Vector2.one;

			// Token: 0x040044D5 RID: 17621
			[SerializeField]
			[HideInInspector]
			private Vector2 _uvOffset = Vector2.zero;

			// Token: 0x040044D6 RID: 17622
			[SerializeField]
			[HideInInspector]
			private bool _overrideNormal;

			// Token: 0x040044D7 RID: 17623
			[SerializeField]
			[HideInInspector]
			private Vector3 _customNormal = Vector3.up;

			// Token: 0x040044D8 RID: 17624
			[SerializeField]
			[HideInInspector]
			private SplineMesh.Channel.Type _type;

			// Token: 0x040044D9 RID: 17625
			[SerializeField]
			[HideInInspector]
			private int _count = 1;

			// Token: 0x040044DA RID: 17626
			[SerializeField]
			[HideInInspector]
			private bool _autoCount;

			// Token: 0x040044DB RID: 17627
			[SerializeField]
			[HideInInspector]
			private double _spacing;

			// Token: 0x040044DC RID: 17628
			[SerializeField]
			[HideInInspector]
			private bool _randomRotation;

			// Token: 0x040044DD RID: 17629
			[SerializeField]
			[HideInInspector]
			private Vector3 _minRotation = Vector3.zero;

			// Token: 0x040044DE RID: 17630
			[SerializeField]
			[HideInInspector]
			private Vector3 _maxRotation = Vector3.zero;

			// Token: 0x040044DF RID: 17631
			[SerializeField]
			[HideInInspector]
			private bool _randomOffset;

			// Token: 0x040044E0 RID: 17632
			[SerializeField]
			[HideInInspector]
			private Vector2 _minOffset = Vector2.one;

			// Token: 0x040044E1 RID: 17633
			[SerializeField]
			[HideInInspector]
			private Vector2 _maxOffset = Vector2.one;

			// Token: 0x040044E2 RID: 17634
			[SerializeField]
			[HideInInspector]
			private bool _randomScale;

			// Token: 0x040044E3 RID: 17635
			[SerializeField]
			[HideInInspector]
			private bool _uniformRandomScale;

			// Token: 0x040044E4 RID: 17636
			[SerializeField]
			[HideInInspector]
			private Vector3 _minScale = Vector3.one;

			// Token: 0x040044E5 RID: 17637
			[SerializeField]
			[HideInInspector]
			private Vector3 _maxScale = Vector3.one;

			// Token: 0x040044E6 RID: 17638
			private int iterator;

			// Token: 0x040044E7 RID: 17639
			[SerializeField]
			[HideInInspector]
			private bool _overrideMaterialID;

			// Token: 0x040044E8 RID: 17640
			[SerializeField]
			[HideInInspector]
			private int _targetMaterialID;

			// Token: 0x040044E9 RID: 17641
			[SerializeField]
			[HideInInspector]
			protected MeshScaleModifier _scaleModifier = new MeshScaleModifier();

			// Token: 0x02000A43 RID: 2627
			// (Invoke) Token: 0x060055F7 RID: 22007
			public delegate float FloatHandler(double percent);

			// Token: 0x02000A44 RID: 2628
			// (Invoke) Token: 0x060055FB RID: 22011
			public delegate Vector2 Vector2Handler(double percent);

			// Token: 0x02000A45 RID: 2629
			// (Invoke) Token: 0x060055FF RID: 22015
			public delegate Vector3 Vector3Handler(double percent);

			// Token: 0x02000A46 RID: 2630
			// (Invoke) Token: 0x06005603 RID: 22019
			public delegate Quaternion QuaternionHandler(double percent);

			// Token: 0x02000A47 RID: 2631
			public enum Type
			{
				// Token: 0x04004722 RID: 18210
				Extrude,
				// Token: 0x04004723 RID: 18211
				Place
			}

			// Token: 0x02000A48 RID: 2632
			public enum UVOverride
			{
				// Token: 0x04004725 RID: 18213
				None,
				// Token: 0x04004726 RID: 18214
				ClampU,
				// Token: 0x04004727 RID: 18215
				ClampV,
				// Token: 0x04004728 RID: 18216
				UniformU,
				// Token: 0x04004729 RID: 18217
				UniformV
			}

			// Token: 0x02000A49 RID: 2633
			[Serializable]
			public class MeshDefinition
			{
				// Token: 0x1700074E RID: 1870
				// (get) Token: 0x06005606 RID: 22022 RVA: 0x0024C217 File Offset: 0x0024A417
				// (set) Token: 0x06005607 RID: 22023 RVA: 0x0024C21F File Offset: 0x0024A41F
				public Mesh mesh
				{
					get
					{
						return this._mesh;
					}
					set
					{
						if (this._mesh != value)
						{
							this._mesh = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x1700074F RID: 1871
				// (get) Token: 0x06005608 RID: 22024 RVA: 0x0024C23C File Offset: 0x0024A43C
				// (set) Token: 0x06005609 RID: 22025 RVA: 0x0024C244 File Offset: 0x0024A444
				public Vector3 rotation
				{
					get
					{
						return this._rotation;
					}
					set
					{
						if (this.rotation != value)
						{
							this._rotation = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000750 RID: 1872
				// (get) Token: 0x0600560A RID: 22026 RVA: 0x0024C261 File Offset: 0x0024A461
				// (set) Token: 0x0600560B RID: 22027 RVA: 0x0024C269 File Offset: 0x0024A469
				public Vector3 offset
				{
					get
					{
						return this._offset;
					}
					set
					{
						if (this._offset != value)
						{
							this._offset = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000751 RID: 1873
				// (get) Token: 0x0600560C RID: 22028 RVA: 0x0024C286 File Offset: 0x0024A486
				// (set) Token: 0x0600560D RID: 22029 RVA: 0x0024C28E File Offset: 0x0024A48E
				public Vector3 scale
				{
					get
					{
						return this._scale;
					}
					set
					{
						if (this._scale != value)
						{
							this._scale = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000752 RID: 1874
				// (get) Token: 0x0600560E RID: 22030 RVA: 0x0024C2AB File Offset: 0x0024A4AB
				// (set) Token: 0x0600560F RID: 22031 RVA: 0x0024C2B3 File Offset: 0x0024A4B3
				public Vector2 uvScale
				{
					get
					{
						return this._uvScale;
					}
					set
					{
						if (this._uvScale != value)
						{
							this._uvScale = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000753 RID: 1875
				// (get) Token: 0x06005610 RID: 22032 RVA: 0x0024C2D0 File Offset: 0x0024A4D0
				// (set) Token: 0x06005611 RID: 22033 RVA: 0x0024C2D8 File Offset: 0x0024A4D8
				public Vector2 uvOffset
				{
					get
					{
						return this._uvOffset;
					}
					set
					{
						if (this._uvOffset != value)
						{
							this._uvOffset = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000754 RID: 1876
				// (get) Token: 0x06005612 RID: 22034 RVA: 0x0024C2F5 File Offset: 0x0024A4F5
				// (set) Token: 0x06005613 RID: 22035 RVA: 0x0024C2FD File Offset: 0x0024A4FD
				public float uvRotation
				{
					get
					{
						return this._uvRotation;
					}
					set
					{
						if (this._uvRotation != value)
						{
							this._uvRotation = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000755 RID: 1877
				// (get) Token: 0x06005614 RID: 22036 RVA: 0x0024C315 File Offset: 0x0024A515
				// (set) Token: 0x06005615 RID: 22037 RVA: 0x0024C31D File Offset: 0x0024A51D
				public float vertexGroupingMargin
				{
					get
					{
						return this._vertexGroupingMargin;
					}
					set
					{
						if (this._vertexGroupingMargin != value)
						{
							this._vertexGroupingMargin = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000756 RID: 1878
				// (get) Token: 0x06005616 RID: 22038 RVA: 0x0024C335 File Offset: 0x0024A535
				// (set) Token: 0x06005617 RID: 22039 RVA: 0x0024C33D File Offset: 0x0024A53D
				public SplineMesh.Channel.MeshDefinition.MirrorMethod mirror
				{
					get
					{
						return this._mirror;
					}
					set
					{
						if (this._mirror != value)
						{
							this._mirror = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000757 RID: 1879
				// (get) Token: 0x06005618 RID: 22040 RVA: 0x0024C355 File Offset: 0x0024A555
				// (set) Token: 0x06005619 RID: 22041 RVA: 0x0024C35D File Offset: 0x0024A55D
				public bool removeInnerFaces
				{
					get
					{
						return this._removeInnerFaces;
					}
					set
					{
						if (this._removeInnerFaces != value)
						{
							this._removeInnerFaces = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000758 RID: 1880
				// (get) Token: 0x0600561A RID: 22042 RVA: 0x0024C375 File Offset: 0x0024A575
				// (set) Token: 0x0600561B RID: 22043 RVA: 0x0024C37D File Offset: 0x0024A57D
				public bool flipFaces
				{
					get
					{
						return this._flipFaces;
					}
					set
					{
						if (this._flipFaces != value)
						{
							this._flipFaces = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x17000759 RID: 1881
				// (get) Token: 0x0600561C RID: 22044 RVA: 0x0024C395 File Offset: 0x0024A595
				// (set) Token: 0x0600561D RID: 22045 RVA: 0x0024C39D File Offset: 0x0024A59D
				public bool doubleSided
				{
					get
					{
						return this._doubleSided;
					}
					set
					{
						if (this._doubleSided != value)
						{
							this._doubleSided = value;
							this.Refresh();
						}
					}
				}

				// Token: 0x0600561E RID: 22046 RVA: 0x0024C3B8 File Offset: 0x0024A5B8
				internal SplineMesh.Channel.MeshDefinition Copy()
				{
					SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(this._mesh);
					meshDefinition.vertices = new Vector3[this.vertices.Length];
					meshDefinition.normals = new Vector3[this.normals.Length];
					meshDefinition.colors = new Color[this.colors.Length];
					meshDefinition.tangents = new Vector4[this.tangents.Length];
					meshDefinition.uv = new Vector2[this.uv.Length];
					meshDefinition.uv2 = new Vector2[this.uv2.Length];
					meshDefinition.uv3 = new Vector2[this.uv3.Length];
					meshDefinition.uv4 = new Vector2[this.uv4.Length];
					meshDefinition.triangles = new int[this.triangles.Length];
					this.vertices.CopyTo(meshDefinition.vertices, 0);
					this.normals.CopyTo(meshDefinition.normals, 0);
					this.colors.CopyTo(meshDefinition.colors, 0);
					this.tangents.CopyTo(meshDefinition.tangents, 0);
					this.uv.CopyTo(meshDefinition.uv, 0);
					this.uv2.CopyTo(meshDefinition.uv2, 0);
					this.uv3.CopyTo(meshDefinition.uv3, 0);
					this.uv4.CopyTo(meshDefinition.uv4, 0);
					this.triangles.CopyTo(meshDefinition.triangles, 0);
					meshDefinition.bounds = new TS_Bounds(this.bounds.min, this.bounds.max);
					meshDefinition.subMeshes = new List<SplineMesh.Channel.MeshDefinition.Submesh>();
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						meshDefinition.subMeshes.Add(new SplineMesh.Channel.MeshDefinition.Submesh(new int[this.subMeshes[i].triangles.Length]));
						this.subMeshes[i].triangles.CopyTo(meshDefinition.subMeshes[meshDefinition.subMeshes.Count - 1].triangles, 0);
					}
					meshDefinition._mirror = this._mirror;
					meshDefinition._offset = this._offset;
					meshDefinition._rotation = this._rotation;
					meshDefinition._scale = this._scale;
					meshDefinition._uvOffset = this._uvOffset;
					meshDefinition._uvScale = this._uvScale;
					meshDefinition._uvRotation = this._uvRotation;
					meshDefinition._flipFaces = this._flipFaces;
					meshDefinition._doubleSided = this._doubleSided;
					return meshDefinition;
				}

				// Token: 0x0600561F RID: 22047 RVA: 0x0024C62C File Offset: 0x0024A82C
				public MeshDefinition(Mesh input)
				{
					this._mesh = input;
					this.Refresh();
				}

				// Token: 0x06005620 RID: 22048 RVA: 0x0024C71C File Offset: 0x0024A91C
				public void Refresh()
				{
					if (this._mesh == null)
					{
						this.vertices = new Vector3[0];
						this.normals = new Vector3[0];
						this.colors = new Color[0];
						this.uv = new Vector2[0];
						this.uv2 = new Vector2[0];
						this.uv3 = new Vector2[0];
						this.uv4 = new Vector2[0];
						this.tangents = new Vector4[0];
						this.triangles = new int[0];
						this.subMeshes = new List<SplineMesh.Channel.MeshDefinition.Submesh>();
						this.vertexGroups = new List<SplineMesh.Channel.MeshDefinition.VertexGroup>();
						return;
					}
					if (this.vertices.Length != this._mesh.vertexCount)
					{
						this.vertices = new Vector3[this._mesh.vertexCount];
					}
					if (this.normals.Length != this._mesh.normals.Length)
					{
						this.normals = new Vector3[this._mesh.normals.Length];
					}
					if (this.colors.Length != this._mesh.colors.Length)
					{
						this.colors = new Color[this._mesh.colors.Length];
					}
					if (this.uv.Length != this._mesh.uv.Length)
					{
						this.uv = new Vector2[this._mesh.uv.Length];
					}
					if (this.uv2.Length != this._mesh.uv2.Length)
					{
						this.uv2 = new Vector2[this._mesh.uv2.Length];
					}
					if (this.uv3.Length != this._mesh.uv3.Length)
					{
						this.uv3 = new Vector2[this._mesh.uv3.Length];
					}
					if (this.uv4.Length != this._mesh.uv4.Length)
					{
						this.uv4 = new Vector2[this._mesh.uv4.Length];
					}
					if (this.tangents.Length != this._mesh.tangents.Length)
					{
						this.tangents = new Vector4[this._mesh.tangents.Length];
					}
					if (this.triangles.Length != this._mesh.triangles.Length)
					{
						this.triangles = new int[this._mesh.triangles.Length];
					}
					this.vertices = this._mesh.vertices;
					this.normals = this._mesh.normals;
					this.colors = this._mesh.colors;
					this.uv = this._mesh.uv;
					this.uv2 = this._mesh.uv2;
					this.uv3 = this._mesh.uv3;
					this.uv4 = this._mesh.uv4;
					this.tangents = this._mesh.tangents;
					this.triangles = this._mesh.triangles;
					this.colors = this._mesh.colors;
					while (this.subMeshes.Count > this._mesh.subMeshCount)
					{
						this.subMeshes.RemoveAt(0);
					}
					while (this.subMeshes.Count < this._mesh.subMeshCount)
					{
						this.subMeshes.Add(new SplineMesh.Channel.MeshDefinition.Submesh(new int[0]));
					}
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						this.subMeshes[i].triangles = this._mesh.GetTriangles(i);
					}
					if (this.colors.Length != this.vertices.Length)
					{
						this.colors = new Color[this.vertices.Length];
						for (int j = 0; j < this.colors.Length; j++)
						{
							this.colors[j] = Color.white;
						}
					}
					this.Mirror();
					if (this._doubleSided)
					{
						this.DoubleSided();
					}
					else if (this._flipFaces)
					{
						this.FlipFaces();
					}
					this.TransformVertices();
					this.CalculateBounds();
					if (this._removeInnerFaces)
					{
						this.RemoveInnerFaces();
					}
					this.GroupVertices();
				}

				// Token: 0x06005621 RID: 22049 RVA: 0x0024CB20 File Offset: 0x0024AD20
				private void RemoveInnerFaces()
				{
					float num = float.MaxValue;
					float num2 = 0f;
					for (int i = 0; i < this.vertices.Length; i++)
					{
						if (this.vertices[i].z < num)
						{
							num = this.vertices[i].z;
						}
						if (this.vertices[i].z > num2)
						{
							num2 = this.vertices[i].z;
						}
					}
					for (int j = 0; j < this.subMeshes.Count; j++)
					{
						List<int> list = new List<int>();
						for (int k = 0; k < this.subMeshes[j].triangles.Length; k += 3)
						{
							bool flag = true;
							bool flag2 = true;
							for (int l = k; l < k + 3; l++)
							{
								int num3 = this.subMeshes[j].triangles[l];
								if (!Mathf.Approximately(this.vertices[num3].z, num2))
								{
									flag = false;
								}
								if (!Mathf.Approximately(this.vertices[num3].z, num))
								{
									flag2 = false;
								}
							}
							if (!flag && !flag2)
							{
								list.Add(this.subMeshes[j].triangles[k]);
								list.Add(this.subMeshes[j].triangles[k + 1]);
								list.Add(this.subMeshes[j].triangles[k + 2]);
							}
						}
						this.subMeshes[j].triangles = list.ToArray();
					}
				}

				// Token: 0x06005622 RID: 22050 RVA: 0x0024CCC4 File Offset: 0x0024AEC4
				private void FlipFaces()
				{
					TS_Mesh ts_Mesh = new TS_Mesh();
					ts_Mesh.normals = this.normals;
					ts_Mesh.tangents = this.tangents;
					ts_Mesh.triangles = this.triangles;
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						ts_Mesh.subMeshes.Add(this.subMeshes[i].triangles);
					}
					MeshUtility.FlipFaces(ts_Mesh);
				}

				// Token: 0x06005623 RID: 22051 RVA: 0x0024CD34 File Offset: 0x0024AF34
				private void DoubleSided()
				{
					TS_Mesh ts_Mesh = new TS_Mesh();
					ts_Mesh.vertices = this.vertices;
					ts_Mesh.normals = this.normals;
					ts_Mesh.tangents = this.tangents;
					ts_Mesh.colors = this.colors;
					ts_Mesh.uv = this.uv;
					ts_Mesh.uv2 = this.uv2;
					ts_Mesh.uv3 = this.uv3;
					ts_Mesh.uv4 = this.uv4;
					ts_Mesh.triangles = this.triangles;
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						ts_Mesh.subMeshes.Add(this.subMeshes[i].triangles);
					}
					MeshUtility.MakeDoublesided(ts_Mesh);
					this.vertices = ts_Mesh.vertices;
					this.normals = ts_Mesh.normals;
					this.tangents = ts_Mesh.tangents;
					this.colors = ts_Mesh.colors;
					this.uv = ts_Mesh.uv;
					this.uv2 = ts_Mesh.uv2;
					this.uv3 = ts_Mesh.uv3;
					this.uv4 = ts_Mesh.uv4;
					this.triangles = ts_Mesh.triangles;
					for (int j = 0; j < this.subMeshes.Count; j++)
					{
						this.subMeshes[j].triangles = ts_Mesh.subMeshes[j];
					}
				}

				// Token: 0x06005624 RID: 22052 RVA: 0x0024CE8C File Offset: 0x0024B08C
				public void Write(TS_Mesh target, int forceMaterialId = -1)
				{
					if (target.vertices.Length != this.vertices.Length)
					{
						target.vertices = new Vector3[this.vertices.Length];
					}
					if (target.normals.Length != this.normals.Length)
					{
						target.normals = new Vector3[this.normals.Length];
					}
					if (target.colors.Length != this.colors.Length)
					{
						target.colors = new Color[this.colors.Length];
					}
					if (target.uv.Length != this.uv.Length)
					{
						target.uv = new Vector2[this.uv.Length];
					}
					if (target.uv2.Length != this.uv2.Length)
					{
						target.uv2 = new Vector2[this.uv2.Length];
					}
					if (target.uv3.Length != this.uv3.Length)
					{
						target.uv3 = new Vector2[this.uv3.Length];
					}
					if (target.uv4.Length != this.uv4.Length)
					{
						target.uv4 = new Vector2[this.uv4.Length];
					}
					if (target.tangents.Length != this.tangents.Length)
					{
						target.tangents = new Vector4[this.tangents.Length];
					}
					if (target.triangles.Length != this.triangles.Length)
					{
						target.triangles = new int[this.triangles.Length];
					}
					this.vertices.CopyTo(target.vertices, 0);
					this.normals.CopyTo(target.normals, 0);
					this.colors.CopyTo(target.colors, 0);
					this.uv.CopyTo(target.uv, 0);
					this.uv2.CopyTo(target.uv2, 0);
					this.uv3.CopyTo(target.uv3, 0);
					this.uv4.CopyTo(target.uv4, 0);
					this.tangents.CopyTo(target.tangents, 0);
					this.triangles.CopyTo(target.triangles, 0);
					if (target.subMeshes == null)
					{
						target.subMeshes = new List<int[]>();
					}
					if (forceMaterialId >= 0)
					{
						while (target.subMeshes.Count > forceMaterialId + 1)
						{
							target.subMeshes.RemoveAt(0);
						}
						while (target.subMeshes.Count < forceMaterialId + 1)
						{
							target.subMeshes.Add(new int[0]);
						}
						for (int i = 0; i < target.subMeshes.Count; i++)
						{
							if (i != forceMaterialId)
							{
								if (target.subMeshes[i].Length != 0)
								{
									target.subMeshes[i] = new int[0];
								}
							}
							else
							{
								if (target.subMeshes[i].Length != this.triangles.Length)
								{
									target.subMeshes[i] = new int[this.triangles.Length];
								}
								this.triangles.CopyTo(target.subMeshes[i], 0);
							}
						}
						return;
					}
					while (target.subMeshes.Count > this.subMeshes.Count)
					{
						target.subMeshes.RemoveAt(0);
					}
					while (target.subMeshes.Count < this.subMeshes.Count)
					{
						target.subMeshes.Add(new int[0]);
					}
					for (int j = 0; j < this.subMeshes.Count; j++)
					{
						if (this.subMeshes[j].triangles.Length != target.subMeshes[j].Length)
						{
							target.subMeshes[j] = new int[this.subMeshes[j].triangles.Length];
						}
						this.subMeshes[j].triangles.CopyTo(target.subMeshes[j], 0);
					}
				}

				// Token: 0x06005625 RID: 22053 RVA: 0x0024D23C File Offset: 0x0024B43C
				private void CalculateBounds()
				{
					Vector3 zero = Vector3.zero;
					Vector3 zero2 = Vector3.zero;
					for (int i = 0; i < this.vertices.Length; i++)
					{
						if (this.vertices[i].x < zero.x)
						{
							zero.x = this.vertices[i].x;
						}
						else if (this.vertices[i].x > zero2.x)
						{
							zero2.x = this.vertices[i].x;
						}
						if (this.vertices[i].y < zero.y)
						{
							zero.y = this.vertices[i].y;
						}
						else if (this.vertices[i].y > zero2.y)
						{
							zero2.y = this.vertices[i].y;
						}
						if (this.vertices[i].z < zero.z)
						{
							zero.z = this.vertices[i].z;
						}
						else if (this.vertices[i].z > zero2.z)
						{
							zero2.z = this.vertices[i].z;
						}
					}
					this.bounds.CreateFromMinMax(zero, zero2);
				}

				// Token: 0x06005626 RID: 22054 RVA: 0x0024D3A8 File Offset: 0x0024B5A8
				private void Mirror()
				{
					if (this._mirror == SplineMesh.Channel.MeshDefinition.MirrorMethod.None)
					{
						return;
					}
					switch (this._mirror)
					{
					case SplineMesh.Channel.MeshDefinition.MirrorMethod.X:
						for (int i = 0; i < this.vertices.Length; i++)
						{
							Vector3[] array = this.vertices;
							int num = i;
							array[num].x = array[num].x * -1f;
							this.normals[i].x = -this.normals[i].x;
						}
						break;
					case SplineMesh.Channel.MeshDefinition.MirrorMethod.Y:
						for (int j = 0; j < this.vertices.Length; j++)
						{
							Vector3[] array2 = this.vertices;
							int num2 = j;
							array2[num2].y = array2[num2].y * -1f;
							this.normals[j].y = -this.normals[j].y;
						}
						break;
					case SplineMesh.Channel.MeshDefinition.MirrorMethod.Z:
						for (int k = 0; k < this.vertices.Length; k++)
						{
							Vector3[] array3 = this.vertices;
							int num3 = k;
							array3[num3].z = array3[num3].z * -1f;
							this.normals[k].z = -this.normals[k].z;
						}
						break;
					}
					for (int l = 0; l < this.triangles.Length; l += 3)
					{
						int num4 = this.triangles[l];
						this.triangles[l] = this.triangles[l + 2];
						this.triangles[l + 2] = num4;
					}
					for (int m = 0; m < this.subMeshes.Count; m++)
					{
						for (int n = 0; n < this.subMeshes[m].triangles.Length; n += 3)
						{
							int num5 = this.subMeshes[m].triangles[n];
							this.subMeshes[m].triangles[n] = this.subMeshes[m].triangles[n + 2];
							this.subMeshes[m].triangles[n + 2] = num5;
						}
					}
					this.CalculateTangents();
				}

				// Token: 0x06005627 RID: 22055 RVA: 0x0024D5C0 File Offset: 0x0024B7C0
				private void TransformVertices()
				{
					Matrix4x4 matrix4x = default(Matrix4x4);
					matrix4x.SetTRS(this._offset, Quaternion.Euler(this._rotation), this._scale);
					Matrix4x4 transpose = matrix4x.inverse.transpose;
					for (int i = 0; i < this.vertices.Length; i++)
					{
						this.vertices[i] = matrix4x.MultiplyPoint3x4(this.vertices[i]);
						this.normals[i] = transpose.MultiplyVector(this.normals[i]).normalized;
					}
					for (int j = 0; j < this.tangents.Length; j++)
					{
						this.tangents[j] = transpose.MultiplyVector(this.tangents[j]);
					}
					for (int k = 0; k < this.uv.Length; k++)
					{
						Vector2[] array = this.uv;
						int num = k;
						array[num].x = array[num].x * this._uvScale.x;
						Vector2[] array2 = this.uv;
						int num2 = k;
						array2[num2].y = array2[num2].y * this._uvScale.y;
						this.uv[k] += this._uvOffset;
						this.uv[k] = Quaternion.AngleAxis(this.uvRotation, Vector3.forward) * this.uv[k];
					}
				}

				// Token: 0x06005628 RID: 22056 RVA: 0x0024D75C File Offset: 0x0024B95C
				private void GroupVertices()
				{
					this.vertexGroups = new List<SplineMesh.Channel.MeshDefinition.VertexGroup>();
					for (int i = 0; i < this.vertices.Length; i++)
					{
						float z = this.vertices[i].z;
						double perc = DMath.Clamp01(DMath.InverseLerp((double)this.bounds.min.z, (double)this.bounds.max.z, (double)z));
						int num = this.FindInsertIndex(this.vertices[i], z);
						if (num >= this.vertexGroups.Count)
						{
							this.vertexGroups.Add(new SplineMesh.Channel.MeshDefinition.VertexGroup(z, perc, new int[]
							{
								i
							}));
						}
						else
						{
							float num2 = Mathf.Abs(this.vertexGroups[num].value - z);
							if (num2 < this.vertexGroupingMargin || Mathf.Approximately(num2, this.vertexGroupingMargin))
							{
								this.vertexGroups[num].AddId(i);
							}
							else if (this.vertexGroups[num].value < z)
							{
								this.vertexGroups.Insert(num, new SplineMesh.Channel.MeshDefinition.VertexGroup(z, perc, new int[]
								{
									i
								}));
							}
							else if (num < this.vertexGroups.Count - 1)
							{
								this.vertexGroups.Insert(num + 1, new SplineMesh.Channel.MeshDefinition.VertexGroup(z, perc, new int[]
								{
									i
								}));
							}
							else
							{
								this.vertexGroups.Add(new SplineMesh.Channel.MeshDefinition.VertexGroup(z, perc, new int[]
								{
									i
								}));
							}
						}
					}
				}

				// Token: 0x06005629 RID: 22057 RVA: 0x0024D8DC File Offset: 0x0024BADC
				private int FindInsertIndex(Vector3 pos, float value)
				{
					int i = 0;
					int num = this.vertexGroups.Count - 1;
					while (i <= num)
					{
						int num2 = i + (num - i) / 2;
						if (this.vertexGroups[num2].value == value)
						{
							return num2;
						}
						if (this.vertexGroups[num2].value < value)
						{
							num = num2 - 1;
						}
						else
						{
							i = num2 + 1;
						}
					}
					return i;
				}

				// Token: 0x0600562A RID: 22058 RVA: 0x0024D93C File Offset: 0x0024BB3C
				private void CalculateTangents()
				{
					if (this.vertices.Length == 0)
					{
						this.tangents = new Vector4[0];
						return;
					}
					this.tangents = new Vector4[this.vertices.Length];
					Vector3[] array = new Vector3[this.vertices.Length];
					Vector3[] array2 = new Vector3[this.vertices.Length];
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						for (int j = 0; j < this.subMeshes[i].triangles.Length; j += 3)
						{
							int num = this.subMeshes[i].triangles[j];
							int num2 = this.subMeshes[i].triangles[j + 1];
							int num3 = this.subMeshes[i].triangles[j + 2];
							float num4 = this.vertices[num2].x - this.vertices[num].x;
							float num5 = this.vertices[num3].x - this.vertices[num].x;
							float num6 = this.vertices[num2].y - this.vertices[num].y;
							float num7 = this.vertices[num3].y - this.vertices[num].y;
							float num8 = this.vertices[num2].z - this.vertices[num].z;
							float num9 = this.vertices[num3].z - this.vertices[num].z;
							float num10 = this.uv[num2].x - this.uv[num].x;
							float num11 = this.uv[num3].x - this.uv[num].x;
							float num12 = this.uv[num2].y - this.uv[num].y;
							float num13 = this.uv[num3].y - this.uv[num].y;
							float num14 = num10 * num13 - num11 * num12;
							float num15 = (num14 == 0f) ? 0f : (1f / num14);
							Vector3 b = new Vector3((num13 * num4 - num12 * num5) * num15, (num13 * num6 - num12 * num7) * num15, (num13 * num8 - num12 * num9) * num15);
							Vector3 b2 = new Vector3((num10 * num5 - num11 * num4) * num15, (num10 * num7 - num11 * num6) * num15, (num10 * num9 - num11 * num8) * num15);
							array[num] += b;
							array[num2] += b;
							array[num3] += b;
							array2[num] += b2;
							array2[num2] += b2;
							array2[num3] += b2;
						}
					}
					for (int k = 0; k < this.vertices.Length; k++)
					{
						Vector3 lhs = this.normals[k];
						Vector3 vector = array[k];
						Vector3.OrthoNormalize(ref lhs, ref vector);
						this.tangents[k].x = vector.x;
						this.tangents[k].y = vector.y;
						this.tangents[k].z = vector.z;
						this.tangents[k].w = ((Vector3.Dot(Vector3.Cross(lhs, vector), array2[k]) < 0f) ? -1f : 1f);
					}
				}

				// Token: 0x0400472A RID: 18218
				[SerializeField]
				[HideInInspector]
				internal Vector3[] vertices = new Vector3[0];

				// Token: 0x0400472B RID: 18219
				[SerializeField]
				[HideInInspector]
				internal Vector3[] normals = new Vector3[0];

				// Token: 0x0400472C RID: 18220
				[SerializeField]
				[HideInInspector]
				internal Vector4[] tangents = new Vector4[0];

				// Token: 0x0400472D RID: 18221
				[SerializeField]
				[HideInInspector]
				internal Color[] colors = new Color[0];

				// Token: 0x0400472E RID: 18222
				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv = new Vector2[0];

				// Token: 0x0400472F RID: 18223
				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv2 = new Vector2[0];

				// Token: 0x04004730 RID: 18224
				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv3 = new Vector2[0];

				// Token: 0x04004731 RID: 18225
				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv4 = new Vector2[0];

				// Token: 0x04004732 RID: 18226
				[SerializeField]
				[HideInInspector]
				internal int[] triangles = new int[0];

				// Token: 0x04004733 RID: 18227
				[SerializeField]
				[HideInInspector]
				internal List<SplineMesh.Channel.MeshDefinition.Submesh> subMeshes = new List<SplineMesh.Channel.MeshDefinition.Submesh>();

				// Token: 0x04004734 RID: 18228
				[SerializeField]
				[HideInInspector]
				internal TS_Bounds bounds = new TS_Bounds(Vector3.zero, Vector3.zero);

				// Token: 0x04004735 RID: 18229
				[SerializeField]
				[HideInInspector]
				internal List<SplineMesh.Channel.MeshDefinition.VertexGroup> vertexGroups = new List<SplineMesh.Channel.MeshDefinition.VertexGroup>();

				// Token: 0x04004736 RID: 18230
				[SerializeField]
				[HideInInspector]
				private Mesh _mesh;

				// Token: 0x04004737 RID: 18231
				[SerializeField]
				[HideInInspector]
				private Vector3 _rotation = Vector3.zero;

				// Token: 0x04004738 RID: 18232
				[SerializeField]
				[HideInInspector]
				private Vector3 _offset = Vector3.zero;

				// Token: 0x04004739 RID: 18233
				[SerializeField]
				[HideInInspector]
				private Vector3 _scale = Vector3.one;

				// Token: 0x0400473A RID: 18234
				[SerializeField]
				[HideInInspector]
				private Vector2 _uvScale = Vector2.one;

				// Token: 0x0400473B RID: 18235
				[SerializeField]
				[HideInInspector]
				private Vector2 _uvOffset = Vector2.zero;

				// Token: 0x0400473C RID: 18236
				[SerializeField]
				[HideInInspector]
				private float _uvRotation;

				// Token: 0x0400473D RID: 18237
				[SerializeField]
				[HideInInspector]
				private SplineMesh.Channel.MeshDefinition.MirrorMethod _mirror;

				// Token: 0x0400473E RID: 18238
				[SerializeField]
				[HideInInspector]
				private float _vertexGroupingMargin;

				// Token: 0x0400473F RID: 18239
				[SerializeField]
				[HideInInspector]
				private bool _removeInnerFaces;

				// Token: 0x04004740 RID: 18240
				[SerializeField]
				[HideInInspector]
				private bool _flipFaces;

				// Token: 0x04004741 RID: 18241
				[SerializeField]
				[HideInInspector]
				private bool _doubleSided;

				// Token: 0x02000A52 RID: 2642
				public enum MirrorMethod
				{
					// Token: 0x04004761 RID: 18273
					None,
					// Token: 0x04004762 RID: 18274
					X,
					// Token: 0x04004763 RID: 18275
					Y,
					// Token: 0x04004764 RID: 18276
					Z
				}

				// Token: 0x02000A53 RID: 2643
				[Serializable]
				public class Submesh
				{
					// Token: 0x06005642 RID: 22082 RVA: 0x0024E37D File Offset: 0x0024C57D
					public Submesh()
					{
					}

					// Token: 0x06005643 RID: 22083 RVA: 0x0024E391 File Offset: 0x0024C591
					public Submesh(int[] input)
					{
						this.triangles = new int[input.Length];
						input.CopyTo(this.triangles, 0);
					}

					// Token: 0x04004765 RID: 18277
					public int[] triangles = new int[0];
				}

				// Token: 0x02000A54 RID: 2644
				[Serializable]
				public class VertexGroup
				{
					// Token: 0x06005644 RID: 22084 RVA: 0x0024E3C0 File Offset: 0x0024C5C0
					public VertexGroup(float val, double perc, int[] vertIds)
					{
						this.percent = perc;
						this.value = val;
						this.ids = vertIds;
					}

					// Token: 0x06005645 RID: 22085 RVA: 0x0024E3E0 File Offset: 0x0024C5E0
					public void AddId(int id)
					{
						int[] array = new int[this.ids.Length + 1];
						this.ids.CopyTo(array, 0);
						array[array.Length - 1] = id;
						this.ids = array;
					}

					// Token: 0x04004766 RID: 18278
					public float value;

					// Token: 0x04004767 RID: 18279
					public double percent;

					// Token: 0x04004768 RID: 18280
					public int[] ids;
				}
			}
		}
	}
}
