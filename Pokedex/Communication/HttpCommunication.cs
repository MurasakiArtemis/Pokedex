using System.Net.Http;
using System.Threading.Tasks;

namespace Pokedex.Communication
{
    class HttpCommunication
    {
        private HttpClient client;
        public HttpCommunication()
        {
            client = new HttpClient();
        }
        public async Task<string> GetResponse(string url)
        {
            string result = "";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return result;
        }
    }
}
