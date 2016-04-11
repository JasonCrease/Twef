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
                sw.WriteLine("Breed,Weight,Height,Price,Lifespan,GoodWithKids,Trainability,Intelligence");

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

                    string line = breed + ",";
                    string weight = "";
                    string height = "";
                    string price = "";
                    string lifespan = "";
                    string goodWithKids = "";
                    string trainability = "";
                    string intelligence = "";

                    if (breedInfo1 != null && breedInfo2 == null)
                    {
                        weight = breedInfo1.Weight.ToString();
                        height = breedInfo1.Height.ToString();
                        price = breedInfo1.Price.ToString();
                        lifespan = breedInfo1.LifeSpan.ToString();
                        goodWithKids = breedInfo1.GoodWithKids.ToString();
                        trainability = breedInfo1.Trainability.ToString();
                        intelligence = breedInfo1.Intelligence.ToString();
                    }
                    if (breedInfo1 == null && breedInfo2 != null)
                    {
                        weight = breedInfo2.Weight.ToString();
                        height = breedInfo2.Height.ToString();
                        price = breedInfo2.Price.ToString();
                        lifespan = breedInfo2.LifeSpan.ToString();
                        goodWithKids = breedInfo2.GoodWithKids.ToString();
                        trainability = breedInfo2.Trainability.ToString();
                        intelligence = breedInfo2.Intelligence.ToString();
                    }
                    if (breedInfo1 != null && breedInfo2 != null)
                    {
                        weight = ((breedInfo1.Weight + breedInfo2.Weight) / 2).ToString();
                        height = ((breedInfo1.Height + breedInfo2.Height) / 2).ToString();
                        price = ((breedInfo1.Price + breedInfo2.Price) / 2).ToString();
                        lifespan = ((breedInfo1.LifeSpan + breedInfo2.LifeSpan) / 2).ToString();
                        goodWithKids = ((breedInfo1.GoodWithKids + breedInfo2.GoodWithKids) / 2).ToString();
                        trainability = ((breedInfo1.Trainability + breedInfo2.Trainability) / 2).ToString();
                        intelligence = ((breedInfo1.Intelligence + breedInfo2.Intelligence) / 2).ToString();
                    }

                    line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", breed, weight, height, price, lifespan, goodWithKids, trainability, intelligence);

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
