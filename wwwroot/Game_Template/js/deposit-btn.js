$(document).ready(function () {
   
    function validateAmount(inputAmount) {
        if(inputAmount < 100){
            $("#inputAmountError").text("Minimum amount for deposit/withdraw is 100rs.").show();
            return false;
        } else {
            $("#inputAmountError").text("").hide();
            return true;
        }
    }
    
    $("#depositBtn").on("click", function (e) {
        e.preventDefault();
        let inputAmount = parseInt($("#inputAmount").val().trim());
        if(validateAmount(inputAmount) == false) return;

        $.ajax({
            url: "/Game/WalletDeposit",
            type: "POST",
            data: {
                amount : inputAmount
            },
            beforeSend: function () {
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    toastr.success("Money added to wallet!");
                } else {
                    toastr.warning("Invalid amount.");
                }
            },
            error: function () {
                toastr.error("Server down");
            },
            complete: function () {
                setTimeout(function () {
                    $("body > #loading-spinner").css("display", "none");
                }, 1000);
                location.reload();
            }
        });
    });

});