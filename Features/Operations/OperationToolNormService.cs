using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Operations
{
    public class OperationToolNormService
    {
        private readonly CourceContext _context;

        public OperationToolNormService(CourceContext context)
        {
            _context = context;
        }

        public List<OperationToolsUsage> GetToolsForOperation(int operationId)
        {
            return _context.OperationToolsUsages
                .Include(otu => otu.ToolType)
                .Where(otu => otu.OperationId == operationId)
                .ToList();
        }

        public string AddTool(int operationId, int toolTypeId, short quantityInUse)
        {
            if (_context.OperationToolsUsages.Any(otu => otu.OperationId == operationId && otu.ToolTypeId == toolTypeId))
                return "Тип инструмента уже добавлен к этой операции.";

            var norm = new OperationToolsUsage
            {
                OperationId = operationId,
                ToolTypeId = toolTypeId,
                QuantityInUse = quantityInUse
            };
            _context.OperationToolsUsages.Add(norm);
            _context.SaveChanges();
            return null;
        }

        public string UpdateTool(int operationId, int toolTypeId, short quantityInUse)
        {
            var norm = _context.OperationToolsUsages
                .FirstOrDefault(otu => otu.OperationId == operationId && otu.ToolTypeId == toolTypeId);
            if (norm == null) return "Норматив не найден.";
            norm.QuantityInUse = quantityInUse;
            _context.SaveChanges();
            return null;
        }

        public string DeleteTool(int operationId, int toolTypeId)
        {
            var norm = _context.OperationToolsUsages
                .FirstOrDefault(otu => otu.OperationId == operationId && otu.ToolTypeId == toolTypeId);
            if (norm == null) return "Норматив не найден.";
            _context.OperationToolsUsages.Remove(norm);
            _context.SaveChanges();
            return null;
        }
    }
}