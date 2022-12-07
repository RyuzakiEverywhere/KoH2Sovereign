using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class UIText : MonoBehaviour
{
	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0004709D File Offset: 0x0004529D
	// (set) Token: 0x060006B4 RID: 1716 RVA: 0x000470A5 File Offset: 0x000452A5
	public string org_text { get; private set; }

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060006B5 RID: 1717 RVA: 0x000470AE File Offset: 0x000452AE
	// (set) Token: 0x060006B6 RID: 1718 RVA: 0x000470B6 File Offset: 0x000452B6
	public bool has_links { get; private set; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060006B7 RID: 1719 RVA: 0x000470BF File Offset: 0x000452BF
	// (set) Token: 0x060006B8 RID: 1720 RVA: 0x000470C7 File Offset: 0x000452C7
	public bool has_numbers { get; private set; }

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x000470D0 File Offset: 0x000452D0
	public bool in_wiki
	{
		get
		{
			return this.article != null;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060006BA RID: 1722 RVA: 0x000470DB File Offset: 0x000452DB
	public string article_def_id
	{
		get
		{
			Wiki.Article article = this.article;
			if (article == null)
			{
				return null;
			}
			return article.def_id;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060006BB RID: 1723 RVA: 0x000470EE File Offset: 0x000452EE
	public bool in_message
	{
		get
		{
			return this.message_wnd != null;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060006BC RID: 1724 RVA: 0x000470FC File Offset: 0x000452FC
	public Tutorial.Topic tutorial_topic
	{
		get
		{
			MessageWnd messageWnd = this.message_wnd;
			DT.Def message_def;
			if (messageWnd == null)
			{
				message_def = null;
			}
			else
			{
				DT.Field def_field = messageWnd.def_field;
				message_def = ((def_field != null) ? def_field.def : null);
			}
			return Tutorial.Topic.Get(message_def, false);
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060006BD RID: 1725 RVA: 0x00047122 File Offset: 0x00045322
	public bool in_tutorial
	{
		get
		{
			return this.tutorial_topic != null;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x060006BE RID: 1726 RVA: 0x0004712D File Offset: 0x0004532D
	private static bool alt
	{
		get
		{
			return Tutorial.active;
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00047134 File Offset: 0x00045334
	public static void ForceNextLinks(UIText.LinkSettings settings)
	{
		if (!UIText.force_next_links.AreDefault())
		{
			Debug.LogError("ForceNextLinks already called");
		}
		UIText.force_next_links = settings;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00047154 File Offset: 0x00045354
	public bool ProcessText(TMP_Text component, ref string text, bool refreshing = false)
	{
		UIText.<>c__DisplayClass38_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (text == null)
		{
			text = "";
		}
		this.text_field = component;
		this.org_text = text;
		this.has_numbers = false;
		if (!refreshing)
		{
			this.article = UIText.cur_article;
			this.message_wnd = UIText.cur_message_wnd;
		}
		bool has_links = this.has_links;
		if (!refreshing)
		{
			this.has_links = false;
		}
		List<UIText.LinkInfo> list = this.links;
		if (list != null)
		{
			list.Clear();
		}
		UIText.sb.Clear();
		UIText.links_stack.Clear();
		CS$<>8__locals1.link_settings = UIText.GetLinkSettings(component);
		Wiki wiki = Wiki.Get();
		int num = 0;
		int i = 0;
		bool flag = true;
		bool flag2 = false;
		while (i < text.Length)
		{
			char c = text[i];
			bool flag3 = char.IsLetter(c);
			if (!flag3)
			{
				flag = true;
			}
			if (char.IsDigit(c))
			{
				this.has_numbers = true;
			}
			if (c == '<')
			{
				if (flag2)
				{
					if (UIText.Match(text, i, "</noparse>"))
					{
						i += 10;
						flag2 = false;
					}
					else
					{
						i++;
					}
				}
				else if (!flag2 && UIText.Match(text, i, "<noparse>"))
				{
					i += 9;
					flag2 = true;
				}
				else if (UIText.Match(text, i, "<link=\""))
				{
					int num2 = i;
					i += 7;
					int num3 = i;
					if (UIText.<ProcessText>g__Seek|38_0(text, ref i, ':', '"') == ':')
					{
						int num4 = i;
						i++;
						int num5 = i;
						if (UIText.<ProcessText>g__Seek|38_0(text, ref i, '"', '\0') == '"')
						{
							int num6 = i;
							i++;
							if (i < text.Length && text[i] == '>')
							{
								i++;
								if (num2 > num)
								{
									UIText.sb.Append(text, num, num2 - num);
								}
								num = i;
								string type = text.Substring(num3, num4 - num3);
								string value = text.Substring(num5, num6 - num5);
								this.<ProcessText>g__InsertLink|38_2(type, value, text, i, ref CS$<>8__locals1);
							}
						}
					}
				}
				else if (UIText.Match(text, i, "</link>"))
				{
					if (i > num)
					{
						UIText.sb.Append(text, num, i - num);
					}
					i += 7;
					num = i;
					this.<ProcessText>g__InsertLinkClose|38_3(ref CS$<>8__locals1);
				}
				else if (UIText.Match(text, i, "<color="))
				{
					int num7 = i;
					i += 7;
					if (i >= text.Length)
					{
						break;
					}
					int num8 = i;
					if (text[i] == '#')
					{
						if (UIText.<ProcessText>g__Seek|38_0(text, ref i, '>', '\0') == '>')
						{
							i++;
						}
					}
					else if (UIText.<ProcessText>g__Seek|38_0(text, ref i, '>', '\0') == '>')
					{
						string org_clr = text.Substring(num8, i - num8);
						i++;
						string text2 = UIText.FixColor(org_clr, component);
						if (text2 != null)
						{
							if (num7 > num)
							{
								UIText.sb.Append(text, num, num7 - num);
							}
							num = i;
							UIText.sb.Append(text2);
						}
					}
				}
				else if (UIText.<ProcessText>g__Seek|38_0(text, ref i, '>', '\0') == '>')
				{
					i++;
				}
			}
			else
			{
				if (flag && flag3 && !flag2)
				{
					flag = false;
					List<Wiki.Keyword> list2 = (wiki != null) ? wiki.GetKeywords(c, false) : null;
					if (list2 != null)
					{
						bool flag4 = false;
						for (int j = 0; j < list2.Count; j++)
						{
							Wiki.Keyword keyword = list2[j];
							if (UIText.MatchKeyword(text, i, keyword.keyword))
							{
								if (i > num)
								{
									UIText.sb.Append(text, num, i - num);
								}
								this.<ProcessText>g__InsertLink|38_2("keyword", keyword.keyword, text, i, ref CS$<>8__locals1);
								UIText.sb.Append(text, i, keyword.keyword.Length);
								i += keyword.keyword.Length;
								num = i;
								this.<ProcessText>g__InsertLinkClose|38_3(ref CS$<>8__locals1);
								flag4 = true;
								break;
							}
						}
						if (flag4)
						{
							continue;
						}
					}
				}
				i++;
			}
		}
		this.RemoveInvalidLinks();
		if (this.links != null && this.links.Count > 0 && UIHyperText.posses_first_link_row != null && UIHyperText.posses_first_link_row.possessed_link_ui_text == null)
		{
			UIHyperText.posses_first_link_row.possessed_link_ui_text = this;
		}
		if (!refreshing && this.has_links != has_links)
		{
			if (this.has_links)
			{
				UIText.ui_texts_with_links.Add(this);
			}
			else
			{
				UIText.ui_texts_with_links.Remove(this);
			}
		}
		if (num == 0)
		{
			return false;
		}
		if (num < text.Length)
		{
			UIText.sb.Append(text, num, text.Length - num);
		}
		text = UIText.sb.ToString();
		UIText.sb.Clear();
		return true;
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00047604 File Offset: 0x00045804
	private void RemoveInvalidLinks()
	{
		if (this.links == null)
		{
			return;
		}
		for (int i = this.links.Count - 1; i >= 0; i--)
		{
			if (!this.links[i].is_valid)
			{
				this.links.RemoveAt(i);
			}
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00047654 File Offset: 0x00045854
	public static string FixColor(string org_clr, TMP_Text component)
	{
		if (string.IsNullOrEmpty(org_clr) || component == null)
		{
			return "";
		}
		bool flag = component.color.maxColorComponent >= 0.4f;
		global::Defs.allow_wildcard_form = false;
		string text = global::Defs.Localize("clr", null, org_clr + (flag ? "_bright" : "_dark"), false, true);
		global::Defs.allow_wildcard_form = true;
		if (text == null)
		{
			return null;
		}
		return text;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x000476C6 File Offset: 0x000458C6
	public bool GetFirstLink(out int idx, out UIText.LinkInfo link)
	{
		if (this.links == null || this.links.Count == 0)
		{
			idx = -1;
			link = default(UIText.LinkInfo);
			return false;
		}
		idx = 0;
		link = this.links[idx];
		return true;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00047700 File Offset: 0x00045900
	public bool FindLink(int char_idx, out int idx, out UIText.LinkInfo link)
	{
		idx = -1;
		link = default(UIText.LinkInfo);
		if (this.links == null)
		{
			return false;
		}
		for (int i = 0; i < this.links.Count; i++)
		{
			UIText.LinkInfo linkInfo = this.links[i];
			if (linkInfo.start_idx > char_idx)
			{
				break;
			}
			if (linkInfo.end_idx > char_idx)
			{
				idx = i;
				link = linkInfo;
			}
		}
		return idx >= 0;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00047768 File Offset: 0x00045968
	private static void ResolveLink(ref string type, ref string value, bool prefer_wiki)
	{
		string a = type;
		if (!(a == "obj"))
		{
			if (!(a == "def"))
			{
				if (!(a == "wiki"))
				{
					if (!(a == "keyword"))
					{
						return;
					}
					Wiki wiki = Wiki.Get();
					Wiki.Keyword keyword = (wiki != null) ? wiki.GetKeyword(value, false) : null;
					if (((keyword != null) ? keyword.articles : null) == null || keyword.articles.Count == 0)
					{
						value = null;
					}
				}
				else
				{
					Wiki wiki2 = Wiki.Get();
					if (((wiki2 != null) ? wiki2.FindArticle(value) : null) == null)
					{
						value = null;
						return;
					}
				}
			}
			else if (!string.IsNullOrEmpty(value))
			{
				global::Defs defs = global::Defs.Get(false);
				DT.Field field;
				if (defs == null)
				{
					field = null;
				}
				else
				{
					DT dt = defs.dt;
					field = ((dt != null) ? dt.Find(value, null) : null);
				}
				DT.Field field2 = field;
				if (field2 == null)
				{
					value = null;
					return;
				}
				DT.Field parent = field2.parent;
				if (((parent != null) ? parent.key : null) == "wiki")
				{
					Wiki wiki3 = Wiki.Get();
					Wiki.Article article = (wiki3 != null) ? wiki3.FindArticle(field2.key) : null;
					if (article == null)
					{
						value = null;
						return;
					}
					type = "wiki";
					value = article.id;
					return;
				}
				else if (prefer_wiki)
				{
					DT.Field field3 = field2.BaseRoot();
					Wiki wiki4 = Wiki.Get();
					if (((wiki4 != null) ? wiki4.FindArticle(field3.key) : null) != null)
					{
						type = "wiki";
						value = field2.Path(false, false, '.');
						return;
					}
				}
			}
		}
		else if (value == "0")
		{
			value = null;
			return;
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x000478D4 File Offset: 0x00045AD4
	public static void ResolveLinkTarget(ref string type, ref string value)
	{
		string a = type;
		if (a == "keyword")
		{
			Wiki wiki = Wiki.Get();
			Wiki.Keyword keyword = (wiki != null) ? wiki.GetKeyword(value, false) : null;
			if (((keyword != null) ? keyword.articles : null) != null && keyword.articles.Count != 0 && keyword.articles.Count == 1)
			{
				Wiki.Article article = keyword.articles[0];
				type = "wiki";
				DT.Def def = article.def_field.def;
				value = ((def != null) ? def.path : null);
			}
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0004795C File Offset: 0x00045B5C
	private void Offset(ref int i, int idx, int offset)
	{
		if (i < idx)
		{
			return;
		}
		i += offset;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x0004796C File Offset: 0x00045B6C
	private void OffsetLinks(int idx, int offset)
	{
		if (this.links == null)
		{
			return;
		}
		for (int i = 0; i < this.links.Count; i++)
		{
			UIText.LinkInfo value = this.links[i];
			this.Offset(ref value.start_idx, idx, offset);
			this.Offset(ref value.end_idx, idx, offset);
			this.links[i] = value;
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x000479D0 File Offset: 0x00045BD0
	private void InsertMarkup(ref string text, int idx, string markup)
	{
		if (idx < 0 || idx >= text.Length)
		{
			return;
		}
		text = text.Insert(idx, markup);
		this.OffsetLinks(idx, markup.Length);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x000479F9 File Offset: 0x00045BF9
	private void DelMarkup(ref string text, int idx, string markup)
	{
		idx -= markup.Length;
		if (idx < 0)
		{
			return;
		}
		if (!UIText.Match(text, idx, markup))
		{
			return;
		}
		text = text.Remove(idx, markup.Length);
		this.OffsetLinks(idx, -markup.Length);
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00047A34 File Offset: 0x00045C34
	public void AddUnderline(int link_idx)
	{
		if (this.links == null || link_idx < 0 || link_idx >= this.links.Count)
		{
			return;
		}
		UIText.LinkInfo linkInfo = this.links[link_idx];
		TMP_Text text_field = this.text_field;
		string text = (text_field != null) ? text_field.text : null;
		if (text == null)
		{
			return;
		}
		this.InsertMarkup(ref text, linkInfo.start_idx, "<u>");
		linkInfo = this.links[link_idx];
		this.InsertMarkup(ref text, linkInfo.end_idx, "</u>");
		this.text_field.text = text;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00047AC0 File Offset: 0x00045CC0
	public void DelUnderline(int link_idx)
	{
		if (this.links == null || link_idx < 0 || link_idx >= this.links.Count)
		{
			return;
		}
		UIText.LinkInfo linkInfo = this.links[link_idx];
		TMP_Text text_field = this.text_field;
		string text = (text_field != null) ? text_field.text : null;
		if (text == null)
		{
			return;
		}
		this.DelMarkup(ref text, linkInfo.start_idx, "<u>");
		linkInfo = this.links[link_idx];
		this.DelMarkup(ref text, linkInfo.end_idx, "</u>");
		this.text_field.text = text;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00047B4C File Offset: 0x00045D4C
	public static void AltChanged()
	{
		for (int i = 0; i < UIText.ui_texts_with_links.Count; i++)
		{
			UIText uitext = UIText.ui_texts_with_links[i];
			if (!(uitext.text_field == null) && !uitext.in_wiki)
			{
				string org_text = uitext.org_text;
				uitext.ProcessText(uitext.text_field, ref org_text, true);
				uitext.text_field.text = org_text;
			}
		}
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00047BB3 File Offset: 0x00045DB3
	public static void SetText(TMP_Text component, string text)
	{
		if (component == null)
		{
			return;
		}
		UIText orAddComponent = component.gameObject.GetOrAddComponent<UIText>();
		if (orAddComponent != null)
		{
			orAddComponent.ProcessText(component, ref text, false);
		}
		component.text = text;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00047BE4 File Offset: 0x00045DE4
	public static void SetTextKey(TMP_Text component, string key, IVars vars = null, string form = null)
	{
		string text = global::Defs.Localize(key, vars, form, true, true);
		UIText.SetText(component, text);
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00047C04 File Offset: 0x00045E04
	public static void SetText(TMP_Text component, DT.Field field, IVars vars = null, string form = null)
	{
		string text = global::Defs.Localize(field, vars, form, true, true);
		UIText.SetText(component, text);
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00047C24 File Offset: 0x00045E24
	public static void SetText(TMP_Text component, DT.Field field, string key, IVars vars = null, string form = null)
	{
		string text = global::Defs.Localize(field, key, vars, form, true, true);
		UIText.SetText(component, text);
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00047C45 File Offset: 0x00045E45
	public static void SetText(GameObject go, string child, string text)
	{
		UIText.SetText(global::Common.FindChildComponent<TMP_Text>(go, child), text);
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00047C54 File Offset: 0x00045E54
	public static void SetTextKey(GameObject go, string child, string key, IVars vars = null, string form = null)
	{
		UIText.SetTextKey(global::Common.FindChildComponent<TMP_Text>(go, child), key, vars, form);
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00047C66 File Offset: 0x00045E66
	public static void SetText(GameObject go, string child, DT.Field field, IVars vars = null, string form = null)
	{
		UIText.SetText(global::Common.FindChildComponent<TMP_Text>(go, child), field, vars, form);
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00047C78 File Offset: 0x00045E78
	public static void SetText(GameObject go, string child, DT.Field field, string key, IVars vars = null, string form = null)
	{
		UIText.SetText(global::Common.FindChildComponent<TMP_Text>(go, child), field, key, vars, form);
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00047C8C File Offset: 0x00045E8C
	private static UIText.LinkSettings GetLinkSettings(TMP_Text component)
	{
		if (!UIText.force_next_links.AreDefault())
		{
			UIText.LinkSettings result = UIText.force_next_links;
			UIText.force_next_links = default(UIText.LinkSettings);
			return result;
		}
		if (component == null)
		{
			return UIText.LinkSettings.Default;
		}
		UIText uitext = null;
		Transform transform = component.transform;
		while (transform != null)
		{
			UIText component2 = transform.GetComponent<UIText>();
			if (!(component2 == null) && !component2.linkSettings.AreDefault())
			{
				uitext = component2;
				break;
			}
			transform = transform.parent;
		}
		if (uitext == null)
		{
			return UIText.LinkSettings.Default;
		}
		return uitext.linkSettings;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00047D18 File Offset: 0x00045F18
	public static bool Match(string text, int idx, string substring)
	{
		int length = substring.Length;
		if (idx + length > text.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			char c = text[idx + i];
			char c2 = substring[i];
			if (c != c2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00047D5C File Offset: 0x00045F5C
	public static bool MatchKeyword(string text, int idx, string keyword)
	{
		int length = keyword.Length;
		if (idx + length > text.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			char c = char.ToLowerInvariant(text[idx + i]);
			char c2 = char.ToLowerInvariant(keyword[i]);
			if (c != c2)
			{
				return false;
			}
		}
		return idx + length == text.Length || !char.IsLetter(text[idx + length]);
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00047DCC File Offset: 0x00045FCC
	public static string ToPlainText(string text)
	{
		StringBuilder stringBuilder = null;
		int length = text.Length;
		int num = 0;
		int num2 = 0;
		for (;;)
		{
			int num3 = text.IndexOf('<', num2);
			if (num3 < 0)
			{
				break;
			}
			num2 = num3 + 1;
			if (num2 >= length)
			{
				break;
			}
			char c = text[num2];
			if (c == '/' || char.IsLetter(c))
			{
				int num4 = text.IndexOf('>', num2);
				if (num4 < 0)
				{
					break;
				}
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length);
				}
				if (num3 > num)
				{
					stringBuilder.Append(text, num, num3 - num);
				}
				num2 = (num = num4 + 1);
				if (UIText.Match(text, num3, "<sprite"))
				{
					int num5 = text.IndexOf("name=\"", num3, num4 - num3);
					if (num5 >= 0)
					{
						num5 += 6;
						int num6 = text.IndexOf('"', num5, num4 - num5);
						if (num5 > 0)
						{
							stringBuilder.Append('<');
							stringBuilder.Append(text, num5, num6 - num5);
							stringBuilder.Append('>');
						}
					}
				}
			}
		}
		if (stringBuilder == null)
		{
			return text;
		}
		if (num < length)
		{
			stringBuilder.Append(text, num, length - num);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00047EE0 File Offset: 0x000460E0
	public static string StripLinks(string text)
	{
		text = text.Replace("</link>", "");
		text = text.Replace("</LINK>", "");
		int num = 0;
		for (;;)
		{
			num = text.IndexOf("<link", num, StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				break;
			}
			int num2 = text.IndexOf('>', num);
			if (num2 < 0)
			{
				break;
			}
			text = text.Remove(num, num2 - num + 1);
		}
		return text;
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00047F44 File Offset: 0x00046144
	private static int StripNestedLinks(ref string text, int link_idx)
	{
		int num = text.IndexOf('>', link_idx) + 1;
		if (num <= 0)
		{
			return -1;
		}
		for (;;)
		{
			int num2 = text.IndexOf("<link", num, StringComparison.OrdinalIgnoreCase);
			if (num2 < 0)
			{
				break;
			}
			int num3 = text.IndexOf("</link>", num, StringComparison.OrdinalIgnoreCase);
			if (num3 < num2)
			{
				return num2;
			}
			UIText.StripNestedLinks(ref text, num2);
			num3 = text.IndexOf("</link>", num2, StringComparison.OrdinalIgnoreCase);
			if (num3 < 0)
			{
				return -1;
			}
			int num4 = text.IndexOf('>', num2) + 1;
			if (num4 < 0 || num4 >= num3)
			{
				return -1;
			}
			text = text.Remove(num3, 7);
			text = text.Remove(num2, num4 - num2);
		}
		return -1;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00047FDC File Offset: 0x000461DC
	public static string StripNestedLinks(string text)
	{
		for (int i = text.IndexOf("<link", StringComparison.OrdinalIgnoreCase); i >= 0; i = UIText.StripNestedLinks(ref text, i))
		{
		}
		return text;
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060006DD RID: 1757 RVA: 0x00048006 File Offset: 0x00046206
	// (set) Token: 0x060006DE RID: 1758 RVA: 0x0004800E File Offset: 0x0004620E
	public TMP_Text text_field { get; private set; }

	// Token: 0x060006DF RID: 1759 RVA: 0x00048017 File Offset: 0x00046217
	protected void OnEnable()
	{
		if (this.text_field == null)
		{
			this.text_field = base.GetComponent<TMP_Text>();
		}
		this.UpdateLocalziation();
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x0004803C File Offset: 0x0004623C
	private void LateUpdate()
	{
		if (this.update_interval <= 0f)
		{
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return;
		}
		if (this.lastUpdateTime + this.update_interval > UnityEngine.Time.time)
		{
			this.UpdateLocalziation();
			this.lastUpdateTime = UnityEngine.Time.time;
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0004808A File Offset: 0x0004628A
	private void OnDestroy()
	{
		if (this.has_links)
		{
			UIText.ui_texts_with_links.Remove(this);
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x000480A0 File Offset: 0x000462A0
	public void SetVars(Vars vars)
	{
		this.Vars = vars;
		this.UpdateLocalziation();
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x000480AF File Offset: 0x000462AF
	public void SetKey(string key, Vars vars)
	{
		this.LocalizationKey = key;
		this.Vars = vars;
		this.UpdateLocalziation();
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000480C5 File Offset: 0x000462C5
	protected void OnValidate()
	{
		this.UpdateLocalziation();
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x000480CD File Offset: 0x000462CD
	public void UpdateLocalziation()
	{
		if (this.text_field != null && !string.IsNullOrEmpty(this.LocalizationKey))
		{
			UIText.SetText(this.text_field, global::Defs.Localize(this.LocalizationKey, this.Vars, null, true, true));
		}
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x0004813C File Offset: 0x0004633C
	[CompilerGenerated]
	internal static char <ProcessText>g__Seek|38_0(string txt, ref int i, char c1, char c2)
	{
		char c3 = '\0';
		while (i < txt.Length)
		{
			c3 = txt[i];
			if (c3 == c1 || c3 == c2)
			{
				break;
			}
			i++;
		}
		return c3;
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00048170 File Offset: 0x00046370
	[CompilerGenerated]
	private bool <ProcessText>g__CheckInore|38_1(string type, string value, string txt, int txt_idx, ref UIText.<>c__DisplayClass38_0 A_5)
	{
		if (type == "keyword")
		{
			UIText.ResolveLinkTarget(ref type, ref value);
		}
		if (type == "wiki")
		{
			if (value == this.article_def_id)
			{
				return true;
			}
			string a = value;
			Wiki.Article currentTOCArticle = UIWikiWindow.CurrentTOCArticle;
			if (a == ((currentTOCArticle != null) ? currentTOCArticle.def_id : null))
			{
				return true;
			}
		}
		UIText.LinkInfo linkInfo = this.<ProcessText>g__CurLink|38_4(ref A_5);
		if (linkInfo.type == null)
		{
			return false;
		}
		if (type == "keyword" && linkInfo.start_idx == UIText.sb.Length && UIText.Match(txt, txt_idx + value.Length, "</link>"))
		{
			return true;
		}
		string type2 = linkInfo.type;
		string value2 = linkInfo.value;
		UIText.ResolveLinkTarget(ref type2, ref value2);
		return type2 == type && value2 == value;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00048244 File Offset: 0x00046444
	[CompilerGenerated]
	private void <ProcessText>g__InsertLink|38_2(string type, string value, string txt, int txt_idx, ref UIText.<>c__DisplayClass38_0 A_5)
	{
		UIText.ResolveLink(ref type, ref value, UIText.alt || this.in_wiki || this.in_tutorial);
		bool flag = string.IsNullOrEmpty(value);
		if (!flag)
		{
			this.has_links = true;
		}
		DT.Field sf;
		UIText.LinkSettings.Mode mode = A_5.link_settings.GetMode(this, type, out sf);
		if (mode == UIText.LinkSettings.Mode.Strip)
		{
			flag = true;
		}
		else if (!flag)
		{
			flag = this.<ProcessText>g__CheckInore|38_1(type, value, txt, txt_idx, ref A_5);
		}
		string text = flag ? null : A_5.link_settings.GetColorTag(this, type, mode, sf);
		if (!string.IsNullOrEmpty(text))
		{
			UIText.sb.Append(text);
		}
		int start_idx = flag ? -1 : UIText.sb.Length;
		UIText.LinkInfo item = new UIText.LinkInfo
		{
			start_idx = start_idx,
			end_idx = -1,
			type = (flag ? null : type),
			value = value,
			color_tag = text
		};
		if (this.links == null)
		{
			this.links = new List<UIText.LinkInfo>();
		}
		UIText.links_stack.Add(this.links.Count);
		this.links.Add(item);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0004835C File Offset: 0x0004655C
	[CompilerGenerated]
	private void <ProcessText>g__InsertLinkClose|38_3(ref UIText.<>c__DisplayClass38_0 A_1)
	{
		if (UIText.links_stack.Count == 0)
		{
			return;
		}
		int index = UIText.links_stack[UIText.links_stack.Count - 1];
		UIText.links_stack.RemoveAt(UIText.links_stack.Count - 1);
		UIText.LinkInfo linkInfo = this.links[index];
		if (linkInfo.start_idx < 0)
		{
			return;
		}
		linkInfo.end_idx = UIText.sb.Length;
		this.links[index] = linkInfo;
		if (!string.IsNullOrEmpty(linkInfo.color_tag))
		{
			UIText.sb.Append("</color>");
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x000483F8 File Offset: 0x000465F8
	[CompilerGenerated]
	private UIText.LinkInfo <ProcessText>g__CurLink|38_4(ref UIText.<>c__DisplayClass38_0 A_1)
	{
		for (int i = UIText.links_stack.Count - 1; i >= 0; i--)
		{
			int index = UIText.links_stack[i];
			UIText.LinkInfo linkInfo = this.links[index];
			if (linkInfo.start_idx >= 0)
			{
				return linkInfo;
			}
		}
		return default(UIText.LinkInfo);
	}

	// Token: 0x040005EA RID: 1514
	public UIText.LinkSettings linkSettings;

	// Token: 0x040005EB RID: 1515
	public UIText.StyleSetting styleSettings;

	// Token: 0x040005EC RID: 1516
	private List<UIText.LinkInfo> links;

	// Token: 0x040005F0 RID: 1520
	public Wiki.Article article;

	// Token: 0x040005F1 RID: 1521
	public MessageWnd message_wnd;

	// Token: 0x040005F2 RID: 1522
	private static List<UIText> ui_texts_with_links = new List<UIText>();

	// Token: 0x040005F3 RID: 1523
	public static Wiki.Article cur_article;

	// Token: 0x040005F4 RID: 1524
	public static MessageWnd cur_message_wnd;

	// Token: 0x040005F5 RID: 1525
	private static StringBuilder sb = new StringBuilder(1024);

	// Token: 0x040005F6 RID: 1526
	private static List<int> links_stack = new List<int>(8);

	// Token: 0x040005F7 RID: 1527
	private static UIText.LinkSettings force_next_links = default(UIText.LinkSettings);

	// Token: 0x040005F8 RID: 1528
	public string LocalizationKey;

	// Token: 0x040005F9 RID: 1529
	public Vars Vars;

	// Token: 0x040005FA RID: 1530
	[Range(0f, 10f)]
	public float update_interval;

	// Token: 0x040005FB RID: 1531
	private float lastUpdateTime;

	// Token: 0x02000580 RID: 1408
	[Serializable]
	public struct LinkSettings
	{
		// Token: 0x060043F7 RID: 17399 RVA: 0x001FFB39 File Offset: 0x001FDD39
		public LinkSettings(UIText.LinkSettings.Mode mode = UIText.LinkSettings.Mode.Default)
		{
			this.linksStyle = null;
			this.mode = mode;
			this.color = Color.clear;
			if (mode == UIText.LinkSettings.Mode.Colorize)
			{
				Debug.LogError("Colorize link settings specified without collor");
			}
		}

		// Token: 0x060043F8 RID: 17400 RVA: 0x001FFB62 File Offset: 0x001FDD62
		public LinkSettings(Color color)
		{
			this.linksStyle = null;
			this.mode = UIText.LinkSettings.Mode.Colorize;
			this.color = color;
		}

		// Token: 0x060043F9 RID: 17401 RVA: 0x001FFB79 File Offset: 0x001FDD79
		public static implicit operator UIText.LinkSettings(UIText.LinkSettings.Mode mode)
		{
			return new UIText.LinkSettings(mode);
		}

		// Token: 0x060043FA RID: 17402 RVA: 0x001FFB81 File Offset: 0x001FDD81
		public static implicit operator UIText.LinkSettings(Color color)
		{
			return new UIText.LinkSettings(color);
		}

		// Token: 0x060043FB RID: 17403 RVA: 0x001FFB89 File Offset: 0x001FDD89
		public bool AreDefault()
		{
			return this.mode == UIText.LinkSettings.Mode.Default && string.IsNullOrEmpty(this.linksStyle);
		}

		// Token: 0x060043FC RID: 17404 RVA: 0x001FFBA0 File Offset: 0x001FDDA0
		private DT.Field GetStyleDef(string link_style, string link_type)
		{
			if (string.IsNullOrEmpty(link_style))
			{
				link_style = "Default";
			}
			DT.Field defField = global::Defs.GetDefField("LinkStyles", null);
			if (defField == null)
			{
				return null;
			}
			DT.Field field = null;
			if (UIText.alt)
			{
				field = defField.FindChild("Alt" + link_style, null, true, true, true, '.');
			}
			if (field == null)
			{
				field = defField.FindChild(link_style, null, true, true, true, '.');
			}
			if (field == null)
			{
				return null;
			}
			DT.Field field2 = field.FindChild(link_type, null, true, true, true, '.');
			if (field2 == null)
			{
				return null;
			}
			return field2;
		}

		// Token: 0x060043FD RID: 17405 RVA: 0x001FFC18 File Offset: 0x001FDE18
		public UIText.LinkSettings.Mode GetMode(UIText ui_text, string link_type, out DT.Field sf)
		{
			if (this.mode != UIText.LinkSettings.Mode.Default && !UIText.alt && link_type != "keyword")
			{
				if (this.mode == UIText.LinkSettings.Mode.Colorize || this.mode == UIText.LinkSettings.Mode.AutoColorize)
				{
					sf = this.GetStyleDef(this.linksStyle, link_type);
				}
				else
				{
					sf = null;
				}
				return this.mode;
			}
			sf = this.GetStyleDef(this.linksStyle, link_type);
			DT.Field field = sf;
			UIText.LinkSettings.Mode result;
			if (Enum.TryParse<UIText.LinkSettings.Mode>(((field != null) ? field.GetString("mode", null, "", true, true, true, '.') : null) ?? "", out result))
			{
				return result;
			}
			if (link_type == "keyword")
			{
				if (UIText.alt || ui_text.in_wiki)
				{
					return UIText.LinkSettings.Mode.AutoColorize;
				}
				return UIText.LinkSettings.Mode.Strip;
			}
			else
			{
				if (UIText.alt && !ui_text.in_wiki)
				{
					return UIText.LinkSettings.AltDefault.mode;
				}
				return UIText.LinkSettings.Default.mode;
			}
		}

		// Token: 0x060043FE RID: 17406 RVA: 0x001FFCF4 File Offset: 0x001FDEF4
		public Color GetColor(UIText ui_text, string link_type, UIText.LinkSettings.Mode mode, DT.Field sf)
		{
			if (mode == UIText.LinkSettings.Mode.NotColorized)
			{
				return Color.clear;
			}
			if (this.mode == UIText.LinkSettings.Mode.Colorize && !UIText.alt && link_type != "keyword")
			{
				return this.color;
			}
			if (sf == null)
			{
				return this.color;
			}
			DT.Field field = sf.FindChild("color", null, true, true, true, '.');
			if (field == null)
			{
				return this.color;
			}
			if (ui_text.text_field != null)
			{
				string path = (ui_text.text_field.color.maxColorComponent >= 0.4f) ? "bright" : "dark";
				DT.Field field2 = field.FindChild(path, null, true, true, true, '.');
				if (field2 != null)
				{
					field = field2;
				}
			}
			return global::Defs.GetColor(field, Color.clear, null);
		}

		// Token: 0x060043FF RID: 17407 RVA: 0x001FFDB0 File Offset: 0x001FDFB0
		public string GetColorTag(UIText ui_text, string link_type, UIText.LinkSettings.Mode mode, DT.Field sf)
		{
			Color lhs = this.GetColor(ui_text, link_type, mode, sf);
			if (lhs == Color.clear)
			{
				return null;
			}
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(lhs) + ">";
		}

		// Token: 0x040030A0 RID: 12448
		public string linksStyle;

		// Token: 0x040030A1 RID: 12449
		public UIText.LinkSettings.Mode mode;

		// Token: 0x040030A2 RID: 12450
		public Color color;

		// Token: 0x040030A3 RID: 12451
		public static UIText.LinkSettings Default = new UIText.LinkSettings
		{
			linksStyle = null,
			mode = UIText.LinkSettings.Mode.AutoColorize
		};

		// Token: 0x040030A4 RID: 12452
		public static UIText.LinkSettings AltDefault = new UIText.LinkSettings
		{
			linksStyle = null,
			mode = UIText.LinkSettings.Mode.Strip
		};

		// Token: 0x020009D7 RID: 2519
		public enum Mode
		{
			// Token: 0x04004570 RID: 17776
			Default,
			// Token: 0x04004571 RID: 17777
			Strip,
			// Token: 0x04004572 RID: 17778
			NotColorized,
			// Token: 0x04004573 RID: 17779
			AutoColorize,
			// Token: 0x04004574 RID: 17780
			Colorize
		}
	}

	// Token: 0x02000581 RID: 1409
	[Serializable]
	public class StyleSetting
	{
		// Token: 0x040030A5 RID: 12453
		public string style;
	}

	// Token: 0x02000582 RID: 1410
	public struct LinkInfo
	{
		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06004402 RID: 17410 RVA: 0x001FFE39 File Offset: 0x001FE039
		public bool is_valid
		{
			get
			{
				return this.start_idx >= 0 && this.end_idx > this.start_idx && this.type != null;
			}
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x001FFE5D File Offset: 0x001FE05D
		public override string ToString()
		{
			return string.Format("[{0} .. {1}] {2}:{3}", new object[]
			{
				this.start_idx,
				this.end_idx,
				this.type,
				this.value
			});
		}

		// Token: 0x040030A6 RID: 12454
		public int start_idx;

		// Token: 0x040030A7 RID: 12455
		public int end_idx;

		// Token: 0x040030A8 RID: 12456
		public string type;

		// Token: 0x040030A9 RID: 12457
		public string value;

		// Token: 0x040030AA RID: 12458
		public string color_tag;
	}
}
