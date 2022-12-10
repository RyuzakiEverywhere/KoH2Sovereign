using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Dreamteck.Splines.Primitives;
using UnityEngine;

namespace Dreamteck.Splines.IO
{
	// Token: 0x020004DE RID: 1246
	public class SVG : SplineParser
	{
		// Token: 0x060041E7 RID: 16871 RVA: 0x001F58E4 File Offset: 0x001F3AE4
		public SVG(string filePath)
		{
			if (File.Exists(filePath))
			{
				string a = Path.GetExtension(filePath).ToLowerInvariant();
				this.fileName = Path.GetFileNameWithoutExtension(filePath);
				if (a != ".svg" && a != ".xml")
				{
					Debug.LogError("SVG Parsing ERROR: Wrong format. Please use SVG or XML");
					return;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.XmlResolver = null;
				try
				{
					xmlDocument.Load(filePath);
				}
				catch (XmlException ex)
				{
					Debug.LogError(ex.Message);
					return;
				}
				this.Read(xmlDocument);
			}
		}

		// Token: 0x060041E8 RID: 16872 RVA: 0x001F59B8 File Offset: 0x001F3BB8
		public SVG(List<SplineComputer> computers)
		{
			this.paths = new List<SplineParser.SplineDefinition>(computers.Count);
			for (int i = 0; i < computers.Count; i++)
			{
				if (!(computers[i] == null))
				{
					Spline spline = new Spline(computers[i].type, computers[i].sampleRate);
					spline.points = computers[i].GetPoints(SplineComputer.Space.World);
					if (spline.type != Spline.Type.Bezier && spline.type != Spline.Type.Linear)
					{
						spline.CatToBezierTangents();
					}
					if (computers[i].isClosed)
					{
						spline.Close();
					}
					this.paths.Add(new SplineParser.SplineDefinition(computers[i].name, spline));
				}
			}
		}

		// Token: 0x060041E9 RID: 16873 RVA: 0x001F5AC0 File Offset: 0x001F3CC0
		public void Write(string filePath, SVG.Axis ax = SVG.Axis.Z)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("svg");
			foreach (SplineParser.SplineDefinition splineDefinition in this.paths)
			{
				string name = "path";
				string name2 = "d";
				if (splineDefinition.type == Spline.Type.Linear)
				{
					name2 = "points";
					if (splineDefinition.closed)
					{
						name = "polygon";
					}
					else
					{
						name = "polyline";
					}
				}
				XmlElement xmlElement2 = xmlDocument.CreateElement(name);
				XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("id");
				xmlAttribute.Value = splineDefinition.name;
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute(name2);
				if (splineDefinition.type == Spline.Type.Linear)
				{
					xmlAttribute.Value = this.EncodePolygon(splineDefinition, ax);
				}
				else
				{
					xmlAttribute.Value = this.EncodePath(splineDefinition, ax);
				}
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("stroke");
				xmlAttribute.Value = "black";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("stroke-width");
				xmlAttribute.Value = "3";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("fill");
				xmlAttribute.Value = "none";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlElement.AppendChild(xmlElement2);
			}
			XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("version");
			xmlAttribute2.Value = "1.1";
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlAttribute2 = xmlDocument.CreateAttribute("xmlns");
			xmlAttribute2.Value = "http://www.w3.org/2000/svg";
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlDocument.AppendChild(xmlElement);
			xmlDocument.Save(filePath);
		}

		// Token: 0x060041EA RID: 16874 RVA: 0x001F5CBC File Offset: 0x001F3EBC
		private Vector2 MapPoint(Vector3 original, SVG.Axis ax)
		{
			switch (ax)
			{
			case SVG.Axis.X:
				return new Vector2(original.z, -original.y);
			case SVG.Axis.Y:
				return new Vector2(original.x, -original.z);
			case SVG.Axis.Z:
				return new Vector2(original.x, -original.y);
			default:
				return original;
			}
		}

