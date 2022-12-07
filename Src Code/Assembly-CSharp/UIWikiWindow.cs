using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200030F RID: 783
public class UIWikiWindow : UIWindow, IVars
{
	// Token: 0x060030D3 RID: 12499 RVA: 0x0018A2FA File Offset: 0x001884FA
	public override string GetDefId()
	{
		return UIWikiWindow.def_id;
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x0018A301 File Offset: 0x00188501
	public static UIWikiWindow.HistoryItem CurHistoryItem()
	{
		if (UIWikiWindow.history_idx < 0)
		{
			return null;
		}
		if (UIWikiWindow.history_idx >= UIWikiWindow.history.Count)
		{
			return null;
		}
		return UIWikiWindow.history[UIWikiWindow.history_idx];
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x0018A330 File Offset: 0x00188530
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialized = true;
		UIHyperText ht_TOC_Body = this.HT_TOC_Body;
		this.SR_TOC = ((ht_TOC_Body != null) ? ht_TOC_Body.GetComponentInParent<ScrollRect>() : null);
		UIHyperText ht_Body = this.HT_Body;
		this.SR_Body = ((ht_Body != null) ? ht_Body.GetComponentInParent<ScrollRect>() : null);
		if (this.Button_Close != null)
		{
			this.Button_Close.onClick = new BSGButton.OnClick(this.HandleOnCloseButtonCLick);
		}
		if (this.Button_Back != null)
		{
			this.Button_Back.onClick = new BSGButton.OnClick(this.HandleOnBackButtonCLick);
			Tooltip.Get(this.Button_Back.gameObject, true).SetText("WikiWindow.button_tooltips.back", null, null);
		}
		if (this.Button_Forward != null)
		{
			this.Button_Forward.onClick = new BSGButton.OnClick(this.HandleOnForwardButtonCLick);
			Tooltip.Get(this.Button_Forward.gameObject, true).SetText("WikiWindow.button_tooltips.forward", null, null);
		}
		if (this.Button_History != null)
		{
			this.Button_History.onClick = new BSGButton.OnClick(this.HandleOnHistoryButtonCLick);
			Tooltip.Get(this.Button_History.gameObject, true).SetText("WikiWindow.button_tooltips.history", null, null);
		}
		UIWikiWindow.history.Clear();
		this.InitShortcutButtons();
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x060030D6 RID: 12502 RVA: 0x0018A47F File Offset: 0x0018867F
	public static Wiki.Article CurrentArticle
	{
		get
		{
			Wiki wiki = Wiki.Get();
			if (wiki == null)
			{
				return null;
			}
			UIWikiWindow uiwikiWindow = UIWikiWindow.current;
			return wiki.FindArticle((uiwikiWindow != null) ? uiwikiWindow.ArticleID : null);
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x060030D7 RID: 12503 RVA: 0x0018A4A2 File Offset: 0x001886A2
	public static Wiki.Article CurrentTOCArticle
	{
		get
		{
			Wiki wiki = Wiki.Get();
			if (wiki == null)
			{
				return null;
			}
			UIWikiWindow uiwikiWindow = UIWikiWindow.current;
			return wiki.FindArticle((uiwikiWindow != null) ? uiwikiWindow.TOCArticleID : null);
		}
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x0018A4C8 File Offset: 0x001886C8
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num <= 2103536335U)
		{
			if (num <= 1248952799U)
			{
				if (num <= 392652633U)
				{
					if (num != 269097364U)
					{
						if (num == 392652633U)
						{
							if (key == "article")
							{
								return new Value(UIWikiWindow.CurrentArticle);
							}
						}
					}
					else if (key == "toc_article_id")
					{
						return this.TOCArticleID;
					}
				}
				else if (num != 1185671095U)
				{
					if (num == 1248952799U)
					{
						if (key == "history")
						{
							return new Value(UIWikiWindow.history);
						}
					}
				}
				else if (key == "vars")
				{
					return new Value(vars);
				}
			}
			else if (num <= 1880179426U)
			{
				if (num != 1321304799U)
				{
					if (num == 1880179426U)
					{
						if (key == "history_uid")
						{
							UIWikiWindow.HistoryItem historyItem = UIWikiWindow.CurHistoryItem();
							if (historyItem == null)
							{
								return Value.Null;
							}
							return historyItem.uid;
						}
					}
				}
				else if (key == "history_idx")
				{
					return UIWikiWindow.history_idx;
				}
			}
			else if (num != 2021149935U)
			{
				if (num == 2103536335U)
				{
					if (key == "toc_scroll")
					{
						return (this.SR_TOC == null) ? -1f : this.SR_TOC.verticalNormalizedPosition;
					}
				}
			}
			else if (key == "history_item")
			{
				return new Value(UIWikiWindow.CurHistoryItem());
			}
		}
		else if (num <= 2880185629U)
		{
			if (num <= 2766234751U)
			{
				if (num != 2357661820U)
				{
					if (num == 2766234751U)
					{
						if (key == "article_id")
						{
							return this.ArticleID;
						}
					}
				}
				else if (key == "context")
				{
					return this.Context;
				}
			}
			else if (num != 2833039000U)
			{
				if (num == 2880185629U)
				{
					if (key == "prev_caption")
					{
						if (UIWikiWindow.history_idx <= 0)
						{
							return Value.Null;
						}
						UIWikiWindow.HistoryItem historyItem2 = UIWikiWindow.history[UIWikiWindow.history_idx - 1];
						if (historyItem2 == null || historyItem2.caption == null)
						{
							return Value.Null;
						}
						return "#" + historyItem2.caption;
					}
				}
			}
			else if (key == "toc_vars")
			{
				return new Value(this.toc_vars);
			}
		}
		else if (num <= 3878312341U)
		{
			if (num != 3255428185U)
			{
				if (num == 3878312341U)
				{
					if (key == "next_caption")
					{
						if (UIWikiWindow.history_idx + 1 >= UIWikiWindow.history.Count)
						{
							return Value.Null;
						}
						UIWikiWindow.HistoryItem historyItem3 = UIWikiWindow.history[UIWikiWindow.history_idx + 1];
						if (historyItem3 == null || historyItem3.caption == null)
						{
							return Value.Null;
						}
						return "#" + historyItem3.caption;
					}
				}
			}
			else if (key == "toc_context")
			{
				return this.TOCContext;
			}
		}
		else if (num != 4175677700U)
		{
			if (num == 4199989948U)
			{
				if (key == "article_caption")
				{
					UIWikiWindow.HistoryItem historyItem4 = UIWikiWindow.CurHistoryItem();
					if (string.IsNullOrEmpty((historyItem4 != null) ? historyItem4.caption : null))
					{
						return Value.Null;
					}
					return "#" + historyItem4.caption;
				}
			}
		}
		else if (key == "toc_article")
		{
			return new Value(UIWikiWindow.CurrentTOCArticle);
		}
		return Value.Unknown;
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x0018A8D0 File Offset: 0x00188AD0
	public string GetTOCArticleDescrID(string toc_article_id)
	{
		if (string.IsNullOrEmpty(toc_article_id))
		{
			return null;
		}
		Wiki wiki = Wiki.Get();
		if (wiki == null)
		{
			return null;
		}
		Wiki.Article article = (wiki != null) ? wiki.FindArticle(toc_article_id) : null;
		if (article == null)
		{
			return null;
		}
		if (article.toc_descr_id == null)
		{
			return null;
		}
		string text = (article.toc_descr_id == "") ? (toc_article_id + "_Description") : article.toc_descr_id;
		if (((wiki != null) ? wiki.FindArticle(text) : null) == null)
		{
			return null;
		}
		return text;
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x0018A948 File Offset: 0x00188B48
	private bool ResolveArticleID(string id, out string article_id, out string toc_article_id)
	{
		article_id = null;
		toc_article_id = null;
		if (string.IsNullOrEmpty(id))
		{
			return false;
		}
		Wiki wiki = Wiki.Get();
		if (wiki == null)
		{
			return false;
		}
		Wiki.Article article = wiki.FindArticle(id);
		if (article == null)
		{
			return false;
		}
		if (article.toc_descr_id != null)
		{
			toc_article_id = id;
		}
		else
		{
			article_id = id;
		}
		return true;
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x0018A990 File Offset: 0x00188B90
	private void FixData(string id, ref string article_id, ref string context, ref string toc_article_id, ref string toc_context)
	{
		if (id == toc_article_id)
		{
			toc_context = context;
			context = null;
		}
		if (article_id != null)
		{
			if (toc_article_id == null)
			{
				toc_article_id = this.TOCArticleID;
				toc_context = this.TOCContext;
			}
		}
		else if (toc_article_id != null)
		{
			article_id = this.GetTOCArticleDescrID(toc_article_id);
			context = null;
			if (article_id == null)
			{
				article_id = this.ArticleID;
				context = this.Context;
			}
		}
		if (string.IsNullOrEmpty(toc_article_id))
		{
			toc_article_id = global::Defs.GetString("WikiArticle", "default_toc_article", null, "");
			toc_context = null;
			if (string.IsNullOrEmpty(article_id))
			{
				article_id = this.GetTOCArticleDescrID(toc_article_id);
			}
		}
		if (string.IsNullOrEmpty(article_id))
		{
			article_id = global::Defs.GetString("WikiArticle", "default_article", null, "");
			context = null;
		}
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x0018AA54 File Offset: 0x00188C54
	private void RememberScrollPositions()
	{
		UIWikiWindow.HistoryItem historyItem = UIWikiWindow.CurHistoryItem();
		if (historyItem != null)
		{
			if (this.SR_TOC != null && historyItem.toc_article_id == this.TOCArticleID)
			{
				historyItem.toc_scroll = Game.clamp(this.SR_TOC.verticalNormalizedPosition, 0f, 1f);
			}
			if (this.SR_Body != null && historyItem.article_id == this.ArticleID)
			{
				historyItem.scroll = Game.clamp(this.SR_Body.verticalNormalizedPosition, 0f, 1f);
			}
		}
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x0018AAEF File Offset: 0x00188CEF
	public void SetData(string article_id, string context, string toc_article_id, string toc_context, bool add_to_history = true)
	{
		this.RememberScrollPositions();
		if (add_to_history)
		{
			this.AddToHistory(article_id, context, toc_article_id, toc_context);
		}
		this.ArticleID = article_id;
		this.Context = context;
		this.TOCArticleID = toc_article_id;
		this.TOCContext = toc_context;
		this.Refresh(true, true);
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x0018AB2C File Offset: 0x00188D2C
	public void SetData(string id, string context = null, bool add_to_history = true)
	{
		if (id == "")
		{
			UIWikiWindow.HistoryItem historyItem = UIWikiWindow.CurHistoryItem();
			if (historyItem != null)
			{
				this.GoToHistory(historyItem, -1);
				return;
			}
		}
		string article_id;
		string toc_article_id;
		this.ResolveArticleID(id, out article_id, out toc_article_id);
		string toc_context = null;
		this.FixData(id, ref article_id, ref context, ref toc_article_id, ref toc_context);
		this.SetData(article_id, context, toc_article_id, toc_context, add_to_history);
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x0018AB80 File Offset: 0x00188D80
	private void InitShortcutButtons()
	{
		this.shortcut_buttons.Clear();
		if (this.Shortcut_Buttons == null)
		{
			return;
		}
		if (this.Shortcut_Buttons.transform.childCount <= 0)
		{
			return;
		}
		GameObject gameObject = this.Shortcut_Buttons.transform.GetChild(0).gameObject;
		gameObject.SetActive(false);
		UICommon.DeleteActiveChildren(this.Shortcut_Buttons.transform);
		DT.Field defField = global::Defs.GetDefField("WikiArticle", "shortuct_buttons");
		if (((defField != null) ? defField.children : null) == null)
		{
			return;
		}
		Wiki wiki = Wiki.Get();
		if (wiki == null)
		{
			return;
		}
		for (int i = 0; i < defField.children.Count; i++)
		{
			DT.Field field = defField.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				Wiki.Article article = wiki.FindArticle(field.key);
				if (article == null)
				{
					Debug.LogError("Unknown wiki article '" + field.key + "'");
				}
				else
				{
					GameObject gameObject2 = global::Common.Spawn(gameObject, this.Shortcut_Buttons.transform, false, "");
					gameObject2.name = field.key;
					gameObject2.SetActive(true);
					Tooltip.Get(gameObject2, true).SetText("wiki." + article.id + ".caption", null, null);
					BSGButton bsgbutton = (gameObject2 != null) ? gameObject2.GetComponent<BSGButton>() : null;
					if (!(bsgbutton == null))
					{
						bsgbutton.AllowSelection(true);
						bsgbutton.onClick = new BSGButton.OnClick(this.HandleOnShorcutButtonClick);
						this.shortcut_buttons.Add(bsgbutton);
						Image component = global::Common.GetComponent<Image>(gameObject2, "Icon");
						if (!(component == null))
						{
							component.overrideSprite = global::Defs.GetObj<Sprite>(field, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x0018AD40 File Offset: 0x00188F40
	private void UpdateShortcutButtons()
	{
		for (int i = 0; i < this.shortcut_buttons.Count; i++)
		{
			BSGButton bsgbutton = this.shortcut_buttons[i];
			bsgbutton.SetSelected(bsgbutton.name == this.TOCArticleID, false);
		}
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x0018AD88 File Offset: 0x00188F88
	public void Refresh(bool scroll_toc = true, bool scroll_article = true)
	{
		this.Init();
		this.defs_version = global::Defs.Version;
		Wiki wiki = Wiki.Get();
		Wiki.Article article = (wiki != null) ? wiki.FindArticle(this.TOCArticleID) : null;
		Wiki.Article article2 = (wiki != null) ? wiki.FindArticle(this.ArticleID) : null;
		this.FillVars(this.toc_vars, article, this.TOCContext);
		this.FillVars(this.vars, article2, this.Context);
		UIWikiWindow.HistoryItem historyItem = UIWikiWindow.CurHistoryItem();
		if (historyItem != null && historyItem.article_id == this.ArticleID && historyItem.caption == null)
		{
			historyItem.caption = (global::Defs.Localize((article2 != null) ? article2.caption_field : null, this.vars, null, false, true) ?? "");
		}
		UIText.SetTextKey(this.TMP_Caption, "WikiWindow.caption", this, null);
		UIHyperText.selected_row_rt = null;
		UIText.cur_article = article;
		UIHyperText ht_TOC_Body = this.HT_TOC_Body;
		if (ht_TOC_Body != null)
		{
			ht_TOC_Body.Load((article != null) ? article.ht_def_field : null, this.toc_vars);
		}
		RectTransform selected_row_rt = UIHyperText.selected_row_rt;
		UIText.cur_article = article2;
		UIHyperText ht_Body = this.HT_Body;
		if (ht_Body != null)
		{
			ht_Body.Load((article2 != null) ? article2.ht_def_field : null, this.vars);
		}
		UIText.cur_article = null;
		UIHyperText.selected_row_rt = null;
		RectTransform rectTransform = base.transform as RectTransform;
		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		Canvas.ForceUpdateCanvases();
		if (scroll_toc && this.SR_TOC != null)
		{
			if (selected_row_rt == null)
			{
				if (this.TOCArticleID != this.last_toc_article_id)
				{
					this.SR_TOC.verticalNormalizedPosition = 1f;
				}
			}
			else
			{
				Rect worldRect = UICommon.GetWorldRect(this.HT_TOC_Body.transform as RectTransform);
				Rect worldRect2 = UICommon.GetWorldRect(selected_row_rt);
				Rect worldRect3 = UICommon.GetWorldRect(this.SR_TOC.transform as RectTransform);
				if (worldRect2.yMin < worldRect3.yMin || worldRect2.yMax > worldRect3.yMax)
				{
					float num = worldRect2.center.y - worldRect.yMin - worldRect3.height / 2f;
					float num2 = worldRect.height - worldRect3.height;
					float num3 = num / num2;
					num3 = Game.clamp(num3, 0f, 1f);
					this.SR_TOC.verticalNormalizedPosition = num3;
				}
			}
		}
		if (scroll_article && this.SR_Body != null && this.ArticleID != this.last_article_id)
		{
			this.SR_Body.verticalNormalizedPosition = 1f;
		}
		this.last_article_id = this.ArticleID;
		this.last_toc_article_id = this.TOCArticleID;
		if (this.Button_Back != null)
		{
			this.Button_Back.Enable(UIWikiWindow.history_idx > 0, false);
		}
		if (this.Button_Forward != null)
		{
			this.Button_Forward.Enable(UIWikiWindow.history_idx + 1 < UIWikiWindow.history.Count, false);
		}
		this.UpdateShortcutButtons();
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x0018B078 File Offset: 0x00189278
	private void FillVars(Vars vars, Wiki.Article article, string context)
	{
		vars.Clear();
		vars.obj = this.ResolveContext(context);
		vars.Set<string>("context_str", context);
		vars.Set<Value>("context_obj", vars.obj);
		if (!string.IsNullOrEmpty((article != null) ? article.context : null))
		{
			vars.Set<Value>(article.context, vars.obj);
		}
		vars.Set<Value>("cur_article", new Value(article));
		if (global::Defs.GetBool("WikiArticle", "use_player_kingdom", null, false))
		{
			vars.Set<Logic.Kingdom>("kingdom", BaseUI.LogicKingdom());
			return;
		}
		vars.Set<Value>("kingdom", Value.Null);
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x0018B120 File Offset: 0x00189320
	private Value ResolveContext(string context)
	{
		DT.Field defField = global::Defs.GetDefField(context, null);
		if (defField == null)
		{
			return Value.Unknown;
		}
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.def : null) != null);
		}
		if (flag)
		{
			return new Value(defField.def.def);
		}
		return new Value(defField);
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x0018B174 File Offset: 0x00189374
	private void AddToHistory(string article_id, string context, string toc_article_id, string toc_context)
	{
		if (UIWikiWindow.history_idx >= 0 && UIWikiWindow.history_idx < UIWikiWindow.history.Count)
		{
			UIWikiWindow.HistoryItem historyItem = UIWikiWindow.history[UIWikiWindow.history_idx];
			if (historyItem.article_id == article_id && historyItem.context == context && historyItem.toc_article_id == toc_article_id && historyItem.toc_context == toc_context)
			{
				return;
			}
		}
		this.DelHistory(UIWikiWindow.history_idx + 1, -1);
		UIWikiWindow.history.Add(new UIWikiWindow.HistoryItem(article_id, context, toc_article_id, toc_context));
		UIWikiWindow.history_idx = UIWikiWindow.history.Count - 1;
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x0018B218 File Offset: 0x00189418
	private void DelHistory(int idx, int count = -1)
	{
		if (count < 0)
		{
			count = UIWikiWindow.history.Count - idx;
		}
		if (idx < 0)
		{
			count += idx;
			idx = 0;
		}
		if (idx >= UIWikiWindow.history.Count)
		{
			return;
		}
		if (idx + count > UIWikiWindow.history.Count)
		{
			count = UIWikiWindow.history.Count - idx;
		}
		if (count <= 0)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			UIWikiWindow.HistoryItem.Del(UIWikiWindow.history[idx + i].uid);
		}
		UIWikiWindow.history.RemoveRange(idx, count);
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x0018B2A4 File Offset: 0x001894A4
	private void GoToHistory(UIWikiWindow.HistoryItem hi, int idx = -1)
	{
		if (hi == null)
		{
			return;
		}
		if (idx < 0)
		{
			idx = UIWikiWindow.history.IndexOf(hi);
			if (idx < 0)
			{
				return;
			}
		}
		this.RememberScrollPositions();
		UIWikiWindow.history_idx = idx;
		this.ArticleID = hi.article_id;
		this.Context = hi.context;
		bool flag = !UICommon.GetModifierKey(UICommon.ModifierKey.Shift);
		if (flag)
		{
			this.TOCArticleID = hi.toc_article_id;
			this.TOCContext = hi.toc_context;
		}
		this.Refresh(!flag || hi.toc_scroll < 0f, hi.scroll < 0f);
		if (flag && hi.toc_scroll >= 0f && this.SR_TOC != null)
		{
			this.SR_TOC.verticalNormalizedPosition = hi.toc_scroll;
		}
		if (hi.scroll >= 0f && this.SR_Body != null)
		{
			this.SR_Body.verticalNormalizedPosition = hi.scroll;
		}
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x0018B398 File Offset: 0x00189598
	private bool CloseHistory(bool refresh)
	{
		if (this.TOCArticleID != "History")
		{
			return false;
		}
		UIWikiWindow.HistoryItem historyItem = UIWikiWindow.CurHistoryItem();
		if (historyItem == null)
		{
			return false;
		}
		if (UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
		{
			return false;
		}
		this.TOCArticleID = historyItem.toc_article_id;
		this.TOCContext = historyItem.toc_context;
		if (refresh)
		{
			this.Refresh(true, true);
		}
		return true;
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x0018B3F4 File Offset: 0x001895F4
	private void Back()
	{
		int num = UIWikiWindow.history_idx - 1;
		if (num < 0 || num >= UIWikiWindow.history.Count)
		{
			return;
		}
		UIWikiWindow.HistoryItem hi = UIWikiWindow.history[num];
		this.GoToHistory(hi, num);
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x0018B430 File Offset: 0x00189630
	private void Forward()
	{
		int num = UIWikiWindow.history_idx + 1;
		if (num < 0 || num >= UIWikiWindow.history.Count)
		{
			return;
		}
		UIWikiWindow.HistoryItem hi = UIWikiWindow.history[num];
		this.GoToHistory(hi, num);
	}

	// Token: 0x060030EA RID: 12522 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButtonCLick(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x060030EB RID: 12523 RVA: 0x0018B46B File Offset: 0x0018966B
	private void HandleOnBackButtonCLick(BSGButton button)
	{
		this.Back();
	}

	// Token: 0x060030EC RID: 12524 RVA: 0x0018B473 File Offset: 0x00189673
	private void HandleOnForwardButtonCLick(BSGButton button)
	{
		this.Forward();
	}

	// Token: 0x060030ED RID: 12525 RVA: 0x0018B47B File Offset: 0x0018967B
	private void HandleOnHistoryButtonCLick(BSGButton button)
	{
		if (!this.CloseHistory(true))
		{
			this.SetData("History", null, false);
		}
	}

	// Token: 0x060030EE RID: 12526 RVA: 0x0018B493 File Offset: 0x00189693
	private void HandleOnShorcutButtonClick(BSGButton button)
	{
		this.SetData(button.name, null, true);
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x0018B4A4 File Offset: 0x001896A4
	protected override void Update()
	{
		base.Update();
		if (this.defs_version != global::Defs.Version && !string.IsNullOrEmpty(this.ArticleID))
		{
			this.Refresh(true, true);
		}
		if (UICommon.GetKeyDown(KeyCode.Mouse3, UICommon.ModifierKey.None, UICommon.ModifierKey.None) || UICommon.GetKeyDown(KeyCode.Backspace, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.Back();
		}
		if (UICommon.GetKeyDown(KeyCode.Mouse4, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.Forward();
		}
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x0018B50B File Offset: 0x0018970B
	private void OnEnable()
	{
		UIWikiWindow.current = this;
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x0018B513 File Offset: 0x00189713
	private void OnDisable()
	{
		if (UIWikiWindow.current == this)
		{
			UIWikiWindow.current = null;
		}
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x0018B528 File Offset: 0x00189728
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIWikiWindow.def_id, null);
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x0018B538 File Offset: 0x00189738
	public static GameObject DecideParent()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		if (baseUI.tutorial_message_container == null || baseUI.tutorial_mouse_blocker == null || !baseUI.tutorial_mouse_blocker.activeInHierarchy)
		{
			return baseUI.message_container;
		}
		return baseUI.tutorial_message_container;
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x0018B58C File Offset: 0x0018978C
	public static void UpdateParent(bool only_if_topmost)
	{
		if (UIWikiWindow.current == null)
		{
			return;
		}
		GameObject gameObject = UIWikiWindow.DecideParent();
		if (gameObject == null || gameObject == UIWikiWindow.current.transform.parent)
		{
			return;
		}
		if (only_if_topmost && UIWikiWindow.current.transform.GetSiblingIndex() != UIWikiWindow.current.transform.childCount - 1)
		{
			return;
		}
		UIWikiWindow.current.transform.SetParent(gameObject.transform);
		UIWikiWindow.current.transform.SetAsFirstSibling();
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x0018B618 File Offset: 0x00189818
	public static void ToggleOpen(string article_id, string context = null)
	{
		if (article_id == "" && UIWikiWindow.current != null)
		{
			article_id = null;
		}
		if (article_id == null)
		{
			if (UIWikiWindow.current != null)
			{
				UIWikiWindow.current.Close(false);
			}
			return;
		}
		GameObject gameObject = UIWikiWindow.DecideParent();
		if (gameObject == null)
		{
			return;
		}
		if (UIWikiWindow.current != null)
		{
			UIWikiWindow.current.transform.SetParent(gameObject.transform);
			UIWikiWindow.current.transform.SetAsLastSibling();
			UIWikiWindow.current.SetData(article_id, context, true);
			return;
		}
		if (WorldUI.Get() == null)
		{
			return;
		}
		GameObject prefab = UIWikiWindow.GetPrefab();
		if (prefab == null)
		{
			return;
		}
		UIWikiWindow.current = UIWikiWindow.Create(article_id, context, prefab, gameObject.transform as RectTransform);
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x0018B6E4 File Offset: 0x001898E4
	public static void ToggleOpen(string article_id, string context, string toc_article_id, string toc_context)
	{
		GameObject gameObject = UIWikiWindow.DecideParent();
		if (gameObject == null)
		{
			return;
		}
		if (UIWikiWindow.current != null)
		{
			UIWikiWindow.current.transform.SetParent(gameObject.transform);
			UIWikiWindow.current.transform.SetAsLastSibling();
			UIWikiWindow.current.SetData(article_id, context, toc_article_id, toc_context, true);
			return;
		}
		GameObject prefab = UIWikiWindow.GetPrefab();
		if (prefab == null)
		{
			return;
		}
		UIWikiWindow.current = UIWikiWindow.Create(article_id, context, toc_article_id, toc_context, prefab, gameObject.transform as RectTransform);
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x0018B76C File Offset: 0x0018996C
	public static UIWikiWindow Get()
	{
		return UIWikiWindow.current;
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x0018B773 File Offset: 0x00189973
	public static bool IsActive()
	{
		return UIWikiWindow.current != null;
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x0018B780 File Offset: 0x00189980
	public static UIWikiWindow Create(string article_id, string context, GameObject prototype, RectTransform parent)
	{
		Debug.Log("Parrent: " + parent);
		if (prototype == null)
		{
			return null;
		}
		if (article_id == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
		UIWikiWindow uiwikiWindow = gameObject.GetComponent<UIWikiWindow>();
		if (uiwikiWindow == null)
		{
			uiwikiWindow = gameObject.AddComponent<UIWikiWindow>();
		}
		uiwikiWindow.SetData(article_id, context, true);
		uiwikiWindow.Open();
		return uiwikiWindow;
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x0018B7F0 File Offset: 0x001899F0
	public static UIWikiWindow Create(string article_id, string context, string toc_article_id, string toc_context, GameObject prototype, RectTransform parent)
	{
		Debug.Log("Parrent: " + parent);
		if (prototype == null)
		{
			return null;
		}
		if (article_id == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
		UIWikiWindow uiwikiWindow = gameObject.GetComponent<UIWikiWindow>();
		if (uiwikiWindow == null)
		{
			uiwikiWindow = gameObject.AddComponent<UIWikiWindow>();
		}
		uiwikiWindow.SetData(article_id, context, toc_article_id, toc_context, true);
		uiwikiWindow.Open();
		return uiwikiWindow;
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x0018B864 File Offset: 0x00189A64
	public static bool HandleLinkClick(UIText.LinkInfo link, object link_obj)
	{
		if (link.type == "obj" || link.type == "def")
		{
			return false;
		}
		DT.Field field = null;
		if (link_obj != null)
		{
			Wiki.Article article;
			if ((article = (link_obj as Wiki.Article)) != null)
			{
				Wiki.Article article2 = article;
				UIWikiWindow uiwikiWindow = UIWikiWindow.current;
				if (uiwikiWindow != null)
				{
					uiwikiWindow.CloseHistory(false);
				}
				UIWikiWindow.ToggleOpen(article2.id, null);
				return true;
			}
			Wiki.Keyword keyword;
			if ((keyword = (link_obj as Wiki.Keyword)) == null)
			{
				UIWikiWindow.HistoryItem historyItem;
				if ((historyItem = (link_obj as UIWikiWindow.HistoryItem)) == null)
				{
					DT.Field field2;
					if ((field2 = (link_obj as DT.Field)) == null)
					{
						DT.Def def;
						if ((def = (link_obj as DT.Def)) == null)
						{
							Def def2;
							if ((def2 = (link_obj as Def)) != null)
							{
								field = def2.field;
							}
						}
						else
						{
							field = def.field;
						}
					}
					else
					{
						field = field2;
					}
				}
				else
				{
					UIWikiWindow.HistoryItem hi = historyItem;
					if (UIWikiWindow.current == null)
					{
						return false;
					}
					UIWikiWindow.current.GoToHistory(hi, -1);
					return true;
				}
			}
			else
			{
				Wiki.Keyword keyword2 = keyword;
				if (keyword2.articles == null || keyword2.articles.Count == 0)
				{
					return true;
				}
				UIWikiWindow uiwikiWindow2 = UIWikiWindow.current;
				if (uiwikiWindow2 != null)
				{
					uiwikiWindow2.CloseHistory(false);
				}
				Wiki.Article article3 = keyword2.articles[0];
				if (keyword2.articles.Count == 1)
				{
					UIWikiWindow.ToggleOpen(article3.id, null);
					return true;
				}
				UIWikiWindow.ToggleOpen(article3.id, null, "Keyword", keyword2.keyword);
				return true;
			}
		}
		if (field == null)
		{
			return false;
		}
		DT.Field field3 = field.BaseRoot();
		Wiki wiki = Wiki.Get();
		Wiki.Article article4 = (wiki != null) ? wiki.FindArticle(field3.key) : null;
		if (article4 == null)
		{
			return false;
		}
		string context = field.Path(false, false, '.');
		UIWikiWindow uiwikiWindow3 = UIWikiWindow.current;
		if (uiwikiWindow3 != null)
		{
			uiwikiWindow3.CloseHistory(false);
		}
		UIWikiWindow.ToggleOpen(article4.id, context);
		return true;
	}

	// Token: 0x040020B4 RID: 8372
	private static string def_id = "WikiWindow";

	// Token: 0x040020B5 RID: 8373
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI TMP_Caption;

	// Token: 0x040020B6 RID: 8374
	[UIFieldTarget("id_TOC_Body")]
	private UIHyperText HT_TOC_Body;

	// Token: 0x040020B7 RID: 8375
	private ScrollRect SR_TOC;

	// Token: 0x040020B8 RID: 8376
	[UIFieldTarget("id_Body")]
	private UIHyperText HT_Body;

	// Token: 0x040020B9 RID: 8377
	private ScrollRect SR_Body;

	// Token: 0x040020BA RID: 8378
	[UIFieldTarget("id_Button_Close")]
	private BSGButton Button_Close;

	// Token: 0x040020BB RID: 8379
	[UIFieldTarget("id_Button_Back")]
	private BSGButton Button_Back;

	// Token: 0x040020BC RID: 8380
	[UIFieldTarget("id_Button_Forward")]
	private BSGButton Button_Forward;

	// Token: 0x040020BD RID: 8381
	[UIFieldTarget("id_Button_History")]
	private BSGButton Button_History;

	// Token: 0x040020BE RID: 8382
	[UIFieldTarget("id_ShortcutButtons")]
	private GameObject Shortcut_Buttons;

	// Token: 0x040020BF RID: 8383
	private List<BSGButton> shortcut_buttons = new List<BSGButton>();

	// Token: 0x040020C0 RID: 8384
	public string TOCArticleID;

	// Token: 0x040020C1 RID: 8385
	public string TOCContext;

	// Token: 0x040020C2 RID: 8386
	public string ArticleID;

	// Token: 0x040020C3 RID: 8387
	public string Context;

	// Token: 0x040020C4 RID: 8388
	public Vars vars = new Vars();

	// Token: 0x040020C5 RID: 8389
	public Vars toc_vars = new Vars();

	// Token: 0x040020C6 RID: 8390
	public static List<UIWikiWindow.HistoryItem> history = new List<UIWikiWindow.HistoryItem>();

	// Token: 0x040020C7 RID: 8391
	public static int history_idx = -1;

	// Token: 0x040020C8 RID: 8392
	private int defs_version;

	// Token: 0x040020C9 RID: 8393
	private string last_article_id;

	// Token: 0x040020CA RID: 8394
	private string last_toc_article_id;

	// Token: 0x040020CB RID: 8395
	private static UIWikiWindow current;

	// Token: 0x02000875 RID: 2165
	public class HistoryItem : IVars
	{
		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x0600513F RID: 20799 RVA: 0x0023C39D File Offset: 0x0023A59D
		public Wiki.Article article
		{
			get
			{
				Wiki wiki = Wiki.Get();
				if (wiki == null)
				{
					return null;
				}
				return wiki.FindArticle(this.article_id);
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06005140 RID: 20800 RVA: 0x0023C3B5 File Offset: 0x0023A5B5
		public Wiki.Article toc_article
		{
			get
			{
				Wiki wiki = Wiki.Get();
				if (wiki == null)
				{
					return null;
				}
				return wiki.FindArticle(this.toc_article_id);
			}
		}

		// Token: 0x06005141 RID: 20801 RVA: 0x0023C3D0 File Offset: 0x0023A5D0
		public HistoryItem(string article_id, string context, string toc_article_id, string toc_context)
		{
			this.uid = ++UIWikiWindow.HistoryItem.last_uid;
			this.article_id = article_id;
			this.context = context;
			this.toc_article_id = toc_article_id;
			this.toc_context = toc_context;
			UIWikiWindow.HistoryItem.registry.Add(this.uid, this);
		}

		// Token: 0x06005142 RID: 20802 RVA: 0x0023C43C File Offset: 0x0023A63C
		public static UIWikiWindow.HistoryItem Get(int uid)
		{
			UIWikiWindow.HistoryItem result;
			if (!UIWikiWindow.HistoryItem.registry.TryGetValue(uid, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x06005143 RID: 20803 RVA: 0x0023C45B File Offset: 0x0023A65B
		public static void Del(int uid)
		{
			UIWikiWindow.HistoryItem.registry.Remove(uid);
		}

		// Token: 0x06005144 RID: 20804 RVA: 0x0023C46C File Offset: 0x0023A66C
		public Value GetVar(string key, IVars vars, bool as_value)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1309284212U)
			{
				if (num <= 269097364U)
				{
					if (num != 232457833U)
					{
						if (num == 269097364U)
						{
							if (key == "toc_article_id")
							{
								return this.toc_article_id;
							}
						}
					}
					else if (key == "link")
					{
						return string.Format("#<link=\"wiki_history:{0}\">{1}</link>", this.uid, this.caption);
					}
				}
				else if (num != 392652633U)
				{
					if (num == 1309284212U)
					{
						if (key == "selected")
						{
							return UIWikiWindow.CurHistoryItem() == this;
						}
					}
				}
				else if (key == "article")
				{
					return new Value(this.article);
				}
			}
			else if (num <= 2357661820U)
			{
				if (num != 1556604621U)
				{
					if (num == 2357661820U)
					{
						if (key == "context")
						{
							return this.context;
						}
					}
				}
				else if (key == "uid")
				{
					return this.uid;
				}
			}
			else if (num != 2766234751U)
			{
				if (num != 3255428185U)
				{
					if (num == 4011007077U)
					{
						if (key == "caption")
						{
							return "#" + this.caption;
						}
					}
				}
				else if (key == "toc_context")
				{
					return this.toc_context;
				}
			}
			else if (key == "article_id")
			{
				return this.article_id;
			}
			return Value.Unknown;
		}

		// Token: 0x06005145 RID: 20805 RVA: 0x0023C64C File Offset: 0x0023A84C
		public override string ToString()
		{
			string text = string.Format("[{0}] {1}", this.uid, this.article_id);
			if (!string.IsNullOrEmpty(this.context))
			{
				text = text + "(" + this.context + ")";
			}
			if (!string.IsNullOrEmpty(this.toc_article_id))
			{
				text = text + "|" + this.toc_article_id;
				if (!string.IsNullOrEmpty(this.toc_context))
				{
					text = text + "(" + this.toc_context + ")";
				}
			}
			return text;
		}

		// Token: 0x04003F43 RID: 16195
		public static int last_uid = 0;

		// Token: 0x04003F44 RID: 16196
		public static Dictionary<int, UIWikiWindow.HistoryItem> registry = new Dictionary<int, UIWikiWindow.HistoryItem>();

		// Token: 0x04003F45 RID: 16197
		public int uid;

		// Token: 0x04003F46 RID: 16198
		public string article_id;

		// Token: 0x04003F47 RID: 16199
		public string context;

		// Token: 0x04003F48 RID: 16200
		public float scroll = -1f;

		// Token: 0x04003F49 RID: 16201
		public string toc_article_id;

		// Token: 0x04003F4A RID: 16202
		public string toc_context;

		// Token: 0x04003F4B RID: 16203
		public float toc_scroll = -1f;

		// Token: 0x04003F4C RID: 16204
		public string caption;
	}
}
