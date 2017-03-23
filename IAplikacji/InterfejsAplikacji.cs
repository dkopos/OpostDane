using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAplikacji
{
    public class InterfejsApli
    {
        //Interfejs nadzorujący, czy dll implementuje oczekiwaną funkcjie
        public interface IOpreracji
        {
            string Operacja(string lewyNapis, string prawyNapis);
        }

        
        //Interfejs wykorzystywany w odczycie metadanych
        //Dzięki niemu w łatwy sposób odczytujemy do czego biblioteka ma być użyta
        //w tym przypadku zostanie wykorzystany do odczytania instrukcji która ma uruchomić 
        //dll. Jednak samo wywołanie odpowiedniej dll. Odbywa się poprzez wywołanie funkcji interfejsu aplikacji. Przy użyciu
        //elementu kolekcji. Zatem ten interfejs pobiera dane czysto informacyjne
        public interface ISymbolOperacji
        {
            string Symbol { get; }
        }

    }
}
