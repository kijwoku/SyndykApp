using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Model.WebPageQuerySelectors;
using SyndykApp.SQL;

namespace SyndykApp.Services
{
    public static class Gratka
    {
        public static async Task Run(decimal maxPrice = 0)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            });

            var advertObject = new GratkaQuerySelector();

            var advertsList = new List<Advertisement>();

            using (var dbContext = new DatabaseContext())
            {
                advertsList = dbContext.GetAdvertisements().ToList();
            }

            foreach (var pageNo in Enumerable.Range(1, 15))
            {
                var customHeaders = new Dictionary<string, string>
                {
                    ["User-Agent"] = "Mozilla/5.1 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0"
                };

                // Set custom headers for the page
                var url = $"https://gratka.pl/nieruchomosci/q/syndyk?page={pageNo}&sort=newest";
                var page = await browser.NewPageAsync();

                await page.SetExtraHTTPHeadersAsync(customHeaders);

                await page.GotoAsync(url);

                Thread.Sleep(2000);

                var anyAdverts = await advertObject.CheckIfAnyAdvertsOnPage(page, pageNo);

                if (anyAdverts)
                {
                    var adverts = await advertObject.GetAdverts(page, "div.listing__teaserWrapper");

                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine(String.Format("GRATKA - strona: {0} Liczba Ogłoszeń: {1}", pageNo, adverts.Count));
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

                    await SimilarityService.ProcessAds(adverts, advertObject, advertsList, maxPrice);
                }
                else
                {
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine(String.Format("GRATKA - strona: {0} BRAK OGŁOSZEŃ", pageNo));
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    break;
                }
                await page.CloseAsync();
            }
            playwright.Dispose();
            await browser.DisposeAsync();
        }
    }
}
