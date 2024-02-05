using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class GratkaQuerySelector : BaseQuerySelector, IBaseQuerySelector
    {
        public string TitleSelector { get => "h2.teaserUnified__title"; }

        public string WebPage { get => "GRATKA"; }


        public string GetDescriptionSelector(string url = "")
        {
            return "div[data-cy='offerDescription']";
        }
        public async Task<bool> CheckIfAnyAdvertsOnPage(IPage element, int pageNumber = 0)
        {
            var selector = "input[data-cy='paginationInput']";
            var activePage = await element.QuerySelectorAsync(selector);
            if (activePage != null)
            {
                var pageNumberText = await activePage.InputValueAsync();
                if (pageNumberText != String.Empty)
                {
                    var number = int.Parse(pageNumberText);
                    return pageNumber == number;
                }
            }
            return false;
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
            if (element != null)
            {
                var selector = "p[data-cy='teaserPrice']";
                var priceSpan = await element.QuerySelectorAsync(selector);
                if (priceSpan != null)
                {
                    var fullPrice = await priceSpan.InnerTextAsync();
                    if (fullPrice.Contains("\n"))
                    {
                        fullPrice = fullPrice.Substring(0, fullPrice.IndexOf("\n"));
                    }
                    return GetPriceQuota(fullPrice);
                }

            }
            throw new CalculationFailureException("Failed to extract quota from element!");
        }
    }
}
