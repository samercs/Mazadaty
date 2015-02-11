﻿using System.Web.Mvc;
using Mzayad.Web.Resources;

namespace Mzayad.Web.Extensions
{
    public static class MvcHtmlStringExtensions
    {
        /// <summary>
        /// Returns the original MvcHtmlString appended with a required field label.
        /// </summary>
        public static MvcHtmlString Required(this MvcHtmlString htmlString, string textLabel = null)
        {
            textLabel = textLabel ?? Global.Required;

            var requiredLabel = "<span class='required-label'><i class='fa fa-pad-right fa-plus'></i><span>" + textLabel + "</span></span>";

            return MvcHtmlString.Create(htmlString + requiredLabel);
        }
    }
}