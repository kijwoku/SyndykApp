using SyndykApp.Services;

class Program
{
    public static async Task Main()
    {
        var maxPrice = 300000;
        var otodomTask = Otodom.Run(maxPrice);
        var olxTask =  Olx.Run(maxPrice);
        var morizonTask =  Morizon.Run(maxPrice);
        var gratkaTask = Gratka.Run(maxPrice);
        var nieruchomosciOnlineTask = NieruchomosciOnline.Run(maxPrice);

        await Task.WhenAll(otodomTask, olxTask, morizonTask, gratkaTask, nieruchomosciOnlineTask);
        //await Task.WhenAll(morizonTask);
        Console.WriteLine("KONIEC.");
    }
}