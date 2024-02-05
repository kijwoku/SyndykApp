using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Model.WebPageQuerySelectors;
using SyndykApp.SQL;

namespace SyndykApp.Services
{
    public static class Morizon
    {
        public static async Task Run(decimal maxPrice = 0)
        {
            var typyNieruchomosci = new[] { "mieszkania", "domy", "komercyjne", "dzialki", "garaze"};

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            });

            var advertObject = new MorizonQuerySelector();

            var advertsList = new List<Advertisement>();

            using (var dbContext = new DatabaseContext())
            {
                advertsList = dbContext.GetAdvertisements().ToList();
            }

            foreach (var typ in typyNieruchomosci)
            {
                foreach(var pageNo in Enumerable.Range(1, 15))
                {
                    var customHeaders = new Dictionary<string, string>
                    {
                        ["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                    };

                    var url = $"https://www.morizon.pl/{typ}/najnowsze/?ps%5Bdescription%5D=syndyk&page={pageNo}";
                    var page = await browser.NewPageAsync();

                    await page.SetExtraHTTPHeadersAsync(customHeaders);

                    await page.GotoAsync(url);

                    var anyAdverts = await advertObject.CheckIfAnyAdvertsOnPage(page, pageNo);

                    if (anyAdverts)
                    {
                        var adverts = await advertObject.GetAdverts(page, "div.card--bottom-margin div.card__outer");

                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine(String.Format("MORIZON - strona: {0} Liczba Ogłoszeń: {1}", pageNo, adverts.Count));
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

                        await SimilarityService.ProcessAds(adverts, advertObject, advertsList, maxPrice);
                    }
                    else
                    {
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine(String.Format("MORIZON - strona: {0} BRAK OGŁOSZEŃ", pageNo));
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        break;
                    }

                    await page.CloseAsync();
                }
            }
            playwright.Dispose();
            await browser.DisposeAsync();
        }
    }
}
