using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Maui.Storage;
using SkiaSharp;
using Spark.Class;

public class Helper
{
    public static double GetImageSize(byte[] imageBytes)
    {
        // Größe des Bildes in Bytes
        int sizeInBytes = imageBytes.Length;

        // Umrechnung in Kilobytes (KB) und Megabytes (MB) für eine benutzerfreundlichere Ausgabe
        double sizeInKB = sizeInBytes / 1024.0;
        double sizeInMB = sizeInKB / 1024.0;

        
        Console.WriteLine($"Bildgröße: {sizeInBytes} Bytes, {sizeInKB:F2} KB, {sizeInMB:F2} MB");

        return sizeInKB;
    }

    public static SKBitmap GetImageDimensions(byte[] imageBytes)
    {
        using var inputMemoryStream = new MemoryStream(imageBytes);
        using var originalBitmap = SKBitmap.Decode(inputMemoryStream);

        return originalBitmap;
    }

    public static SKBitmap ResizeImage(SKBitmap source, int newWidth, int newHeight)
    {
        return source.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
    }

    public static SKBitmap ResizeImage(byte[] imageBytes, int newWidth, int newHeight)
    {
        try
        {
            using (var stream = new MemoryStream(imageBytes))
            using (var originalImage = SKBitmap.Decode(stream))
            {
                return originalImage.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Resize Image: {ex}");
        }

        return null;
    }

    public static ImageSource ToImageSource(SKBitmap skBitmap)
    {
        try
        {
            using (var image = SKImage.FromBitmap(skBitmap))
            using (var data = image.Encode())
            {
                var byteArray = data.ToArray();  // Erstellen einer Byte-Array-Kopie des Streams
                return ImageSource.FromStream(() => new MemoryStream(byteArray));  // Verwendung der Kopie
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SKBitmap to Imagesource failed: {ex}");
        }
        return null;
    }

    public static ImageSource ConvertToThumbnail(byte[] image)
    {
        int newWidth = 1280;
        int newHeight = 800;
        var temp = ResizeImage(image, newWidth, newHeight);

        return ToImageSource(temp);
    }

    public static byte[] ResizeImageToMaxSize(byte[] originalImageBytes, int maxFileSizeKb)
    {
        int quality = 100;  // Startqualität für JPEG-Kompression
        double factor = 0.0;
        byte[] resizedImageBytes = null;

        while (quality > 0)  // Weitermachen, bis die Qualität 0 erreicht (sollte nie passieren)
        {
            factor = quality / 100.0;
            // Bild aus Byte-Array dekodieren
            using var inputMemoryStream = new MemoryStream(originalImageBytes);
            using var originalBitmap = SKBitmap.Decode(inputMemoryStream);

            // Neue Abmessungen berechnen, um die Proportionen beizubehalten
            double ratio = (double)maxFileSizeKb / Math.Max(originalBitmap.Width, originalBitmap.Height);

            ratio = ratio * factor;
            int newWidth = (int)(originalBitmap.Width * ratio);
            int newHeight = (int)(originalBitmap.Height * ratio);

            // Bildgröße ändern
            using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
            using var resizedImage = SKImage.FromBitmap(resizedBitmap);

            // Bild in JPEG mit der aktuellen Qualität komprimieren
            using var data = resizedImage.Encode(SKEncodedImageFormat.Png, 100);

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



    public static double GetFileSizeInKB(byte[] fileBytes)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            return 0;
        return fileBytes.Length / 1024.0;
    }

    public static ObservableCollection<NumberWithScore> GetRankedPotentialNumbers(string input)
    {
        try
        {
            string pattern = @"(?<=\n)(\d+(\.\d+)?)([a-zA-Z]{0,3})?(?=\n)";
            MatchCollection matches = Regex.Matches(input, pattern);

            Dictionary<string, NumberWithScore> uniqueResults = new Dictionary<string, NumberWithScore>();
            ObservableCollection<NumberWithScore> obsList = new ObservableCollection<NumberWithScore>();

            foreach (Match match in matches)
            {
                int points = 0;
                var number = match.Groups[1].Value;
                var suffix = match.Groups[3].Value;

                // Punkte für Länge der Zahl ohne Dezimalstellen
                var numberWithoutDecimal = number.Split('.')[0];
                int length = numberWithoutDecimal.Length;
                if (length >= 6 && length <= 8)
                {
                    points += 150;
                }

                // Punkte für kwh
                if (suffix.ToLower() == "kwh")
                {
                    points += 100;
                }

                // Punkte für Dezimaltrenner
                if (number.Contains("."))
                {
                    points += 50;
                }

                // Punkte für 6-7 Ziffern vor dem Dezimaltrenner und nur eine danach
                if (length >= 6 && length <= 7 && number.Split('.').Length > 1 && number.Split('.')[1].Length == 1)
                {
                    points += 200;
                }

                // Punkte für andere Suffixe
                if (!string.IsNullOrEmpty(suffix) && suffix.Length <= 3 && suffix.ToLower() != "kwh")
                {
                    points += 20;
                }

                // Punkte für Gesamtlänge
                if (number.Length == 6)
                {
                    points += 20;
                }
                else if (number.Length == 5)
                {
                    points += 10;
                }

                var numberWithScore = new NumberWithScore
                {
                    NumberSequence = $"{number}{(string.IsNullOrEmpty(suffix) ? "" : " " + suffix)}",
                    Score = points
                };

                if (points > 0 && !uniqueResults.ContainsKey(numberWithScore.NumberSequence))
                {
                    uniqueResults.Add(numberWithScore.NumberSequence, numberWithScore);
                    obsList.Add(numberWithScore);
                }
            }

            // Liste nach Punktzahl sortieren und zurückgeben
            obsList.OrderByDescending(c => c.Score);

            return obsList;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
