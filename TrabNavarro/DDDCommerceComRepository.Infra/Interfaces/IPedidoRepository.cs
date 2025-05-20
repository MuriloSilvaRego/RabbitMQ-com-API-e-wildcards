using DDDCommerceComRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCommerceComRepository.Infra.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Pedido>> ObterTodosAsync();
        Task CriarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
    }
}