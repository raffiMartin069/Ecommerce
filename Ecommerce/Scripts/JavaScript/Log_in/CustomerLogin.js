$(() => {
    const cusLogin = () => {
        if (!$("#cus_email").val()) {
            return false;
        }

        else if (!$("#cus_pass").val()) {
            return false;
        }
        return true;
    };

    const sbmt = () => {
        $("#cus_sbmt").on("click", (e) => {
            e.preventDefault();
            if (!cusLogin()) {
                alert("Please fill all the fields");
                return;
            }
            alert("Login Successful");
        });
    };

    sbmt();
})