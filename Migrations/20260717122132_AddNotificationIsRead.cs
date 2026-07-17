using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LoadingQueue.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanySettingsList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    RequireApprovalForCompletion = table.Column<bool>(type: "bit", nullable: false),
                    DefaultPageSize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettingsList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySettingsList_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CardNumberOrSheba = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    CarNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    LandlinePhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueueNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: true),
                    ShippingCompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WaybillNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FreightAmount = table.Column<decimal>(type: "money", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QueueTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CakeCartonCount = table.Column<int>(type: "int", nullable: true),
                    NutCartonCount = table.Column<int>(type: "int", nullable: true),
                    ExitNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    QueueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DriverPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    DriverCarNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    DriverCardNumber = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queues_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Queues_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Queues_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "Code", "CreatedAt", "Description", "IsActive", "LogoPath", "Name", "NationalId", "Phone" },
                values: new object[] { 1, null, "A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, "شرکت پیش‌فرض", null, null });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "CarNumber", "CardNumberOrSheba", "CompanyId", "DeletedAt", "FullName", "IsDeleted", "PhoneNumber", "RegisterDate" },
                values: new object[,]
                {
                    { 1, "45350", "6037990000000001", 1, null, "حسن محمدی", false, "09124744854", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "96858", "6037990000000002", 1, null, "رضا رضایی", false, "09122458591", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "14130", "6037990000000003", 1, null, "فرهاد یوسفی", false, "09122571945", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "74716", "6037990000000004", 1, null, "امیر جعفری", false, "09121445199", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "93818", "6037990000000005", 1, null, "داوود قاسمی", false, "09128038374", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "85384", "6037990000000006", 1, null, "جواد شریفی", false, "09121109031", new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "53384", "6037990000000007", 1, null, "مهدی یوسفی", false, "09123608513", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "23194", "6037990000000008", 1, null, "امیر صادقی", false, "09127374122", new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "54718", "6037990000000009", 1, null, "حسن رحیمی", false, "09125437923", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "78227", "6037990000000010", 1, null, "علی شریفی", false, "09127350753", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "47949", "6037990000000011", 1, null, "حسین نوری", false, "09127067228", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "18146", "6037990000000012", 1, null, "فرهاد قاسمی", false, "09124823498", new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "39987", "6037990000000013", 1, null, "اکبر کریمی", false, "09122694522", new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, "68750", "6037990000000014", 1, null, "یوسف حسینی", false, "09127120868", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, "55314", "6037990000000015", 1, null, "مهدی رحیمی", false, "09125479144", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, "91275", "6037990000000016", 1, null, "حسین رستمی", false, "09129961380", new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, "69488", "6037990000000017", 1, null, "جواد مرادی", false, "09125528972", new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, "97432", "6037990000000018", 1, null, "داوود جعفری", false, "09121938483", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, "50510", "6037990000000019", 1, null, "جواد احمدی", false, "09125491946", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, "82997", "6037990000000020", 1, null, "حسین قاسمی", false, "09126279418", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, "60758", "6037990000000021", 1, null, "امیر اکبری", false, "09128698256", new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, "27352", "6037990000000022", 1, null, "رضا حسینی", false, "09125408072", new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, "84508", "6037990000000023", 1, null, "فرهاد یوسفی", false, "09127073292", new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, "75605", "6037990000000024", 1, null, "جواد حیدری", false, "09122525206", new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, "29742", "6037990000000025", 1, null, "علی رضایی", false, "09123684052", new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, "18494", "6037990000000026", 1, null, "بهرام رستمی", false, "09127402509", new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, "77357", "6037990000000027", 1, null, "سعید شریفی", false, "09121192619", new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, "44887", "6037990000000028", 1, null, "حسن نوری", false, "09126707197", new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, "65261", "6037990000000029", 1, null, "حسن نجفی", false, "09128612220", new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, "74880", "6037990000000030", 1, null, "محمد حسینی", false, "09123997281", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, "90405", "6037990000000031", 1, null, "ابراهیم رضایی", false, "09129517169", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, "29482", "6037990000000032", 1, null, "سعید قاسمی", false, "09123710343", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, "10713", "6037990000000033", 1, null, "داوود طاهری", false, "09126438436", new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, "24471", "6037990000000034", 1, null, "صادق محمدی", false, "09126159230", new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, "40999", "6037990000000035", 1, null, "جواد احمدی", false, "09122321324", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, "18878", "6037990000000036", 1, null, "حسین اکبری", false, "09129937326", new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, "94586", "6037990000000037", 1, null, "رضا حیدری", false, "09123770370", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, "87533", "6037990000000038", 1, null, "سجاد طاهری", false, "09124553384", new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, "49508", "6037990000000039", 1, null, "داوود قاسمی", false, "09127264956", new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, "67223", "6037990000000040", 1, null, "کاظم طاهری", false, "09125159166", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, "53121", "6037990000000041", 1, null, "جواد کریمی", false, "09124860684", new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, "10172", "6037990000000042", 1, null, "فرهاد جعفری", false, "09121987737", new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, "14980", "6037990000000043", 1, null, "جواد کریمی", false, "09126543670", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, "40385", "6037990000000044", 1, null, "حسین طاهری", false, "09129143903", new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, "26840", "6037990000000045", 1, null, "امیر نوری", false, "09128930103", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, "62294", "6037990000000046", 1, null, "جواد اکبری", false, "09122582524", new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 47, "55533", "6037990000000047", 1, null, "حسن یوسفی", false, "09127897151", new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 48, "96769", "6037990000000048", 1, null, "کاظم احمدی", false, "09122651177", new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 49, "53919", "6037990000000049", 1, null, "علی کاظمی", false, "09122833230", new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 50, "34649", "6037990000000050", 1, null, "جواد قاسمی", false, "09128526486", new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Queues",
                columns: new[] { "Id", "CakeCartonCount", "CompanyId", "Destination", "DriverCarNumber", "DriverCardNumber", "DriverId", "DriverName", "DriverPhone", "ExitNumber", "FreightAmount", "NutCartonCount", "QueueDate", "QueueNumber", "QueueTime", "ShippingCompanyId", "ShippingCompanyName", "SortOrder", "Status", "WaybillNumber" },
                values: new object[,]
                {
                    { 1, 20, 1, "ارومیه", "54718", "6037990000000009", 9, "حسن رحیمی", "09125437923", "4", 8500000m, 22, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 0, 0, 0), null, "باربری وطن", 0, 1, "WB100001" },
                    { 2, 25, 1, "ارومیه", "44887", "6037990000000028", 28, "حسن نوری", "09126707197", "4", 12700000m, 22, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 40, 0, 0), null, "باربری وطن", 0, 1, "WB100002" },
                    { 3, 22, 1, "تبریز", "18146", "6037990000000012", 12, "فرهاد قاسمی", "09124823498", "1", 10800000m, 10, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 30, 0, 0), null, "باربری امید", 0, 1, "WB100003" },
                    { 4, 15, 1, "قم", "97432", "6037990000000018", 18, "داوود جعفری", "09121938483", "2", 8500000m, null, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 30, 0, 0), null, "باربری ترابر", 0, 1, "WB100004" },
                    { 5, 20, 1, "رشت", "74880", "6037990000000030", 30, "محمد حسینی", "09123997281", "1", 9200000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 30, 0, 0), null, "باربری امید", 0, 1, "WB100005" },
                    { 6, 30, 1, "ساری", "91275", "6037990000000016", 16, "حسین رستمی", "09129961380", "2", 10200000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 30, 0, 0), null, "باربری ترابر", 0, 1, "WB100006" },
                    { 7, null, 1, "کرمان", "93818", "6037990000000005", 5, "داوود قاسمی", "09128038374", "3", 8500000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 20, 0, 0), null, "باربری ترابر", 0, 1, "WB100007" },
                    { 8, 28, 1, "تبریز", "65261", "6037990000000029", 29, "حسن نجفی", "09128612220", "4", 14800000m, 16, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 15, 0, 0), null, "باربری وطن", 0, 1, "WB100008" },
                    { 9, 25, 1, "یزد", "18878", "6037990000000036", 36, "حسین اکبری", "09129937326", "1", 14800000m, null, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 15, 0, 0), null, "باربری امید", 0, 1, "WB100009" },
                    { 10, null, 1, "اهواز", "53384", "6037990000000007", 7, "مهدی یوسفی", "09123608513", "2", 14100000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 16, 0, 0, 0), null, "باربری امید", 0, 1, "WB100010" },
                    { 11, 28, 1, "شیراز", "74716", "6037990000000004", 4, "امیر جعفری", "09121445199", "3", 9800000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 17, 10, 0, 0), null, "باربری ترابر", 0, 1, "WB100011" },
                    { 12, 35, 1, "مشهد", "40999", "6037990000000035", 35, "جواد احمدی", "09122321324", "1", 12700000m, 18, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 30, 0, 0), null, "باربری وطن", 0, 1, "WB100012" },
                    { 13, 25, 1, "ساری", "45350", "6037990000000001", 1, "حسن محمدی", "09124744854", "3", 13400000m, 12, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 10, 0, 0), null, "باربری ترابر", 0, 1, "WB100013" },
                    { 14, 35, 1, "ارومیه", "85384", "6037990000000006", 6, "جواد شریفی", "09121109031", "3", 9200000m, 14, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 15, 0, 0), null, "باربری امید", 0, 1, "WB100014" },
                    { 15, 22, 1, "تبریز", "26840", "6037990000000045", 45, "امیر نوری", "09128930103", "4", 11500000m, 18, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 30, 0, 0), null, "باربری ترابر", 0, 1, "WB100015" },
                    { 16, 18, 1, "ارومیه", "47949", "6037990000000011", 11, "حسین نوری", "09127067228", "3", 8500000m, 22, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 45, 0, 0), null, "باربری وطن", 0, 1, "WB100016" },
                    { 17, 20, 1, "تهران", "77357", "6037990000000027", 27, "سعید شریفی", "09121192619", "4", 11500000m, 18, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 10, 0, 0), null, "باربری امید", 0, 1, "WB100017" },
                    { 18, 35, 1, "همدان", "29482", "6037990000000032", 32, "سعید قاسمی", "09123710343", "1", 12000000m, 10, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100018" },
                    { 19, 35, 1, "ارومیه", "90405", "6037990000000031", 31, "ابراهیم رضایی", "09129517169", "3", 9800000m, 10, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 45, 0, 0), null, "باربری ترابر", 0, 1, "WB100019" },
                    { 20, 30, 1, "رشت", "53121", "6037990000000041", 41, "جواد کریمی", "09124860684", "3", 9800000m, 14, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100020" },
                    { 21, 22, 1, "قزوین", "75605", "6037990000000024", 24, "جواد حیدری", "09122525206", "1", 14800000m, 18, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 10, 0, 0), null, "باربری وطن", 0, 1, "WB100021" },
                    { 22, 32, 1, "ساری", "14130", "6037990000000003", 3, "فرهاد یوسفی", "09122571945", "4", 8500000m, 16, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 10, 0, 0), null, "باربری وطن", 0, 1, "WB100022" },
                    { 23, 22, 1, "شیراز", "84508", "6037990000000023", 23, "فرهاد یوسفی", "09127073292", "2", 10200000m, 10, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 40, 0, 0), null, "باربری وطن", 0, 1, "WB100023" },
                    { 24, 18, 1, "قم", "68750", "6037990000000014", 14, "یوسف حسینی", "09127120868", "4", 10800000m, 18, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 15, 0, 0), null, "باربری وطن", 0, 1, "WB100024" },
                    { 25, 28, 1, "ساری", "40385", "6037990000000044", 44, "حسین طاهری", "09129143903", "3", 13400000m, 10, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100025" },
                    { 26, 15, 1, "کرمان", "91275", "6037990000000016", 16, "حسین رستمی", "09129961380", "4", 10800000m, 12, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 30, 0, 0), null, "باربری امید", 0, 1, "WB100026" },
                    { 27, 30, 1, "همدان", "14980", "6037990000000043", 43, "جواد کریمی", "09126543670", "2", 11500000m, 12, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 20, 0, 0), null, "باربری وطن", 0, 1, "WB100027" },
                    { 28, 15, 1, "تهران", "53384", "6037990000000007", 7, "مهدی یوسفی", "09123608513", "4", 12000000m, 16, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 15, 0, 0), null, "باربری وطن", 0, 1, "WB100028" },
                    { 29, null, 1, "یزد", "96769", "6037990000000048", 48, "کاظم احمدی", "09122651177", "1", 11500000m, 20, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 40, 0, 0), null, "باربری امید", 0, 1, "WB100029" },
                    { 30, 35, 1, "ارومیه", "18878", "6037990000000036", 36, "حسین اکبری", "09129937326", "4", 10800000m, 18, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 16, 40, 0, 0), null, "باربری ترابر", 0, 1, "WB100030" },
                    { 31, 35, 1, "کرج", "77357", "6037990000000027", 27, "سعید شریفی", "09121192619", "4", 10800000m, 14, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 17, 10, 0, 0), null, "باربری وطن", 0, 1, "WB100031" },
                    { 32, 20, 1, "کرج", "78227", "6037990000000010", 10, "علی شریفی", "09127350753", "1", 14800000m, 18, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 20, 0, 0), null, "باربری ترابر", 0, 1, "WB100032" },
                    { 33, 30, 1, "شیراز", "26840", "6037990000000045", 45, "امیر نوری", "09128930103", "4", 10200000m, 20, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 20, 0, 0), null, "باربری وطن", 0, 1, "WB100033" },
                    { 34, 35, 1, "یزد", "47949", "6037990000000011", 11, "حسین نوری", "09127067228", "2", 10200000m, null, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 45, 0, 0), null, "باربری وطن", 0, 1, "WB100034" },
                    { 35, 35, 1, "اصفهان", "18146", "6037990000000012", 12, "فرهاد قاسمی", "09124823498", "2", 10800000m, 20, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100035" },
                    { 36, 22, 1, "شیراز", "67223", "6037990000000040", 40, "کاظم طاهری", "09125159166", "1", 10200000m, 14, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100036" },
                    { 37, 18, 1, "اهواز", "96858", "6037990000000002", 2, "رضا رضایی", "09122458591", "2", 14100000m, null, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 20, 0, 0), null, "باربری امید", 0, 1, "WB100037" },
                    { 38, 32, 1, "همدان", "53121", "6037990000000041", 41, "جواد کریمی", "09124860684", "2", 12000000m, 22, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 10, 0, 0), null, "باربری ترابر", 0, 1, "WB100038" },
                    { 39, 18, 1, "همدان", "27352", "6037990000000022", 22, "رضا حسینی", "09125408072", "2", 8500000m, 22, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 10, 0, 0), null, "باربری ترابر", 0, 1, "WB100039" },
                    { 40, 15, 1, "رشت", "18494", "6037990000000026", 26, "بهرام رستمی", "09127402509", "1", 12700000m, 16, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 45, 0, 0), null, "باربری ترابر", 0, 1, "WB100040" },
                    { 41, 32, 1, "شیراز", "74880", "6037990000000030", 30, "محمد حسینی", "09123997281", "4", 13400000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 15, 0, 0), null, "باربری ترابر", 0, 1, "WB100041" },
                    { 42, 22, 1, "همدان", "54718", "6037990000000009", 9, "حسن رحیمی", "09125437923", "3", 10800000m, 18, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 15, 0, 0), null, "باربری امید", 0, 1, "WB100042" },
                    { 43, 20, 1, "اصفهان", "34649", "6037990000000050", 50, "جواد قاسمی", "09128526486", "2", 9800000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 20, 0, 0), null, "باربری ترابر", 0, 1, "WB100043" },
                    { 44, 30, 1, "تبریز", "14980", "6037990000000043", 43, "جواد کریمی", "09126543670", "4", 9200000m, 22, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 15, 0, 0), null, "باربری ترابر", 0, 1, "WB100044" },
                    { 45, 30, 1, "تهران", "24471", "6037990000000034", 34, "صادق محمدی", "09126159230", "1", 10200000m, 22, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 45, 0, 0), null, "باربری وطن", 0, 1, "WB100045" },
                    { 46, 15, 1, "کرج", "18878", "6037990000000036", 36, "حسین اکبری", "09129937326", "4", 12700000m, 20, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 15, 0, 0), null, "باربری ترابر", 0, 1, "WB100046" },
                    { 47, null, 1, "رشت", "49508", "6037990000000039", 39, "داوود قاسمی", "09127264956", "2", 13400000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 20, 0, 0), null, "باربری وطن", 0, 1, "WB100047" },
                    { 48, 15, 1, "کرج", "60758", "6037990000000021", 21, "امیر اکبری", "09128698256", "4", 12700000m, 22, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 15, 0, 0), null, "باربری وطن", 0, 1, "WB100048" },
                    { 49, 20, 1, "مشهد", "65261", "6037990000000029", 29, "حسن نجفی", "09128612220", "1", 12700000m, 10, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 20, 0, 0), null, "باربری ترابر", 0, 1, "WB100049" },
                    { 50, 20, 1, "یزد", "67223", "6037990000000040", 40, "کاظم طاهری", "09125159166", "1", 12000000m, null, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 16, 10, 0, 0), null, "باربری امید", 0, 1, "WB100050" },
                    { 51, 22, 1, "کرج", "10713", "6037990000000033", 33, "داوود طاهری", "09126438436", "3", 11500000m, null, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 17, 15, 0, 0), null, "باربری وطن", 0, 1, "WB100051" },
                    { 52, 25, 1, "شیراز", "44887", "6037990000000028", 28, "حسن نوری", "09126707197", "1", 12000000m, 12, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 7, 20, 0, 0), null, "باربری وطن", 0, 1, "WB100052" },
                    { 53, 28, 1, "رشت", "26840", "6037990000000045", 45, "امیر نوری", "09128930103", "1", 8500000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 8, 40, 0, 0), null, "باربری ترابر", 0, 1, "WB100053" },
                    { 54, 22, 1, "تهران", "10172", "6037990000000042", 42, "فرهاد جعفری", "09121987737", "1", 8500000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 9, 45, 0, 0), null, "باربری ترابر", 0, 1, "WB100054" },
                    { 55, 20, 1, "مشهد", "47949", "6037990000000011", 11, "حسین نوری", "09127067228", "1", 10200000m, null, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 10, 40, 0, 0), null, "باربری ترابر", 0, 1, "WB100055" },
                    { 56, 25, 1, "تبریز", "90405", "6037990000000031", 31, "ابراهیم رضایی", "09129517169", "1", 12700000m, 20, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 11, 10, 0, 0), null, "باربری ترابر", 0, 1, "WB100056" },
                    { 57, null, 1, "شیراز", "94586", "6037990000000037", 37, "رضا حیدری", "09123770370", "4", 9200000m, 10, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 12, 15, 0, 0), null, "باربری امید", 0, 1, "WB100057" },
                    { 58, 18, 1, "همدان", "69488", "6037990000000017", 17, "جواد مرادی", "09125528972", "3", 10200000m, 16, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 13, 0, 0, 0), null, "باربری وطن", 0, 1, "WB100058" },
                    { 59, null, 1, "کرمان", "91275", "6037990000000016", 16, "حسین رستمی", "09129961380", "4", 9200000m, 10, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 14, 15, 0, 0), null, "باربری ترابر", 0, 1, "WB100059" },
                    { 60, 35, 1, "قم", "77357", "6037990000000027", 27, "سعید شریفی", "09121192619", "4", 9200000m, 20, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new TimeSpan(0, 15, 0, 0, 0), null, "باربری ترابر", 0, 1, "WB100060" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettingsList_CompanyId",
                table: "CompanySettingsList",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyId",
                table: "Drivers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_CompanyId",
                table: "Queues",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_DriverId",
                table: "Queues",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_ShippingCompanyId",
                table: "Queues",
                column: "ShippingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_CompanyId",
                table: "ShippingCompanies",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CompanySettingsList");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Queues");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "ShippingCompanies");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
