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
            alert("Please fill all the fields");
            return;
        }

        let formData = $(this).serialize();

        try {
            $.ajax({
                url: "../LogIn/LogIn",
                method: "POST",
                data: formData,
                success: function (data) {
                    if (!data.status) {
                        return swal({
                            title: "Log in failed",
                            text: data.response,
                            icon: "fail",
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