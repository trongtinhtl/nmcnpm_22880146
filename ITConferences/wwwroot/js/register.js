$(document).ready(function () {

    const form = $('#registerForm');

    form.on('change', 'input', function () {
        if (this.value) {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback').hide();
        }
        else {
            $(this).addClass('is-invalid');
            $(this).siblings('.invalid-feedback').show();
        }

        var elements = form.find('input.is-invalid');

        if (elements.length) {
            form.find('[type="submit"]').addClass('disabled');
        }
        else {
            form.find('[type="submit"]').removeClass('disabled');
        }
    })
});