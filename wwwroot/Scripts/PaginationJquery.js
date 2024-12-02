//Start Pengaturan Scroll Efek Pada Pagination Index
$(document).ready(function () {
    // Add a smooth scroll effect for better navigation
    $('.pagination-link, .pagination-link-block').on('click', function (e) {
        e.preventDefault();
        const targetUrl = $(this).attr('href');
        $('html, body').animate({ scrollTop: 0 }, 500, function () {
            window.location.href = targetUrl;
        });
    });
});
//End Pengaturan Scroll Efek Pada Pagination Index

//Start Pengaturan Double Click Pada Table Index
document.addEventListener("DOMContentLoaded", () => {
    const rows = document.querySelectorAll("tr[data-href]");

    rows.forEach(row => {
        row.addEventListener("dblclick", () => {
            window.location.href = row.dataset.href;
        });
    });
});
//End Pengaturan Double Click Pada Table Index

//Start Pengaturan Click Pada Button DIV Index
document.addEventListener("DOMContentLoaded", () => {
    const rows = document.querySelectorAll("div[data-href]");

    rows.forEach(row => {
        row.addEventListener("click", () => {
            window.location.href = row.dataset.href;
        });
    });
});
//End Pengaturan Click Pada Button DIV Index