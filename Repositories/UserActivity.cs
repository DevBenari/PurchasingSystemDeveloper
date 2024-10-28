namespace PurchasingSystemDeveloper.Repositories
{
    public class UserActivity
    {
        public DateTimeOffset CreateDateTime { get; set; }
        public Guid CreateBy { get; set; }
        public DateTimeOffset UpdateDateTime { get; set; }
        public Guid UpdateBy { get; set; }        
        public DateTimeOffset DeleteDateTime { get; set; }
        public Guid DeleteBy { get; set; }
        public bool IsCancel { get; set; }
        public bool IsDelete { get; set; }
    }
}
