using BigData.UI.DAL.DTO_s;
using BigData.UI.DAL.Entities;
using BigData.UI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace BigData.UI.Controllers
{
    public class CarController : Controller
    {
        private readonly string _connectionString = "Server = DESKTOP-4K38GM2; initial catalog = CARPLATES; integrated security = true";
        private readonly IMemoryCache _cache;

        public CarController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var cacheKey = "ResultViewModel";
            var result = _cache.Get<ResultViewModel>(cacheKey);

            if (result is not null)
            {
                return View(result);
            }
            await using var connection = new SqlConnection(_connectionString);

            var brandMax = (await connection.QueryAsync<BrandResultDto>("SELECT TOP 5 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).ToList();
            var brandMin = (await connection.QueryAsync<BrandResultDto>("SELECT TOP 5 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).ToList();
            var plateMax = (await connection.QueryAsync<PlateResultDto>("SELECT TOP 5 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).ToList();
            var plateMin = (await connection.QueryAsync<PlateResultDto>("SELECT TOP 5 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).ToList();
            var shiftType = (await connection.QueryAsync<ShiftTypeResultDto>("SELECT TOP 3 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count DESC")).ToList();
            var fuelType = (await connection.QueryAsync<FuelResultDto>("SELECT TOP 3 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL")).ToList();
            var caseType = (await connection.QueryAsync<CaseTypeResultDto>("SELECT TOP 3 CASETYPE, COUNT(*) AS count FROM PLATES GROUP BY CASETYPE ORDER BY count DESC")).ToList();


            result = new ResultViewModel
            {
                BrandMax = brandMax,
                BrandMin = brandMin,
                PlateMax = plateMax,
                PlateMin = plateMin,
                ShiftType = shiftType,
                FuelType = fuelType,
                CaseType = caseType,
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
            };
            _cache.Set(cacheKey, result, cacheEntryOptions);

            return View(result);
        }




        public async Task<IActionResult> Search(string keyword)
        {

            string query = @"
            SELECT TOP 10000 BRAND, SUBSTRING(PLATE, 1, 2) AS PlatePrefix, SHIFTTYPE, FUEL
            FROM PLATES
            WHERE BRAND LIKE '%' + @Keyword + '%'
               OR PLATE LIKE '%' + @Keyword + '%'
               OR SHIFTTYPE LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
        ";

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Sorguyu çalıştırın ve sonuçları alın
            var searchResults = await connection.QueryAsync<SearchResultDto>(query, new { Keyword = keyword });

            // Sonuçları JSON formatında döndürün
            return Json(searchResults);

        }
    }
}
