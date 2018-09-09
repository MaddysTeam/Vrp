using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Business.Utils
{
	public class ResModelBinder : DefaultModelBinder
	{
		protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			object value = propertyBinder.BindModel(controllerContext, bindingContext);

			return value;
		}
	}
}