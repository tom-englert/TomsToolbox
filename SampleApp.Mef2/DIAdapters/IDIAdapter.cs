namespace SampleApp.Mef2.DIAdapters
{
    using System;

    using TomsToolbox.Composition;

    internal interface IDIAdapter : IDisposable
    {
        IExportProvider Initialize();
    }
}