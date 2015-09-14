var zoneInfos = {};

$(function () {

    // Dialog and corresponding button registration
    (function () {
        var registerDialog = function (dialogId, dialogButtonId) {
            $(dialogId).dialog({
                modal: true,
                autoOpen: false
            });

            // Link to open the dialog
            $(dialogButtonId).click(function (event) {
                $(dialogId).dialog('open');
                event.preventDefault();
            });
        };
        registerDialog("#budgetDialog", "#budgetDialogButton");
        registerDialog("#cityEvaluationDialog", "#evaluationDialogButton");
    })();

    var simulation = $.connection.simulationHub;
    var buttonDefinitionStates = {};
    var currentButton = null;
    var imageCache = {};
    var canvas = document.getElementById('gameCanvas');

    var persistZoneInfo = function (zoneInfo) {
        zoneInfos[zoneInfo.key] = zoneInfo;
        zoneInfos[zoneInfo.key].drawn = false;
    };

    var drawZoneInfoForBitmapLayer = function (zoneInfo, selectBitmapLayer) {
        var context = canvas.getContext('2d');
        if (imageCache.hasOwnProperty(selectBitmapLayer(zoneInfo)) && imageCache[selectBitmapLayer(zoneInfo)] !== null) {
            context.drawImage(imageCache[selectBitmapLayer(zoneInfo)], zoneInfo.point.x * 25, zoneInfo.point.y * 25);
        } else {
            var tileImage = new Image();
            tileImage.src = '/tile/' + selectBitmapLayer(zoneInfo);
            tileImage.onload = function () {
                context.drawImage(tileImage, zoneInfo.point.x * 25, zoneInfo.point.y * 25);
                imageCache[selectBitmapLayer(zoneInfo)] = tileImage;
            };
        }
    };

    var drawZoneInfo = function (zoneInfo) {

        if (canvas.width < zoneInfo.point.x * 25)
            canvas.width = zoneInfo.point.x * 25;
        if (canvas.height < zoneInfo.point.y * 25)
            canvas.height = zoneInfo.point.y * 25;

        if (zoneInfo.bitmapLayerOne !== 0) {
            drawZoneInfoForBitmapLayer(zoneInfo, function (x) { return x.bitmapLayerOne; });
            if (zoneInfo.bitmapLayerTwo !== 0) {
                drawZoneInfoForBitmapLayer(zoneInfo, function (x) { return x.bitmapLayerTwo; });
            }
        } else {
            var context = canvas.getContext('2d');
            context.beginPath();
            context.fillStyle = zoneInfo.color;
            context.rect(zoneInfo.point.x * 25, zoneInfo.point.y * 25, 25, 25);
            context.fill();
        }

        zoneInfo.drawn = true;
    };

    simulation.client.submitAreaMessage = function (message) {
        document.getElementById('areaMessageLabel').innerHTML = message;
    }

    simulation.client.submitAreaHotMessage = function (message) {
        alert(message.title + '\n' + message.message);
    }
    var EventPublisherService = function () {
        var currentState;
        var listeners = [];
        this.loadNewState = function (incomingState) {
            currentState = incomingState;
            for (var i in listeners) {
                if (listeners.hasOwnProperty(i)) {
                    listeners[i](incomingState);
                }
            }
        }
        this.addOnNewStateListener = function (listener) {
            listeners.push(listener);
        }
    };

    // City evaluation
    (function () {
        var cityEvaluationStateService = new EventPublisherService();
        cityEvaluationStateService.addOnNewStateListener(function (cityEvaluationState) {

            var handleLabelAndValueTable = function (tableId, labelAndValueSet, title) {
                $(tableId).empty();

                $(tableId).append('<tr><th colspan="2">' + title + '</th></tr>');
                for (var i in labelAndValueSet) {
                    if (labelAndValueSet.hasOwnProperty(i)) {
                        var x = labelAndValueSet[i];
                        $(tableId).append('<tr><td>' + x.label + '</td><td>' + x.value + '</td></tr>');
                    }
                }
            };
            handleLabelAndValueTable('#cityEvaluationOverallTable', cityEvaluationState.overallLabelsAndValues, 'Overall');
            handleLabelAndValueTable('#cityEvaluationBudgetTable', cityEvaluationState.cityBudgetLabelsAndValues, 'City budget');
            handleLabelAndValueTable("#cityEvaluationIssuesTable", cityEvaluationState.issueLabelAndValues, 'Issues');
            handleLabelAndValueTable("#cityEvaluationGeneralOpinionTable", cityEvaluationState.generalOpinion, 'General opinion');

        });


        simulation.client.onYearAndMonthChanged = function (yearAndMonthChangedState) {
            document.getElementById('currentYearAndMonthLabel').innerHTML = yearAndMonthChangedState.yearAndMonthDescription;
            cityEvaluationStateService.loadNewState(yearAndMonthChangedState);
        }
    })();

    // City budget
    (function () {
        var cityBudgetStateService = new EventPublisherService();
        cityBudgetStateService.addOnNewStateListener(function (cityBudgetState) {
            $('#budgetTaxTable').empty();
            var taxStates = cityBudgetState.taxStates;

            $('#budgetTaxTable').append('<tr><th colspan="2">Taxes</th></tr>');
            for (var i in taxStates) {
                if (taxStates.hasOwnProperty(i)) {
                    var taxState = taxStates[i];
                    $('#budgetTaxTable').append('<tr><td>' + taxState.name + '</td><td>' + taxState.projectedIncome + '</td></tr>');
                }
            }
            var cityServiceStates = cityBudgetState.cityServiceStates;

            $('#budgetTaxTable').append('<tr><th colspan="2">City services</th></tr>');
            for (var i in cityServiceStates) {
                if (cityServiceStates.hasOwnProperty(i)) {
                    var cityServiceState = cityServiceStates[i];
                    $('#budgetTaxTable').append('<tr><td>' + cityServiceState.name + '</td><td>' + cityServiceState.projectedExpenses + '</td></tr>');
                }
            }
        });

        simulation.client.submitCityBudgetValue = function (e) {
            document.getElementById('currentFundsLabel').innerHTML = e.currentAmount;
            document.getElementById('projectedIncomeLabel').innerHTML = e.projectedIncome;
            cityBudgetStateService.loadNewState(e.cityBudgetState);
        }

    })();

    simulation.client.submitAndDraw = function (zoneInfo) {
        persistZoneInfo(zoneInfo);
        drawZoneInfo(zoneInfo);
    };

    simulation.client.submitZoneInfos = function (zoneInfos) {
        for (var i in zoneInfos) {
            if (zoneInfos.hasOwnProperty(i)) {
                var zoneInfo = zoneInfos[i];
                persistZoneInfo(zoneInfo);
                drawZoneInfo(zoneInfo);
            }
        }
    };

    simulation.client.submitMenuStructure = function (incomingButtonDefinitions) {
        console.log('Invocation of submitMenuStructure');
        (function () {
            for (var p in incomingButtonDefinitions) {
                if (incomingButtonDefinitions.hasOwnProperty(p)) {
                    var incomingButtonDefinition = incomingButtonDefinitions[p];
                    if (typeof (buttonDefinitionStates[incomingButtonDefinition.name]) === typeof (undefined)) {
                        buttonDefinitionStates[incomingButtonDefinition.name] = {
                            buttonDefinition: incomingButtonDefinition,
                            drawn: false
                        };
                    }
                }
            }
        })();

        for (var i in buttonDefinitionStates) {
            if (buttonDefinitionStates.hasOwnProperty(i)) {
                var buttonDefinitionState = buttonDefinitionStates[i];
                var buttonBar = document.getElementById('buttonBar');
                if (!buttonDefinitionState.drawn) {
                    var newButtonElement = document.createElement('button');
                    newButtonElement.innerHTML = buttonDefinitionState.buttonDefinition.name;
                    buttonBar.appendChild(newButtonElement);

                    var registerButton = function (inputButtonDefinition) {
                        var x = inputButtonDefinition;
                        return function () {
                            currentButton = x.buttonDefinition;
                        };
                    }
                    newButtonElement.addEventListener('click', registerButton(buttonDefinitionState));

                    buttonDefinitionState.drawn = true;
                }
            }
        }
        $("input[type=submit], a, button").button();
    };

    $.connection.hub.start().done(function () {

        console.log('Hub started succesfully. Initiating post-hub startup phase...');

        // Mouse events and handlers
        (function () {

            function getMousePos(targetCanvas, evt) {
                var rect = targetCanvas.getBoundingClientRect();
                return {
                    x: evt.clientX - rect.left,
                    y: evt.clientY - rect.top
                };
            }

            var currentFocusedCell = { x: 0, y: 0 };
            var lastConsumedCell = {};

            var isNetworkZoning = false;

            var consumeZone = function (button, cell) {
                if (lastConsumedCell.x !== cell.x || lastConsumedCell.y !== cell.y) {
                    simulation.server.consumeZone(button.name, cell.x, cell.y);
                    lastConsumedCell = cell;
                }
            }

            canvas.addEventListener('mousemove', function (evt) {
                var mousePos = getMousePos(canvas, evt);
                currentFocusedCell = { x: Math.floor(mousePos.x / 25), y: Math.floor(mousePos.y / 25) };
                if (isNetworkZoning) {
                    consumeZone(currentButton, currentFocusedCell);
                }
            }, false);

            canvas.addEventListener('mousedown', function () {
                if (currentButton !== null && currentButton.isClickAndDrag) {
                    isNetworkZoning = true;
                }
            });

            canvas.addEventListener('mouseup', function () {
                isNetworkZoning = false;
            });

            canvas.addEventListener('click', function () {
                if (currentButton !== null) {
                    consumeZone(currentButton, currentFocusedCell);
                }
            });
        })();

        simulation.server.requestMenuStructure();
        console.log('Post-hub startup phase completed.');
    });
});