using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;
using SyndykApp.Services;
using System;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class MorizonQuerySelector : BaseQuerySelector, IBaseQuerySelector
    {
        public string TitleSelector { get => "span[data-cy='listing-item-title']"; }
        public string WebPage { get => "MORIZON"; }

        public string GetDescriptionSelector(string url = "")
        {
            return "section div:is([style*='max-height'])";
        }

        public async Task<bool> CheckIfAnyAdvertsOnPage(IPage element, int pageNumber = 0)
        {
            if (element.Url.IndexOf("page=") != -1)
            {
                var pageNo = Int32.Parse(element.Url.Substring(element.Url.IndexOf("page=") + 5, 1));
                return pageNumber == pageNo;
            }
            else
            {
                return true;
            }
        }

        public override async Task<string> GetTitle(IElementHandle element, string selector)
        {
            return "Morizon - Ogłoszenie";
        }

        public override async Task<string> GetLink(IElementHandle element)
        {
            var linkElement = await element.QuerySelectorAsync("a");
            if (linkElement != null)
            {
                var link = await linkElement.GetAttributeAsync("href");
                if (link != null && link.Length > 10)
                {
                    return String.Format("https://www.morizon.pl{0}", link);
                }
            }
            throw new HTMLElementNotFoundException("Link element not found!");
        }
        public async Task<decimal> GetPrice(IElementHandle element)
        {
            var link = await GetLink(element);
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

                await page.GotoAsync(link);

                var selector = "div[id='basic-info-price-row'] div span";
                var priceSpan = await page.QuerySelectorAsync(selector);
                if (priceSpan != null)
                {
                    var fullPrice = await priceSpan.InnerTextAsync();
                    return GetPriceQuota(fullPrice);
                }
                else
                {
                    throw new HTMLElementNotFoundException("Description element not found!");
                }
            });
        }
    }
}

