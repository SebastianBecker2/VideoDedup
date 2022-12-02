namespace CustomSelectFileDlg.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Element = ResizableButtonArray.Element;

    internal class RootElementsRequestedEventArgs : EventArgs
    {
        public IEnumerable<Element>? RootElements { get; set; }
    }
}
