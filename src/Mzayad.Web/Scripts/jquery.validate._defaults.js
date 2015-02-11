/*!
 * jQuery validation defaults
 *
 * Sets or overrides jQuery unobstrusive validation defaults.
 *
 * Note: File named *_default.js to be alphabetically last when used with ASP.NET bundling.
 *
 */

(function($) {
    $.validator.setDefaults({
        ignore: "" // override so hiddens are not ignored
    });
}(jQuery));