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
        let fname = $("#user-fname").val();
        let lname = $("#user-lname").val();
        let mname = $("#user-mname").val();
        let email = $("#user-email").val();
        let phone = $("#user-phone").val();

        if (!TextFieldControl(fname)) {
            alert("First name is required.");
            return;
        }

        if (!TextFieldControl(lname)) {
            alert("Last name is required.");
            return false;
        }

        if (!TextFieldControl(mname)) {
            alert("Middle name is required.");
            return false;
        }

        if (!TextFieldControl(email)) {
            alert("Email is required.");
            return false;
        }

        if (!TextFieldControl(phone)) {
            alert("Phone number is required.");
            return false;
        }
        return true;
    }

    function userAddressControl() {
        let line = $("#line1").val();
        /*let line2 = $("#line2").val();*/
        let city = $("#city").val();
        let province = $("#province").val();
        let zipCode = $("#zipcode").val();

        if (!TextFieldControl(line)) {
            alert("Address is required.");
            return false;
        }

        if (!TextFieldControl(city)) {
            alert("City is required.");
            return false;
        }

        if (!TextFieldControl(province)) {
            alert("Province is required.");
            return false;
        }

        if (!TextFieldControl(zipCode)) {
            alert("Zip code is required.");
            return false;
        }
        return true;
    }

    function productControl() {
        let make = $("#make").val();
        let model = $("#model").val();
        let modelId = $("#mod-id").val();
        let distributor = $("#vendor").val();
        let purchaseDate = $("#purchase-date").val();

        if (!TextFieldControl(make)) {
            alert("Make is required.");
            return false;
        }

        if (!TextFieldControl(model)) {
            alert("Model is required.");
            return false;
        }

        if (!TextFieldControl(modelId)) {
            alert("Model ID is required.");
            return false;
        }

        if (!TextFieldControl(distributor)) {
            alert("Vendor is required.");
            return false;
        }

        if (!TextFieldControl(purchaseDate)) {
            alert("Purchase date is required.");
            return false;
        }

        let price = $("#price").val();
        let quantity = $("#quantity").val();
        let warranty = $("#warranty").val();

        if (!TextFieldControl(price)) {
            alert("Price is required.");
            return false;
        }

        if (!TextFieldControl(quantity)) {
            alert("Quantity is required.");
            return false;
        }

        if (!TextFieldControl(warranty)) {
            alert("Warranty is required.");
            return false;
        }
        return true;
    }

    $("#entry-form").on("submit", function (e) {
        e.preventDefault();

        if (!userControl()) {
            return;
        } else if (!userAddressControl()) {
            return;
        } else if (!productControl()) {
            return;
        }

        $.post("../ProductEntry/UserApi/product/api", {
            data: 1
        }).done((data) => {

        }).fail((error) => {
            console.error(error);
        });
        
        
        this.submit();
        $("#entry-form")[0].reset();
    })
})