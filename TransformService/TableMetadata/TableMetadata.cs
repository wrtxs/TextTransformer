using System.Collections.Generic;
using System.Linq;

namespace TransformService.TableMetadata
{
    public class TableMetadata
    {
        public static readonly string TitleAttributeName = "title";
        public static readonly string OriginalColumnWidthsAttributeName = "original-column-widths";
        public static readonly string ActualColumnWidthsAttributeName = "actual-column-widths";
        public static readonly int MaxColumnWidthSize = 400;


        /// <summary>
        /// Заголовок таблицы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Исходные размеры ширин колонок таблицы (полученные из JSON файла)
        /// </summary>
        public IEnumerable<int> OriginalColumnWidths { get; private set; }

        /// <summary>
        /// Измененные размеры ширин колонок таблицы (заданные в визуальных редакторах)
        /// </summary>
        public IEnumerable<int> ActualColumnWidths { get; private set; }

        //public TableMetadata() : this()
        //{
        //}

        //public TableMetadata(string title) : this(title, null, null)
        //{
        //}

        //public TableMetadata(string title, string colWidthsStrValues) : this(title, GetColumnWidthsFromString(colWidthsStrValues), null)
        //{
        //}

        public TableMetadata(string title = null, IEnumerable<int> originalColumnWidths = null,
            IEnumerable<int> actualColumnWidths = null)
        {
            Title = title;
            OriginalColumnWidths = originalColumnWidths != null
                ? new List<int>(originalColumnWidths)
                : System.Array.Empty<int>();
            ActualColumnWidths = actualColumnWidths != null
                ? new List<int>(actualColumnWidths)
                : System.Array.Empty<int>();
        }

        public TableMetadata Clone() => new(Title, OriginalColumnWidths, ActualColumnWidths);
    }
}