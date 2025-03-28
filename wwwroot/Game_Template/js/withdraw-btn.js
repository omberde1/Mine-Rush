$(document).ready(function () {
   
    function validateAmount(inputAmount) {
        if(inputAmount < 100){
            $("#inputAmountError").text("Minimum amount for deposit/withdraw is 100rs.").show();
            return false;
        } else {
            $("#inputAmountError").text("").hide();
        }
        
        if(inputAmount > parseFloat($("#playerBalance").data("balance"))) {
            $("#inputAmountError").text("Withdraw money exceeds wallet balance.").show();
            return false;
        } else {
            $("#inputAmountError").text("").hide();
        }

        return true;
    }
    
    $("#withdrawBtn").on("click", function (e) {
        e.preventDefault();
        let inputAmount = parseFloat($("#inputAmount").val().trim());
        if(validateAmount(inputAmount) == false) return;

        $.ajax({
            url: "/Game/WalletWithdraw",
            type: "POST",
            data: {
                amount : inputAmount
            },
            beforeSend: function () {
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    toastr.success("Money withdrawn successfully!");
                } else {
                    toastr.warning("Invalid amount.");
                }
            },
            error: function () {
                toastr.error("Server down");
            },
            complete: function () {
                $("body > #loading-spinner").css("display", "none");
                location.reload();
            }
        });
    });

});