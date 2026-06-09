using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Products
{
    public class ProductService
    {
        private readonly CourceContext _context;

        public ProductService(CourceContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public void AddProduct(string name, decimal cost, byte operationsRequired)
        {
            var product = new Product
            {
                Name = name,
                Cost = cost,
                OperationsRequired = operationsRequired
            };
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(int productId, string name, decimal cost, byte operationsRequired)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                product.Name = name;
                product.Cost = cost;
                product.OperationsRequired = operationsRequired;
                _context.SaveChanges();
            }
        }

        public string DeleteProduct(int productId)
        {
            var product = _context.Products
                .Include(p => p.ProductsOperations)
                .FirstOrDefault(p => p.ProductId == productId);
            if (product == null) return "Продукт не найден";

            bool hasWorkOrders = _context.WorkOrders.Any(wo => wo.ProductId == productId);
            if (hasWorkOrders) return "Нельзя удалить продукт, так как существуют связанные наряды.";

            if (product.ProductsOperations != null && product.ProductsOperations.Any())
                _context.ProductsOperations.RemoveRange(product.ProductsOperations);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return null;
        }
    }
}