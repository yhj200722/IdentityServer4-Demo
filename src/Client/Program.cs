// See https://aka.ms/new-console-template for more information
using IdentityModel.Client;


// discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}
else
{
    Console.WriteLine("成功获取发现文档");

    // request token
    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientId = "client",
        ClientSecret = "secret",
        Scope = "api1"
    });

    if (tokenResponse.IsError)
    {
        Console.WriteLine(tokenResponse.Error);
        return;
    }

    Console.WriteLine("获取token成功");
    Console.WriteLine(tokenResponse.Json);

    
    try
    {
        // call api
        var apiClient = new HttpClient();
        apiClient.SetBearerToken(tokenResponse.AccessToken);

        var response = await apiClient.GetAsync("https://localhost:6001/identity");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("响应内容");
            Console.WriteLine(content);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"访问api失败：{ex}");
        throw;
    }
    

    Console.ReadKey();
}