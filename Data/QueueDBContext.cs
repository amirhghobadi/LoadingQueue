using LoadingQueue.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Data;

/// <summary>
/// DbContext اصلی پروژه. از IdentityDbContext ارث‌بری می‌کنه تا
/// جدول‌های کاربر/نقش/لاگین (Identity) هم خودکار داخلش ساخته بشن،
/// در کنار جدول‌های خودمون (Driver, Queue, Company, ShippingCompany).
/// </summary>
public class QueueDBContext : IdentityDbContext<ApplicationUser>
{
    public QueueDBContext(DbContextOptions<QueueDBContext> options)
        : base(options)
    {

    }

    public DbSet<Driver> Drivers => Set<Driver>();

    public DbSet<Queue> Queues => Set<Queue>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<ShippingCompany> ShippingCompanies => Set<ShippingCompany>();

    public DbSet<CompanySettings> CompanySettingsList => Set<CompanySettings>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // رابطه‌ی Driver (۱) ---- (چند) Queue
        // OnDelete: Restrict یعنی اگه راننده‌ای نوبت داشته باشه،
        // نمی‌شه رکوردش رو مستقیم از دیتابیس حذف کرد (به همین دلیل
        // در DriverRepository.DeleteAsync از حذف نرم/Soft Delete استفاده شده).
        modelBuilder.Entity<Queue>()
            .HasOne(q => q.Driver)
            .WithMany(d => d.Queues)
            .HasForeignKey(q => q.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Company)
            .WithMany(c => c.Drivers)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Queue>()
            .HasOne(q => q.Company)
            .WithMany(c => c.Queues)
            .HasForeignKey(q => q.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShippingCompany>()
            .HasOne(s => s.Company)
            .WithMany(c => c.ShippingCompanies)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanySettings>()
            .HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Company)
            .WithMany()
            .HasForeignKey(n => n.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Queue>()
            .HasOne(q => q.ShippingCompany)
            .WithMany(s => s.Queues)
            .HasForeignKey(q => q.ShippingCompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // نوع دیتابیسی مبلغ کرایه: money (به‌جای decimal پیش‌فرض)
        modelBuilder.Entity<Queue>()
            .Property(q => q.FreightAmount)
            .HasColumnType("money");

        // محدودیت طول رشته‌ها برای جدول Driver
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.Property(x => x.FullName)
                .HasMaxLength(100);

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(11);

            entity.Property(x => x.CarNumber)
                .HasMaxLength(6);

            entity.Property(x => x.CardNumberOrSheba)
                .HasMaxLength(26);



            // فیلتر سراسری: رانندگان حذف‌شده (IsDeleted=true) خودکار
            // از همه‌ی کوئری‌ها حذف می‌شن مگه اینکه IgnoreQueryFilters() صدا بزنیم.
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // محدودیت طول رشته‌ها برای جدول Queue
        // (فیلدهای DriverName/DriverPhone/... اینجا هم محدود می‌شن چون
        // Snapshot از اطلاعات Driver هستن - نگاه کن به کامنت‌های Models/Queue.cs)
        modelBuilder.Entity<Queue>(entity =>
        {
            entity.Property(x => x.DriverName)
                .HasMaxLength(100);

            entity.Property(x => x.DriverPhone)
                .HasMaxLength(11);

            entity.Property(x => x.DriverCarNumber)
                .HasMaxLength(6);

            entity.Property(x => x.DriverCardNumber)
                .HasMaxLength(26);

            entity.Property(x => x.ShippingCompanyName)
                .HasMaxLength(50);

            entity.Property(x => x.WaybillNumber)
                .HasMaxLength(50);

            entity.Property(x => x.Destination)
                .HasMaxLength(50);

            entity.Property(x => x.ExitNumber)
                .HasMaxLength(20);
        });

        modelBuilder.Entity<Company>(entity =>
{
    entity.Property(x => x.Name).HasMaxLength(100);
    entity.Property(x => x.NationalId).HasMaxLength(20);
    entity.Property(x => x.Phone).HasMaxLength(15);
});

        modelBuilder.Entity<ShippingCompany>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100);
            entity.Property(x => x.NationalId).HasMaxLength(20);
            entity.Property(x => x.ManagerName).HasMaxLength(100);
            entity.Property(x => x.MobilePhone).HasMaxLength(11);
            entity.Property(x => x.LandlinePhone).HasMaxLength(15);
        });

        SeedData(modelBuilder);
    }

    // داده‌های اولیه (Seed) برای تست: ۱۰۰ راننده + چند نوبت نمونه.
    // این داده‌ها فقط موقع اجرای Migration اضافه می‌شن، تغییری در
    // ساختار جدول نمی‌دن. اگه دیگه لازم نداری می‌تونی بعداً حذفشون کنی.
    private static void SeedData(ModelBuilder modelBuilder)
    {

        // شرکت پیش‌فرض که همه‌ی راننده‌ها/نوبت‌های قبلی بهش وصل می‌شن
        modelBuilder.Entity<Company>().HasData(new Company
        {
            Id = 1,
            Name = "شرکت پیش‌فرض",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        });

        modelBuilder.Entity<Queue>().HasData(GetQueue());
        modelBuilder.Entity<Driver>().HasData(GetDriver());

    }



    public static IEnumerable<Queue> GetQueue() => new List<Queue>
{
   new Queue
            {
                Id = 1,
                CompanyId = 1,
                DriverId = 9,
                DriverName = "حسن رحیمی",
                DriverPhone = "09125437923",
                DriverCarNumber = "54718",
                DriverCardNumber = "6037990000000009",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100001",
                FreightAmount = 8500000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(7,0,0),
                CakeCartonCount = 20,
                NutCartonCount = 22,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 2,
                CompanyId = 1,
                DriverId = 28,
                DriverName = "حسن نوری",
                DriverPhone = "09126707197",
                DriverCarNumber = "44887",
                DriverCardNumber = "6037990000000028",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100002",
                FreightAmount = 12700000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(8,40,0),
                CakeCartonCount = 25,
                NutCartonCount = 22,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 3,
                CompanyId = 1,
                DriverId = 12,
                DriverName = "فرهاد قاسمی",
                DriverPhone = "09124823498",
                DriverCarNumber = "18146",
                DriverCardNumber = "6037990000000012",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100003",
                FreightAmount = 10800000m,
                Destination = "تبریز",
                QueueTime = new TimeSpan(9,30,0),
                CakeCartonCount = 22,
                NutCartonCount = 10,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 4,
                CompanyId = 1,
                DriverId = 18,
                DriverName = "داوود جعفری",
                DriverPhone = "09121938483",
                DriverCarNumber = "97432",
                DriverCardNumber = "6037990000000018",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100004",
                FreightAmount = 8500000m,
                Destination = "قم",
                QueueTime = new TimeSpan(10,30,0),
                CakeCartonCount = 15,
                NutCartonCount = null,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 5,
                CompanyId = 1,
                DriverId = 30,
                DriverName = "محمد حسینی",
                DriverPhone = "09123997281",
                DriverCarNumber = "74880",
                DriverCardNumber = "6037990000000030",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100005",
                FreightAmount = 9200000m,
                Destination = "رشت",
                QueueTime = new TimeSpan(11,30,0),
                CakeCartonCount = 20,
                NutCartonCount = 12,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 6,
                CompanyId = 1,
                DriverId = 16,
                DriverName = "حسین رستمی",
                DriverPhone = "09129961380",
                DriverCarNumber = "91275",
                DriverCardNumber = "6037990000000016",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100006",
                FreightAmount = 10200000m,
                Destination = "ساری",
                QueueTime = new TimeSpan(12,30,0),
                CakeCartonCount = 30,
                NutCartonCount = 12,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 7,
                CompanyId = 1,
                DriverId = 5,
                DriverName = "داوود قاسمی",
                DriverPhone = "09128038374",
                DriverCarNumber = "93818",
                DriverCardNumber = "6037990000000005",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100007",
                FreightAmount = 8500000m,
                Destination = "کرمان",
                QueueTime = new TimeSpan(13,20,0),
                CakeCartonCount = null,
                NutCartonCount = 12,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 8,
                CompanyId = 1,
                DriverId = 29,
                DriverName = "حسن نجفی",
                DriverPhone = "09128612220",
                DriverCarNumber = "65261",
                DriverCardNumber = "6037990000000029",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100008",
                FreightAmount = 14800000m,
                Destination = "تبریز",
                QueueTime = new TimeSpan(14,15,0),
                CakeCartonCount = 28,
                NutCartonCount = 16,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 9,
                CompanyId = 1,
                DriverId = 36,
                DriverName = "حسین اکبری",
                DriverPhone = "09129937326",
                DriverCarNumber = "18878",
                DriverCardNumber = "6037990000000036",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100009",
                FreightAmount = 14800000m,
                Destination = "یزد",
                QueueTime = new TimeSpan(15,15,0),
                CakeCartonCount = 25,
                NutCartonCount = null,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 10,
                CompanyId = 1,
                DriverId = 7,
                DriverName = "مهدی یوسفی",
                DriverPhone = "09123608513",
                DriverCarNumber = "53384",
                DriverCardNumber = "6037990000000007",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100010",
                FreightAmount = 14100000m,
                Destination = "اهواز",
                QueueTime = new TimeSpan(16,0,0),
                CakeCartonCount = null,
                NutCartonCount = 12,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 11,
                CompanyId = 1,
                DriverId = 4,
                DriverName = "امیر جعفری",
                DriverPhone = "09121445199",
                DriverCarNumber = "74716",
                DriverCardNumber = "6037990000000004",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100011",
                FreightAmount = 9800000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(17,10,0),
                CakeCartonCount = 28,
                NutCartonCount = 12,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 12,
                CompanyId = 1,
                DriverId = 35,
                DriverName = "جواد احمدی",
                DriverPhone = "09122321324",
                DriverCarNumber = "40999",
                DriverCardNumber = "6037990000000035",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100012",
                FreightAmount = 12700000m,
                Destination = "مشهد",
                QueueTime = new TimeSpan(7,30,0),
                CakeCartonCount = 35,
                NutCartonCount = 18,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 13,
                CompanyId = 1,
                DriverId = 1,
                DriverName = "حسن محمدی",
                DriverPhone = "09124744854",
                DriverCarNumber = "45350",
                DriverCardNumber = "6037990000000001",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100013",
                FreightAmount = 13400000m,
                Destination = "ساری",
                QueueTime = new TimeSpan(8,10,0),
                CakeCartonCount = 25,
                NutCartonCount = 12,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 14,
                CompanyId = 1,
                DriverId = 6,
                DriverName = "جواد شریفی",
                DriverPhone = "09121109031",
                DriverCarNumber = "85384",
                DriverCardNumber = "6037990000000006",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100014",
                FreightAmount = 9200000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(9,15,0),
                CakeCartonCount = 35,
                NutCartonCount = 14,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 15,
                CompanyId = 1,
                DriverId = 45,
                DriverName = "امیر نوری",
                DriverPhone = "09128930103",
                DriverCarNumber = "26840",
                DriverCardNumber = "6037990000000045",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100015",
                FreightAmount = 11500000m,
                Destination = "تبریز",
                QueueTime = new TimeSpan(10,30,0),
                CakeCartonCount = 22,
                NutCartonCount = 18,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 16,
                CompanyId = 1,
                DriverId = 11,
                DriverName = "حسین نوری",
                DriverPhone = "09127067228",
                DriverCarNumber = "47949",
                DriverCardNumber = "6037990000000011",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100016",
                FreightAmount = 8500000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(11,45,0),
                CakeCartonCount = 18,
                NutCartonCount = 22,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 17,
                CompanyId = 1,
                DriverId = 27,
                DriverName = "سعید شریفی",
                DriverPhone = "09121192619",
                DriverCarNumber = "77357",
                DriverCardNumber = "6037990000000027",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100017",
                FreightAmount = 11500000m,
                Destination = "تهران",
                QueueTime = new TimeSpan(12,10,0),
                CakeCartonCount = 20,
                NutCartonCount = 18,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 18,
                CompanyId = 1,
                DriverId = 32,
                DriverName = "سعید قاسمی",
                DriverPhone = "09123710343",
                DriverCarNumber = "29482",
                DriverCardNumber = "6037990000000032",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100018",
                FreightAmount = 12000000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(13,0,0),
                CakeCartonCount = 35,
                NutCartonCount = 10,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 19,
                CompanyId = 1,
                DriverId = 31,
                DriverName = "ابراهیم رضایی",
                DriverPhone = "09129517169",
                DriverCarNumber = "90405",
                DriverCardNumber = "6037990000000031",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100019",
                FreightAmount = 9800000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(14,45,0),
                CakeCartonCount = 35,
                NutCartonCount = 10,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 20,
                CompanyId = 1,
                DriverId = 41,
                DriverName = "جواد کریمی",
                DriverPhone = "09124860684",
                DriverCarNumber = "53121",
                DriverCardNumber = "6037990000000041",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100020",
                FreightAmount = 9800000m,
                Destination = "رشت",
                QueueTime = new TimeSpan(15,0,0),
                CakeCartonCount = 30,
                NutCartonCount = 14,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,9)
            },
            new Queue
            {
                Id = 21,
                CompanyId = 1,
                DriverId = 24,
                DriverName = "جواد حیدری",
                DriverPhone = "09122525206",
                DriverCarNumber = "75605",
                DriverCardNumber = "6037990000000024",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100021",
                FreightAmount = 14800000m,
                Destination = "قزوین",
                QueueTime = new TimeSpan(7,10,0),
                CakeCartonCount = 22,
                NutCartonCount = 18,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 22,
                CompanyId = 1,
                DriverId = 3,
                DriverName = "فرهاد یوسفی",
                DriverPhone = "09122571945",
                DriverCarNumber = "14130",
                DriverCardNumber = "6037990000000003",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100022",
                FreightAmount = 8500000m,
                Destination = "ساری",
                QueueTime = new TimeSpan(8,10,0),
                CakeCartonCount = 32,
                NutCartonCount = 16,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 23,
                CompanyId = 1,
                DriverId = 23,
                DriverName = "فرهاد یوسفی",
                DriverPhone = "09127073292",
                DriverCarNumber = "84508",
                DriverCardNumber = "6037990000000023",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100023",
                FreightAmount = 10200000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(9,40,0),
                CakeCartonCount = 22,
                NutCartonCount = 10,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 24,
                CompanyId = 1,
                DriverId = 14,
                DriverName = "یوسف حسینی",
                DriverPhone = "09127120868",
                DriverCarNumber = "68750",
                DriverCardNumber = "6037990000000014",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100024",
                FreightAmount = 10800000m,
                Destination = "قم",
                QueueTime = new TimeSpan(10,15,0),
                CakeCartonCount = 18,
                NutCartonCount = 18,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 25,
                CompanyId = 1,
                DriverId = 44,
                DriverName = "حسین طاهری",
                DriverPhone = "09129143903",
                DriverCarNumber = "40385",
                DriverCardNumber = "6037990000000044",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100025",
                FreightAmount = 13400000m,
                Destination = "ساری",
                QueueTime = new TimeSpan(11,0,0),
                CakeCartonCount = 28,
                NutCartonCount = 10,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 26,
                CompanyId = 1,
                DriverId = 16,
                DriverName = "حسین رستمی",
                DriverPhone = "09129961380",
                DriverCarNumber = "91275",
                DriverCardNumber = "6037990000000016",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100026",
                FreightAmount = 10800000m,
                Destination = "کرمان",
                QueueTime = new TimeSpan(12,30,0),
                CakeCartonCount = 15,
                NutCartonCount = 12,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 27,
                CompanyId = 1,
                DriverId = 43,
                DriverName = "جواد کریمی",
                DriverPhone = "09126543670",
                DriverCarNumber = "14980",
                DriverCardNumber = "6037990000000043",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100027",
                FreightAmount = 11500000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(13,20,0),
                CakeCartonCount = 30,
                NutCartonCount = 12,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 28,
                CompanyId = 1,
                DriverId = 7,
                DriverName = "مهدی یوسفی",
                DriverPhone = "09123608513",
                DriverCarNumber = "53384",
                DriverCardNumber = "6037990000000007",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100028",
                FreightAmount = 12000000m,
                Destination = "تهران",
                QueueTime = new TimeSpan(14,15,0),
                CakeCartonCount = 15,
                NutCartonCount = 16,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 29,
                CompanyId = 1,
                DriverId = 48,
                DriverName = "کاظم احمدی",
                DriverPhone = "09122651177",
                DriverCarNumber = "96769",
                DriverCardNumber = "6037990000000048",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100029",
                FreightAmount = 11500000m,
                Destination = "یزد",
                QueueTime = new TimeSpan(15,40,0),
                CakeCartonCount = null,
                NutCartonCount = 20,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 30,
                CompanyId = 1,
                DriverId = 36,
                DriverName = "حسین اکبری",
                DriverPhone = "09129937326",
                DriverCarNumber = "18878",
                DriverCardNumber = "6037990000000036",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100030",
                FreightAmount = 10800000m,
                Destination = "ارومیه",
                QueueTime = new TimeSpan(16,40,0),
                CakeCartonCount = 35,
                NutCartonCount = 18,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 31,
                CompanyId = 1,
                DriverId = 27,
                DriverName = "سعید شریفی",
                DriverPhone = "09121192619",
                DriverCarNumber = "77357",
                DriverCardNumber = "6037990000000027",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100031",
                FreightAmount = 10800000m,
                Destination = "کرج",
                QueueTime = new TimeSpan(17,10,0),
                CakeCartonCount = 35,
                NutCartonCount = 14,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 32,
                CompanyId = 1,
                DriverId = 10,
                DriverName = "علی شریفی",
                DriverPhone = "09127350753",
                DriverCarNumber = "78227",
                DriverCardNumber = "6037990000000010",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100032",
                FreightAmount = 14800000m,
                Destination = "کرج",
                QueueTime = new TimeSpan(7,20,0),
                CakeCartonCount = 20,
                NutCartonCount = 18,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 33,
                CompanyId = 1,
                DriverId = 45,
                DriverName = "امیر نوری",
                DriverPhone = "09128930103",
                DriverCarNumber = "26840",
                DriverCardNumber = "6037990000000045",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100033",
                FreightAmount = 10200000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(8,20,0),
                CakeCartonCount = 30,
                NutCartonCount = 20,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 34,
                CompanyId = 1,
                DriverId = 11,
                DriverName = "حسین نوری",
                DriverPhone = "09127067228",
                DriverCarNumber = "47949",
                DriverCardNumber = "6037990000000011",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100034",
                FreightAmount = 10200000m,
                Destination = "یزد",
                QueueTime = new TimeSpan(9,45,0),
                CakeCartonCount = 35,
                NutCartonCount = null,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 35,
                CompanyId = 1,
                DriverId = 12,
                DriverName = "فرهاد قاسمی",
                DriverPhone = "09124823498",
                DriverCarNumber = "18146",
                DriverCardNumber = "6037990000000012",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100035",
                FreightAmount = 10800000m,
                Destination = "اصفهان",
                QueueTime = new TimeSpan(10,0,0),
                CakeCartonCount = 35,
                NutCartonCount = 20,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 36,
                CompanyId = 1,
                DriverId = 40,
                DriverName = "کاظم طاهری",
                DriverPhone = "09125159166",
                DriverCarNumber = "67223",
                DriverCardNumber = "6037990000000040",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100036",
                FreightAmount = 10200000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(11,0,0),
                CakeCartonCount = 22,
                NutCartonCount = 14,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 37,
                CompanyId = 1,
                DriverId = 2,
                DriverName = "رضا رضایی",
                DriverPhone = "09122458591",
                DriverCarNumber = "96858",
                DriverCardNumber = "6037990000000002",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100037",
                FreightAmount = 14100000m,
                Destination = "اهواز",
                QueueTime = new TimeSpan(12,20,0),
                CakeCartonCount = 18,
                NutCartonCount = null,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 38,
                CompanyId = 1,
                DriverId = 41,
                DriverName = "جواد کریمی",
                DriverPhone = "09124860684",
                DriverCarNumber = "53121",
                DriverCardNumber = "6037990000000041",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100038",
                FreightAmount = 12000000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(13,10,0),
                CakeCartonCount = 32,
                NutCartonCount = 22,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 39,
                CompanyId = 1,
                DriverId = 22,
                DriverName = "رضا حسینی",
                DriverPhone = "09125408072",
                DriverCarNumber = "27352",
                DriverCardNumber = "6037990000000022",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100039",
                FreightAmount = 8500000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(14,10,0),
                CakeCartonCount = 18,
                NutCartonCount = 22,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 40,
                CompanyId = 1,
                DriverId = 26,
                DriverName = "بهرام رستمی",
                DriverPhone = "09127402509",
                DriverCarNumber = "18494",
                DriverCardNumber = "6037990000000026",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100040",
                FreightAmount = 12700000m,
                Destination = "رشت",
                QueueTime = new TimeSpan(15,45,0),
                CakeCartonCount = 15,
                NutCartonCount = 16,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,10)
            },
            new Queue
            {
                Id = 41,
                CompanyId = 1,
                DriverId = 30,
                DriverName = "محمد حسینی",
                DriverPhone = "09123997281",
                DriverCarNumber = "74880",
                DriverCardNumber = "6037990000000030",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100041",
                FreightAmount = 13400000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(7,15,0),
                CakeCartonCount = 32,
                NutCartonCount = 16,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 42,
                CompanyId = 1,
                DriverId = 9,
                DriverName = "حسن رحیمی",
                DriverPhone = "09125437923",
                DriverCarNumber = "54718",
                DriverCardNumber = "6037990000000009",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100042",
                FreightAmount = 10800000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(8,15,0),
                CakeCartonCount = 22,
                NutCartonCount = 18,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 43,
                CompanyId = 1,
                DriverId = 50,
                DriverName = "جواد قاسمی",
                DriverPhone = "09128526486",
                DriverCarNumber = "34649",
                DriverCardNumber = "6037990000000050",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100043",
                FreightAmount = 9800000m,
                Destination = "اصفهان",
                QueueTime = new TimeSpan(9,20,0),
                CakeCartonCount = 20,
                NutCartonCount = 16,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 44,
                CompanyId = 1,
                DriverId = 43,
                DriverName = "جواد کریمی",
                DriverPhone = "09126543670",
                DriverCarNumber = "14980",
                DriverCardNumber = "6037990000000043",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100044",
                FreightAmount = 9200000m,
                Destination = "تبریز",
                QueueTime = new TimeSpan(10,15,0),
                CakeCartonCount = 30,
                NutCartonCount = 22,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 45,
                CompanyId = 1,
                DriverId = 34,
                DriverName = "صادق محمدی",
                DriverPhone = "09126159230",
                DriverCarNumber = "24471",
                DriverCardNumber = "6037990000000034",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100045",
                FreightAmount = 10200000m,
                Destination = "تهران",
                QueueTime = new TimeSpan(11,45,0),
                CakeCartonCount = 30,
                NutCartonCount = 22,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 46,
                CompanyId = 1,
                DriverId = 36,
                DriverName = "حسین اکبری",
                DriverPhone = "09129937326",
                DriverCarNumber = "18878",
                DriverCardNumber = "6037990000000036",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100046",
                FreightAmount = 12700000m,
                Destination = "کرج",
                QueueTime = new TimeSpan(12,15,0),
                CakeCartonCount = 15,
                NutCartonCount = 20,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 47,
                CompanyId = 1,
                DriverId = 39,
                DriverName = "داوود قاسمی",
                DriverPhone = "09127264956",
                DriverCarNumber = "49508",
                DriverCardNumber = "6037990000000039",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100047",
                FreightAmount = 13400000m,
                Destination = "رشت",
                QueueTime = new TimeSpan(13,20,0),
                CakeCartonCount = null,
                NutCartonCount = 16,
                ExitNumber = "2",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 48,
                CompanyId = 1,
                DriverId = 21,
                DriverName = "امیر اکبری",
                DriverPhone = "09128698256",
                DriverCarNumber = "60758",
                DriverCardNumber = "6037990000000021",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100048",
                FreightAmount = 12700000m,
                Destination = "کرج",
                QueueTime = new TimeSpan(14,15,0),
                CakeCartonCount = 15,
                NutCartonCount = 22,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 49,
                CompanyId = 1,
                DriverId = 29,
                DriverName = "حسن نجفی",
                DriverPhone = "09128612220",
                DriverCarNumber = "65261",
                DriverCardNumber = "6037990000000029",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100049",
                FreightAmount = 12700000m,
                Destination = "مشهد",
                QueueTime = new TimeSpan(15,20,0),
                CakeCartonCount = 20,
                NutCartonCount = 10,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 50,
                CompanyId = 1,
                DriverId = 40,
                DriverName = "کاظم طاهری",
                DriverPhone = "09125159166",
                DriverCarNumber = "67223",
                DriverCardNumber = "6037990000000040",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100050",
                FreightAmount = 12000000m,
                Destination = "یزد",
                QueueTime = new TimeSpan(16,10,0),
                CakeCartonCount = 20,
                NutCartonCount = null,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 51,
                CompanyId = 1,
                DriverId = 33,
                DriverName = "داوود طاهری",
                DriverPhone = "09126438436",
                DriverCarNumber = "10713",
                DriverCardNumber = "6037990000000033",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100051",
                FreightAmount = 11500000m,
                Destination = "کرج",
                QueueTime = new TimeSpan(17,15,0),
                CakeCartonCount = 22,
                NutCartonCount = null,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 52,
                CompanyId = 1,
                DriverId = 28,
                DriverName = "حسن نوری",
                DriverPhone = "09126707197",
                DriverCarNumber = "44887",
                DriverCardNumber = "6037990000000028",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100052",
                FreightAmount = 12000000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(7,20,0),
                CakeCartonCount = 25,
                NutCartonCount = 12,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 53,
                CompanyId = 1,
                DriverId = 45,
                DriverName = "امیر نوری",
                DriverPhone = "09128930103",
                DriverCarNumber = "26840",
                DriverCardNumber = "6037990000000045",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100053",
                FreightAmount = 8500000m,
                Destination = "رشت",
                QueueTime = new TimeSpan(8,40,0),
                CakeCartonCount = 28,
                NutCartonCount = 16,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 54,
                CompanyId = 1,
                DriverId = 42,
                DriverName = "فرهاد جعفری",
                DriverPhone = "09121987737",
                DriverCarNumber = "10172",
                DriverCardNumber = "6037990000000042",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100054",
                FreightAmount = 8500000m,
                Destination = "تهران",
                QueueTime = new TimeSpan(9,45,0),
                CakeCartonCount = 22,
                NutCartonCount = 16,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 55,
                CompanyId = 1,
                DriverId = 11,
                DriverName = "حسین نوری",
                DriverPhone = "09127067228",
                DriverCarNumber = "47949",
                DriverCardNumber = "6037990000000011",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100055",
                FreightAmount = 10200000m,
                Destination = "مشهد",
                QueueTime = new TimeSpan(10,40,0),
                CakeCartonCount = 20,
                NutCartonCount = null,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 56,
                CompanyId = 1,
                DriverId = 31,
                DriverName = "ابراهیم رضایی",
                DriverPhone = "09129517169",
                DriverCarNumber = "90405",
                DriverCardNumber = "6037990000000031",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100056",
                FreightAmount = 12700000m,
                Destination = "تبریز",
                QueueTime = new TimeSpan(11,10,0),
                CakeCartonCount = 25,
                NutCartonCount = 20,
                ExitNumber = "1",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 57,
                CompanyId = 1,
                DriverId = 37,
                DriverName = "رضا حیدری",
                DriverPhone = "09123770370",
                DriverCarNumber = "94586",
                DriverCardNumber = "6037990000000037",
                ShippingCompanyName = "باربری امید",
                WaybillNumber = "WB100057",
                FreightAmount = 9200000m,
                Destination = "شیراز",
                QueueTime = new TimeSpan(12,15,0),
                CakeCartonCount = null,
                NutCartonCount = 10,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 58,
                CompanyId = 1,
                DriverId = 17,
                DriverName = "جواد مرادی",
                DriverPhone = "09125528972",
                DriverCarNumber = "69488",
                DriverCardNumber = "6037990000000017",
                ShippingCompanyName = "باربری وطن",
                WaybillNumber = "WB100058",
                FreightAmount = 10200000m,
                Destination = "همدان",
                QueueTime = new TimeSpan(13,0,0),
                CakeCartonCount = 18,
                NutCartonCount = 16,
                ExitNumber = "3",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 59,
                CompanyId = 1,
                DriverId = 16,
                DriverName = "حسین رستمی",
                DriverPhone = "09129961380",
                DriverCarNumber = "91275",
                DriverCardNumber = "6037990000000016",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100059",
                FreightAmount = 9200000m,
                Destination = "کرمان",
                QueueTime = new TimeSpan(14,15,0),
                CakeCartonCount = null,
                NutCartonCount = 10,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
            new Queue
            {
                Id = 60,
                CompanyId = 1,
                DriverId = 27,
                DriverName = "سعید شریفی",
                DriverPhone = "09121192619",
                DriverCarNumber = "77357",
                DriverCardNumber = "6037990000000027",
                ShippingCompanyName = "باربری ترابر",
                WaybillNumber = "WB100060",
                FreightAmount = 9200000m,
                Destination = "قم",
                QueueTime = new TimeSpan(15,0,0),
                CakeCartonCount = 35,
                NutCartonCount = 20,
                ExitNumber = "4",
                QueueDate = new DateTime(2026,7,8)
            },
};
    public static IEnumerable<Driver> GetDriver() => new List<Driver>
    {

new Driver{Id=1,CompanyId=1,FullName="حسن محمدی",CardNumberOrSheba="6037990000000001",CarNumber="45350",PhoneNumber="09124744854",RegisterDate=new DateTime(2026,1,1).AddDays(1)},
            new Driver{Id=2,CompanyId=1,FullName="رضا رضایی",CardNumberOrSheba="6037990000000002",CarNumber="96858",PhoneNumber="09122458591",RegisterDate=new DateTime(2026,1,1).AddDays(2)},
            new Driver{Id=3,CompanyId=1,FullName="فرهاد یوسفی",CardNumberOrSheba="6037990000000003",CarNumber="14130",PhoneNumber="09122571945",RegisterDate=new DateTime(2026,1,1).AddDays(3)},
            new Driver{Id=4,CompanyId=1,FullName="امیر جعفری",CardNumberOrSheba="6037990000000004",CarNumber="74716",PhoneNumber="09121445199",RegisterDate=new DateTime(2026,1,1).AddDays(4)},
            new Driver{Id=5,CompanyId=1,FullName="داوود قاسمی",CardNumberOrSheba="6037990000000005",CarNumber="93818",PhoneNumber="09128038374",RegisterDate=new DateTime(2026,1,1).AddDays(5)},
            new Driver{Id=6,CompanyId=1,FullName="جواد شریفی",CardNumberOrSheba="6037990000000006",CarNumber="85384",PhoneNumber="09121109031",RegisterDate=new DateTime(2026,1,1).AddDays(6)},
            new Driver{Id=7,CompanyId=1,FullName="مهدی یوسفی",CardNumberOrSheba="6037990000000007",CarNumber="53384",PhoneNumber="09123608513",RegisterDate=new DateTime(2026,1,1).AddDays(7)},
            new Driver{Id=8,CompanyId=1,FullName="امیر صادقی",CardNumberOrSheba="6037990000000008",CarNumber="23194",PhoneNumber="09127374122",RegisterDate=new DateTime(2026,1,1).AddDays(8)},
            new Driver{Id=9,CompanyId=1,FullName="حسن رحیمی",CardNumberOrSheba="6037990000000009",CarNumber="54718",PhoneNumber="09125437923",RegisterDate=new DateTime(2026,1,1).AddDays(9)},
            new Driver{Id=10,CompanyId=1,FullName="علی شریفی",CardNumberOrSheba="6037990000000010",CarNumber="78227",PhoneNumber="09127350753",RegisterDate=new DateTime(2026,1,1).AddDays(10)},
            new Driver{Id=11,CompanyId=1,FullName="حسین نوری",CardNumberOrSheba="6037990000000011",CarNumber="47949",PhoneNumber="09127067228",RegisterDate=new DateTime(2026,1,1).AddDays(11)},
            new Driver{Id=12,CompanyId=1,FullName="فرهاد قاسمی",CardNumberOrSheba="6037990000000012",CarNumber="18146",PhoneNumber="09124823498",RegisterDate=new DateTime(2026,1,1).AddDays(12)},
            new Driver{Id=13,CompanyId=1,FullName="اکبر کریمی",CardNumberOrSheba="6037990000000013",CarNumber="39987",PhoneNumber="09122694522",RegisterDate=new DateTime(2026,1,1).AddDays(13)},
            new Driver{Id=14,CompanyId=1,FullName="یوسف حسینی",CardNumberOrSheba="6037990000000014",CarNumber="68750",PhoneNumber="09127120868",RegisterDate=new DateTime(2026,1,1).AddDays(14)},
            new Driver{Id=15,CompanyId=1,FullName="مهدی رحیمی",CardNumberOrSheba="6037990000000015",CarNumber="55314",PhoneNumber="09125479144",RegisterDate=new DateTime(2026,1,1).AddDays(15)},
            new Driver{Id=16,CompanyId=1,FullName="حسین رستمی",CardNumberOrSheba="6037990000000016",CarNumber="91275",PhoneNumber="09129961380",RegisterDate=new DateTime(2026,1,1).AddDays(16)},
            new Driver{Id=17,CompanyId=1,FullName="جواد مرادی",CardNumberOrSheba="6037990000000017",CarNumber="69488",PhoneNumber="09125528972",RegisterDate=new DateTime(2026,1,1).AddDays(17)},
            new Driver{Id=18,CompanyId=1,FullName="داوود جعفری",CardNumberOrSheba="6037990000000018",CarNumber="97432",PhoneNumber="09121938483",RegisterDate=new DateTime(2026,1,1).AddDays(18)},
            new Driver{Id=19,CompanyId=1,FullName="جواد احمدی",CardNumberOrSheba="6037990000000019",CarNumber="50510",PhoneNumber="09125491946",RegisterDate=new DateTime(2026,1,1).AddDays(19)},
            new Driver{Id=20,CompanyId=1,FullName="حسین قاسمی",CardNumberOrSheba="6037990000000020",CarNumber="82997",PhoneNumber="09126279418",RegisterDate=new DateTime(2026,1,1).AddDays(20)},
            new Driver{Id=21,CompanyId=1,FullName="امیر اکبری",CardNumberOrSheba="6037990000000021",CarNumber="60758",PhoneNumber="09128698256",RegisterDate=new DateTime(2026,1,1).AddDays(21)},
            new Driver{Id=22,CompanyId=1,FullName="رضا حسینی",CardNumberOrSheba="6037990000000022",CarNumber="27352",PhoneNumber="09125408072",RegisterDate=new DateTime(2026,1,1).AddDays(22)},
            new Driver{Id=23,CompanyId=1,FullName="فرهاد یوسفی",CardNumberOrSheba="6037990000000023",CarNumber="84508",PhoneNumber="09127073292",RegisterDate=new DateTime(2026,1,1).AddDays(23)},
            new Driver{Id=24,CompanyId=1,FullName="جواد حیدری",CardNumberOrSheba="6037990000000024",CarNumber="75605",PhoneNumber="09122525206",RegisterDate=new DateTime(2026,1,1).AddDays(24)},
            new Driver{Id=25,CompanyId=1,FullName="علی رضایی",CardNumberOrSheba="6037990000000025",CarNumber="29742",PhoneNumber="09123684052",RegisterDate=new DateTime(2026,1,1).AddDays(25)},
            new Driver{Id=26,CompanyId=1,FullName="بهرام رستمی",CardNumberOrSheba="6037990000000026",CarNumber="18494",PhoneNumber="09127402509",RegisterDate=new DateTime(2026,1,1).AddDays(26)},
            new Driver{Id=27,CompanyId=1,FullName="سعید شریفی",CardNumberOrSheba="6037990000000027",CarNumber="77357",PhoneNumber="09121192619",RegisterDate=new DateTime(2026,1,1).AddDays(27)},
            new Driver{Id=28,CompanyId=1,FullName="حسن نوری",CardNumberOrSheba="6037990000000028",CarNumber="44887",PhoneNumber="09126707197",RegisterDate=new DateTime(2026,1,1).AddDays(28)},
            new Driver{Id=29,CompanyId=1,FullName="حسن نجفی",CardNumberOrSheba="6037990000000029",CarNumber="65261",PhoneNumber="09128612220",RegisterDate=new DateTime(2026,1,1).AddDays(29)},
            new Driver{Id=30,CompanyId=1,FullName="محمد حسینی",CardNumberOrSheba="6037990000000030",CarNumber="74880",PhoneNumber="09123997281",RegisterDate=new DateTime(2026,1,1).AddDays(30)},
            new Driver{Id=31,CompanyId=1,FullName="ابراهیم رضایی",CardNumberOrSheba="6037990000000031",CarNumber="90405",PhoneNumber="09129517169",RegisterDate=new DateTime(2026,1,1).AddDays(31)},
            new Driver{Id=32,CompanyId=1,FullName="سعید قاسمی",CardNumberOrSheba="6037990000000032",CarNumber="29482",PhoneNumber="09123710343",RegisterDate=new DateTime(2026,1,1).AddDays(32)},
            new Driver{Id=33,CompanyId=1,FullName="داوود طاهری",CardNumberOrSheba="6037990000000033",CarNumber="10713",PhoneNumber="09126438436",RegisterDate=new DateTime(2026,1,1).AddDays(33)},
            new Driver{Id=34,CompanyId=1,FullName="صادق محمدی",CardNumberOrSheba="6037990000000034",CarNumber="24471",PhoneNumber="09126159230",RegisterDate=new DateTime(2026,1,1).AddDays(34)},
            new Driver{Id=35,CompanyId=1,FullName="جواد احمدی",CardNumberOrSheba="6037990000000035",CarNumber="40999",PhoneNumber="09122321324",RegisterDate=new DateTime(2026,1,1).AddDays(35)},
            new Driver{Id=36,CompanyId=1,FullName="حسین اکبری",CardNumberOrSheba="6037990000000036",CarNumber="18878",PhoneNumber="09129937326",RegisterDate=new DateTime(2026,1,1).AddDays(36)},
            new Driver{Id=37,CompanyId=1,FullName="رضا حیدری",CardNumberOrSheba="6037990000000037",CarNumber="94586",PhoneNumber="09123770370",RegisterDate=new DateTime(2026,1,1).AddDays(37)},
            new Driver{Id=38,CompanyId=1,FullName="سجاد طاهری",CardNumberOrSheba="6037990000000038",CarNumber="87533",PhoneNumber="09124553384",RegisterDate=new DateTime(2026,1,1).AddDays(38)},
            new Driver{Id=39,CompanyId=1,FullName="داوود قاسمی",CardNumberOrSheba="6037990000000039",CarNumber="49508",PhoneNumber="09127264956",RegisterDate=new DateTime(2026,1,1).AddDays(39)},
            new Driver{Id=40,CompanyId=1,FullName="کاظم طاهری",CardNumberOrSheba="6037990000000040",CarNumber="67223",PhoneNumber="09125159166",RegisterDate=new DateTime(2026,1,1).AddDays(40)},
            new Driver{Id=41,CompanyId=1,FullName="جواد کریمی",CardNumberOrSheba="6037990000000041",CarNumber="53121",PhoneNumber="09124860684",RegisterDate=new DateTime(2026,1,1).AddDays(41)},
            new Driver{Id=42,CompanyId=1,FullName="فرهاد جعفری",CardNumberOrSheba="6037990000000042",CarNumber="10172",PhoneNumber="09121987737",RegisterDate=new DateTime(2026,1,1).AddDays(42)},
            new Driver{Id=43,CompanyId=1,FullName="جواد کریمی",CardNumberOrSheba="6037990000000043",CarNumber="14980",PhoneNumber="09126543670",RegisterDate=new DateTime(2026,1,1).AddDays(43)},
            new Driver{Id=44,CompanyId=1,FullName="حسین طاهری",CardNumberOrSheba="6037990000000044",CarNumber="40385",PhoneNumber="09129143903",RegisterDate=new DateTime(2026,1,1).AddDays(44)},
            new Driver{Id=45,CompanyId=1,FullName="امیر نوری",CardNumberOrSheba="6037990000000045",CarNumber="26840",PhoneNumber="09128930103",RegisterDate=new DateTime(2026,1,1).AddDays(45)},
            new Driver{Id=46,CompanyId=1,FullName="جواد اکبری",CardNumberOrSheba="6037990000000046",CarNumber="62294",PhoneNumber="09122582524",RegisterDate=new DateTime(2026,1,1).AddDays(46)},
            new Driver{Id=47,CompanyId=1,FullName="حسن یوسفی",CardNumberOrSheba="6037990000000047",CarNumber="55533",PhoneNumber="09127897151",RegisterDate=new DateTime(2026,1,1).AddDays(47)},
            new Driver{Id=48,CompanyId=1,FullName="کاظم احمدی",CardNumberOrSheba="6037990000000048",CarNumber="96769",PhoneNumber="09122651177",RegisterDate=new DateTime(2026,1,1).AddDays(48)},
            new Driver{Id=49,CompanyId=1,FullName="علی کاظمی",CardNumberOrSheba="6037990000000049",CarNumber="53919",PhoneNumber="09122833230",RegisterDate=new DateTime(2026,1,1).AddDays(49)},
            new Driver{Id=50,CompanyId=1,FullName="جواد قاسمی",CardNumberOrSheba="6037990000000050",CarNumber="34649",PhoneNumber="09128526486",RegisterDate=new DateTime(2026,1,1).AddDays(50)},



    };
}