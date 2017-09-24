var KendoFormatter = (function () {
    function KendoFormatter() {
    }
    KendoFormatter.getYesNo = function (value) {
        if (value) {
            return "<span class='text-yes'>Yes</span>";
        }
        else {
            return "<span class='text-no'>No</span>";
        }
    };
    KendoFormatter.formatDate = function (value) {
        return "xxx";
    };
    return KendoFormatter;
}());
//# sourceMappingURL=site-kendo.js.map