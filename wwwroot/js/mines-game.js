$(document).ready(function () {

    console.log("Yo! Play game");

    
    $(".mine-btn").on("click", function () {
        $(this).addClass("clicked");
        $(this).html("💎");
        console.log("You clicked.");
    });
 
});