		// Token: 0x060041EB RID: 16875 RVA: 0x001F5D1C File Offset: 0x001F3F1C
		private void Read(XmlDocument doc)
		{
			this.transformBuffer.Clear();
			this.Traverse(doc.ChildNodes);
		}

		// Token: 0x060041EC RID: 16876 RVA: 0x001F5D38 File Offset: 0x001F3F38
		private void Traverse(XmlNodeList nodes)
		{
			foreach (object obj in nodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				int num = 0;
				string name = xmlNode.Name;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num2 <= 673280137U)
				{
					if (num2 <= 400234023U)
					{
						if (num2 != 85768329U)
						{
							if (num2 == 400234023U)
							{
								if (name == "line")
								{
									num = this.ReadLine(xmlNode);
								}
							}
						}
						else if (name == "polygon")
						{
							num = this.ReadPolygon(xmlNode, true);
						}
					}
					else if (num2 != 408269525U)
					{
						if (num2 == 673280137U)
						{
							if (name == "circle")
							{
								num = this.ReadEllipse(xmlNode);
							}
						}
					}
					else if (name == "polyline")
					{
						num = this.ReadPolygon(xmlNode, false);
					}
				}
				else if (num2 <= 2667997009U)
				{
					if (num2 != 2223459638U)
					{
						if (num2 == 2667997009U)
						{
							if (name == "ellipse")
							{
								num = this.ReadEllipse(xmlNode);
							}
						}
					}
					else if (name == "path")
					{
						num = this.ReadPath(xmlNode);
					}
				}
				else if (num2 != 3792446982U)
				{
					if (num2 == 3940830471U)
					{
						if (name == "rect")
						{
							num = this.ReadRectangle(xmlNode);
						}
					}
				}
				else if (name == "g")
				{
					num = this.ParseTransformation(xmlNode);
				}
				this.Traverse(xmlNode.ChildNodes);
				if (num > 0)
				{
					this.transformBuffer.RemoveRange(this.transformBuffer.Count - num, num);
				}
			}
		}

