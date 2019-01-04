﻿using Xunit;

namespace Yaapii.JSON.Test
{
    public class StrictJSONTests
    {
        [Fact]
        public void RejectsInvalidJson()
        {
            var json = "{ \"test\": \"a word\" }";
            var schema = "{ \"type\": \"object\", \"properties\": { \"test\": { \"type\": \"number\" } } }";

            Assert.Throws<InvalidJSONException>(() =>
            {
                new StrictJSON(new JSON(json), schema).Value("test");
            });
        }

        [Fact]
        public void AcceptsValidJson()
        {
            var json = "{ \"test\": \"a word\" }";
            var schema = "{ \"type\": \"object\", \"properties\": { \"test\": { \"type\": \"string\" } } }";

            Assert.Equal(
                "a word",
                new StrictJSON(
                    new JSON(json),
                    schema
                ).Value("test")
            );
        }
    }
}