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
            swal({
                title: "Oopss...",
                text: "Please enter your I.D.",
                icon: "warning",
                button: "Continue",
            });
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
            swal({
                title: "Oopss...",
                text: "Make is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (!TextFieldControl(model)) {
            swal({
                title: "Oopss...",
                text: "Model is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (!TextFieldControl(distributor)) {
            swal({
                title: "Oopss...",
                text: "Distributor is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (!TextFieldControl(purchaseDate)) {
            swal({
                title: "Oopss...",
                text: "Purchased date is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        let price = $("#price").val();
        let quantity = $("#quantity").val();
        let warranty = $("#warranty").val();
        let img = $("#prodimg")[0];

        if (!TextFieldControl(price)) {
            swal({
                title: "Oopss...",
                text: "Price is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        } else if (price <= 0) {
            swal({
                title: "Oopss...",
                text: "Price can not be equal or lesser than zero",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (!TextFieldControl(quantity)) {
            swal({
                title: "Oopss...",
                text: "Quantity is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        } else if (quantity <= 0) {
            swal({
                title: "Oopss...",
                text: "Quanttiy can not be zero or lesser than zero.",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (!TextFieldControl(warranty)) {
            swal({
                title: "Oopss...",
                text: "Warranty is required",
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        if (img.files.length === 0) {
            swal({
                title: "Oopss...",
                text: "Image is required",
                icon: "warning",
                button: "Continue",
            });
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