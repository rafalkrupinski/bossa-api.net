 "BossaAPI .NET Class Library"
-------------------------------------------------------

Biblioteka "pjank.BossaAPI.dll" - napisana w C# (.NET 3.5) - powstała, by
ułatwić korzystanie z możliwości API udostępnionego niedawno przez DM BOSSA.
Udostęnia ona czytelne klasy reprezentujące m.in.: poszczególne rachunki 
użytkownika, papiery tam zgromadzone, ich notowania oraz aktywne zlecenia.
Wszystko w takiej formie, by nawet laik, który dopiero zaczyna programować,
był w stanie z nich efektywnie korzystać. I to korzystać *zawsze tak samo* -
niezależnie czy sama komunikacja opiera się na współpracy z aplikacją NOL3
(jak dzisiaj), czy może bezpośrednio z serwerami DM (w przyszłości) albo
jeszcze inaczej (np. z komórki do własnego serwera w domu, gdzie działa NOL3).

Bibliotekę udostępniam na licencji Apache v2.0 - wraz z pełnym, otwartym
kodem źródłowym. W ten sposób każdy może ją wykorzystać w swoich projektach -
nieważne, czy to projekt komercyjny, czy darmowy, otwarty czy też nie... 
Obowiązuje jedynie zasada, by nie zabrakło tam nigdy wzmianki o pierwotnym
autorze (czyli o mnie :)) i wszystkich kolejnych (jeśli tacy się pojawią
i będą udostepniać swoje modyfikacje, do czego oczywiście zachęcam).

                                                Przemysław Jankowski
-------------------------------------------------------------------------------
  http://www.pjank.net/bossa.api/             e-mail: bossa.api@pjank.net      
-------------------------------------------------------------------------------


Szybki start:

 -> uruchom aplikację "DemoApp.exe" -
     tak zobaczysz jak (i czy w ogóle :)) to wszystko działa...
 -> przejrzyj opis (z przykładami) na początku pliku "BossaAPI\src\Bossa.cs"
 -> uruchom "DemoConsole" i obejrzyj jej kod źródłowy, m.in. katalog 'Modules'
 -> podlinkuj do swojego projektu "pjank.BossaAPI.dll" i korzystaj ;-)


Mamy tu obecnie 4 projekty (biblioteka + kilka przykładów jej użycia):

1) "BossaAPI" - kod samej biblioteki, podzielony na kilka warstw:

  a) "zewnętrzna", udostępniająca w prostej formie większość funkcji API
     Obejmuje najważniejszą klasę "Bossa" i dostępne z niej potem kolejne: 
     "BosAccount", "BosPaper", "BosOrder", "BosInstrument", "BosTrade" itd. 
     (wszystko w podkatalogach "src\AccountData\" i "src\MarketData\").

  b) niskopoziomowa obsługa protokołu FIXML
     Komplet klas znajdujący się w katalogu "src\Networking\Fixml\". 
     Dla każdego rodzaju komunikatu (tych wysyłanych, jak i odbieranych)
     mamy indywidualną klasę C# oferującą wszystkie możliwe parametry
     w formie łatwo dostępnych pól obiektu (reprezentowanych przez kolejne
     klasy albo typy wyliczeniowe odpowiednie do rodzaju danego parametru).
     Jeśli z jakiegoś powodu chcesz samodzielnie oprogramować komunikację
     np. bezpośrednio z aplikacją NOL3, to będzie najlepsze rozwiązanie - 
     pozwala skupić się na treści komunikatów zamiast obsłudze samego XML'a
     i zmniejsza ryzyko popełniania błędów.
     Możesz też użyć dodatkowej klasy "NolClient" (katalog "src\Networking"),
     która wspomaga całą komunikację z NOLem - zajmuje się ustalaniem numeru
     portu do nawiązania połączenia, zalogowaniem użytkownika, obsługą kanału
     asynchronicznego w oddzielnym wątku itp. itd.     

  c) warstwa pośrednia - łącząca dwie powyższe i zaprojektowana w taki sposób,
     by umożliwić bardzo szybką podmianę protokołu FIXML czymś zupełnie innym.
     Obejmuje interfejs "IBosClient" (którego jedyną na razie implementację
     stanowi wspomniana klasa "NolClient") oraz klasy transportowe (DTO) 
     służące do przekazywania danych między kolejnymi warstwami biblioteki.

2) DemoApp - przykładowa aplikacja GUI 

  Aplikacja demonstruje większość dostępnych funkcji, włącznie z możliwością
  oglądania tabeli z aktualnymi notowaniami i składania zleceń. Jednocześnie
  w okienku logu umożliwia podgląd wszystkiego, co się dzieje "pod maską".
  Całość wykorzystuje "zewnętrzną" warstwę biblioteki - klasę "Bossa".

3) DemoConsole - aplikacja konsolowa obejmująca całą serię przykładów

  Demonstruje sposób wykorzystania wybranych elementów biblioteki - zarówno
  prostszej klasy "Bossa", jak i mniej lub bardziej zaawansowane użycie klasy
  "NolClient"... a nawet połączenie obu tych metod w jednej aplikacji.
  Każdy z przykładów zapisano w formie samodzielnego "modułu", których lista
  wyświetla się w formie menu po uruchomieniu aplikacji.
  Polecam zajrzenie do kodu źródłowego interesujących nas modułów.

4) MarketMonitor - bardziej "praktyczna" przykładowa aplikacja konsolowa

  Programik, który możemy wywołać z parametrami - listą instrumentów...
  a na wyjściu otrzymamy bieżące notowania tych wybranych instrumentów.



===== 2012-06-03 == wersja 0.4 ================================================

Rozbudowa klasy "Bossa" o obsługę takich danych, jak: LOP, wolumen sesji,
cena otwarcia/zamknięcia itp.
Rozszerzenie i uporządkowanie dołączonych aplikacji przykładowych.
I szereg drobniejszych poprawek zrealizowanych w ciągu ostatniego roku.

===== 2011-03-28 == wersja 0.3 ================================================

W miarę zamknięta "zewnętrzna warstwa" biblioteki w postaci klasy "Bossa"
i tego, co się z niej dalej wywodzi (lista rachunków, dane dotyczące tych
rachunków, lista instrumentów, ich notowania, składanie zleceń itp.).
Dodano też drugą przykładową aplikację - tym razem z interfejsem graficznym -
która pozwala niemal całkowicie zarządzać swoim rachunkiem podczas sesji (może
nieco surowa forma, ale to w końcu głównie prezentacja możliwości biblioteki).      

===== 2011-02-01 == wersja 0.2 ================================================

Pierwsza upubliczniona wersja biblioteki.
Generalnie ukończony (na ile to możliwe, przy dostępnej na dzień dzisiejszy 
wersji protokołu/aplikacji/dokumentacji oferowanych przez ludzi z Bossy) szereg 
"niskopoziomowych" klas odpowiedzialnych za komunikację FIXML z aplikacją NOL3.
Do tego klasa "NolClient" umożliwiająca już teraz zbieranie bieżących notowań 
wybranych instrumentów w postaci łatwej do dalszego przetwarzania.

===============================================================================
