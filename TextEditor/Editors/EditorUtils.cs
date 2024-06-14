using TransformService.TableMetadata;

namespace TextEditor.Editors
{
    internal static class EditorUtils
    {
        public static void SetTableMetadata(ITableMetadataManager tableMetadataManager, string htmlData,
            bool savePrevTableName)
        {
            var tableMetadata = TableMetadataUtils.GetFirstTableMetadata(htmlData); // Получаем метаданные таблицы
            var curTableMetadata = tableMetadataManager.GetTableMetadata();

            // Сохраняем прежнее наименование таблицы при отсутствии наименования в новой таблице
            if (savePrevTableName && string.IsNullOrEmpty(tableMetadata.Title))
                tableMetadata.Title =
                    curTableMetadata
                        .Title;

            tableMetadataManager.SetTableMetadata(tableMetadata);
        }
    }
}
