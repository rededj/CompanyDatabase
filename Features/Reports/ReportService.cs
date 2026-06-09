using System;
using System.Collections.Generic;
using System.Linq;
using BDcource.Models;
using Microsoft.EntityFrameworkCore;

namespace BDcource.Features.Reports
{
    public class ReportService
    {
        private readonly CourceContext _context;

        public ReportService(CourceContext context)
        {
            _context = context;
        }

        public List<OperationMaterialReport> GetOperationsWithMaterials(int workshopId)
        {
            var query = from op in _context.Operations
                        where op.WorkshopId == workshopId
                        join norm in _context.OperationMaterialsUsages on op.OperationId equals norm.OperationId into norms
                        from norm in norms.DefaultIfEmpty()
                        join mat in _context.Materials on norm.MaterialId equals mat.MaterialId into mats
                        from mat in mats.DefaultIfEmpty()
                        select new OperationMaterialReport
                        {
                            OperationDescription = op.Description,
                            MaterialName = mat != null ? mat.Name : null,
                            RequiredQuantity = norm != null ? norm.RequiredQuantity : (short?)null
                        };
            return query.ToList();
        }

        public List<ToolTypeUsageReport> GetToolsSortedByUsage()
        {
            var query = from ti in _context.ToolIssuances
                        join t in _context.Tools on ti.SerialNumber equals t.SerialNumber
                        group ti by t.ToolTypeId into g
                        select new ToolTypeUsageReport
                        {
                            ToolTypeName = g.First().SerialNumberNavigation.ToolType.Name,
                            UsageCount = g.Count()
                        };
            return query.OrderByDescending(t => t.UsageCount).ToList();
        }

        public List<WorkOrderToolReport> GetWorkOrdersWithTools(DateOnly startDate, DateOnly endDate)
        {
            var query = from wo in _context.WorkOrders
                        where wo.RegistrationDate >= startDate && wo.RegistrationDate <= endDate
                        join ti in _context.ToolIssuances on wo.WorkOrderId equals ti.WorkOrderId into tools
                        from ti in tools.DefaultIfEmpty()
                        select new WorkOrderToolReport
                        {
                            ProductName = wo.Product.Name,
                            RegistrationDate = wo.RegistrationDate,
                            DueDate = wo.DueDate,
                            RequiredQuantity = wo.RequiredQuantity,
                            Completed = wo.Completed,
                            ToolSerialNumber = ti != null ? ti.SerialNumber : null,
                            ToolTypeName = ti != null ? ti.SerialNumberNavigation.ToolType.Name : null,
                            IssueDate = ti != null ? ti.IssueDateTime : (DateOnly?)null,
                            ReturnDate = ti != null ? ti.ReturnDateTime : (DateOnly?)null
                        };
            return query.ToList();
        }

        public List<MaterialUsageReport> GetMaterialsSortedByUsage()
        {
            var query = from mi in _context.MaterialIssuances
                        group mi by mi.MaterialId into g
                        select new MaterialUsageReport
                        {
                            MaterialName = g.First().Material.Name,
                            TotalQuantity = g.Sum(mi => mi.ActualQuantity)
                        };
            return query.OrderByDescending(m => m.TotalQuantity).ToList();
        }

        public List<ProductToolReport> GetProductsWithTools()
        {
            var query = from po in _context.ProductsOperations
                        join op in _context.Operations on po.OperationId equals op.OperationId
                        join otu in _context.OperationToolsUsages on op.OperationId equals otu.OperationId
                        join tt in _context.ToolTypes on otu.ToolTypeId equals tt.ToolTypeId
                        join p in _context.Products on po.ProductId equals p.ProductId
                        select new ProductToolReport
                        {
                            ProductName = p.Name,
                            ToolTypeName = tt.Name,
                            QuantityInUse = otu.QuantityInUse
                        };
            var grouped = from r in query
                          group r by new { r.ProductName, r.ToolTypeName } into g
                          select new ProductToolReport
                          {
                              ProductName = g.Key.ProductName,
                              ToolTypeName = g.Key.ToolTypeName,
                              QuantityInUse = (short)g.Sum(x => x.QuantityInUse)
                          };
            return grouped.OrderBy(r => r.ProductName).ThenBy(r => r.ToolTypeName).ToList();
        }

        public List<ProductionReportItem> GetProductionReport()
        {
            var productQuantities = _context.WorkOrders
                .Where(wo => wo.Completed)
                .GroupBy(wo => wo.ProductId)
                .Select(g => new { ProductId = g.Key, Total = g.Sum(wo => wo.RequiredQuantity) })
                .ToList(); 

            var productWorkshops = from po in _context.ProductsOperations
                                   join o in _context.Operations on po.OperationId equals o.OperationId
                                   select new { po.ProductId, o.WorkshopId };

            var productIds = productQuantities.Select(pq => pq.ProductId).ToList();
            var distinctProductWorkshops = productWorkshops
                .Where(pw => productIds.Contains(pw.ProductId))
                .Select(pw => new { pw.ProductId, pw.WorkshopId })
                .Distinct()
                .ToList(); 

            var report = from pw in distinctProductWorkshops
                         join w in _context.Workshops on pw.WorkshopId equals w.WorkshopId
                         join p in _context.Products on pw.ProductId equals p.ProductId
                         join pq in productQuantities on pw.ProductId equals pq.ProductId
                         select new ProductionReportItem
                         {
                             WorkshopName = w.WorkshopName,
                             ProductName = p.Name,
                             TotalQuantity = pq.Total
                         };

            return report.OrderBy(r => r.WorkshopName).ThenBy(r => r.ProductName).ToList();
        }
    }


    public class OperationMaterialReport
    {
        public string OperationDescription { get; set; }
        public string MaterialName { get; set; }
        public short? RequiredQuantity { get; set; }
    }

    public class ToolTypeUsageReport
    {
        public string ToolTypeName { get; set; }
        public int UsageCount { get; set; }
    }

    public class WorkOrderToolReport
    {
        public string ProductName { get; set; }
        public DateOnly RegistrationDate { get; set; }
        public DateOnly DueDate { get; set; }
        public int RequiredQuantity { get; set; }
        public bool Completed { get; set; }
        public string ToolSerialNumber { get; set; }
        public string ToolTypeName { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
    }

    public class MaterialUsageReport
    {
        public string MaterialName { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class ProductToolReport
    {
        public string ProductName { get; set; }
        public string ToolTypeName { get; set; }
        public short QuantityInUse { get; set; }
    }

    public class ProductionReportItem
    {
        public string WorkshopName { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
    }
}