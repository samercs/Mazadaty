(function ($, wow) {

    var setHiddenLocalizedContent = function () {
        var $this = $(this);
        $this.siblings("input[type='hidden']").val($this.val());
    };

    var autoSubmitForm = function() {
        $(this).parents("form").submit();
    };

    var goBack = function(e) {
        e.preventDefault();
        history.back();
    }

    var bindEvents = function () {
        $("form").on("change", ".auto-submit", autoSubmitForm);
        $("form").on("click", ".btn-cancel", goBack);
        $(".localized-content").on("input", ".localized-input[data-primary='true']", setHiddenLocalizedContent);
    };

    var initRequiredLabels = function() {
        $(".required-label").slice(1).children("span").hide();
    }

    var initWowAnimations = function() {
        wow.init();
    };

    $(function () {
        bindEvents();
        initRequiredLabels();
        initWowAnimations();  
    });

})(jQuery, new WOW());

var kendoFormatter = {
    getYesNo: function (value) {
        if (value === true) {
            return "<span class='text-yes'>Yes</span>";
        } else {
            return "<span class='text-no'>No</span>";
        }
    }
}