$(document).ready(function () {

    $(".mine-btn").on("click", function () {
        let isDisabled = $(this).hasClass("disabled");
        if (isDisabled == true) return;

        let tilePosition = $(this).data("index");
        let clickedButton = $(this); // Store reference to the clicked button
        let shouldReload = false;

        $.ajax({
            url: "/Game/GameTileClick",
            type: "POST",
            data: {
                tileClickedPosition: tilePosition
            },
            beforeSend: function () {
                $("body > #loading-spinner-2").css("display", "flex");
            },
            success: function (response) {
                if (response.diamond) {
                    // ✅ If diamond is detected
                    clickedButton.html("💎");
                    clickedButton.addClass("clicked"); // Apply flip effect
                    setTimeout(function () {
                        clickedButton.addClass("disabled"); // Disable after animation
                    }, 300); // Delay for flip animation
                    
                    // Update profit value in input field
                    let newProfit = parseFloat(response.profit);
                    $("#totalProfit").val(newProfit.toFixed(2));
                }
                else if (!response.diamond) {
                    // ❌ If bomb is clicked
                    toastr.error("Game lost.");
                    clickedButton.html("💣");
                    clickedButton.addClass("clicked"); // Flip effect
                    $(".mine-btn").addClass("disabled").removeClass("clicked"); // IMPORTANT

                    setTimeout(function () {
                        clickedButton.addClass("disabled"); // Disable after animation
                        toastr.error("Game lost.");
                    }, 3000);

                    shouldReload = true;
                }
                else if (!response.success) {
                    // ❌ Invalid move or session error
                    toastr.error(`${response.message}`);
                }
                else {
                    toastr.error("Error occured.");
                }
            },
            error: function () {
                toastr.error("Internal server error.");
            },
            complete: function () {
                $("body > #loading-spinner-2").css("display", "none");
                if (shouldReload) {
                    setTimeout(function () {
                        location.reload();
                    }, 3000);
                }
            }
        });

    });

});