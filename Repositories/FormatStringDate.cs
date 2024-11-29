namespace PurchasingSystemDeveloper.Repositories
{
    public static class FormatStringDate
    {
        public static string ReadStringDate(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("dd MMMM yyyy");
        }
    }
}
