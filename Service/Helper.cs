using System.Collections.ObjectModel;
using System.Globalization;
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

    public static async Task<ImageSource> ConvertToThumbnail(byte[] image)
    {
        // TODO: Thumbnail creation depending on internetspeed
        // maximum 3 sec

        // lower resolution for lower upload

        // 2,5sec --> 25mbits upload
        int newWidth = 1600;
        int newHeight = 1200;

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
    /// <param name="matches"></param>
    /// <returns></returns>
    public static (string bestMatch, double highestSimilarity) FindBestMatch(double target, List<string> matches)
    {
        string bestMatch = null;
        double highestSimilarity = 0;

        var targetAsString = target.ToString();

        NormalizedLevenshtein lev = new NormalizedLevenshtein();

        foreach (var match in matches)
        {
            double similarity = lev.Similarity(targetAsString, match);
            Console.WriteLine($"String: {match}, Similarity: {similarity}");

            if (similarity > highestSimilarity)
            {
                highestSimilarity = similarity;
                bestMatch = match;
            }
        }

        return (bestMatch, highestSimilarity);
    }
    static double AdjustDecimalPlace(string target, string result)
    {
        int targetDecimalPlace = target.IndexOf(',');
        int targetLengthBeforeDecimal = (targetDecimalPlace != -1) ? targetDecimalPlace : target.Length;

        // Wenn das Resultat mehr Ziffern hat als das Ziel vor dem Dezimalpunkt,
        // setze den Dezimalpunkt an die entsprechende Stelle im Resultat
        if (result.Length > targetLengthBeforeDecimal)
        {
            result = result.Insert(targetLengthBeforeDecimal, ".");
        }
        else if (result.Length == targetLengthBeforeDecimal)
        {
            // Wenn kein Komma im Ziel gefunden wird und die Länge des Resultats gleich der Länge des Ziels ist,
            // füge .0 am Ende des Ergebnisses hinzu
            result += ".0";
        }
        else
        {
            Console.WriteLine($"Hier sollte man mit 0 auffüllen: {result}");
        }

        double res = 0;
        double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out res);

        return res;

    }

    public static ObservableCollection<NumberWithScore> GetRankedPotentialNumbers(string input, double currentMeterReading)
    {
        try
        {
            ObservableCollection<NumberWithScore> obsList = new ObservableCollection<NumberWithScore>();

            // Anzahl der Ziffern vor dem Dezimalpunkt im currentMeterReading
            int targetDigitsBeforeDecimal = currentMeterReading.ToString().IndexOf(',') != -1 ?
                                            currentMeterReading.ToString().IndexOf(',') :
                                            currentMeterReading.ToString().Length;

            var patterns = new List<string>
            {
                @"(?<=\n)(\d+(\.\d+)?)([a-zA-Z]{0,3})?(?=\n)",
                @"(?<=^|\n)(\d+(\.\d+)?)([a-zA-Z]{0,3})(?=\n|$)",
                @"(?:^|\n)(\d+(\.\d+)?)([a-zA-Z]{0,3})(?=\n|$)",
                @"(?<=\n|^)(\d+(\s*\d+)*)(?=\n|$)",
                @"(?<=\n|^)(\d+-\d+)(?=\n|$)"
            };

            var uniqueMatches = new HashSet<string>();

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(input, pattern);

                foreach (Match match in matches)
                {
                    var matchValue = match.Value
                           .Replace(" ", "") // remove space
                           .Replace("-", "") // remove hyphen
                           .Replace("\n", "") // remove line break
                           .Replace("\r", ""); // remove carriage-return

                    // Überprüfen, ob mehr als ein Punkt im matchValue vorhanden ist
                    int dotCount = matchValue.Count(c => c == '.');

                    // Anzahl der Ziffern im matchValue
                    int matchValueDigits = matchValue.IndexOf('.') != -1 ?
                                           matchValue.IndexOf('.') - 1 :
                                           matchValue.Length;


                    // Überprüfen, ob das Match bereits im HashSet ist und ob die Länge des matchValue innerhalb des zulässigen Bereichs liegt
                    if (!uniqueMatches.Contains(matchValue) && dotCount < 2 && Math.Abs(matchValueDigits - targetDigitsBeforeDecimal) < 2)
                    {
                        uniqueMatches.Add(matchValue);
                        //Console.WriteLine($"Match: {matchValue}");
                    }
                }
            }

            var res = FindBestMatch(currentMeterReading, uniqueMatches.ToList());

            var resAdjusted = AdjustDecimalPlace(currentMeterReading.ToString(), res.bestMatch);

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"BestMatch: {res.bestMatch} {res.highestSimilarity}");
            Console.WriteLine($"BestMatchAdjusted: {resAdjusted}");
            obsList.Add(new NumberWithScore() { Number = resAdjusted, Score = res.highestSimilarity });
            //obsList.Add(new NumberWithScore() { NumberSequence = res.bestMatch, Score = res.highestSimilarity });
            Console.WriteLine("-------------------------------------------------");

            return obsList;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
