let driverTable = null;
let enterHandler = null;
let selected = false;
let searchDialogOpen = false;

//============================================
// با زدن Enter داخل باکس شماره ماشین، دیالوگ جستجو باز می‌شه
// (قبلاً هیچ‌جا این اتصال وجود نداشت و openDriverSearch هیچ‌وقت صدا زده نمی‌شد)
//============================================
$(function () {

    $("#txtCarNumber").on("keydown", function (e) {

        if (e.key !== "Enter")
            return;

        e.preventDefault();

        openDriverSearch();

    });

});


function openDriverSearch() {

    if (searchDialogOpen)
        return;

    searchDialogOpen = true;

    let car = $("#txtCarNumber").val().trim();

    if (car === "") {
        searchDialogOpen = false;
        return;
    }

    $.get("/Driver/Search",
        {
            carNumber: car
        },
        function (drivers) {

            if (drivers.length === 0) {

                searchDialogOpen = false;

                Swal.fire({
                    icon: "warning",
                    title: "راننده‌ای پیدا نشد"
                });

                return;
            }

            selected = false;

            Swal.fire({

                title: "انتخاب راننده",

                width: 900,

                html: "<div id='driverTable'></div>",

                showConfirmButton: false,

                willClose: function () {

                    searchDialogOpen = false;

                    if (enterHandler) {
                        document.removeEventListener("keydown", enterHandler);
                        enterHandler = null;
                    }

                    if (driverTable) {
                        driverTable.destroy();
                        driverTable = null;
                    }

                    selected = false;
                },

                didOpen: function () {

                    driverTable = new Tabulator("#driverTable", {

                        data: drivers,

                        index: "id",

                        layout: "fitColumns",

                        height: 420,

                        selectableRows: 1,

                        columnDefaults: {
                            vertAlign: "middle",
                            headerHozAlign: "center",
                            hozAlign: "center",
                            headerSort: false,
                            resizable: false
                        },

                        columns: [

                            {
                                title: "شماره ماشین",
                                field: "carNumber",
                                width: 120
                            },

                            {
                                title: "نام راننده",
                                field: "fullName",
                                widthGrow: 2
                            },

                            {
                                title: "شماره کارت / شبا",
                                field: "cardNumber",
                                minWidth: 240,
                                formatter: function (cell) {

                                    let v = cell.getValue();

                                    if (!v)
                                        return "";

                                    if (v.startsWith("IR"))
                                        return formatSheba(v);

                                    return formatCard(v);
                                }
                            },

                            {
                                title: "شماره تماس",
                                field: "phoneNumber",
                                width: 170,
                                formatter: function (cell) {

                                    return formatPhone(cell.getValue());

                                }
                            }

                        ]

                    });

                    driverTable.on("tableBuilt", function () {

                        let rows = driverTable.getRows();

                        if (rows.length)
                            rows[0].select();

                    });

                    driverTable.on("rowClick", function (e, row) {

                        row.select();

                    });

                    driverTable.on("rowDblClick", function (e, row) {

                        if (selected)
                            return;

                        selected = true;

                        createQueue(row.getData().id);

                    });

                    // باگ قبلی: شرط "if (searchDialogOpen) return" برعکس
                    // بود و همیشه اجرا می‌شد، یعنی انتخاب با Enter هیچ‌وقت
                    // به کد پایینش نمی‌رسید. الان درست شد.
                    enterHandler = function (e) {

                        if (e.key != "Enter")
                            return;

                        e.preventDefault();

                        if (selected)
                            return;

                        let rows = driverTable.getSelectedRows();

                        if (!rows.length)
                            return;

                        selected = true;

                        createQueue(rows[0].getData().id);

                    };

                    document.addEventListener("keydown", enterHandler);

                }

            });

        });

}


//============================================
// این تابع کلاً تو پروژه گم بود (نه تعریف شده بود نه صدا زده می‌شد
// جایی به‌درستی) - با دابل‌کلیک یا Enter روی یه راننده صدا زده می‌شه،
// یه نوبت خالی برای اون راننده می‌سازه و جدول اصلی رو رفرش می‌کنه.
//============================================
function createQueue(driverId) {

    $.post("/Queue/CreateEmpty",
        {
            driverId: driverId,
            date: selectedDate
        },
        function (res) {

            if (!res.success) {

                Swal.fire("خطا", res.message || "ثبت نوبت انجام نشد", "error");
                selected = false;
                return;
            }

            Swal.close();

            $("#txtCarNumber").val("");
            table.clearFilter(true);

            table.setData("/Queue/GetData?date=" + selectedDate).then(function () {

                let row = table.getRow(res.id);

                if (row) {

                    row.scrollTo();
                    row.getCell("shippingCompanyName").edit();
                }

            });

        }
    ).fail(function () {

        Swal.fire("خطا", "ارتباط با سرور برقرار نشد", "error");
        selected = false;

    });

}