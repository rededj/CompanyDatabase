using System;
using System.Collections.Generic;
using System.Linq;
using BDcource.Models;
using Microsoft.EntityFrameworkCore;


namespace BDcource.Features.Reports
{
    public class MonthlyReportItem
    {
        public string ProductName { get; set; }
        public string IndicatorName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }

    public class MonthlyReportService
    {
        private readonly CourceContext _context;

        public MonthlyReportService(CourceContext context)
        {
            _context = context;
        }

        public List<MonthlyReportItem> GetMonthlyReport(int month, int year)
        {
            var result = new List<MonthlyReportItem>();

            var workOrders = _context.WorkOrders
                .Where(wo => wo.Completed && wo.DueDate.Year == year && wo.DueDate.Month == month)
                .GroupBy(wo => wo.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(wo => wo.RequiredQuantity)
                })
                .ToList();

            foreach (var wo in workOrders)
            {
                var product = _context.Products.Find(wo.ProductId);
                string productName = product?.Name ?? "Неизвестно";

                result.Add(new MonthlyReportItem
                {
                    ProductName = productName,
                    IndicatorName = "Выполнено нарядов",
                    Quantity = wo.OrderCount,
                    Unit = "шт"
                });
                result.Add(new MonthlyReportItem
                {
                    ProductName = productName,
                    IndicatorName = "Произведено продукции",
                    Quantity = wo.TotalQuantity,
                    Unit = "шт"
                });
            }

            var materialIssuances = _context.MaterialIssuances
                .Where(mi => mi.IssueDateTime.Year == year && mi.IssueDateTime.Month == month)
                .Join(_context.WorkOrders, mi => mi.WorkOrderId, wo => wo.WorkOrderId, (mi, wo) => new { mi, wo })
                .GroupBy(x => new { x.wo.ProductId, x.mi.MaterialId })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    MaterialId = g.Key.MaterialId,
                    TotalQty = g.Sum(x => x.mi.ActualQuantity)
                })
                .ToList();

            foreach (var mat in materialIssuances)
            {
                var product = _context.Products.Find(mat.ProductId);
                var material = _context.Materials.Find(mat.MaterialId);
                if (product == null || material == null) continue;

                result.Add(new MonthlyReportItem
                {
                    ProductName = product.Name,
                    IndicatorName = $"Материал: {material.Name}",
                    Quantity = mat.TotalQty,
                    Unit = "ед"
                });
            }

            var toolIssuances = _context.ToolIssuances
                .Where(ti => ti.IssueDateTime.Year == year && ti.IssueDateTime.Month == month)
                .Join(_context.WorkOrders, ti => ti.WorkOrderId, wo => wo.WorkOrderId, (ti, wo) => new { ti, wo })
                .GroupBy(x => new { x.wo.ProductId, x.ti.SerialNumberNavigation.ToolTypeId })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ToolTypeId = g.Key.ToolTypeId,
                    UsageCount = g.Count()
                })
                .ToList();

            foreach (var tool in toolIssuances)
            {
                var product = _context.Products.Find(tool.ProductId);
                var toolType = _context.ToolTypes.Find(tool.ToolTypeId);
                if (product == null || toolType == null) continue;

                result.Add(new MonthlyReportItem
                {
                    ProductName = product.Name,
                    IndicatorName = $"Оборудование: {toolType.Name}",
                    Quantity = tool.UsageCount,
                    Unit = "выдач"
                });
            }

            return result.OrderBy(r => r.ProductName)
                         .ThenBy(r => r.IndicatorName)
                         .ToList();
        }
    }

}