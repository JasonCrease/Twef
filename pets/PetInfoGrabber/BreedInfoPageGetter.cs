﻿using System;
using System.IO;
using System.Net;

namespace PetInfoGrabber
{
    internal class BreedInfoPageGetter
    {
        public static WebClient client = new WebClient();

        internal static string GetPage(string breed)
        {
            breed = RegularizeBreedName(breed);

            string downloadString = "";
            string fileName = ".\\..\\..\\BreedPages\\" + breed + ".txt";

            try
            {
                if (File.Exists(fileName))
                    return File.ReadAllText(fileName);

                string webPage = "http://www.dogbreedslist.info/all-dog-breeds/" + breed + ".html";
                downloadString = client.DownloadString(webPage);
                downloadString = downloadString.Replace("&nbsp;", " ");
                downloadString = downloadString.Replace("&ndash;", "-");
                File.WriteAllText(fileName, downloadString);
            }
            catch (WebException)
            {
                throw new BreedNotFoundException();
            }

            return downloadString;
        }

        static string[,] replacements = new string[,] {
                { "Pit-Bull", "American-Pit-Bull-Terrier" },
                { "Chihuahua-Shorthair", "Chihuahua" },
                { "German-Shepherd", "german-shepherd-dog" },
                { "Miniature-Poodle", "Poodle" },
                { "Standard-Poodle", "Poodle" },
                { "Catahoula", "Catahoula-Leopard-Dog" },
                { "Chihuahua-Longhair", "Chihuahua" },
                { "Staffordshire", "Staffordshire-Bull-Terrier" },
                { "Pointer", "English-Pointer" },
                { "Black-Mouth-Cur", "Blackmouth-Cur" },
                { "Toy-Poodle", "Poodle" },
                { "Miniature-Poodle", "Poodle" },
                { "Queensland-Heeler", "Australian-Cattle-Dog" },
                { "Maltese", "Maltese-Dog" },
                { "Anatol-Shepherd", "Anatolian-Shepherd-Dog" },
                { "Doberman-Pinsch", "Doberman-Pinscher" },
                { "Cocker-Spaniel", "American-Cocker-Spaniel" },
                { "Collie-Smooth", "Smooth-Collie" },
                { "Collie-Rough", "Rough-Collie" },
                { "Mastiff", "English-Mastiff" },
                { "Dachshund-Longhair", "Dachshund" },
                { "Dachshund-Wirehair", "Dachshund" },
                { "Chinese-Sharpei", "Shar-Pei" },
                { "Rhod-Ridgeback", "Rhodesian-Ridgeback" },
                { "Akita", "American-Akita" },
                { "English-Bulldog", "Olde-English-Bulldogge" },
                { "Papillon", "Papillon-Dog" },
                { "American-Eskimo", "American-Eskimo-Dog" },
                { "West-Highland", "West-Highland-White-Terrier" },
                { "Harrier", "Harrier-dog" },
                { "Cavalier-Span", "Cavalier-King-Charles-Spaniel" },
                { "Wire-Hair-Fox-Terrier", "Wire-Fox-Terrier" },
                { "St.-Bernard-Rough-Coat", "St-Bernard" },
                { "Tan-Hound", "Austrian-Black-and-Tan-Hound" },
                { "Flat-Coat-Retriever", "Flat-Coated-Retriever" },
                { "Brussels-Griffon", "Bruss-Griffon" },
            };

        private static string RegularizeBreedName(string breed)
        {
            for (int i = 0; i < replacements.Length / 2; i++)
            {
                if (breed == replacements[i, 0]) return replacements[i, 1];
            }

            return breed;
        }
    }
}