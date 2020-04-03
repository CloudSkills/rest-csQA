using System.Collections.Generic;
using Newtonsoft.Json;

namespace csQA.DTO
{
    public class Question
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }

        [JsonProperty("attachments")]
        public List<Link> Attachments { get; set; }
    }

    public class Link
    {
        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
