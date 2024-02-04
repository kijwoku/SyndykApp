using Microsoft.Playwright;
using SyndykApp.Model;
using SyndykApp.Services;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
class Program
{
    public static async Task Main()
    {
        Otodom.Run();
        await Olx.Run();

        string str1 = @"a a s  w e q w e q a a s  w e 1 q w e p";
        string str2 = @"a a s  w e 1 q w e p a a s  w e 1 q w e p";
        var asd = @"Syndyk masy upadłości Andrzeja Dreszera ogłasza 5. konkurs na zbycie udziału 1/2 we współwłasności nieruchomości budynkowej z wolnej ręki w drodze konkursu ofert wg poniższych warunków:

            I. sprzedaży podlega udział 1/2 we współwłasności kamienicy położonej w Krośniewicach pl. Wolności nr 21 - operat szacunkowy z dnia 24.01.2023 roku sporządzony w postępowaniu egzekucyjnym KM 176/22; działka o pow. 662 m.kw.; powierzchnia użytkowa budynku kamienicy: 350 m.kw. oraz dwa budynki garażowe o pow. 17 i 14 m.kw.; teren zabudowy mieszkaniowej śródmiejskiej. Dla nieruchomości prowadzona jest księga wieczysta nr LD1Y/00031912/5; wadium w wysokości 30.000,00 zł należy wpłacić na konto masy upadłości nr 84 1140 1108 0000 5041 4300 1023 mBank SA; Wartość rynkowa udziału 1/2 ustalona w drodze wyceny przez biegłego ustalona została na 257 400,00 zł – dla sprzedaży wymuszonej w postępowaniu egzekucyjnym.

            Zainteresowani nabyciem mogą zgłaszać zapytania na adres: biuro(at)mikuccy pl – w celu otrzymania skanu operatu szacunkowego.

            Przystępujący do konkursu ofert powinien:

            złożyć ofertę zakupu na piśmie w zaklejonej kopercie z opisem: ""oferta Krośniewice"" na adres biura syndyka: Maciej Mikucki, Kancelaria Mikuccy, ul. Narutowicza 45 lok. 4, 90-130 Łódź do dnia 22.02.2024 roku do godz. 15:00 (decyduje fizyczny wpływ oferty do biura syndyka);
            w ofercie należy wskazać dokładne dane osobowe, mailowe i adresowe oraz nr telefonu nabywcy;
            wpłacić wadium w odpowiedniej wysokości na konto masy upadłości nr 84 1140 1108 0000 5041 4300 1023 mBank SA - wadium powinno być wpłacone przez składającego ofertę i zaksięgowane na w/w rachunku bankowym najpóźniej do końca dnia 21.02.2024 roku;
            o wyniku konkursu syndyk poinformuje oferentów mailowo lub pisemnie;
            termin zawarcia umowy sprzedaży w formie aktu notarialnego ze zwycięzcą konkursu ofert: ca 90 dni od dnia rozstrzygnięcia konkursu (koszty umowy ponosi nabywca);
            wadium wpłacone przez uczestnika, który wygra konkurs zaliczone będzie na poczet ceny sprzedaży. Wadium wpłacone przez pozostałych ulegnie zwrotowi niezwłocznie po zakończeniu konkursu. Wadium przepadnie na rzecz masy upadłości w razie uchylenia się, bądź odstąpienia uczestnika, który wygrał konkurs od zawarcia umowy sprzedaży w formie aktu notarialnego w miejscu i terminie wyznaczonym przez syndyka;
            cena sprzedaży nieruchomości pomniejszona o wpłacone wadium powinna być wpłacona przez zwycięzcę konkursu ofert i zaksięgowana na w/w rachunku bankowym masy upadłości najpóźniej dzień przed wyznaczonym dniem zawarcia umowy u notariusza.
            cena minimalna wynosi: 51 480,00 zł (20% sumy oszacowania).
            UWAGA: drugi ze współwłaścicieli zamierza zbyć swój udział 1/2 w przedmiotowej kamienicy za ca 257 400,00 zł - cena do negocjacji - (można zatem nabyć całość nieruchomości - po ustaleniach z drugim współwłaścicielem - co do zasad sprzedaży jego udziału w wysokości 1/2).";

        var asd2 = @"Syndyk masy upadłości Andrzeja Dreszera ogłasza 5. konkurs na zbycie udziału 1/4 we współwłasności nieruchomości budynkowej z wolnej ręki w drodze konkursu ofert wg poniższych warunków:

            I. sprzedaży podlega udział 1/3 we współwłasności kamienicy położonej w Krośniewicach pl. Wolności nr 22 - operat szacunkowy w dniu 24.03.2023 roku sporządzony w postępowaniu egzekucyjnym KM 176/22; działka o pow. 662 m.kw.; powierzchnia użytkowa budynku kamienicy: 350 m.kw. oraz dwa budynki garażowe o pow. 17 i 14 m.kw.; teren zabudowy mieszkaniowej śródmiejskiej. Dla nieruchomości prowadzona jest księga wieczysta nr LD1Y/00031912/5; wadium w wysokości 30.000,00 zł należy wpłacić na konto masy upadłości nr 84 1140 1108 0000 5041 4300 1023 mBank SA; Wartość rynkowa udziału 1/2 ustalona w drodze wyceny przez biegłego ustalona została na 257 400,00 zł – dla sprzedaży wymuszonej w postępowaniu egzekucyjnym.

            Zainteresowani nabyciem mogą zgłaszać zapytania na adres: biuro(at)mikuccy pl – w celu otrzymania skanu operatu szacunkowego.


            złożyć ofertę zakupu na piśmie w zaklejonej kopercie z opisem: ""oferta Krośniewice"" na adres biura syndyka: Maciej Mikucki, Kancelaria Mikuccy, ul. Narutowicza 45 lok. 4, 90-130 Łódź do dnia 22.02.2024 roku do godz. 15:00 (decyduje fizyczny wpływ oferty do biura syndyka);
            w ofercie należy wskazać dokładne dane osobowe, mailowe i adresowe oraz nr telefonu nabywcy;
            wpłacić wadium w odpowiedniej wysokości na konto masy upadłości nr 84 1140 1108 0000 5041 4300 1023 mBank SA - wadium powinno być wpłacone przez składającego ofertę i zaksięgowane na w/w rachunku bankowym najpóźniej do końca dnia 21.02.2024 roku;
            o wyniku konkursu syndyk poinformuje oferentów mailowo lub pisemnie;
            termin zawarcia umowy sprzedaży w formie aktu notarialnego ze zwycięzcą konkursu ofert: ca 90 dni od dnia rozstrzygnięcia konkursu (koszty umowy ponosi nabywca);
            wadium wpłacone przez uczestnika, który wygra konkurs zaliczone będzie na poczet ceny sprzedaży. Wadium wpłacone przez pozostałych ulegnie zwrotowi niezwłocznie po zakończeniu konkursu. Wadium przepadnie na rzecz masy upadłości w razie uchylenia się, bądź odstąpienia uczestnika, który wygrał konkurs od zawarcia umowy sprzedaży w formie aktu notarialnego w miejscu i terminie wyznaczonym przez syndyka;
            cena sprzedaży nieruchomości pomniejszona o wpłacone wadium powinna być wpłacona przez zwycięzcę konkursu ofert i zaksięgowana na w/w rachunku bankowym masy upadłości najpóźniej dzień przed wyznaczonym dniem zawarcia umowy u notariusza.
            cena minimalna wynosi: 51 480,00 zł (20% sumy oszacowania).
            UWAGA: drugi ze współwłaścicieli zamierza zbyć swój udział 1/2 w przedmiotowej kamienicy za ca 257 400,00 zł - cena do negocjacji - (można zatem nabyć całość nieruchomości - po ustaleniach z drugim współwłaścicielem - co do zasad sprzedaży jego udziału w wysokości 1/4).";

        var similarity2 = SimilarityService.AreDescriptionsSimilar(str1, str2);
        var ewq = SimilarityService.GetFirstLetterOfEveryWord(asd);
        var ewq2 = SimilarityService.GetFirstLetterOfEveryWord(asd2);

        var similarity3 = SimilarityService.AreDescriptionsSimilar(ewq, ewq2);


        Console.WriteLine($"Podobieństwo: {similarity2:P}");
    }
}