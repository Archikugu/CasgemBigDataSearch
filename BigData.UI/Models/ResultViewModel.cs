using BigData.UI.DAL.DTO_s;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigData.UI.Models
{
    public class ResultViewModel
    {
        public List<BrandResultDto>? BrandMax { get; set; }
        public List<BrandResultDto>? BrandMin { get; set; }
        public List<PlateResultDto>? PlateMax { get; set; }
        public List<PlateResultDto>? PlateMin { get; set; }
        public List<ShiftTypeResultDto>? ShiftType { get; set; }
        public List<FuelResultDto>? FuelType { get; set; }
        public List<CaseTypeResultDto>? CaseType { get; set; }
    }
}
