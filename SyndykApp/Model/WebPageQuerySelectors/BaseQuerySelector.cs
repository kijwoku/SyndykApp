using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;
using SyndykApp.Services;
using System.Text.RegularExpressions;
using System.Xml.Linq;
namespace SyndykApp.Model.WebPageQuerySelectors
{
    public abstract class BaseQuerySelector
    {
        public Dictionary<string, string> _customHeaders = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0"
        };

        public async Task<IReadOnlyList<IElementHandle>> GetAdverts(IPage page, string selector)
        {
            return await page.QuerySelectorAllAsync(selector);
        }

        public virtual async Task<string> GetTitle(IElementHandle element, string selector)
        {
            var titleElement = await element.QuerySelectorAsync(selector);
            if (titleElement != null)
            {
                return await titleElement.InnerTextAsync();
            }
            else
            {
                throw new HTMLElementNotFoundException("Title element not found!");
            }
        }

        protected decimal GetPriceQuota(string fullPrice)
        {
            int pennies = 0, bills = 0;
            if (fullPrice.Contains(','))
            {
                pennies = Int32.Parse(string.Concat(Regex.Matches(fullPrice.Substring(fullPrice.IndexOf(',')), @"\d").Select(match => match.Value)));
                bills = Int32.Parse(string.Concat(Regex.Matches(fullPrice.Substring(0, fullPrice.IndexOf(',')), @"\d").Select(match => match.Value)));
            }
            else
            {
                bills = Int32.Parse(string.Concat(Regex.Matches(fullPrice, @"\d").Select(match => match.Value)));
            }
            if (bills <= 0)
            {
                throw new CalculationFailureException("Failed to extract quota from element!");
            }
            return bills + (decimal)pennies / 100;
        }

        public async Task<string?> GetDescription(string url, string querySelector)
        {
            return await Task.Run(async () =>
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                });

                var page = await browser.NewPageAsync();

                await page.SetExtraHTTPHeadersAsync(_customHeaders);

                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

                await Task.Delay(1000);

                var descriptionDiv = await page.QuerySelectorAsync(querySelector);
                if (descriptionDiv != null)
                {
                    return SimilarityService.RemoveHtmlTags((await descriptionDiv.InnerHTMLAsync()).Trim());
                }
                else
                {
                    throw new HTMLElementNotFoundException("Description element not found!");
                }
            });
        }
        public abstract Task<string> GetLink(IElementHandle element);

    }
}
