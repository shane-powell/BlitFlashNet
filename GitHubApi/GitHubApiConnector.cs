using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlitFlashNet.Structures;

namespace BlitFlashNet.GitHubApi
{
    public static class GitHubApiConnector
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<GitHubRelease> GetLatestRelease(string owner, string repo)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                var streamTask = client.GetStreamAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
                var release = await JsonSerializer.DeserializeAsync<GitHubRelease>(await streamTask);

                return release;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public static async Task<List<GitHubReleaseAsset>> GetReleaseAssets(GitHubRelease release)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                var streamTask = client.GetStreamAsync($"https://api.github.com/repos/Pimoroni/32blit-beta/releases/{release.Id}/assets");
                var assets = await JsonSerializer.DeserializeAsync<List<GitHubReleaseAsset>>(await streamTask);

                return assets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
