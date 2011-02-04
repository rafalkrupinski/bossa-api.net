 "BossaAPI .NET Class Library"
-------------------------------------------------------

Biblioteka "pjank.BossaAPI.dll" - napisana w C# (.NET 3.5) - powstała, by
ułatwić korzystanie z możliwości API udostępnionego niedawno przez DM BOSSA.
Docelowo biblioteka będzie udostępniała czytelne klasy reprezentujące m.in.:
rachunek maklerski, poszczególne papiery na nim zgromadzone, ich notowania
oraz aktywne zlecenia. Wszystko w takiej formie, by nawet laik, który dopiero
zaczyna przygodę z tworzeniem np. makr w Visual Basicu, był w stanie z nich
efektywnie korzystać. I to korzystać zawsze tak samo, niezależnie od użytego
"w środku" rozwiązania komunikacyjnego. Na dzień dzisiejszy wszystko wewnątrz
opiera się na współpracy z aplikacją NOL3 Comarchu, ale kiedyś pewnie będziemy
mogli łączyć się bezpośrednio z domem maklerskim... albo jeszcze inną metodą.

Bibliotekę udostępniam na licencji Apache v2.0 - wraz z pełnym, otwartym
kodem źródłowym. W ten sposób każdy może ją wykorzystać w swoich projektach -
nieważne, czy to projekt komercyjny, czy darmowy, otwarty czy też nie... 
Obowiązuje jedynie zasada, by nie zabrakło tam nigdy wzmianki o jej pierwotnym
autorze (czyli o mnie :)), jak i wszystkich kolejnych (jeśli tacy się pojawią
i będą udostepniać swoje modyfikacje, do czego oczywiście zachęcam).

                                                Przemysław Jankowski
-------------------------------------------------------------------------------
  http://www.pjank.net/bossa.api/             e-mail: bossa.api@pjank.net      



"Instrukcja obsługi" (w skrócie)

1. Mamy tu obecnie dołączone dwa projekty: "BossaAPI" oraz "TestApp1".
   Pierwszy z nich to kod samej biblioteki, drugi - prosta aplikacja konsolowa
   demonstrująca najważniejsze z dostępnych na tę chwilę funkcji biblioteki.

   Zaczynamy więc najlepiej od przyjrzenia się plikowi "TestApp1\Program.cs",
   sam kod jak i dołączone do niego komentarze powinny już sporo wyjaśnić.

2. Jeśli chodzi o samą dll-kę, składa się ona w tej chwili z dwóch części:
     "namespace pjank.BossaAPI" - to sam szkielet biblioteki
     "namespace pjank.BossaAPI.Fixml" - cała obsługa protokołu FIXML

   Sam układ folderów jest bardziej złożony... dość jednak zaznaczyć, że klasy
   należące do "namespace pjank.BossaAPI.Fixml" znajdziemy wewnątrz folderu
   "BossaAPI\src\Networking\Fixml" (pogrupowane dalej wg typów komunikatów).

   Reszta folderów poza tym "Fixml" należy do głównego "namespace" i stąd,
   jak na razie, interesuje nas tylko "BossaAPI\src\Networking\NolClient.cs".
   Na pozostałe proszę póki co nawet nie zwracać uwagi ;-)






===== 2011-02-01 == wersja 0.2 ================================================

   Pierwsza upubliczniona wersja biblioteki.

   Generalnie ukończony (na ile to możliwe, przy dostępnej na dzień dzisiejszy 
wersji protokołu/aplikacji/dokumentacji oferowanych przez ludzi z Bossy) szereg 
"niskopoziomowych" klas odpowiedzialnych za komunikację FIXML z aplikacją NOL3.
Dla każdego rodzaju komunikatu (zarówno tych wysyłanych, jak i odbieranych)
mamy indywidualną klasę C#/.NET udostępniającą wszystkie parametry komunikatu
w formie łatwo dostępnych właściwości (które same w sobie też są najczęściej
reprezentowane inną klasą lub typem wyliczeniowym odpowiednim dla konkretnego
typu danego parametru).

   Do tego klasa "NolClient" wspomagająca komunikację z NOLem (ustalenie portu
dla połączenia, zalogowanie użytkownika, obsługa kanału asynchronicznego itd.)
oraz umożliwiająca już teraz zbieranie bieżących notowań wybranych instrumentów
w postaci łatwej do dalszego przetwarzania.

===============================================================================
