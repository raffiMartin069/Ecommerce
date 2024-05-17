$(function () {
    const phCities = () => {
        return new Promise((resolve, reject) => {
            $.get("https://psgc.gitlab.io/api/cities.json").done((data) => {
                resolve(data);
            }).fail((error) => {
                reject(error);
            });
        });
    }

    // API Call for Cities and Province in Philippines.
    // Promises ensure that the data is fetched before the next function is called.
    // This is to avoid the data not being fetched before the function is called.
    // It ensure that the the function runs parellel with other functions.
    const phProvinces = () => {
        return new Promise((resolve, reject) => {
            $.get("https://psgc.gitlab.io/api/provinces.json").done((data) => {
                resolve(data);
            }).fail((error) => {
                reject(error);
            });
        });
    }

    phCities().then((data) => {
        let response = JSON.parse(JSON.stringify(data));
        let cities = [];
        for (let i = 0; i < response.length; i++) {
            cities.push(response[i].name.replace(/City of /gi, ""));
        }

        // Sort the cities array
        cities.sort().forEach(city => {
            $("#city").append(`<option>${city}</option>`);
        });


    }).catch((error) => {
        console.error("Error fetching cities:", error);
    });


    phProvinces().then((data) => {
        let response = JSON.parse(JSON.stringify(data));
        let province = [];
        for (let i = 0; i < response.length; i++) {
            province.push(response[i].name);
        }
        province.sort().forEach(province => {
            $("#province").append(`<option>${province}</option>`);
        });
    }).catch((error) => {
        console.error("Error fetching provinces:", error);
    });

    const brands = () => {
        return new Promise((resolve, reject) => {
            $.post("/ProductEntry/MobileBrands/product/api").done((data) => {
                resolve(data)
            }).fail((error) => {
                reject(error);
            });
        })
    };

    //THIS IS THE API CALL FOR THE BRANDS
    // COMEBACK TO THIS LATER

    //brands().then((data) => {

    //    let brands = [];
    //    for (let i = 0; i < data.length; i++) {
    //        brands.push(data[i]);
    //        console.log(brands)
    //    }


    //    brands.forEach(brand => {
    //        $("#dist_list").append(`<option>${brand}</option>`);
    //    })
    //}).catch((error) => {
    //    console.error("Error fetching brands:", error);
    //});

});