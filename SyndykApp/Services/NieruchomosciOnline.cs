using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Model.WebPageQuerySelectors;
using SyndykApp.SQL;

namespace SyndykApp.Services
{
    public static class NieruchomosciOnline
    {
        public static async Task Run(decimal maxPrice, IBrowser browser)
        {
            var typyNieruchomosci = new[] { "mieszkanie", "dom", "dzialka", "lokal-uzytkowy", "budynek-uzytkowy" };

            var advertObject = new NieruchomosciOnlineQuerySelector();

            var advertsList = new List<Advertisement>();

            using (var dbContext = new DatabaseContext())
            {
                advertsList = dbContext.GetAdvertisements().ToList();
            }

            foreach (var typ in typyNieruchomosci)
            {
                foreach(var pageNo in Enumerable.Range(1, 2))
                {
                    var customHeaders = new Dictionary<string, string>
                    {
                        ["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                    };

                    var url = $"https://www.nieruchomosci-online.pl/szukaj.html?3,{typ},sprzedaz&o=modDate,desc&q=syndyk&p={pageNo}";
                    var page = await browser.NewPageAsync();

                    await page.SetExtraHTTPHeadersAsync(customHeaders);

                    await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                    await Task.Delay(5000);

                    var anyAdverts = await advertObject.CheckIfAnyAdvertsOnPage(page, pageNo);
                    var asd = await page.ContentAsync();
                    if (anyAdverts)
                    {
                        var adverts = await advertObject.GetAdverts(page, "div.tile-tile");

                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine(String.Format("NieruchomosciOnline - strona: {0} Liczba Ogłoszeń: {1}", pageNo, adverts.Count));
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

                        await SimilarityService.ProcessAds(adverts, advertObject, advertsList, maxPrice);
                    }
                    else
                    {
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine(String.Format("NieruchomosciOnline - strona: {0} BRAK OGŁOSZEŃ", pageNo));
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        break;
                    }
                    await page.CloseAsync();
                }
            }
        }
    }
}
