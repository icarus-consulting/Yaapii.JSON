using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Yaapii.Atoms.Error;
using Yaapii.Atoms.Scalar;

namespace Yaapii.JSON
{
    /// <summary>
    /// A json, patched with json path and new value
    /// </summary>
    public sealed class JSONPatched : IJSON
    {
        private readonly Sticky<IJSON> json;

        /// <summary>
        /// A json, patched with json path and new value
        /// </summary>
        public JSONPatched(IJSON json, string jsonPath, float value) : this(json, jsonPath, new JValue(value))
        { }

        /// <summary>
        /// A json, patched with json path and new value
        /// </summary>
        public JSONPatched(IJSON json, string jsonPath, int value) : this(json, jsonPath, new JValue(value))
        { }

        /// <summary>
        /// A json, patched with json path and new value
        /// </summary>
        public JSONPatched(IJSON json, string jsonPath, double value) : this(json, jsonPath, new JValue(value))
        { }

        /// <summary>
        /// A json, patched with json path and new value
        /// </summary>
        public JSONPatched(IJSON json, string jsonPath, string value) : this(json, jsonPath, new JValue(value))
        { }
        
        /// <summary>
        /// A json, patched with json path and new value
        /// </summary>
        public JSONPatched(IJSON json, string jsonPath, JToken value) : this(() =>
        {
            var jObject = json.Token();
            var tokenValue = jObject.SelectToken(jsonPath);
            new FailNull(
                tokenValue,
                new InvalidOperationException($"Can't patch value for json path '{jsonPath}', because the path doesn't exist in:{Environment.NewLine} {jObject.ToString()}")
            ).Go();
            jObject[tokenValue.Path] = value;
            return new JSONOf(jObject);
        })
        { }

        private JSONPatched(Func<IJSON> json)
        {
            this.json = new Sticky<IJSON>(json);
        }
        public IJSON Node(string jsonPath)
        {
            return this.json.Value().Node(jsonPath);
        }

        public IList<IJSON> Nodes(string jsonPath)
        {
            return this.json.Value().Nodes(jsonPath);
        }

        public JToken Token()
        {
            return this.json.Value().Token();
        }

        public string Value(string jsonPath)
        {
            return this.json.Value().Value(jsonPath);
        }

        public IList<string> Values(string jsonPath)
        {
            return this.json.Value().Values(jsonPath);
        }
    }
}
