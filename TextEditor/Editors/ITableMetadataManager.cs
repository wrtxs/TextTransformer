using TransformService.TableMetadata;

namespace TextEditor.Editors
{
    internal interface ITableMetadataManager
    {
        public TableMetadata GetTableMetadata(bool needActualColumnWidths = false);
        public void SetTableMetadata(TableMetadata tableMetadata);
    }
}