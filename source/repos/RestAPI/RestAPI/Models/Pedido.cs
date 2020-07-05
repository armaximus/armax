using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Models {
    public class Pedido {
        public List<Produto> Produtos { get; }

        public Pedido(List<Produto> produtos) {
            Produtos = produtos ?? new List<Produto>();
        }
    }
}
