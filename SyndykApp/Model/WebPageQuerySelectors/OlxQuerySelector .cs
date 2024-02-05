﻿using Microsoft.Playwright;
using SyndykApp.Model.Exceptions;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public class OlxQuerySelector : BaseQuerySelector, IBaseQuerySelector
    {
        public string TitleSelector { get => "div[type='list'] h6"; }

        public string WebPage { get => "OLX   "; }


        public string GetDescriptionSelector(string url = "")
        {
            return (url.Contains("www.otodom.pl") ? "div[data-cy='adPageAdDescription']" : "div[data-cy='ad_description'] div") ?? String.Empty;
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

        public override async Task<string> GetLink(IElementHandle element)
        {
            var selector = "a";
            var linkElement = await element.QuerySelectorAsync(selector);
            if (linkElement != null)
            {
                var link = await linkElement.GetAttributeAsync("href");
                if (link != null && link.Length > 10)
                {
                    if (link.Contains("www.otodom.pl"))
                    {
                        return link;
                    }
                    else
                    {
                        return String.Format("https://www.olx.pl{0}", link);
                    }
                }
                
            }
            throw new HTMLElementNotFoundException("Link element not found!");
        }

        public async Task<decimal> GetPrice(IElementHandle element)
        {
            if (element != null)
            {
                var selector = "p[data-testid='ad-price']";
                var priceSpan = await element.QuerySelectorAsync(selector);
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
