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
            connection.on("ReceiveTrain", (train, trainLifetime) => {
                //remove loader
                $("#loading-message").remove();

                const $rowHTML = this.trainRowContent(train);

                // Append to the train container
                $rowHTML.hide().prependTo('#train-rows-content').fadeIn(400);

                this.showProgressBarBackground($rowHTML, trainLifetime);
            });

            // Remove a train row when instructed by the server
            connection.on("RemoveTrain", function (trainID) {
                $("#train-row-" + trainID).fadeOut(400, function () {
                    $(this).remove();
                });
            });

            // Handle incident added event to change view history buttons across clients
            connection.on("IncidentAdded", (trainID) => {
                //show incidents history button if hidden
                this.changeHistoryButton($('#train-row-' + trainID).find(".btn-no-incidents"), trainID, false, true);
                this.changeHistoryButton($('#train-row-' + trainID).find(".btn-view-history"), trainID);
            });

            //call StartStreaming method from TrainHub
            connection.start().then(() => {
                console.log("Connected to TrainHub");

                connection.send("StartStreaming").catch(err => console.error(err.toString()));
            });

            connection.onclose(async () => {
                console.log("Disconnected, retrying in 5s...");
                setTimeout(() => connection.start(), 5000);
            });
        },

        // add train row HTML content area
        trainRowContent: function (train) {
            // Clone row template
            const $rowHTML = $("#train-row").clone().attr("id", "train-row-" + train.trainId).removeClass("d-none");

            //add main columns
            $rowHTML.children(".train-number").text(train.trainNumber);
            $rowHTML.children(".train-name").text(train.trainName);
            $rowHTML.children(".train-nexstation").text(train.nextStation);
            $rowHTML.children(".train-delaytime").text(train.delayTime);
            $rowHTML.children(".train-last-updated-time").text(train.lastUpdatedTime);

            //add Incident button if the delay is present
            this.addDelayArea($rowHTML, train);

            return $rowHTML;
        },

        //Add Train incident via Ajax 
        addTrainIncident: function () {
            $('#save-incident-form').on('click', (event) => {
                const $thisButton = $(event.currentTarget);
                const $currentModal = $thisButton.closest('.add-incident-modal');
                const $form = $currentModal.find('.add-incident-form');

                // Trigger unobtrusive validation
                if (!$form.valid()) {
                    $.validator.unobtrusive.parse($form);
                    return;
                }

                //disable save button to avoid repeated ajax calls
                $thisButton.prop('disabled', true);

                this.addTrainIncidentAjax($form, $thisButton, $currentModal);
            });
        },

        //Ajax method
        addTrainIncidentAjax: function ($form, $thisButton, $currentModal) {
            const $modalFooterArea = $currentModal.find('.modal-footer');
            const $modalHeading = $form.siblings('.train-number-heading');
            const $btnSpinner = $thisButton.find('.spinner-border');
            const $serverErrorsArea = $form.children('.server-errors');


            $.ajax({
                url: '/trains/addIncident',
                type: 'POST',
                data: $form.serialize(),
                beforeSend: function () {
                    $serverErrorsArea.empty();
                    $btnSpinner.removeClass('d-none');
                },
                success: (response) => {

                    if (response.success) {
                        //set success area message
                        const $success = $form.siblings('.incident-success');
                        $success.text(response.message).removeClass('d-none');

                        //hide form area
                        $form.add($modalFooterArea).add($modalHeading).hide();

                        //get train id
                        const trainID = $form.children('.train-id-input').val();

                        setTimeout(() => {
                            //hide success message
                            $success.addClass('d-none');

                            // Reset all fields in the form
                            $form[0].reset();

                            $currentModal.modal('hide');

                            //show incidents history button if hidden
                            this.changeHistoryButton($('#train-row-' + trainID).find(".btn-no-incidents"), trainID, false, true);
                            this.changeHistoryButton($('#train-row-' + trainID).find(".btn-view-history"), trainID);
                        }, 1500);

                    } else if (response.errors) {
                        $serverErrorsArea.append(response.errors);
                    }
                },
                error: function (xhr) {
                    // Handle errors
                    console.log('Error saving incident: ' + xhr.responseText);
                },
                complete: function () {
                    $btnSpinner.addClass('d-none');

                    setTimeout(() => {
                        $thisButton.prop('disabled', false);
                        $form.add($modalFooterArea).add($modalHeading).show();
                    }, 2000);
                }
            });
        },

        //add delay area HTML
        addDelayArea: function ($rowHTML, train) {
            if (train.hasDelay) {
                $rowHTML.addClass("text-white bg-danger");

                //add button
                $rowHTML.children(".train-add-incident").html(`
                        <button type="button" class="btn btn-sm btn-dark" 
                            data-train-id="${train.trainId}"
                            data-train-number="${train.trainNumber}"
                            data-bs-toggle="modal" 
                            data-bs-target="#add-incident-modal">
                            Add
                        </button>
                    `);

                //add train info to opened modal
                this.populateModalWithTrainInfo();

                //show Incident History button if the train has incidents
                const $incidentHistoryArea = $rowHTML.children(".train-incident-history");
                if (train.hasIncident) {
                    this.changeHistoryButton($incidentHistoryArea.children('.btn-view-history'), train.trainId);
                }
                else {
                    this.changeHistoryButton($incidentHistoryArea.children('.btn-no-incidents'), train.trainId, false);
                }
            }
        },

        //Add specific Train info to the modal when opened
        populateModalWithTrainInfo: function () {
            $('#train-table').on('click', '.train-add-incident button', function () {
                const $modal = $('#add-incident-modal');
                const trainId = $(this).data('train-id');
                const trainNumber = $(this).data('train-number');

                // add train id to the hidden input form
                $modal.find('.train-id-input').val(trainId);

                //add train number to the heading
                $modal.find('.train-number-heading span').text(trainNumber);
            });
        },

        // Clear errors and fields when modal is closed
        clearModalFields: function () {
            $('#add-incident-modal').on('hidden.bs.modal', function () {
                const $form = $(this).find('.add-incident-form');

                //clear form server errors
                $form.children('.server-errors').empty();

                // Clear server-side errors
                $form.find('.server-errors').empty();

                // Clear unobtrusive validation errors from spans
                $form.find('.field-validation-error').empty();
                $form.find('.input-validation-error').removeClass('input-validation-error');

                // Reset all fields in the form
                $form[0].reset();
                $form.find('.train-id-input').val('');
            });
        },

        //add extra attributes to the history button
        changeHistoryButton: function ($button, trainId, viewHistoryButton = true, hide = false) {
            //add button link
            if (viewHistoryButton) {
                $button.attr('href', 'trains/' + trainId + '/incidents');
            }

            if (hide) {
                $button.addClass("d-none");
            }
            else {
                $button.removeClass("d-none");
            }
        },

        //train progress bar
        showProgressBarBackground: function ($rowHTML, trainLifetime) {
            const $progressBar = $rowHTML.children(".progress-fill");

            let startTime = Date.now();

            const interval = setInterval(() => {
                const elapsedMs = Date.now() - startTime; // milliseconds
                const percent = Math.min((elapsedMs / trainLifetime) * 100, 100);

                // update the CSS width with the percentage
                $progressBar.css("width", percent + "%");

                if (percent >= 100) clearInterval(interval);
            }, 50);
        },

    };

    //initialize the TrainApp functions
    TrainApp.init();
});
