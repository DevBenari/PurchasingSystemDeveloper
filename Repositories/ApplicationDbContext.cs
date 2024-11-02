using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Models;

namespace PurchasingSystemDeveloper.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    #region Areas Master Data
    public DbSet<UserActive> UserActives { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TermOfPayment> TermOfPayments { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<LeadTime> LeadTimes { get; set; }
    public DbSet<InitialStock> InitialStocks { get; set; }
    public DbSet<WarehouseLocation> WarehouseLocations { get; set; }
    public DbSet<UnitLocation> UnitLocations { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    #endregion

    #region Areas Order
    public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
    public DbSet<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    public DbSet<ApprovalQtyDifference> ApprovalQtyDifferences { get; set; }
    public DbSet<Email> Emails { get; set; }
    #endregion

    #region Areas Warehouse
    public DbSet<ReceiveOrder> ReceiveOrders { get; set; }
    public DbSet<UnitOrder> UnitOrders { get; set; }
    public DbSet<UnitOrderDetail> UnitOrderDetails { get; set; }
    public DbSet<WarehouseTransfer> WarehouseTransfers { get; set; }
    public DbSet<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
    public DbSet<QtyDifference> QtyDifferences { get; set; }
    public DbSet<QtyDifferenceDetail> QtyDifferenceDetails { get; set; }
    #endregion

    #region Areas Unit Request
    public DbSet<UnitRequest> UnitRequests { get; set; }
    public DbSet<UnitRequestDetail> UnitRequestDetails { get; set; }
    public DbSet<ApprovalUnitRequest> ApprovalUnitRequests { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seeding data untuk AspNetRoles
        builder.Entity<IdentityRole>().HasData(
                 new IdentityRole { Id = "1c1fb17a-b16f-4877-b63e-0c981fce4e55", Name = "Dashboard", NormalizedName = "DASHBOARD", ConcurrencyStamp = "DASHBOARD" },
                 new IdentityRole { Id = "29667799-9a77-476f-a4a2-f98f16e3413e", Name = "Dashboard Master Data", NormalizedName = "DASHBOARD MASTER DATA", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "3155ad0b-3ff9-4336-80a9-a83f4d619392", Name = "Master Data", NormalizedName = "MASTER DATA", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "49b394a6-6d27-4872-932e-e3a50a075808", Name = "Role", NormalizedName = "ROLE", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "5ba73d55-d106-4bf6-8097-8e2b9ee5fd47", Name = "Users", NormalizedName = "USERS", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "6815db3b-87e3-482b-a75a-e0555364d54c", Name = "Bank", NormalizedName = "BANK", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "73f28f66-00fe-479d-835f-83a46a2a1274", Name = "Lead Time", NormalizedName = "LEAD TIME", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "8c65bb85-5e35-4306-a0d6-594c94ac0f94", Name = "Product", NormalizedName = "PRODUCT", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "90e4a277-18cb-4c89-9800-e94a0a87e64e", Name = "Warehouse", NormalizedName = "WAREHOUSE", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "91331c73-4934-4010-b7a6-8c78ec7e0799", Name = "Supplier", NormalizedName = "SUPPLIER", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "9225dc73-b091-4658-ab5c-67e10ca8f194", Name = "Get Data API", NormalizedName = "GET DATA API", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "939a0bfa-0dd5-4d4c-81d9-c177ed1fb622", Name = "Term Of Payment", NormalizedName = "TERM OF PAYMENT", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "944116d7-c153-449e-97a7-51fa0f835d45", Name = "Calculate Min-Max Stock", NormalizedName = "CALCULATE MIN-MAX STOCK", ConcurrencyStamp = "MASTER DATA" },
                 new IdentityRole { Id = "95d13c25-2b6d-4667-bbe2-8d149c20ef93", Name = "Unit Request", NormalizedName = "UNIT REQUEST", ConcurrencyStamp = "UNIT REQUEST" },
                 new IdentityRole { Id = "962a8321-789d-488c-97ba-e4bb60feb9ef", Name = "Approval Purchase Req", NormalizedName = "APPROVAL PURCHASE REQ", ConcurrencyStamp = "APPROVAL PURCHASE REQ" },
                 new IdentityRole { Id = "973250b1-e8f7-4a25-aece-350724c7281a", Name = "IndexRole", NormalizedName = "INDEXROLE", ConcurrencyStamp = "7cf929a2-f126-4179-84ff-5366f63f6ff7" },
                 new IdentityRole { Id = "98efd437-dd11-4ad9-966a-dea874e5eeed", Name = "Stock Monitoring", NormalizedName = "STOCK MONITORING", ConcurrencyStamp = "STOCK MONITORING" },
                 new IdentityRole { Id = "99471211-2e67-4b1d-a3d4-dfbaacf36586", Name = "Qty Difference", NormalizedName = "QTY DIFFERENCE", ConcurrencyStamp = "QTY DIFFERENCE" },
                 new IdentityRole { Id = "99516357-af9d-42f4-992f-f038e6128b4a", Name = "Purchase Order", NormalizedName = "PURCHASE ORDER", ConcurrencyStamp = "PURCHASE ORDER" },
                 new IdentityRole { Id = "996aa5d5-ca72-4b4b-abc1-4135cdb8cf76", Name = "Approval Unit Request", NormalizedName = "APPROVAL UNIT REQUEST", ConcurrencyStamp = "APPROVAL UNIT REQUEST" },
                 new IdentityRole { Id = "997984b2-3085-4ce5-b91a-88f6537e13ea", Name = "KPI", NormalizedName = "KPI", ConcurrencyStamp = "KPI" },
                 new IdentityRole { Id = "99808c34-0ba4-49c4-afd7-96d1ae9dd55e", Name = "Purchase Request", NormalizedName = "PURCHASE REQUEST", ConcurrencyStamp = "PURCHASE REQUEST" },
                 new IdentityRole { Id = "999d95b7-81ec-4ca1-b565-021c2f42befd", Name = "Receive Order", NormalizedName = "RECEIVE ORDER", ConcurrencyStamp = "RECEIVE ORDER" },
                 new IdentityRole { Id = "9999707a-96a7-4fba-87ce-09ceac2bb27b", Name = "Report", NormalizedName = "REPORT", ConcurrencyStamp = "REPORT" }
        );

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        // Uncomment jika Anda memiliki konfigurasi untuk ApplicationRole
        // builder.ApplyConfiguration(new ApplicationRoleEntityConfiguration());

        foreach (var foreignKey in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.KodeUser).HasMaxLength(255);
        builder.Property(u => u.NamaUser).HasMaxLength(255);
    }
}
