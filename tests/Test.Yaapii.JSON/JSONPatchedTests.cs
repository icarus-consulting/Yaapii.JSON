using Newtonsoft.Json.Linq;
using Xunit;
using Yaapii.Atoms.IO;

namespace Yaapii.JSON.Test
{
    public sealed class JSONPatchedTests
    {
        [Fact]
        public void Patches2()
        {
            var unpatched =
             new JSONOf(
                 new ResourceOf(
                     "Datum/data.json",
                     typeof(JSONTests)
                 )
             );

            var patched =
                new JSONPatched(
                    unpatched,
                    $"$.addresses[0].name",
                    "Quick Stop"
                );

            Assert.Equal(
               "Quick Stop",
               patched.Value($"$.addresses[0].name")
           );
        }
       
        [Fact]
        public void WorksOnMultipleLevels()
        {
            Assert.Equal(
                "success",
                new JSONPatched(
                    new JSONOf(
                        new JObject(
                            new JProperty("object",
                                new JObject(
                                    new JProperty("value", "empty")
                                )
                            )
                        )
                    ),
                    "object.value",
                    "success"
                ).Value("object.value")
            );
        }
    }
}
