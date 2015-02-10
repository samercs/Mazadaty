(function ($) {

    $(function () {
        bindEvents();
    });

    var bindEvents = function() {
        $("form").on("change", ".auto-submit", autoSubmitForm);
        $(".localized-content").on("input", ".localized-input[data-primary='true']", setHiddenLocalizedContent);   
    };

    var setHiddenLocalizedContent = function () {
        var $this = $(this);
        $this.siblings("input[type='hidden']").val($this.val());
    };

    var autoSubmitForm = function() {
        $(this).parents("form").submit();
    };

})(jQuery);