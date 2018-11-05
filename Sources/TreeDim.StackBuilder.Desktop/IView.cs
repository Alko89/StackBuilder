#region Using directives
using System;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public interface IView
    {
        IDocument Document { get; }
        void Close();
        void Activate();

        event EventHandler Closed;
    }
}
