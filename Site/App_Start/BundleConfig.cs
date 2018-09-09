using System.Web;
using System.Web.Optimization;

namespace Res
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{

			//
			// jquery
			bundles.Add(new ScriptBundle("~/js/jquery").Include(
			"~/assets/plugins/jquery-1.11.1/jquery-{version}.js"
			));

			// jquery.validate
			bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
			"~/assets/plugins/jquery.validate-1.13.1/dist/jquery.validate.js",
			"~/assets/plugins/jquery.validate-1.13.1/dist/localization/messages_zh.js",
			"~/assets/plugins/jquery.validate.unobtrusive/jquery.validate.unobtrusive.js",
			"~/assets/plugins/jquery.validate.bootstrap/jquery.validate.bootstrap.js"
			));

			// modernizr
			bundles.Add(new ScriptBundle("~/js/modernizr").Include(
			"~/assets/plugins/modernizr-2.6.2/modernizr-*"));

			// bootstrap
			bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
			"~/assets/plugins/bootstrap-3.3.2/js/bootstrap.js",
			"~/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
			"~/assets/plugins/bootstrap-datepicker/js/locales/bootstrap-datepicker.zh-CN.js",
			"~/assets/plugins/bootstrap-switch-3.3.2/dist/js/bootstrap-switch.js"
			));
			// bootstrap & font-awesome
			bundles.Add(new StyleBundle("~/css/bootstrap").Include(
			"~/assets/plugins/font-awesome-4.2.0/css/font-awesome.css",
			"~/assets/plugins/bootstrap-3.3.2/css/bootstrap.css",
			"~/assets/plugins/bootstrap-datepicker/css/datepicker.css",
			"~/assets/plugins/bootstrap-switch-3.3.2/dist/css/bootstrap3/bootstrap-switch.css"
			));

			// tagsinput
			bundles.Add(new ScriptBundle("~/js/tagsinput").Include(
							"~/assets/plugins/bootstrap-tagsinput-0.4.2/bootstrap-tagsinput.js",
							"~/assets/plugins/typeahead-0.10.5/typeahead.bundle.js"));
			bundles.Add(new StyleBundle("~/css/tagsinput").Include(
							"~/assets/plugins/bootstrap-tagsinput-0.4.2/bootstrap-tagsinput.css"));

			// dropzone
			bundles.Add(new ScriptBundle("~/js/dropzone").Include(
							"~/assets/plugins/dropzone-4.0.0/dropzone.js"));

			//	zeroclipboard
			bundles.Add(new ScriptBundle("~/js/zeroclipboard").Include(
							"~/assets/plugins/zeroclipboard-2.2.0/ZeroClipboard.min.js"));

			// doT
			bundles.Add(new ScriptBundle("~/js/doT").Include(
							"~/assets/plugins/doT-1.0.3/doT.js"));

			// app
			bundles.Add(new ScriptBundle("~/js/app").Include(
							"~/assets/js/app.js"));
			bundles.Add(new StyleBundle("~/css/app").Include(
							"~/assets/css/app.css"));
			bundles.Add(new StyleBundle("~/css/app2").Include(
					"~/assets/css/app2.css"));



			//众筹
			bundles.Add(new ScriptBundle("~/js/Ace").Include(
					"~/assets/js/Ace.js"));
			bundles.Add(new StyleBundle("~/css/Ace").Include(
							"~/assets/css/Ace.css"));

			bundles.Add(new ScriptBundle("~/css/bootstrapzc").Include(
			"~/assets/plugins/font-awesome-4.2.0/css/font-awesome.css",
					"~/assets/css/bootstrap.css"));
		   bundles.Add(new ScriptBundle("~/js/jqueryzc").Include(
					"~/assets/js/jquery-1.11.2.min.js"));
			  bundles.Add(new ScriptBundle("~/js/bootstrapzc").Include(
					"~/assets/js/bootstrap.js"));


			//	展示平台
			bundles.Add(new StyleBundle("~/css/app_zs").Include(
							"~/assets/css/app-zs.css"));

		}
	}
}
