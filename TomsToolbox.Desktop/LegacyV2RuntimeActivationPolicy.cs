namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;

    /// <summary>
    /// Configure the LegacyV2RuntimeActivationPolicy from code. 
    /// See <see href="https://msdn.microsoft.com/de-de/library/bbx34a2h.aspx"/> for documentation.
    /// </summary>
    public static class LegacyV2RuntimeActivationPolicy
    {
        /// <summary>
        /// Activates the LegacyV2RuntimeActivationPolicy. 
        /// <para/>
        /// Useful e.g. for unit tests where you have no access to the app.config of the test runner process.
        /// </summary>
        public static void Activate()
        {
            var clrRuntimeInfo = (ICLRRuntimeInfo)RuntimeEnvironment.GetRuntimeInterfaceAsObject(Guid.Empty, typeof(ICLRRuntimeInfo).GUID);
            Contract.Assume(clrRuntimeInfo != null);
            clrRuntimeInfo.BindAsLegacyV2Runtime();
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
        private interface ICLRRuntimeInfo
        {
            void xGetVersionString();
            void xGetRuntimeDirectory();
            void xIsLoaded();
            void xIsLoadable();
            void xLoadErrorString();
            void xLoadLibrary();
            void xGetProcAddress();
            void xGetInterface();
            void xSetDefaultStartupFlags();
            void xGetDefaultStartupFlags();

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void BindAsLegacyV2Runtime();
        }
    }
}
