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
        public int CatFriendly  { get; private set; }
        public int DogFriendly { get; private set; }
        public int Adaptability { get; private set; }
        public int Shedding { get; private set; }
        public bool Hypoallergenic { get; private set; }

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

                binf.GoodWithKids = GetStarsBetween(breedPage, "Good with Kids", "Cat Friendly");
                binf.CatFriendly = GetStarsBetween(breedPage, "Cat Friendly", "Dog Friendly");
                binf.DogFriendly = GetStarsBetween(breedPage, "Dog Friendly", "Trainability");
                binf.Trainability = GetStarsBetween(breedPage, "Trainability", "Shedding");
                binf.Shedding  = GetStarsBetween(breedPage, "Shedding", "Watchdog");
                binf.Intelligence = GetStarsBetween(breedPage, "Intelligence", "Grooming");
                binf.Adaptability = GetStarsBetween(breedPage, "Adaptability", "Hypoallergenic");


                Match hypoMatch = Regex.Match(breedPage, "Hypoallergenic(.*)Overview", RegexOptions.Singleline);
                string hypoStr = hypoMatch.Groups[1].Value.Trim();

                binf.Hypoallergenic = false;
                if (hypoStr.Contains("Yes")) binf.Hypoallergenic = true;
            }
            catch (BreedNotFoundException)
            {
            }

            breedCache[breed] = binf;
            return binf;
        }

        private static int GetStarsBetween(string breedPage, string s1, string s2)
        {
            Match intelligenceMatch = Regex.Match(breedPage, s1+ "(.*)" + s2, RegexOptions.Singleline);
            string intelligenceStr = intelligenceMatch.Groups[1].Value.Trim();
            int x = Regex.Matches(intelligenceStr, "star01").Count;
            if (x == 0 || x > 5) x = 3;

            return x;
        }
    }
}