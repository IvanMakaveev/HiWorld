var likeCommentFunction = function (event) {
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
                form.find("button").removeClass('btn-outline-primary').addClass('btn-primary').html('Liked <i class="fas fa-heart"></i>');
                form.find("span").text(parseInt(form.find("span").text()) + 1)
            }
            else {
                form.attr('liked', "false")
                form.find("button").removeClass('btn-primary').addClass('btn-outline-primary').html('Like <i class="fas fa-heart"></i>');
                form.find("span").text(parseInt(form.find("span").text()) - 1)
            }
        },
    });
}


function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}


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
                form.find("button").removeClass('btn-outline-primary').addClass('btn-primary').html('Liked <i class="fas fa-heart"></i>');
                form.find("span").text(parseInt(form.find("span").text()) + 1)
            }
            else {
                form.attr('liked', "false")
                form.find("button").removeClass('btn-primary').addClass('btn-outline-primary').html('Like <i class="fas fa-heart"></i>');
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
            var profileId = data.profileId
            var id = data.id
            var firstName = data.profileFirstName
            var lastName = data.profileLastName
            var text = data.text
            var createdOnString = data.createdOnString
            var likes = data.likes
            form.parent().parent().parent().find('.collapse').append('<div class="card card-body"><a href="/Profiles/ById/' + profileId + '" class="card-title h5 mb-0 text-dark align-self-start"">' +
                escapeHtml(firstName) + ' ' + escapeHtml(lastName) + '</a><p class="card-text">' + escapeHtml(text) + '</p><p class="card-text"><small class="text-muted">Created on: ' +
                createdOnString + '</small></p><form class="likeCommentButton" liked="false" action="/Comments/Like/' + id + '" method="post"><input name="__RequestVerificationToken" type="hidden" value="' +
                antiForgeryToken + '"/><button class="btn btn-outline-primary">Like <i class="fas fa-heart"></i></button><span class="ml-1 card-text">' +
                likes + '</span></form></div>')

            var newForm = form.parent().parent().parent().find('.likeCommentButton')
            newForm.submit(likeCommentFunction)
        },
    });
});

$('.likeCommentButton').submit(likeCommentFunction);