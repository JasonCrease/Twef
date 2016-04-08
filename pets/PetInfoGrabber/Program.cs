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
            string[] allLines = File.ReadAllLines(".\\..\\..\\breedList.csv");
            List<string> breedList = new List<string>();

            foreach (string line in allLines)
            {
                if (line == "\"x\"")
                    continue;

               string breedname = line.Replace("\\", "").Replace("\"", "").Replace(" Mix", "").Trim();
                breedname = breedname.Replace(" ", "-");
                breedList.Add(breedname);
            }

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
                    breedInfo1 = BreedInfo.GetInfo(breed1);
                    breedInfo2 = BreedInfo.GetInfo(breed2);

                    if (breedInfo1 == null)
                        Console.WriteLine("Breed {0} not found", breed1);
                    if (breedInfo2 == null)
                        Console.WriteLine("Breed {0} not found", breed2);
                    if (breedInfo1 != null)
                        Console.WriteLine("Breed {0} found", breed1);
                    if (breedInfo2 != null)
                        Console.WriteLine("Breed {0} found", breed2);
                }
                else
                {
                    mix = false;
                    breedInfo1 = BreedInfo.GetInfo(breed);

                    if (breedInfo1 != null)
                        Console.WriteLine("Breed {0} found", breed);
                    if (breedInfo1 == null)
                        Console.WriteLine("Breed {0} not found", breed);
                }

            }
        }
    }
}
