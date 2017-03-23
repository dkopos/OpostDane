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
        [Export(typeof(IOpreracji))]
        [ExportMetadata("Symbol", "A")]
        class innaSuma : IOpreracji
        {
            public String Operacja(string lewy, string prawy)
            {
                return prawy + lewy + prawy;
            }
        }


    }
}
