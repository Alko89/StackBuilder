#region Using directives
using System.Collections.Generic;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    interface IDocumentFactory
    {
        void NewDocument();
        void OpenDocument(string filePath);
        void SaveDocument();
        void CloseDocument();

        void AddDocument(IDocument doc);
        List<IDocument> Documents { get; }

        IView ActiveView { get; }
        IDocument ActiveDocument { get; }
    }
}
