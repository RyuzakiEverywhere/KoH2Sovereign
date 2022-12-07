using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class UIWikiTooltip : MonoBehaviour, Tooltip.IHandler
{
	// Token: 0x060030D0 RID: 12496 RVA: 0x0018A231 File Offset: 0x00188431
	private void Awake()
	{
		this.caption = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_Caption");
		this.body = global::Common.FindChildComponent<UIHyperText>(base.gameObject, "id_Body");
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x0018A260 File Offset: 0x00188460
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt != Tooltip.Event.Fill)
		{
			return false;
		}
		if (((tooltip != null) ? tooltip.vars : null) == null)
		{
			return false;
		}
		DT.Field field = tooltip.vars.Get("article_def", false).Get<DT.Field>();
		if (field == null)
		{
			return false;
		}
		Wiki wiki = Wiki.Get();
		Wiki.Article article = (wiki != null) ? wiki.FindArticle(field.key) : null;
		if (article == null)
		{
			return false;
		}
		UIText.cur_article = article;
		UIText.SetText(this.caption, article.caption_field, null, null);
		UIHyperText uihyperText = this.body;
		if (uihyperText != null)
		{
			uihyperText.Load(article.ht_def_field, null);
		}
		UIText.cur_article = null;
		return true;
	}

	// Token: 0x040020B2 RID: 8370
	private TextMeshProUGUI caption;

	// Token: 0x040020B3 RID: 8371
	private UIHyperText body;
}
