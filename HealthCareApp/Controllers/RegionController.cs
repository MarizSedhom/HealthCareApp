using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class RegionController : Controller
    {
        private readonly IGenericRepoServices<Region> RegionRepo;
        public RegionController(IGenericRepoServices<Region> RegionRepo)
        {
            this.RegionRepo = RegionRepo;
        }

        public IActionResult Index(int page = 1, int pageSize = 18)
        {
            int skip = (page - 1) * pageSize;
            var result = RegionRepo.FindAllForSearch(s => true, skip, pageSize);
            var totalCount = RegionRepo.Count();

            var resultItems = result.Select(r => new {
                id = r.Id,
                name = r.RegionNameEn
            }).ToList();

            return Json(new
            {
                items = resultItems,
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet]
        public IActionResult GetByParent(int parentId, int page = 1, int pageSize = 18)
        {
            var query = RegionRepo.FindAll(r => r.CityId == parentId, r => r.City);
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new { r.Id, name = r.RegionNameEn })
                .ToList();

            return Json(new
            {
                items,
                currentPage = page,
                totalPages
            });
        }

    }
}