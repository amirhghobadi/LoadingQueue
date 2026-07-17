//============================
// شماره ماشین
//============================
function formatCarNumber(value) {

    if (!value)
        return "";

    value = value.toString().replace("-", "");

    if (value.length == 5)
        return value.substring(0, 2) + "-" + value.substring(2);

    return value;
}


//============================
// شماره تماس
//============================
//============================
// شماره تماس
// پشتیبانی از دو حالت:
// - ۱۱ رقمی با صفر اول: 09301812133 -> 0930-181-2133
// - ۱۰ رقمی بدون صفر اول: 9301812133 -> 930-181-2133
//============================
function formatPhone(value) {

    if (!value)
        return "";

    value = value.toString();

    if (value.length == 11) {

        return value.substring(0, 4) + "-" +
            value.substring(4, 7) + "-" +
            value.substring(7);

    }

    if (value.length == 10) {

        return value.substring(0, 3) + "-" +
            value.substring(3, 6) + "-" +
            value.substring(6);

    }

    return value;

}


//============================
// شماره کارت
//============================
function formatCard(value) {

    if (!value)
        return "";

    value = value.toString();

    if (value.length == 16)
        return value.match(/.{1,4}/g).join("-");

    return value;

}


//============================
// شماره شبا
// (باگ قبلی: "return" با اینتر از expression جدا بود، جاوااسکریپت
// خودکار یه ; بعدش می‌ذاشت و تابع همیشه undefined برمی‌گردوند.
// الان return و expression روی یک خط هستن.)
//============================
function formatSheba(value) {

    if (!value)
        return "";

    value = value.toString();

    if (value.startsWith("IR") && value.length == 26) {

        return value.substring(0, 4) + "-" +
            value.substring(4, 8) + "-" +
            value.substring(8, 12) + "-" +
            value.substring(12, 16) + "-" +
            value.substring(16, 20) + "-" +
            value.substring(20, 24) + "-" +
            value.substring(24);

    }

    return value;

}


//============================
// مبلغ
//============================
function formatMoney(value) {

    if (value == null || value == "")
        return "";

    return Number(
        value.toString().replace(/,/g, "")
    ).toLocaleString("en-US");

}


//============================
// ساعت
//============================
function formatTime(value) {

    if (!value)
        return "";

    value = value.toString().replace(":", "");

    if (value.length == 4) {

        return value.substring(0, 2) + ":" +
            value.substring(2);

    }

    return value;

}


//============================================
// ویرایشگر سفارشی برای فیلد عددی (کارتن کیک/آجیل)
//============================================
function emptyNumberEditor(cell, onRendered, success, cancel) {

    let input = document.createElement("input");

    input.type = "text";
    input.style.width = "100%";
    input.style.boxSizing = "border-box";
    input.value = cell.getValue() ?? "";

    onRendered(function () {
        input.focus();
        input.select();
    });

    function save() {

        let val = input.value.trim();

        success(val === "" ? null : Number(val));
    }

    input.addEventListener("blur", save);

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter")
            save();

        if (e.key === "Escape")
            cancel();

    });

    return input;
}


//============================================
// ویرایشگر سفارشی مبلغ کرایه
// فرق با قبل: الان حین تایپ (رویداد input) هم کاما جدا می‌کنه،
// نه فقط بعد از تموم شدن ویرایش. برای همینم دیگه لازم نیست تو
// queue-table.js دستی کاما رو قبل/بعد از ویرایش اضافه/حذف کنیم.
//============================================
function moneyLiveEditor(cell, onRendered, success, cancel) {

    let input = document.createElement("input");

    input.type = "text";
    input.style.width = "100%";
    input.style.boxSizing = "border-box";
    input.style.textAlign = "center";

    let initial = cell.getValue();

    input.value = initial
        ? Number(initial.toString().replace(/,/g, "")).toLocaleString("en-US")
        : "";

    onRendered(function () {
        input.focus();
        input.select();
    });

    // حین تایپ، کاماهای هزارگان زنده اضافه می‌شن
    input.addEventListener("input", function () {

        let raw = input.value.replace(/[^\d]/g, "");

        input.value = raw === ""
            ? ""
            : Number(raw).toLocaleString("en-US");

    });

    function save() {

        let raw = input.value.replace(/[^\d]/g, "");

        success(raw === "" ? null : Number(raw));
    }

    input.addEventListener("blur", save);

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter")
            save();

        if (e.key === "Escape")
            cancel();

    });

    return input;
}


