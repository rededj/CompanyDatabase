using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Operations
{
    public class OperationService
    {
        private readonly CourceContext _context;

        public OperationService(CourceContext context)
        {
            _context = context;
        }

        public List<Operation> GetAllOperations()
        {
            return _context.Operations
                .Include(o => o.Workshop)
                .Include(o => o.BlueprintNumberNavigation)
                .ToList();
        }

        public void AddOperation(int workshopId, string description, TimeOnly averageDuration, string blueprintNumber)
        {
            var operation = new Operation
            {
                WorkshopId = workshopId,
                Description = description,
                AverageDuration = averageDuration,
                BlueprintNumber = blueprintNumber
            };
            _context.Operations.Add(operation);
            _context.SaveChanges();
        }

        public void UpdateOperation(int operationId, int workshopId, string description, TimeOnly averageDuration, string blueprintNumber)
        {
            var operation = _context.Operations.Find(operationId);
            if (operation != null)
            {
                operation.WorkshopId = workshopId;
                operation.Description = description;
                operation.AverageDuration = averageDuration;
                operation.BlueprintNumber = blueprintNumber;
                _context.SaveChanges();
            }
        }

        public string DeleteOperation(int operationId)
        {
            var operation = _context.Operations
                .Include(o => o.OperationMaterialsUsages)
                .Include(o => o.OperationToolsUsages)
                .FirstOrDefault(o => o.OperationId == operationId);
            if (operation == null) return "Операция не найдена";

            bool inProducts = _context.ProductsOperations.Any(po => po.OperationId == operationId);
            if (inProducts) return "Операция используется в составе продуктов. Сначала удалите её из продуктов.";

            bool inMaterialIssuance = _context.MaterialIssuances.Any(mi => mi.OperationId == operationId);
            if (inMaterialIssuance) return "Операция уже использовалась в выдаче материалов.";

            bool inToolIssuance = _context.ToolIssuances.Any(ti => ti.OperationId == operationId);
            if (inToolIssuance) return "Операция уже использовалась в выдаче инструментов.";

            if (operation.OperationMaterialsUsages.Any())
                _context.OperationMaterialsUsages.RemoveRange(operation.OperationMaterialsUsages);
            if (operation.OperationToolsUsages.Any())
                _context.OperationToolsUsages.RemoveRange(operation.OperationToolsUsages);

            _context.Operations.Remove(operation);

            _context.SaveChanges();
            return null;
        }
    }
}