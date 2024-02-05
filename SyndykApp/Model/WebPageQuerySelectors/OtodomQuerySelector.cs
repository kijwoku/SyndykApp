using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;
using System.Text.RegularExpressions;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class OtodomQuerySelector : BaseQuerySelector, IBaseQuerySelector
    {
        public string TitleSelector { get => "span[data-cy='listing-item-title']"; }
        public string WebPage { get => "OTODOM"; }

        public string GetDescriptionSelector(string url = "")
        {
            return "div[data-cy='adPageAdDescription']";
        }

        public async Task<bool> CheckIfAnyAdvertsOnPage(IPage element, int pageNumber = 0)
        {
            var selector = "div[data-cy='no-search-results']";
            var notFoundFrame = await element.QuerySelectorAsync(selector);
            return notFoundFrame == null;
        }

        public override async Task<string> GetLink(IElementHandle element)
        {
            var selector = "a[data-cy='listing-item-link']";
            var linkElement = await element.QuerySelectorAsync(selector);
            if (linkElement != null)
            {
                var link = await linkElement.GetAttributeAsync("href");
                if (link != null && link.Length > 10)
                {
                    return String.Format("https://www.otodom.pl{0}", link);
                }
            }
            throw new HTMLElementNotFoundException("Link element not found!");
        }
        public async Task<decimal> GetPrice(IElementHandle element)
        {
            var selector = "article";
            var article = await element.QuerySelectorAsync(selector);
            if (article != null)
            {
                var articleSelector = "div:nth-last-child(3) span:first-child";
                var priceSpan = await article.QuerySelectorAsync(articleSelector);
                if (priceSpan != null)
                {
                    var fullPrice = await priceSpan.InnerTextAsync();
                    return GetPriceQuota(fullPrice);
                }
            }
            throw new CalculationFailureException("Failed to extract quota from element!");
        }
    }
}
