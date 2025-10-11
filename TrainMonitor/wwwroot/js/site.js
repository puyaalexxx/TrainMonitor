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

                //remove loader
                $("#loading-message").remove();

                const $rowHTML = this.trainRowContent(train);

                // Append to the train container
                $rowHTML.hide().prependTo('#train-rows-content').fadeIn(400);
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

        trainRowContent: function (train) {
            // Clone row template
            const $rowHTML = $("#train-row").clone().removeAttr("id").removeClass("d-none");

            //add main columns
            $rowHTML.children(".train-number").text(train.trainNumber);
            $rowHTML.children(".train-name").text(train.trainName);
            $rowHTML.children(".train-nexstation").text(train.nextStation);
            $rowHTML.children(".train-delaytime").text(train.delayTime);
            $rowHTML.children(".train-last-updated-time").text(train.lastUpdatedTime);

            //add Incident button if the delay is present
            if (train.hasDelay) {
                $rowHTML.addClass("text-white bg-danger");

                $rowHTML.children(".train-add-incident").html(`
                        <button type="button" class="btn btn-sm btn-dark" 
                            data-bs-toggle="modal" 
                            data-bs-target="#add-incident-${train.trainId}">
                            Add
                        </button>
                    `);

                //show Incident History button if the train has incidents
                const $incidentHistoryArea = $rowHTML.children(".train-incident-history");
                if (train.hasIncident) {
                    $incidentHistoryArea.children('.btn-view-history').attr('href', 'trains/' + train.trainId + '/incidents').removeClass('d-none');
                }
                else {
                    $incidentHistoryArea.children('.btn-no-incidents').removeClass('d-none');
                }
            }

            return $rowHTML;
        },

        //Add Train incident via Ajax 
        addTrainIncident: function () {
            $('#train-table').on('click', '.save-incident', function () {
                const $thisButton = $(this);
                const $currentModal = $thisButton.closest('.add-incident-modal');
                const $form = $('#add-incident-form-' + $currentModal.data('train-id'));

                // Trigger unobtrusive validation
                if (!$form.valid()) {
                    $.validator.unobtrusive.parse($form);
                    return; // form is invalid, do not continue
                }

                // disable button to avoid double submissions
                $thisButton.prop('disabled', true);

                this.addTrainIncidentAjax($form, $thisButton, $currentModal);
            });
        },

        //Ajax method
        addTrainIncidentAjax: function ($form, $thisButton, $currentModal) {
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
                            $("#btn-no-incidents").addClass("d-none");
                            $("#btn-view-history").removeClass("d-none");
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
