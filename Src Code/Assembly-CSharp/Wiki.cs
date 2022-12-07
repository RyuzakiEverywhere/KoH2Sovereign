using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class Wiki
{
	// Token: 0x060006ED RID: 1773 RVA: 0x00048449 File Offset: 0x00046649
	public static Wiki Get()
	{
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		return defs.wiki;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x0004845C File Offset: 0x0004665C
	public Wiki()
	{
		this.parser = new Wiki.Parser(this);
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x000484C8 File Offset: 0x000466C8
	public void Clear()
	{
		this.text_files.Clear();
		this.texts.Clear();
		this.def_files.Clear();
		this.articles_in_order.Clear();
		this.articles_by_name.Clear();
		this.categories.Clear();
		this.keywords.Clear();
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00048524 File Offset: 0x00046724
	public void ExtractTexts(string text, string filename)
	{
		DT.Field field = new DT.Field(null);
		field.type = "file";
		field.key = Path.GetFileName(filename);
		field.value_str = DT.Enquote(filename);
		field.children = new List<DT.Field>();
		this.text_files.Add(field);
		this.parser.ExtractTexts(text, filename, field);
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00048580 File Offset: 0x00046780
	public void Parse(string text, string filename, int line = 1)
	{
		this.defs_root = null;
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return;
		}
		this.dt = defs.dt;
		if (this.dt == null)
		{
			return;
		}
		string text2 = filename + ".def";
		DT.Field field;
		if (!this.def_files.TryGetValue(text2, out field))
		{
			field = new DT.Field(this.dt);
			field.type = "file";
			field.key = Path.GetFileName(text2);
			field.value_str = DT.Enquote(text2);
			field.children = new List<DT.Field>();
			this.dt.files.Add(field);
			this.def_files.Add(text2, field);
		}
		this.defs_root = field.FindChild("wiki", null, true, true, true, '.');
		if (this.defs_root == null)
		{
			this.defs_root = field.AddChild("wiki");
			this.defs_root.type = "extend";
		}
		this.parser.Parse(text, filename, line);
		this.defs_root = null;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00048684 File Offset: 0x00046884
	public Wiki.Article FindArticle(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		Wiki.Article result;
		if (!this.articles_by_name.TryGetValue(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x000486B0 File Offset: 0x000468B0
	public void AddArticle(Wiki.Article article)
	{
		Wiki.Article article2 = this.FindArticle(article.id);
		if (article2 != null)
		{
			int num = global::Defs.ParseTextVersion(article2.location.filename);
			if (global::Defs.ParseTextVersion(article.location.filename) <= num)
			{
				Debug.LogError(string.Format("Duplicated article id '{0}': {1}\n{2}", article.id, article.location, article2.location));
			}
			int num2 = this.articles_in_order.IndexOf(article2);
			if (num2 >= 0)
			{
				this.articles_in_order[num2] = article;
			}
			else
			{
				this.articles_in_order.Add(article);
			}
		}
		else
		{
			this.articles_in_order.Add(article);
		}
		this.articles_by_name[article.id] = article;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00048768 File Offset: 0x00046968
	public void OnTextsLoaded()
	{
		foreach (KeyValuePair<string, global::Defs.TextInfo> keyValuePair in this.texts)
		{
			DT.Field textField = keyValuePair.Value.GetTextField(null, false);
			if (textField != null)
			{
				string text = textField.String(null, "");
				string filename = textField.FilePath();
				this.Parse(text, filename, textField.line);
			}
		}
		for (int i = 0; i < this.articles_in_order.Count; i++)
		{
			Wiki.Article article = this.articles_in_order[i];
			if (article.categories != null)
			{
				for (int j = 0; j < article.categories.Count; j++)
				{
					Wiki.Category category = article.categories[j];
					List<Wiki.Category> list;
					if (!this.categories.TryGetValue(category.category, out list))
					{
						list = new List<Wiki.Category>();
						this.categories.Add(category.category, list);
					}
					list.Add(category);
				}
			}
			if (article.keywords != null)
			{
				for (int k = 0; k < article.keywords.Count; k++)
				{
					string keyword = article.keywords[k];
					Wiki.Keyword keyword2 = this.GetKeyword(keyword, true);
					if (keyword2 != null)
					{
						keyword2.articles.Add(article);
					}
				}
			}
			if (article.parent_field != null)
			{
				article.CreateDefField(this);
			}
		}
		foreach (KeyValuePair<string, DT.Field> keyValuePair2 in this.def_files)
		{
			DT.Field value = keyValuePair2.Value;
			this.dt.PostProcessFile(value);
		}
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00048940 File Offset: 0x00046B40
	public List<Wiki.Keyword> GetKeywords(char first_char, bool create = false)
	{
		first_char = char.ToLowerInvariant(first_char);
		List<Wiki.Keyword> list;
		if (!this.keywords.TryGetValue(first_char, out list) && create)
		{
			list = new List<Wiki.Keyword>();
			this.keywords.Add(first_char, list);
		}
		return list;
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x0004897C File Offset: 0x00046B7C
	public Wiki.Keyword GetKeyword(string keyword, bool create = false)
	{
		if (string.IsNullOrEmpty(keyword))
		{
			return null;
		}
		List<Wiki.Keyword> list = this.GetKeywords(keyword[0], create);
		if (list == null)
		{
			return null;
		}
		int i;
		Wiki.Keyword keyword2;
		for (i = 0; i < list.Count; i++)
		{
			keyword2 = list[i];
			if (keyword2.keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase))
			{
				return keyword2;
			}
			if (keyword2.keyword.Length < keyword.Length)
			{
				break;
			}
		}
		if (!create)
		{
			return null;
		}
		keyword2 = new Wiki.Keyword
		{
			keyword = keyword
		};
		if (i >= list.Count)
		{
			list.Add(keyword2);
		}
		else
		{
			list.Insert(i, keyword2);
		}
		return keyword2;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00048A14 File Offset: 0x00046C14
	public static Value PopulateHistory(UIHyperText.CallbackParams arg)
	{
		Wiki.reversed_history.Clear();
		for (int i = UIWikiWindow.history.Count - 1; i >= 0; i--)
		{
			UIWikiWindow.HistoryItem item = UIWikiWindow.history[i];
			Wiki.reversed_history.Add(item);
		}
		return new Value(Wiki.reversed_history);
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00048A64 File Offset: 0x00046C64
	public static Value PopulateCategory(UIHyperText.CallbackParams arg)
	{
		string @string = arg.def.GetString("category", arg, "", true, true, true, '.');
		if (string.IsNullOrEmpty(@string))
		{
			return Value.Unknown;
		}
		Wiki wiki = Wiki.Get();
		if (wiki == null)
		{
			return Value.Unknown;
		}
		List<Wiki.Category> val;
		if (!wiki.categories.TryGetValue(@string, out val))
		{
			return Value.Unknown;
		}
		return new Value(val);
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00048ACC File Offset: 0x00046CCC
	public static Value PopulateKeyword(UIHyperText.CallbackParams arg)
	{
		string @string = arg.def.GetString("keyword", arg, "", true, true, true, '.');
		if (string.IsNullOrEmpty(@string))
		{
			return Value.Unknown;
		}
		Wiki wiki = Wiki.Get();
		if (wiki == null)
		{
			return Value.Unknown;
		}
		Wiki.Keyword keyword = wiki.GetKeyword(@string, false);
		if (keyword == null)
		{
			return Value.Unknown;
		}
		return new Value(keyword.articles);
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00048B34 File Offset: 0x00046D34
	public static Value PopulateDefs(UIHyperText.CallbackParams arg)
	{
		string @string = arg.def.GetString("type", arg, "", true, true, true, '.');
		if (string.IsNullOrEmpty(@string))
		{
			return Value.Unknown;
		}
		DT.Field defField = global::Defs.GetDefField(@string, null);
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.defs : null) != null);
		}
		if (!flag)
		{
			return Value.Unknown;
		}
		bool @bool = arg.def.GetBool("hide_unnamed", null, false, true, true, true, '.');
		string path = null;
		if (@bool)
		{
			if (!(@string == "Unit"))
			{
				if (@string == "Settlement")
				{
					path = "description";
				}
			}
			else
			{
				path = "name";
			}
		}
		List<Value> list = new List<Value>(defField.def.defs.Count);
		int i = 0;
		while (i < defField.def.defs.Count)
		{
			DT.Def def2 = defField.def.defs[i];
			if (!@bool || def2.field == null)
			{
				goto IL_104;
			}
			DT.Field field = def2.field.FindChild(path, null, false, true, true, '.');
			if (field != null && !string.IsNullOrEmpty(field.value_str))
			{
				goto IL_104;
			}
			IL_143:
			i++;
			continue;
			IL_104:
			if ((def2.field == null || !def2.field.GetBool("hide_in_wiki", null, false, true, true, true, '.')) && def2.def != null)
			{
				list.Add(def2.def);
				goto IL_143;
			}
			goto IL_143;
		}
		return new Value(list);
	}

	// Token: 0x040005FD RID: 1533
	public List<DT.Field> text_files = new List<DT.Field>();

	// Token: 0x040005FE RID: 1534
	public Dictionary<string, global::Defs.TextInfo> texts = new Dictionary<string, global::Defs.TextInfo>();

	// Token: 0x040005FF RID: 1535
	public Dictionary<string, DT.Field> def_files = new Dictionary<string, DT.Field>();

	// Token: 0x04000600 RID: 1536
	public List<Wiki.Article> articles_in_order = new List<Wiki.Article>();

	// Token: 0x04000601 RID: 1537
	private Dictionary<string, Wiki.Article> articles_by_name = new Dictionary<string, Wiki.Article>();

	// Token: 0x04000602 RID: 1538
	private Dictionary<string, List<Wiki.Category>> categories = new Dictionary<string, List<Wiki.Category>>();

	// Token: 0x04000603 RID: 1539
	private Dictionary<char, List<Wiki.Keyword>> keywords = new Dictionary<char, List<Wiki.Keyword>>();

	// Token: 0x04000604 RID: 1540
	private Wiki.Parser parser;

	// Token: 0x04000605 RID: 1541
	public DT dt;

	// Token: 0x04000606 RID: 1542
	public DT.Field defs_root;

	// Token: 0x04000607 RID: 1543
	private static List<UIWikiWindow.HistoryItem> reversed_history = new List<UIWikiWindow.HistoryItem>();

	// Token: 0x02000584 RID: 1412
	public class Article : IVars
	{
		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06004404 RID: 17412 RVA: 0x001FFE9D File Offset: 0x001FE09D
		public bool is_TOC
		{
			get
			{
				return this.toc_descr_id != null;
			}
		}

		// Token: 0x06004405 RID: 17413 RVA: 0x001FFEA8 File Offset: 0x001FE0A8
		public override string ToString()
		{
			return string.Format("Wiki article '{0}' at {1}", this.id, this.location);
		}

		// Token: 0x06004406 RID: 17414 RVA: 0x001FFEC8 File Offset: 0x001FE0C8
		public DT.Field CreateDefField(Wiki wiki)
		{
			this.def_field = new DT.Field(wiki.dt);
			this.def_field.type = "def";
			this.def_field.key = this.id;
			this.def_field.base_path = "WikiArticle";
			this.def_field.line = this.location.line;
			this.parent_field.AddChild(this.def_field);
			if (this.caption != null)
			{
				this.caption_field = this.def_field.SetValue("caption", DT.Enquote(this.caption), this.caption);
				this.caption_field.type = "text";
				if (!global::Defs.IsDevLanguage() && global::Defs.IsPlaying())
				{
					this.caption_field.dt = null;
					global::Defs.Get(false).AddText("wiki." + this.id + ".caption", this.caption_field, null, true);
				}
			}
			if (this.body != null)
			{
				this.ht_def_field = this.body.CreateDefField(wiki, this, this.def_field);
			}
			return this.def_field;
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06004407 RID: 17415 RVA: 0x001FFFE7 File Offset: 0x001FE1E7
		public string def_id
		{
			get
			{
				DT.Field field = this.def_field;
				if (field == null)
				{
					return null;
				}
				DT.Def def = field.def;
				if (def == null)
				{
					return null;
				}
				return def.path;
			}
		}

		// Token: 0x06004408 RID: 17416 RVA: 0x00200008 File Offset: 0x001FE208
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			if (key == "article")
			{
				return new Value(this);
			}
			if (!(key == "selected"))
			{
				return Value.Unknown;
			}
			UIWikiWindow uiwikiWindow = UIWikiWindow.Get();
			return ((uiwikiWindow != null) ? uiwikiWindow.ArticleID : null) == this.id;
		}

		// Token: 0x040030AD RID: 12461
		public string id;

		// Token: 0x040030AE RID: 12462
		public string caption;

		// Token: 0x040030AF RID: 12463
		public string toc_descr_id;

		// Token: 0x040030B0 RID: 12464
		public string context;

		// Token: 0x040030B1 RID: 12465
		public List<string> keywords;

		// Token: 0x040030B2 RID: 12466
		public List<Wiki.Category> categories;

		// Token: 0x040030B3 RID: 12467
		public Wiki.Table body;

		// Token: 0x040030B4 RID: 12468
		public DT.Field parent_field;

		// Token: 0x040030B5 RID: 12469
		public DT.Field def_field;

		// Token: 0x040030B6 RID: 12470
		public DT.Field caption_field;

		// Token: 0x040030B7 RID: 12471
		public DT.Field ht_def_field;

		// Token: 0x040030B8 RID: 12472
		public Wiki.Location location;
	}

	// Token: 0x02000585 RID: 1413
	public class Element
	{
		// Token: 0x0600440A RID: 17418 RVA: 0x0020005F File Offset: 0x001FE25F
		public Element(string type, Wiki.Location location, Wiki.Row parent_row)
		{
			this.parent_row = parent_row;
			this.ht_type = type;
			this.location = location;
			if (parent_row != null)
			{
				parent_row.elements.Add(this);
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x0600440B RID: 17419 RVA: 0x0020008B File Offset: 0x001FE28B
		public virtual string Path
		{
			get
			{
				string format = "{0}.{1}{2}";
				Wiki.Row row = this.parent_row;
				return string.Format(format, (row != null) ? row.Path : null, this.ht_type, this.location.line);
			}
		}

		// Token: 0x0600440C RID: 17420 RVA: 0x002000BF File Offset: 0x001FE2BF
		public override string ToString()
		{
			return string.Format("Wiki {0} at {1}", this.ht_type, this.location);
		}

		// Token: 0x0600440D RID: 17421 RVA: 0x002000DC File Offset: 0x001FE2DC
		public virtual void AddProperty(string key, string value)
		{
			if (key == "based_on")
			{
				if (this.based_on != null)
				{
					Game.Log(string.Format("{0}: Duplicated property '{1}'", this.location, key), Game.LogType.Error);
				}
				this.based_on = value;
				return;
			}
			string text = null;
			if (key.EndsWith(".based_on", StringComparison.Ordinal))
			{
				text = value;
				value = null;
				key = key.Substring(0, key.Length - 9);
			}
			if (this.properties == null)
			{
				this.properties = new List<Wiki.Property>();
			}
			int i = 0;
			while (i < this.properties.Count)
			{
				Wiki.Property property = this.properties[i];
				if (!(property.key != key))
				{
					if (text != null)
					{
						if (property.based_on != null)
						{
							Game.Log(string.Format("{0}: Duplicated property '{1}.based_on'", this.location, key), Game.LogType.Error);
						}
						property.based_on = text;
						return;
					}
					Game.Log(string.Format("{0}: Duplicated property '{1}'", this.location, key), Game.LogType.Error);
					property.value = value;
					this.properties[i] = property;
					return;
				}
				else
				{
					i++;
				}
			}
			this.properties.Add(new Wiki.Property
			{
				key = key,
				based_on = text,
				value = value
			});
		}

		// Token: 0x0600440E RID: 17422 RVA: 0x00200224 File Offset: 0x001FE424
		public virtual DT.Field CreateDefField(Wiki wiki, Wiki.Article article, DT.Field parent)
		{
			DT.Field field = new DT.Field(wiki.dt);
			field.type = this.ht_type;
			field.key = string.Format("{0}{1}", this.ht_type, this.location.line);
			field.line = this.location.line;
			if (this.based_on != null)
			{
				field.base_path = this.based_on;
			}
			if (parent != null)
			{
				parent.AddChild(field);
			}
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Count; i++)
				{
					Wiki.Property property = this.properties[i];
					DT.Field field2 = field.CreateChild(property.key, '.');
					if (property.based_on != null)
					{
						field2.base_path = property.based_on;
					}
					if (property.value != null)
					{
						field2.value_str = property.value;
					}
				}
			}
			return field;
		}

		// Token: 0x040030B9 RID: 12473
		public Wiki.Row parent_row;

		// Token: 0x040030BA RID: 12474
		public string ht_type;

		// Token: 0x040030BB RID: 12475
		public string based_on;

		// Token: 0x040030BC RID: 12476
		public List<Wiki.Property> properties;

		// Token: 0x040030BD RID: 12477
		public Wiki.Location location;
	}

	// Token: 0x02000586 RID: 1414
	public class Table : Wiki.Element
	{
		// Token: 0x0600440F RID: 17423 RVA: 0x00200301 File Offset: 0x001FE501
		public Table(Wiki.Location location, Wiki.Article parent_article) : base("def", location, null)
		{
			this.parent_article = parent_article;
		}

		// Token: 0x06004410 RID: 17424 RVA: 0x00200322 File Offset: 0x001FE522
		public Table(Wiki.Location location, Wiki.Row parent_row) : base("hypertext", location, parent_row)
		{
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06004411 RID: 17425 RVA: 0x0020033C File Offset: 0x001FE53C
		public override string Path
		{
			get
			{
				Wiki.Row parent_row = this.parent_row;
				string arg = ((parent_row != null) ? parent_row.Path : null) ?? this.parent_article.id;
				return string.Format("{0}.{1}{2}", arg, this.ht_type, this.location.line);
			}
		}

		// Token: 0x06004412 RID: 17426 RVA: 0x0020038C File Offset: 0x001FE58C
		public override string ToString()
		{
			return string.Format("Wiki {0} at {1}", this.ht_type, this.location);
		}

		// Token: 0x06004413 RID: 17427 RVA: 0x002003AC File Offset: 0x001FE5AC
		public override DT.Field CreateDefField(Wiki wiki, Wiki.Article article, DT.Field parent)
		{
			DT.Field field = base.CreateDefField(wiki, article, parent);
			if (field == null)
			{
				return null;
			}
			if (this.parent_article != null)
			{
				field.key = "body";
				field.base_path = "WikiHypertext";
			}
			for (int i = 0; i < this.rows.Count; i++)
			{
				this.rows[i].CreateDefField(wiki, article, field);
			}
			return field;
		}

		// Token: 0x040030BE RID: 12478
		public Wiki.Article parent_article;

		// Token: 0x040030BF RID: 12479
		public List<Wiki.Row> rows = new List<Wiki.Row>();
	}

	// Token: 0x02000587 RID: 1415
	public class Row : Wiki.Element
	{
		// Token: 0x06004414 RID: 17428 RVA: 0x00200412 File Offset: 0x001FE612
		public Row(Wiki.Table parent_table, Wiki.Location location, string type = "row") : base(type, location, null)
		{
			this.parent_table = parent_table;
			if (parent_table != null)
			{
				parent_table.rows.Add(this);
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06004415 RID: 17429 RVA: 0x0020043E File Offset: 0x001FE63E
		public override string Path
		{
			get
			{
				string format = "{0}.{1}{2}";
				Wiki.Table table = this.parent_table;
				return string.Format(format, (table != null) ? table.Path : null, this.ht_type, this.location.line);
			}
		}

		// Token: 0x06004416 RID: 17430 RVA: 0x00200474 File Offset: 0x001FE674
		public override DT.Field CreateDefField(Wiki wiki, Wiki.Article article, DT.Field parent)
		{
			DT.Field field = base.CreateDefField(wiki, article, parent);
			for (int i = 0; i < this.elements.Count; i++)
			{
				this.elements[i].CreateDefField(wiki, article, field);
			}
			return field;
		}

		// Token: 0x040030C0 RID: 12480
		public Wiki.Table parent_table;

		// Token: 0x040030C1 RID: 12481
		public List<Wiki.Element> elements = new List<Wiki.Element>();
	}

	// Token: 0x02000588 RID: 1416
	public class Rows : Wiki.Row
	{
		// Token: 0x06004417 RID: 17431 RVA: 0x002004B7 File Offset: 0x001FE6B7
		public Rows(Wiki.Table parent_table, Wiki.Location location) : base(parent_table, location, "rows")
		{
		}
	}

	// Token: 0x02000589 RID: 1417
	public class Text : Wiki.Element
	{
		// Token: 0x06004418 RID: 17432 RVA: 0x002004C6 File Offset: 0x001FE6C6
		public Text(Wiki.Location location, Wiki.Row parent_row) : base("text_field", location, parent_row)
		{
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x002004D8 File Offset: 0x001FE6D8
		public override DT.Field CreateDefField(Wiki wiki, Wiki.Article article, DT.Field parent)
		{
			DT.Field field = base.CreateDefField(wiki, article, parent);
			DT.Field field2 = field.AddChild("text");
			field2.type = "text";
			field2.value_str = "\"" + this.text + "\"";
			field2.line = field.line;
			if (!global::Defs.IsDevLanguage() && global::Defs.IsPlaying())
			{
				field2.dt = null;
				global::Defs defs = global::Defs.Get(false);
				if (defs != null)
				{
					defs.AddText(field2.Path(false, false, '.'), field2, null, true);
				}
			}
			return field;
		}

		// Token: 0x040030C2 RID: 12482
		public string text;
	}

	// Token: 0x0200058A RID: 1418
	public class Image : Wiki.Element
	{
		// Token: 0x0600441A RID: 17434 RVA: 0x00200562 File Offset: 0x001FE762
		public Image(Wiki.Location location, Wiki.Row parent_row) : base("sprite", location, parent_row)
		{
		}

		// Token: 0x0600441B RID: 17435 RVA: 0x00200574 File Offset: 0x001FE774
		public override void AddProperty(string key, string value)
		{
			if (key == "asset")
			{
				if (this.asset_path != null)
				{
					Game.Log(string.Format("{0}: Duplicated property '{1}'", this.location, key), Game.LogType.Error);
				}
				this.asset_path = value;
				return;
			}
			base.AddProperty(key, value);
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x002005C2 File Offset: 0x001FE7C2
		public override DT.Field CreateDefField(Wiki wiki, Wiki.Article article, DT.Field parent)
		{
			DT.Field field = base.CreateDefField(wiki, article, parent);
			field.value_str = this.asset_path;
			return field;
		}

		// Token: 0x040030C3 RID: 12483
		public string asset_path;
	}

	// Token: 0x0200058B RID: 1419
	public class Space : Wiki.Element
	{
		// Token: 0x0600441D RID: 17437 RVA: 0x002005D9 File Offset: 0x001FE7D9
		public Space(Wiki.Location location, Wiki.Row parent_row) : base("space", location, parent_row)
		{
		}
	}

	// Token: 0x0200058C RID: 1420
	public class Separator : Wiki.Row
	{
		// Token: 0x0600441E RID: 17438 RVA: 0x002005E8 File Offset: 0x001FE7E8
		public Separator(Wiki.Table parent_table, Wiki.Location location) : base(parent_table, location, "separator")
		{
		}
	}

	// Token: 0x0200058D RID: 1421
	public class Category : IVars
	{
		// Token: 0x0600441F RID: 17439 RVA: 0x002005F8 File Offset: 0x001FE7F8
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			if (key == "article")
			{
				return new Value(this.article);
			}
			if (!(key == "link_text"))
			{
				if (key == "link_open")
				{
					return "@{wiki." + this.article.id + ":link}";
				}
				if (!(key == "link_close"))
				{
					return Value.Unknown;
				}
				return "@{wiki." + this.article.id + ":/link}";
			}
			else
			{
				if (string.IsNullOrEmpty(this.link_text))
				{
					return new Value(this.article.caption_field);
				}
				return this.link_text;
			}
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x002006B6 File Offset: 0x001FE8B6
		public override string ToString()
		{
			return string.Format("[{0}, {1}] '{2}'", this.category, this.article, this.link_text);
		}

		// Token: 0x040030C4 RID: 12484
		public string category;

		// Token: 0x040030C5 RID: 12485
		public Wiki.Article article;

		// Token: 0x040030C6 RID: 12486
		public string link_text;
	}

	// Token: 0x0200058E RID: 1422
	public class Keyword
	{
		// Token: 0x06004422 RID: 17442 RVA: 0x002006D4 File Offset: 0x001FE8D4
		public override string ToString()
		{
			return string.Format("keyword '{0}' -> {1} article(s)", this.keyword, this.articles.Count);
		}

		// Token: 0x040030C7 RID: 12487
		public string keyword;

		// Token: 0x040030C8 RID: 12488
		public List<Wiki.Article> articles = new List<Wiki.Article>();
	}

	// Token: 0x0200058F RID: 1423
	public struct Property
	{
		// Token: 0x06004424 RID: 17444 RVA: 0x0020070C File Offset: 0x001FE90C
		public override string ToString()
		{
			string str = string.IsNullOrEmpty(this.based_on) ? "" : (" : " + this.based_on);
			return this.key + str + " = " + this.value;
		}

		// Token: 0x040030C9 RID: 12489
		public string key;

		// Token: 0x040030CA RID: 12490
		public string based_on;

		// Token: 0x040030CB RID: 12491
		public string value;
	}

	// Token: 0x02000590 RID: 1424
	public struct Location
	{
		// Token: 0x06004425 RID: 17445 RVA: 0x00200755 File Offset: 0x001FE955
		public Location(string filename, int line)
		{
			this.filename = filename;
			this.line = line;
		}

		// Token: 0x06004426 RID: 17446 RVA: 0x00200765 File Offset: 0x001FE965
		public Location(Wiki.Parser.Pos pos)
		{
			this.filename = pos.filename;
			this.line = pos.line;
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x0020077F File Offset: 0x001FE97F
		public override string ToString()
		{
			return string.Format("{0}({1})", this.filename, this.line);
		}

		// Token: 0x040030CC RID: 12492
		public string filename;

		// Token: 0x040030CD RID: 12493
		public int line;
	}

	// Token: 0x02000591 RID: 1425
	public class Parser
	{
		// Token: 0x06004428 RID: 17448 RVA: 0x0020079C File Offset: 0x001FE99C
		public Parser(Wiki wiki)
		{
			this.wiki = wiki;
		}

		// Token: 0x06004429 RID: 17449 RVA: 0x002007BB File Offset: 0x001FE9BB
		private void Warning(string msg)
		{
			this.Warning(msg, this.cur_pos);
		}

		// Token: 0x0600442A RID: 17450 RVA: 0x002007CA File Offset: 0x001FE9CA
		private void Error(string msg)
		{
			this.Error(msg, this.cur_pos);
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x002007D9 File Offset: 0x001FE9D9
		private void Warning(string msg, Wiki.Parser.Pos pos)
		{
			Game.Log(string.Format("{0}({1}): {2}: {3}", new object[]
			{
				pos.filename,
				pos.line,
				msg,
				pos.LineText()
			}), Game.LogType.Warning);
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x00200816 File Offset: 0x001FEA16
		private void Error(string msg, Wiki.Parser.Pos pos)
		{
			Game.Log(string.Format("{0}({1}): {2}: {3}", new object[]
			{
				pos.filename,
				pos.line,
				msg,
				pos.LineText()
			}), Game.LogType.Error);
		}

		// Token: 0x0600442D RID: 17453 RVA: 0x00200853 File Offset: 0x001FEA53
		private void Warning(string msg, Wiki.Location fl)
		{
			Game.Log(string.Format("{0}: {1}", fl, msg), Game.LogType.Warning);
		}

		// Token: 0x0600442E RID: 17454 RVA: 0x0020086C File Offset: 0x001FEA6C
		private void Error(string msg, Wiki.Location fl)
		{
			Game.Log(string.Format("{0}): {1}", fl, msg), Game.LogType.Error);
		}

		// Token: 0x0600442F RID: 17455 RVA: 0x00200888 File Offset: 0x001FEA88
		public void ExtractTexts(string text, string filename, DT.Field ff)
		{
			this.cur_pos.text = text;
			this.cur_pos.filename = filename;
			this.cur_pos.line = 1;
			this.cur_pos.line_idx = 0;
			this.cur_pos.idx = 0;
			if (text == null)
			{
				return;
			}
			string article_id = null;
			while (this.cur_pos.Valid)
			{
				this.cur_pos.SkipBlanks(true);
				if (this.cur_pos.Skip("#article", true))
				{
					int idx = this.cur_pos.idx;
					this.cur_pos.idx = this.cur_pos.line_idx;
					this.ExtractArticleText(article_id, ff);
					this.cur_article_pos = this.cur_pos;
					this.cur_pos.idx = idx;
					this.cur_pos.SkipBlanks(true);
					article_id = this.cur_pos.ReadIdentifier(true);
				}
				this.cur_pos.SkipLine(true);
			}
			this.ExtractArticleText(article_id, ff);
			this.ExtractTexts(null, null, null);
		}

		// Token: 0x06004430 RID: 17456 RVA: 0x00200988 File Offset: 0x001FEB88
		private void ExtractArticleText(string article_id, DT.Field ff)
		{
			if (string.IsNullOrEmpty(article_id))
			{
				return;
			}
			global::Defs defs = global::Defs.Get(false);
			string text = "article." + article_id;
			string text2 = this.cur_pos.text.Substring(this.cur_article_pos.idx, this.cur_pos.idx - this.cur_article_pos.idx).Trim();
			text2 = global::Defs.FixLigatures(text2, null, true, true);
			if (global::Defs.colorize_localized_texts > 0)
			{
				text2 = global::Defs.ColorizeLocalizedText(text2, "{L}", "{/L}", true);
			}
			DT.Field field = ff.AddChild(text);
			field.value_str = "#article " + article_id + " ...";
			field.value = text2;
			field.line = this.cur_article_pos.line;
			global::Defs.TextInfo textInfo;
			if (!this.wiki.texts.TryGetValue(text, out textInfo))
			{
				textInfo = global::Defs.GetTextInfo(text, true, false);
				this.wiki.texts.Add(text, textInfo);
			}
			defs.AddText(textInfo, field, null, true);
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x00200A80 File Offset: 0x001FEC80
		public void Parse(string text, string filename, int line = 1)
		{
			this.cur_pos.text = text;
			this.cur_pos.filename = filename;
			this.cur_pos.line = line;
			this.cur_pos.line_idx = 0;
			this.cur_pos.idx = 0;
			this.cur_article = null;
			this.cur_table = null;
			this.cur_row = null;
			this.cur_text_element = null;
			this.cur_text_text.Clear();
			this.append_new_line = false;
			if (text == null)
			{
				return;
			}
			while (this.cur_pos.Valid)
			{
				this.cur_pos.SkipBlanks(true);
				if (this.cur_pos.Skip("#", true))
				{
					this.FinishText();
					this.ParseDirective();
				}
				else
				{
					this.AccumulateText();
				}
				this.cur_pos.SkipLine(true);
			}
			this.FinishArticle();
			this.Parse(null, null, 1);
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x00200B5C File Offset: 0x001FED5C
		private void AccumulateText()
		{
			Wiki.Location location = new Wiki.Location(this.cur_pos);
			int idx = this.cur_pos.idx;
			if (!this.cur_pos.SkipToEndOfLine(true))
			{
				return;
			}
			if (this.cur_article == null)
			{
				this.Warning("Text outside article ignored");
				return;
			}
			if (this.cur_text_location.filename == null)
			{
				this.cur_text_location = location;
			}
			if (this.append_new_line)
			{
				this.cur_text_text.Append("{p}");
			}
			if (this.cur_pos.Char(-1) == '\\')
			{
				this.cur_text_text.Append(this.cur_pos.text, idx, this.cur_pos.idx - idx - 1);
				this.append_new_line = false;
				return;
			}
			this.cur_text_text.Append(this.cur_pos.text, idx, this.cur_pos.idx - idx);
			this.append_new_line = true;
		}

		// Token: 0x06004433 RID: 17459 RVA: 0x00200C40 File Offset: 0x001FEE40
		private void FinishText()
		{
			if (this.cur_text_text.Length != 0)
			{
				Wiki.Row row = this.EnsureInRow(this.cur_text_location);
				if (row != null)
				{
					Wiki.Text text = this.cur_text_element;
					if (text == null)
					{
						text = new Wiki.Text(this.cur_text_location, row);
					}
					text.text = this.cur_text_text.ToString().Replace("\"", "{q}");
				}
				this.cur_text_text.Clear();
			}
			this.append_new_line = false;
			this.cur_text_element = null;
			this.cur_text_location.filename = null;
		}

		// Token: 0x06004434 RID: 17460 RVA: 0x00200CC8 File Offset: 0x001FEEC8
		private void ParseDirective()
		{
			string text = this.cur_pos.ReadIdentifier(true);
			this.cur_pos.SkipBlanks(true);
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1251777503U)
			{
				if (num <= 474734565U)
				{
					if (num != 29848306U)
					{
						if (num != 392652633U)
						{
							if (num == 474734565U)
							{
								if (text == "TOC")
								{
									this.ParseTOC();
									return;
								}
							}
						}
						else if (text == "article")
						{
							this.ParseArticle();
							return;
						}
					}
					else if (text == "/table")
					{
						this.ParseEndTable();
						return;
					}
				}
				else if (num <= 894689925U)
				{
					if (num != 708293528U)
					{
						if (num == 894689925U)
						{
							if (text == "space")
							{
								this.ParseSpace();
								return;
							}
						}
					}
					else if (text == "rows")
					{
						this.ParseRows();
						return;
					}
				}
				else if (num != 1141775739U)
				{
					if (num == 1251777503U)
					{
						if (text == "table")
						{
							this.ParseTable();
							return;
						}
					}
				}
				else if (text == "row")
				{
					this.ParseRow();
					return;
				}
			}
			else if (num <= 3008443898U)
			{
				if (num != 1787359420U)
				{
					if (num != 2357661820U)
					{
						if (num == 3008443898U)
						{
							if (text == "image")
							{
								this.ParseImage();
								return;
							}
						}
					}
					else if (text == "context")
					{
						this.ParseContext();
						return;
					}
				}
				else if (text == "separator")
				{
					this.ParseSeparator();
					return;
				}
			}
			else if (num <= 3475980913U)
			{
				if (num != 3185987134U)
				{
					if (num == 3475980913U)
					{
						if (text == "category")
						{
							this.ParseCategory();
							return;
						}
					}
				}
				else if (text == "text")
				{
					this.ParseText();
					return;
				}
			}
			else if (num != 4011007077U)
			{
				if (num == 4210524501U)
				{
					if (text == "keywords")
					{
						this.ParseKeywords();
						return;
					}
				}
			}
			else if (text == "caption")
			{
				this.ParseCaption();
				return;
			}
			this.Error("Unknown directive '" + text + "'");
		}

		// Token: 0x06004435 RID: 17461 RVA: 0x00200F58 File Offset: 0x001FF158
		private void FinishArticle()
		{
			this.FinishText();
			Wiki.Article article = this.cur_article;
			if (((article != null) ? article.id : null) == null)
			{
				return;
			}
			if (this.cur_article.caption == null)
			{
				this.Warning("Article '" + this.cur_article.id + "' has no caption", this.cur_article.location);
			}
			this.wiki.AddArticle(this.cur_article);
			this.cur_article = null;
			this.cur_table = null;
			this.cur_row = null;
			this.cur_text_element = null;
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x00200FE8 File Offset: 0x001FF1E8
		private void ParseArticle()
		{
			int idx = this.cur_pos.idx;
			this.cur_pos.idx = this.cur_pos.line_idx;
			this.FinishArticle();
			this.cur_article_pos = this.cur_pos;
			this.cur_pos.idx = idx;
			string text = this.cur_pos.ReadIdentifier(true);
			if (string.IsNullOrEmpty(text))
			{
				this.Error("Missing article id");
				return;
			}
			this.cur_article = new Wiki.Article
			{
				id = text,
				location = new Wiki.Location(this.cur_pos),
				parent_field = this.wiki.defs_root
			};
		}

		// Token: 0x06004437 RID: 17463 RVA: 0x0020108C File Offset: 0x001FF28C
		private void ParseTOC()
		{
			if (this.cur_article == null)
			{
				this.Error("TOC outside article");
				return;
			}
			if (this.cur_article.toc_descr_id != null)
			{
				this.Error("Duplicated TOC");
			}
			this.cur_article.toc_descr_id = this.cur_pos.ReadIdentifier(true);
			if (this.cur_article.toc_descr_id == null)
			{
				this.cur_article.toc_descr_id = "";
			}
		}

		// Token: 0x06004438 RID: 17464 RVA: 0x002010FC File Offset: 0x001FF2FC
		private void ParseContext()
		{
			if (this.cur_article == null)
			{
				this.Error("context outside article");
				return;
			}
			if (this.cur_article.context != null)
			{
				this.Error("Duplicated context");
			}
			this.cur_article.context = this.cur_pos.ReadIdentifier(true);
			if (this.cur_article.context == null)
			{
				this.cur_article.context = "";
			}
		}

		// Token: 0x06004439 RID: 17465 RVA: 0x0020116C File Offset: 0x001FF36C
		private void ParseKeywords()
		{
			if (this.cur_article == null)
			{
				this.Error("keywords outside article");
				return;
			}
			for (;;)
			{
				string text = this.cur_pos.ReadUnitl(',', true, true);
				if (!string.IsNullOrEmpty(text))
				{
					text = global::Defs.FixRawTextLigatures(text, null);
					if (this.cur_article.keywords == null)
					{
						this.cur_article.keywords = new List<string>();
					}
					this.cur_article.keywords.Add(text);
				}
				if (this.cur_pos.Char(0) != ',')
				{
					break;
				}
				this.cur_pos.MoveNext(true);
				this.cur_pos.SkipBlanks(true);
			}
		}

		// Token: 0x0600443A RID: 17466 RVA: 0x00201208 File Offset: 0x001FF408
		private void ParseCategory()
		{
			if (this.cur_article == null)
			{
				this.Error("keywords outside article");
				return;
			}
			string text = this.cur_pos.ReadUnitl(':', true, true);
			if (string.IsNullOrEmpty(text))
			{
				this.Error("empty category");
				return;
			}
			if (this.cur_article.categories != null)
			{
				for (int i = 0; i < this.cur_article.categories.Count; i++)
				{
					if (this.cur_article.categories[i].category == text)
					{
						this.Error("duplicated category '" + text + "'");
						return;
					}
				}
			}
			string text2 = "";
			if (this.cur_pos.Char(0) == ':')
			{
				this.cur_pos.MoveNext(true);
				this.cur_pos.SkipBlanks(true);
				int idx = this.cur_pos.idx;
				if (this.cur_pos.SkipToEndOfLine(true))
				{
					text2 = this.cur_pos.text.Substring(idx, this.cur_pos.idx - idx).Replace("\"", "{q}");
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = "@" + text2;
			}
			if (this.cur_article.categories == null)
			{
				this.cur_article.categories = new List<Wiki.Category>();
			}
			this.cur_article.categories.Add(new Wiki.Category
			{
				category = text,
				article = this.cur_article,
				link_text = text2
			});
		}

		// Token: 0x0600443B RID: 17467 RVA: 0x00201384 File Offset: 0x001FF584
		private void ParseCaption()
		{
			if (this.cur_article == null)
			{
				this.Error("caption outside article");
				return;
			}
			if (this.cur_article.caption != null)
			{
				this.Warning("Overriding article caption '" + this.cur_article.caption + "'");
			}
			int idx = this.cur_pos.idx;
			if (!this.cur_pos.SkipToEndOfLine(true))
			{
				this.cur_article.caption = "";
				return;
			}
			this.cur_article.caption = this.cur_pos.text.Substring(idx, this.cur_pos.idx - idx).Replace("\"", "{q}");
		}

		// Token: 0x0600443C RID: 17468 RVA: 0x00201438 File Offset: 0x001FF638
		private void ParseProperties(Wiki.Element e)
		{
			for (;;)
			{
				this.cur_pos.SkipBlanks(true);
				string text = this.cur_pos.ReadIdentifier(true);
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				while (this.cur_pos.Char(0) == '.')
				{
					text += ".";
					this.cur_pos.idx = this.cur_pos.idx + 1;
					text += this.cur_pos.ReadIdentifier(true);
				}
				if (!this.cur_pos.Skip(" = ", true))
				{
					return;
				}
				char c = this.cur_pos.Char(0);
				string text2;
				if (c == '"')
				{
					this.cur_pos.idx = this.cur_pos.idx + 1;
					text2 = "\"" + this.cur_pos.ReadUnitl(c, false, false);
					c = this.cur_pos.Char(0);
					if (c == '"')
					{
						text2 += c.ToString();
						this.cur_pos.MoveNext(true);
					}
				}
				else if (c == '(' || c == '#' || c == '?')
				{
					int idx = this.cur_pos.idx;
					Expression expression = Expression.ReadExpression(this.cur_pos.text, ref this.cur_pos.idx, int.MaxValue);
					text2 = this.cur_pos.text.Substring(idx, this.cur_pos.idx - idx);
					if (expression == null || expression.type == Expression.Type.Invalid)
					{
						this.Error("Invalid expression: '" + text2 + "'");
					}
				}
				else
				{
					text2 = this.cur_pos.ReadUnitl(' ', true, false);
				}
				e.AddProperty(text, text2);
			}
		}

		// Token: 0x0600443D RID: 17469 RVA: 0x002015CC File Offset: 0x001FF7CC
		private Wiki.Table EnsureInTable()
		{
			if (this.cur_table != null)
			{
				return this.cur_table;
			}
			if (this.cur_article == null)
			{
				this.Error("not in article");
				return null;
			}
			if (this.cur_article.body == null)
			{
				this.cur_article.body = new Wiki.Table(this.cur_article.location, this.cur_article);
			}
			return this.cur_article.body;
		}

		// Token: 0x0600443E RID: 17470 RVA: 0x00201638 File Offset: 0x001FF838
		private void ParseRow()
		{
			Wiki.Table table = this.EnsureInTable();
			if (table == null)
			{
				return;
			}
			this.cur_row = new Wiki.Row(table, new Wiki.Location(this.cur_pos), "row");
			this.ParseProperties(this.cur_row);
		}

		// Token: 0x0600443F RID: 17471 RVA: 0x00201678 File Offset: 0x001FF878
		private void ParseRows()
		{
			Wiki.Table table = this.EnsureInTable();
			if (table == null)
			{
				return;
			}
			this.cur_row = new Wiki.Rows(table, new Wiki.Location(this.cur_pos));
			this.ParseProperties(this.cur_row);
		}

		// Token: 0x06004440 RID: 17472 RVA: 0x002016B4 File Offset: 0x001FF8B4
		private Wiki.Row EnsureInRow(Wiki.Location location)
		{
			if (this.cur_row != null)
			{
				return this.cur_row;
			}
			Wiki.Table table = this.EnsureInTable();
			if (table == null)
			{
				return null;
			}
			this.cur_row = new Wiki.Row(table, location, "row");
			return this.cur_row;
		}

		// Token: 0x06004441 RID: 17473 RVA: 0x002016F4 File Offset: 0x001FF8F4
		private void ParseTable()
		{
			Wiki.Row row = this.EnsureInRow(new Wiki.Location(this.cur_pos));
			if (row == null)
			{
				return;
			}
			this.cur_table = new Wiki.Table(new Wiki.Location(this.cur_pos), row);
			this.cur_row = null;
			this.ParseProperties(this.cur_table);
		}

		// Token: 0x06004442 RID: 17474 RVA: 0x00201744 File Offset: 0x001FF944
		private void ParseEndTable()
		{
			if (this.cur_table == null)
			{
				this.Error("mismatched /table");
				return;
			}
			this.cur_row = this.cur_table.parent_row;
			Wiki.Row row = this.cur_row;
			this.cur_table = ((row != null) ? row.parent_table : null);
			if (this.cur_table != null && this.cur_table.parent_row == null)
			{
				this.cur_table = null;
			}
		}

		// Token: 0x06004443 RID: 17475 RVA: 0x002017AC File Offset: 0x001FF9AC
		private void ParseText()
		{
			Wiki.Row row = this.EnsureInRow(new Wiki.Location(this.cur_pos));
			if (row == null)
			{
				return;
			}
			this.cur_text_element = new Wiki.Text(new Wiki.Location(this.cur_pos), row);
			this.ParseProperties(this.cur_text_element);
		}

		// Token: 0x06004444 RID: 17476 RVA: 0x002017F4 File Offset: 0x001FF9F4
		private void ParseImage()
		{
			Wiki.Row row = this.EnsureInRow(new Wiki.Location(this.cur_pos));
			if (row == null)
			{
				return;
			}
			Wiki.Image e = new Wiki.Image(new Wiki.Location(this.cur_pos), row);
			this.ParseProperties(e);
		}

		// Token: 0x06004445 RID: 17477 RVA: 0x00201830 File Offset: 0x001FFA30
		private void ParseSpace()
		{
			Wiki.Row row = this.EnsureInRow(new Wiki.Location(this.cur_pos));
			if (row == null)
			{
				return;
			}
			Wiki.Space e = new Wiki.Space(new Wiki.Location(this.cur_pos), row);
			this.ParseProperties(e);
		}

		// Token: 0x06004446 RID: 17478 RVA: 0x0020186C File Offset: 0x001FFA6C
		private void ParseSeparator()
		{
			Wiki.Table table = this.EnsureInTable();
			if (table == null)
			{
				return;
			}
			Wiki.Separator e = new Wiki.Separator(table, new Wiki.Location(this.cur_pos));
			this.ParseProperties(e);
			this.cur_row = null;
		}

		// Token: 0x040030CE RID: 12494
		public Wiki wiki;

		// Token: 0x040030CF RID: 12495
		public Wiki.Parser.Pos cur_pos;

		// Token: 0x040030D0 RID: 12496
		public Wiki.Article cur_article;

		// Token: 0x040030D1 RID: 12497
		public Wiki.Parser.Pos cur_article_pos;

		// Token: 0x040030D2 RID: 12498
		public Wiki.Table cur_table;

		// Token: 0x040030D3 RID: 12499
		public Wiki.Row cur_row;

		// Token: 0x040030D4 RID: 12500
		public Wiki.Text cur_text_element;

		// Token: 0x040030D5 RID: 12501
		public Wiki.Location cur_text_location;

		// Token: 0x040030D6 RID: 12502
		public StringBuilder cur_text_text = new StringBuilder(1024);

		// Token: 0x040030D7 RID: 12503
		public bool append_new_line;

		// Token: 0x020009D8 RID: 2520
		public struct Pos
		{
			// Token: 0x17000726 RID: 1830
			// (get) Token: 0x060054ED RID: 21741 RVA: 0x00247CBC File Offset: 0x00245EBC
			public bool Valid
			{
				get
				{
					return this.text != null && this.idx >= 0 && this.idx < this.text.Length;
				}
			}

			// Token: 0x060054EE RID: 21742 RVA: 0x00247CE4 File Offset: 0x00245EE4
			public bool IsValidIdx(int i)
			{
				return this.text != null && i >= 0 && i < this.text.Length;
			}

			// Token: 0x060054EF RID: 21743 RVA: 0x00247D08 File Offset: 0x00245F08
			public char RawChar(int ofs = 0)
			{
				int num = this.idx + ofs;
				if (!this.IsValidIdx(num))
				{
					return '\0';
				}
				return this.text[num];
			}

			// Token: 0x060054F0 RID: 21744 RVA: 0x00247D38 File Offset: 0x00245F38
			public char Char(int ofs = 0)
			{
				char c = this.RawChar(ofs);
				if (c == '\r')
				{
					c = '\n';
				}
				return c;
			}

			// Token: 0x060054F1 RID: 21745 RVA: 0x00247D58 File Offset: 0x00245F58
			public string LineText()
			{
				if (this.text == null)
				{
					return "<null>";
				}
				int num = this.line_idx;
				while (num < this.text.Length && this.text[num] != '\r' && this.text[num] != '\n')
				{
					num++;
				}
				return this.text.Substring(this.line_idx, num - this.line_idx);
			}

			// Token: 0x060054F2 RID: 21746 RVA: 0x00247DC8 File Offset: 0x00245FC8
			public override string ToString()
			{
				if (!this.Valid)
				{
					return "<EOF>";
				}
				string text = "";
				int i = this.line_idx;
				while (i < this.idx)
				{
					text += this.text[i++].ToString();
				}
				text += "|->";
				text += this.text[i++].ToString();
				text += "<-|";
				while (i < this.text.Length && this.text[i] != '\r' && this.text[i] != '\n')
				{
					text += this.text[i++].ToString();
				}
				return text;
			}

			// Token: 0x060054F3 RID: 21747 RVA: 0x00247EA4 File Offset: 0x002460A4
			public bool MoveNext(bool skip_comments = true)
			{
				char c = this.RawChar(0);
				if (c == '\0')
				{
					return false;
				}
				this.idx++;
				if (c == '\n' || c == '\r')
				{
					if (this.RawChar(0) == '\u0017' - c)
					{
						this.idx++;
					}
					this.line++;
					this.line_idx = this.idx;
				}
				this.SkipComments(skip_comments);
				return true;
			}

			// Token: 0x060054F4 RID: 21748 RVA: 0x00247F14 File Offset: 0x00246114
			private void SkipComments(bool skip_comments)
			{
				if (!skip_comments)
				{
					return;
				}
				if (this.RawChar(0) != '/')
				{
					return;
				}
				char c = this.RawChar(1);
				if (c == '/')
				{
					this.idx += 2;
					for (;;)
					{
						c = this.RawChar(0);
						if (c == '\r' || c == '\n' || c == '\0')
						{
							break;
						}
						this.idx++;
					}
				}
			}

			// Token: 0x060054F5 RID: 21749 RVA: 0x00247F74 File Offset: 0x00246174
			public bool SkipBlanks(bool skip_comments = true)
			{
				bool result = false;
				while (Wiki.Parser.Pos.IsSLBlank(this.Char(0)))
				{
					result = true;
					this.idx++;
				}
				this.SkipComments(skip_comments);
				return result;
			}

			// Token: 0x060054F6 RID: 21750 RVA: 0x00247FAC File Offset: 0x002461AC
			public bool SkipToEndOfLine(bool trim_right = true)
			{
				int num = this.idx;
				int num2 = this.idx - 1;
				for (;;)
				{
					char c = this.Char(0);
					if (c == '\n' || c == '\0')
					{
						break;
					}
					if (!trim_right || !Wiki.Parser.Pos.IsSLBlank(c))
					{
						num2 = this.idx;
					}
					this.MoveNext(true);
				}
				this.idx = num2 + 1;
				return this.idx > num;
			}

			// Token: 0x060054F7 RID: 21751 RVA: 0x00248008 File Offset: 0x00246208
			public bool SkipLine(bool skip_comments = true)
			{
				for (;;)
				{
					char c = this.Char(0);
					if (!this.MoveNext(skip_comments))
					{
						break;
					}
					if (c == '\n')
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060054F8 RID: 21752 RVA: 0x00248030 File Offset: 0x00246230
			public bool Skip(string pattern, bool skip_comments = true)
			{
				Wiki.Parser.Pos pos = this;
				int i = 0;
				while (i < pattern.Length)
				{
					if (!pos.Valid)
					{
						return false;
					}
					char c = pattern[i];
					if (c == ' ')
					{
						pos.SkipBlanks(skip_comments);
						i++;
					}
					else
					{
						if (pos.Char(0) != c)
						{
							return false;
						}
						i++;
						pos.MoveNext(skip_comments);
					}
				}
				this = pos;
				this.SkipComments(skip_comments);
				return true;
			}

			// Token: 0x060054F9 RID: 21753 RVA: 0x002480A1 File Offset: 0x002462A1
			private static bool IsSLBlank(char c)
			{
				return c == ' ' || c == '\t';
			}

			// Token: 0x060054FA RID: 21754 RVA: 0x002480AF File Offset: 0x002462AF
			private static bool IsIdChar(char c, bool first)
			{
				return c == '_' || (first && c == '/') || char.IsLetter(c) || (!first && char.IsDigit(c));
			}

			// Token: 0x060054FB RID: 21755 RVA: 0x002480DA File Offset: 0x002462DA
			private static bool Match(char c, char pattern)
			{
				if (pattern == ' ')
				{
					return c == ' ' || c == '\t';
				}
				if (pattern == '\n')
				{
					return c == '\n' || c == '\r';
				}
				return c == pattern;
			}

			// Token: 0x060054FC RID: 21756 RVA: 0x00248104 File Offset: 0x00246304
			public string ReadIdentifier(bool skip_comments = true)
			{
				if (!Wiki.Parser.Pos.IsIdChar(this.Char(0), true))
				{
					return null;
				}
				int num = this.idx;
				this.idx++;
				while (this.idx < this.text.Length && Wiki.Parser.Pos.IsIdChar(this.Char(0), false))
				{
					this.idx++;
				}
				int num2 = this.idx;
				this.SkipComments(skip_comments);
				return this.text.Substring(num, num2 - num);
			}

			// Token: 0x060054FD RID: 21757 RVA: 0x00248188 File Offset: 0x00246388
			public string ReadUnitl(char cEnd, bool skip_comments, bool trim_right)
			{
				int num = this.idx;
				int num2 = this.idx - 1;
				for (;;)
				{
					char c = this.Char(0);
					if (c == '\0' || c == '\n' || Wiki.Parser.Pos.Match(c, cEnd))
					{
						break;
					}
					if (!trim_right || !Wiki.Parser.Pos.IsSLBlank(c))
					{
						num2 = this.idx;
					}
					this.MoveNext(skip_comments);
				}
				if (num2 < num)
				{
					return "";
				}
				return this.text.Substring(num, num2 + 1 - num);
			}

			// Token: 0x04004575 RID: 17781
			public string text;

			// Token: 0x04004576 RID: 17782
			public string filename;

			// Token: 0x04004577 RID: 17783
			public int line;

			// Token: 0x04004578 RID: 17784
			public int line_idx;

			// Token: 0x04004579 RID: 17785
			public int idx;
		}
	}
}
