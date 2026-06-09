using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.ProductsOperations
{
    public class ProductsOperationsService
    {
        private readonly CourceContext _context;

        public ProductsOperationsService(CourceContext context)
        {
            _context = context;
        }

        public List<Operation> GetOperationsForProduct(int productId)
        {
            return _context.ProductsOperations
                .Where(po => po.ProductId == productId)
                .Include(po => po.Operation)          
                    .ThenInclude(o => o.Workshop)     
                .Select(po => po.Operation)           
                .ToList();
        }

        public List<Product> GetProductsForOperation(int operationId)
        {
            return _context.ProductsOperations
                .Where(po => po.OperationId == operationId)
                .Select(po => po.Product)
                .ToList();
        }

        public string AddLink(int productId, int operationId)
        {
            if (_context.ProductsOperations.Any(po => po.ProductId == productId && po.OperationId == operationId))
                return "Связь уже существует.";

            _context.ProductsOperations.Add(new ProductsOperation
            {
                ProductId = productId,
                OperationId = operationId
            });
            _context.SaveChanges();
            return null;
        }

        public string RemoveLink(int productId, int operationId)
        {
            var link = _context.ProductsOperations.FirstOrDefault(po => po.ProductId == productId && po.OperationId == operationId);
            if (link == null) return "Связь не найдена.";

            _context.ProductsOperations.Remove(link);
            _context.SaveChanges();
            return null;
        }
    }
}