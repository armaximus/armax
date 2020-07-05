using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ServerAPI.Models {
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
            List<Objeto> robosDisponiveis = new List<Objeto>();
            foreach (Objeto robo in BancoDeDados.Robos) {
                string retorno = RealizarRequisicao($"{robo.ID}/EstaDisponivel", "get");
                if (bool.Parse(retorno))
                    robosDisponiveis.Add(robo);
            }


            // Enviar a solicitação para todos os robos disponíveis via API para descobrir qual é o mais próximo do objetivo
            foreach (Estante e in estantesComOsProdutosDoPedido) {
                int menorCaminho = int.MaxValue;
                Robo roboQueVaiPraEstante = null;

                if (robosDisponiveis.Any()) {
                    foreach (Robo robo in robosDisponiveis) {
                        int distancia = int.Parse(RealizarRequisicao($"{robo.ID}/EstaDisponivel", "post"));

                        if (menorCaminho > distancia) {
                            menorCaminho = distancia;
                            roboQueVaiPraEstante = robo;
                        }
                    }
                } else {
                    pedidos.Append(pedido);
                }

                // Agora que se sabe qual é o robô de menor caminho, envia o robô para a estante
                RealizarRequisicao($"{roboQueVaiPraEstante.ID}/DefinirObjetivo", "post", JsonConvert.SerializeObject(e));

                robosDisponiveis.Remove(roboQueVaiPraEstante);
            }

            //Retornar um status 200 (OK) com a mensagem de "Robo de id {ID} está indo buscar o produto"
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalLink">o que vem depois de api/Robo/</param>
        /// <param name="tipoMetodo">get, post, pathc, delete, etc.</param>
        private static string RealizarRequisicao(string finalLink, string tipoMetodo, string body = "") {
            HttpWebRequest requisicao = WebRequest.CreateHttp("http://localhost:5000/api/Robo/" + finalLink);
            requisicao.Method = tipoMetodo.ToUpper();

            using (WebResponse resposta = requisicao.GetResponse()) {
                using (Stream streamDados = resposta.GetResponseStream()) {
                    using (StreamReader reader = new StreamReader(streamDados)) {
                        string stringResposta = reader.ReadToEnd();
                        Console.WriteLine(stringResposta);
                        return stringResposta;
                    }
                }
            }
        }
    }
}
