using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlitFlashNet.Structures
{
    public class GithubAuthor
    {
        private string login;

        [JsonPropertyName("login")]
        public string Login
        {
            get => login;
            set => login = value;
        }
    }
}
