using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Models {
    public class PontoEntrega : Objeto {
        public override string RepresentacaoVisual { get { return "P"; } }

        public PontoEntrega(int posicaoX, int posicaoY) : base(posicaoX, posicaoY) { }

        protected override void Mover() {
            throw new Exception("O galpão não pode ser movido!");
        }
    }
}
