using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Model.WebPageQuerySelectors;
using SyndykApp.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyndykApp.Services
{
    public static class Otodom
    {
        public static async Task Run()
        {
            var typyNieruchomosci = new[] { "mieszkanie", "kawalerka", "dom", "dzialka", "lokal", "garaz" };

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            });

            var advertObject = new OtodomQuerySelector();

            var ads = new List<Advertisement>();

            foreach (var typ in typyNieruchomosci)
            {
                foreach(var pageNo in Enumerable.Range(1, 15))
                {
                    var customHeaders = new Dictionary<string, string>
                    {
                        ["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                    };

                    // Set custom headers for the page
                    var url = $"https://www.otodom.pl/pl/wyniki/sprzedaz/{typ}/cala-polska?limit=36&ownerTypeSingleSelect=ALL&description=syndyk&by=DEFAULT&direction=DESC&viewType=listing&page={pageNo}";
                    var page = await browser.NewPageAsync();

                    await page.SetExtraHTTPHeadersAsync(customHeaders);

                    await page.GotoAsync(url);

                    var adverts = await advertObject.GetAdverts(page);

                    if(adverts.Count == 0)
                    {
                        break;
                    }
                    foreach (var advertisement in adverts)
                    {
                        var title = await advertObject.GetTitle(advertisement);

                        var link = await advertObject.GetLink(advertisement);

                        var price = await advertObject.GetPrice(advertisement);

                        var description = await advertObject.GetDescription(page, link);

                        Console.WriteLine($"OTODOM - Tytuł: {title}, ID: {1}, Cena: {price} zł, Strona: {pageNo}");

                        var ad = new Advertisement
                        {
                            Link = link,
                            Price = price,
                            Title = title,
                            Description = description
                        };

                        using (var dbContext = new DatabaseContext())
                        {
                            dbContext.InsertAdvertisement(ad);
                        }
                    }
                }
            }
            playwright.Dispose();
            await browser.DisposeAsync();
        }
    }
}
