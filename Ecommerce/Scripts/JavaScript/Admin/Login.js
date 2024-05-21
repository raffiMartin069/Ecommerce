$(() => {
    $("#cus_sbmt").on("click", () => {

        let isValid = true;
        let obj = ["cus_email", "cus_pass"];

        for (let i = 0; i < obj.length; i++) {
            if (!$("#" + obj[i]).val()) {
                alert("Please supply the fields");
                isValid = false;
                break;
            }
        }
        // if condition is true then it will show the alert.
        if (isValid) {
            alert("Login Successful");
        }

    });
})