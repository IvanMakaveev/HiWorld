var tagIndex = Number.parseInt(document.currentScript.getAttribute('current-tags'));
function AddTagField() {
    $("#TagsContainer")
        .append("<input type='text' name='Tags[" + tagIndex + "]' class='form-control mt-1'/>");

    tagIndex += 1;
}
function RemoveTagField() {
    if (tagIndex > 0) {
        $("#TagsContainer")
            .children().last().remove();

        tagIndex -= 1;
    }
}