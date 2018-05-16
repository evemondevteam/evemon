using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Models.Extended;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Serializable class for an ESI method and its path. Each APIConfiguration maintains a list of APIMethodsEnum.
    /// </summary>
    public class ESIMethod
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        private ESIMethod(Enum method, string path)
        {
            Method = method;
            Path = path;
        }

        /// <summary>
        /// Returns the APIMethodsEnum enumeration member for this APIMethod.
        /// </summary>
        public Enum Method { get; }

        /// <summary>
        /// Returns the defined URL suffix path for this APIMethod.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a set of API methods with their default urls.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ESIMethod> CreateDefaultSet() =>
            ESIMethods.Methods.Where(method => method.ToString() != "None").Select(methodName =>
                new
                {
                    methodName,
                    methodURL = NetworkConstants.ResourceManager.GetString($"ESI{methodName}")
                }).Where(method => method.methodURL != null)
                .Select(method => new ESIMethod(method.methodName, method.methodURL));
    }
}
