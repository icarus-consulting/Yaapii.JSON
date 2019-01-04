using Newtonsoft.Json.Linq;
using System;
using Xunit;
using Yaapii.Atoms.IO;

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
