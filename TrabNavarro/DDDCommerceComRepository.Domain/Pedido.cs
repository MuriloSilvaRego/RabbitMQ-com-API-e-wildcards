using DDDCommerceComRepository.Domain;
using System.Text.Json.Serialization;
using System;

namespace DDDCommerceComRepository.Domain
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public string NomeCliente { get; private set; }
        public bool DespachadoStatus { get; private set; }

        public Pedido(string nomeCliente, bool despachadoStatus)
        {
            if (string.IsNullOrWhiteSpace(nomeCliente))
                throw new ArgumentException("O nome do cliente não pode estar vazio.");

            Id = Guid.NewGuid();
            NomeCliente = nomeCliente;
            DespachadoStatus = despachadoStatus;
        }

        public void AtualizarPedido(bool despachadoStatus)
        {
            DespachadoStatus = despachadoStatus;
        }
    }
}



