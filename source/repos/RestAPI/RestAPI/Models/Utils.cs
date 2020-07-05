using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Models {
    public static class Utils {
        private static Dictionary<string, long> UltimoIDDeCadaTipo = new Dictionary<string, long>();

        public static long GerarNovoID(string Identificador) {
            if (!UltimoIDDeCadaTipo.Keys.Contains(Identificador))
                UltimoIDDeCadaTipo.Add(Identificador, 0);
            return UltimoIDDeCadaTipo[Identificador]++;
        }
    }
}
