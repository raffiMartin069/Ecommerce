$(function () {
            let btn = "";

            $(".update, .delete").on("click", function (e) {
                let btn = $(this).val();
                let quantity = parseInt($(this).siblings('.quantity').val());
                let cartItemId = parseInt($(this).siblings('.cartItemId').val());
                let productId = parseInt($(this).siblings('.prodId').val());
                let stocks = parseInt($(this).closest('.row').find('.stocks').val());

                if (quantity < 1) {
                    swal({
                        title: "Ooopss...",
                        text: "Quantity can not be lesser than 1. If you wish to remove click the Delete button below.",
                        icon: "error",
                        button: "Continue",
                    }).then((value) => {
                        location.reload();
                    });
                    return;
                }

                if (productId < 1) {
                    swal({
                        title: "Ooopss...",
                        text: "Something went wrong. We'll get back later.",
                        icon: "warning",
                        button: "Continue",
                    }).then((value) => {
                        location.reload();
                    });
                    return;
                }

                if (quantity > stocks) {
                    if (btn == "Update") {
                        swal({
                            title: "Review Quantity",
                            text: "Please ensure that placed quantity is not greater thank stocks.",
                            icon: "warning",
                            button: "Return",
                        }).then((value) => {
                            location.reload();
                        });
                        return false;
                    }
                }

                let formData = new FormData();
                formData.append("qty", quantity);
                formData.append("actionBtn", btn);
                formData.append("cartItemId", cartItemId);
                formData.append("productId", productId);

                $.ajax({
                    url: "../Home/UpdateDeleteItem",
                    method: "POST",
                    data: formData,
                    processData: false, 
                    contentType: false,
                    success: function (data) {
                        if (!data.response) {
                            swal({
                                title: "Something went wrong",
                                text: data.mess,
                                icon: "warning",
                                button: "Return",
                            }).then((value) => {
                                location.reload();
                            });
                            return;
                        }
                        swal({
                            title: "Operation successful",
                            text: data.mess,
                            icon: "success",
                            button: "Return",
                        }).then((value) => {
                            location.reload();
                        });
                        return;
                    },
                    error: function (error) {
                        console.error(error);
                    }
                })
            })
            $(".stocks").prop("disabled", true);

        })