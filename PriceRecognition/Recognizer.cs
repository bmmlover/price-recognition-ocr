using Puma.Net;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PriceRecognition
{
    class Recognizer
    {
        string imageText;
        Bitmap picture;
        OtsuThreshold otsu;
        public Recognizer(string[] arg)
        {
            picture = new Bitmap(arg[0]);
            otsu = new OtsuThreshold();
        }
        public void Preprocess()
        {
            Bitmap temp = (Bitmap)picture.Clone();
            otsu.Convert2GrayScale(temp);
            int threshold = otsu.getOtsuThreshold((Bitmap)temp);
            otsu.thresholdImage(temp, threshold);
            picture = (Bitmap)temp.Clone();
        }
        public void ImageToText()
        {
            string result = "";
            //picture.Save("picture.png");
            PumaPage image = new PumaPage(picture);
            using (image)
            {
                image.FileFormat = PumaFileFormat.TxtAscii;
                image.AutoRotateImage = false;
                image.EnableSpeller = false;
                image.Language = PumaLanguage.RussianEnglish;
                image.PreserveLineBreaks = false;

                try
                {
                    result = image.RecognizeToString();
                }
                catch (Exception)
                {
                    Console.WriteLine("Recognition failed");
                }
                finally
                {
                    image.Dispose();
                }
            }

            picture.Dispose();
            result = result.Replace(" ", "");
            imageText = String.Copy(result);
        }
        public List<string> FindPrices()
        {
            List<string> prices = new List<string>();

            string[] patterns = new string[2];
            patterns[0] = @"(Э|Е|E|f|\$|EUR|€|£)[0-9]{1,}(.?|,?)[0-9]{1,}";
            patterns[1] = @"[0-9]{1,}(.?|,?)[0-9]{1,}(P|p|Р|р|\$|€|£)";

            foreach (string pattern in patterns)
            {
                try
                {
                    foreach (Match m in Regex.Matches(imageText, pattern))
                    {
                        string val = String.Copy(m.Value);
                        if (val.Contains("Э"))
                        {
                            val = val.Replace("Э", "e");
                        }
                        if (val.Contains("EUR"))
                        {
                            val = val.Replace("EUR", "e");
                        }
                        if (val.Contains("eur"))
                        {
                            val = val.Replace("eur", "e");
                        }
                        if (val.Contains("Е")) //russian
                        {
                            val = val.Replace("Е", "f");
                        }
                        if (val.Contains("E")) //english
                        {
                            val = val.Replace("E", "f");
                        }
                        prices.Add(val);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return prices;
        }
    }
}
