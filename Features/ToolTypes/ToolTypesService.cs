using System.Collections.Generic;
using System.Linq;
using BDcource.Models;

namespace BDcource.Features.ToolTypes
{
    public class ToolTypeService
    {
        private readonly CourceContext _context;

        public ToolTypeService(CourceContext context)
        {
            _context = context;
        }

        public List<ToolType> GetAllToolTypes()
        {
            return _context.ToolTypes.ToList();
        }

        public string AddToolType(string name, string description)
        {
            var toolType = new ToolType
            {
                Name = name,
                Description = description,
                InStock = 0,
                Allocated = 0
            };
            _context.ToolTypes.Add(toolType);
            _context.SaveChanges();
            return null;
        }

        public string UpdateToolType(int toolTypeId, string name, string description)
        {
            var toolType = _context.ToolTypes.Find(toolTypeId);
            if (toolType == null) return "Тип не найден";
            toolType.Name = name;
            toolType.Description = description;
            _context.SaveChanges();
            return null;
        }

        public string DeleteToolType(int toolTypeId)
        {
            var toolType = _context.ToolTypes.Find(toolTypeId);
            if (toolType == null) return "Тип не найден";

            bool inNormatives = _context.OperationToolsUsages.Any(otu => otu.ToolTypeId == toolTypeId);
            if (inNormatives) return "Нельзя удалить тип инструмента, так как он используется в нормативах операций.";

            bool hasTools = _context.Tools.Any(t => t.ToolTypeId == toolTypeId);
            if (hasTools) return "Нельзя удалить тип, так как есть инструменты этого типа.";

            _context.ToolTypes.Remove(toolType);
            _context.SaveChanges();
            return null;
        }
    }
}