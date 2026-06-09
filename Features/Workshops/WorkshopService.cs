using System.Collections.Generic;
using System.Linq;
using BDcource.Models;

namespace BDcource.Features.Workshops
{
    public class WorkshopService
    {
        private readonly CourceContext _context;

        public WorkshopService(CourceContext context)
        {
            _context = context;
        }

        public List<Workshop> GetAllWorkshops()
        {
            return _context.Workshops.ToList();
        }

        public string AddWorkshop(string name, string address)
        {
            var workshop = new Workshop
            {
                WorkshopName = name,
                Adress = address
            };
            _context.Workshops.Add(workshop);
            _context.SaveChanges();
            return null;
        }

        public string UpdateWorkshop(int workshopId, string name, string address)
        {
            var workshop = _context.Workshops.Find(workshopId);
            if (workshop == null) return "Цех не найден";
            workshop.WorkshopName = name;
            workshop.Adress = address;
            _context.SaveChanges();
            return null;
        }

        public string DeleteWorkshop(int workshopId)
        {
            var workshop = _context.Workshops.Find(workshopId);
            if (workshop == null) return "Цех не найден";


            bool hasOperations = _context.Operations.Any(o => o.WorkshopId == workshopId);
            if (hasOperations) return "Нельзя удалить цех, так как в нём есть операции.";

            bool hasToolIssuances = _context.ToolIssuances.Any(ti => ti.WorkshopId == workshopId);
            if (hasToolIssuances) return "Нельзя удалить цех, так как есть выдачи инструментов в этот цех.";

            _context.Workshops.Remove(workshop);
            _context.SaveChanges();
            return null;
        }
    }
}