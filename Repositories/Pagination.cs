using Microsoft.AspNetCore.Mvc.Rendering;

namespace PurchasingSystemDeveloper.Repositories
{
    public class Pagination<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public int? SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}
