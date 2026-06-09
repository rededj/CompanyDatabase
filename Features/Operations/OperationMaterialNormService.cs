using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Operations
{
    public class OperationMaterialNormService
    {
        private readonly CourceContext _context;

        public OperationMaterialNormService(CourceContext context)
        {
            _context = context;
        }

        public List<OperationMaterialsUsage> GetMaterialsForOperation(int operationId)
        {
            return _context.OperationMaterialsUsages
                .Include(omu => omu.Material)
                .Where(omu => omu.OperationId == operationId)
                .ToList();
        }

        public string AddMaterial(int operationId, int materialId, short requiredQuantity)
        {
            if (_context.OperationMaterialsUsages.Any(omu => omu.OperationId == operationId && omu.MaterialId == materialId))
                return "Материал уже добавлен к этой операции.";

            var norm = new OperationMaterialsUsage
            {
                OperationId = operationId,
                MaterialId = materialId,
                RequiredQuantity = requiredQuantity
            };
            _context.OperationMaterialsUsages.Add(norm);
            _context.SaveChanges();
            return null;
        }

        public string UpdateMaterial(int operationId, int materialId, short requiredQuantity)
        {
            var norm = _context.OperationMaterialsUsages
                .FirstOrDefault(omu => omu.OperationId == operationId && omu.MaterialId == materialId);
            if (norm == null) return "Норматив не найден.";
            norm.RequiredQuantity = requiredQuantity;
            _context.SaveChanges();
            return null;
        }

        public string DeleteMaterial(int operationId, int materialId)
        {
            var norm = _context.OperationMaterialsUsages
                .FirstOrDefault(omu => omu.OperationId == operationId && omu.MaterialId == materialId);
            if (norm == null) return "Норматив не найден.";
            _context.OperationMaterialsUsages.Remove(norm);
            _context.SaveChanges();
            return null;
        }
    }
}