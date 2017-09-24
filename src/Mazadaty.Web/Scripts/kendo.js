var KendoFormatter = (function () {
    function KendoFormatter() {
    }
    KendoFormatter.prototype.getYesNo = function (value) {
        if (value) {
            return "<span class='text-yes'>Yes</span>";
        } else {
            return "<span class='text-no'>No</span>";
        }
    };
    return KendoFormatter;
})();
//# sourceMappingURL=kendo.js.map
