﻿using System;
using Kendo.Mvc.UI.Fluent;

namespace Mzayad.Web.Extensions
{
    public static class KendoExtensions
    {
        /// <summary>
        /// Initializes a Kendo UI grid with default settings.
        /// </summary>
        public static GridBuilder<T> Init<T>(this GridBuilder<T> gridBuilder, Action<CrudOperationBuilder> ajaxDataSource) where T : class
        {
            const int pageSize = 100;
            var gridName = "Grid_" + Guid.NewGuid();

            return gridBuilder
                .Name(gridName)
                .TableHtmlAttributes(new {@class = new Css().GridTable})
                .Pageable()
                .Sortable()
                .DataSource(dataSource => dataSource
                    .Ajax()
                    .Read(ajaxDataSource)
                    .PageSize(pageSize))
                .Deferred();
        }

        /// <summary>
        /// Defines a bound hyperlink column.
        /// </summary>
        public static GridTemplateColumnBuilder<T> LinkColumn<T>(this GridColumnFactory<T> column, string text, string href) where T : class
        {
            return column
                .Template(i => "")
                .HtmlAttributes(new { @class = "link-cell" })
                .ClientTemplate(string.Format("<a href=\"{0}\">{1}</a>", href, text));
        }
    }
}