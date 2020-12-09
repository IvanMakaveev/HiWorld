$('.likeButton').submit(function (event) {
    event.preventDefault();
    var antiForgeryToken = $(this).find('input[name=__RequestVerificationToken]').val();
    var form = $(this);
    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        success: function (data) {
            if (form.attr('liked') === "false") {
                form.attr('liked', "true")
                form.find("button").removeClass('btn-outline-primary').addClass('btn-outline-danger').html('Dislike <i class="fas fa-heart-broken"></i>');
                form.find("span").text(parseInt(form.find("span").text()) + 1)
            }
            else {
                form.attr('liked', "false")
                form.find("button").removeClass('btn-outline-danger').addClass('btn-outline-primary').html('Like <i class="fas fa-heart"></i>');
                form.find("span").text(parseInt(form.find("span").text()) - 1)
            }
        },
    });
});

$('.deleteButton').submit(function (event) {
    event.preventDefault();
    var antiForgeryToken = $(this).find('input[name=__RequestVerificationToken]').val();
    var form = $(this);
    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        success: function (data) {
            form.parent().parent().remove()
        },
    });
});

$('.commentButton').submit(function (event) {
    event.preventDefault();
    var antiForgeryToken = $(this).find('input[name=__RequestVerificationToken]').val();
    var form = $(this);
    var data = form.serialize()
    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        data: data,
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        success: function (data) {
            var id = data.profileId
            var firstName = data.profileFirstName
            var lastName = data.profileLastName
            var text = data.text
            form.parent().parent().parent().find('.collapse').append('<div class="card card-body"><a href="/Profiles/ById/' + id + '" class="card-title h5">' +
                firstName + ' ' + lastName + '</a><p class="card-text">' + text + '</p></div>')
        },
    });
});