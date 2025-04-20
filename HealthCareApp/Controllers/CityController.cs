using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{

    public class CityController : Controller
    {
        private readonly IGenericRepoServices<City> CityRepo;
        public CityController(IGenericRepoServices<City> CityRepo)
        {
            this.CityRepo = CityRepo;
        }

        public IActionResult Index(int page = 1, int pageSize = 18)
        {
            int skip = (page - 1) * pageSize;
            var result = CityRepo.FindAllForSearch(s => true, skip, pageSize);
            var totalCount = CityRepo.Count();

            var resultItems = result.Select(r => new {
                id = r.Id,
                name = r.CityNameEn
            }).ToList();

            return Json(new
            {
                items = resultItems,
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
    }
}
