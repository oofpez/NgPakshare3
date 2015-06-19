using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Wimt.CachingFramework.Configuration;
using Wimt.CachingFramework.Extensions;

namespace Wimt.CachingFramework.Caches
{
    public class BlobCache : BaseCache
    {
        protected CloudBlobContainer BlobContainer { get; private set; }

        public BlobCache(string name, bool isEnabled, string storageAccount, string tableName)
            : base(name, isEnabled)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageAccount);

            var client = cloudStorageAccount.CreateCloudBlobClient();
            client.DefaultRequestOptions = new BlobRequestOptions()
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 3)
            };

            BlobContainer = client.GetContainerReference(tableName);
        }

        public override object Get(string key)
        {
            BlobContainer.CreateIfNotExists();
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(key);
            string leaseId;

            if (!blob.Exists())
            {
                return null;
            }

            try
            {
                leaseId = blob.AcquireLease(TimeSpan.FromSeconds(60), null);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode != 409 &&
                    ex.RequestInformation.HttpStatusCode != 412 &&
                    ex.RequestInformation.HttpStatusCode != 404)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }

            if (blob.ExpiresAt() == null)
            {
                throw new ArgumentException(String.Format("Blob {0} does not have expiry date metadata at {1}", key, BlobContainer.Uri), "key");
            }

            if (blob.ExpiresAt() <= DateTime.UtcNow)
            {
                blob.DeleteIfExistsAsync();
                return null;
            }

            try
            {
                var formatter = new BinaryFormatter()
                {
                    AssemblyFormat = FormatterAssemblyStyle.Simple,
                    FilterLevel = TypeFilterLevel.Low,
                    TypeFormat = FormatterTypeStyle.TypesAlways
                };

                using (var stream = blob.OpenRead())
                {
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Blob stream deserialisation of {0} in {1} failed unexpectedly", key, BlobContainer.Uri), ex);
            }
            finally
            {
                blob.ReleaseLease(new AccessCondition() { LeaseId = leaseId });
            }
        }

        public override void Set(string key, object value)
        {
            this.Set(key, value, DateTime.MaxValue);
        }

        public override void Set(string key, object value, TimeSpan validFor)
        {
            var expiresAt = DateTime.UtcNow.Add(validFor);

            this.Set(key, value, expiresAt);
        }

        public override void Set(string key, object value, DateTime expiresAt)
        {
            BlobContainer.CreateIfNotExists();
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(key);

            blob.SetExpiry(expiresAt);

            try
            {
                var formatter = new BinaryFormatter()
                {
                    AssemblyFormat = FormatterAssemblyStyle.Simple,
                    FilterLevel = TypeFilterLevel.Low,
                    TypeFormat = FormatterTypeStyle.TypesAlways
                };

                using (var stream = blob.OpenWrite())
                {
                    formatter.Serialize(stream, value);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Blob stream serialisation of {0} in {1} failed unexpectedly", key, BlobContainer.Uri), ex);
            }
        }

        public override void Remove(string key)
        {
            BlobContainer.CreateIfNotExists();
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(key);
            blob.DeleteIfExists();
        }

        public override void Clear()
        {
            if (BlobContainer.Exists())
            {
                foreach (var blob in BlobContainer.ListBlobs())
                {
                    var blockBlob = blob as CloudBlockBlob;

                    if (blockBlob != null)
                    {
                        blockBlob.DeleteIfExists();
                    }
                }
            }
        }

        public override bool Exists(string key)
        {
            if (BlobContainer.Exists())
            {
                return BlobContainer.GetBlockBlobReference(key).Exists();
            }

            return false;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        internal static BlobCache InitialiseFrom(BlobCacheElement element)
        {
            var storageAccount = CloudConfigurationManager.GetSetting(element.StorageAccountKey);

            if (String.IsNullOrWhiteSpace(storageAccount))
            {
                throw new ArgumentNullException("StorageAccountKey", String.Format("There is no configuration item for '{0}'.", element.StorageAccountKey));
            }

            return new BlobCache(element.Name, element.IsEnabled, storageAccount, element.Container);
        }
    }
}