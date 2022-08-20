namespace TomsToolbox.Desktop;

using System;
using System.IO;
using System.Net;

/// <summary>
/// Helper for web methods.
/// </summary>
public static class WebHelper
{
    /// <summary>
    /// Creates an HTTP web request with the system proxy settings.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>The request.</returns>
    public static WebRequest CreateHttpWebRequest(Uri uri)
    {
        var webRequest = WebRequest.Create(uri);
        var webProxy = WebRequest.DefaultWebProxy ?? new WebProxy();
        webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        webRequest.Proxy = webProxy;

        return webRequest;
    }

    /// <summary>
    /// Downloads the data from the specified URI using a GET request.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>A stream containing the downloaded data.</returns>
    public static MemoryStream Download(Uri uri)
    {
        var webRequest = CreateHttpWebRequest(uri);
        var localStream = new MemoryStream();

        using (var webResponse = webRequest.GetResponse())
        {
            using (var responseStream = webResponse.GetResponseStream())
            {
                responseStream?.CopyTo(localStream);
            }
        }

        localStream.Position = 0;
        return localStream;
    }
}