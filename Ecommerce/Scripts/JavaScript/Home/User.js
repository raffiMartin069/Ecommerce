$(document).ready(function () {
    var maxHeight = 0;

    $(".card").each(function () {
        if ($(this).height() > maxHeight) { maxHeight = $(this).height(); }
    });

    $(".card").height(maxHeight);

    // Use class instead id to bound all id's in one button.
    // they are dynamic since we are looping the data.
    $(".addToCart").on("click", function () {
        let productId = $(this).val();
        let qty = $(this).closest('.card').find('.quantity').val();
        let stocks = $(this).closest('.card').find('.stocks').val();

        if (parseInt(qty) > parseInt(stocks)) {
            swal({
                title: "Review Quantity",
                text: "Please ensure that placed quantity is not greater thank stocks.",
                icon: "warning",
                button: "Return",
            });
            let qty = $(this).closest('.card').find('.quantity').val('');
            return;
        }

        if (qty === null || qty === "" || qty === 0) {
            swal({
                title: "Forgot something?",
                text: "Please enter a quantity",
                icon: "warning",
                button: "Return",
            });
            let qty = $(this).closest('.card').find('.quantity').val('');
            return;
        }

        $.ajax({
            url: "../Home/AddToCart",
            method: "GET",
            data: {
                'id': productId,
                'prodQty': qty,
                'prodStock': stocks
            },
            success: (response) => {
                if (!response) {
                    console.log(response)
                    swal({
                        title: "Hmmm...",
                        text: "Please check either quantity is greater than stocks or stocks have missing input.",
                        icon: "error",
                        button: "Continue",
                    });
                    let qty = $(this).closest('.card').find('.quantity').val('');
                    return;
                }
                swal({
                    title: "Thank you!",
                    text: "Item added to cart.",
                    icon: "success",
                    button: "Continue",
                });
                let qty = $(this).closest('.card').find('.quantity').val('');
                return;
            },
            error: (error) => {
                swal({
                    title: "Something went wrong",
                    text: "Don't worry, we will do our best to fix it!",
                    icon: "error",
                    button: "Continue",
                });
                let qty = $(this).closest('.card').find('.quantity').val('');
                return;
            }
        })
    })

    $(".stocks").prop("disabled", true);

    $(".stocks").on("click", function () {
        console.log("I am enabled");
    });

    $('.quantity').on('input', function () {
        if ($(this).val() < 1) {
            $(this).val('');
        }
    });
});