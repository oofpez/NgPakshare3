using PostSharp.Aspects;
using System;
using System.Linq;
using System.Text;

namespace Wimt.CachingFramework
{
    [Serializable]
    public class CacheKeyBuilder
    {
        private string signatureName;

        public CacheKeyBuilder(string signature)
        {
            signatureName = signature;
        }

        public string GetFriendlyKey(MethodInterceptionArgs methodArguments)
        {
            var stringBuilder = new StringBuilder(signatureName);
            stringBuilder.Append("(");

            foreach (var arg in methodArguments.Arguments.Where(x => x != null))
            {
                stringBuilder.Append(arg.ToString()).Append(",");
            }

            return stringBuilder.Remove(stringBuilder.Length - 1, 1).Append(")").ToString();
        }
    }
}