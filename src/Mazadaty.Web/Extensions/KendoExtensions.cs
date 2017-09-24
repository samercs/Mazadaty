using Kendo.Mvc.UI.Fluent;
using System;

namespace Mazadaty.Web.Extensions
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
                .TableHtmlAttributes(new { @class = new Css().GridTable })
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

        public static GridBoundColumnBuilder<T> FormatDateTime<T>(this GridBoundColumnBuilder<T> builder) where T : class
        {
            return builder.Format("{0:dd/MM/yyyy hh:mm tt}");
        }

        public static GridBoundColumnBuilder<T> FormatCurrency<T>(this GridBoundColumnBuilder<T> builder) where T : class
        {
            return builder.Format("KD {0}");
        }

        public static GridBoundColumnBuilder<T> ImageCell<T>(this GridBoundColumnBuilder<T> column) where T : class
        {
            return column.HtmlAttributes(new { @class = "image-cell" });
        }
    }
}
