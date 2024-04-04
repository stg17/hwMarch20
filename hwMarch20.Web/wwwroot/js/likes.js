$(() => {

    const EnableButton = function () {
        let id = $("#image-id").val();
        $.get("/home/enablebutton", {id}, function (check) {
            $("#like-button").prop('disabled', check);
            console.log(check);
        })
    }


        $("#like-button").on('click', function () {

            let id = $("#image-id").val();
            $.post("/home/addlike", { id }, function () {
                UpdateLikes();
                EnableButton();
            })
        });

        const UpdateLikes = function () {
            let id = $("#image-id").val();
            $.get("/home/updatelikes", { id }, function (count) {
                $("#likes-count").text(count);
            });
        }

        EnableButton();


        setInterval(function () {
            UpdateLikes();
        }, 500);


    });