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
			bundles.Add(new ScriptBundle("~/js/jquery-2").Include(
			"~/assets/plugins/jquery-2.1.1/jquery-{version}.js"
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

			// bootgrid
			bundles.Add(new ScriptBundle("~/js/bootgrid").Include(
							"~/assets/plugins/jquery.bootgrid-1.1.4/jquery.bootgrid.js"));
			bundles.Add(new StyleBundle("~/css/bootgrid").Include(
							"~/assets/plugins/jquery.bootgrid-1.1.4/jquery.bootgrid.css"));

			// tagsinput
			bundles.Add(new ScriptBundle("~/js/tagsinput").Include(
							"~/assets/plugins/bootstrap-tagsinput-0.4.2/bootstrap-tagsinput.js",
							"~/assets/plugins/typeahead-0.10.5/typeahead.bundle.js"));
			bundles.Add(new StyleBundle("~/css/tagsinput").Include(
							"~/assets/plugins/bootstrap-tagsinput-0.4.2/bootstrap-tagsinput.css"));

			// multiselect
			bundles.Add(new ScriptBundle("~/js/multiselect").Include(
							"~/assets/plugins/bootstrap-multiselect-0.9.8/dist/js/bootstrap-multiselect.js"));
			bundles.Add(new StyleBundle("~/css/multiselect").Include(
							"~/assets/plugins/bootstrap-multiselect-0.9.8/dist/css/bootstrap-multiselect.css"));

			// alertify
			bundles.Add(new ScriptBundle("~/js/alertify").Include(
							"~/assets/plugins/alertify.js-0.3.11/src/alertify.js"));
			bundles.Add(new StyleBundle("~/css/alertify").Include(
							"~/assets/plugins/alertify.js-0.3.11/themes/alertify.core.css",
							"~/assets/plugins/alertify.js-0.3.11/themes/alertify.bootstrap.css"));

			// dropzone
			bundles.Add(new ScriptBundle("~/js/dropzone").Include(
							"~/assets/plugins/dropzone-4.0.0/dropzone.js"));

			// uploadify
			bundles.Add(new ScriptBundle("~/js/uploadify").Include(
							"~/assets/plugins/uploadify-2.2/jquery.uploadify.js"));
			bundles.Add(new StyleBundle("~/css/uploadify").Include(
							"~/assets/plugins/uploadify-2.2/uploadify.css"));

			// knockout
			bundles.Add(new ScriptBundle("~/js/knockout").Include(
							"~/assets/plugins/knockout-3.2.0/knockout-3.2.0.js"));

			// summernote
			bundles.Add(new ScriptBundle("~/js/summernote").Include(
							"~/assets/plugins/summernote-0.6.0/dist/summernote.js",
							"~/assets/plugins/summernote-0.6.0/lang/summernote-zh-CN.js"));
			bundles.Add(new StyleBundle("~/css/summernote").Include(
							"~/assets/plugins/summernote-0.6.0/dist/summernote.css"));

			// jstree
			bundles.Add(new ScriptBundle("~/js/jstree").Include(
							"~/assets/plugins/jstree-3.1.1/dist/jstree.js"));
			bundles.Add(new StyleBundle("~/css/jstree").Include(
							"~/assets/plugins/jstree-3.1.1/dist/themes/default/style.css"));

			// sparkline
			bundles.Add(new ScriptBundle("~/js/chart/sparkline").Include(
							"~/assets/plugins/jquery.sparkline-2.1.2/dist/jquery.sparkline.js"));

			// chart - flot
			bundles.Add(new ScriptBundle("~/js/chart/flot").Include(
							"~/assets/plugins/flot-0.8.3/jquery.flot.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.time.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.resize.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.pie.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.stack.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.crosshair.js",
							"~/assets/plugins/flot-0.8.3/jquery.flot.categories.js",
							"~/assets/plugins/flot.tooltip-0.8.4/js/jquery.flot.tooltip.js"
							));

			// doT
			bundles.Add(new ScriptBundle("~/js/doT").Include(
							"~/assets/plugins/doT-1.0.3/doT.js"));

			// themes - king
			bundles.Add(new ScriptBundle("~/js/themes/king").Include(
							"~/assets/themes/king/js/king-common.js"/*,
							"~/assets/themes/king/js/deliswitch.js"*/));
			bundles.Add(new StyleBundle("~/css/themes/king").Include(
							"~/assets/themes/king/css/main.css"/*,
							"~/assets/themes/king/css/style-switcher.css"*/));



			// app
			bundles.Add(new ScriptBundle("~/js/app").Include(
							"~/assets/js/app.js"));
			bundles.Add(new StyleBundle("~/css/app").Include(
							"~/assets/css/app.css"));

		}
	}
}
