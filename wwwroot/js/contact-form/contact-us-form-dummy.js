$(document).ready(function () {
    function validateContactDummyForm() {
        let valid = true;
        let name = $("#name").val().trim();
        let email = $("#email").val().trim();
        let subject = $("#subject").val().trim();
        let message = $("#message").val().trim();

        // name error
        if(name.length < 3){
            $("#nameError").text("Minimum 3 characters.").show();
            valid = false;
        } else {
            $("#nameError").text("").hide();
        }

        // email error
        if(!email.match(/^\S+@\S+\.\S+$/)){
            $("#emailError").text("Enter a valid Email ID.").show();
            valid = false;
        } else {
            $("#emailError").text("").hide();
        }

        // subject error
        if(subject.length < 5){
            $("#subjectError").text("Minimum 5 characters.").show();
            valid = false;
        } else {
            $("#subjectError").text("").hide();
        }

        // message error
        if(message.length < 10){
            $("#messageError").text("Minimum 10 characters.").show();
            valid = false;
        } else {
            $("#messageError").text("").hide();
        }

        if(valid == true){
            $("#feedbackBtn").removeClass("disabled");
        } else {
            $("#feedbackBtn").addClass("disabled");
        }
        return valid;
    }

    $("#name, #email, #subject, #message").on("keyup", validateContactDummyForm);

    $("feedbackBtn").on("click", function (e) {
        e.preventDefualt();
        if(!validateContactDummyForm || $(this).hasClass("disabled")) {
            toastr.error("Fill all the details correctly.");
        } else{
            toastr.success("Feedback received.");
        }
        location.reload();
        return;
    })

});