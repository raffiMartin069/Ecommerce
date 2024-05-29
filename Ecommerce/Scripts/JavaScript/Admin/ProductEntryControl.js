$(function () {
    const charDisable = function () {
        // Disables characters since in the mark up it is set into a type="text".
        // This also avoids user from entering a math character such as +, -, *, /, etc.
        $('#user-phone').on('input', function () {
            var value = $(this).val();
            var mask_value = value.replace(/\D/g, '');
            $(this).val(mask_value);
        });

        $('#price').on('input', function () {
            var value = $(this).val();
            var mask_value = value.replace(/\D/g, '');
            $(this).val(mask_value);
        });

        $('#quantity').on('input', function () {
            var value = $(this).val();
            var mask_value = value.replace(/\D/g, '');
            $(this).val(mask_value);
        });
    }

    charDisable();

    function TextFieldControl(x) {
        if (x === "") {
            return false;
        }
        if (x === null) {
            return false;
        }
        if (x === undefined) {
            return false;
        }
        return true;
    }

    function userControl() {
        let userId = $("#userId").val();
        if (!TextFieldControl(userId)) {
            alert("I.D. is required.");
            return;
        }

        return true;
    }

    function productControl() {
        let make = $("#make").val();
        let model = $("#model").val();
        let distributor = $("#distributor").val();
        let purchaseDate = $("#purchase-date").val();

        if (!TextFieldControl(make)) {
            alert("Make is required.");
            return false;
        }

        if (!TextFieldControl(model)) {
            alert("Model is required.");
            return false;
        }

        if (!TextFieldControl(distributor)) {
            alert("Distributor is required.");
            return false;
        }

        if (!TextFieldControl(purchaseDate)) {
            alert("Purchase date is required.");
            return false;
        }

        let price = $("#price").val();
        let quantity = $("#quantity").val();
        let warranty = $("#warranty").val();
        let img = $("#prodimg")[0];

        if (!TextFieldControl(price)) {
            alert("Price is required.");
            return false;
        } else if (price <= 0) {
            alert("Price must be greater than 0.00.");
        }

        if (!TextFieldControl(quantity)) {
            alert("Quantity is required.");
            return false;
        } else if (quantity <= 0) {
            alert("Quantity must be greater than 0.");
            return false;
        }

        if (!TextFieldControl(warranty)) {
            alert("Warranty is required.");
            return false;
        }

        if (img.files.length === 0) {
            alert("Image is required.");
            return false;
        }
        return true;
    }

    $("#entry-form").on("submit", function (e) {
        e.preventDefault();
        if (!userControl()) {
            return;
        } else if (!productControl()) {
            return;
        }
        
        let formData = new FormData(this);
        // Ajax request
        try {
            $.ajax({
                url: "../Store/Entries",
                method: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: (response) => {
                    if (!response.success) {
                         swal({
                            title: "Failed to register",
                            text: response.response,
                            icon: "error",
                            button: "Continue",
                         });
                        return;
                    }
                    swal({
                        title: response.title,
                        text: response.response,
                        icon: "success",
                        button: "Continue",
                    });
                    $('#entry-form')[0].reset();
                    return;
                },
                fail: () => {
                    console.error("Something went wrong!");
                }
            })
        } catch (e) {
            console.error("Error message product entry: " + e);
        }
    })
})