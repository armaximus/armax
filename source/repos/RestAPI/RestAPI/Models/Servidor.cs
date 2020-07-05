using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Models {
    public class Servidor {
        private static Queue<Pedido> pedidos = new Queue<Pedido>();

        public static void ProcessarPedido(Pedido pedido) {
            // Recebe um JSON com o pedido e busca a estante onde está o produto.

            List<Estante> estantesComOsProdutosDoPedidoDuplicado = new List<Estante>();

            foreach (Produto produto in pedido.Produtos)
                estantesComOsProdutosDoPedidoDuplicado.Add(BancoDeDados.Estantes.FirstOrDefault(r => r.Produtos.Any(p => p.NomeProduto == produto.NomeProduto)));

            List<Estante> estantesComOsProdutosDoPedido = new List<Estante>();
            foreach (Estante e in estantesComOsProdutosDoPedidoDuplicado) {
                if (!estantesComOsProdutosDoPedido.Contains(e))
                    estantesComOsProdutosDoPedido.Add(e);
            }

            // Descobrir quais robôs estão disponíveis
            List<Robo> robosDisponiveis = new List<Robo>();
            foreach (Robo robo in BancoDeDados.Robos) {
                if (robo.EstaDisponivel())
                    robosDisponiveis.Add(robo);
            }


            // Enviar a solicitação para todos os robos disponíveis via API para descobrir qual é o mais próximo do objetivo
            foreach (Estante e in estantesComOsProdutosDoPedido) {
                int menorCaminho = int.MaxValue;
                Robo roboQueVaiPraEstante = null;

                if (robosDisponiveis.Any()) {
                    foreach (Robo robo in robosDisponiveis) {
                        int distancia = robo.VerificarDistancia(e);
                        if (menorCaminho > distancia) {
                            menorCaminho = distancia;
                            roboQueVaiPraEstante = robo;
                        }
                    }
                } else {
                    pedidos.Append(pedido);
                }

                // Agora que se sabe qual é o robô de menor caminho, envia o robô para a estante
                roboQueVaiPraEstante.DefinirObjetivo(e);

                robosDisponiveis.Remove(roboQueVaiPraEstante);
            }

            //Retornar um status 200 (OK) com a mensagem de "Robo de id {ID} está indo buscar o produto"
        }
    }
}
