$(() => {

    const validation = () => {
        var inputs = ['reg_fname', 'reg_lname', 'reg_email', 'reg_phone', 'reg_street', 'reg_brgy', 'reg_city', 'reg_province', 'reg_zip', 'reg_pass'];
        var isValid = true;

        for (var i = 0; i < inputs.length; i++) {
            var input = $('#' + inputs[i]);
            if (!input.val()) {
                alert('Please fill out the ' + input.attr('name') + ' field.');
                isValid = false;
                break;
            }
        }
        return isValid;
    }

    

    $('#reg_sbmt').click(function () {

        if (!validation()) {
            return;
        }

        let formData = new FormData($("form")[0]);
        try {
            $.ajax({
                url: "../Store/Register",
                method: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: (data) => {
                    if (!data) {
                        return swal({
                            title: "All set!",
                            text: "Registration failed!",
                            icon: "fail",
                            button: "Continue",
                        });
                    }
                    //return swal({
                    //    title: "All set!",
                    //    text: "Registration successful!",
                    //    icon: "success",
                    //    button: "Continue",
                    //});

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