$(document).ready(function () {
    function validateForm() {
        let username = $("#username").val().trim();
        let email = $("#email").val().trim();
        let password = $("#password").val();
        let rePassword = $("#rePassword").val();
        let valid = true;

        // Username validation
        if (username.length < 3) {
            $("#usernameError").text("Minimum 3 characters.").show();
            valid = false;
        } else {
            $("#usernameError").text("").hide();
        }
        
        // Email validation
        if (!email.match(/^\S+@\S+\.\S+$/)) {
            $("#emailError").text("Enter valid email address.").show();
            valid = false;
        } else {
            $("#emailError").text("").hide();
        }
        
        // Password validation
        if (password.length < 6) {
            $("#passwordError").text("Password atleast have 6 characters.").show();
            valid = false;
        } else {
            $("#passwordError").text("").hide();
        }
        
        // Repassword validation
        if (rePassword !== password) {
            $("#rePasswordError").text("Passwords do not match.").show();
            valid = false;
        } else {
            $("#rePasswordError").text("").hide();
        }

        // Enable Register Button
        if (valid == true) {
            $("#registerBtn").removeClass("disabled");
        } else {
            $("#registerBtn").addClass("disabled");
        }

        return valid;
    }
    
    $("#username, #email, #password, #rePassword").on("keyup", validateForm);
    
    $("#registerBtn").on("click", function (e) {
        e.preventDefault();
        // if ($("#registerBtn").is(":disabled")) return;
        
        if ($(this).hasClass("disabled")) return;
        if (!validateForm()) return;

        $.ajax({
            url: "/Game/Register",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Username: $("#username").val().trim(),
                Email: $("#email").val().trim(),
                Password: $("#password").val()
            }),
            beforeSend: function () {
                console.log("started.");
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                if (response.success) {
                    console.log("Account created!");
                    toastr.success("Account created!");
                } else {
                    console.log("Email/Username already exists!");
                    toastr.error("Email/Username already exists!");
                }
            },
            error: function () {
                console.log("Server down");
                toastr.error("Server down");
            },
            complete: function () {  // <-- This runs AFTER success/error
                console.log("AJAX completed.");
                $("body > #loading-spinner").css("display", "none");
            }
        });

    });


});