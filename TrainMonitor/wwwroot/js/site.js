//add incident
jQuery(function ($) {
    $('#train-table').on('click', '.save-incident', function () {
        const $thisButton = $(this);
        const $currentModal = $thisButton.closest('.add-incident-modal');
        const trainID = $currentModal.data('train-id');
        const $form = $('#add-incident-form-' + trainID);

        // Trigger unobtrusive validation
        if (!$form.valid()) {
            $.validator.unobtrusive.parse($form);
            return; // form is invalid, do not continue
        }

        // disable button to avoid double submissions
        $thisButton.prop('disabled', true);

        $.ajax({
            url: '/trains/addIncident',
            type: 'POST',
            data: $form.serialize(),
            beforeSend: function () {
                $form.children('.server-errors').empty();
                $thisButton.find('.spinner-border').removeClass('d-none');
            },
            success: function (response) {

                if (response.success) {

                    const $success = $form.prev('.incident-success');
                    $success.text(response.message).removeClass('d-none');


                    setTimeout(() => {
                        //hide success message
                        $success.addClass('d-none');

                        // Reset all fields in the form
                        $form[0].reset();

                        $currentModal.modal('hide');

                        //show incidents history button if hidden
                        $("#btn-no-incidents-" + trainID).addClass("d-none");
                        $("#btn-view-history-" + trainID).removeClass("d-none");
                    }, 2000);

                } else if (response.errors) {
                    $form.children('.server-errors').append(response.errors);
                }
            },
            error: function (xhr) {
                // Handle errors
                console.log('Error saving incident: ' + xhr.responseText);
            },
            complete: function () {
                $thisButton.find('.spinner-border').addClass('d-none');
                $thisButton.prop('disabled', false);
            }
        });
    });

    // Clear errors when modal is closed
    $('#train-table').on('hidden.bs.modal', '.add-incident-modal', function () {
        const $form = $(this).find('form');
        $form.children('.server-errors').empty();

        // Clear server-side errors
        $form.find('.server-errors').empty();

        // Clear unobtrusive validation errors from spans
        $form.find('.field-validation-error').empty();
        $form.find('.input-validation-error').removeClass('input-validation-error');
    });
});
