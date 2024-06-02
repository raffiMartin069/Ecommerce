$(function() {
    const cusLogin = () => {
        if (!$("#cus_email").val()) {
            return false;
        }

        else if (!$("#cus_pass").val()) {
            return false;
        }
        return true;
    };

    $("#loginForm").on("submit", function (e) {
        e.preventDefault();
        if (!cusLogin()) {
            swal({
                title: "Oopss..Forgot something?",
                text: "Please fill up missing fields.",
                icon: "warning",
                button: "Continue",
            });
            return;
        }

        let formData = $(this).serialize();

        try {
            $.ajax({
                url: "../LogIn/LogIn",
                method: "POST",
                data: formData,
                success: function (data) {
                    if (!data.response) {
                        return swal({
                            title: "Log in failed",
                            text: data.content,
                            icon: "error",
                            button: "Continue",
                        });
                    } else {
                        window.location.href = data.redirectUrl;
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error("Error occured: ", jqXHR.responseText, jqXHR.status, jqXHR.statusText);
                }
            })
        }
        catch (e) {
            console.error(e);
        }

    })
})