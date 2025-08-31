function loadMovies(url = null) {
    var search = $("input[name='searchString']").val();
    var category = $("select[name='categoryId']").val();

    url = url || $("form[data-ajax='true']").attr("action");

    $("#moviesContainer").fadeTo(200, 0.5);

    $.ajax({
        url: url,
        type: "GET",
        data: { searchString: search, categoryId: category },
        success: function (result) {
            $("#moviesContainer").html(result).fadeTo(200, 1);
        },
        error: function () {
            $("#moviesContainer").html("<div class='alert alert-danger'>⚠️ Something went wrong. Try again!</div>");
        }
    });
}

$(document).ready(function () {
    // 🔎 Live Search (debounce)
    let typingTimer;
    $("input[name='searchString']").on("keyup", function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(() => loadMovies(), 400);
    });

    // 📂 Category Change
    $("select[name='categoryId']").on("change", function () {
        loadMovies();
    });

    // 📄 Pagination Ajax
    $(document).on("click", ".page-link-ajax", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        loadMovies(url);
    });

    // 🎛 Toggle Filter
    $("#toggleFilter").on("click", function () {
        $("#filterPanel").slideToggle();
    });

    // 🗂 Toggle Grid/List
    $("#toggleView").on("click", function () {
        $(".movies-wrapper").toggleClass("list-view grid-view");
    });
});
