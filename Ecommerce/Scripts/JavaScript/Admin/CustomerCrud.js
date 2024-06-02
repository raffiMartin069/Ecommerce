$(function () {

    const disable = () => {
        let items = [
            "fname", "lname", "phone", "update", "delete", "email", "street", "brgy", "city", "province", "zip"
        ]
        items.forEach((item) => {
            $("#" + item).prop("disabled", true);
        })
    }

    const enable = () => {
        let items = [
            "fname", "lname", "phone", "update", "delete", "street", "brgy", "city", "province", "zip"
        ]
        items.forEach((item) => {
            $("#" + item).prop("disabled", false);
        })
    }

    disable();

    const validate = () => {
        let items = [
            "fname", "lname", "phone", "street", "brgy", "city", "province", "zip"
        ]
        for (let item of items) {
            if ($("#" + item).val().trim() === "") {
                swal({
                    title: "Validation Error",
                    text: `Please fill up all fields.`,
                    icon: "error",
                    button: "Continue",
                });
                return false;
            }
        }

        let zip = $("#zip").val().trim();
        if (zip.length !== 4) {
            swal({
                title: "Validation Error",
                text: `Zip code must be exactly 4 digits long.`,
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        let phone = $("#phone").val().trim();
        if (phone.length !== 11) {
            swal({
                title: "Validation Error",
                text: `Phone must be exactly 11 digits long.`,
                icon: "warning",
                button: "Continue",
            });
            return false;
        }
        return true;
    }

    var actionValue = "";
    $("#zip").on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 4);
    });

    $("#phone").on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 11);
    });


    // Capture the click event on the form buttons
    $('#prodSettings input[type="submit"]').click(function (event) {
        // Get the clicked button's value
        actionValue = $(this).val();
    });

    $("#prodSettings").on("submit", function (e) {
        e.preventDefault();

        if (actionValue != "Search") {
            if (!validate()) {
                return;
            }
        }

        let formData = new FormData(this);

        if (actionValue === "Delete") {
            let userConfirmed = confirm("Are you sure you want to delete this user?");
            if (!userConfirmed) {
                // User clicked Cancel, do not submit the form
                return;
            }
        }

        formData.append("clickAction", actionValue);

        

        $.ajax({
            url: "https://localhost:44315/Store/UserAdminAction",
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
                    $("#fname").val(response.contents[0].UserModel.USER_FNAME);
                    $("#lname").val(response.contents[0].UserModel.USER_LNAME);
                    $("#phone").val(response.contents[0].UserModel.USER_PHONE);
                    $("#email").val(response.contents[0].Credential.C_EMAIL);
                    $("#street").val(response.contents[0].Address.AD_STREET);
                    $("#brgy").val(response.contents[0].Address.AD_BRGY);
                    $("#city").val(response.contents[0].Address.AD_CITY);
                    $("#province").val(response.contents[0].Address.AD_PROVINCE);
                    $("#zip").val(response.contents[0].Address.AD_ZIPCODE);
                }

                if (response.response && response.action === "Update") {
                    swal({
                        title: "Information updated",
                        text: response.content,
                        icon: "success",
                        button: "Continue",
                    }).then((value) => {
                        // This code runs when the swal button is clicked
                        window.location.reload();
                    });
                }

                if (response.response && response.action === "Delete") {
                    swal({
                        title: "Account deleted succesfully!",
                        text: response.content,
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