//============================================
// ویرایشگر سفارشی شماره کارت / شبا
// - تو حالت نمایش عادی: با IR و فرمت کامل نشون داده می‌شه (توسط formatter در queue-table.js)
// - تو حالت ویرایش: IR نشون داده نمی‌شه، فقط رقم‌ها (تا راحت بشه تایپ کرد)
// - موقع ذخیره: اگه دقیقاً 24 رقم بود (بدون IR) خودکار IR اول اضافه می‌شه
//   اگه 16 رقم بود (شماره کارت) همونطور که هست ذخیره می‌شه
//============================================
function cardShebaEditor(cell, onRendered, success, cancel) {

    let input = document.createElement("input");

    input.type = "text";
    input.style.width = "100%";
    input.style.boxSizing = "border-box";
    input.style.textAlign = "center";

    let raw = cell.getValue() ?? "";

    // اگه از قبل IR داشت، برای راحتی ویرایش، IR رو موقتاً کنار می‌ذاریم
    if (raw.toString().startsWith("IR"))
        raw = raw.toString().substring(2);

    input.value = raw;

    onRendered(function () {
        input.focus();
        input.select();
    });

    function save() {

        let digits = input.value.replace(/[^\d]/g, "").trim();

        if (digits === "") {
            success(null);
            return;
        }

        // 24 رقم خالص شبا -> IR رو خودکار اول اضافه کن
        if (digits.length === 24) {
            success("IR" + digits);
            return;
        }

        // 16 رقم -> شماره کارت عادی
        success(digits);
    }

    input.addEventListener("blur", save);

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter")
            save();

        if (e.key === "Escape")
            cancel();

    });

    return input;
}

//============================================
// ویرایشگر سفارشی ساعت
// موقع کلیک روی سلول: بلافاصله فقط رقم‌ها نشون داده می‌شن و
// کامل انتخاب (select) می‌شن تا با تایپ رو هم بشن (مثلاً 1611)
// موقع ذخیره (Enter/خروج از سلول): به فرمت HH:mm برمی‌گرده (16:15)
//============================================
function timeEditor(cell, onRendered, success, cancel) {

    let input = document.createElement("input");

    input.type = "text";
    input.style.width = "100%";
    input.style.boxSizing = "border-box";
    input.style.textAlign = "center";

    let raw = (cell.getValue() ?? "").toString().replace(":", "");

    input.value = raw;

    onRendered(function () {
        input.focus();
        input.select();
    });

    function save() {

        let digits = input.value.replace(/[^\d]/g, "").trim();

        if (digits === "") {
            success(null);
            return;
        }

        if (digits.length === 3)
            digits = "0" + digits;

        if (digits.length !== 4) {
            // فرمت نامعتبره، مقدار قبلی رو نگه می‌داریم
            cancel();
            return;
        }

        success(digits.substring(0, 2) + ":" + digits.substring(2));
    }

    input.addEventListener("blur", save);

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter")
            save();

        if (e.key === "Escape")
            cancel();

    });

    return input;
}


//============================================
// ویرایشگر عمومی برای فیلدهای متنی ساده
// موقع کلیک روی سلول، کل متن داخلش انتخاب (select) می‌شه
// تا بشه بلافاصله با تایپ جایگزینش کرد.
//============================================
function selectAllEditor(cell, onRendered, success, cancel) {

    let input = document.createElement("input");

    input.type = "text";
    input.style.width = "100%";
    input.style.boxSizing = "border-box";
    input.style.textAlign = "center";
    input.value = cell.getValue() ?? "";

    onRendered(function () {
        input.focus();
        input.select();
    });

    function save() {
        success(input.value.trim());
    }

    input.addEventListener("blur", save);

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter")
            save();

        if (e.key === "Escape")
            cancel();

    });

    return input;
}