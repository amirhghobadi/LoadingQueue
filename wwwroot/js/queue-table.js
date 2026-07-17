//============================================
// متغیرهای سراسری
//============================================
var table;
var shippingCompanyNames = [];


//============================================
// اول لیست باربری‌های فعال شرکت جاری رو می‌گیریم، بعد جدول رو می‌سازیم
// (چون دراپ‌داون ستون «باربری» به این لیست نیاز داره)
//============================================
$.get("/ShippingCompany/GetActiveNames", function (names) {

    shippingCompanyNames = names || [];

    initQueueTable();

}).fail(function () {

    // اگه به هر دلیلی درخواست باربری‌ها Fail شد، بازم جدول رو با
    // لیست خالی بساز تا کل صفحه از کار نیفته
    shippingCompanyNames = [];

    initQueueTable();

});


//============================================
// تعریف اصلی جدول Tabulator صف بارگیری
//============================================
function initQueueTable() {

    table = new Tabulator("#queueTable", {

        ajaxURL: "/Queue/GetData",

        ajaxConfig: "GET",

        layout: "fitColumns",

        responsiveLayout: false,

        height: "calc(100vh - 165px)",

        movableColumns: true,

        movableRows: true,

        selectableRows: 1,

        editTriggerEvent: "dblclick",

        pagination: true,

        paginationSize: 100,

        index: "id",

        placeholder: "اطلاعاتی وجود ندارد.",

        columnDefaults: {

            vertAlign: "middle",

            headerHozAlign: "center",

            hozAlign: "center",

            headerSort: false,

            resizable: false

        },

        columns: [

            {
                title: "#",
                formatter: "rownum",
                rowHandle: true,
                width: 45,
                frozen: true,
                cssClass: "drag-handle-cell"
            },

            {
                title: "شماره نوبت",
                field: "queueNumber",
                width: 110,
                frozen: true
            },

            {
                title: "وضعیت",
                field: "status",
                width: 130,
                frozen: true,

                editor: "list",
                editorParams: {
                    values: {
                        1: "در انتظار",
                        2: "در حال بارگیری",
                        3: "تکمیل شده",
                        4: "لغو شده"
                    }
                },

                formatter: function (cell) {

                    let v = cell.getValue();

                    let map = {
                        1: { text: "در انتظار", cls: "badge bg-warning text-dark" },
                        2: { text: "در حال بارگیری", cls: "badge bg-info text-dark" },
                        3: { text: "تکمیل شده", cls: "badge bg-success" },
                        4: { text: "لغو شده", cls: "badge bg-danger" }
                    };

                    let s = map[v] || { text: "-", cls: "badge bg-secondary" };

                    return `<span class="${s.cls}">${s.text}</span>`;
                }
            },

            {
                title: "باربری",
                field: "shippingCompanyName",
                minWidth: 130,
                frozen: true,

                editor: "list",
                editorParams: {
                    values: shippingCompanyNames,
                    clearable: true
                }
            },

            {
                title: "شماره بارنامه",
                field: "waybillNumber",
                editor: selectAllEditor,
                minWidth: 100
            },

            {
                title: "شماره کارت / شبا",
                field: "driverCardNumber",
                editor: cardShebaEditor,
                formatter: function (cell) {

                    let v = cell.getValue();

                    if (v && v.startsWith("IR"))
                        return formatSheba(v);

                    return formatCard(v);
                },
                minWidth: 240
            },

            {
                title: "راننده",
                field: "driverName",
                editor: selectAllEditor,
                minWidth: 170
            },

            {
                title: "کرایه",
                field: "freightAmount",
                editor: moneyLiveEditor,
                formatter: function (cell) {

                    return formatMoney(cell.getValue());
                },
                width: 120
            },

            {
                title: "مقصد",
                field: "destination",
                editor: selectAllEditor,
                minWidth: 130
            },

            {
                title: "شماره ماشین",
                field: "driverCarNumber",
                editor: selectAllEditor,
                formatter: function (cell) {

                    return formatCarNumber(cell.getValue());
                },
                width: 100
            },

            {
                title: "ساعت",
                field: "queueTime",
                editor: timeEditor,
                formatter: function (cell) {

                    return formatTime(cell.getValue());
                },
                width: 70
            },

            {
                title: "کیک",
                field: "cakeCartonCount",
                editor: emptyNumberEditor,
                width: 60
            },

            {
                title: "آجیل",
                field: "nutCartonCount",
                editor: emptyNumberEditor,
                width: 60
            },

            {
                title: "خروجی",
                field: "exitNumber",
                editor: selectAllEditor,
                width: 80
            },

            {
                title: "شماره تماس",
                field: "driverPhone",
                editor: selectAllEditor,
                formatter: function (cell) {

                    return formatPhone(cell.getValue());
                },
                minWidth: 110
            }

        ]

    });


    table.on("cellEdited", function (cell) {

        // وضعیت از یه Endpoint جدا (ChangeStatus) ذخیره می‌شه، نه UpdateCell معمولی
        if (cell.getField() === "status") {

            $.post("/Queue/ChangeStatus",
                {
                    id: cell.getRow().getData().id,
                    status: cell.getValue()
                }
            ).fail(function () {

                Swal.fire("خطا", "تغییر وضعیت ذخیره نشد", "error");

            });

            return;
        }

        $.ajax({

            url: "/Queue/UpdateCell",

            type: "POST",

            data: {
                id: cell.getRow().getData().id,
                field: cell.getField(),
                value: cell.getValue()
            },

            error: function () {

                Swal.fire("خطا", "ذخیره انجام نشد", "error");

            }

        });

    });


    //============================================
    // ردیف خالی همیشه در پایین جدول (برای ثبت نوبت جدید)
    //============================================
    table.on("dataLoaded", function () {
        addEmptyRow();
    });


    //============================================
    // ذخیره‌ی ترتیب جدید بعد از جابجایی ردیف با درگ
    //============================================
    table.on("rowMoved", function () {

        let ids = table.getRows()
            .map(r => r.getData().id)
            .filter(id => id !== -1);

        $.ajax({

            url: "/Queue/Reorder",

            type: "POST",

            contentType: "application/json",

            data: JSON.stringify(ids),

            error: function () {

                Swal.fire("خطا", "ذخیره ترتیب انجام نشد", "error");

            }

        });

    });


    //============================================
    // انتخاب پایدار ردیف با کلیک (کلیک دوم روی ردیف انتخاب‌شده،
    // انتخاب رو toggle/پاک نکنه)
    //============================================
    table.on("rowClick", function (e, row) {

        if (!row.isSelected()) {
            row.select();
        }

    });

}


//============================================
// ردیف خالی همیشه در پایین جدول (برای ثبت نوبت جدید)
//============================================
function addEmptyRow() {

    let rows = table.getRows();

    if (rows.length == 0)
        return;

    let last = rows[rows.length - 1].getData();

    if (last.id === -1)
        return;

    table.addRow({
        id: -1,
        queueNumber: "",
        status: 1,
        shippingCompanyName: "",
        waybillNumber: "",
        driverCardNumber: "",
        driverName: "",
        freightAmount: null,
        destination: "",
        driverCarNumber: "",
        queueTime: "",
        cakeCartonCount: null,
        nutCartonCount: null,
        exitNumber: "",
        driverPhone: ""
    }, false);

}