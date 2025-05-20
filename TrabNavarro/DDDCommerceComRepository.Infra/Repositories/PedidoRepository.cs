using DDDCommerceComRepository.Domain;
using DDDCommerceComRepository.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCommerceComRepository.Infra.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly SqlContext _context;

        public PedidoRepository(SqlContext context)
        {
            _context = context;
        }

        public async Task<Pedido> ObterPorIdAsync(Guid id) =>
            await _context.Pedidos.FindAsync(id);

        public async Task<IEnumerable<Pedido>> ObterTodosAsync() =>
            await _context.Pedidos.ToListAsync();

        public async Task CriarAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }
}
