$(function () {

    const disable = () => {
        let items = [
            "make", "model", "warranty", "update", "delete", "price", "quantity", "distributor", "desc"
        ]
        items.forEach((item) => {
            $("#" + item).prop("disabled", true);
        })
    }

    const enable = () => {
        let items = [
            "make", "model", "warranty", "update", "delete", "price", "quantity", "distributor", "desc"
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

    $('#quantity, #warranty').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $("#prodSettings").on("submit", function (e) {
        e.preventDefault();

        var selectedOptionValue = $("#distributor").val(); // This will give you the value of the selected option (i.e., the id)
        var selectedOptionText = $("#distributor").find('option:selected').text(); // This will give you the text of the selected option (i.e., the brand name)


        let formData = new FormData(this);


        formData.append("clickAction", actionValue);
        formData.append("distName", selectedOptionText);
        $.ajax({
            url: "https://localhost:44315/Store/ProductUpdate",
            method: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (!response.response) {
                    swal({
                        title: "Request failed.",
                        text: response.content,
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
                    /*$("#default").html(response.contents[6]);*/
                    $("#desc").val(response.contents[5]);

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