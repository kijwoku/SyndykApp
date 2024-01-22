using Microsoft.Playwright;
using SyndykApp.Model;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
class Program
{
    public static async Task Main()
    {
        var typyNieruchomosci = new[] { "mieszkanie", "kawalerka", "dom", "dzialka", "lokal", "garaz" };

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
        });

        var ads = new List<Advertisement>();

        foreach (var typ in typyNieruchomosci)
        {
            var customHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
            };

            // Set custom headers for the page
            var url = $"https://www.otodom.pl/pl/wyniki/sprzedaz/{typ}/cala-polska?limit=36&ownerTypeSingleSelect=ALL&description=syndyk&by=DEFAULT&direction=DESC&viewType=listing&page=1";
            var page = await browser.NewPageAsync();
            
            await page.SetExtraHTTPHeadersAsync(customHeaders);

            await page.GotoAsync(url);

            var asd = await page.ContentAsync();

            var ogloszenia = await page.QuerySelectorAllAsync("li[data-cy='listing-item']");

            foreach (var ogloszenie in ogloszenia)
            {
                var tytulElement = await ogloszenie.QuerySelectorAsync("span[data-cy='listing-item-title']");
                var tytul = await tytulElement.InnerTextAsync();

                var linkElement = await ogloszenie.QuerySelectorAsync("a[data-cy='listing-item-link']");
                var link = await linkElement.GetAttributeAsync("href");
                link = String.Format("https://www.otodom.pl/{0}", link);

                var article = await ogloszenie.QuerySelectorAsync("article");
                var cenaSpan = await article.QuerySelectorAsync("div:nth-last-child(2) span:first-child");
                var cenaText = await cenaSpan.InnerTextAsync();
                var cena  = Int32.Parse(string.Concat(Regex.Matches(cenaText, @"\d").Select(match => match.Value)));

                Console.WriteLine($"Typ: {typ}, Tytuł: {tytul}, Link: {link}, Cena: {cena}");

                ads.Add(new Advertisement()
                {
                    Title = tytul,
                    HtmlContent = null,
                    Link = link,
                    Price = cena,
                    OffertID = ""
                });
            }
        }
        playwright.Dispose();
        await browser.DisposeAsync();


        await Task.Run(async () => {
            await FillHTMLContent(ads);
        });
    }

    private static async Task FillHTMLContent(IEnumerable<Advertisement> ads)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/4.0 (iPad; CPU OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148"
        });


        var page = await context.NewPageAsync();

        var customHeaders = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/4.0 (iPad; CPU OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148"
        };

        await page.SetExtraHTTPHeadersAsync(customHeaders);

        foreach(var ad in ads)
        {
            await page.GotoAsync(ad.Link);

            string pageSource = await page.ContentAsync();

            var ofertaDiv = await page.QuerySelectorAsync("div:text('Nr oferty w Otodom:')");
            var idOferty = await ofertaDiv.InnerTextAsync();
            var id = string.Concat(Regex.Matches(idOferty, @"\d").Select(match => match.Value));

            AddToDB(new Advertisement()
            {
                Title = ad.Title,
                HtmlContent = pageSource,
                Link = ad.Link,
                Price = ad.Price,
                OffertID = id
            });
        }
        
    }

    private static void AddToDB(Advertisement ad)
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SyndykDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

        // Komenda SQL do dodania ogłoszenia
        string sql = "INSERT INTO dbo.Adverts (Title, Link, Price, HTMLContent, OffertID) VALUES (@Title, @Link, @Price, @HTMLContent, @OffertID)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Title", ad.Title);
            command.Parameters.AddWithValue("@Link", ad.Link);
            command.Parameters.AddWithValue("@Price", ad.Price);
            command.Parameters.AddWithValue("@HTMLContent", ad.HtmlContent);
            command.Parameters.AddWithValue("@OffertID", ad.OffertID);

            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}