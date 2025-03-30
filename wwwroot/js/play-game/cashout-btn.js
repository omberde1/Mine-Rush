$(document).ready(function () {

    $("#cashoutBtn").on("click", function (e) {
        e.preventDefault();

        $.ajax({
            url: "/Game/CashoutGame",
            type: "POST",
            beforeSend: function () {
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    $("#cashoutBtn").hide();
                    $("#startGameBtn").show();
                    $("#betAmount").prop("disabled", false);
                    $("#minesCount").prop("disabled", false);
                    $(".mine-btn").addClass("disabled").text("❓").removeClass("clicked"); // IMPORTANT
                    let profitReset = parseFloat(0.00);
                    $("#totalProfit").val(profitReset.toFixed(2));

                    toastr.success("Cashout complete.");
                }
                else {
                    toastr.error(`${response.message}`);
                }
            },
            error: function () {
                toastr.error("Internal server error");
            },
            complete: function () {
                $("body > #loading-spinner").css("display", "none");
                // location.reload();
            }
        });

    });

});