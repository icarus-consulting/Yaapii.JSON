using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Scalar;

namespace Yaapii.JSON
{
    /// <summary>
    /// An envelope for <see cref="IJSON"/>
    /// </summary>
    public abstract class JSONEnvelope : IJSON
    {
        private readonly ScalarOf<IJSON> fixedOrigin;
        private readonly Func<IJSON> json;
        private readonly bool live;

        /// <summary>
        /// An envelope for <see cref="Yaapii.JSON.IJSON"/>
        /// </summary>
        public JSONEnvelope(IJSON json, bool live) : this(
            () => json,
            live
        )
        { }

        /// <summary>
        /// An envelope for <see cref="Yaapii.JSON.IJSON"/>
        /// </summary>
        public JSONEnvelope(Func<IJSON> json, bool live)
        {
            this.fixedOrigin = new ScalarOf<IJSON>(json);
            this.json = json;
            this.live = live;
        }

        public IJSON Node(string jsonPath)
        {
            return Document().Node(jsonPath);
        }

        public IList<IJSON> Nodes(string jsonPath)
        {
            return Document().Nodes(jsonPath);
        }

        public JToken Token()
        {
            return Document().Token();
        }

        public string Value(string jsonPath)
        {
            return Document().Value(jsonPath);
        }

        public IList<string> Values(string jsonPath)
        {
            return Document().Values(jsonPath);
        }

        private IJSON Document()
        {
            IJSON doc;
            if (this.live)
            {
                doc = this.json();
            }
            else
            {
                doc = this.fixedOrigin.Value();
            }

            return doc;
        }
    }
}
