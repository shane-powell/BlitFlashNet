using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlitFlashNet.Structures
{
    public class GitHubReleaseAsset
    {
        private int id;

        private string name;

        private string downloadUrl;

        private DateTime updatedDate;


        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("name")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl
        {
            get => downloadUrl;
            set => downloadUrl = value;
        }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedDate
        {
            get => updatedDate;
            set => updatedDate = value;
        }
    }
}
