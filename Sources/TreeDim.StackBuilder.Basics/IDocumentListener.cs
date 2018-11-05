#region Using directives


#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region IDocumentListener
    /// <summary>
    /// Listener class that is notified when the document is modified
    /// </summary>
    public interface IDocumentListener
    {
        // new
        void OnNewDocument(Document doc);
        void OnNewTypeCreated(Document doc, ItemBase itemBase);
        void OnNewAnalysisCreated(Document doc, Analysis analysis);
        void OnAnalysisUpdated(Document doc, Analysis analysis);
        void OnNewAnalysisCreated(Document doc, HAnalysis analysis);
        void OnAnalysisUpdated(Document doc, HAnalysis analysis);
        // remove
        void OnTypeRemoved(Document doc, ItemBase itemBase);
        void OnAnalysisRemoved(Document doc, ItemBase itemBase); 
    }
    #endregion
}
