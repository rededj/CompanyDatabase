using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Tools
{
    public class ToolService
    {
        private readonly CourceContext _context;

        public ToolService(CourceContext context)
        {
            _context = context;
        }

        public List<Tool> GetAllTools()
        {
            return _context.Tools
                .Include(t => t.ToolType)
                .Include(t => t.CurrentWorkOrder)
                    .ThenInclude(wo => wo.Product)
                .ToList();
        }

        public string AddTool(string serialNumber, int toolTypeId, DateOnly arrivalDate)
        {
            bool exists = _context.Tools.Any(t => t.SerialNumber == serialNumber);
            if (exists) return "Инструмент с таким серийным номером уже существует.";

            var tool = new Tool
            {
                SerialNumber = serialNumber,
                ToolTypeId = toolTypeId,
                ArrivalDate = arrivalDate,
                CurrentWorkOrderId = null
            };
            _context.Tools.Add(tool);
            _context.SaveChanges();
            return null;
        }

        public string UpdateTool(string serialNumber, DateOnly arrivalDate)
        {
            var tool = _context.Tools.Find(serialNumber);
            if (tool == null) return "Инструмент не найден";
            tool.ArrivalDate = arrivalDate;
            _context.SaveChanges();
            return null;
        }

        public string DeleteTool(string serialNumber)
        {
            var tool = _context.Tools
                .Include(t => t.ToolType)
                .FirstOrDefault(t => t.SerialNumber == serialNumber);
            if (tool == null) return "Инструмент не найден";

            bool hasIssuances = _context.ToolIssuances.Any(ti => ti.SerialNumber == serialNumber);
            if (hasIssuances) return "Нельзя удалить инструмент, так как он числится в выдачах.";

            if (tool.CurrentWorkOrderId != null) return "Инструмент сейчас выдан. Сначала верните его.";

            _context.Tools.Remove(tool);
            _context.SaveChanges();
            return null;
        }
    }
}