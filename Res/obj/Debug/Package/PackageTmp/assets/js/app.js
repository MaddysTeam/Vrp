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


function initTableCheckbox(tableId) {
	var table = '#'+tableId;
	var $thr = $(table+' thead tr');
	var $checkAllTh = $('<th class="width30"><input type="checkbox" id="checkAll" name="checkAll" /></th>');
	if ($('#checkAll').size() <= 0) {
		/*将全选/反选复选框添加到表头最前，即增加一列*/
		$thr.prepend($checkAllTh);
	}
	/*“全选/反选”复选框*/
	var $checkAll = $thr.find('input');
	$checkAll.click(function (event) {
		/*将所有行的选中状态设成全选框的选中状态*/
		$tbr.find('input').prop('checked', $(this).prop('checked'));
		/*并调整所有选中行的CSS样式*/
		if ($(this).prop('checked')) {
			$tbr.find('input').parent().parent().addClass('warning');
		} else {
			$tbr.find('input').parent().parent().removeClass('warning');
		}
		/*阻止向上冒泡，以防再次触发点击操作*/
		event.stopPropagation();
	});
	/*点击全选框所在单元格时也触发全选框的点击操作*/
	$checkAllTh.click(function () {
		$(this).find('input').click();
	});
	var $tbr = $(table+' tbody tr');
	var $checkItemTd = $('<td><input type="checkbox" name="checkItem" class="checkItem"/></td>');
	/*每一行都在最前面插入一个选中复选框的单元格*/
	$tbr.prepend($checkItemTd);
	/*点击每一行的选中复选框时*/
	$tbr.find('input').click(function (event) {
		/*调整选中行的CSS样式*/
		$(this).parent().parent().toggleClass('warning');
		/*如果已经被选中行的行数等于表格的数据行数，将全选框设为选中状态，否则设为未选中状态*/
		$checkAll.prop('checked', $tbr.find('input:checked').length == $tbr.length ? true : false);
		/*阻止向上冒泡，以防再次触发点击操作*/
		event.stopPropagation();
	});
	/*点击每一行时也触发该行的选中操作*/
	$tbr.click(function () {
		$(this).find('input').click();
	});
}


function RelationSelect(src, tat, rule, none, fixInit) {
	var $src = $(src), $tat = $(tat);
	function doit(init) {
		var key = $src.val();
		$tat.empty();
		if (none) {
			$tat.append($("<option>").val(0).text(none)).change();
		}
		$.each(rule, function (i, n) {
			if (n.key == key) {
				$tat.append($("<option>").val(n.id).text(n.name));
			}
		});
		if (init) $tat.val(init);
	}

	$src.on("change", function () {
		doit();
	});

	doit(fixInit || $tat.val());
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


function imageError(img) {
	img.src = '/assets/img/cover.png';
	img.onerror = null;
}

$(function () {

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
	var summernotes = $(".summernote");
	summernotes.length > 0 && summernotes.summernote({

		height: 300,

		minHeight: 300,

		placeholder: "请输入正文",

		lang: 'zh-CN',

		oninit: function () {
			this.code($("#" + $(this).data("whatever")).val());
	},

		onblur: function (event) {
			$("#" + $(event.data).data("whatever")).val($(this).code());
		},

		onImageUpload: function (files, editor, welEditable) {
			sendFile(files[0], editor, welEditable);
		}
	});

	function sendFile(file, editor, welEditable) {
		data = new FormData();
		data.append("file", file);
		$.ajax({
			data: data,
			type: "POST",
			url: "/Attachment/UploadCover",
			cache: false,
			contentType: false,
			processData: false,
			success: function (data) {
				editor.insertImage(welEditable, data.showPath);
			}
		});
	}

//	//
//	// table-identify-column
	$(".table-identify-column>input").on("click", function (e) {
		e.stopPropagation();
		$(this).closest("table").find("td:first-child > input").prop("checked", $(this).prop("checked"));
	});



});
