using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Xunit;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Text;

namespace Yaapii.JSON.Test
{
    public sealed class JSONTests
    {
        [Fact]
        public void RetrievesValue()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Equal("default", json.Value("addresses[:1].type"));
        }

        [Fact]
        public void UsesUTF8EncodingPerDefault()
        {
            var result =
                new BytesOf(
                    new JObject(
                        new JProperty("value", "öäü")
                    ).ToString(),
                    Encoding.UTF8
                ).AsBytes();
            var json =
                new JSONOf(
                    new InputOf(result)
                );

            Assert.Equal(
                result, 
                new BytesOf(
                    json.Token().ToString(),
                    Encoding.UTF8
                ).AsBytes()
            );
        }

        [Theory]
        [InlineData("UTF-7")]
        [InlineData("UTF-8")]
        [InlineData("UTF-16")]
        [InlineData("UTF-32")]
        public void DealsWithEncodings(string name)
        {
            var encoding = Encoding.GetEncoding(name);
            var inBytes = encoding.GetBytes("{ text: 'üöä' }");

            Assert.Equal(
                "üöä",
                new JSONOf(
                    new InputOf(inBytes),
                    encoding
                ).Value("$.text")
            );
        }


        [Fact]
        public void RejectsEmptyString()
        {
            Assert.Throws<ArgumentException>(() => new JSONOf(String.Empty).Token());
        }

        [Fact]
        public void ErrorWhenNotAValue()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Value("addresses"));
        }

        [Fact]
        public void ErrorWhenMultipleValues()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Value("addresses[*].type"));
        }

        [Fact]
        public void ErrorWhenValueNotFound()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Value("XYZ"));
        }

        [Fact]
        public void RetrievesJson()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Equal(
                JObject.Parse("{  \"name\": \"Drugstore\",  \"type\": \"default\"}").ToString(),
                json.Node("addresses[:1]").Token().ToString()
            );
        }

        [Fact]
        public void ErrorWhenNotANode()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Node("name"));
        }

        [Fact]
        public void ErrorWhenMultipleNodes()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Node("addresses[*].type"));
        }

        [Fact]
        public void RetrievesMultipleNodes()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.True(json.Nodes("addresses[*]").Count == 2);
        }

        [Fact]
        public void RejectsInvalidTypeInMultipleNodes()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.Throws<ArgumentException>(() => json.Nodes("$.*").Count);
        }

        [Fact]
        public void RetrievesMultipleValues()
        {
            var json =
                new JSONOf(
                    new ResourceOf(
                        "Datum/data.json",
                        typeof(JSONTests)
                    )
                );

            Assert.True(json.Values("addresses[*].type").Count == 2);
        }
    }
}
