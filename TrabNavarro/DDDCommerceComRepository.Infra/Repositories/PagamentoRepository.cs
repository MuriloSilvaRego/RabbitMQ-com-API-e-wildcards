using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDCommerceComRepository.Domain;
using DDDCommerceComRepository.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DDDCommerceComRepository.Infra.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly SqlContext _context;

        public PagamentoRepository(SqlContext context)
        {
            _context = context;
        }

        public async Task<Pagamento> ObterPorIdAsync(Guid id) =>
            await _context.Pagamentos.FindAsync(id);

        public async Task<IEnumerable<Pagamento>> ObterTodosAsync() =>
            await _context.Pagamentos.ToListAsync();

        public async Task CriarAsync(Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();
        }
    }
}