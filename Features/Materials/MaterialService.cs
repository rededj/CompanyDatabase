using System.Collections.Generic;
using System.Linq;
using BDcource.Models;

namespace BDcource.Features.Materials
{
    public class MaterialService
    {
        private readonly CourceContext _context;

        public MaterialService(CourceContext context)
        {
            _context = context;
        }

        public List<Material> GetAllMaterials()
        {
            return _context.Materials.ToList();
        }

        public void AddMaterial(string name, int numberOf)
        {
            var material = new Material
            {
                Name = name,
                NumberOf = numberOf
            };
            _context.Materials.Add(material);
            _context.SaveChanges();
        }

        public void UpdateMaterial(int materialId, string name, int numberOf)
        {
            var material = _context.Materials.Find(materialId);
            if (material != null)
            {
                material.Name = name;
                material.NumberOf = numberOf;
                _context.SaveChanges();
            }
        }

        public string DeleteMaterial(int materialId)
        {
            var material = _context.Materials.Find(materialId);
            if (material == null) return "Материал не найден";

            bool inNormatives = _context.OperationMaterialsUsages.Any(omu => omu.MaterialId == materialId);
            if (inNormatives) return "Нельзя удалить материал, так как он используется в нормативах операций.";

            bool inIssuances = _context.MaterialIssuances.Any(mi => mi.MaterialId == materialId);
            if (inIssuances) return "Нельзя удалить материал, так как он использовался в выдачах.";

            _context.Materials.Remove(material);
            _context.SaveChanges();
            return null;
        }
    }
}