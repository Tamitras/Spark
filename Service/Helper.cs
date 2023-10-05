using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using Image = SixLabors.ImageSharp.Image;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using Size = SixLabors.ImageSharp.Size;

public class Helper
{
    ///// <summary>
    ///// Convert Image Stream to .jpeg byte array
    ///// </summary>
    ///// <param name="imageStream"></param>
    ///// <returns></returns>
    //public static byte[] ConvertImageToJpeg(Stream imageStream)
    //{
    //    try
    //    {
    //        using var bitmap = SKBitmap.Decode(imageStream);
    //        using var image = SKImage.FromBitmap(bitmap);
    //        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);  // 100 ist die Qualität (0 bis 100)
    //        return data.ToArray();
    //    }
    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}


public static byte[] ResizeAndCompressImage(byte[] originalImageBytes, int maxFileSizeKb, int maxWidthHeight)
{
    using var image = Image.Load(originalImageBytes);

    // Verkleinern des Bildes
    image.Mutate(x => x.Resize(new ResizeOptions
    {
        Size = new Size(maxWidthHeight, maxWidthHeight),
        Mode = ResizeMode.Max
    }));

    int quality = 90;  // Startqualität für JPEG-Kompression
    byte[] resizedImageBytes = null;

    while (quality > 0)
    {
        using var outputMemoryStream = new MemoryStream();
        image.Save(outputMemoryStream, new JpegEncoder { Quality = quality });
        resizedImageBytes = outputMemoryStream.ToArray();
        if (resizedImageBytes.Length <= maxFileSizeKb * 1024)
        {
            break;
        }
        quality -= 5;  // Qualität für den nächsten Durchlauf reduzieren
    }

    return resizedImageBytes;
}

    public static byte[] ResizeImageToMaxSize(byte[] originalImageBytes, int maxFileSizeKb, int maxWidthHeight)
    {
        int quality = 100;  // Startqualität für JPEG-Kompression
        byte[] resizedImageBytes = null;

        while (quality > 0)  // Weitermachen, bis die Qualität 0 erreicht (sollte nie passieren)
        {
            // Bild aus Byte-Array dekodieren
            using var inputMemoryStream = new MemoryStream(originalImageBytes);
            using var originalBitmap = SKBitmap.Decode(inputMemoryStream);

            // Neue Abmessungen berechnen, um die Proportionen beizubehalten
            var ratio = (double)maxWidthHeight / Math.Max(originalBitmap.Width, originalBitmap.Height);
            int newWidth = (int)(originalBitmap.Width * ratio);
            int newHeight = (int)(originalBitmap.Height * ratio);

            // Bildgröße ändern
            using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
            using var resizedImage = SKImage.FromBitmap(resizedBitmap);

            // Bild in JPEG mit der aktuellen Qualität komprimieren
            using var data = resizedImage.Encode(SKEncodedImageFormat.Png, quality);

            double fileSizeInKB = Helper.GetFileSizeInKB(data.ToArray());
            Console.WriteLine($"File size new: {fileSizeInKB} KB of {maxFileSizeKb}");

            // Wenn die Dateigröße unter dem Maximum liegt, aber immer noch über einem akzeptablen Wert liegt, beenden
            if (fileSizeInKB <= maxFileSizeKb)
            {
                resizedImageBytes = data.ToArray();
                break;
            }

            // Qualität für den nächsten Durchlauf reduzieren
            quality -= 2;
        }

        return resizedImageBytes;
    }


    //public static byte[] ResizeImageToMaxSize(byte[] originalImageBytes, int maxFileSizeKb, int maxWidthHeight)
    //    {
    //        int quality = 90;  // Startqualität für JPEG-Kompression
    //        byte[] resizedImageBytes = null;

    //        while (quality > 0)  // Weitermachen, bis die Qualität 0 erreicht (sollte nie passieren)
    //        {
    //            // Bild aus Byte-Array dekodieren
    //            using var inputMemoryStream = new MemoryStream(originalImageBytes);
    //            using var originalBitmap = SKBitmap.Decode(inputMemoryStream);

    //            // Neue Abmessungen berechnen, um die Proportionen beizubehalten
    //            var ratio = (double)maxWidthHeight / Math.Max(originalBitmap.Width, originalBitmap.Height);
    //            int newWidth = (int)(originalBitmap.Width * ratio);
    //            int newHeight = (int)(originalBitmap.Height * ratio);

    //            // Bildgröße ändern
    //            using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
    //            using var resizedImage = SKImage.FromBitmap(resizedBitmap);

    //            // Bild in JPEG mit der aktuellen Qualität komprimieren
    //            using var data = resizedImage.Encode(SKEncodedImageFormat.Jpeg, quality);

    //            double fileSizeInKB = Helper.GetFileSizeInKB(data.ToArray());
    //            Console.WriteLine($"File size new: {fileSizeInKB} KB");

    //            // Wenn die Dateigröße unter dem Maximum liegt, beenden
    //            if (data.Size <= maxFileSizeKb * 1024)
    //            {
    //                resizedImageBytes = data.ToArray();
    //                break;
    //            }

    //            // Qualität für den nächsten Durchlauf reduzieren
    //            quality -= 5;
    //        }

    //        return resizedImageBytes;
    //    }

    public static double GetFileSizeInKB(byte[] fileBytes)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            return 0;
        return fileBytes.Length / 1024.0;
    }

}
