using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Wimt.CachingFramework.Extensions
{
    internal static class BlobExtensions
    {
        internal const string ExpiresAtKey = "expiresAt";

        public static DateTime ExpiresAt(this CloudBlockBlob blob)
        {
            return DateTime.Parse(blob.Metadata[ExpiresAtKey]);
        }

        public static void SetExpiry(this CloudBlockBlob blob, DateTime date)
        {
            blob.Metadata[ExpiresAtKey] = date.ToString();
        }
    }
}