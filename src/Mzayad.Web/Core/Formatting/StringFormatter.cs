using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Mzayad.Web.Core.Formatting
{
    public class StringFormatter
    {
        public static string SanitizePhoneNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = StripNonDigits(input);

            if (input.Length < 9)
            {
                input = "965" + input;
            }

            return "+" + input;
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static readonly Regex NotDigitsRegex = new Regex(@"[^0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Gets a string with all non-numeric digits removed.
        /// </summary>
        public static string StripNonDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return NotDigitsRegex.Replace(input, "").Trim();
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Gets a string with all HTML tags removed.
        /// </summary>
        public static string StripHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return HtmlRegex.Replace(input, "");
        }

        /// <summary>
        /// Compiled regular expressions for performance.
        /// </summary>
        static readonly Regex LocalizationRegex1 = new Regex(@"^\[\{", RegexOptions.Compiled);
        static readonly Regex LocalizationRegex2 = new Regex(@"""?}]$", RegexOptions.Compiled);
        static readonly Regex LocalizationRegex3 = new Regex(@"""k"":""[a-z]{2}"",""v"":""", RegexOptions.Compiled);
        static readonly Regex LocalizationRegex4 = new Regex(@"""},\{", RegexOptions.Compiled);


        /// <summary>
        /// Gets a string with localization JSON removed.
        /// </summary>
        public static string StripLocalizationJson(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = LocalizationRegex1.Replace(input, "");
            input = LocalizationRegex2.Replace(input, "");
            input = LocalizationRegex3.Replace(input, "");
            input = LocalizationRegex4.Replace(input, " ");

            return input;
        }

        public static string FormatCamelCase(object camelCasedString)
        {
            var returnValue = camelCasedString.ToString();

            // Strip leading "_" character
            returnValue = Regex.Replace(returnValue, "^_", "").Trim();
            // Add a space between each lower case character and upper case character
            returnValue = Regex.Replace(returnValue, "([a-z])([A-Z])", "$1 $2").Trim();
            // Add a space between 2 upper case characters when the second one is followed by a lower space character
            returnValue = Regex.Replace(returnValue, "([A-Z])([A-Z][a-z])", "$1 $2").Trim();

            return returnValue;
        }

        /// <summary>
        /// Gets a string with MS Word HTML removed.
        /// </summary>
        public static string StripWordHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = StripHtmlComments(input);
            input = StripXmlTags(input);
            input = StripWordTags(input);
            input = StripEmptyTags(input);

            return input;
        }

        private static string StripHtmlComments(string input)
        {
            var regex = new Regex(@"\<!--.*?-->", RegexOptions.Singleline);
            return regex.Replace(input, "");
        }

        private static string StripXmlTags(string input)
        {
            var regex = new Regex(@"<xml>.*?</xml>", RegexOptions.Singleline);
            return regex.Replace(input, "");
        }

        private static string StripWordTags(string input)
        {
            var regex = new Regex(@"<([a-z]{1}:\w+)>.*?</(\1)>", RegexOptions.Singleline);
            return regex.Replace(input, "");
        }

        private static string StripEmptyTags(string input)
        {
            var regex = new Regex(@"<(\w+)></(\1)>", RegexOptions.Singleline);
            return regex.Replace(input, "");
        }
    }
}