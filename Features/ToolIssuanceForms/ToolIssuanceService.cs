using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.ToolIssuanceForms
{
    public class ToolIssuanceService
    {
        private readonly CourceContext _context;

        public ToolIssuanceService(CourceContext context)
        {
            _context = context;
        }

        public List<ToolIssuance> GetAllIssuances()
        {
            return _context.ToolIssuances.ToList();
        }

        public string AddIssuance(int workOrderId, int operationId, string serialNumber, int workshopId, int userId, DateOnly issueDate, DateOnly plannedReturnDate)
        {
            var tool = _context.Tools.Find(serialNumber);
            if (tool == null) return "Инструмент не найден";
            if (tool.CurrentWorkOrderId != null) return "Инструмент уже выдан и не возвращён";

            var issuance = new ToolIssuance
            {
                WorkOrderId = workOrderId,
                OperationId = operationId,
                SerialNumber = serialNumber,
                WorkshopId = workshopId,
                UserId = userId,
                IssueDateTime = issueDate,
                ReturnDateTime = plannedReturnDate,
                ActualReturnDate = null
            };
            tool.CurrentWorkOrderId = workOrderId;

            var toolType = _context.ToolTypes.Find(tool.ToolTypeId);
            if (toolType != null)
            {
                toolType.InStock -= 1;
                toolType.Allocated = (toolType.Allocated ?? 0) + 1;
            }

            _context.ToolIssuances.Add(issuance);
            _context.SaveChanges();
            return null;
        }

        public string UpdateIssuance(int issuanceId, int workOrderId, int operationId, int workshopId, DateOnly plannedReturnDate)
        {
            var issuance = _context.ToolIssuances
                .Include(ti => ti.SerialNumberNavigation)
                .FirstOrDefault(ti => ti.IssuanceId == issuanceId);
            if (issuance == null) return "Выдача не найдена";
            if (issuance.ActualReturnDate != null) return "Нельзя редактировать уже возвращённую выдачу";

            if (issuance.WorkOrderId != workOrderId)
            {
                issuance.SerialNumberNavigation.CurrentWorkOrderId = workOrderId;
            }

            issuance.WorkOrderId = workOrderId;
            issuance.OperationId = operationId;
            issuance.WorkshopId = workshopId;
            issuance.ReturnDateTime = plannedReturnDate;
            _context.SaveChanges();
            return null;
        }

        public string ReturnTool(int issuanceId, DateOnly actualReturnDate)
        {
            var issuance = _context.ToolIssuances
                .Include(ti => ti.SerialNumberNavigation)
                .FirstOrDefault(ti => ti.IssuanceId == issuanceId);
            if (issuance == null) return "Выдача не найдена";
            if (issuance.ActualReturnDate != null) return "Инструмент уже возвращён";
            issuance.ActualReturnDate = actualReturnDate;
            var tool = issuance.SerialNumberNavigation;
            tool.CurrentWorkOrderId = null;
            var toolType = _context.ToolTypes.Find(tool.ToolTypeId);
            if (toolType != null)
            {
                toolType.InStock += 1;
                toolType.Allocated = (toolType.Allocated ?? 0) - 1;
            }

            _context.SaveChanges();
            return null;
        }

        public string DeleteIssuance(int issuanceId)
        {
            var issuance = _context.ToolIssuances
                .Include(ti => ti.SerialNumberNavigation)
                .FirstOrDefault(ti => ti.IssuanceId == issuanceId);
            if (issuance == null) return "Выдача не найдена";

            if (issuance.ActualReturnDate == null)
            {
                var tool = issuance.SerialNumberNavigation;
                tool.CurrentWorkOrderId = null;

                var toolType = _context.ToolTypes.Find(tool.ToolTypeId);
                if (toolType != null)
                {
                    toolType.InStock += 1;
                    toolType.Allocated = (toolType.Allocated ?? 0) - 1;
                }
            }

            _context.ToolIssuances.Remove(issuance);
            _context.SaveChanges();
            return null;
        }
    }
}