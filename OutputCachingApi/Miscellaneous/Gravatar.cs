using System.Net.Mime;

namespace OutputCachingApi.Miscellaneous;

public static class Gravatar
{
    private const string _type = "monsterid"; // identicon, monsterid, wavatar
    private const int _size    = 150;

    public static async Task WriteGravatar(HttpContext context)
    {
        string hash = Guid.NewGuid().ToString("n");

        context.Response.ContentType = MediaTypeNames.Text.Html;

        await context.Response.WriteAsync($"<img src=\"https://www.gravatar.com/avatar/{hash}?s={_size}&d={_type}\"/>");

        await context.Response.WriteAsync($"<pre>Generated at {DateTime.Now:hh:mm:ss.ff}</pre>");
    }
}
