$(document).ready(function () {

    $("#cashoutBtn").hide();

    $.ajax({
        url: "/Game/GetActiveGame",
        type: "POST",
        beforeSend: function () {
            $("body > #loading-spinner").css("display", "flex");
        },
        success: function (response) {
            if (response.success === true) {
                // ✅ Update profit display & Cashout buttons
                $("#startGameBtn").hide();
                $("#cashoutBtn").show();
                
                $("#betAmount").prop("disabled", false);
                $("#minesCount").prop("disabled", false);
                
                let currentBetAmount = parseFloat(response.betAmountDisplay);
                $("#betAmount").val(currentBetAmount.toFixed(2));
                let currentMinesSelected = parseInt(response.minesSelectedDisplay);
                $("#minesCount").val(currentMinesSelected);
                let currentProfit = parseFloat(response.profitDisplay);
                $("#totalProfit").val(currentProfit.toFixed(2));

                $("#betAmount").prop("disabled", true);
                $("#minesCount").prop("disabled", true);
                
                // ✅ Parse and update specific buttons
                let tilesArray = response.tilesPosition.trimEnd(",").split(",").map(Number);
                tilesArray.forEach(index => {
                    $(`.mine-btn[data-index='${index}']`).text("💎");
                });

                // ✅ Remove 'disabled' class for unrevealed tiles
                $(".mine-btn").each(function () {
                    let tileIndex = $(this).data("index");
                    if (!tilesArray.includes(tileIndex)) {
                        $(this).removeClass("disabled");
                    }
                });
                toastr.success("Game in-progress.");
            } else {
                toastr.error(`${response.message}`);
            }
        },
        error: function () {
            toastr.error("Internal server error.");
        },
        complete: function () {
            $("body > #loading-spinner").css("display", "none");
        }
    });

});