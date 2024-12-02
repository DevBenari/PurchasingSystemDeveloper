$(document).ready(function () {
    // Add active class on menu link click
    $(".menu-link").on("click", function (e) {
        e.preventDefault(); // Prevent default navigation for demo purposes
        $(".menu-link").removeClass("active");
        $(this).addClass("active");

        // Redirect to the clicked link's href
        window.location.href = $(this).attr("href");
    });

    // Dropdown animation (Bootstrap already handles the dropdown toggle, adding smooth animation here)
    $(".dropdown-toggle").on("click", function () {
        const menu = $(this).next(".dropdown-menu");
        menu.stop(true, true).slideToggle(300); // Smooth slide animation
    });

    // Highlight menu based on URL (optional, for current page detection)
    const currentUrl = window.location.href;
    $(".menu-link").each(function () {
        if (currentUrl.includes($(this).attr("href"))) {
            $(this).addClass("active");
        }
    });
});