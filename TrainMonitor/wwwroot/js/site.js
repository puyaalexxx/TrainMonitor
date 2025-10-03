//add incident
$(document).ready(function () {
    $('#train-table .save-incident').on('click', function () {
        const $thisButton = $(this);
        const currentModalID = $thisButton.closest('.add-incident-modal').data('train-id');
        const $form = $('#add-incident-form-' + currentModalID);


        // Trigger unobtrusive validation
        if (!$form.valid()) {
            return; // form is invalid, do not continue
        }

        const formData = $form.serialize();

        console.log(formData);

        $.ajax({
            url: '', // controller/action - /Train/AddIncident
            type: 'POST',
            data: formData,
            beforeSend: function () {
                $thisButton.find('.spinner-border').removeClass('d-none');
            },
            success: function (response) {
                // Close modal
                //$('#add-incident').modal('hide');

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
