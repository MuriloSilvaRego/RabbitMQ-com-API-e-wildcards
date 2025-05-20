using System;
using System.Text.Json.Serialization;

namespace DDDCommerceComRepository.Domain
{
    public class Pagamento
    {

        [JsonPropertyOrder(2)]
        public Guid PedidoId { get; private set; }

        [JsonPropertyOrder(3)]
        public decimal Valor { get; private set; }

        [JsonPropertyOrder(4)]
        public bool StatusProcessado { get; private set; } 

        [JsonPropertyOrder(1)]
        public Guid PagamentoId { get; private set; }

       // private Pagamento() { }

        public Pagamento(decimal valor, Guid pedidoId, bool statusProcessado )
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do pagamento deve ser maior que zero.");

            PagamentoId = Guid.NewGuid();
            Valor = valor;
            StatusProcessado = statusProcessado;
            PedidoId = pedidoId; 
        }

        public void ProcessarPagamento(bool statusProcessado)
        {
            StatusProcessado = statusProcessado;
        }
    }
}