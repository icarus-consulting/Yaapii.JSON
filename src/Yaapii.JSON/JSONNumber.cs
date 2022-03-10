using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;

namespace Yaapii.JSON
{
    /// <summary>
    /// <see cref="IsNumber"/> extracted from an json using jsonPath.
    /// </summary>
    public sealed class JSONNumber : NumberEnvelope
    {
        /// <summary>
        /// <see cref="IsNumber"/> extracted from an json using jsonPath.
        /// </summary>
        public JSONNumber(IJSON json, string jsonPath) : base(
            new ScalarOf<double>(() =>
                new NumberOf(json.Value(jsonPath)).AsDouble()
            )
        )
        { }
    }
}
