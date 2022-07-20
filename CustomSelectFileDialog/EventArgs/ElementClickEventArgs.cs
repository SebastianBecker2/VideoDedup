namespace CustomSelectFileDlg.EventArgs
{
    using System;

    internal class ElementClickEventArgs : EventArgs
    {
        public ResizableButtonArray.Element Element { get; set; }

        public ElementClickEventArgs(ResizableButtonArray.Element element) =>
            Element = element;
    }
}
