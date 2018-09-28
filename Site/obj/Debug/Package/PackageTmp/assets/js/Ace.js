$(".a_btn,.a_btn_s").click(function () {
    if ($(this).is(".a_btn"))
        $(this).next().slideDown().end().attr("class", "a_btn_s");
    else
        $(this).next().slideUp().end().attr("class", "a_btn");
})

$(".l_main").height($(".r_main").outerHeight() - 2);
$(".file_btn").click(function () { $(this).next("input[type=file]").click() })


$(".star img").hover(function () {
    $(".star img:lt(" + $(this).index() + ")").attr("src", "img/ystar.png")
}, function () {
    $(".star img" + ($(this).index() - 2 > 0 ? ":gt(" + ($(this).index() - 2) + ")" : "")).attr("src", "img/nstar.png")
});
$(".star img").click(function () {
    $(".star img").unbind("mouseenter").unbind("mouseleave");
})


$("textarea").keyup(function () {
    if ($(this).next().is(".img_ta_num")) {
        $(".img_ta_num").html((240 - $(this).val().length))
    }
})