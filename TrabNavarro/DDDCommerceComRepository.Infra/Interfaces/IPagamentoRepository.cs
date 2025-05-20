using DDDCommerceComRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCommerceComRepository.Infra.Interfaces
{

    public interface IPagamentoRepository
    {
        Task<Pagamento> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Pagamento>> ObterTodosAsync();
        Task CriarAsync(Pagamento pagamento);
        Task AtualizarAsync(Pagamento pagamento);
    }
}