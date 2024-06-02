$(function () {
    const validation = () => {
        var inputs = ['reg_fname', 'reg_lname', 'reg_email', 'reg_phone', 'reg_street', 'reg_brgy', 'reg_city', 'reg_province', 'reg_zip', 'reg_pass'];
        var isValid = true;

        for (var i = 0; i < inputs.length; i++) {
            var input = $('#' + inputs[i]);
            if (!input.val()) {
                swal({
                    title: "Oops...",
                    text: `Please check for missing fields.`,
                    icon: "warning",
                    button: "Continue",
                });
                isValid = false;
                break;
            }
        }

        let zip = $("#reg_zip").val().trim();
        if (zip.length !== 4) {
            swal({
                title: "Validation Error",
                text: `Zip code must be exactly 4 digits long.`,
                icon: "warning",
                button: "Continue",
            });
            return false;
        }

        let reg_phone = $("#reg_phone").val().trim();
        if (reg_phone.length !== 11) {
            swal({
                title: "Validation Error",
                text: `Phone must be exactly 11 digits long.`,
                icon: "warning",
                button: "Continue",
            });
            return false;
        }
        return isValid;
    }

    $("#reg_zip").on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 4);
    });

    $("#reg_phone").on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 11);
    });

    $('#reg_phone, #reg_zip').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $("#register").on("submit", function (e) {
        e.preventDefault();
        if (!validation()) {
            return;
        }


        let formData = new FormData(this);

        $.ajax({
            url: "../LogIn/Register",
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
                swal({
                    title: "Welcome!",
                    text: response.content,
                    icon: "success",
                    button: "Continue",
                }).then((value) => {
                    // This code runs when the swal button is clicked
                    window.location.href = "https://localhost:44315/LogIn";
                });
            },
            fail: function (error) {
                console.error("Something went wrong: " + error);
            }
        })
    })
})