		// Token: 0x060041ED RID: 16877 RVA: 0x001F5F28 File Offset: 0x001F4128
		public List<SplineComputer> CreateSplineComputers(Vector3 position, Quaternion rotation, SVG.Element elements = SVG.Element.All)
		{
			List<SplineComputer> list = new List<SplineComputer>();
			if (elements == SVG.Element.All || elements == SVG.Element.Path)
			{
				foreach (SplineParser.SplineDefinition splineDefinition in this.paths)
				{
					list.Add(splineDefinition.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Polygon)
			{
				foreach (SplineParser.SplineDefinition splineDefinition2 in this.polygons)
				{
					list.Add(splineDefinition2.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Ellipse)
			{
				foreach (SplineParser.SplineDefinition splineDefinition3 in this.ellipses)
				{
					list.Add(splineDefinition3.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Rectangle)
			{
				foreach (SplineParser.SplineDefinition splineDefinition4 in this.rectangles)
				{
					list.Add(splineDefinition4.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Line)
			{
				foreach (SplineParser.SplineDefinition splineDefinition5 in this.lines)
				{
					list.Add(splineDefinition5.CreateSplineComputer(position, rotation));
				}
			}
			return list;
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x001F60D8 File Offset: 0x001F42D8
		public List<Spline> CreateSplines(SVG.Element elements = SVG.Element.All)
		{
			List<Spline> list = new List<Spline>();
			if (elements == SVG.Element.All || elements == SVG.Element.Path)
			{
				foreach (SplineParser.SplineDefinition splineDefinition in this.paths)
				{
					list.Add(splineDefinition.CreateSpline());
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Polygon)
			{
				foreach (SplineParser.SplineDefinition splineDefinition2 in this.polygons)
				{
					list.Add(splineDefinition2.CreateSpline());
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Ellipse)
			{
				foreach (SplineParser.SplineDefinition splineDefinition3 in this.ellipses)
				{
					list.Add(splineDefinition3.CreateSpline());
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Rectangle)
			{
				foreach (SplineParser.SplineDefinition splineDefinition4 in this.rectangles)
				{
					list.Add(splineDefinition4.CreateSpline());
				}
			}
			if (elements == SVG.Element.All || elements == SVG.Element.Line)
			{
				foreach (SplineParser.SplineDefinition splineDefinition5 in this.lines)
				{
					list.Add(splineDefinition5.CreateSpline());
				}
			}
			return list;
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x001F627C File Offset: 0x001F447C
		private int ReadRectangle(XmlNode rectNode)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = -1f;
			float num6 = -1f;
			string attributeContent = this.GetAttributeContent(rectNode, "x");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num);
			attributeContent = this.GetAttributeContent(rectNode, "y");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num2);
			attributeContent = this.GetAttributeContent(rectNode, "width");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num3);
			attributeContent = this.GetAttributeContent(rectNode, "height");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num4);
			attributeContent = this.GetAttributeContent(rectNode, "rx");
			if (attributeContent != "ERROR")
			{
				float.TryParse(attributeContent, out num5);
			}
			attributeContent = this.GetAttributeContent(rectNode, "ry");
			if (attributeContent != "ERROR")
			{
				float.TryParse(attributeContent, out num6);
			}
			else
			{
				num6 = num5;
			}
			string text = this.GetAttributeContent(rectNode, "id");
			if (num5 == -1f && num6 == -1f)
			{
				Rectangle rectangle = new Rectangle();
				rectangle.offset = new Vector2(num + num3 / 2f, -num2 - num4 / 2f);
				rectangle.size = new Vector2(num3, num4);
				if (text == "ERROR")
				{
					text = this.fileName + "_rectangle" + (this.rectangles.Count + 1);
				}
				this.buffer = new SplineParser.SplineDefinition(text, rectangle.CreateSpline());
			}
			else
			{
				RoundedRectangle roundedRectangle = new RoundedRectangle();
				roundedRectangle.offset = new Vector2(num + num3 / 2f, -num2 - num4 / 2f);
				roundedRectangle.size = new Vector2(num3, num4);
				roundedRectangle.xRadius = num5;
				roundedRectangle.yRadius = num6;
				if (text == "ERROR")
				{
					text = this.fileName + "_roundedRectangle" + (this.rectangles.Count + 1);
				}
				this.buffer = new SplineParser.SplineDefinition(text, roundedRectangle.CreateSpline());
			}
			int result = this.ParseTransformation(rectNode);
			this.WriteBufferTo(this.rectangles);
			return result;
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x001F64F0 File Offset: 0x001F46F0
		private int ReadLine(XmlNode lineNode)
		{
			float x = 0f;
			float num = 0f;
			float x2 = 0f;
			float num2 = 0f;
			string attributeContent = this.GetAttributeContent(lineNode, "x1");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out x);
			attributeContent = this.GetAttributeContent(lineNode, "y1");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num);
			attributeContent = this.GetAttributeContent(lineNode, "x2");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out x2);
			attributeContent = this.GetAttributeContent(lineNode, "y2");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num2);
			string text = this.GetAttributeContent(lineNode, "id");
			if (text == "ERROR")
			{
				text = this.fileName + "_line" + (this.ellipses.Count + 1);
			}
			this.buffer = new SplineParser.SplineDefinition(text, Spline.Type.Linear);
			this.buffer.position = new Vector2(x, -num);
			this.buffer.CreateLinear();
			this.buffer.position = new Vector2(x2, -num2);
			this.buffer.CreateLinear();
			int result = this.ParseTransformation(lineNode);
			this.WriteBufferTo(this.lines);
			return result;
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x001F665C File Offset: 0x001F485C
		private int ReadEllipse(XmlNode ellipseNode)
		{
			float x = 0f;
			float num = 0f;
			float num2 = 0f;
			float yRadius = 0f;
			string attributeContent = this.GetAttributeContent(ellipseNode, "cx");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out x);
			attributeContent = this.GetAttributeContent(ellipseNode, "cy");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out num);
			attributeContent = this.GetAttributeContent(ellipseNode, "r");
			string text = "circle";
			if (attributeContent == "ERROR")
			{
				text = "ellipse";
				attributeContent = this.GetAttributeContent(ellipseNode, "rx");
				if (attributeContent == "ERROR")
				{
					return 0;
				}
				float.TryParse(attributeContent, out num2);
				attributeContent = this.GetAttributeContent(ellipseNode, "ry");
				if (attributeContent == "ERROR")
				{
					return 0;
				}
			}
			else
			{
				float.TryParse(attributeContent, out num2);
				yRadius = num2;
			}
			float.TryParse(attributeContent, out yRadius);
			Ellipse ellipse = new Ellipse();
			ellipse.offset = new Vector2(x, -num);
			ellipse.xRadius = num2;
			ellipse.yRadius = yRadius;
			string text2 = this.GetAttributeContent(ellipseNode, "id");
			if (text2 == "ERROR")
			{
				text2 = string.Concat(new object[]
				{
					this.fileName,
					"_",
					text,
					this.ellipses.Count + 1
				});
			}
			this.buffer = new SplineParser.SplineDefinition(text2, ellipse.CreateSpline());
			int result = this.ParseTransformation(ellipseNode);
			this.WriteBufferTo(this.ellipses);
			return result;
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x001F6800 File Offset: 0x001F4A00
		private int ReadPolygon(XmlNode polyNode, bool closed)
		{
			string attributeContent = this.GetAttributeContent(polyNode, "points");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			List<float> list = base.ParseFloatArray(attributeContent);
			if (list.Count % 2 != 0)
			{
				Debug.LogWarning("There is an error with one of the polygon shapes.");
				return 0;
			}
			string text = this.GetAttributeContent(polyNode, "id");
			if (text == "ERROR")
			{
				text = this.fileName + (closed ? "_polygon " : "_polyline") + (this.polygons.Count + 1);
			}
			this.buffer = new SplineParser.SplineDefinition(text, Spline.Type.Linear);
			int num = list.Count / 2;
			for (int i = 0; i < num; i++)
			{
				this.buffer.position = new Vector2(list[2 * i], -list[1 + 2 * i]);
				this.buffer.CreateLinear();
			}
			if (closed)
			{
				this.buffer.CreateClosingPoint();
				this.buffer.closed = true;
			}
			int result = this.ParseTransformation(polyNode);
			this.WriteBufferTo(this.polygons);
			return result;
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x001F6918 File Offset: 0x001F4B18
		private int ParseTransformation(XmlNode node)
		{
			string attributeContent = this.GetAttributeContent(node, "transform");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			List<SplineParser.Transformation> list = this.ParseTransformations(attributeContent);
			this.transformBuffer.AddRange(list);
			return list.Count;
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x001F695C File Offset: 0x001F4B5C
		private List<SplineParser.Transformation> ParseTransformations(string transformContent)
		{
			List<SplineParser.Transformation> list = new List<SplineParser.Transformation>();
			foreach (object obj in Regex.Matches(transformContent.ToLowerInvariant(), "(?<function>translate|rotate|scale|skewx|skewy|matrix)\\s*\\((\\s*(?<param>-?\\s*\\d+(\\.\\d+)?)\\s*\\,*\\s*)+\\)"))
			{
				Match match = (Match)obj;
				if (match.Groups["function"].Success)
				{
					CaptureCollection captures = match.Groups["param"].Captures;
					string value = match.Groups["function"].Value;
					if (!(value == "translate"))
					{
						if (!(value == "rotate"))
						{
							if (!(value == "scale"))
							{
								if (!(value == "skewx"))
								{
									if (!(value == "skewy"))
									{
										if (value == "matrix")
										{
											if (captures.Count >= 6)
											{
												list.Add(new SplineParser.MatrixTransform(float.Parse(captures[0].Value), float.Parse(captures[1].Value), float.Parse(captures[2].Value), float.Parse(captures[3].Value), float.Parse(captures[4].Value), float.Parse(captures[5].Value)));
											}
										}
									}
									else if (captures.Count >= 1)
									{
										list.Add(new SplineParser.SkewY(float.Parse(captures[0].Value)));
									}
								}
								else if (captures.Count >= 1)
								{
									list.Add(new SplineParser.SkewX(float.Parse(captures[0].Value)));
								}
							}
							else if (captures.Count >= 2)
							{
								list.Add(new SplineParser.Scale(new Vector2(float.Parse(captures[0].Value), float.Parse(captures[1].Value))));
							}
						}
						else if (captures.Count >= 1)
						{
							list.Add(new SplineParser.Rotate(float.Parse(captures[0].Value)));
						}
					}
					else if (captures.Count >= 2)
					{
						list.Add(new SplineParser.Translate(new Vector2(float.Parse(captures[0].Value), float.Parse(captures[1].Value))));
					}
				}
			}
			return list;
		}

		// Token: 0x060041F5 RID: 16885 RVA: 0x001F6C08 File Offset: 0x001F4E08
		private int ReadPath(XmlNode pathNode)
		{
			string attributeContent = this.GetAttributeContent(pathNode, "d");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			string text = this.GetAttributeContent(pathNode, "id");
			if (text == "ERROR")
			{
				text = this.fileName + "_path " + (this.paths.Count + 1);
			}
			foreach (string text2 in from t in Regex.Split(attributeContent, "(?=[A-Za-z])")
			where !string.IsNullOrEmpty(t)
			select t)
			{
				char c = text2.Substring(0, 1).Single<char>();
				if (c <= 'Z')
				{
					if (c <= 'H')
					{
						if (c != 'C')
						{
							if (c == 'H')
							{
								this.PathHorizontalLineTo(text2, false);
							}
						}
						else
						{
							this.PathCurveTo(text2, SVG.PathSegment.Type.Cubic, false);
						}
					}
					else
					{
						switch (c)
						{
						case 'L':
							this.PathLineTo(text2, false);
							break;
						case 'M':
							this.PathStart(text, text2, false);
							break;
						case 'N':
						case 'O':
						case 'P':
						case 'R':
						case 'U':
							break;
						case 'Q':
							this.PathCurveTo(text2, SVG.PathSegment.Type.Quadratic, false);
							break;
						case 'S':
							this.PathCurveTo(text2, SVG.PathSegment.Type.CubicShort, false);
							break;
						case 'T':
							this.PathCurveTo(text2, SVG.PathSegment.Type.QuadraticShort, false);
							break;
						case 'V':
							this.PathVerticalLineTo(text2, false);
							break;
						default:
							if (c == 'Z')
							{
								this.PathClose();
							}
							break;
						}
					}
				}
				else if (c <= 'h')
				{
					if (c != 'c')
					{
						if (c == 'h')
						{
							this.PathHorizontalLineTo(text2, true);
						}
					}
					else
					{
						this.PathCurveTo(text2, SVG.PathSegment.Type.Cubic, true);
					}
				}
				else
				{
					switch (c)
					{
					case 'l':
						this.PathLineTo(text2, true);
						break;
					case 'm':
						this.PathStart(text, text2, true);
						break;
					case 'n':
					case 'o':
					case 'p':
					case 'r':
					case 'u':
						break;
					case 'q':
						this.PathCurveTo(text2, SVG.PathSegment.Type.Quadratic, true);
						break;
					case 's':
						this.PathCurveTo(text2, SVG.PathSegment.Type.CubicShort, true);
						break;
					case 't':
						this.PathCurveTo(text2, SVG.PathSegment.Type.QuadraticShort, true);
						break;
					case 'v':
						this.PathVerticalLineTo(text2, true);
						break;
					default:
						if (c == 'z')
						{
							this.PathClose();
						}
						break;
					}
				}
			}
			int result = this.ParseTransformation(pathNode);
			if (this.buffer != null)
			{
				this.WriteBufferTo(this.paths);
			}
			return result;
		}

		// Token: 0x060041F6 RID: 16886 RVA: 0x001F6E98 File Offset: 0x001F5098
		private void PathStart(string name, string coords, bool relative)
		{
			if (this.buffer != null)
			{
				this.WriteBufferTo(this.paths);
			}
			this.buffer = new SplineParser.SplineDefinition(name, Spline.Type.Bezier);
			Vector2[] array = base.ParseVector2(coords);
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i];
				if (relative)
				{
					this.buffer.position += vector;
				}
				else
				{
					this.buffer.position = vector;
				}
				this.buffer.CreateLinear();
			}
		}

		// Token: 0x060041F7 RID: 16887 RVA: 0x001F6F1C File Offset: 0x001F511C
		private void PathClose()
		{
			this.buffer.closed = true;
		}

		// Token: 0x060041F8 RID: 16888 RVA: 0x001F6F2C File Offset: 0x001F512C
		private void PathLineTo(string coords, bool relative)
		{
			Vector2[] array = base.ParseVector2(coords);
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i];
				if (relative)
				{
					this.buffer.position += vector;
				}
				else
				{
					this.buffer.position = vector;
				}
				this.buffer.CreateLinear();
			}
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x001F6F90 File Offset: 0x001F5190
		private void PathHorizontalLineTo(string coords, bool relative)
		{
			foreach (float num in base.ParseFloat(coords))
			{
				if (relative)
				{
					SplineParser.SplineDefinition buffer = this.buffer;
					buffer.position.x = buffer.position.x + num;
				}
				else
				{
					this.buffer.position.x = num;
				}
				this.buffer.CreateLinear();
			}
		}

		// Token: 0x060041FA RID: 16890 RVA: 0x001F6FF0 File Offset: 0x001F51F0
		private void PathVerticalLineTo(string coords, bool relative)
		{
			foreach (float num in base.ParseFloat(coords))
			{
				if (relative)
				{
					SplineParser.SplineDefinition buffer = this.buffer;
					buffer.position.y = buffer.position.y - num;
				}
				else
				{
					this.buffer.position.y = -num;
				}
				this.buffer.CreateLinear();
			}
		}

		// Token: 0x060041FB RID: 16891 RVA: 0x001F7050 File Offset: 0x001F5250
		private void PathCurveTo(string coords, SVG.PathSegment.Type type, bool relative)
		{
			SVG.PathSegment[] array = this.ParsePathSegment(coords, type);
			for (int i = 0; i < array.Length; i++)
			{
				SplinePoint lastPoint = this.buffer.GetLastPoint();
				lastPoint.type = SplinePoint.Type.Broken;
				Vector3 position = lastPoint.position;
				Vector3 endPoint = array[i].endPoint;
				Vector3 vector = array[i].startTangent;
				Vector3 vector2 = array[i].endTangent;
				switch (type)
				{
				case SVG.PathSegment.Type.CubicShort:
					vector = position - lastPoint.tangent;
					break;
				case SVG.PathSegment.Type.Quadratic:
					this.buffer.tangent = array[i].startTangent;
					vector = position + 0.6666667f * (this.buffer.tangent - position);
					vector2 = endPoint + 0.6666667f * (this.buffer.tangent - endPoint);
					break;
				case SVG.PathSegment.Type.QuadraticShort:
				{
					Vector3 a = position + (position - this.buffer.tangent);
					vector = position + 0.6666667f * (a - position);
					vector2 = endPoint + 0.6666667f * (a - endPoint);
					break;
				}
				}
				if (type == SVG.PathSegment.Type.CubicShort || type == SVG.PathSegment.Type.QuadraticShort)
				{
					lastPoint.type = SplinePoint.Type.SmoothMirrored;
				}
				else if (relative)
				{
					lastPoint.SetTangent2Position(position + vector);
				}
				else
				{
					lastPoint.SetTangent2Position(vector);
				}
				this.buffer.SetLastPoint(lastPoint);
				if (relative)
				{
					this.buffer.position += endPoint;
					this.buffer.tangent = position + vector2;
				}
				else
				{
					this.buffer.position = endPoint;
					this.buffer.tangent = vector2;
				}
				this.buffer.CreateBroken();
			}
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x001F721B File Offset: 0x001F541B
		private void WriteBufferTo(List<SplineParser.SplineDefinition> list)
		{
			this.buffer.Transform(this.transformBuffer);
			list.Add(this.buffer);
			this.buffer = null;
		}

		// Token: 0x060041FD RID: 16893 RVA: 0x001F7244 File Offset: 0x001F5444
		private SVG.PathSegment[] ParsePathSegment(string coord, SVG.PathSegment.Type type)
		{
			List<float> list = base.ParseFloatArray(coord.Substring(1));
			int num = 0;
			switch (type)
			{
			case SVG.PathSegment.Type.Cubic:
				num = list.Count / 6;
				break;
			case SVG.PathSegment.Type.CubicShort:
				num = list.Count / 4;
				break;
			case SVG.PathSegment.Type.Quadratic:
				num = list.Count / 4;
				break;
			case SVG.PathSegment.Type.QuadraticShort:
				num = list.Count / 2;
				break;
			}
			if (num == 0)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Error in ",
					coord,
					" ",
					type
				}));
				return new SVG.PathSegment[]
				{
					new SVG.PathSegment()
				};
			}
			SVG.PathSegment[] array = new SVG.PathSegment[num];
			for (int i = 0; i < num; i++)
			{
				switch (type)
				{
				case SVG.PathSegment.Type.Cubic:
					array[i] = new SVG.PathSegment(new Vector2(list[6 * i], -list[1 + 6 * i]), new Vector2(list[2 + 6 * i], -list[3 + 6 * i]), new Vector2(list[4 + 6 * i], -list[5 + 6 * i]));
					break;
				case SVG.PathSegment.Type.CubicShort:
					array[i] = new SVG.PathSegment(Vector2.zero, new Vector2(list[4 * i], -list[1 + 4 * i]), new Vector2(list[2 + 4 * i], -list[3 + 4 * i]));
					break;
				case SVG.PathSegment.Type.Quadratic:
					array[i] = new SVG.PathSegment(new Vector2(list[4 * i], -list[1 + 4 * i]), Vector2.zero, new Vector2(list[2 + 4 * i], -list[3 + 4 * i]));
					break;
				case SVG.PathSegment.Type.QuadraticShort:
					array[i] = new SVG.PathSegment(Vector2.zero, Vector2.zero, new Vector2(list[4 * i], -list[1 + 4 * i]));
					break;
				}
			}
			return array;
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x001F742C File Offset: 0x001F562C
		private string EncodePath(SplineParser.SplineDefinition definition, SVG.Axis ax)
		{
			string text = "M";
			for (int i = 0; i < definition.pointCount; i++)
			{
				SplinePoint splinePoint = definition.points[i];
				Vector3 vector = this.MapPoint(splinePoint.tangent, ax);
				Vector3 vector2 = this.MapPoint(splinePoint.position, ax);
				if (i == 0)
				{
					text = string.Concat(new object[]
					{
						text,
						vector2.x,
						",",
						vector2.y
					});
				}
				else
				{
					SplinePoint splinePoint2 = definition.points[i - 1];
					Vector3 vector3 = this.MapPoint(splinePoint2.tangent2, ax);
					text = string.Concat(new object[]
					{
						text,
						"C",
						vector3.x,
						",",
						vector3.y,
						",",
						vector.x,
						",",
						vector.y,
						",",
						vector2.x,
						",",
						vector2.y
					});
				}
			}
			if (definition.closed)
			{
				text += "z";
			}
			return text;
		}

		// Token: 0x060041FF RID: 16895 RVA: 0x001F75A0 File Offset: 0x001F57A0
		private string EncodePolygon(SplineParser.SplineDefinition definition, SVG.Axis ax)
		{
			string text = "";
			for (int i = 0; i < definition.pointCount; i++)
			{
				Vector3 vector = this.MapPoint(definition.points[i].position, ax);
				if (text != "")
				{
					text += ",";
				}
				text = string.Concat(new object[]
				{
					text,
					vector.x,
					",",
					vector.y
				});
			}
			return text;
		}

		// Token: 0x06004200 RID: 16896 RVA: 0x001F7630 File Offset: 0x001F5830
		private string GetAttributeContent(XmlNode node, string attributeName)
		{
			for (int i = 0; i < node.Attributes.Count; i++)
			{
				if (node.Attributes[i].Name == attributeName)
				{
					return node.Attributes[i].InnerText;
				}
			}
			return "ERROR";
		}

		// Token: 0x04002DDF RID: 11743
		private List<SplineParser.SplineDefinition> paths = new List<SplineParser.SplineDefinition>();

		// Token: 0x04002DE0 RID: 11744
		private List<SplineParser.SplineDefinition> polygons = new List<SplineParser.SplineDefinition>();

		// Token: 0x04002DE1 RID: 11745
		private List<SplineParser.SplineDefinition> ellipses = new List<SplineParser.SplineDefinition>();

		// Token: 0x04002DE2 RID: 11746
		private List<SplineParser.SplineDefinition> rectangles = new List<SplineParser.SplineDefinition>();

		// Token: 0x04002DE3 RID: 11747
		private List<SplineParser.SplineDefinition> lines = new List<SplineParser.SplineDefinition>();

		// Token: 0x04002DE4 RID: 11748
		private List<SplineParser.Transformation> transformBuffer = new List<SplineParser.Transformation>();

		// Token: 0x020009B7 RID: 2487
		public enum Axis
		{
			// Token: 0x0400453A RID: 17722
			X,
			// Token: 0x0400453B RID: 17723
			Y,
			// Token: 0x0400453C RID: 17724
			Z
		}

		// Token: 0x020009B8 RID: 2488
		internal class PathSegment
		{
			// Token: 0x060054BB RID: 21691 RVA: 0x002473E4 File Offset: 0x002455E4
			internal PathSegment(Vector2 s, Vector2 e, Vector2 c)
			{
				this.startTangent = s;
				this.endTangent = e;
				this.endPoint = c;
			}

			// Token: 0x060054BC RID: 21692 RVA: 0x0024743C File Offset: 0x0024563C
			internal PathSegment()
			{
			}

			// Token: 0x0400453D RID: 17725
			internal Vector3 startTangent = Vector3.zero;

			// Token: 0x0400453E RID: 17726
			internal Vector3 endTangent = Vector3.zero;

			// Token: 0x0400453F RID: 17727
			internal Vector3 endPoint = Vector3.zero;

			// Token: 0x02000A4D RID: 2637
			internal enum Type
			{
				// Token: 0x04004748 RID: 18248
				Cubic,
				// Token: 0x04004749 RID: 18249
				CubicShort,
				// Token: 0x0400474A RID: 18250
				Quadratic,
				// Token: 0x0400474B RID: 18251
				QuadraticShort
			}
		}

		// Token: 0x020009B9 RID: 2489
		public enum Element
		{
			// Token: 0x04004541 RID: 17729
			All,
			// Token: 0x04004542 RID: 17730
			Path,
			// Token: 0x04004543 RID: 17731
			Polygon,
			// Token: 0x04004544 RID: 17732
			Ellipse,
			// Token: 0x04004545 RID: 17733
			Rectangle,
			// Token: 0x04004546 RID: 17734
			Line
		}
	}
}
