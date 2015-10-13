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
                $(dialogId).dialog("open");
                event.preventDefault();
            });
        };
        registerDialog("#budgetDialog", "#budgetDialogButton");
        registerDialog("#cityEvaluationDialog", "#evaluationDialogButton");
    })();

    var simulation = $.connection.simulationHub;
    var buttonDefinitionStates = {};
    var currentButton = null, clearButton = null;
    var imageCache = {};
    var canvasLayer1 = document.getElementById("gameCanvasLayer1");
    var canvasLayer2 = document.getElementById("gameCanvasLayer2");
    var canvasLayer3 = document.getElementById("gameCanvasLayer3");
    var canvasLayer4 = document.getElementById("gameCanvasLayer4");
    var canvasLayer5 = document.getElementById("gameCanvasLayer5");
    var canvasLayer6 = document.getElementById("gameCanvasLayer6");
    var canvasLayers = [canvasLayer1, canvasLayer2, canvasLayer3, canvasLayer4, canvasLayer5, canvasLayer6];

    canvasLayer6.onselectstart = function () { return false; }

    var drawZoneInfoForBitmapLayer = function (zoneInfo, selectBitmapHashCode, selectCanvas, selectPoint) {
        var context = selectCanvas().getContext("2d");
        if (imageCache.hasOwnProperty(selectBitmapHashCode(zoneInfo)) && imageCache[selectBitmapHashCode(zoneInfo)] !== null) {
            var image = imageCache[selectBitmapHashCode(zoneInfo)];

            if (image.isLoaded) {
                context.drawImage(image, selectPoint(zoneInfo).x * 25, selectPoint(zoneInfo).y * 25);
            } else {
                image.drawPendingZones.push(function () {
                    context.drawImage(image, selectPoint(zoneInfo).x * 25, selectPoint(zoneInfo).y * 25);
                });
            }
        }
        else {
            var tileImage = new Image();
            tileImage.drawPendingZones = [];
            imageCache[selectBitmapHashCode(zoneInfo)] = tileImage;
            tileImage.src = "/tile/" + selectBitmapHashCode(zoneInfo);
            tileImage.onload = function () {
                tileImage.isLoaded = true;
                context.drawImage(tileImage, selectPoint(zoneInfo).x * 25, selectPoint(zoneInfo).y * 25);
                if (tileImage.drawPendingZones !== null) {
                    for (var i in tileImage.drawPendingZones) {
                        if (tileImage.drawPendingZones.hasOwnProperty(i)) {
                            tileImage.drawPendingZones[i]();
                        }
                    }
                }
            };
        }
        return {
            clearRect: function (targetContext) {
                targetContext.clearRect(selectPoint(zoneInfo).x * 25, selectPoint(zoneInfo).y * 25, 25, 25);
            }
        };
    };

    var currentDataMeter = "";

    var drawZoneInfo = function (zoneInfo) {

        for (var i = 0; i < canvasLayers.length; i++) {
            var canvasLayer = canvasLayers[i];
            if (canvasLayer.width < zoneInfo.point.x * 25)
                canvasLayer.width = zoneInfo.point.x * 25;
            if (canvasLayer.height < zoneInfo.point.y * 25)
                canvasLayer.height = zoneInfo.point.y * 25;
        }

        (function () {
            if (currentDataMeter === "") {
                return;
            }

            var matches = $.grep(zoneInfo.dataMeterResults, function (e) { return e.name === currentDataMeter; });

            if (matches.length === 1) {
                var dataMeterResult = matches[0];
                console.log(dataMeterResult.level + " - " + dataMeterResult.name);

                if (dataMeterResult.colour !== "") {
                    var context = canvasLayer5.getContext("2d");
                    context.beginPath();
                    context.fillStyle = dataMeterResult.colour;
                    context.rect(zoneInfo.point.x * 25, zoneInfo.point.y * 25, 25, 25);
                    context.fill();
                } else {
                    canvasLayer5.getContext("2d").clearRect(zoneInfo.point.x * 25, zoneInfo.point.y * 25, 25, 25);
                }
            } else {
                throw {
                    name: "Missing datameter.",
                    message: "Data Meter '" + currentDataMeter + "' is not contained within the specified ZoneInfo."
                }
            }

        }());

        if (zoneInfo.bitmapLayerOne !== 0) {
            drawZoneInfoForBitmapLayer(zoneInfo,
                function (x) { return x.bitmapLayerOne; },
                function () { return canvasLayer1; },
                function (x) { return x.point; });

            if (zoneInfo.bitmapLayerTwo !== 0) {
                drawZoneInfoForBitmapLayer(zoneInfo,
                    function (x) { return x.bitmapLayerTwo; },
                    function () { return canvasLayer3; },
                    function (x) { return x.point; });
            } else {
                canvasLayer3.getContext("2d").clearRect(zoneInfo.point.x * 25, zoneInfo.point.y * 25, 25, 25);
            }
        } else {
            var context = canvasLayer1.getContext("2d");
            context.beginPath();
            context.fillStyle = zoneInfo.color;
            context.rect(zoneInfo.point.x * 25, zoneInfo.point.y * 25, 25, 25);
            context.fill();
        }

        zoneInfo.drawn = true;
    };

    simulation.client.submitAreaMessage = function (message) {
        document.getElementById("areaMessageLabel").innerHTML = message;
    }

    simulation.client.submitAreaHotMessage = function (message) {
        alert(message.title + "\n" + message.message);
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

                $(tableId).append("<tr><th colspan=\"2\">" + title + "</th></tr>");
                for (var i in labelAndValueSet) {
                    if (labelAndValueSet.hasOwnProperty(i)) {
                        var x = labelAndValueSet[i];
                        $(tableId).append("<tr><td>" + x.label + "</td><td>" + x.value + "</td></tr>");
                    }
                }
            };
            handleLabelAndValueTable("#cityEvaluationOverallTable", cityEvaluationState.overallLabelsAndValues, "Overall");
            handleLabelAndValueTable("#cityEvaluationBudgetTable", cityEvaluationState.cityBudgetLabelsAndValues, "City budget");
            handleLabelAndValueTable("#cityEvaluationIssuesTable", cityEvaluationState.issueLabelAndValues, "Issues");
            handleLabelAndValueTable("#cityEvaluationGeneralOpinionTable", cityEvaluationState.generalOpinion, "General opinion");

        });


        simulation.client.onYearAndMonthChanged = function (yearAndMonthChangedState) {
            document.getElementById("currentYearAndMonthLabel").innerHTML = yearAndMonthChangedState.yearAndMonthDescription;
            cityEvaluationStateService.loadNewState(yearAndMonthChangedState);
        }
    })();

    // City budget
    (function () {
        var cityBudgetStateService = new EventPublisherService();
        cityBudgetStateService.addOnNewStateListener(function (cityBudgetState) {
            $("#budgetTaxTable").empty();
            var taxStates = cityBudgetState.taxStates;

            $("#budgetTaxTable").append("<tr><th colspan=\"2\">Taxes</th></tr>");
            for (var i in taxStates) {
                if (taxStates.hasOwnProperty(i)) {
                    var taxState = taxStates[i];
                    $("#budgetTaxTable").append("<tr><td>" + taxState.name + "</td><td>" + taxState.projectedIncome + "</td></tr>");
                }
            }
            var cityServiceStates = cityBudgetState.cityServiceStates;

            $("#budgetTaxTable").append("<tr><th colspan=\"2\">City services</th></tr>");
            for (var i in cityServiceStates) {
                if (cityServiceStates.hasOwnProperty(i)) {
                    var cityServiceState = cityServiceStates[i];
                    $("#budgetTaxTable").append("<tr><td>" + cityServiceState.name + "</td><td>" + cityServiceState.projectedExpenses + "</td></tr>");
                }
            }
        });

        simulation.client.submitCityBudgetValue = function (e) {
            document.getElementById("currentFundsLabel").innerHTML = e.currentAmount;
            document.getElementById("projectedIncomeLabel").innerHTML = e.projectedIncome;
            cityBudgetStateService.loadNewState(e.cityBudgetState);
        }

    })();

    simulation.client.submitAndDraw = function (zoneInfo) {
        drawZoneInfo(zoneInfo);
    };

    simulation.client.submitZoneInfos = function (zoneInfos) {
        for (var i in zoneInfos) {
            if (zoneInfos.hasOwnProperty(i)) {
                var zoneInfo = zoneInfos[i];
                drawZoneInfo(zoneInfo);
            }
        }
    };

    simulation.client.submitMenuStructure = function (request) {


        var createButton = function (text, target, clickHandler) {
            var newButtonElement = document.createElement("button");
            newButtonElement.innerHTML = "OL: " + text;
            target.appendChild(newButtonElement);
            newButtonElement.addEventListener("click", clickHandler);
            return newButtonElement;
        };
        var miscButtonBar = document.getElementById("miscButtonBar");

        (function () {
            var registerDataMeter = function (dataMeter) {
                createButton(dataMeter, miscButtonBar, function (e) {
                    currentDataMeter = dataMeter;
                    canvasLayer5.getContext("2d").clearRect(0, 0, canvasLayer5.width, canvasLayer5.height);
                });
            };

            var dataMeterInstances = request.dataMeterInstances;

            registerDataMeter("");

            for (var d in dataMeterInstances) {
                if (dataMeterInstances.hasOwnProperty(d)) {
                    var dataMeter = dataMeterInstances[d];
                    registerDataMeter(dataMeter);
                };
            };
        })();


        var incomingButtonDefinitions = request.buttonDefinitions;
        console.log("Invocation of submitMenuStructure");
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
                var buttonBar = document.getElementById("buttonBar");
                if (!buttonDefinitionState.drawn) {
                    var newButtonElement = document.createElement("button");
                    newButtonElement.innerHTML = buttonDefinitionState.buttonDefinition.name;
                    buttonBar.appendChild(newButtonElement);

                    var registerButton = function (inputButtonDefinition) {
                        var x = inputButtonDefinition;
                        return function () {
                            currentButton = x.buttonDefinition;
                        };
                    }
                    newButtonElement.addEventListener("click", registerButton(buttonDefinitionState));
                    if (buttonDefinitionState.buttonDefinition.isClearButton) {
                        if (clearButton === null)
                            clearButton = buttonDefinitionState.buttonDefinition;
                        else {
                            throw {
                                name: "Multiple 'clear'-buttons detected",
                                message: "Multiple 'clear'-buttons were registered. Only the presence of a single 'clear'-button is supported."
                            };
                        }
                    }

                    buttonDefinitionState.drawn = true;
                }
            }
        }
        $("input[type=submit], a, button").button();
    };

    (function () {
        var previouslyDrawnVehicleTiles = [];
        simulation.client.submitVehicleStates = function (vehicleStates) {

            var contextOne = canvasLayer2.getContext("2d");
            var contextTwo = canvasLayer4.getContext("2d");
            var cancelObj;

            while (previouslyDrawnVehicleTiles.length) {
                cancelObj = previouslyDrawnVehicleTiles.pop();
                cancelObj.clearRect(contextOne);
                cancelObj.clearRect(contextTwo);
            }

            canvasLayer2.getContext("2d").clearRect(0, 0, canvasLayer2.width, canvasLayer2.height);
            canvasLayer4.getContext("2d").clearRect(0, 0, canvasLayer4.width, canvasLayer4.height);
            for (var i in vehicleStates) {
                if (vehicleStates.hasOwnProperty(i)) {
                    var vehicleState = vehicleStates[i];
                    cancelObj = drawZoneInfoForBitmapLayer(vehicleState,
                        function (x) { return x.bitmapId; },
                        function () { return vehicleState.isShip ? canvasLayer2 : canvasLayer4; },
                        function (x) { return x.pointOne; });

                    previouslyDrawnVehicleTiles.push(cancelObj);
                }
            }
        }
    })();

    $.connection.hub.start().done(function () {

        console.log("Hub started succesfully. Initiating post-hub startup phase...");

        // Mouse events and handlers
        (function () {

            function getMousePos(targetCanvas, evt) {
                var rect = targetCanvas.getBoundingClientRect();
                return {
                    x: evt.clientX - rect.left,
                    y: evt.clientY - rect.top
                };
            }
            (function () {
                var currentFocusedCell = { x: 0, y: 0 };
                var lastConsumedCell = null;

                var clickAndDragState = null;

                (function () {
                    var ClickDragState = function () {
                        var isNetworkZoning = false;
                        var isNetworkDemolishing = false;

                        this.reset = function () {
                            isNetworkZoning = isNetworkDemolishing = false;
                        }

                        this.activateNetworkZoning = function () {
                            this.reset();
                            isNetworkZoning = true;
                        }

                        this.activateNetworkDemolishing = function () {
                            this.reset();
                            isNetworkDemolishing = true;
                        }

                        this.getIsNetworkZoning = function () {
                            return isNetworkZoning;
                        }

                        this.getIsNetworkDemolishing = function () {
                            return isNetworkDemolishing;
                        }

                        this.getStateAsString = function () {
                            if (isNetworkZoning && !isNetworkDemolishing) {
                                return "isNetworkZoning";
                            } else if (isNetworkDemolishing && !isNetworkZoning) {
                                return "isNetworkDemolishing";
                            } else if (!isNetworkDemolishing && !isNetworkZoning) {
                                return "none";
                            }
                            throw {
                                name: "Current state of ClickDragState is invalid",
                                message: "ClickDragState cannot be both in 'networkZoning' and 'networkDemolishing' state."
                            };
                        }
                    }

                    clickAndDragState = new ClickDragState();
                }());

                var consumeZone = function (button, cell) {
                    if (lastConsumedCell === null
                        || lastConsumedCell.x !== cell.x
                        || lastConsumedCell.y !== cell.y
                        || lastConsumedCell.button !== button) {
                        simulation.server.consumeZone(button.name, cell.x, cell.y);
                        lastConsumedCell = cell;
                        lastConsumedCell.button = button;

                        var context = canvasLayer6.getContext("2d");
                        context.globalAlpha = 0.5;
                        context.beginPath();
                        context.fillStyle = "red";
                        context.rect(cell.x * 25, cell.y * 25, 25, 25);
                        context.fill();
                        setTimeout(function () {
                            var half = 25 / 2;
                            var quarter = half / 2;
                            context.clearRect(cell.x * 25, cell.y * 25, 25, 25);
                            context.beginPath();
                            context.fillStyle = "orange";
                            context.rect((cell.x * 25) + quarter, (cell.y * 25) + quarter, 25 - half, 25 - half);
                            context.fill();
                        }, 100);
                        setTimeout(function () {
                            context.clearRect(cell.x * 25, cell.y * 25, 25, 25);
                        }, 300);
                    }
                }

                var previousHighlight = null;

                canvasLayer6.addEventListener("mousemove", function (evt) {
                    var mousePos = getMousePos(canvasLayer1, evt);
                    var cell = { x: Math.floor(mousePos.x / 25), y: Math.floor(mousePos.y / 25) };
                    currentFocusedCell = cell;

                    if (previousHighlight !== null && (
                            previousHighlight.cell.x !== cell.x
                            || previousHighlight.cell.y !== cell.y
                            || previousHighlight.clickDragState !== clickAndDragState.getStateAsString())) {
                        previousHighlight.clear();
                        if (clickAndDragState.getIsNetworkZoning()) {
                            consumeZone(currentButton, currentFocusedCell);
                        } else if (clickAndDragState.getIsNetworkDemolishing()) {
                            consumeZone(clearButton, currentFocusedCell);
                        }
                    }

                    previousHighlight = {
                        cell: cell,
                        clear: function () {
                            var context = canvasLayer6.getContext("2d");
                            context.clearRect((cell.x * 25) - 10, (cell.y * 25) - 10, 25 + 20, 25 + 20);
                        },
                        draw: function () {
                            var context = canvasLayer6.getContext("2d");
                            context.globalAlpha = 0.5;
                            context.beginPath();
                            context.strokeStyle = "red";
                            context.lineWidth = 1;
                            context.rect(cell.x * 25, cell.y * 25, 25, 25);
                            context.stroke();
                        },
                        clickDragState: clickAndDragState.getStateAsString()
                    };

                    previousHighlight.draw();

                }, false);

                canvasLayer6.addEventListener("mousedown", function (ev) {
                    if (currentButton !== null) {
                        if (ev.which === 3) {
                            clickAndDragState.activateNetworkDemolishing();
                            consumeZone(clearButton, currentFocusedCell);
                        } else if (currentButton.isClickAndDrag) {
                            clickAndDragState.activateNetworkZoning();
                            consumeZone(currentButton, currentFocusedCell);
                        }
                    }
                });

                canvasLayer6.addEventListener("mouseup", function () {
                    clickAndDragState.reset();
                });

                canvasLayer6.addEventListener("mouseleave", function () {
                    clickAndDragState.reset();
                });

                canvasLayer6.addEventListener("click", function (ev) {
                    if (currentButton !== null) {
                        consumeZone(ev.which !== 3 ? currentButton : clearButton, currentFocusedCell);
                    }
                });

                canvasLayer6.addEventListener('contextmenu', function (ev) {
                    ev.preventDefault();
                    return false;
                }, false);
            })();
        })();

        simulation.server.requestMenuStructure();
        console.log("Post-hub startup phase completed.");
    });
});