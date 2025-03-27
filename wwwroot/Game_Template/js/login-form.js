$(document).ready(function () {

    function validateForm() {
        let username = $("#username").val().trim();
        let password = $("#password").val();
        let valid = true;

        // Username validation
        if (username.length < 3) {
            $("#usernameError").text("Enter a valid username/email.").show();
            valid = false;
        } else {
            $("#usernameError").text("").hide();
        }

        // Password validation
        if (password.length < 6) {
            $("#passwordError").text("Password must have at least 6 characters.").show();
            valid = false;
        } else {
            $("#passwordError").text("").hide();
        }

        // Enable Register Button
        if (valid == true) {
            $("#loginBtn").removeClass("disabled");
        } else {
            $("#loginBtn").addClass("disabled");
        }

        return valid;
    }

    $("#username, #password").on("keyup", validateForm);

    $("#loginBtn").on("click", function (e) {
        e.preventDefault();

        if ($(this).hasClass("disabled")) return;
        if (!validateForm()) return; // Validate before submitting



        $.ajax({
            url: "/Game/Login",
            type: "POST",
            data: {
                username_email: $("#username").val().trim(),
                password: $("#password").val()
            },
            beforeSend: function () {
                console.log("started.");
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    console.log("LoggedIn");
                    location.reload();
                } else {
                    console.log("Wrong crdentials");
                    toastr.error("Wrong crdentials");
                }
            },
            error: function () {
                toastr.error("Server down");
            },
            complete: function () {
                console.log("AJAX completed.");
                $("body > #loading-spinner").css("display", "none");
            }
        });
    })


});