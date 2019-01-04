using System;

namespace Yaapii.Json
{
    /// <summary>
    /// Tell that a json is invalid when tested against a json schema.
    /// </summary>
    public sealed class InvalidJSONException : Exception
    {
        /// <summary>
        /// Tell that a json is invalid when tested against a json schema.
        /// </summary>
        public InvalidJSONException() : base()
        { }

        public InvalidJSONException(string message) : base(message)
        { }
    }
}
