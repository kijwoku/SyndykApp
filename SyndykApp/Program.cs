using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Services;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
class Program
{
    public static async Task Main()
    {
        //Otodom.Run();
        await Olx.Run();

        string str1 = @"a a s  w e q w e q a a s  w e 1 q w e p";
        string str2 = @"a a s  w e 1 q w e p a a s  w e 1 q w e p";

        var similarity2 = SimilarityService.AreDescriptionsSimilar(str1, str2);

        Console.WriteLine($"Podobieństwo: {similarity2:P}");
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