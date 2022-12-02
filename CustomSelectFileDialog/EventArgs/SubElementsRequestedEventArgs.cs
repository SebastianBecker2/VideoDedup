namespace CustomSelectFileDlg.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Element = ResizableButtonArray.Element;

    internal class SubElementsRequestedEventArgs : EventArgs
    {
        public Element? Element { get; set; }
        public IEnumerable<Element>? SubElements { get; set; }

        public SubElementsRequestedEventArgs(Element? element) =>
            Element = element;
    }
}
