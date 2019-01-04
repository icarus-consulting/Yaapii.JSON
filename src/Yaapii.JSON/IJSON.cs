using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Yaapii.JSON
{
    /// <summary>
    /// A readonly JSON document offering access to contents via jsonpath.
    /// https://goessner.net/articles/JsonPath/
    /// </summary>
    public interface IJSON
    {
        /// <summary>
        /// Retrieve a single node via jsonpath expression. 
        /// The selection result must be a single node, not more, not less, otherwise it fails.
        /// </summary>
        /// <param name="jsonPath">jsonpath selection</param>
        /// <returns>the node</returns>
        IJSON Node(string jsonPath);

        /// <summary>
        /// Retrieve single value via jsonpath expression. 
        /// The selection result must be a single node, not more, not less, otherwise it fails.
        /// </summary>
        /// <param name="jsonPath">jsonpath selection</param>
        /// <returns>the value</returns>
        string Value(string jsonPath);

        /// <summary>
        /// Retrieve a subset of nodes via jsonpath expression.
        /// The selection result can be any amount of nodes, including zero.
        /// </summary>
        /// <param name="jsonPath">jsonpath selection</param>
        /// <returns>selected nodes</returns>
        IList<IJSON> Nodes(string jsonPath);

        /// <summary>
        /// Retrieve a subset of values via jsonpath expression.
        /// The selection result can be any amount of nodes, including zero.
        /// </summary>
        /// <param name="jsonPath">jsonpath selection</param>
        /// <returns>selected values</returns>
        IList<string> Values(string jsonPath);

        /// <summary>
        /// Retrieve the raw token.
        /// </summary>
        /// <returns></returns>
        JToken Token();
    }
}