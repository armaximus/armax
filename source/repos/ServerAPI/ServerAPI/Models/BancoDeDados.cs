using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.Models {
    public class BancoDeDados {
        public static List<Objeto> TodosOsObjetos = new List<Objeto>();
        public static List<Estante> Estantes = new List<Estante>();
        public static List<Robo> Robos = new List<Robo>();
        public static Objeto PontoDeEntrega;

        public static void CriarObjetos() {
            TodosOsObjetos = new List<Objeto>();
            Estantes = new List<Estante>();
            Robos = new List<Robo>();

            int teste = 0;

            for (int i = 1; i < 10 - 3; i++) {
                int teste2 = 0;
                for (int j = 0; j < 8; j++) {
                    Estante e = new Estante(i, j);
                    Estantes.Add(e);
                    TodosOsObjetos.Add(e);
                    teste2++;
                    if (teste2 == 2) {
                        j++;
                        teste2 = 0;
                    }
                }
                teste++;

                if (teste == 2) {
                    i++;
                    teste = 0;
                }
            }

            Estantes[1].Produtos.Add(new Produto("Banana"));
            Estantes[14].Produtos.Add(new Produto("Maçã"));

            for (int i = 1; i < 8 - 1; i++) {
                Robo r = new Robo(10 - 2, i);
                Robos.Add(r);
                TodosOsObjetos.Add(r);
            }

            TodosOsObjetos.Add(PontoDeEntrega);
        }
    }
}
