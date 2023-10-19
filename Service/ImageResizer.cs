using SixLabors.ImageSharp.Formats.Jpeg;
using Image = SixLabors.ImageSharp.Image;

public class ImageResizer
{
    public static async Task<byte[]> ReduceFileSizeAsync(byte[] imageBytes)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = await Image.LoadAsync(inputStream);

        var encoder = new JpegEncoder { Quality = 85 };  // Set the compression quality (0 to 100, lower is higher compression/smaller file size)

        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, encoder);

        return outputStream.ToArray();
    }
}
