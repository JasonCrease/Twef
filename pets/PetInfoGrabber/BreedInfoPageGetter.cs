using System;
using System.Net;

namespace PetInfoGrabber
{
    internal class BreedInfoPageGetter
    {
        public static WebClient client = new WebClient();

        internal static string GetPage(string breed)
        {
            string downloadString = "";

            try {
                string webPage = "http://www.dogbreedslist.info/all-dog-breeds/" + breed + ".html";
                downloadString = client.DownloadString(webPage);
            }
            catch(WebException)
            {
                throw new BreedNotFoundException();
            }

            return downloadString;
        }
    }
}