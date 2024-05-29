$(function () {

    const disable = () => {
        let items = [
            "make", "model", "warranty", "update", "delete", "price", "quantity"
        ]
        items.forEach((item) => {
            $("#" + item).prop("disabled", true);
        })
    }

    const enable = () => {
        let items = [
            "make", "model", "warranty", "update", "delete", "price", "quantity"
        ]
        items.forEach((item) => {
            $("#" + item).prop("disabled", false);
        })
    }

    disable();


    var actionValue = "";

    // Capture the click event on the form buttons
    $('#prodSettings input[type="submit"]').click(function (event) {
        // Get the clicked button's value
        actionValue = $(this).val();
    });

    $("#prodSettings").on("submit", function(e) {
        e.preventDefault();

        let formData = new FormData(this);
        formData.append("action", actionValue);
        $.ajax({
            url: "../Store/ProductUpdate",
            method: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (!response.response) {
                    swal({
                        title: "Request failed.",
                        text: response.response,
                        icon: "error",
                        button: "Continue",
                    });
                    return;
                }

                if (response.response && response.action === "Search") {
                    enable();
                    $("#make").val(response.contents[0]);
                    $("#model").val(response.contents[1]);
                    $("#warranty").val(response.contents[2]);
                    $("#quantity").val(response.contents[4]);
                    $("#price").val(response.contents[3]);
                    console.table(response.contents)
                    
                }

                if (response.response && response.action === "Update") {
                    swal({
                        title: "Product Update",
                        text: "Information Updated!",
                        icon: "success",
                        button: "Continue",
                    }).then((value) => {
                        // This code runs when the swal button is clicked
                        window.location.reload();
                    });
                }

                if (response.response && response.action === "Delete") {
                    swal({
                        title: "Product Update",
                        text: "Information Updated!",
                        icon: "success",
                        button: "Continue",
                    }).then((value) => {
                        // This code runs when the swal button is clicked
                        window.location.reload();
                    });
                }

            },
            failed: function (error) {
                console.error(error);
            }
        })
    })
})