using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Model.WebPageQuerySelectors;

namespace SyndykApp.Services
{
    public static class Olx
    {
        public static async Task Run()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            });

            var advertObject = new OlxQuerySelector();

            var ads = new List<Advertisement>();

            foreach (var pageNo in Enumerable.Range(1, 15))
            {
                var customHeaders = new Dictionary<string, string>
                {
                    ["User-Agent"] = "Mozilla/5.1 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0"
                };

                // Set custom headers for the page
                var url = $"https://www.olx.pl/nieruchomosci/q-syndyk/?page=${pageNo}&search%5Border%5D=created_at%3Adesc";
                var page = await browser.NewPageAsync();

                await page.SetExtraHTTPHeadersAsync(customHeaders);

                await page.GotoAsync(url);

                var adverts = await advertObject.GetAdverts(page);

                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine(adverts.Count);
                Console.WriteLine("-----------------------------------------------------------");

                if (adverts.Count == 0)
                {
                    break;
                }

                foreach (var advertisement in adverts)
                {
                    var title = await advertObject.GetTitle(advertisement);

                    var link = await advertObject.GetLink(advertisement);

                    var price = await advertObject.GetPrice(advertisement);

                    var description = await advertObject.GetDescription(page,link);

                    //var offertID = await advertObject.GetID(page, link);

                    Console.WriteLine($"OLX - Tytuł: {title}, ID: {1}, Cena: {price} zł, Strona: {pageNo}");

                    //ads.Add(new Advertisement()
                    //{
                    //    Title = tytul,
                    //    HtmlContent = null,
                    //    Link = link,
                    //    Price = amount,
                    //    OffertID = ""
                    //});
                }
            }
            playwright.Dispose();
            await browser.DisposeAsync();
        }
    }
}
