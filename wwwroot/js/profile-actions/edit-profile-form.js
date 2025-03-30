$(document).ready(function () {
    function validateForm() {
        let valid = true;
        let username = $("#newUsername").val().trim();
        let email = $("#newEmail").val().trim();
        let password = $("#newPassword").val();

        // Username validation
        if (username.length < 3) {
            $("#newUsernameError").text("Minimum 3 characters.").show();
            valid = false;
        } else {
            $("#newUsernameError").text("").hide();
        }

        // Email validation
        if (!email.match(/^\S+@\S+\.\S+$/)) {
            $("#newEmailError").text("Enter valid email address.").show();
            valid = false;
        } else {
            $("#newEmailError").text("").hide();
        }

        // Password validation
        if (password.length < 6) {
            $("#newPasswordError").text("Password atleast have 6 characters.").show();
            valid = false;
        } else {
            $("#newPasswordError").text("").hide();
        }

        // Enable Edit Profile Button
        if (valid == true) {
            $("#editProfileBtn").removeClass("disabled");
        } else {
            $("#editProfileBtn").addClass("disabled");
        }
        return valid;
    }

    $("#newUsername, #newEmail, #newPassword").on("keyup", validateForm);

    $("#editProfileBtn").on("click", function (e) {
        e.preventDefault();

        if ($(this).hasClass("disabled")) return;
        if (!validateForm()) return;

        var playerData = {
            Username: $("#newUsername").val().trim(),
            Email: $("#newEmail").val().trim(),
            Password: $("#newPassword").val()
        }

        $.ajax({
            url: "/Game/ProfileEdit",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(playerData),
            // headers: { // Add anti-forgery token
            //     "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            // },
            beforeSend: function () {
                console.log("started.");
                $("body > #loading-spinner").css("display", "flex");
            },
            success: function (response) {
                // if (response.success) {
                //     console.log("Profile Updated!");
                //     toastr.success("Profile Updated!");
                // } else {
                //     console.log("This Email/Username already exists!");
                //     toastr.error("This Email/Username already exists!");
                // }
                alert(`${response.message}`);
            },
            error: function (xhr, status, error) {
                console.error("Error:", xhr.responseText);
                console.error("status:", status);
                console.error("error:", error);
            },
            complete: function () {  // <-- This runs AFTER success/error
                console.log("AJAX completed.");
                setTimeout(function () {
                    $("body > #loading-spinner").css("display", "none");
                }, 2000);
                // location.reload();
            }
        });

    });


});