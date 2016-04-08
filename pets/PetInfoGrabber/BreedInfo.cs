using System;
using System.Collections.Generic;

namespace PetInfoGrabber
{
    internal class BreedInfo
    {
        public double Weight { get; private set; }
        public double Height { get; private set; }

        public static Dictionary<string, BreedInfo> breedCache = new Dictionary<string, BreedInfo>();

        internal static BreedInfo GetInfo(string breed)
        {
            try
            {
                string breedPage = BreedInfoPageGetter.GetPage(breed);
                BreedInfo binf = new BreedInfo();
                binf.Weight = 6;
                binf.Height = 3;
                return binf;
            }
            catch(BreedNotFoundException)
            {
                return null;
            }
        }
    }
}