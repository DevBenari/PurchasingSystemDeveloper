namespace PurchasingSystemDeveloper.Repositories
{
    public static class GetDateRangeHelper
    {
        public static (DateTimeOffset?, DateTimeOffset?) GetDateRange(string filterOptions)
        {
            var now = DateTimeOffset.Now;
            return filterOptions switch
            {
                "Today" => (now.Date, now.Date),
                "Last Day" => (now.AddDays(-1).Date, now.AddDays(-1).Date),
                "Last 7 Days" => (now.AddDays(-7).Date, now.Date),
                "Last 30 Days" => (now.AddDays(-30).Date, now.Date),
                "This Month" => (new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset), now.Date),
                "Last Month" =>
                    (new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset).AddMonths(-1),
                     new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset).AddDays(-1)),
                _ => (null, null),
            };
        }
    }    
}
