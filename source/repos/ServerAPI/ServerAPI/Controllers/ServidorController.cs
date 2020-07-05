using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServidorController : ControllerBase {
        // POST: api/Servidor/ProcessarPedido
        [HttpPost("ProcessarPedido")]
        public string PorocessarPedido([FromBody] Pedido pedido)
        {
            Servidor.ProcessarPedido(pedido);
            return "O pedido está sendo processado!";
        }
    }
}
