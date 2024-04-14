using System.Collections.Generic;
using System.Linq;

namespace TransformService.TableMetadata
{
    public class TableMetadata
    {
        public static readonly string TitleAttributeName = "title";
        public static readonly string ColWidthsAttributeName = "colWidths";

        /// <summary>
        /// Заголовок таблицы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ширины колонок таблицы
        /// </summary>
        public IEnumerable<int> ColumnWidths { get; private set; }

        public TableMetadata() : this(null)
        {
        }

        public TableMetadata(string title) : this(title, (IEnumerable<int>)null)
        {
        }

        public TableMetadata(string title, string colWidthsStrValues) : this(title, GetColumnWidths(colWidthsStrValues))
        {
        }

        public TableMetadata(string title, IEnumerable<int> columnWidths)
        {
            Title = title;
            ColumnWidths = columnWidths ?? System.Array.Empty<int>();
        }

        public string GetColumnWidthsString() => string.Join(";", ColumnWidths);

        public TableMetadata Clone() => new(Title, new List<int>(ColumnWidths));

        private static IList<int> GetColumnWidths(string values)
        {
            var colWidths = new List<int>();

            try
            {
                if (!string.IsNullOrEmpty(values))
                    colWidths.AddRange(values.Split(';').Select(int.Parse));
            }
            catch
            {
                //
            }

            return colWidths;
        }
    }
}