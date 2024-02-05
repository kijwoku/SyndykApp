using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;
using System.Text.RegularExpressions;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class NieruchomosciOnlineQuerySelector : BaseQuerySelector, IBaseQuerySelector
    {
        public string TitleSelector { get => "a"; }
        public string WebPage { get => "NieruchomosciOnline"; }

        public string GetDescriptionSelector(string url = "")
        {
            return "div[id='boxCustomDesc'] div";
        }

        public async Task<bool> CheckIfAnyAdvertsOnPage(IPage element, int pageNumber = 0)
        {
            if (element.Url.IndexOf("p=") != -1)
            {
                var pageNo = Int32.Parse(element.Url.Substring(element.Url.IndexOf("p=") + 2, 1));
                return pageNumber == pageNo;
            }
            else
            {
                return true;
            }
        }

        public override async Task<string> GetLink(IElementHandle element)
        {
            var selector = "a";
            var linkElement = await element.QuerySelectorAsync(selector);
            if (linkElement != null)
            {
                var link = await linkElement.GetAttributeAsync("href");
                if (link != null && link.Length > 10)
                {
                    return link;
                }
            }
            throw new HTMLElementNotFoundException("Link element not found!");
        }
        public async Task<decimal> GetPrice(IElementHandle element)
        {
            var selector = "p.primary-display span";
            var priceSpan = await element.QuerySelectorAsync(selector);
            if (priceSpan != null)
            {
                var fullPrice = await priceSpan.InnerTextAsync();
                return GetPriceQuota(fullPrice);
            }
            throw new CalculationFailureException("Failed to extract quota from element!");
        }
    }
}
