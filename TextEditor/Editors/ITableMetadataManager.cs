using TransformService.TableMetadata;

namespace TextEditor.Editors
{
    internal interface ITableMetadataManager
    {
        public TableMetadata GetTableMetadata();
        public void SetTableMetadata(TableMetadata tableMetadata);
    }
}
