using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Text;

namespace Yaapii.JSON
{
    /// <summary>
    /// A JSON whose format is validated using a JSON schema.
    /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
    /// </summary>
    public sealed class StrictJSON : JSONEnvelope
    {
        private readonly IScalar<IJSON> origin;

        /// <summary>
        /// A JSON whose format is validated using a JSON schema.
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IInput origin, IInput schema) : this(new JSONOf(origin), new TextOf(schema))
        { }

        /// <summary>
        /// A JSON whose format is validated using a JSON schema.
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IJSON origin, IInput schema) : this(origin, new TextOf(schema))
        { }

        ///<summary>
        /// A JSON whose format is validated using a JSON schema.
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IInput origin, string schema) : this(new JSONOf(origin), new TextOf(schema))
        { }

        /// <summary>
        /// A JSON whose format is validated using a JSON schema.
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IJSON origin, string schema) : this(origin, new TextOf(schema))
        { }

        /// <summary>
        /// A JSON whose format is validated using a JSON schema.
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IInput origin, IText schema) : this(new JSONOf(origin), schema)
        { }

        /// <summary>
        /// A JSON whose format is validated using a JSON schema. 
        /// Throws a <see cref="InvalidJSONException"/> if the JSON does not match the schema.
        /// </summary>
        public StrictJSON(IJSON origin, IText schema) : base(() =>
            {
                try
                {
                    IList<string> errors = new List<string>();
                    var token = origin.Token();
                    var jschema = JSchema.Parse(schema.AsString());
                    if (!token.IsValid(jschema, out errors))
                    {
                        throw
                            new InvalidJSONException(
                                new Formatted(
                                    "Json is invalid: \r\n{0}\r\n\r\nJson: {1}",
                                    new Joined("\r\n", errors).AsString(),
                                    token.ToString()
                                ).AsString()
                            );
                    }
                    return origin;
                }
                catch (JsonReaderException ex)
                {
                    throw new InvalidJSONException($"Cannot parse json: " + ex.Message);
                }
            },
            false
        )
        { }
    }
}
