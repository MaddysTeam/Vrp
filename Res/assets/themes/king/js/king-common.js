﻿$(document).ready(function () {

	function e() {
		$(window).width() < 977
		? $(".left-sidebar").hasClass("minified") && ($(".left-sidebar").removeClass("minified"), $(".left-sidebar").addClass("init-minified"))
		: $(".left-sidebar").hasClass("init-minified") && $(".left-sidebar").removeClass("init-minified").addClass("minified");
	}

	function t(e, t) {
		$(".widget-ajax .alert").removeClass("alert-info").addClass("alert-success").find("span").text(t);
		$(".widget-ajax .alert").find("i").removeClass().addClass("fa fa-check-circle");
		e.prop("disabled", !1);
		e.find("i").removeClass().addClass("fa fa-floppy-o");
		e.find("span").text("Update");
	}

	function o() {
		if (3 > a) {
			$globalVolume = localStorage.getItem("global-volume");
			if (null == $globalVolume || "1" == $globalVolume) {
				l.play(); i = setTimeout(o, 8e3); a++;
			}
		}
	}

	function n(e, t) {
		null == t || "1" == t
		? e.removeClass("fa-volume-off").addClass("fa-volume-up")
		: e.removeClass("fa-volume-up").addClass("fa-volume-off");
	}

	$(".main-menu .js-sub-menu-toggle").click(function (e) {
		e.preventDefault();
		$li = $(this).parents("li");
		if ($li.hasClass("active")) {
			$li.find(".toggle-icon").removeClass("fa-angle-down").addClass("fa-angle-left");
			$li.removeClass("active");
		} else {
			$li.find(".toggle-icon").removeClass("fa-angle-left").addClass("fa-angle-down");
			$li.addClass("active");
		}
		$li.find(".sub-menu").slideToggle(300);
	});

	$(".js-toggle-minified").clickToggle(
		function () {
			$(".left-sidebar").addClass("minified");
			$(".content-wrapper").addClass("expanded");
			$(".left-sidebar .sub-menu").css("display", "none").css("overflow", "hidden");
			$(".sidebar-minified").find("i.fa-angle-left").toggleClass("fa-angle-right");
		},
		function () {
			$(".left-sidebar").removeClass("minified");
			$(".content-wrapper").removeClass("expanded");
			$(".sidebar-minified").find("i.fa-angle-left").toggleClass("fa-angle-right");
		}
	);

	$(".main-nav-toggle").clickToggle(
		function () { $(".left-sidebar").slideDown(300) },
		function () { $(".left-sidebar").slideUp(300) }
	);

	$mainContentCopy = $(".main-content").clone();

	$('.searchbox input[type="search"]').keydown(function () {
		var e = $(this);
		setTimeout(function () {
			var t = e.val();

			// changed by leocaan, if chinese code trigger on 2 chars.
			var len = t.match(new RegExp("[u4e00-u9fa5]")) ? 2 : 1;
			if (t.length > len) {
				var o = new RegExp(t, "i"), n = [];
				$(".widget-header h3").each(function () {
					var e = $(this).text().match(o);
					if ("" != e && null != e)
						n.push($(this).parents(".widget"));
				});
				n.length > 0 ? ($(".main-content .widget").hide(), $.each(n, function (e, t) { t.show() })) : console.log("widget not found");
			} else
				$(".main-content .widget").show();
		}, 0);
	});

	$(".widget .btn-remove").click(function (e) {
		e.preventDefault();
		$(this).parents(".widget").fadeOut(300, function () {
			$(this).remove();
		});
	});

	$(".widget .btn-toggle-expand").clickToggle(
		function (e) {
			e.preventDefault();
			$(this).parents(".widget").find(".widget-content").slideUp(300);
			$(this).find("i.fa-chevron-up").toggleClass("fa-chevron-down");
		},
		function (e) {
			e.preventDefault();
			$(this).parents(".widget").find(".widget-content").slideDown(300);
			$(this).find("i.fa-chevron-up").toggleClass("fa-chevron-down");
		}
	);

	$(".widget .btn-focus").clickToggle(
		function (e) {
			e.preventDefault();
			$(this).find("i.fa-eye").toggleClass("fa-eye-slash");
			$(this).parents(".widget").find(".btn-remove").addClass("link-disabled");
			$(this).parents(".widget").addClass("widget-focus-enabled");
			$('<div id="focus-overlay"></div>').hide().appendTo("body").fadeIn(300);
		},
		function (e) {
			e.preventDefault();
			$theWidget = $(this).parents(".widget");
			$(this).find("i.fa-eye").toggleClass("fa-eye-slash");
			$theWidget.find(".btn-remove").removeClass("link-disabled");
			$("body").find("#focus-overlay").fadeOut(function () {
				$(this).remove(),
				$theWidget.removeClass("widget-focus-enabled");
			});
		}
	);

	$(window).bind("resize", e);

	$("body").tooltip({ selector: "[data-toggle=tooltip]", container: "body" });

	$(".alert .close").click(function (e) {
		e.preventDefault();
		$(this).parents(".alert").fadeOut(300);
	});

	$(".btn-help").popover({
		container: "body",
		placement: "top",
		html: !0,
		title: '<i class="fa fa-book"></i> Help',
		content: "Help summary goes here. Options can be passed via data attributes <code>data-</code> or JavaScript. Please check <a href='http://getbootstrap.com/javascript/#popovers'>Bootstrap Doc</a>"
	});

	$(".demo-popover1 #popover-title").popover({
		html: !0,
		title: '<i class="fa fa-cogs"></i> Popover Title',
		content: "This popover has title and support HTML content. Quickly implement process-centric networks rather than compelling potentialities. Objectively reinvent competitive technologies after high standards in process improvements. Phosfluorescently cultivate 24/365."
	});

	$(".demo-popover1 #popover-hover").popover({
		html: !0,
		title: '<i class="fa fa-cogs"></i> Popover Title',
		trigger: "hover",
		content: "Activate the popover on hover. Objectively enable optimal opportunities without market positioning expertise. Assertively optimize multidisciplinary benefits rather than holistic experiences. Credibly underwhelm real-time paradigms with."
	});

	$(".demo-popover2 .btn").popover();

	$(".widget-header-toolbar .btn-ajax").click(function (e) {
		e.preventDefault(),
		$theButton = $(this),
		$.ajax({
			url: "php/widget-ajax.php",
			type: "POST",
			dataType: "json",
			cache: !1,
			beforeSend: function () {
				$theButton.prop("disabled", !0);
				$theButton.find("i").removeClass().addClass("fa fa-spinner fa-spin");
				$theButton.find("span").text("Loading...");
			},
			success: function (e) {
				setTimeout(function () { t($theButton, e.msg) }, 1e3);
			},
			error: function (e, t, o) {
				console.log("AJAX ERROR: \n" + o);
			}
		});
	});

	if ($(".widget-header .multiselect").length > 0) {
		$(".widget-header .multiselect").multiselect({
			dropRight: !0,
			buttonClass: "btn btn-warning btn-sm"
		});
	}

	if ($(".today-reminder").length > 0) {
		var i, a = 0, l = new Audio;
		l.src = navigator.userAgent.match("Firefox/") ? "assets/audio/bell-ringing.ogg" : "assets/audio/bell-ringing.mp3", o();
	}

	if ($(".bs-switch").length > 0) {
		$(".bs-switch").bootstrapSwitch();
	}

	// if you page want full height, set same element cssClass="full-page"
	$(".full-page").length > 0 && (
		$(".content-wrapper").css("min-height", $(".wrapper").outerHeight(!0) - $(".top-bar").outerHeight(!0))
	)

	//var s = new Tour({
	//	basePath: "/dashboard/kingadmin-v1.3/",
	//	steps: [{
	//		element: "#tour-searchbox", title: 'Bootstrap Tour <span class="badge element-bg-color-blue">New</span>', content: "<p>Hello there, this tour can <strong>guide new user</strong> or show new feature/update on your website. Oh and why don't you try search <em>'sales'</em> here to see Widget Live Search on action.</p><p><em>To navigate the site please follow the tour first or click \"End tour\".</em></p>", placement: "bottom"
	//	},
	//	{ element: "#sales-stat-tab", title: "Dynamic Content", content: "You can select statistic view based on predefined period here." },
	//	{ element: "#tour-focus", title: "Focus Mode", content: "Focus your attention and hide all irrelevant contents.", placement: "left" },
	//	{ element: ".del-switcher-toggle", title: "Color Skins", content: "Open the hidden panel here by clicking this icon to apply built-in skins.", placement: "left", backdrop: !0 },
	//	{ element: "#start-tour", title: "Start/Restart Tour", content: "Click this link later if you need to <strong>restart the tour</strong>.", placement: "bottom", backdrop: !0 }],
	//	template: "<div class='popover tour'> <div class='arrow'></div> <h3 class='popover-title'></h3><div class='popover-content'></div><div class='popover-navigation'><button class='btn btn-default' data-role='prev'>芦 Prev</button><button class='btn btn-primary' data-role='next'>Next 禄</button><button class='btn btn-default' data-role='end'>End tour</button></div></div>",
	//	onEnd: function () {
	//		$("#start-tour").prop("disabled", !1)
	//	}
	//});

	if ($(".top-general-alert").length > 0 && null == localStorage.getItem("general-alert")) {
		$(".top-general-alert").delay(800).slideDown("medium");
		$(".top-general-alert .close").click(function () {
			$(this).parent().slideUp("fast");
			localStorage.setItem("general-alert", "closed");
		});
	};

	//s.init(),

	//$("#start-tour").click(function () {
	//	s.ended() ? s.restart() : s.start(), $(this).prop("disabled", !0)
	//}),

	$btnGlobalvol = $(".btn-global-volume");

	$theIcon = $btnGlobalvol.find("i");

	n($theIcon, localStorage.getItem("global-volume"));

	$btnGlobalvol.click(function () {
		var e = localStorage.getItem("global-volume");
		null == e || "1" == e ? localStorage.setItem("global-volume", 0) : localStorage.setItem("global-volume", 1);
		n($theIcon, localStorage.getItem("global-volume"));
	});

	if ($(".select2").length > 0)
		$(".select2").select2();
	if ($(".select2-multiple").length > 0)
		$(".select2-multiple").select2();
});


$.fn.clickToggle = function (e, t) {
	return this.each(function () {
		var o = !1;
		$(this).bind("click", function () {
			return o ? (o = !1, t.apply(this, arguments)) : (o = !0, e.apply(this, arguments));
		})
	})
};
