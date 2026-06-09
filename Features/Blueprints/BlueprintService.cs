using System.Collections.Generic;
using System.Linq;
using BDcource.Models;

namespace BDcource.Features.Blueprints
{
    public class BlueprintService
    {
        private readonly CourceContext _context;

        public BlueprintService(CourceContext context)
        {
            _context = context;
        }

        public List<Blueprint> GetAllBlueprints()
        {
            return _context.Blueprints.ToList();
        }

        public void AddBlueprint(string blueprintNumber, string technicalRequirements)
        {
            var blueprint = new Blueprint
            {
                BlueprintNumber = blueprintNumber,
                TechnicalRequirements = technicalRequirements
            };
            _context.Blueprints.Add(blueprint);
            _context.SaveChanges();
        }

        public void UpdateBlueprint(string blueprintNumber, string technicalRequirements)
        {
            var blueprint = _context.Blueprints.Find(blueprintNumber);
            if (blueprint != null)
            {
                blueprint.TechnicalRequirements = technicalRequirements;
                _context.SaveChanges();
            }
        }

        public string DeleteBlueprint(string blueprintNumber)
        {
            var blueprint = _context.Blueprints.Find(blueprintNumber);
            if (blueprint == null) return "Чертёж не найден";

            bool hasOperations = _context.Operations.Any(o => o.BlueprintNumber == blueprintNumber);
            if (hasOperations) return "Нельзя удалить чертёж, так как он используется в операциях.";

            _context.Blueprints.Remove(blueprint);
            _context.SaveChanges();
            return null;
        }
    }
}