//
// ajax
$.ajaxSetup({ cache: false });
var ajaxErrorMessage = "操作出现异常！";

//
// boot-grid
var gridOptions = {

	ajax: true,

	padding: 4,

	rowCount: [10, 20, 50],

	labels: {
		all: "全部",
		infos: "显示 {{ctx.start}} 到 {{ctx.end}} 共 {{ctx.total}} 记录",
		loading: "数据加载中...",
		noResults: "无可寻记录",
		refresh: "重新加载",
		search: "搜索"
	},

	templates: {
		footer: "<div id=\"{{ctx.id}}\" class=\"{{css.footer}}\"><div class=\"row infoBar\"><p class=\"{{css.pagination}}\"></p><p class=\"{{css.infos}}\"></p></div></div>"
	},

	formatters: {

		"Boolean": function (column, row) { return row[column.id] ? "<i class=\"fa fa-check-square-o fa-lg\"></i>" : ""; },

		"Current": function (column, row) { return "$" + row[column.id]; },

		"DateTime": function (column, row) { return row[column.id]; },

		"DateOnly": function (column, row) { return row[column.id]; },

		"Email": function (column, row) { return "<a href='mailto:" + row[column.id] + "'>" + row[column.id] + "</a>"; },

		"Percent": function (column, row) { return row[column.id] + " %"; },

		"TimeOnly": function (column, row) { return row[column.id]; },

		"Url": function (column, row) {
			var text = row[column.id], url = text;
			if (!(typeof text === "string")) { url = text.url; text = text.text; }
			return "<a href='" + url + "'>" + text + "</a>";
		},

	},

	responseHandler: function (response) {
		if (response.errMessage) {
			alert(response.errMessage);
			return { current: 1, rowCount: 10, rows: [], total: 0 };
		}
		else {
			return response;
		}
	},

	loadDataErrorHandler: function (errMessage) {
		alert(errMessage);
	}

};

// join bag
function joinBag(id) {
	$.post("/Course/Joinbag", { id: id }, function (state, status) {
		if (status == "success") {
			if (state) {
				$("#bagCount").text($("#bagCount").text() * 1 + 1);
				alertify.success("课程已加入打包器。");
			} else {
				alertify.error("打包器中已包含该课程。");
			}
		} else {
			alertify.error(ajaxErrorMessage);
		}
	});
}
//$(function () {
//	$(document).on("click", ".joinbag", function (e) {
//		e.stopPropagation();
//		e.preventDefault();
//		joinBag($(this).data('whatever'));
//	});
//	$(document).on("click", ".delete-in-list", function (e) {
//		e.stopPropagation();
//		e.preventDefault();
//		deleteChoose($(this).data('whatever'));
//	});
//});

//// viewModel
//function removeListItemInViewModel(key, id, list) {
//	list = list || viewModel.dataList();
//	$.each(list, function (k, d) {
//		if (d[key] == id) {
//			viewModel.dataList.splice(k, 1);
//			return false;
//		}
//	});
//}
//function getListItemInViewModel(key, id, list) {
//	var find;
//	list = list || viewModel.dataList();
//	$.each(list, function (k, d) {
//		if (d[key] == id) {
//			find = d;
//			return false;
//		}
//	});
//	return find;
//}



//$(function () {

//	//
//	// alertify
//	alertify && alertify.set({

//		buttonReverse: true,

//		labels: {
//			ok: "确定",
//			cancel: "取消"
//		}

//	});


//	//
//	// sumernote
//	var summernotes = $(".summernote");
//	summernotes.length > 0 && summernotes.summernote({

//		height: 300,

//		minHeight: 300,

//		placeholder: "请输入正文",

//		lang: 'zh-CN',

//		oninit: function () {
//			this.code($("#" + $(this).data("whatever")).val());
//		},

//		onblur: function (event) {
//			$("#" + $(event.data).data("whatever")).val($(this).code());
//		},

//		onImageUpload: function (files, editor, welEditable) {
//			sendFile(files[0], editor, welEditable);
//		}
//	});

//	function sendFile(file, editor, welEditable) {
//		data = new FormData();
//		data.append("file", file);
//		$.ajax({
//			data: data,
//			type: "POST",
//			url: "/Attachment/Upload",
//			cache: false,
//			contentType: false,
//			processData: false,
//			success: function (data) {
//				editor.insertImage(welEditable, data.showPath);
//			}
//		});
//	}

//	//
//	// table-identify-column
//	$(".table-identify-column>input").on("click", function (e) {
//		e.stopPropagation();
//		$(this).closest("table").find("td:first-child > input").prop("checked", $(this).prop("checked"));
//	});



//});
