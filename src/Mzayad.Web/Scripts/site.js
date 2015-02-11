(function ($, wow) {

    var setHiddenLocalizedContent = function () {
        var $this = $(this);
        $this.siblings("input[type='hidden']").val($this.val());
    };

    var autoSubmitForm = function() {
        $(this).parents("form").submit();
    };

    var showHiddenMenuList = function (e) {
        var li = $(this).parent();
        //li.hide();
        li.siblings(".menu-list-hidden").fadeToggle();
        e.preventDefault();
        console.log("a");
    };

    var bindEvents = function () {
        $("form").on("change", ".auto-submit", autoSubmitForm);
        $(".localized-content").on("input", ".localized-input[data-primary='true']", setHiddenLocalizedContent);
        $(".menu-list").on("click", ".show-hidden", showHiddenMenuList);
    };

    var initRequiredLabels = function() {
        $(".required-label").slice(1).children("span").hide();
    }

    var initWowAnimations = function() {
        new wow().init();
    };

    $(function () {
        bindEvents();
        initRequiredLabels();
        initWowAnimations();
    });

})(jQuery, WOW);