using SyndykApp.Model;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using SyndykApp.SQL;
using SyndykApp.Model.WebPageQuerySelectors;
using Microsoft.Playwright;

namespace SyndykApp.Services
{
    public static class SimilarityService
    {
        private static bool AreDescriptionsSimilar(string s, string t)
        {
            return GetDamerauLevenshteinDistance(s, t) < 100;
        }

        private static Advertisement CheckIfAdvertisementExists(List<Advertisement> advertisements, Advertisement ad)
        {
            return advertisements.FirstOrDefault(x => (x.Title == ad.Title && x.Price == ad.Price) || x.Link == ad.Link || AreDescriptionsSimilar(x.Description, ad.Description));
        }

        private static string GetFirstLetterOfEveryWord(string text)
        {
            return String.Join("",Regex.Matches(text, @"\b(\w{1})")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray());
                
        }

        public static string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        private static int GetDamerauLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(s, "String Cannot Be Null Or Empty");
            }

            if (string.IsNullOrEmpty(t))
            {
                throw new ArgumentNullException(t, "String Cannot Be Null Or Empty");
            }

            int n = s.Length; // length of s
            int m = t.Length; // length of t

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            int[] p = new int[n + 1]; //'previous' cost array, horizontally
            int[] d = new int[n + 1]; // cost array, horizontally

            // indexes into strings s and t
            int i; // iterates through s
            int j; // iterates through t

            for (i = 0; i <= n; i++)
            {
                p[i] = i;
            }

            for (j = 1; j <= m; j++)
            {
                char tJ = t[j - 1]; // jth character of t
                d[0] = j;

                for (i = 1; i <= n; i++)
                {
                    int cost = s[i - 1] == tJ ? 0 : 1; // cost
                                                       // minimum of cell to the left+1, to the top+1, diagonally left and up +cost                
                    d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
                }

                // copy current distance counts to 'previous row' distance counts
                int[] dPlaceholder = p; //placeholder to assist in swapping p and d
                p = d;
                d = dPlaceholder;
            }

            // our last action in the above loop was to switch d and p, so p now 
            // actually has the most recent cost counts

            return p[n];
        }


        private static void SendEmail(string content, string title)
        {
            try
            {
                string smtpAddress = "smtp.gmail.com";
                int portNumber = 587;
                bool enableSSL = true;
                string emailFromAddress = "testapkaotomoto@gmail.com"; //Sender Email Address  
                string password = "ghvg qsnq ygba xghq"; //Sender Password  
                string emailToAddress = "mieszkanie2021wroclaw@gmail.com"; //Receiver Email Address  

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Subject = title;
                    mail.Body = content;
                    mail.IsBodyHtml = false;
                    //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static async Task ProcessAds(IReadOnlyList<IElementHandle> advertsElements, IBaseQuerySelector advertObject, List<Advertisement> sourceAdvertisements, decimal maxPrice = 0)
        {
            foreach (var advertisement in advertsElements)
            {
                try
                {
                    var link = await advertObject.GetLink(advertisement);

                    var title = await advertObject.GetTitle(advertisement, advertObject.TitleSelector);

                    if(title != null && title.ToLower().Trim().Contains("udział"))
                    {
                        continue;
                    }

                    var description = await advertObject.GetDescription(link, advertObject.GetDescriptionSelector(link)) ?? String.Empty;

                    if (Regex.IsMatch(description, @"\s+(\b[1-9]|[1-9][0-9])\/([1-9]|[1-9][0-9])\b\s+") && description.ToLower().Trim().Contains("udział"))
                    {
                        continue;
                    }

                    var price = await advertObject.GetPrice(advertisement);

                    if(price > maxPrice)
                    {
                        continue;
                    }

                    Console.WriteLine($"{advertObject.WebPage} - Tytuł: {title}, Link: {link}, Cena: {price} zł");

                    var ad = new Advertisement
                    {
                        Link = link,
                        Price = price,
                        Title = title,
                        Description = GetFirstLetterOfEveryWord(description)
                    };

                    var similarAdvertisement = CheckIfAdvertisementExists(sourceAdvertisements, ad);

                    if (similarAdvertisement != null)
                    {
                        if(similarAdvertisement.Price != ad.Price)
                        {
                            var content = String.Format("${3} \r\n Link: ${0} \r\n Poprzednia cena: ${1} \r\n Aktualna cena: ${2}", ad.Link, similarAdvertisement.Price, ad.Price, ad.Title);
                            //SendEmail(content, "Cena nieruchomości uległa zmianie!");
                        }
                    }

                    else
                    {
                        using (var dbContext = new DatabaseContext())
                        {
                            //SendEmail(ad.Link, ad.Title);
                            dbContext.InsertAdvertisement(ad);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var link = await advertObject.GetLink(advertisement);
                    Console.WriteLine("WYSTĄPIŁ BŁĄD !!!", link);
                }
                
            }
            await Task.Delay(10000); // aby nie dostać bloka od strony
        }
    }
}
