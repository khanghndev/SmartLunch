using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var endpoint = "https://syd.cloud.appwrite.io/v1";
        var projectId = "69bab7660023ca1dd830";
        var bucketId = "69bfa6de000fdacda87d";
        var apiKey = "standard_0b339e8363452bfa4f19b257b466f629cc81806d0e60758f3e47e97eaa2934330811c8fd1451896ce231516677f2af546d6925679ef58ef15c19c6854d711c6fc8f9f9fac053c38ae678e01a58d7eeae5f5275c06987490198fc67853f667afc1ea3fb1f8fe7a550c4f836a7135239ab2c6ed7c0f6d909a7879ec0c24dbbc928";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Appwrite-Project", projectId);
        client.DefaultRequestHeaders.Add("X-Appwrite-Key", apiKey);

        var url = $"{endpoint}/storage/buckets/{bucketId}/files?limit=5";
        Console.WriteLine($"Listing files from: {url}");
        
        var res = await client.GetAsync(url);
        var content = await res.Content.ReadAsStringAsync();
        
        Console.WriteLine($"Status: {res.StatusCode}");
        Console.WriteLine($"Response: {content}");
    }
}
