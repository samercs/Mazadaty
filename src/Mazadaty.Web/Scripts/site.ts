($ => {

    var setHiddenLocalizedContent = function () {
        var $this = $(this);
        $this.siblings("input[type='hidden']").val($this.val());
    };

    var autoSubmitForm = function() {
        $(this).parents("form").submit();
    };

    var goBack = e => {
        e.preventDefault();
        history.back();
    };

    var submitOnce = function(e) {

        var $this = $(this);
        if (typeof($this.valid) === typeof(Function) && !$this.valid()) {
            return;
        }

        if ($this.data("submitting") === true) {
            e.preventDefault();
        } else {

            $this.data("submitting", true);

            var button = $this.find("button[type='submit']");
            var submittingText = button.data("submittingText") || "Submitting, please wait...";

            button.addClass("submitting")
                .html(`<i class='fa fa-cog fa-spin fa-lg'></i> ${submittingText}`)
                .blur();
        }
    };

    var setDateTimePicker = function() {

        var parent = $(this).parents(".datetime-picker");
        var hidden = parent.find(".datetime-value");
        var date = parent.find(".date-picker").datepicker("getDate");
        var hour = parent.find(".hour-picker").val();
        var minute = parent.find(".minute-picker").val();

        date.setHours(hour);
        date.setMinutes(minute);

        hidden.val(date.toISOString());
    };

    var bindEvents = () => {
        $("form").on("change", ".auto-submit", autoSubmitForm);
        $("form").on("click", ".btn-cancel", goBack);
        $("form.submit-once").on("submit", submitOnce);
        $(".localized-content").on("input", ".localized-input[data-primary='true']", setHiddenLocalizedContent);
        $(".datetime-picker").on("change", "input, select", setDateTimePicker);
    };

    var initRequiredLabels = () => {
        $(".required-label").slice(1).children("span").hide();
    }

    $(() => {
        bindEvents();
        initRequiredLabels();
    });

})(jQuery);

module App {
    
    export class TimeUtilities {
        static getHours(seconds: number) {
            return Math.floor(seconds/3600);
        }
        static getMinutes(seconds: number) {
            return Math.floor((seconds / 60) % 60);
        }
        static getSeconds(seconds: number) {
            return Math.floor(seconds % 60);
        }
    }

    export class CkEditor {

        static init() {
            // ReSharper disable SuspiciousThisUsage
            
            const editorHeight = 300;

            var fixValidation = function () {

                this.document.on("input", () => {

                    var instance = CKEDITOR.currentInstance;
                    var name = instance.name;
                    var data = instance.getData();
                    var parent = $(`li[data-for='${name}']`);
                    var inputs = parent.find(".localized-hidden, .localized-input");

                    if (data.length >= 0) {
                        inputs.val(data).addClass("valid");
                    } else {
                        inputs.val("").removeClass("valid");
                    }
                });
            }

            $("textarea.localized-input[data-language!='ar']").each(function () {
                CKEDITOR.replace(this.id, { height: editorHeight }).on("instanceReady", fixValidation);
            });

            $("textarea.localized-input[data-language='ar']").each(function () {
                CKEDITOR.replace(this.id, { height: editorHeight, contentsLangDirection: "rtl" });
            });
            
            // ReSharper restore SuspiciousThisUsage
        }
    }

    export class Images {
        public static lazyLoad(): void {
            const images = document.getElementsByClassName("lazy-load");
            for (let i = images.length; i--;) {
                const image = images[i] as HTMLImageElement;
                image.src = image.dataset["src"];
            }
        }
    }
}


var TimeUtilities = {
    getDays(days) {
        return days === 1 ? "1 day " : days + " days ";
    },
    getHours(hours) {
        return hours === 1 ? "1 hour " : hours + " hours ";
    },
    getMinutes(minutes) {
        return minutes === 1 ? "1 minute " : minutes + " minutes ";
    },
    getSeconds(seconds) {
        return seconds === 1 ? "1 second " : seconds + " seconds ";
    },
    getTimeLeft(seconds) {

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