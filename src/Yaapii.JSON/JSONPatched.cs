using Newtonsoft.Json.Linq;
using System;
using Yaapii.Atoms;
using Yaapii.Atoms.Error;

namespace Yaapii.JSON
{
    /// <summary>
    /// A json, patched with json path and new value
    /// </summary>
    public sealed class JSONPatched : JSONEnvelope
    {
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
        public JSONPatched(IJSON json, string jsonPath, JToken value) : base(() =>
            {
                var jObject = json.Token();
                var tokenValue = jObject.SelectToken(jsonPath);
                new FailNull(
                    tokenValue,
                    new InvalidOperationException($"Can't patch value for json path '{jsonPath}', because the path doesn't exist in:{Environment.NewLine} {jObject.ToString()}")
                ).Go();
                jObject.SelectToken(jsonPath).Replace(value);
                return new JSONOf(jObject);
            },
            false
        )
        { }
    }
}
