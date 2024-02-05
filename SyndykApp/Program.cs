using Microsoft.Playwright;
using SyndykApp.Services;
using System.Diagnostics;

class Program
{
    public static async Task Main()
    {
        var sw = Stopwatch.StartNew();
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
        });


        var maxPrice = 300000;
        var otodomTask = Otodom.Run(maxPrice, browser);
        var olxTask = Olx.Run(maxPrice, browser);
        var morizonTask = Morizon.Run(maxPrice, browser);
        var gratkaTask = Gratka.Run(maxPrice, browser);
        var nieruchomosciOnlineTask = NieruchomosciOnline.Run(maxPrice, browser);

        await Task.WhenAll(otodomTask, olxTask, morizonTask, gratkaTask, nieruchomosciOnlineTask);

        //var morizonTask = Morizon.Run(maxPrice, browser);
        //await Task.WhenAll(morizonTask);
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine(String.Format("KONIEC !!! Czas trwania: ${0} minut", sw.Elapsed.TotalSeconds / 60 ));

        Console.ResetColor();
    }
}