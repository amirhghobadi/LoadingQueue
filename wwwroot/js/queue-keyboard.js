// فوکوس اولیه
$(function () {

    $("#txtCarNumber").focus();

});


// جهت پایین
document.addEventListener("keydown", function (e) {

    if (e.key != "ArrowDown")
        return;

    let cell = table.getSelectedCells()[0];

    if (!cell)
        return;

    let next = cell.navigateDown();

    if (next)
        next.edit();

});


// جهت بالا
document.addEventListener("keydown", function (e) {

    if (e.key != "ArrowUp")
        return;

    let cell = table.getSelectedCells()[0];

    if (!cell)
        return;

    let prev = cell.navigateUp();

    if (prev)
        prev.edit();

});


// جهت چپ
document.addEventListener("keydown", function (e) {

    if (e.key != "ArrowLeft")
        return;

    let cell = table.getSelectedCells()[0];

    if (!cell)
        return;

    let prev = cell.navigateLeft();

    if (prev)
        prev.edit();

});


// جهت راست
document.addEventListener("keydown", function (e) {

    if (e.key != "ArrowRight")
        return;

    let cell = table.getSelectedCells()[0];

    if (!cell)
        return;

    let next = cell.navigateRight();

    if (next)
        next.edit();

});


// F2
document.addEventListener("keydown", function (e) {

    if (e.key != "F2")
        return;

    e.preventDefault();

    $("#txtCarNumber").focus();

});


// Ctrl+F
document.addEventListener("keydown", function (e) {

    if (!e.ctrlKey)
        return;

    if (e.key.toLowerCase() != "f")
        return;

    e.preventDefault();

    $("#txtWaybill").focus();

});


// ESC
document.addEventListener("keydown", function (e) {

    if (e.key == "Escape") {

        Swal.close();

    }

});


// Delete
document.addEventListener("keydown", function (e) {

    if (e.key != "Delete")
        return;

    let row = table.getSelectedRows()[0];

    if (!row)
        return;

    Swal.fire({

        title: "حذف شود؟",

        icon: "warning",

        showCancelButton: true,

        confirmButtonText: "بله",

        cancelButtonText: "خیر"

    }).then(function (r) {

        if (!r.isConfirmed)
            return;

        $.post("/Queue/Delete",
            {
                id: row.getData().id
            },
            function () {

                row.delete();

            });

    });

});