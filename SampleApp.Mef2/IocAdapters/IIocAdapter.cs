namespace SampleApp.Mef2.IocAdapters
{
    using System;

    using TomsToolbox.Essentials;

    internal interface IIocAdapter : IDisposable
    {
        IExportProvider Initialize();
    }
}