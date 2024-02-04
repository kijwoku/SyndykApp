using Microsoft.Playwright;
using SyndykApp.Services;
using System.Text.RegularExpressions;


namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class OtodomQuerySelector
    {
        Dictionary<string, string> _customHeaders2 = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.1 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100102 Firefox/42.0"
        };

        public async Task<IReadOnlyList<IElementHandle>> GetAdverts(IPage page)
        {
            return await page.QuerySelectorAllAsync("div[data-cy='listing-item']");
        }
        public async Task<string> GetTitle(IElementHandle element)
        {
            var titleElement = await element.QuerySelectorAsync("span[data-cy='listing-item-title']");
            return await titleElement?.InnerTextAsync() ?? "Brak tytułu";
        }

        public async Task<string> GetLink(IElementHandle element)
        {
            var linkElement = await element.QuerySelectorAsync("a[data-cy='listing-item-link']");
            var link = await linkElement.GetAttributeAsync("href");
            return String.Format("https://www.otodom.pl{0}", link);
        }

        public async Task<decimal> GetPrice(IElementHandle element)
        {
            if(element != null)
            {
                var article = await element.QuerySelectorAsync("article");
                var priceSpan = await article.QuerySelectorAsync("div:nth-last-child(3) span:first-child");
                var fullPrice = await priceSpan.InnerTextAsync();
                int pennies = 0, bills = 0;
                if (fullPrice.Contains(","))
                {
                    pennies = Int32.Parse(string.Concat(Regex.Matches(fullPrice.Substring(fullPrice.IndexOf(',')), @"\d").Select(match => match.Value)));
                    bills = Int32.Parse(string.Concat(Regex.Matches(fullPrice.Substring(0, fullPrice.IndexOf(',')), @"\d").Select(match => match.Value)));
                }
                else
                {
                    bills = Int32.Parse(string.Concat(Regex.Matches(fullPrice, @"\d").Select(match => match.Value)));
                }
                return bills + (decimal)pennies / 100;
            }
            return 0;
        }

        public async Task<string?> GetDescription(IPage page, string url)
        {
            return await Task.Run(async () =>
            {
                if (url != null)
                {
                    using var playwright = await Playwright.CreateAsync();
                    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = true,
                        ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                    });

                    var page = await browser.NewPageAsync();

                    await page.SetExtraHTTPHeadersAsync(_customHeaders2);

                    await page.GotoAsync(url);

                    var pageContent = await page.ContentAsync();
                    var descriptionDiv = await page.QuerySelectorAsync("div[data-cy='adPageAdDescription']");
                    var description = SimilarityService.RemoveHtmlTags((await descriptionDiv.InnerHTMLAsync()).Trim());


                    return description;
                }
                return String.Empty;
            });
        }

        public async Task<string?> GetID(IPage page, string url)
        {
            var offertID = await Task.Run(async () =>
            {
                if (url != null)
                {
                    using var playwright = await Playwright.CreateAsync();
                    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = true,
                        ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                    });

                    var page = await browser.NewPageAsync();

                    await page.SetExtraHTTPHeadersAsync(_customHeaders2);

                    await page.GotoAsync(url);

                    var pageContent = await page.ContentAsync();

                    return Int32.Parse(string.Concat(Regex.Matches(pageContent.Substring(pageContent.IndexOf("Nr oferty w Otodom:"), 35), @"\d").Select(match => match.Value)));
                }
                return 0;
            });

            return offertID.ToString();
        }
    }
}
