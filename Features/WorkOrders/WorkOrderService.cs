using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.WorkOrders
{
    public class WorkOrderService
    {
        private readonly CourceContext _context;

        public WorkOrderService(CourceContext context)
        {
            _context = context;
        }

        public List<WorkOrder> GetAllWorkOrders()
        {
            return _context.WorkOrders
                .Include(wo => wo.Product)
                .ToList();
        }

        public string AddWorkOrder(int productId, DateOnly registrationDate, DateOnly dueDate, int requiredQuantity, bool completed)
        {
            var workOrder = new WorkOrder
            {
                ProductId = productId,
                RegistrationDate = registrationDate,
                DueDate = dueDate,
                RequiredQuantity = requiredQuantity,
                Completed = completed
            };
            _context.WorkOrders.Add(workOrder);
            _context.SaveChanges();
            return null;
        }

        public string UpdateWorkOrder(int workOrderId, int productId, DateOnly registrationDate, DateOnly dueDate, int requiredQuantity, bool completed)
        {
            var workOrder = _context.WorkOrders.Find(workOrderId);
            if (workOrder == null) return "Наряд не найден";

            if (workOrder.Completed && workOrder.Completed == completed)
                return "Нельзя редактировать выполненный наряд. Сначала снимите отметку о выполнении.";

            bool hasMaterialIssuances = _context.MaterialIssuances.Any(mi => mi.WorkOrderId == workOrderId);
            bool hasToolIssuances = _context.ToolIssuances.Any(ti => ti.WorkOrderId == workOrderId);

            if ((hasMaterialIssuances || hasToolIssuances) && workOrder.ProductId != productId)
                return "Нельзя изменить продукт, так как по наряду уже были выдачи.";

            workOrder.ProductId = productId;
            workOrder.RegistrationDate = registrationDate;
            workOrder.DueDate = dueDate;
            workOrder.RequiredQuantity = requiredQuantity;
            workOrder.Completed = completed;

            _context.SaveChanges();
            return null;
        }

        public string DeleteWorkOrder(int workOrderId)
        {
            var workOrder = _context.WorkOrders
                .Include(wo => wo.MaterialIssuances)
                .Include(wo => wo.ToolIssuances)
                .FirstOrDefault(wo => wo.WorkOrderId == workOrderId);
            if (workOrder == null) return "Наряд не найден";

            if (workOrder.MaterialIssuances.Any() || workOrder.ToolIssuances.Any())
                return "Нельзя удалить наряд, так как по нему уже были выдачи материалов или инструментов.";

            if (workOrder.Completed)
                return "Нельзя удалить выполненный наряд.";

            _context.WorkOrders.Remove(workOrder);
            _context.SaveChanges();
            return null;
        }
    }
}