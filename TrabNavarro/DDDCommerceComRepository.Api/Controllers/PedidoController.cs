using DDDCommerceComRepository.Domain;
using DDDCommerceComRepository.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DDDCommerceComRepository.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _pedidoRepository.ObterTodosAsync());

       

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            if (pedido == null)
                return BadRequest("Dados inválidos.");

            var novoPedido = new Pedido(pedido.NomeCliente, pedido.DespachadoStatus);
            await _pedidoRepository.CriarAsync(novoPedido);

            return CreatedAtAction(nameof(Get), new { id = novoPedido.Id }, novoPedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] bool despachadoStatus)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null) return NotFound("Pedido não encontrado.");

            pedido.AtualizarPedido(despachadoStatus);
            await _pedidoRepository.AtualizarAsync(pedido);

            return NoContent();
        }
    }
}