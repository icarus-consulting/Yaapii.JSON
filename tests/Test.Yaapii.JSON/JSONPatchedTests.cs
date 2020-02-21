using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Yaapii.JSON.Test
{
    public sealed class JSONPatchedTests
    {
        [Fact]
        public void Patches()
        {
            Assert.Equal(
                "success",
                new JSONPatched(
                    new JSONOf(
                        new JObject(
                            new JProperty("value", "empty")
                        )
                    ),
                    "value",
                    "success"
                ).Value("value")
            );
        }
    }
}
