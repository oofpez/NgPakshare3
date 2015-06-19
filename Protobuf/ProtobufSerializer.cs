using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;

namespace Wimt.CachingFramework.Protobuf
{
    public static class ProtobufSerializer
    {

        public static bool isValidProtoObject(object o)
        {
            var hasAttribute = Attribute.IsDefined(o.GetType(), typeof(ProtoContractAttribute));

            return hasAttribute;
        }

        public static void SerisalizeAndWrite<T>(T protoObject,Stream writeStream )
        {
            try
            {
                Serializer.Serialize( writeStream, protoObject);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Protobuf stream serialisation of {0} failed unexpectedly", typeof(T)), ex);
            }

        }

        public static T DeserializeAndWrite<T>(Stream readStream)
        {
            try
            {
                return Serializer.Deserialize<T>(readStream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Protobuf stream deserialisation of {0} failed unexpectedly", typeof(T)), ex);
            }

        }
            
        
    }
}
