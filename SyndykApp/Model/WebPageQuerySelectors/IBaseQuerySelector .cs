using Microsoft.Playwright;

namespace SyndykApp.Model.WebPageQuerySelectors
{
    public interface IBaseQuerySelector
    {
        string WebPage { get; }
        string TitleSelector { get; }
        string GetDescriptionSelector(string url = "");
        Task<IReadOnlyList<IElementHandle>> GetAdverts(IPage page, string selector);
        Task<string> GetTitle(IElementHandle element, string selector);
        Task<string?> GetDescription(string url, string selector);
        Task<string> GetLink(IElementHandle element);
        Task<decimal> GetPrice(IElementHandle element);
        Task<bool> CheckIfAnyAdvertsOnPage(IPage element, int pageNumber = 0);

    }
}
