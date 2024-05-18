using TransformService.TableMetadata;

namespace TextEditor.Editors
{
    internal interface IEditorService
    {
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="htmlData"></param>
        ///// <param name="applyTableStyle"></param>
        ///// <param name="autoFitTable"></param>
        //void SetHtmlContent(string htmlData, bool applyTableStyle, bool autoFitTable);

        /// <summary>
        /// Записать в редактор HTML представление содержимого
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="parameters"></param>
        void SetHtmlContent(string htmlData, IEditorParameters parameters);

        /// <summary>
        /// Получить из редактора HTML представление содержимого
        /// </summary>
        /// <returns></returns>
        string GetHtmlContent();

        /// <summary>
        /// Признак наличия содерж
        /// </summary>
        /// <returns></returns>
        bool HasContent();

        /// <summary>
        /// Событие, возникающее при изменении содержимого редактора 
        /// </summary>
        public event EventHandler ContentChanged;
    }
}