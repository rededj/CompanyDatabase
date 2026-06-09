using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.MaterialIssuanceForms
{
    public class MaterialIssuanceService
    {
        private readonly CourceContext _context;

        public MaterialIssuanceService(CourceContext context)
        {
            _context = context;
        }

        public List<Models.MaterialIssuance> GetAllIssuances()
        {
            return _context.MaterialIssuances.ToList();
        }

        public string AddIssuance(int workOrderId, int operationId, int userId, int materialId, int actualQuantity, DateOnly issueDate)
        {
            var material = _context.Materials.Find(materialId);
            if (material == null) return "Материал не найден";
            if (material.NumberOf < actualQuantity) return "Недостаточно материала на складе";

            var issuance = new Models.MaterialIssuance
            {
                WorkOrderId = workOrderId,
                OperationId = operationId,
                UserId = userId,
                MaterialId = materialId,
                ActualQuantity = actualQuantity,
                IssueDateTime = issueDate.ToDateTime(TimeOnly.MinValue) 
            };
            material.NumberOf -= actualQuantity;
            _context.MaterialIssuances.Add(issuance);
            _context.SaveChanges();
            return null;
        }

        public string UpdateIssuance(int issuanceId, int workOrderId, int operationId, int userId, int actualQuantity, DateOnly issueDate)
        {
            var issuance = _context.MaterialIssuances
                .Include(mi => mi.Material)
                .FirstOrDefault(mi => mi.IssuanceId == issuanceId);
            if (issuance == null) return "Выдача не найдена";

            if (issuance.ActualQuantity != actualQuantity)
            {
                var material = issuance.Material;
                material.NumberOf += issuance.ActualQuantity;
                if (material.NumberOf < actualQuantity)
                {
                    material.NumberOf -= issuance.ActualQuantity;
                    return "Недостаточно материала на складе для нового количества";
                }
                material.NumberOf -= actualQuantity;
            }

            issuance.WorkOrderId = workOrderId;
            issuance.OperationId = operationId;
            issuance.UserId = userId;
            issuance.ActualQuantity = actualQuantity;
            issuance.IssueDateTime = issueDate.ToDateTime(TimeOnly.MinValue); 
            _context.SaveChanges();
            return null;
        }

        public string DeleteIssuance(int issuanceId)
        {
            var issuance = _context.MaterialIssuances.Find(issuanceId);
            if (issuance == null) return "Выдача не найдена";
            _context.MaterialIssuances.Remove(issuance);
            _context.SaveChanges();
            return null;
        }
    }
}