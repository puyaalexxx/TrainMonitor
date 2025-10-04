//add incident
jQuery(function ($) {
    $('#train-table .save-incident').on('click', function () {
        const $thisButton = $(this);
        const $currentModal = $thisButton.closest('.add-incident-modal');
        const currentModalID = $currentModal.data('train-id');
        const $form = $('#add-incident-form-' + $currentModal.data('train-id'));

        // Trigger unobtrusive validation
        if (!$form.valid()) {
            $.validator.unobtrusive.parse($form);
            return; // form is invalid, do not continue
        }

        $.ajax({
            url: '/trains/addIncident', // controller/action - 
            type: 'POST',
            data: $form.serialize(),
            beforeSend: function () {
                $thisButton.find('.spinner-border').removeClass('d-none');
            },
            success: function (response) {

                console.log(response);

                // Close modal
                $('#add-incident').modal('hide');

                // Optionally, show success message or update train row
                console.log('Incident saved successfully!');
            },
            error: function (xhr) {
                // Handle errors
                console.log('Error saving incident: ' + xhr.responseText);
            },
            complete: function () {
                $thisButton.find('.spinner-border').addClass('d-none');
            }
        });
    });
});
