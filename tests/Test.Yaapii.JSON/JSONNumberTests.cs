using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.JSON;
using Yaapii.JSON.Test;

namespace Test.Yaapii.JSON
{
    public sealed class JSONNumberTests
    {
        [Fact]
        public void ReadsNumber()
        {
            var json =
               new JSONOf(
                   new ResourceOf(
                       "Datum/data.json",
                       typeof(JSONTests)
                   )
               );
            Assert.Equal(4, new JSONNumber(json, $"$.addresses[0].number").AsInt());
        }

        [Fact]
        public void RejectsInvalidString()
        {
            var json =
               new JSONOf(
                   new ResourceOf(
                       "Datum/data.json",
                       typeof(JSONTests)
                   )
               );
            Assert.Throws<System.ArgumentException>(() => new JSONNumber(json, $"$.addresses[0].type").AsInt());
        }
    }
}
