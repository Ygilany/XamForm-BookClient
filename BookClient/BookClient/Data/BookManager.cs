using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BookClient.Data
{
    public class BookManager
    {
        const string Url = "http://xam150.azurewebsites.net/api/books/";
        private string authorizationKey;

        private async Task<HttpClient> GetClient()
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(authorizationKey))
            {
                authorizationKey = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
            }
            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            HttpClient client = await this.GetClient();
            string json = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<Book>>(json);

        }

        public async Task<Book> Add(string title, string author, string genre)
        {
            Book book = new Book()
            {
                ISBN = "",
                Title = title,
                Authors = new List<string>(new[] { author }),
                Genre = genre,
                PublishDate = DateTime.Now
            };
            StringContent bookContent = new StringContent(
                JsonConvert.SerializeObject(book),
                Encoding.UTF8,
                "application/json"
            );
            HttpClient client = await this.GetClient();
            HttpResponseMessage response = await client.PostAsync(Url, bookContent);

            return JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
        }

        public async Task Update(Book book)
        {
            HttpClient client = await GetClient();
            await client.PutAsync(
                Url + "/" + book.ISBN,
                new StringContent(
                    JsonConvert.SerializeObject(book),
                    Encoding.UTF8, "application/json"
                )
            );
        }

        public async Task Delete(string isbn)
        {
        	HttpClient client = await GetClient();
        	await client.DeleteAsync(Url + isbn);
        }
    }
}

