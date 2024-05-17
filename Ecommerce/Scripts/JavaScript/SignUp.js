$(() => {
    $('#reg_sbmt').click(function (e) {
        e.preventDefault();

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

        if (isValid) {
            alert('Registration successful!');
        }
    });
})