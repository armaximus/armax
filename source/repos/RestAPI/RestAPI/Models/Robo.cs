using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace RestAPI.Models {
    public class Robo : Objeto {
        public override string RepresentacaoVisual { get { return "R"; } }
        private Objeto Objetivo = null;
        private Direcao DirecaoAtual = Direcao.Norte;
        private const int sleepTime = 1000;

        List<int[]> locaisPassados = new List<int[]>();

        private Thread SeMover;

        public Robo(int posicaoX, int posicaoY) : base(posicaoX, posicaoY) { }

        public void DefinirObjetivo(Objeto objetivo) {
            if (Objetivo != null)
                throw new Exception($"O robo de ID {ID} já possui um objetivo");

            Objetivo = objetivo;

            SeMover = new Thread(Mover);
            SeMover.Start();
        }

        protected override void Mover() {
            IrAteObjetivo();
            VincularAOutroObjeto(Objetivo);
            VoltarComObjetoParaOGPontoDeEntrega();
            DevolverObjetoParaLocalInicial();
            DesvincularObjeto();
            VoltarParaOLocalInicial();
        }

        public int VerificarDistancia(Objeto objetivo) {
            // Calcular a rota até o objeto
            // Lembrando que na ida o robô pode passar por debaixo das estantes
            int quantidadeQuadradosAPercorrer = 0;

            if (PosicaoX < objetivo.PosicaoX)
                quantidadeQuadradosAPercorrer += objetivo.PosicaoX - PosicaoX;
            else
                quantidadeQuadradosAPercorrer += PosicaoX - objetivo.PosicaoX;

            if (PosicaoY < objetivo.PosicaoY)
                quantidadeQuadradosAPercorrer += objetivo.PosicaoY - PosicaoY;
            else
                quantidadeQuadradosAPercorrer += PosicaoY - objetivo.PosicaoY;

            return quantidadeQuadradosAPercorrer;
        }

        public bool EstaDisponivel() {
            return Objetivo == null;
        }

        private void IrAteObjetivo(bool utilizarValoresIniciais = false, bool utilizarCorredores = false) {
            int posicaoX = utilizarValoresIniciais ? Objetivo.PosicaoXInicial : Objetivo.PosicaoX;
            int posicaoY = utilizarValoresIniciais ? Objetivo.PosicaoYInicial : Objetivo.PosicaoY;

            while (posicaoX != PosicaoX || posicaoY != PosicaoY) {
                AjustarDirecao(posicaoX, posicaoY);
                VerificarColisao(utilizarCorredores, posicaoX, posicaoY);
                VerificarLoop(posicaoX, posicaoY);

                Andar();
                Thread.Sleep(sleepTime);
            }

            locaisPassados = new List<int[]>();
        }

        private void AjustarDirecao(int posicaoXObjetivo, int posicaoYObjetivo) {
            if (PosicaoX != posicaoXObjetivo) {
                if (PosicaoX > posicaoXObjetivo)
                    DirecaoAtual = Direcao.Norte;
                else
                    DirecaoAtual = Direcao.Sul;
            } else {
                if (PosicaoY > posicaoYObjetivo)
                    DirecaoAtual = Direcao.Leste;
                else
                    DirecaoAtual = Direcao.Oeste;
            }
        }

        private void Andar() {
            switch (DirecaoAtual) {
                case Direcao.Norte:
                    PosicaoX--;
                    break;
                case Direcao.Leste:
                    PosicaoY--;
                    break;
                case Direcao.Sul:
                    PosicaoX++;
                    break;
                case Direcao.Oeste:
                    PosicaoY++;
                    break;
            }
        }

        private void VoltarComObjetoParaOGPontoDeEntrega() {
            Objetivo = BancoDeDados.PontoDeEntrega;
            IrAteObjetivo(utilizarCorredores: true);
        }

        private void DevolverObjetoParaLocalInicial() {
            Objetivo = ObjetoVinculado;
            IrAteObjetivo(true, true);
        }

        private void VoltarParaOLocalInicial() {
            Objetivo = this;
            IrAteObjetivo(true);
            Objetivo = null;
        }

        private void VerificarColisao(bool verificarEstantesTambem, int posicaoXObjetivo, int posicaoYObjetivo) {
            bool existeconflito = true;

            List<Objeto> objetosParaVerificar = verificarEstantesTambem ? BancoDeDados.TodosOsObjetos : BancoDeDados.TodosOsObjetos.Where(r => r.GetType() != typeof(Estante)).ToList();
            List<Direcao> direcoesTentadas = new List<Direcao>();
            objetosParaVerificar = objetosParaVerificar.Where(r => r.GetType() != typeof(PontoEntrega)).ToList();

            while (existeconflito) {
                switch (DirecaoAtual) {
                    case Direcao.Norte:
                        if (objetosParaVerificar.Any(r => r.PosicaoX == PosicaoX - 1 && r.PosicaoY == PosicaoY))
                            MudarDirecaoParaLadoMaisPerto(Orientacao.Vertical, posicaoXObjetivo, posicaoYObjetivo, direcoesTentadas);
                        else
                            existeconflito = false;
                        break;
                    case Direcao.Leste:
                        if (objetosParaVerificar.Any(r => r.PosicaoY == PosicaoY - 1 && r.PosicaoX == PosicaoX))
                            MudarDirecaoParaLadoMaisPerto(Orientacao.Horizontal, posicaoXObjetivo, posicaoYObjetivo, direcoesTentadas);
                        else
                            existeconflito = false;
                        break;
                    case Direcao.Sul:
                        if (objetosParaVerificar.Any(r => r.PosicaoX == PosicaoX + 1 && r.PosicaoY == PosicaoY))
                            MudarDirecaoParaLadoMaisPerto(Orientacao.Vertical, posicaoXObjetivo, posicaoYObjetivo, direcoesTentadas);
                        else
                            existeconflito = false;
                        break;
                    case Direcao.Oeste:
                        if (objetosParaVerificar.Any(r => r.PosicaoY == PosicaoY + 1 && r.PosicaoX == PosicaoX))
                            MudarDirecaoParaLadoMaisPerto(Orientacao.Horizontal, posicaoXObjetivo, posicaoYObjetivo, direcoesTentadas);
                        else
                            existeconflito = false;
                        break;
                }
                direcoesTentadas.Add(DirecaoAtual);
            }
        }

        private void VerificarLoop(int posicaoX, int posicaoY) {
            int[] localAtual = new int[] { PosicaoX, PosicaoY };

            if (!locaisPassados.Any(r => r[0] == localAtual[0] && r[1] == localAtual[1]))
                locaisPassados.Add(localAtual);
            else {
                switch (DirecaoAtual) {
                    case Direcao.Norte:
                        MudarDirecao(Direcao.Sul);
                        break;
                    case Direcao.Sul:
                        MudarDirecao(Direcao.Norte);
                        break;
                    case Direcao.Leste:
                        MudarDirecao(Direcao.Oeste);
                        break;
                    case Direcao.Oeste:
                        MudarDirecao(Direcao.Leste);
                        break;
                }
            }
        }

        private void MudarDirecaoParaLadoMaisPerto(Orientacao orientacao, int posicaoXObjetivo, int posicaoYObjetivo, List<Direcao> direcoesTentadas) {
            if (orientacao == Orientacao.Horizontal) {
                if (PosicaoX > posicaoXObjetivo) {
                    if (!direcoesTentadas.Contains(Direcao.Norte))
                        MudarDirecao(Direcao.Norte);
                    else
                        MudarDirecao(Direcao.Sul);
                } else {
                    if (!direcoesTentadas.Contains(Direcao.Sul))
                        MudarDirecao(Direcao.Sul);
                    else
                        MudarDirecao(Direcao.Norte);
                }
            } else {
                if (PosicaoY > posicaoYObjetivo) {
                    if (!direcoesTentadas.Contains(Direcao.Leste))
                        MudarDirecao(Direcao.Leste);
                    else
                        MudarDirecao(Direcao.Oeste);
                } else {
                    if (!direcoesTentadas.Contains(Direcao.Oeste))
                        MudarDirecao(Direcao.Oeste);
                    else
                        MudarDirecao(Direcao.Leste);
                }
            }
        }

        private void MudarDirecao(Direcao novaDirecao) {
            // TODO: Virar o robô visualmente

            DirecaoAtual = novaDirecao;
        }
    }
}
