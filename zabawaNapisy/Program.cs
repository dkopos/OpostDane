using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IAplikacji.InterfejsApli;

namespace zabawaNapisy
{

    //Pierwsza klasa symulująca dll. Poniewarz zaszyta w kodzie
    //Jak widać zawiera dwie metadane, informacjie jaki kontak spełnia, czyli jaki interfejs obsługuje
    //oraz dodatkowe informacjie kolekcji słownikowej. dzienki urzyciu podwójnego cudzysłowia wiadomo że jest to string
    //Interfejs musi byc obsłużony inaczej niewiadomo w jaki sposób biblioteka miała by sie po otrzymaniu danych zachować
    [Export(typeof(IOpreracji))]
    [ExportMetadata("Symbol", "C")]
    class usuwanie : IOpreracji
    {
        public String Operacja(string lewy,string prawy)
        {
            return lewy.Replace(prawy,"");
        }
    }

    //Kolejna klasa symulująca dll. Poniewarz zaszyta w kodzie
    [Export(typeof(IOpreracji))]
    [ExportMetadata("Symbol", "S")]
    class lewyDoPrawego : IOpreracji
    {
        public String Operacja(string lewy, string prawy)
        {
            return lewy+prawy;
        }
    }

    /* [Export(typeof(IOpreracji))]
     [ExportMetadata("Symbol", "A")]
     class innaSuma : IOpreracji
     {
         public String Operacja(string lewy, string prawy)
         {
             return prawy+ lewy + prawy;
         }
     }*/



    // Kontener przechowywujący operandy
    class KontenerNapisy
    {
        public string lewyNapis { get; set; }
        public string instrukcja { get; set; }
        public string prawyNapis { get; set; }
    
    }



    //Program Główny
    class Program
    {
        static void Main(string[] args)
        {

           // Program p = new Program(); //Composition is performed in the constructor
            String s;
            KontenerNapisy przetwarzanyObiekt;
            ZarządcaDLC p = new ZarządcaDLC();
            Console.WriteLine("Podaj dwa napisy połaczone komenda");
            while (true)
            {
                s = Console.ReadLine();
                 przetwarzanyObiekt= RozdzielNapisy(s);
              

                Console.WriteLine(p.Wykonaj(przetwarzanyObiekt));
               
            }
        }


        //funkcja rozdzielenia napisów na podstawie tablicy ASCI
        //Po wykryciu elementu spoza małych liter alfabetu
        //wykonywany jest case1 czyli czy zmienna różna od małych liter alfabetu
        //reszta zapisywana jest jako napis prawy
        //Po wszystkim zwracany jest obiekt KontenerNapisy. Reprezentujący trzy elementy
        // napis prawy i lewy oraz instrukcje która informuje w jaki sposób ciąg ma być przetwarzany
        static KontenerNapisy RozdzielNapisy(String input)
        {
            KontenerNapisy ob = new KontenerNapisy();
            int napis = 0;

            for (int i= 0; i< input.Length;i++)
            {
                
      
                switch (napis)
                {
                    case 0:
                        if (!((int)input[i] > 96 && (int)input[i] < 123)) { napis++; };
                        ob.lewyNapis = ob.lewyNapis + input[i];
                        break;
                    case 1:
                        if (((int)input[i] > 96 && (int)input[i] < 123)) { napis++; };
                        ob.instrukcja = input[i].ToString();
                        napis++;
                        break;
                    default:
                        ob.prawyNapis = ob.prawyNapis + input[i];
                        break;

                }

            }


            return ob;
        }

    }

    class ZarządcaDLC
    {
        private CompositionContainer kontener;
        
        [ImportMany(typeof(IOpreracji))]
        //Zmienna przechowująca listę załadowanych bibliotek. Dyrektywa powyżej ImportMany
        //sprawia że jest ona wypełniana klasami. Typeof() definiuje jest jakiego typu maja być to klasy
        //w tym przypadku spełniające interfejs IOperacji. Następnie ta lista jest rzutowana na kolekcjie IEnumerable
        //zatem można tutaj zdefiniować dodatkowy warunek. np. Wszystkie których metadane spełniają interfejs ISymbolOperacji
        //Lub używając kolekcji słownikowej będzie tworzony jedynie obiekt i informacje o metadanych jednak bez sprawdzania jakiego typu 
        //jest tam zapisana informacja. Perszy element zawsze jest w słowniku string jak w metadanych a drugi typ object. Może przechować
        //dowolny typ języka C#. 
        //IEnumerable<Lazy<IOpreracji, ISymbolOperacji>> przyłaczoneBibliotekiDLC;
        IEnumerable<Lazy<IOpreracji, IDictionary<string, object>>> przyłaczoneBibliotekiDLC;
        
        public ZarządcaDLC()
        {
            //Katalog agregujący będący nadrzędnym w stosunku do pozostałych katalogów
            var katalogAgregujący = new AggregateCatalog();

            //Dodanie innych dokumentów znajdujących się w tym samym pliku co program
            katalogAgregujący.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));

            //katalog fizyczny w którym program będzie poszukiwał  dll spełniających warunki programu
            //w tym przypadku jedyny warunek został zawarty  w dyrektywie import many
            // czyli spełnienie interfejsu IOpreracji. Następnie dane są dodawane do kolekcji IEnumerable poniżej dyrektywy
            //Jeśli nie są spełnione warunki tej kolekcji to obiekt także niezostanie załadowany
            katalogAgregujący.Catalogs.Add(new DirectoryCatalog("C:\\ABC"));

            //Utworzenie Kontenera który informuje o tym gdzie dll mają być szukane
            kontener = new CompositionContainer(katalogAgregujący);

            //importowanie znalezionych rozszerzeń
            try
            {
                this.kontener.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }


        }

        //funkcja obsługująca wykonywanie dołączonych bibliotek
        public string Wykonaj(KontenerNapisy napis)
        {
            
            if (przyłaczoneBibliotekiDLC == null) { return "wystapil blad z biblioteka MEF"; }
            
            foreach (var i in przyłaczoneBibliotekiDLC)
             {
                if (i.Metadata["Symbol"].Equals(napis.instrukcja))
                {
                    return i.Value.Operacja(napis.lewyNapis, napis.prawyNapis).ToString();
                }
                //Ta sama operacja co powyżej jednak zmienna korzysta z metadanych, używając interfejsu zamiast słownika
                /* if (i.Metadata.Symbol.Equals(napis.instrukcja))
                 {
                    return i.Value.Operacja(napis.lewyNapis, napis.prawyNapis).ToString();
                 }*/
            }


            return "Operacja nieznaleziona";

        }

    }


 }
