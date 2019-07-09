namespace TomsToolbox.Desktop
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Text;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Helper Functions for Privileges and Elevation Status
    /// </summary>
    public static class UserAccountControl
    {
        private const int E_FAIL = -2147467259;

        /// <summary>
        /// Prompts the user for credential.
        /// </summary>
        /// <param name="parentHandle">The parent for the dialog.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="message">The message.</param>
        /// <param name="authenticationError">The previous authentication error, if any; 0 to hide previous error information.</param>
        /// <param name="template">The credential used to initialize the dialog; maybe <c>null</c> to start with an empty dialog.</param>
        /// <returns>
        /// The credentials entered by the user, or <c>null</c> if the user has canceled the operation.
        /// </returns>
        [CanBeNull]
        public static NetworkCredential PromptForCredential(IntPtr parentHandle, [CanBeNull] string caption, [CanBeNull] string message, int authenticationError, [CanBeNull] NetworkCredential template)
        {
            using (var inCredBuffer = PackAuthenticationBuffer(template))
            {
                return PromptForCredential(parentHandle, caption, message, authenticationError, inCredBuffer);
            }
        }

        /// <summary>
        /// Logs on the user with the given credentials as an interactive user.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <param name="userToken">The user token.</param>
        /// <returns>
        /// 0 if the function succeeds, a HRESULT of the last error if the function fails.
        /// </returns>
        public static int LogOnInteractiveUser([NotNull] this NetworkCredential credential, [CanBeNull] out SafeTokenHandle userToken)
        {
            return LogOnInteractiveUser(credential.UserName, credential.Domain, credential.Password, out userToken);
        }

        /// <summary>
        /// Logs on the user with the given credentials as an interactive user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="userToken">The user token.</param>
        /// <returns>
        /// 0 if the function succeeds, a HRESULT of the last error if the function fails.
        /// </returns>
        public static int LogOnInteractiveUser([CanBeNull] string userName, [CanBeNull] string domain, [CanBeNull] string password, [CanBeNull] out SafeTokenHandle userToken)
        {
            ParseUserDomain(ref userName, ref domain);

            if (NativeMethods.LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out var pUserToken))
            {
                userToken = new SafeTokenHandle(pUserToken);
                return 0;
            }

            userToken = null;
            var error = Marshal.GetHRForLastWin32Error();
            return error == 0 ? E_FAIL : error;
        }

        /// <summary>
        /// The function checks whether the primary access token of the process belongs 
        /// to a user account that is a member of the local Administrators group, even if 
        /// it currently is not elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group. Returns false 
        /// if the token does not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception with the last error code.
        /// </exception>
        public static bool IsCurrentUserInAdminGroup()
        {
            // Open the access token of the current process for query and duplicate.
            if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY | TOKEN_DUPLICATE, out var phToken))
            {
                throw new Win32Exception();
            }

            using (var hToken = new SafeTokenHandle(phToken))
            {
                return IsUserInAdminGroup(hToken);
            }
        }

        /// <summary>
        /// The function checks whether the object associated with the access token belongs
        /// to a user account that is a member of the local Administrators group, even if
        /// it currently is not elevated.
        /// </summary>
        /// <param name="userToken">The user token.</param>
        /// <returns>
        /// Returns true if the object associated with the access token belongs to user
        /// account that is a member of the local Administrators group. Returns false
        /// if the token does not.
        /// </returns>
        /// <exception cref="Win32Exception">
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">When any native Windows API call fails, the function throws a Win32Exception with the last error code.</exception>
        public static bool IsUserInAdminGroup([NotNull] SafeTokenHandle userToken)
        {
            // Determine whether system is running Windows Vista or later operating 
            // systems (major version >= 6) because they support linked tokens, but 
            // previous versions (major version < 6) do not.
            var version = Environment.OSVersion.Version;

            if ((version != null) && (version.Major >= 6))
            {
                // Running Windows Vista or later (major version >= 6). 
                // Determine token type: limited, elevated, or default. 

                // Allocate a buffer for the elevation type information.
                using (var pElevationType = new SafeNativeMemory(sizeof(TOKEN_ELEVATION_TYPE)))
                {
                    // Retrieve token elevation type information.
                    if (!NativeMethods.GetTokenInformation(userToken, TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType))
                    {
                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    var elevType = pElevationType.ReadInt32().ToEnum<TOKEN_ELEVATION_TYPE>();

                    // If limited, get the linked elevated token for further check.
                    if (elevType == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                    {
                        // Allocate a buffer for the linked token.
                        using (var pLinkedToken = new SafeNativeMemory(IntPtr.Size))
                        {
                            // Get the linked token.
                            if (!NativeMethods.GetTokenInformation(userToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken))
                            {
                                throw new Win32Exception();
                            }

                            // Marshal the linked token value from native to .NET.
                            var hLinkedToken = pLinkedToken.ReadIntPtr();
                            using (var linkedToken = new SafeTokenHandle(hLinkedToken))
                            {
                                return IsAdministrator(linkedToken);
                            }
                        }
                    }
                }
            }

            // CheckTokenMembership requires an impersonation token. If we just got 
            // a linked token, it already is an impersonation token.  If we did not 
            // get a linked token, duplicate the original into an impersonation 
            // token for CheckTokenMembership.

            if (!NativeMethods.DuplicateToken(userToken, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, out var phTokenToCheck))
            {
                throw new Win32Exception();
            }

            using (var hTokenToCheck = new SafeTokenHandle(phTokenToCheck))
            {
                // Check if the token to be checked contains admin SID.
                return IsAdministrator(hTokenToCheck);
            }
        }

        /// <summary>
        /// The function checks whether the current process is run as administrator.
        /// In other words, it dictates whether the primary access token of the 
        /// process belongs to user account that is a member of the local 
        /// Administrators group and it is elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group and it is 
        /// elevated. Returns false if the token does not.
        /// </returns>
        public static bool IsRunAsAdmin()
        {
            using (var id = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(id);

                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Determines whether the current user is in the specified group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// <c>true</c> if the user blongs to the specified group; otherwise <c>false</c>
        /// </returns>
        public static bool IsCurrentUserInGroup([CanBeNull] string groupName)
        {
            using (var id = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(id);

                return principal.IsInRole(groupName);
            }
        }

        /// <summary>
        /// Determines whether the current user is in the specified group.
        /// </summary>
        /// <param name="userToken">The user token.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// <c>true</c> if the user blongs to the specified group; otherwise <c>false</c>
        /// </returns>
        public static bool IsUserInGroup([NotNull] SafeTokenHandle userToken, [CanBeNull] string groupName)
        {
            var token = userToken.DangerousGetHandle();
            if (token == IntPtr.Zero)
                return false;

            using (var id = new WindowsIdentity(token))
            {
                var principal = new WindowsPrincipal(id);
                return principal.IsInRole(groupName);
            }
        }

        /// <summary>
        /// The function gets the elevation information of the current process. It 
        /// dictates whether the process is elevated or not. Token elevation is only 
        /// available on Windows Vista and newer operating systems, thus 
        /// IsProcessElevated throws a C++ exception if it is called on systems prior 
        /// to Windows Vista. It is not appropriate to use this function to determine 
        /// whether a process is run as administartor.
        /// </summary>
        /// <returns>
        /// Returns true if the process is elevated. Returns false if it is not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        /// <remarks>
        /// TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
        /// type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
        /// TokenElevationTypeFull) of the process. It is different from TokenElevation 
        /// in that, when UAC is turned off, elevation type always returns 
        /// TokenElevationTypeDefault even though the process is elevated (Integrity 
        /// Level == High). In other words, it is not safe to say if the process is 
        /// elevated based on elevation type. Instead, we should use TokenElevation. 
        /// </remarks>
        public static bool IsProcessElevated()
        {
            // Open the access token of the current process with TOKEN_QUERY.
            if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY, out var phToken))
            {
                throw new Win32Exception();
            }

            using (var hToken = new SafeTokenHandle(phToken))
            {
                // Allocate a buffer for the elevation information.
                using (var pTokenElevation = new SafeNativeMemory<TOKEN_ELEVATION>())
                {
                    // Retrieve token elevation information.
                    if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevation, pTokenElevation))
                    {
                        // When the process is run on operating systems prior to Windows 
                        // Vista, GetTokenInformation returns false with the error code 
                        // ERROR_INVALID_PARAMETER because TokenElevation is not supported 
                        // on those operating systems.
                        if ((Marshal.GetLastWin32Error() & 0xFFFF) == ERROR_INVALID_PARAMETER)
                        {
                            // => so on XP we are always "Elevated"!
                            return true;
                        }

                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_ELEVATION struct from native to .NET object.
                    var elevation = (TOKEN_ELEVATION)pTokenElevation;

                    // TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the token 
                    // has elevated privileges; otherwise, a zero value.
                    return (elevation.TokenIsElevated != 0);
                }
            }
        }

        /// <summary>
        /// The function gets the integrity level of the current process. Integrity 
        /// level is only available on Windows Vista and newer operating systems, thus 
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
        /// prior to Windows Vista.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of 
        /// these values:
        /// 
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used 
        ///    by processes started by the Anonymous group. Blocks most write access.
        ///    (SID: S-1-16-0x0)
        ///    
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects 
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
        /// 
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is 
        ///    used by normal applications being launched while UAC is enabled. 
        ///    (SID: S-1-16-0x2000)
        ///    
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used 
        ///    by administrative applications launched through elevation when UAC is 
        ///    enabled, or normal applications if UAC is disabled and the user is an 
        ///    administrator. (SID: S-1-16-0x3000)
        ///    
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is 
        ///    used by services and other system-level applications (such as Wininit, 
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        public static int GetProcessIntegrityLevel()
        {
            // Open the access token of the current process with TOKEN_QUERY.
            if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY, out var phToken))
            {
                throw new Win32Exception();
            }

            using (var hToken = new SafeTokenHandle(phToken))
            {
                // Then we must query the size of the integrity level information 
                // associated with the token. Note that we expect GetTokenInformation 
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code 
                // because we've given it a null buffer. On exit cbTokenIntegrityLevel will tell 
                // the size of the group information.
                if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0, out var cbTokenIntegrityLevel))
                {
                    var error = Marshal.GetLastWin32Error();
                    if (error != ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to 
                        // Windows Vista, GetTokenInformation returns false with the 
                        // ERROR_INVALID_PARAMETER error code because 
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                using (var pTokenIntegrityLevel = new SafeNativeMemory<TOKEN_MANDATORY_LABEL>(cbTokenIntegrityLevel))
                {
                    // Now we ask for the integrity level information again. This may fail 
                    // if an administrator has added this account to an additional group 
                    // between our first call to GetTokenInformation and this one.
                    if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIntegrityLevel))
                    {
                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                    var tokenIntegrityLevel = (TOKEN_MANDATORY_LABEL)pTokenIntegrityLevel;

                    // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
                    // S-1-16-0x1000 stands for low integrity level SID). There is one 
                    // and only one sub-authority.
                    var pIntegrityLevel = NativeMethods.GetSidSubAuthority(tokenIntegrityLevel.Label.Sid, 0);
                    return Marshal.ReadInt32(pIntegrityLevel);
                }
            }
        }

        /// <summary>
        /// Gets a handle of the UAC (user account control) shield icon.
        /// </summary>
        /// <value>The handle of the icon.</value>
        /// <returns>
        /// The handle of the UAC (user account control) shield icon.
        /// </returns>
        public static IntPtr UacShieldIcon
        {
            get
            {
                var iconInfo = new SHSTOCKICONINFO();
                iconInfo.cbSize = (uint)Marshal.SizeOf(iconInfo);

                var hr = NativeMethods.SHGetStockIconInfo(
                    SHSTOCKICONID.SIID_SHIELD,
                    SHGSI.SHGSI_ICON | SHGSI.SHGSI_SMALLICON,
                    ref iconInfo);

                return hr != 0 ? IntPtr.Zero : iconInfo.hIcon;
            }
        }

        [NotNull]
        private static SafeNativeMemory PackAuthenticationBuffer([CanBeNull] NetworkCredential credential)
        {
            var userName = credential?.UserName;

            if (string.IsNullOrEmpty(userName))
                return new SafeNativeMemory();

            var inCredSize = 0;
            NativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, IntPtr.Zero, ref inCredSize);
            var buffer = new SafeNativeMemory(inCredSize);
            NativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, buffer.DangerousGetHandle(), ref inCredSize);
            return buffer;
        }

        [CanBeNull]
        private static NetworkCredential PromptForCredential(IntPtr parentHandle, [CanBeNull] string caption, [CanBeNull] string message, int authenticationError, [NotNull] SafeNativeMemory inCredBuffer)
        {
            var save = true;
            uint authPackage = 0;

            var credui = new CREDUI_INFO
            {
                cbSize = Marshal.SizeOf(typeof(CREDUI_INFO)),
                pszCaptionText = caption,
                pszMessageText = message,
                hwndParent = parentHandle
            };

            if (0 != NativeMethods.CredUIPromptForWindowsCredentials(ref credui, authenticationError, ref authPackage, inCredBuffer.DangerousGetHandle(), inCredBuffer.Size, out var outCredBufferPtr, out int outCredSize, ref save, 0))
                return null;

            using (var outCredBuffer = new SafeNativeMemory(outCredBufferPtr, outCredSize))
            {
                return UnpackAuthenticationBuffer(outCredBuffer);
            }
        }
        private static bool IsAdministrator([NotNull] SafeHandle hTokenToCheck)
        {
            var token = hTokenToCheck.DangerousGetHandle();
            if (token == IntPtr.Zero)
                return false;

            using (var id = new WindowsIdentity(token))
            {
                var principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        [CanBeNull]
        private static NetworkCredential UnpackAuthenticationBuffer([NotNull] SafeNativeMemory outCredBuffer)
        {
            const int maxLen = 100;

            var usernameBuf = new StringBuilder(maxLen);
            var passwordBuf = new StringBuilder(maxLen);
            var domainBuf = new StringBuilder(maxLen);

            var maxUserLen = maxLen;
            var maxDomainLen = maxLen;
            var maxPasswordLen = maxLen;

            if (!NativeMethods.CredUnPackAuthenticationBuffer(1, outCredBuffer.DangerousGetHandle(), outCredBuffer.Size, usernameBuf, ref maxUserLen, domainBuf, ref maxDomainLen, passwordBuf, ref maxPasswordLen))
                return null;

            return new NetworkCredential()
            {
                UserName = usernameBuf.ToString(),
                Password = passwordBuf.ToString(),
                Domain = domainBuf.ToString()
            };
        }

        private static void ParseUserDomain([CanBeNull] ref string userName, [CanBeNull] ref string domain)
        {
            if (string.IsNullOrEmpty(userName))
                return;

            if (!string.IsNullOrEmpty(domain))
                return;

            var parts = userName.Split('\\');
            if (parts.Length != 2)
                return;

            domain = parts[0];
            userName = parts[1];
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedMember.Local

        private const int ERROR_INVALID_PARAMETER = 0x57;
        // Token Specific Access Rights

        private const uint TOKEN_DUPLICATE = 0x0002;
        private const uint TOKEN_QUERY = 0x0008;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        private enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        private enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SID_AND_ATTRIBUTES
        {
            public readonly IntPtr Sid;
            public readonly uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_ELEVATION
        {
            public readonly int TokenIsElevated;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_MANDATORY_LABEL
        {
            public SID_AND_ATTRIBUTES Label;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr), CanBeNull]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr), CanBeNull]
            public string pszCaptionText;
            public readonly IntPtr hbmBanner;
        }

        private enum SHSTOCKICONID : uint
        {
            SIID_SHIELD = 77
        }

        [Flags]
        private enum SHGSI : uint
        {
            SHGSI_ICON = 0x000000100,
            SHGSI_SMALLICON = 0x000000001
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public readonly IntPtr hIcon;
            public readonly int iSysIconIndex;
            public readonly int iIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            [CanBeNull]
            public readonly string szPath;
        }

        private static class NativeMethods
        {
            [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenProcessToken(IntPtr hProcess, uint desiredAccess, out IntPtr hToken);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DuplicateToken([NotNull] SafeTokenHandle existingTokenHandle, SECURITY_IMPERSONATION_LEVEL impersonationLevel, out IntPtr duplicateTokenHandle);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetTokenInformation([NotNull] SafeTokenHandle hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr pTokenInfo, int tokenInfoLength, out int returnLength);
            public static bool GetTokenInformation([NotNull] SafeTokenHandle hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, [NotNull] SafeNativeMemory pTokenInfo)
            {
                return GetTokenInformation(hToken, tokenInfoClass, pTokenInfo.DangerousGetHandle(), pTokenInfo.Size, out _);
            }

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool LogonUser([CanBeNull] string lpszUsername, [CanBeNull] string lpszDomain, [CanBeNull] string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

            [DllImport("credui.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.U4)]
            public static extern uint CredUIPromptForWindowsCredentials(ref CREDUI_INFO info, int authError, ref uint authPackage, IntPtr inAuthBuffer,
                int inAuthBufferSize, out IntPtr refOutAuthBuffer, out int refOutAuthBufferSize, [MarshalAs(UnmanagedType.Bool)] ref bool fSave, int flags);

            [DllImport("credui.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, int cbAuthBuffer, [CanBeNull] StringBuilder pszUserName, ref int pcchMaxUserName, [CanBeNull] StringBuilder pszDomainName, ref int pcchMaxDomainame, [CanBeNull] StringBuilder pszPassword, ref int pcchMaxPassword);

            [DllImport("credui.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CredPackAuthenticationBuffer(int dwFlags, [CanBeNull] string pszUserName, [CanBeNull] string pszPassword, IntPtr pPackedCredentials, ref int pcbPackedCredentials);

            [DllImport("Shell32.dll", SetLastError = false)]
            public static extern int SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);
        }
    }
}