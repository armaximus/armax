using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServerAPI.Models {
    public class Produto {
        [JsonProperty("id")]
        public long ID { get; private set; }
        [JsonProperty("nome")]
        public string NomeProduto { get; private set; }

        public Produto(string nomeProduto) {
            NomeProduto = nomeProduto;
            ID = Utils.GerarNovoID("P");
        }
    }
}
