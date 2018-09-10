using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symber.Web.Report
{

	public class PickListAPRptColumn : APRptColumn
	{

		#region [ Fields ]


		private string _innerKey;
		private bool _ajaxLoad;


		#endregion


		#region [ Constructors ]


		public PickListAPRptColumn(Int64APColumnDef columnDef, string innerKey)
			: base(columnDef)
		{
			_innerKey = innerKey;
		}


		public PickListAPRptColumn(Int64APColumnDef columnDef, string id, string title, string innerKey)
			: base(columnDef, id, title)
		{
			_innerKey = innerKey;
		}


		#endregion


		#region [ Properties ]


		public string PickListInnerKey
		{
			get { return _innerKey; }
		}


		public bool AjaxLoad
		{
			get { return _ajaxLoad; }
			set { _ajaxLoad = value; }
		}


		#endregion


		#region [ Methods ]


		public IEnumerable<System.Web.Mvc.SelectListItem> GetSelectList()
		{
			var cache = APBplDef.ResPickListBpl.Cached(_innerKey);
			var defaultItem = cache.DefaultItem;
			foreach (var item in cache.Items)
			{
				yield return new System.Web.Mvc.SelectListItem()
				{
					Value = item.PickListItemId.ToString(),
					Text = item.Name,
					Selected = (defaultItem != null && defaultItem.PickListItemId == item.PickListItemId)
				};
			}
		}
		public IEnumerable<System.Web.Mvc.SelectListItem> GetSelectList(long strengthValue)
		{
			var cache = APBplDef.ResPickListBpl.Cached(_innerKey);
			var defaultItem = cache.DefaultItem;
			foreach (var item in cache.Items)
			{
				if (item.StrengthenValue == strengthValue)
				{
					yield return new System.Web.Mvc.SelectListItem()
					{
						Value = item.PickListItemId.ToString(),
						Text = item.Name,
						Selected = (defaultItem != null && defaultItem.PickListItemId == item.PickListItemId)
					};
				}
			}
		}

		public IEnumerable<System.Web.Mvc.SelectListItem> GetSelectListExclude(params long[] ids)
		{
			var cache = APBplDef.ResPickListBpl.Cached(_innerKey);
			var defaultItem = cache.DefaultItem;
			foreach (var item in cache.Items)
			{
				if (Array.IndexOf(ids, item.PickListItemId) == -1)
					yield return new System.Web.Mvc.SelectListItem()
					{
						Value = item.PickListItemId.ToString(),
						Text = item.Name,
						Selected = (defaultItem != null && defaultItem.PickListItemId == item.PickListItemId)
					};
			}
		}

		public string GetName(long pickListItemPKID)
		{
			return APBplDef.ResPickListBpl.Cached(_innerKey).ItemName(pickListItemPKID);
		}


		public long GetPKID()
		{
			return APBplDef.ResPickListBpl.Cached(_innerKey).PKID;
		}


		public List<ResPickListItem> GetItems()
		{
			return APBplDef.ResPickListBpl.Cached(_innerKey).Items;
		}


		#endregion


		#region [ Override Implementation of APColumnEx ]



		public override APSqlWherePhrase ParseQueryWherePhrase(APRptFilterComparator comparator, params string[] values)
		{
			throw new NotImplementedException();
		}


		#endregion
	}

}