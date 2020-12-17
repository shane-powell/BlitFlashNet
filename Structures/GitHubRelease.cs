using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlitFlashNet.Structures
{
    public class GitHubRelease
    {
        private int id;

        private string tagName;

        private List<GitHubReleaseAsset> assets;

        private GithubAuthor author;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("tag_name")]
        public string TagName { get => tagName; set => tagName = value; }

        [JsonPropertyName("assets")]
        public List<GitHubReleaseAsset> Assets
        {
            get => assets;
            set => assets = value;
        }

        [JsonPropertyName("author")]
        public GithubAuthor Author
        {
            get => author;
            set => author = value;
        }
    }
}
