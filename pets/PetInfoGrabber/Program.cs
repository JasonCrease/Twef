using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace PetInfoGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            string notFoundBreedsFile = ".\\..\\..\\notFoundBreedsFile.txt";

            string[] allLines = File.ReadAllLines(".\\..\\..\\..\\dogbreedList.csv");
            List<string> breedList = new List<string>();

            foreach (string line in allLines)
            {
                if (line == "\"x\"")
                    continue;

                if (!breedList.Contains(line))
                    breedList.Add(line);
            }

            using (StreamWriter sw = new StreamWriter(new FileStream(".\\..\\..\\BreedListOut.csv", FileMode.Create)))
            {
                sw.WriteLine("Breed,Weight,Height,Price,Lifespan,GoodWithKids,Trainability," +
                             "Intelligence,Adaptability,CatFriendly,DogFriendly,Shedding,Hypoallergenic,Watchdog");

                foreach (string breed in breedList)
                {
                    bool mix = false;
                    BreedInfo breedInfo1 = null;
                    BreedInfo breedInfo2 = null;

                    if (breed.Contains("/"))
                    {
                        mix = true;
                        string breed1 = breed.Split('/')[0];
                        string breed2 = breed.Split('/')[1];
                        breedInfo1 = BreedInfo.GetInfo(NicefyName(breed1));
                        breedInfo2 = BreedInfo.GetInfo(NicefyName(breed2));

                        if (breedInfo1 == null)
                        {
                            File.AppendAllText(notFoundBreedsFile, breed1 + "\r\n");
                            Console.WriteLine("Breed {0} not found", breed1);
                        }
                        if (breedInfo2 == null)
                        {
                            File.AppendAllText(notFoundBreedsFile, breed2 + "\r\n");
                            Console.WriteLine("Breed {0} not found", breed2);
                        }
                    }
                    else
                    {
                        mix = false;
                        breedInfo1 = BreedInfo.GetInfo(NicefyName(breed));

                        if (breedInfo1 == null)
                        {
                            File.AppendAllText(notFoundBreedsFile, breed + "\r\n");
                            Console.WriteLine("Breed {0} not found", breed);
                        }
                    }

                    int weight = 50;
                    int height = 25;
                    int price = 800;
                    int lifespan = 14;

                    int goodWithKids = 3;
                    int trainability = 3;
                    int intelligence = 4;
                    int adaptability = 4;
                    int catFriendly = 3;
                    int dogFriendly = 3;
                    int shedding = 3;
                    int watchdog = 3;
                    bool hypoallergenic = false;

                    if (breedInfo1 != null && breedInfo2 == null)
                    {
                        weight = breedInfo1.Weight;
                        height = breedInfo1.Height;
                        price = breedInfo1.Price;
                        lifespan = breedInfo1.LifeSpan;
                        goodWithKids = breedInfo1.GoodWithKids;
                        trainability = breedInfo1.Trainability;
                        watchdog = breedInfo1.Watchdog;
                        intelligence = breedInfo1.Intelligence;
                        adaptability = breedInfo1.Adaptability;
                        catFriendly = breedInfo1.CatFriendly;
                        dogFriendly = breedInfo1.DogFriendly;
                        shedding = breedInfo1.Shedding;
                        hypoallergenic = breedInfo1.Hypoallergenic;
                    }
                    if (breedInfo1 == null && breedInfo2 != null)
                    {
                        weight = breedInfo2.Weight;
                        height = breedInfo2.Height;
                        price = breedInfo2.Price;
                        lifespan = breedInfo2.LifeSpan;
                        goodWithKids = breedInfo2.GoodWithKids;
                        trainability = breedInfo2.Trainability;
                        watchdog = breedInfo2.Watchdog;
                        intelligence = breedInfo2.Intelligence;
                        adaptability = breedInfo2.Adaptability;
                        catFriendly = breedInfo2.CatFriendly;
                        dogFriendly = breedInfo2.DogFriendly;
                        shedding = breedInfo2.Shedding;
                        hypoallergenic = breedInfo2.Hypoallergenic;
                    }
                    if (breedInfo1 != null && breedInfo2 != null)
                    {
                        weight = ((breedInfo1.Weight + breedInfo2.Weight) / 2);
                        height = ((breedInfo1.Height + breedInfo2.Height) / 2);
                        price = ((breedInfo1.Price + breedInfo2.Price) / 2);
                        lifespan = ((breedInfo1.LifeSpan + breedInfo2.LifeSpan) / 2);
                        goodWithKids = ((breedInfo1.GoodWithKids + breedInfo2.GoodWithKids) / 2);
                        trainability = ((breedInfo1.Trainability + breedInfo2.Trainability) / 2);
                        watchdog = ((breedInfo1.Watchdog + breedInfo2.Watchdog) / 2);
                        intelligence = ((breedInfo1.Intelligence + breedInfo2.Intelligence) / 2);
                        adaptability = ((breedInfo1.Adaptability + breedInfo2.Adaptability) / 2);
                        catFriendly = ((breedInfo1.CatFriendly + breedInfo2.CatFriendly) / 2);
                        dogFriendly = ((breedInfo1.DogFriendly + breedInfo2.DogFriendly) / 2);
                        shedding = ((breedInfo1.Shedding + breedInfo2.Shedding) / 2);
                        hypoallergenic = breedInfo1.Hypoallergenic && breedInfo2.Hypoallergenic;
                    }

                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", breed, weight, height, price, lifespan, goodWithKids,
                        trainability, intelligence, adaptability, catFriendly, dogFriendly, shedding, hypoallergenic, watchdog);

                    sw.WriteLine(line);
                }

                sw.Flush();
            }
        }

        private static string NicefyName(string breed)
        {
            return breed.Replace("\\", "").Replace("\"", "").Trim().Replace(" ", "-").Replace("-Mix", "");
        }
    }
}
