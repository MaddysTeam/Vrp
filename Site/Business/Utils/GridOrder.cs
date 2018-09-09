using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Business
{

	public class GridOrder
	{

		public string Id { get; set; }
		public APSqlOrderAccording Order { get; set; }

		public static GridOrder GetSortDef(FormCollection fc)
		{
			foreach (string key in fc.AllKeys)
			{
				if (key.StartsWith("sort["))
				{
					return new GridOrder()
					{
						Id = key.Substring(5, key.Length - 6),
						Order = (fc[key] == "asc") ? APSqlOrderAccording.Asc : APSqlOrderAccording.Desc
					};
				}
			}

			return null;
		}

	}

}