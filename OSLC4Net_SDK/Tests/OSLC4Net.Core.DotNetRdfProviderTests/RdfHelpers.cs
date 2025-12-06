using System.Net.Http.Formatting;
using System.Net.Http.Headers;

public static class RdfHelpers
{
    internal static async Task<string> SerializeAsync<T>(MediaTypeFormatter formatter, T value,
     MediaTypeHeaderValue mediaType)
    {
        Stream stream = new MemoryStream();
        await using (stream.ConfigureAwait(false))
        {
            using HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            await formatter.WriteToStreamAsync(typeof(T), value, stream, content, null);
            stream.Position = 0;

            return await content.ReadAsStringAsync();
        }
    }


    internal static async Task<T?> DeserializeAsync<T>(MediaTypeFormatter formatter, string str,
        MediaTypeHeaderValue mediaType) where T : class
    {
        Stream stream = new MemoryStream();
        await using (stream.ConfigureAwait(false))
        {
            await using var writer = new StreamWriter(stream);
            using HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            await writer.WriteAsync(str);
            await writer.FlushAsync();

            stream.Position = 0;

            return await formatter.ReadFromStreamAsync(typeof(T), stream, content, null) as T;
        }
    }
    internal static async Task<string> SerializeCollectionAsync<T>(MediaTypeFormatter formatter,
    IEnumerable<T> value, MediaTypeHeaderValue mediaType)
    {
        Stream stream = new MemoryStream();
        await using (stream.ConfigureAwait(false))
        {
            using HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            await formatter.WriteToStreamAsync(typeof(T), value, stream, content, null);
            stream.Position = 0;

            return await content.ReadAsStringAsync();
        }
    }



    internal static async Task<IEnumerable<T>?> DeserializeCollectionAsync<T>(
        MediaTypeFormatter formatter,
        string str, MediaTypeHeaderValue mediaType) where T : class
    {
        Stream stream = new MemoryStream();
        await using (stream.ConfigureAwait(false))
        {
            await using var writer = new StreamWriter(stream);
            using HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            await writer.WriteAsync(str);
            await writer.FlushAsync();

            stream.Position = 0;

            return await formatter.ReadFromStreamAsync(typeof(List<T>), stream, content, null) as
                IEnumerable<T>;
        }
    }
}