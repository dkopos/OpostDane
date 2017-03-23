using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IAplikacji.InterfejsApli;

namespace DLLKolejnyProjekt
{
    public class KlasaDLLProjektu
    {
        //DLL umieszczone w oddzielnym projekcie w tej samej przestrzeni nazw co projekt podstawowy
        [Export(typeof(IOpreracji))]
        [ExportMetadata("Symbol", "Z")]
        class innaSuma : IOpreracji
        {
            public String Operacja(string lewy, string prawy)
            {
                return prawy + lewy + prawy;
            }
        }


    }
}
