(function ($) {

    $(function () {
        $("#AddressViewModel_CountryCode").on("change", changeCountry);
    });

    var addressLoading = $("#AddressEntryLoading");
    var addressForm = $("#AddressEntryAddress");
    var countryFlag = $("#AddressEntryFlag");

    var changeCountry = function () {
        var $this = $(this);
        var countryCode = $this.val();
        if (countryCode.length === 0) {
            return;
        }

        setCountryFlag(countryCode);
        showLoading();
        addressForm.load("/Checkout/ChangeCountry?countryCode=" + countryCode, hideLoading);
    };

    var setCountryFlag = function (countryCode) {
        countryCode = countryCode.toLowerCase();
        countryFlag.removeClass().addClass("flag flag-" + countryCode);
    };

    var showLoading = function () {
        addressForm.css("visibility", "hidden");
        addressLoading.show();
    };

    var hideLoading = function () {
        addressLoading.hide();
        addressForm.css("visibility", "visible");
        resetFormValidation();
    };

    var resetFormValidation = function() {
        $("#main form")
            .unbind()
            .removeData('validator')
            .removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse("#main form");
    };

})(jQuery);