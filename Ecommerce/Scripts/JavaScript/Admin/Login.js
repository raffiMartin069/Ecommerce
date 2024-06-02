$(function() {
    $("#login_form").on("submit", function (e) {
        e.preventDefault();

        if (!$("#cus_email").val()) {
            swal({
                title: "Ooopss...",
                text: "Please fill up email.",
                icon: "warning",
                button: "Continue",
            });
            return;
        }

        if (!$("#cus_pass").val()) {
            swal({
                title: "Ooopss...",
                text: "Please fill up password.",
                icon: "warning",
                button: "Continue",
            });
            return;
        }

        let formData = new FormData(this);
        $.ajax({
            url: "../Store/AdminAuthentication",
            method: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                if (!data.response) {
                    swal({
                        title: "Ooopss...",
                        text: data.mess,
                        icon: "warning",
                        button: "Continue",
                    });
                    return;
                }
                window.location.href = data.redirectUrl;
            },
            error: function (error) {
                console.error(error);
            }
        })

    })

})