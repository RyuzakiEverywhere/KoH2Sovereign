using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B1 RID: 1201
	[ExecuteInEditMode]
	[AddComponentMenu("Dreamteck/Splines/Node Connector")]
	public class Node : MonoBehaviour
	{
		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06003EF7 RID: 16119 RVA: 0x001E15C9 File Offset: 0x001DF7C9
		// (set) Token: 0x06003EF8 RID: 16120 RVA: 0x001E15D1 File Offset: 0x001DF7D1
		public bool transformNormals
		{
			get
			{
				return this._transformNormals;
			}
			set
			{
				if (value != this._transformNormals)
				{
					this._transformNormals = value;
					this.UpdatePoints();
				}
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06003EF9 RID: 16121 RVA: 0x001E15E9 File Offset: 0x001DF7E9
		// (set) Token: 0x06003EFA RID: 16122 RVA: 0x001E15F1 File Offset: 0x001DF7F1
		public bool transformSize
		{
			get
			{
				return this._transformSize;
			}
			set
			{
				if (value != this._transformSize)
				{
					this._transformSize = value;
					this.UpdatePoints();
				}
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06003EFB RID: 16123 RVA: 0x001E1609 File Offset: 0x001DF809
		// (set) Token: 0x06003EFC RID: 16124 RVA: 0x001E1611 File Offset: 0x001DF811
		public bool transformTangents
		{
			get
			{
				return this._transformTangents;
			}
			set
			{
				if (value != this._transformTangents)
				{
					this._transformTangents = value;
					this.UpdatePoints();
				}
			}
		}

		// Token: 0x06003EFD RID: 16125 RVA: 0x001E1629 File Offset: 0x001DF829
		private void Awake()
		{
			this.trs = base.transform;
			this.SampleTransform();
		}

		// Token: 0x06003EFE RID: 16126 RVA: 0x001E163D File Offset: 0x001DF83D
		private void LateUpdate()
		{
			this.Run();
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x001E163D File Offset: 0x001DF83D
		private void Update()
		{
			this.Run();
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x001E1648 File Offset: 0x001DF848
		private bool TransformChanged()
		{
			return this.lastPosition != this.trs.position || this.lastRotation != this.trs.rotation || this.lastScale != this.trs.lossyScale;
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x001E169D File Offset: 0x001DF89D
		private void SampleTransform()
		{
			this.lastPosition = this.trs.position;
			this.lastScale = this.trs.lossyScale;
			this.lastRotation = this.trs.rotation;
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x001E16D2 File Offset: 0x001DF8D2
		private void Run()
		{
			if (this.TransformChanged())
			{
				this.UpdateConnectedComputers(null);
				this.SampleTransform();
			}
		}

		// Token: 0x06003F03 RID: 16131 RVA: 0x001E16EC File Offset: 0x001DF8EC
		public SplinePoint GetPoint(int connectionIndex, bool swapTangents)
		{
			SplinePoint splinePoint = this.PointToWorld(this.connections[connectionIndex].point);
			if (this.connections[connectionIndex].invertTangents && swapTangents)
			{
				Vector3 tangent = splinePoint.tangent;
				splinePoint.tangent = splinePoint.tangent2;
				splinePoint.tangent2 = tangent;
			}
			return splinePoint;
		}

		// Token: 0x06003F04 RID: 16132 RVA: 0x001E173C File Offset: 0x001DF93C
		public void SetPoint(int connectionIndex, SplinePoint worldPoint, bool swappedTangents)
		{
			Node.Connection connection = this.connections[connectionIndex];
			connection.point = this.PointToLocal(worldPoint);
			if (connection.invertTangents && swappedTangents)
			{
				Vector3 tangent = connection.point.tangent;
				connection.point.tangent = connection.point.tangent2;
				connection.point.tangent2 = tangent;
			}
			if (this.type == Node.Type.Smooth)
			{
				if (connection.point.type == SplinePoint.Type.SmoothFree)
				{
					for (int i = 0; i < this.connections.Length; i++)
					{
						if (i != connectionIndex)
						{
							Vector3 vector = (connection.point.tangent - connection.point.position).normalized;
							if (vector == Vector3.zero)
							{
								vector = -(connection.point.tangent2 - connection.point.position).normalized;
							}
							float magnitude = (this.connections[i].point.tangent - this.connections[i].point.position).magnitude;
							float magnitude2 = (this.connections[i].point.tangent2 - this.connections[i].point.position).magnitude;
							this.connections[i].point = connection.point;
							this.connections[i].point.tangent = this.connections[i].point.position + vector * magnitude;
							this.connections[i].point.tangent2 = this.connections[i].point.position - vector * magnitude2;
						}
					}
					return;
				}
				for (int j = 0; j < this.connections.Length; j++)
				{
					if (j != connectionIndex)
					{
						this.connections[j].point = connection.point;
					}
				}
			}
		}

		// Token: 0x06003F05 RID: 16133 RVA: 0x001E193D File Offset: 0x001DFB3D
		private void OnDestroy()
		{
			this.ClearConnections();
		}

		// Token: 0x06003F06 RID: 16134 RVA: 0x001E1948 File Offset: 0x001DFB48
		public void ClearConnections()
		{
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				if (this.connections[i].spline != null)
				{
					this.connections[i].spline.DisconnectNode(this.connections[i].pointIndex);
				}
			}
			this.connections = new Node.Connection[0];
		}

		// Token: 0x06003F07 RID: 16135 RVA: 0x001E19AC File Offset: 0x001DFBAC
		public void UpdateConnectedComputers(SplineComputer excludeComputer = null)
		{
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				if (!this.connections[i].isValid)
				{
					this.RemoveConnection(i);
				}
				else if (!(this.connections[i].spline == excludeComputer))
				{
					if (this.type == Node.Type.Smooth && i != 0)
					{
						this.SetPoint(i, this.GetPoint(0, false), false);
					}
					SplinePoint point = this.GetPoint(i, true);
					if (!this.transformNormals)
					{
						point.normal = this.connections[i].spline.GetPointNormal(this.connections[i].pointIndex, SplineComputer.Space.World);
					}
					if (!this.transformTangents)
					{
						point.tangent = this.connections[i].spline.GetPointTangent(this.connections[i].pointIndex, SplineComputer.Space.World);
						point.tangent2 = this.connections[i].spline.GetPointTangent2(this.connections[i].pointIndex, SplineComputer.Space.World);
					}
					if (!this.transformSize)
					{
						point.size = this.connections[i].spline.GetPointSize(this.connections[i].pointIndex, SplineComputer.Space.World);
					}
					this.connections[i].spline.SetPoint(this.connections[i].pointIndex, point, SplineComputer.Space.World);
				}
			}
		}

		// Token: 0x06003F08 RID: 16136 RVA: 0x001E1B00 File Offset: 0x001DFD00
		public void UpdatePoint(SplineComputer computer, int pointIndex, SplinePoint point, bool updatePosition = true)
		{
			this.trs.position = point.position;
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i].spline == computer && this.connections[i].pointIndex == pointIndex)
				{
					this.SetPoint(i, point, true);
				}
			}
		}

		// Token: 0x06003F09 RID: 16137 RVA: 0x001E1B60 File Offset: 0x001DFD60
		private void UpdatePoints()
		{
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				if (!this.connections[i].isValid)
				{
					this.RemoveConnection(i);
				}
				else
				{
					SplinePoint point = this.connections[i].spline.GetPoint(this.connections[i].pointIndex, SplineComputer.Space.World);
					point.SetPosition(base.transform.position);
					this.SetPoint(i, point, true);
				}
			}
		}

		// Token: 0x06003F0A RID: 16138 RVA: 0x001E1BD8 File Offset: 0x001DFDD8
		protected void RemoveInvalidConnections()
		{
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				if (this.connections[i] == null || !this.connections[i].isValid)
				{
					this.RemoveConnection(i);
				}
			}
		}

		// Token: 0x06003F0B RID: 16139 RVA: 0x001E1C1C File Offset: 0x001DFE1C
		public virtual void AddConnection(SplineComputer computer, int pointIndex)
		{
			this.RemoveInvalidConnections();
			Node node = computer.GetNode(pointIndex);
			if (node != null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					computer.name,
					" is already connected to node ",
					node.name,
					" at point ",
					pointIndex
				}));
				return;
			}
			SplinePoint point = computer.GetPoint(pointIndex, SplineComputer.Space.World);
			point.SetPosition(base.transform.position);
			ArrayUtility.Add<Node.Connection>(ref this.connections, new Node.Connection(computer, pointIndex, this.PointToLocal(point)));
			if (this.connections.Length == 1)
			{
				this.SetPoint(this.connections.Length - 1, point, true);
			}
			this.UpdateConnectedComputers(null);
		}

		// Token: 0x06003F0C RID: 16140 RVA: 0x001E1CD4 File Offset: 0x001DFED4
		protected SplinePoint PointToLocal(SplinePoint worldPoint)
		{
			worldPoint.position = Vector3.zero;
			worldPoint.tangent = base.transform.InverseTransformPoint(worldPoint.tangent);
			worldPoint.tangent2 = base.transform.InverseTransformPoint(worldPoint.tangent2);
			worldPoint.normal = base.transform.InverseTransformDirection(worldPoint.normal);
			worldPoint.size /= (base.transform.localScale.x + base.transform.localScale.y + base.transform.localScale.z) / 3f;
			return worldPoint;
		}

		// Token: 0x06003F0D RID: 16141 RVA: 0x001E1D7C File Offset: 0x001DFF7C
		protected SplinePoint PointToWorld(SplinePoint localPoint)
		{
			localPoint.position = base.transform.position;
			localPoint.tangent = base.transform.TransformPoint(localPoint.tangent);
			localPoint.tangent2 = base.transform.TransformPoint(localPoint.tangent2);
			localPoint.normal = base.transform.TransformDirection(localPoint.normal);
			localPoint.size *= (base.transform.localScale.x + base.transform.localScale.y + base.transform.localScale.z) / 3f;
			return localPoint;
		}

		// Token: 0x06003F0E RID: 16142 RVA: 0x001E1E28 File Offset: 0x001E0028
		public virtual void RemoveConnection(SplineComputer computer, int pointIndex)
		{
			int num = -1;
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i].pointIndex == pointIndex && this.connections[i].spline == computer)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return;
			}
			this.RemoveConnection(num);
		}

		// Token: 0x06003F0F RID: 16143 RVA: 0x001E1E80 File Offset: 0x001E0080
		private void RemoveConnection(int index)
		{
			Node.Connection[] array = new Node.Connection[this.connections.Length - 1];
			SplineComputer spline = this.connections[index].spline;
			int pointIndex = this.connections[index].pointIndex;
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (i < index)
				{
					array[i] = this.connections[i];
				}
				else if (i != index)
				{
					array[i - 1] = this.connections[i];
				}
			}
			this.connections = array;
		}

		// Token: 0x06003F10 RID: 16144 RVA: 0x001E1EF8 File Offset: 0x001E00F8
		public virtual bool HasConnection(SplineComputer computer, int pointIndex)
		{
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				if (!this.connections[i].isValid)
				{
					this.RemoveConnection(i);
				}
				else if (this.connections[i].spline == computer && this.connections[i].pointIndex == pointIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003F11 RID: 16145 RVA: 0x001E1F5A File Offset: 0x001E015A
		public Node.Connection[] GetConnections()
		{
			return this.connections;
		}

		// Token: 0x04002C9C RID: 11420
		[HideInInspector]
		public Node.Type type;

		// Token: 0x04002C9D RID: 11421
		[SerializeField]
		[HideInInspector]
		protected Node.Connection[] connections = new Node.Connection[0];

		// Token: 0x04002C9E RID: 11422
		[SerializeField]
		[HideInInspector]
		private bool _transformSize = true;

		// Token: 0x04002C9F RID: 11423
		[SerializeField]
		[HideInInspector]
		private bool _transformNormals = true;

		// Token: 0x04002CA0 RID: 11424
		[SerializeField]
		[HideInInspector]
		private bool _transformTangents = true;

		// Token: 0x04002CA1 RID: 11425
		private Vector3 lastPosition;

		// Token: 0x04002CA2 RID: 11426
		private Vector3 lastScale;

		// Token: 0x04002CA3 RID: 11427
		private Quaternion lastRotation;

		// Token: 0x04002CA4 RID: 11428
		private Transform trs;

		// Token: 0x02000983 RID: 2435
		[Serializable]
		public class Connection
		{
			// Token: 0x170006EC RID: 1772
			// (get) Token: 0x06005415 RID: 21525 RVA: 0x00245484 File Offset: 0x00243684
			public SplineComputer spline
			{
				get
				{
					return this._computer;
				}
			}

			// Token: 0x170006ED RID: 1773
			// (get) Token: 0x06005416 RID: 21526 RVA: 0x0024548C File Offset: 0x0024368C
			public int pointIndex
			{
				get
				{
					return this._pointIndex;
				}
			}

			// Token: 0x170006EE RID: 1774
			// (get) Token: 0x06005417 RID: 21527 RVA: 0x00245494 File Offset: 0x00243694
			internal bool isValid
			{
				get
				{
					return !(this._computer == null) && this._pointIndex < this._computer.pointCount;
				}
			}

			// Token: 0x06005418 RID: 21528 RVA: 0x002454BC File Offset: 0x002436BC
			internal Connection(SplineComputer comp, int index, SplinePoint inputPoint)
			{
				this._pointIndex = index;
				this._computer = comp;
				this.point = inputPoint;
			}

			// Token: 0x04004437 RID: 17463
			public bool invertTangents;

			// Token: 0x04004438 RID: 17464
			[SerializeField]
			private int _pointIndex;

			// Token: 0x04004439 RID: 17465
			[SerializeField]
			private SplineComputer _computer;

			// Token: 0x0400443A RID: 17466
			[SerializeField]
			[HideInInspector]
			internal SplinePoint point;
		}

		// Token: 0x02000984 RID: 2436
		public enum Type
		{
			// Token: 0x0400443C RID: 17468
			Smooth,
			// Token: 0x0400443D RID: 17469
			Free
		}
	}
}
