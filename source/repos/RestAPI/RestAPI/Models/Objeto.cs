using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestAPI.Models {
    public abstract class Objeto {
        public abstract string RepresentacaoVisual { get; }
        [JsonProperty("id")]
        public long ID { get; protected set; }
        public Objeto ObjetoVinculado { get; private set; }

        [JsonProperty("posicaoX")]
        public int PosicaoX { get; protected set; }
        [JsonProperty("posicaoY")]
        public int PosicaoY { get; protected set; }
        public int PosicaoXInicial { get; protected set; }
        public int PosicaoYInicial { get; protected set; }

        protected abstract void Mover();

        private Thread moverObjetoVinculado;

        public Objeto(int posicaoX, int posicaoY) {
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
            PosicaoXInicial = posicaoX;
            PosicaoYInicial = posicaoY;
            ID = Utils.GerarNovoID(RepresentacaoVisual);
        }

        protected void VincularAOutroObjeto(Objeto objeto) {
            if (ObjetoVinculado != null)
                throw new Exception($"O objeto de representação {RepresentacaoVisual} e ID {ID} já está vinculado ao objeto de representação {ObjetoVinculado.RepresentacaoVisual} e ID {ObjetoVinculado.ID}!");

            if (objeto.PosicaoX != PosicaoX || objeto.PosicaoY != PosicaoY)
                throw new Exception("Os dois objetos devem estar na mesma posicao para poderem serem vinculados!");

            ObjetoVinculado = objeto;
            if (objeto.ObjetoVinculado == null) {
                objeto.VincularAOutroObjeto(this);
                moverObjetoVinculado = new Thread(ObjetoVinculado.Mover);
                moverObjetoVinculado.Start();
            }
        }

        protected void DesvincularObjeto() {
            if (ObjetoVinculado == null)
                throw new Exception($"O objeto de representação {RepresentacaoVisual} e ID {ID} não está vinculado a nenhum objeto, portanto não pode ser desvinculado!");

            moverObjetoVinculado.Abort();
            ObjetoVinculado.ObjetoVinculado = null;
            ObjetoVinculado = null;
        }

        public enum Direcao {
            Norte,
            Leste,
            Sul,
            Oeste
        }

        public enum Orientacao {
            Vertical,
            Horizontal
        }
    }
}
