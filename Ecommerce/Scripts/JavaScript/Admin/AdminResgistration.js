$(() => {

    const validation = () => {
        var inputs = ['reg_fname', 'reg_lname', 'reg_email', 'reg_phone', 'reg_street', 'reg_brgy', 'reg_city', 'reg_province', 'reg_zip', 'reg_pass'];
        var isValid = true;

        for (var i = 0; i < inputs.length; i++) {
            var input = $('#' + inputs[i]);
            if (!input.val()) {
                swal({
                    title: "You might have missed something!",
                    text: `Please fill missing fields.`,
                    icon: "warning",
                    button: "Continue",
                });
                isValid = false;
                return;
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
    

    $('#reg_sbmt').click(function () {

        if (!validation()) {
            return;
        }

        let formData = new FormData($("form")[0]);
        try {
            $.ajax({
                url: "../Store/RegisterAdmin",
                method: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: (data) => {
                    if (!data.response) {
                        return swal({
                            title: "Oopss..",
                            text: data.content,
                            icon: "fail",
                            button: "Continue",
                        });
                    }
                    return swal({
                        title: "All set!",
                        text: data.content,
                        icon: "success",
                        button: "Continue",
                    });

                },
                fail: (error) => {
                    console.error("Something went wrong " + error);
                }
            })
        } catch (e) {
            console.error("We are currently fixing the issue " + e);
        }
    });

    $("form").on("submit", (e) => {
        e.preventDefault();
    })
})