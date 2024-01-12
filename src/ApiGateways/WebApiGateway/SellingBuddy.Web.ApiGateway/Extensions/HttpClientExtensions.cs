namespace SellingBuddy.Web.ApiGateway.Extensions;

public static class HttpClientExtensions
{
    public async static Task<TResult> PostGetResponseAsync<TResult, TValue>(this HttpClient client, String url, TValue value)
    {
        var httpResult = await client.PostAsJsonAsync(url, value);

        return httpResult.IsSuccessStatusCode ? await httpResult.Content.ReadFromJsonAsync<TResult>() : default;
    }

    public async static Task PostAsync<TValue>(this HttpClient client, String url, TValue value)
    {
        await client.PostAsJsonAsync(url, value);
    }

    public async static Task<T> GetResponseAsync<T>(this HttpClient client, String url)
    {
        return await client.GetFromJsonAsync<T>(url);
    }
}
