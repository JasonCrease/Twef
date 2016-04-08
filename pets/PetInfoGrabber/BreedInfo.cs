using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetInfoGrabber
{
    internal class BreedInfo
    {
        public int LifeSpan { get; private set; }
        public int Price { get; private set; }
        public double Weight { get; private set; }
        public double Height { get; private set; }

        public static Dictionary<string, BreedInfo> breedCache = new Dictionary<string, BreedInfo>();

        internal static BreedInfo GetInfo(string breed)
        {
            BreedInfo binf = null;

            try
            {
                if (breedCache.ContainsKey(breed))
                    return breedCache[breed];

                string breedPage = BreedInfoPageGetter.GetPage(breed);
                binf = new BreedInfo();

                if (breedPage.Contains("years"))
                {
                    Match lifeSpanBit = Regex.Match(breedPage, "[-|to][\\s]*([0-9]+)\\.?[0-9]?[\\s]*years", RegexOptions.Singleline);
                    binf.LifeSpan = int.Parse(lifeSpanBit.Groups[1].Value.Trim());
                }
                if (breedPage.Contains("USD"))
                {
                    Match priceBit = Regex.Match(breedPage, "\\$([0-9]+)[\\s]*USD", RegexOptions.Singleline);
                    binf.Price = int.Parse(priceBit.Groups[1].Value.Trim());
                }
                if (breedPage.Contains("Height"))
                {
                    Match heightBit = Regex.Match(breedPage, "[-|to][\\s]*([0-9]+)\\.?[0-9]?[\\s]*[cm|centimeters]", RegexOptions.Singleline);
                    Match heightBitImperial = Regex.Match(breedPage, "([0-9]+)\\.?[0-9]?[\\s]*(in|inches)", RegexOptions.Singleline);

                    if (heightBit.Groups.Count > 1)
                        binf.Height = int.Parse(heightBit.Groups[1].Value.Trim());
                    else
                        binf.Height = int.Parse(heightBitImperial.Groups[1].Value.Trim()) * 2.5;
                }
                if (breedPage.Contains("Weight"))
                {
                    Match weightBit       = Regex.Match(breedPage, "[-|to|under][\\s]*([0-9]+)\\.?[0-9]?[\\s]*[lb|pounds]", RegexOptions.Singleline);
                    Match weightBitMetric = Regex.Match(breedPage, "-[\\s]*([0-9]+)\\.?[0-9]?[\\s]*[kg|kilograms]", RegexOptions.Singleline);

                    if(weightBit.Groups.Count > 1)
                        binf.Weight = int.Parse(weightBit.Groups[1].Value.Trim());
                    else
                        binf.Weight = int.Parse(weightBitMetric.Groups[1].Value.Trim()) * 2.2;
                }
            }
            catch(BreedNotFoundException)
            {
            }

            breedCache[breed] = binf;
            return binf;
        }
    }
}