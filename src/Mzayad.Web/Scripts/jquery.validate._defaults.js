/*!
 * jQuery validation defaults
 *
 * Sets or overrides jQuery unobstrusive validation defaults.
 *
 * Note: File named *_default.js to be alphabetically last when used with ASP.NET bundling.
 *
 */

(function($) {

    var isDateTime = function(value) {
        // custom expression to test for d/m/Y H:i
        return value.match(/^(0?[1-9]|[12][0-9]|3[0-1])[/., -](0?[1-9]|1[0-2])[/., -](19|20)?\d{2}(\s\d{2}:\d{2})*$/);
    };

    $.validator.setDefaults({
        ignore: "" // override so hiddens are not ignored

    });

    $.validator.addMethod(
        "date",
        function (value) {
            return (value === "") ? true : isDateTime(value);
        },
        "* invalid"
       );

}(jQuery));