namespace SampleApp.Mef2.IocAdapters
{
    using System;

    using TomsToolbox.Composition;

    internal interface IIocAdapter : IDisposable
    {
        IExportProvider Initialize();
    }
}