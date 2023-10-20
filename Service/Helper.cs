using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using F23.StringSimilarity;
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
        // TODO: Thumbnail creation depending on internetspeed
        // maximum 3 sec

        // lower resolution for lower upload

        // 2,5sec --> 25mbits upload
        int newWidth = 1920;
        int newHeight = 1024;

        // higher resolution for better upload


        var temp = ResizeImage(image, newWidth, newHeight);

        return ToImageSource(temp);
    }

    public static double GetFileSizeInKB(byte[] fileBytes)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            return 0;
        return fileBytes.Length / 1024.0;
    }

    /// <summary>
    /// Finds the best match for a specific string
    /// </summary>
    /// <param name="target"></param>
    /// <param name="strings"></param>
    /// <returns></returns>
    public static (string bestMatch, double highestSimilarity) FindBestMatch(string target, List<string> strings)
    {
        string bestMatch = null;
        double highestSimilarity = 0;

        NormalizedLevenshtein lev = new NormalizedLevenshtein();

        foreach (var str in strings)
        {
            double similarity = lev.Similarity(target, str);
            Console.WriteLine($"String: {str}, Similarity: {similarity}");

            if (similarity > highestSimilarity)
            {
                highestSimilarity = similarity;
                bestMatch = str;
            }
        }

        return (bestMatch, highestSimilarity);
    }

    public static ObservableCollection<NumberWithScore> GetRankedPotentialNumbers(string input, int estimatedDigits = 6)
    {
        try
        {
            string pattern = @"(?<=\n)(\d+(\.\d+)?)([a-zA-Z]{0,3})?(?=\n)";

            //Console.WriteLine(input);

            MatchCollection matches = Regex.Matches(input, pattern);
            List<string> matchList = matches.Cast<Match>().Select(match => match.Value).ToList();

            Dictionary<string, NumberWithScore> uniqueResults = new Dictionary<string, NumberWithScore>();
            ObservableCollection<NumberWithScore> obsList = new ObservableCollection<NumberWithScore>();

            var res = FindBestMatch("0002723.3", matchList);

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"BestMatch: {res.bestMatch} {res.highestSimilarity}");
            obsList.Add(new NumberWithScore() { NumberSequence = res.bestMatch, Score = res.highestSimilarity });
            Console.WriteLine("-------------------------------------------------");

            //foreach (Match match in matches)
            //{


            //    int points = 0;
            //    var number = match.Groups[1].Value;
            //    var suffix = match.Groups[3].Value;

            //    // Punkte für Länge der Zahl ohne Dezimalstellen
            //    var numberWithoutDecimal = number.Split('.')[0];
            //    int length = numberWithoutDecimal.Length;
            //    if (length >= 6 && length <= 8)
            //    {
            //        points += 150;
            //    }

            //    if (estimatedDigits == length)
            //    {
            //        points += 200;
            //    }

            //    // Punkte für kwh
            //    if (suffix.ToLower() == "kwh")
            //    {
            //        points += 100;
            //    }

            //    // Punkte für Dezimaltrenner
            //    if (number.Contains("."))
            //    {
            //        points += 50;
            //    }

            //    // Punkte für 6-7 Ziffern vor dem Dezimaltrenner und nur eine danach
            //    if (length >= 6 && length <= 7 && number.Split('.').Length > 1 && number.Split('.')[1].Length == 1)
            //    {
            //        points += 200;
            //    }

            //    // Punkte für andere Suffixe
            //    if (!string.IsNullOrEmpty(suffix) && suffix.Length <= 3 && suffix.ToLower() != "kwh")
            //    {
            //        points += 20;
            //    }

            //    // Punkte für Gesamtlänge
            //    if (number.Length == 6)
            //    {
            //        points += 20;
            //    }
            //    else if (number.Length == 5)
            //    {
            //        points += 10;
            //    }

            //    var numberWithScore = new NumberWithScore
            //    {
            //        NumberSequence = number,
            //        Score = points
            //    };

            //    if (points > 0 && !uniqueResults.ContainsKey(numberWithScore.NumberSequence))
            //    {
            //        uniqueResults.Add(numberWithScore.NumberSequence, numberWithScore);
            //        obsList.Add(numberWithScore);
            //    }
            //}

            //// Liste nach Punktzahl sortieren und zurückgeben (neue liste wird erstellt)
            //obsList = new ObservableCollection<NumberWithScore>(obsList.OrderByDescending(c => c.Score));

            //Console.WriteLine("-------------------------------------------------");
            //foreach (var el in obsList)
            //{
            //    Console.WriteLine($"{el.NumberSequence} - ({el.Score})");
            //}
            //Console.WriteLine("-------------------------------------------------");

            return obsList;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
