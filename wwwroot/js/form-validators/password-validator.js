$(document).ready(function () {
    
    $("#togglePassword1").on("click", function () {
        let password = $("#password");
        let icon = $(this).find("i");

        if (password.attr("type") === "password") {
            password.attr("type", "text");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        } else {
            password.attr("type", "password");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        }
    });

    $("#togglePassword2").on("click", function () {
        let repassword = $("#rePassword");
        let icon = $(this).find("i");

        if (repassword.attr("type") === "password") {
            repassword.attr("type", "text");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        } else {
            repassword.attr("type", "password");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        }
    });

    $("#togglePassword3").on("click", function () {
        let repassword = $("#newPassword");
        let icon = $(this).find("i");

        if (repassword.attr("type") === "password") {
            repassword.attr("type", "text");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        } else {
            repassword.attr("type", "password");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        }
    });

});