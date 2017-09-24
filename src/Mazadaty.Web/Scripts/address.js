$(function () {

    
    $('.country-code').change(function() {
        
        copyData();

        var $this = $(this);
        var countryCode = $this.val();

        if (countryCode.length === 0) {
            return;
        }

        setCountryFlag(countryCode);
        showLoading();
        addressForm.load('/account/changecountry?countryCode=' + countryCode, hideLoading);

        $("#Address_CountryCode").val($(this).val());
        $('#PhoneNumberViewModel_PhoneCountryCode').val(getPhoneCountryCode($(this).val()));
        $('#input-group-addon-country-code').html(getPhoneCountryCode($(this).val()));
    });
   

    var addressLoading = $('#AddressEntryLoading');
    var addressForm = $('#AddressEntryAddress');
    var countryFlag = $('#AddressEntryFlag');

    

    var setCountryFlag = function (countryCode) {
        countryCode = countryCode.toLowerCase();
        countryFlag.removeClass().addClass('flag flag-' + countryCode);
    };

    var showLoading = function () {
        addressForm.css('visibility', 'hidden');
        addressLoading.show();
    };

    var hideLoading = function () {
        addressLoading.hide();
        addressForm.css('visibility', 'visible');
        resetFormValidation();


        pastData();
    };

    var resetFormValidation = function() {
        $('#main form')
            .unbind()
            .removeData('validator')
            .removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse('#main form');
    };

    var address = function() {

        this.Line1 = '';
        this.Line2 = '';
        this.Line3 = '';
        this.Line4 = '';
        this.CityArea = '';
        this.StateProvince = '';
        this.PostalCode = '';

    };

    var data = new address();

    var copyData = function() {

        data.Line1 = $('#Address_AddressLine1').val();
        data.Line2 = $('#Address_AddressLine2').val();
        data.Line4 = $('Address_AddressLine4').val();
        data.Line3 = $('Address_AddressLine3').val();
        data.CityArea = $('#Address_CityArea').val();
        data.StateProvince = $('#Address_StateProvince').val();
        data.PostalCode = $('#Address_PostalCode').val();
        
    };

    var pastData = function() {

        $('#Address_AddressLine1').val(data.Line1);
        $('#Address_AddressLine2').val(data.Line2);
        $('#Address_AddressLine3').val(data.Line3);
        $('#Address_AddressLine4').val(data.Line4);
        $('#Address_CityArea').val(data.CityArea);
        $('#Address_StateProvince').val(data.StateProvince);
        $('#Address_PostalCode').val(data.PostalCode);
        
    };

    var getPhoneCountryCode = function (countryCode){
        switch (countryCode) {
            case 'KW':
                return '+965';
            case 'SA':
                return '+966';
            case 'AE':
                return '+971';
            case 'BH':
                return '+973';
            case 'QA':
                return '+974';
            case 'OM':
                return '+968';
        default:
        }
    }

});