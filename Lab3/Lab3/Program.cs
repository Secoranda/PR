using System;
using Flurl.Http;
using Flurl;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using System.Text;

namespace Lab3
{
    class Program
    {

        static void Main(string[] args)
        {
            using (var client = new FlurlClient("https://httpbin.org"))
            {
                Console.WriteLine("GET METHOD:");
                Console.WriteLine(Getmethod(client).GetAwaiter().GetResult());
                Console.WriteLine("POST METHOD:");
                Console.WriteLine(Postmethod(client).GetAwaiter().GetResult());
                Console.WriteLine("PUT METHOD:");
                Console.WriteLine(Putmethod(client).GetAwaiter().GetResult());
                Console.WriteLine("DELETE METHOD:");
                Console.WriteLine(Deletemethod(client).GetAwaiter().GetResult());
                Console.WriteLine("PATCH METHOD:");
                Console.WriteLine(Patchmethod(client).GetAwaiter().GetResult());
                Console.ReadKey();
            }
        }

        static async Task<String> Getmethod(FlurlClient client)
        {
            var image = await client.Request("/image/svg").GetStringAsync();
            return image;
        }

        static async Task<String> Postmethod(FlurlClient client)
        {
            var values = new Dictionary<string, string>
            {
                //any values/anything will work,but will keep it empty for checking result with seversite
            };
            var posting = await client.Request("/post").PostUrlEncodedAsync(values).Result.Content.ReadAsStringAsync();
            return posting;
        }
  

        static async Task<String> Deletemethod(FlurlClient client)
        {
            
            var delm = await client.Request("/delete").DeleteAsync().Result.Content.ReadAsStringAsync();
            return delm;
        }

        static async Task<String> Putmethod(FlurlClient client)
        {
            var jsonString = "{Something in my data TROLOOLOOLOO}";
            var HttpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var putm = await client.Request("/put").PutAsync(HttpContent, default,  HttpCompletionOption.ResponseContentRead).Result.Content.ReadAsStringAsync();
            return putm;
        }

        static async Task<String> Patchmethod(FlurlClient client)
        {
            var patch = await client.Request("/patch").PatchAsync(null, default, HttpCompletionOption.ResponseContentRead).Result.Content.ReadAsStringAsync();
            return patch;
        }

    }
}
    

