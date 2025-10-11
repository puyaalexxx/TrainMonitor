jQuery(function ($) {
    const TrainApp = {

        init: function () {
            this.setupSignalR();

            this.addTrainIncident();

            this.clearModalFields();
        },

        //Setup SignalR connection for adding Trains to the List
        setupSignalR: function () {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/trainHub")
                .build();

            // Receive a single train from the server
            connection.on("ReceiveTrain", (train, timestamp) => {
                console.log("Train received:", train);
                console.log("Timestamp:", timestamp);




                // Use jQuery to manipulate DOM
                let html = '';

                if (train.hasDelay) {
                    html += `<div class="row py-2 text-decoration-none text-white bg-danger border-top">
                                <div class="col">${train.trainName}</div>

                                <div class="col-1">
                                    <button type="button" class="btn btn-sm btn-dark add-incident" data-bs-toggle="modal" data-bs-target="#add-incident-${train.trainId}">Add</button>
                                 </div>

                                <div class="col-2">
                                <button class="btn btn-sm btn-info incident-history" data-train-id="${train.trainId}">History</button>
                             </div>`;

                    // Optional: modal HTML (or you can render it separately)
                    html += `<!-- Modal HTML for adding incident -->`;

                    html += `</div>`;
                } else {
                    html += `<div class="row py-2 text-decoration-none text-dark bg-grey-light border-end border-start border-top">`;

                    html += `<div class="col">${train.trainName}</div>`; // simplified

                    html += `<div class="col-1"></div><div class="col-2"></div>`;

                    html += `</div>`;
                }

                // Append to the container
                $('#train-rows-target').prepend(html);
            });

            connection.start().then(() => {
                console.log("Connected to TrainHub");
                connection.invoke("StartStreaming").catch(err => console.error(err.toString()));
            });

            connection.onclose(async () => {
                console.log("Disconnected, retrying in 5s...");
                setTimeout(() => connection.start(), 5000);
            });
        },



        //Add Train incident via ajax 
        addTrainIncident: function () {
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
        },

        // Clear errors and fields when modal is closed
        clearModalFields: function () {
            $('#train-table').on('hidden.bs.modal', '.add-incident-modal', function () {
                const $form = $(this).find('form');
                $form.children('.server-errors').empty();

                // Clear server-side errors
                $form.find('.server-errors').empty();

                // Clear unobtrusive validation errors from spans
                $form.find('.field-validation-error').empty();
                $form.find('.input-validation-error').removeClass('input-validation-error');
            });
        },

    };

    //initialize the TrainApp functions
    TrainApp.init();
});
