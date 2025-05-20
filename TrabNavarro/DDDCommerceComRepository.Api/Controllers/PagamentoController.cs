using DDDCommerceComRepository.Domain;
using DDDCommerceComRepository.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDCommerceComRepository.Api.Controllers
{
    [ApiController]
    [Route("api/pagamentos")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPedidoRepository _pedidoRepository;

        public PagamentoController(IPagamentoRepository pagamentoRepository, IPedidoRepository pedidoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _pedidoRepository = pedidoRepository;
        }

        // Retorna todos os pagamentos
        [HttpGet]
        public async Task<IActionResult> ObterTodosPagamentos()
        {
            var pagamentos = await _pagamentoRepository.ObterTodosAsync();
            return Ok(pagamentos);
        }

        // Cadastra um pagamento e vincula ao pedido
        [HttpPost]
        public async Task<IActionResult> CriarPagamento([FromBody] Pagamento pagamento)
        {
            if (pagamento == null || pagamento.Valor <= 0)
                return BadRequest("Dados inválidos. O valor do pagamento deve ser maior que zero.");

            var pedido = await _pedidoRepository.ObterPorIdAsync(pagamento.PedidoId);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            var novoPagamento = new Pagamento(pagamento.Valor, pagamento.PedidoId, pagamento.StatusProcessado);
            await _pagamentoRepository.CriarAsync(novoPagamento);

            return CreatedAtAction(nameof(ObterTodosPagamentos), new { id = novoPagamento.PagamentoId }, $"{novoPagamento.PagamentoId};{novoPagamento.PedidoId}");
        }

        // Atualiza o status do pagamento (Processa pagamento)
        [HttpPut("{id}/processar")]
        public async Task<IActionResult> ProcessarPagamento(Guid id,[FromBody] bool statusProcessado)
        {
            var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);
            if (pagamento == null)
                return NotFound("Pagamento não encontrado.");

            pagamento.ProcessarPagamento(statusProcessado);
            await _pagamentoRepository.AtualizarAsync(pagamento);

            return Ok("Pagamento processado com sucesso.");
        }
    }
}