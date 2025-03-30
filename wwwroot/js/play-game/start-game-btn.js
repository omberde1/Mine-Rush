$(document).ready(function () {

    $("#startGameBtn").on("click", function (e) {
        e.preventDefault();
        let _betAmount = parseFloat($("#betAmount").val());
        let _minesCount = parseInt($("#minesCount").val());

        $.ajax({
            url: "/Game/StartGame",
            type: "POST",
            data: {
                betAmount: _betAmount,
                minesCount: _minesCount
            },
            beforeSend: function () {
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    $("#startGameBtn").hide();
                    $("#cashoutBtn").show();
                    $("#betAmount").prop("disabled", true);
                    $("#minesCount").prop("disabled", true);
                    $(".mine-btn").removeClass("disabled");
                    
                    toastr.success("Game started...");
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
            }
        });

    });

});