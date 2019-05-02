using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Yaapii.Atoms;
using Yaapii.Atoms.Error;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace Yaapii.JSON
{
    /// <summary>
    /// A readonly JSON, read from a data source.
    /// </summary>
    public sealed class JSONOf : IJSON
    {
        private readonly IScalar<JToken> token;
        private readonly Encoding encoding;

        /// <summary>
        /// A readonly JSON from a input.
        /// It is assumed that the encoding is UTF8.
        /// </summary>
        public JSONOf(IInput json) : this(json, Encoding.Default)
        { }

        /// <summary>
        /// A readonly JSON from a string.
        /// </summary>
        public JSONOf(string json) : this(new InputOf(json), Encoding.Default)
        { }

        /// <summary>
        /// A readonly JSON from a string.
        /// </summary>
        public JSONOf(string json, Encoding encoding) : this(new InputOf(json), encoding)
        { }

        /// <summary>
        /// A readonly JSON from a <see cref="IInput"/>.
        /// </summary>
        public JSONOf(IInput json, Encoding encoding) : this(
            new Sticky<string>(() =>
            {
                var str = new TextOf(json, encoding).AsString();
                if (String.IsNullOrEmpty(str))
                {
                    throw new ArgumentException("cannot work with empty json.");
                }
                return str;
            })
        )
        { }

        /// <summary>
        /// A readonly JSON from a <see cref="JToken"/>.
        /// </summary>
        public JSONOf(JToken token) : this(new ScalarOf<JToken>(token))
        { }

        /// <summary>
        /// A readonly JSON from a <see cref="IScalar{OutValue}"/>.
        /// </summary>
        public JSONOf(IScalar<string> json) : this(new ScalarOf<JToken>(() =>
        {
            try
            {
                return JToken.Parse(json.Value());
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentException($"Cannot parse json: {ex.Message} - {json.Value()}");
            }
        })
        )
        { }

        /// <summary>
        /// A readonly JSON from a <see cref="IScalar{OutValue}"/>.
        /// </summary>
        public JSONOf(IScalar<JToken> json)
        {
            this.token = json;
        }

        public IJSON Node(string jsonPath)
        {
            var nodes = this.Nodes(jsonPath);
            new FailEmpty<IJSON>(
                nodes,
                new ArgumentException("Cannot find '" + jsonPath + "' in\r\n" + this.token.Value().ToString())
            ).Go();
            if (nodes.Count > 1)
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Only a single node is retrievable with Node(), but '{0}' was: {1}",
                            jsonPath,
                            new Joined(
                                ",\r\n",
                                new Yaapii.Atoms.Enumerable.Mapped<IJSON, IText>(
                                    node => new TextOf(node.Token().ToString()),
                                    nodes
                                )
                            ).AsString()
                        ).AsString()
                    );
            }
            return nodes[0];
        }

        public IList<IJSON> Nodes(string jsonPath)
        {
            var result = new List<IJSON>();
            var tokens = new List<JToken>(this.token.Value().SelectTokens(jsonPath));
            foreach (var token in tokens)
            {
                if (token is JObject || token is JProperty)
                {
                    result.Add(new JSONOf(token));
                }
                else
                {
                    throw new ArgumentException(
                        new Formatted(
                            "Only objects and properties are retrievable with node(), but you selection includes: '{0}'",
                            token.ToString()
                        ).AsString()
                    );
                }
            }
            return new ListOf<IJSON>(result); //make immutable
        }

        public IList<string> Values(string jsonPath)
        {
            IList<string> result = new List<string>();
            IList<JToken> tokens = new List<JToken>(this.token.Value().SelectTokens(jsonPath));

            foreach (var token in tokens)
            {
                if (token is JValue)
                {
                    result.Add((token as JValue).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    throw
                        new ArgumentException(
                            new Formatted(
                                "Only values are retrievable with values(), but your selection includes: '{0}'",
                                token.ToString()
                            ).AsString()
                        );
                }
            }
            return new ListOf<string>(result); //make immutable
        }

        public string Value(string jsonPath)
        {
            IList<string> values = this.Values(jsonPath);
            new FailEmpty<string>(
                values,
                new ArgumentException("Cannot find '" + jsonPath + "' in\r\n" + this.token.Value().ToString())
            ).Go();
            if (values.Count > 1)
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Only a single value is retrievable with Value(), but '{0}' was: {1}",
                            jsonPath,
                            new Joined(
                                ",\r\n",
                                values
                            ).AsString()
                        ).AsString()
                    );
            }
            return values[0];
        }

        public JToken Token()
        {
            return this.token.Value();
        }

        public override string ToString()
        {
            return this.token.Value().ToString();
        }
    }
}
