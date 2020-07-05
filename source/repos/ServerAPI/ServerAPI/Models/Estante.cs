using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.Models {
    public class Estante : Objeto {
        public override string RepresentacaoVisual { get { return "E"; } }
        public List<Produto> Produtos = new List<Produto>();

        public Estante(int posicaoX, int posicaoY) : base(posicaoX, posicaoY) { }

        protected override void Mover() {
            while (true) {
                if (ObjetoVinculado == null)
                    throw new Exception("Uma estante não pode se mover se não tiver nenhum objeto vinculado a ela!");

                PosicaoX = ObjetoVinculado.PosicaoX;
                PosicaoY = ObjetoVinculado.PosicaoY;
            }
        }
    }
}
