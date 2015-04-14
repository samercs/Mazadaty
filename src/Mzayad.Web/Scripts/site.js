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

var TimeUtilities = {
    getDays: function (days) {
        return days === 1 ? "1 day " : days + " days ";
    },
    getHours: function (hours) {
        return hours === 1 ? "1 hour " : hours + " hours ";
    },
    getMinutes: function (minutes) {
        return minutes === 1 ? "1 minute " : minutes + " minutes ";
    },
    getSeconds: function (seconds) {
        return seconds === 1 ? "1 second " : seconds + " seconds ";
    },
    getTimeLeft: function (seconds) {

        var self = this;
        var minutes = Math.floor(seconds / 60);
        var hours = Math.floor(minutes / 60);
        var days = Math.floor(hours / 24);

        hours = hours - (days * 24);
        minutes = minutes - (days * 24 * 60) - (hours * 60);
        seconds = seconds - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60);

        var label = "Time Left: ";

        if (days > 0) {
            return label + self.getDays(days) + self.getHours(hours) + self.getMinutes(minutes) + self.getSeconds(seconds);
        } else if (hours > 0) {
            return label + self.getHours(hours) + self.getMinutes(minutes) + self.getSeconds(seconds);
        } else if (minutes > 0) {
            return label + self.getMinutes(minutes) + self.getSeconds(seconds);
        } else {
            return label + self.getSeconds(seconds);
        }
    }
}