using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dreamteck.Splines.IO
{
	// Token: 0x020004DD RID: 1245
	public class CSV : SplineParser
	{
		// Token: 0x060041D8 RID: 16856 RVA: 0x001F4F40 File Offset: 0x001F3140
		public CSV(SplineComputer computer)
		{
			Spline spline = new Spline(computer.type, computer.sampleRate);
			spline.points = computer.GetPoints(SplineComputer.Space.World);
			if (spline.type != Spline.Type.Bezier && spline.type != Spline.Type.Linear)
			{
				spline.CatToBezierTangents();
			}
			if (computer.isClosed)
			{
				spline.Close();
			}
			this.buffer = new SplineParser.SplineDefinition(computer.name, spline);
			this.fileName = computer.name;
			this.columns.Add(CSV.ColumnType.Position);
			this.columns.Add(CSV.ColumnType.Tangent);
			this.columns.Add(CSV.ColumnType.Tangent2);
		}

		// Token: 0x060041D9 RID: 16857 RVA: 0x001F4FE8 File Offset: 0x001F31E8
		public CSV(string filePath, List<CSV.ColumnType> customColumns = null)
		{
			if (File.Exists(filePath))
			{
				string a = Path.GetExtension(filePath).ToLowerInvariant();
				this.fileName = Path.GetFileNameWithoutExtension(filePath);
				if (a != ".csv")
				{
					Debug.LogError("CSV Parsing ERROR: Wrong format. Please use SVG or XML");
					return;
				}
				string[] lines = File.ReadAllLines(filePath);
				if (customColumns == null)
				{
					this.columns.Add(CSV.ColumnType.Position);
					this.columns.Add(CSV.ColumnType.Tangent);
					this.columns.Add(CSV.ColumnType.Tangent2);
					this.columns.Add(CSV.ColumnType.Normal);
					this.columns.Add(CSV.ColumnType.Size);
					this.columns.Add(CSV.ColumnType.Color);
				}
				else
				{
					this.columns = new List<CSV.ColumnType>(customColumns);
				}
				this.buffer = new SplineParser.SplineDefinition(this.fileName, Spline.Type.CatmullRom);
				this.Read(lines);
			}
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x001F50B8 File Offset: 0x001F32B8
		private void Read(string[] lines)
		{
			int num = 0;
			using (List<CSV.ColumnType>.Enumerator enumerator = this.columns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case CSV.ColumnType.Position:
						num += 3;
						break;
					case CSV.ColumnType.Tangent:
						num += 3;
						break;
					case CSV.ColumnType.Tangent2:
						num += 3;
						break;
					case CSV.ColumnType.Normal:
						num += 3;
						break;
					case CSV.ColumnType.Size:
						num++;
						break;
					case CSV.ColumnType.Color:
						num += 4;
						break;
					}
				}
			}
			for (int i = 1; i < lines.Length; i++)
			{
				lines[i] = Regex.Replace(lines[i], "\\s+", "");
				string[] array = lines[i].Split(new char[]
				{
					','
				});
				if (array.Length != num)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Unexpected element count on row ",
						i,
						". Expected ",
						num,
						" found ",
						array.Length,
						" Please make sure that all values exist and the column order is correct."
					}));
				}
				else
				{
					float[] array2 = new float[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						float.TryParse(array[j], out array2[j]);
					}
					int num2 = 0;
					using (List<CSV.ColumnType>.Enumerator enumerator = this.columns.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							switch (enumerator.Current)
							{
							case CSV.ColumnType.Position:
								this.buffer.position = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
								break;
							case CSV.ColumnType.Tangent:
								this.buffer.tangent = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
								break;
							case CSV.ColumnType.Tangent2:
								this.buffer.tangent2 = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
								break;
							case CSV.ColumnType.Normal:
								this.buffer.normal = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
								break;
							case CSV.ColumnType.Size:
								this.buffer.size = array2[num2++];
								break;
							case CSV.ColumnType.Color:
								this.buffer.color = new Color(array2[num2++], array2[num2++], array2[num2++], array2[num2++]);
								break;
							}
						}
					}
					this.buffer.CreateSmooth();
				}
			}
		}

		// Token: 0x060041DB RID: 16859 RVA: 0x001F53B4 File Offset: 0x001F35B4
		public SplineComputer CreateSplineComputer(Vector3 position, Quaternion rotation)
		{
			return this.buffer.CreateSplineComputer(position, rotation);
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x001F53C3 File Offset: 0x001F35C3
		public Spline CreateSpline()
		{
			return this.buffer.CreateSpline();
		}

		// Token: 0x060041DD RID: 16861 RVA: 0x001F53D0 File Offset: 0x001F35D0
		public void FlatX()
		{
			for (int i = 0; i < this.buffer.pointCount; i++)
			{
				SplinePoint value = this.buffer.points[i];
				value.position.x = 0f;
				value.tangent.x = 0f;
				value.tangent2.x = 0f;
				value.normal = Vector3.right;
				this.buffer.points[i] = value;
			}
		}

		// Token: 0x060041DE RID: 16862 RVA: 0x001F5458 File Offset: 0x001F3658
		public void FlatY()
		{
			for (int i = 0; i < this.buffer.pointCount; i++)
			{
				SplinePoint value = this.buffer.points[i];
				value.position.y = 0f;
				value.tangent.y = 0f;
				value.tangent2.y = 0f;
				value.normal = Vector3.up;
				this.buffer.points[i] = value;
			}
		}

		// Token: 0x060041DF RID: 16863 RVA: 0x001F54E0 File Offset: 0x001F36E0
		public void FlatZ()
		{
			for (int i = 0; i < this.buffer.pointCount; i++)
			{
				SplinePoint value = this.buffer.points[i];
				value.position.z = 0f;
				value.tangent.z = 0f;
				value.tangent2.z = 0f;
				value.normal = Vector3.back;
				this.buffer.points[i] = value;
			}
		}

		// Token: 0x060041E0 RID: 16864 RVA: 0x001F5566 File Offset: 0x001F3766
		private void AddTitle(ref string[] content, string title)
		{
			if (!string.IsNullOrEmpty(content[0]))
			{
				string[] array = content;
				int num = 0;
				array[num] += ",";
			}
			string[] array2 = content;
			int num2 = 0;
			array2[num2] += title;
		}

		// Token: 0x060041E1 RID: 16865 RVA: 0x001F5599 File Offset: 0x001F3799
		private void AddVector3Title(ref string[] content, string prefix)
		{
			this.AddTitle(ref content, string.Concat(new string[]
			{
				prefix,
				"X,",
				prefix,
				"Y,",
				prefix,
				"Z"
			}));
		}

		// Token: 0x060041E2 RID: 16866 RVA: 0x001F55D4 File Offset: 0x001F37D4
		private void AddColorTitle(ref string[] content, string prefix)
		{
			this.AddTitle(ref content, string.Concat(new string[]
			{
				prefix,
				"R,",
				prefix,
				"G,",
				prefix,
				"B",
				prefix,
				"A"
			}));
		}

		// Token: 0x060041E3 RID: 16867 RVA: 0x001F5623 File Offset: 0x001F3823
		private void AddVector3(ref string[] content, int index, Vector3 vector)
		{
			this.AddFloat(ref content, index, vector.x);
			this.AddFloat(ref content, index, vector.y);
			this.AddFloat(ref content, index, vector.z);
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x001F564F File Offset: 0x001F384F
		private void AddColor(ref string[] content, int index, Color color)
		{
			this.AddFloat(ref content, index, color.r);
			this.AddFloat(ref content, index, color.g);
			this.AddFloat(ref content, index, color.b);
			this.AddFloat(ref content, index, color.a);
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x001F5689 File Offset: 0x001F3889
		private void AddFloat(ref string[] content, int index, float value)
		{
			if (!string.IsNullOrEmpty(content[index]))
			{
				string[] array = content;
				array[index] += ",";
			}
			string[] array2 = content;
			array2[index] += value.ToString();
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x001F56C4 File Offset: 0x001F38C4
		public void Write(string filePath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
			{
				throw new DirectoryNotFoundException("The file is being saved to a non-existing directory.");
			}
			List<SplinePoint> points = this.buffer.points;
			string[] contents = new string[points.Count + 1];
			using (List<CSV.ColumnType>.Enumerator enumerator = this.columns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case CSV.ColumnType.Position:
						this.AddVector3Title(ref contents, "Position");
						break;
					case CSV.ColumnType.Tangent:
						this.AddVector3Title(ref contents, "Tangent");
						break;
					case CSV.ColumnType.Tangent2:
						this.AddVector3Title(ref contents, "Tangent2");
						break;
					case CSV.ColumnType.Normal:
						this.AddVector3Title(ref contents, "Normal");
						break;
					case CSV.ColumnType.Size:
						this.AddTitle(ref contents, "Size");
						break;
					case CSV.ColumnType.Color:
						this.AddColorTitle(ref contents, "Color");
						break;
					}
				}
			}
			foreach (CSV.ColumnType columnType in this.columns)
			{
				for (int i = 1; i <= points.Count; i++)
				{
					int index = i - 1;
					switch (columnType)
					{
					case CSV.ColumnType.Position:
						this.AddVector3(ref contents, i, points[index].position);
						break;
					case CSV.ColumnType.Tangent:
						this.AddVector3(ref contents, i, points[index].tangent);
						break;
					case CSV.ColumnType.Tangent2:
						this.AddVector3(ref contents, i, points[index].tangent2);
						break;
					case CSV.ColumnType.Normal:
						this.AddVector3(ref contents, i, points[index].normal);
						break;
					case CSV.ColumnType.Size:
						this.AddFloat(ref contents, i, points[index].size);
						break;
					case CSV.ColumnType.Color:
						this.AddColor(ref contents, i, points[index].color);
						break;
					}
				}
			}
			File.WriteAllLines(filePath, contents);
		}

		// Token: 0x04002DDE RID: 11742
		public List<CSV.ColumnType> columns = new List<CSV.ColumnType>();

		// Token: 0x020009B6 RID: 2486
		public enum ColumnType
		{
			// Token: 0x04004533 RID: 17715
			Position,
			// Token: 0x04004534 RID: 17716
			Tangent,
			// Token: 0x04004535 RID: 17717
			Tangent2,
			// Token: 0x04004536 RID: 17718
			Normal,
			// Token: 0x04004537 RID: 17719
			Size,
			// Token: 0x04004538 RID: 17720
			Color
		}
	}
}
