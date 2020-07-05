using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Models {
    public class Produto {
        public long ID { get; private set; }
        public string NomeProduto { get; private set; }

        public Produto(string nomeProduto) {
            NomeProduto = nomeProduto;
            ID = Utils.GerarNovoID("P");
        }
    }
}
