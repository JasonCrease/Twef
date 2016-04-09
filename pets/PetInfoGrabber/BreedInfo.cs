using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PetInfoGrabber
{
    internal class BreedInfo
    {
        public int LifeSpan { get; private set; }
        public int Price { get; private set; }
        public int Weight { get; private set; }
        public int Height { get; private set; }
        public int GoodWithKids { get; private set; }
        public int Trainability { get; private set; }
        public int Intelligence { get; private set; }

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
                else
                    binf.LifeSpan = 14;

                if (breedPage.Contains("USD"))
                {
                    Match priceBit = Regex.Match(breedPage, "\\$([0-9]+)[\\s]*USD", RegexOptions.Singleline);
                    binf.Price = int.Parse(priceBit.Groups[1].Value.Trim());
                }
                else
                    binf.Price = 950;

                if (breedPage.Contains("Height"))
                {
                    Match heightBit = Regex.Match(breedPage, "[-|to|~][\\s]*([0-9]+)\\.?[0-9]?[\\s]*[cm|centimeters]", RegexOptions.Singleline);
                    Match heightBitImperial = Regex.Match(breedPage, "([0-9]+)\\.?[0-9]?[\\s]*(in|inches)", RegexOptions.Singleline);

                    if (heightBit.Groups.Count > 1)
                        binf.Height = int.Parse(heightBit.Groups[1].Value.Trim());
                    else
                        binf.Height = (int)(int.Parse(heightBitImperial.Groups[1].Value.Trim()) * 2.5);
                }
                else
                    binf.Height = 25;

                if (breedPage.Contains("Weight"))
                {
                    Match weightBit       = Regex.Match(breedPage, "[-|to|under][\\s]*([0-9]+)\\.?[0-9]?[\\s]*(lb|pounds)", RegexOptions.Singleline);
                    Match weightBitMetric = Regex.Match(breedPage, "-[\\s]*([0-9]+)\\.?[0-9]?[\\s]*(Kg|kg|kilograms|kg\\))<", RegexOptions.Singleline);

                    if(weightBit.Groups.Count > 1)
                        binf.Weight = int.Parse(weightBit.Groups[1].Value.Trim());
                    else
                        binf.Weight = (int)(int.Parse(weightBitMetric.Groups[1].Value.Trim()) * 2.2);
                }
                else
                    binf.Weight = 50;

                Match goodWithKidsMatch = Regex.Match(breedPage, "Good with Kids(.*)Cat Friendly", RegexOptions.Singleline);
                string goodWithKidsStr = goodWithKidsMatch.Groups[1].Value.Trim();
                binf.GoodWithKids = Regex.Matches(goodWithKidsStr, "star01").Count;

                if (binf.GoodWithKids == 0 || binf.GoodWithKids > 5) binf.GoodWithKids = 3;

                Match trainabilityMatch = Regex.Match(breedPage, "Trainability(.*)Shedding", RegexOptions.Singleline);
                string trainabilityStr = trainabilityMatch.Groups[1].Value.Trim();
                binf.Trainability = Regex.Matches(trainabilityStr, "star01").Count;
                if (binf.Trainability == 0 || binf.Trainability > 5) binf.Trainability = 3;

                Match intelligenceMatch = Regex.Match(breedPage, "Intelligence(.*)Grooming", RegexOptions.Singleline);
                string intelligenceStr = intelligenceMatch.Groups[1].Value.Trim();
                binf.Intelligence = Regex.Matches(intelligenceStr, "star01").Count;
                if (binf.Intelligence == 0 || binf.Intelligence > 5) binf.Intelligence = 3;
            }
            catch (BreedNotFoundException)
            {
            }

            breedCache[breed] = binf;
            return binf;
        }
    }
}