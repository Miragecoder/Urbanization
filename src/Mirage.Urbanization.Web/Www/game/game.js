"use strict";
if (!confirm("You are about to load 'Urbanization' into your browser.\r\n\r\n" +
    "The game is still in development so you may encounter performance issues when " +
    "running on Google Chrome or Opera, or any other WebKit-based browser. Should issues " +
    "arise, try re-opening it in Mozilla Firefox, Microsoft Edge or Internet Explorer. " +
    "A desktop application is also available. \r\n\r\n" + 
    "For more information, see:\r\nhttp://www.github.com/Miragecoder/Urbanization/ \r\n\r\n" + 
    "Would you like to proceed loading the game?")) {
    throw { message:"User aborted web client initialization.", name:"UserAborted"};
}


$(function () {

    var raiseHotMessage = null;

    // Dialog and corresponding button registration
    (function () {

        var setSpanText = function (spanId, spanText) {

            var span = document.getElementById(spanId);

            while (span.firstChild) {
                span.removeChild(span.firstChild);
            }
            span.appendChild(document.createTextNode(spanText));
        };


        var registerDialog = function (dialogId, width, modal) {
            $(dialogId).dialog({
                modal: modal,
                autoOpen: false,
                width: width
            });
        };

        var registerDialogWithButton = function (dialogId, dialogButtonId, width, onClick, modal) {
            registerDialog(dialogId, width, modal);

            // Link to open the dialog
            $(dialogButtonId).click(function (event) {
                $(dialogId).dialog("open");
                event.preventDefault();
                onClick();
            });
        };
        var doNothing = function () { };

        registerDialogWithButton("#buildDialog", "#buildButton", 500, doNothing, false);
        registerDialogWithButton("#budgetDialog", "#budgetDialogButton", 500, doNothing, true);
        registerDialogWithButton("#cityEvaluationDialog", "#evaluationDialogButton", 400, doNothing, true);
        registerDialogWithButton("#overlayDialog", "#overlayDialogButton", 300, doNothing, true);
        registerDialogWithButton("#aboutDialog", "#aboutDialogButton", 300, doNothing, true);
        registerDialogWithButton("#graphDialog", "#graphDialogButton", 650, function () {
            document.getElementById("refreshChartsButton").click();
        }, true);
        registerDialog("#hotMessageDialog", 200, true);

        raiseHotMessage = function (title, message) {
            $("#hotMessageDialog").dialog({ title: title }).dialog("open");
            setSpanText("hotMessageText", message);
        };
    })();

    var simulation = new signalR.HubConnectionBuilder().withUrl("/simulationHub").build();
    var buttonDefinitionStates = {};
    var dummyButton = {
        horizontalCellOffset: 0,
        verticalCellOffset: 0,
        widthInCells: 1,
        heightInCells: 1
    };
    var currentButton = dummyButton, clearButton = null;
    var canvasLayer1 = document.getElementById("gameCanvasLayer1");
    var canvasLayer2 = document.getElementById("gameCanvasLayer2");
    var canvasLayer3 = document.getElementById("gameCanvasLayer3");
    var canvasLayer4 = document.getElementById("gameCanvasLayer4");
    var canvasLayer5 = document.getElementById("gameCanvasLayer5");
    var canvasLayer6 = document.getElementById("gameCanvasLayer6");
    var canvasLayers = [canvasLayer1, canvasLayer2, canvasLayer3, canvasLayer4, canvasLayer5, canvasLayer6];

    var atlasImg = new Image();
    atlasImg.src = "/tileset/";
    atlasImg.onload = function () {

        var atlasCanvas = document.createElement("canvas");
        atlasCanvas.width = atlasImg.width;
        atlasCanvas.height = atlasImg.height;
        atlasCanvas.getContext("2d").drawImage(atlasImg, 0, 0);

        var cityInfo = null;
        $.getJSON("/cityinfo/", function (data) {
            cityInfo = data;

            for (var i = 0; i < canvasLayers.length; i++) {
                var canvasLayer = canvasLayers[i];
                if (canvasLayer.width < cityInfo.mapWidth * 25)
                    canvasLayer.width = cityInfo.mapWidth * 25;
                if (canvasLayer.height < cityInfo.mapHeight * 25)
                    canvasLayer.height = cityInfo.mapHeight * 25;
            }

            canvasLayer6.onselectstart = function () { return false; };

            var drawZoneInfoForBitmapLayer = function (zoneInfo, selectBitmapHashCode, selectCanvas, selectPoint, isDrawingVehicle) {
                var context = selectCanvas().getContext("2d");

                var hashCode = selectBitmapHashCode(zoneInfo);

                var cellWidthOrHeight = isDrawingVehicle ? 50 : 25;
                var tilesPerRow = isDrawingVehicle ? cityInfo.vehicleTilesPerRow : cityInfo.cellsPerAtlasRow;
                var yOffset = isDrawingVehicle ? cityInfo.cellSpriteOffset : 0;

                context.drawImage(atlasCanvas,
                    (hashCode % tilesPerRow) * cellWidthOrHeight,
                    Math.floor(Math.floor(hashCode / tilesPerRow) * cellWidthOrHeight) + yOffset,
                    cellWidthOrHeight,
                    cellWidthOrHeight,
                    selectPoint(zoneInfo).x * 25,
                    selectPoint(zoneInfo).y * 25, cellWidthOrHeight, cellWidthOrHeight);

                return {
                    clearRect: function (targetContext) {
                        targetContext.clearRect(selectPoint(zoneInfo).x * cellWidthOrHeight, selectPoint(zoneInfo).y * cellWidthOrHeight, cellWidthOrHeight, cellWidthOrHeight);
                    }
                };
            };

            var currentDataMeter = { name: "None", webId: 0 };

            var synchronizedAnimator;
            (function () {

                var getZoneInfoId = function (zoneInfo) {
                    return zoneInfo.x + "_" + zoneInfo.y;
                };

                var Animator = function (delay, length) {
                    var currentIndex = 0;
                    var maxIndex = length - 1;
                    var isStarted = false;
                    var handlers = [];

                    var start = function () {
                        (function loopFunction() {
                            for (var h in handlers) {
                                if (handlers.hasOwnProperty(h)) {
                                    var handler = handlers[h];
                                    handler(currentIndex);
                                }
                            }
                            if (currentIndex === maxIndex)
                                currentIndex = 0;
                            else
                                currentIndex++;
                            setTimeout(loopFunction, delay);
                        })();
                    };

                    this.registerHandler = function (handler) {
                        handlers.push(handler);
                        if (!isStarted)
                            start();
                        isStarted = true;
                    };

                };

                var CellAnimator = function (animatedCellBitmapSet, selectCanvasLayer) {
                    var animatedZoneInfos = [];
                    var animators = [];

                    this.start = function () {
                        var animatorId = animatedCellBitmapSet.delay + "_" + animatedCellBitmapSet.bitmapIds.length;
                        if (!animators.hasOwnProperty(animatorId)) {
                            animators[animatorId] = new Animator(animatedCellBitmapSet.delay, animatedCellBitmapSet.bitmapIds.length);
                        }

                        var animator = animators[animatorId];

                        animator.registerHandler(function (currentIndex) {

                            var selectBitmapHashCode = function () { return animatedCellBitmapSet.bitmapIds[currentIndex]; };

                            for (var i in animatedZoneInfos) {
                                if (animatedZoneInfos.hasOwnProperty(i)) {
                                    var animatedZoneInfo = animatedZoneInfos[i];
                                    if (animatedZoneInfo !== null) {
                                        drawZoneInfoForBitmapLayer(
                                            animatedZoneInfo.getZoneInfo(),
                                            selectBitmapHashCode,
                                            selectCanvasLayer,
                                            animatedZoneInfo.getZoneInfo,
                                            false
                                        );
                                    }
                                }
                            }
                        });
                    };

                    this.addAnimatedZoneInfo = function (animatedZoneInfo) {
                        var zoneInfoId = getZoneInfoId(animatedZoneInfo.getZoneInfo());
                        animatedZoneInfos[zoneInfoId] = animatedZoneInfo;
                    };

                    this.removeAnimatedZoneInfo = function (animatedZoneInfo) {
                        var zoneInfoId = getZoneInfoId(animatedZoneInfo.getZoneInfo());
                        animatedZoneInfos[zoneInfoId] = null;
                    };
                };

                var AnimatedZoneInfo = function (zoneInfo) {
                    var currentCellAnimator = null;
                    var that = this;

                    this.getZoneInfo = function () {
                        return zoneInfo;
                    };

                    this.enlistToCellAnimator = function (cellAnimator) {
                        if (currentCellAnimator !== null) {
                            currentCellAnimator.removeAnimatedZoneInfo(that);
                        }
                        if (cellAnimator !== null) {
                            cellAnimator.addAnimatedZoneInfo(that);
                        }
                        currentCellAnimator = cellAnimator;
                    };
                };

                var SynchronizedAnimator = function (bitmapLayerSelector, selectCanvasLayer) {
                    var animatedCellBitmapSets = [];
                    var animatedZoneInfos = [];

                    var processPrivate = function (zoneInfo) {
                        var bitmapLayer = bitmapLayerSelector(zoneInfo);
                        var cellAnimator = null;

                        if (bitmapLayer !== null && bitmapLayer.bitmapIds.length > 1) {
                            if (!animatedCellBitmapSets.hasOwnProperty(bitmapLayer.id)) {
                                cellAnimator = new CellAnimator(bitmapLayer, selectCanvasLayer);
                                animatedCellBitmapSets[bitmapLayer.id] = cellAnimator;
                                cellAnimator.start();
                            }
                            cellAnimator = animatedCellBitmapSets[bitmapLayer.id];
                        }
                        var zoneInfoId = getZoneInfoId(zoneInfo);
                        if (!animatedZoneInfos.hasOwnProperty(zoneInfoId)) {
                            animatedZoneInfos[zoneInfoId] = new AnimatedZoneInfo(zoneInfo);
                        }
                        var animatedZoneInfo = animatedZoneInfos[zoneInfoId];
                        animatedZoneInfo.enlistToCellAnimator(cellAnimator);
                    };

                    this.process = function (zoneInfo) {
                        processPrivate(zoneInfo, function (x) { return x.bitmapLayerOne; });
                        processPrivate(zoneInfo, function (x) { return x.bitmapLayerTwo; });
                    };
                };
                synchronizedAnimator = {
                    layerOne: new SynchronizedAnimator(function (x) { return x.bitmapLayerOne; }, function () { return canvasLayer1; }),
                    layerTwo: new SynchronizedAnimator(function (x) { return x.bitmapLayerTwo; }, function () { return canvasLayer3; })
                };
            })();

            var drawZoneInfo = function (zoneInfo) {

                synchronizedAnimator.layerOne.process(zoneInfo);
                synchronizedAnimator.layerTwo.process(zoneInfo);

                if (zoneInfo.bitmapLayerOne !== null) {
                    drawZoneInfoForBitmapLayer(zoneInfo,
                        function (x) { return x.bitmapLayerOne.bitmapIds[0]; },
                        function () { return canvasLayer1; },
                        function (x) { return x; },
                        false);

                    if (zoneInfo.bitmapLayerTwo !== null) {
                        drawZoneInfoForBitmapLayer(zoneInfo,
                            function (x) { return x.bitmapLayerTwo.bitmapIds[0]; },
                            function () { return canvasLayer3; },
                            function (x) { return x; },
                            false);
                    } else {
                        canvasLayer3.getContext("2d").clearRect(zoneInfo.x * 25, zoneInfo.y * 25, 25, 25);
                    }
                } else {
                    var context = canvasLayer1.getContext("2d");
                    context.beginPath();
                    context.fillStyle = "BurlyWood";
                    context.rect(zoneInfo.x * 25, zoneInfo.y * 25, 25, 25);
                    context.fill();
                }
            };

            simulation.on("submitAreaMessage", function (message) {
                document.getElementById("areaMessageLabel").innerHTML = message;
            });

            simulation.on("submitAreaHotMessage", function (message) {
                raiseHotMessage(message.title, message.message);
            });
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
                };
                this.addOnNewStateListener = function (listener) {
                    listeners.push(listener);
                };
            };

            // City evaluation
            (function () {
                var cityEvaluationStateService = new EventPublisherService();
                cityEvaluationStateService.addOnNewStateListener(function (cityEvaluationState) {

                    var handleLabelAndValueTable = function (tableId, labelAndValueSet, title, isCurrency) {
                        $(tableId).empty();

                        $(tableId).append("<tr><th colspan=\"2\">" + title + "</th></tr>");
                        for (var i in labelAndValueSet) {
                            if (labelAndValueSet.hasOwnProperty(i)) {
                                var x = labelAndValueSet[i];
                                if (isCurrency) {
                                    $(tableId).append("<tr><td>" + x.label + "</td><td class=\"currencycol\">" + accounting.formatMoney(x.value) + "</td></tr>");
                                } else {
                                    $(tableId).append("<tr><td>" + x.label + "</td><td>" + x.value + "</td></tr>");
                                }
                            }
                        }
                    };
                    handleLabelAndValueTable("#cityEvaluationOverallTable", cityEvaluationState.overallLabelsAndValues, "Overall", false);
                    handleLabelAndValueTable("#cityEvaluationBudgetTable", cityEvaluationState.cityBudgetLabelsAndValues, "City budget", true);
                    handleLabelAndValueTable("#cityEvaluationIssuesTable", cityEvaluationState.issueLabelAndValues, "Issues", false);
                    handleLabelAndValueTable("#cityEvaluationGeneralOpinionTable", cityEvaluationState.generalOpinion, "General opinion", false);

                });
                                
                simulation.on("onYearAndMonthChanged", function (yearAndMonthChangedState) {
                    document.getElementById("currentYearAndMonthLabel").innerHTML = yearAndMonthChangedState.yearAndMonthDescription;
                    cityEvaluationStateService.loadNewState(yearAndMonthChangedState);
                });
            })();

            // City budget
            (function () {

                var cityBudgetStateService = new EventPublisherService();
                cityBudgetStateService.addOnNewStateListener(function (cityBudgetState) {
                    $("#budgetTaxTable").empty();
                    var taxStates = cityBudgetState.taxStates;

                    var writeHeader = function (nameHeader, projectedHeader, rateHeader, actions) {
                        var taxRow = document.createElement("tr");

                        var addCell = function (name) {
                            var labelCell = document.createElement("th");
                            labelCell.innerHTML = name;
                            taxRow.appendChild(labelCell);
                        };
                        addCell(nameHeader);
                        addCell(projectedHeader);
                        addCell(rateHeader);
                        addCell(actions);

                        document.getElementById("budgetTaxTable").appendChild(taxRow);
                    };

                    var writeTaxStateRow = function (taxState, isSummary, getName, getProjected, getRate, lowerFunc, raiseFunc) {
                        var taxRow = document.createElement("tr");

                        (function () {
                            var labelCell = document.createElement("td");
                            labelCell.innerHTML = getName(taxState);
                            taxRow.appendChild(labelCell);
                        })();

                        (function () {
                            var labelCell = document.createElement("td");
                            labelCell.class = "currencycol";
                            labelCell.innerHTML = accounting.formatMoney(getProjected(taxState));
                            taxRow.appendChild(labelCell);
                        })();

                        if (isSummary) {
                            var labelCell = document.createElement("td");
                            labelCell.innerHTML = "";
                            taxRow.appendChild(labelCell);
                            labelCell = document.createElement("td");
                            labelCell.innerHTML = "";
                            taxRow.appendChild(labelCell);
                        } else {
                            (function () {
                                var labelCell = document.createElement("td");
                                labelCell.innerHTML = getRate(taxState);
                                taxRow.appendChild(labelCell);
                            })();

                            (function () {
                                var labelCell = document.createElement("td");
                                taxRow.appendChild(labelCell);

                                var disableButtons;

                                var raiseButton = document.createElement("button");
                                raiseButton.innerHTML = "Raise";
                                raiseButton.addEventListener("click", function () {
                                    raiseFunc(simulation, taxState);
                                    disableButtons();
                                });
                                labelCell.appendChild(raiseButton);

                                var dropButton = document.createElement("button");
                                dropButton.innerHTML = "Lower";
                                dropButton.addEventListener("click", function () {
                                    lowerFunc(simulation, taxState);
                                    disableButtons();
                                });

                                disableButtons = function () {
                                    dropButton.disabled = true;
                                    raiseButton.disabled = true;
                                };

                                labelCell.appendChild(dropButton);
                            })();
                        }

                        document.getElementById("budgetTaxTable").appendChild(taxRow);
                    };
                    (function () {
                        writeHeader("Tax", "Projected income", "Current rate", "Actions");

                        for (var i in taxStates) {
                            if (taxStates.hasOwnProperty(i)) {
                                var taxState = taxStates[i];
                                writeTaxStateRow(taxState,
                                    false,
                                    function (e) { return e.name; },
                                    function (e) { return e.projectedIncome; },
                                    function (e) { return e.currentRate; },
                                    function (server, taxState) { server.invoke("lowerTax", taxState.name); },
                                    function (server, taxState) { server.invoke("raiseTax", taxState.name); });
                            }
                        }
                        writeTaxStateRow(cityBudgetState.totalTaxState,
                            true,
                            function (e) { return e.name; },
                            function (e) { return e.projectedIncome; });
                    })();
                    (function () {
                        writeHeader("City services", "Projected expenses", "Current rate", "Actions");
                        for (var i in cityBudgetState.cityServiceStates) {
                            if (cityBudgetState.cityServiceStates.hasOwnProperty(i)) {
                                var cityServiceState = cityBudgetState.cityServiceStates[i];
                                writeTaxStateRow(cityServiceState,
                                    false,
                                    function (e) { return e.name; },
                                    function (e) { return e.projectedExpenses; },
                                    function (e) { return e.currentRate; },
                                    function (server, cityServiceState) { server.invoke("lowerCityServiceFunding", cityServiceState.name); },
                                    function (server, cityServiceState) { server.invoke("raiseCityServiceFunding", cityServiceState.name); });
                            }
                        }
                        writeTaxStateRow(cityBudgetState.totalCityServiceState,
                            true,
                            function (e) { return e.name; },
                            function (e) { return e.projectedExpenses; });
                    })();

                    (function () {
                        var taxRow = document.createElement("tr");

                        (function () {
                            var labelCell = document.createElement("th");
                            labelCell.innerHTML = "Totals";
                            labelCell.colSpan = 4;
                            taxRow.appendChild(labelCell);
                        })();

                        document.getElementById("budgetTaxTable").appendChild(taxRow);
                    })();

                    (function () {
                        var addRow = function (label, value) {
                            var taxRow = document.createElement("tr");
                            var addCell = function (text) {
                                var labelCell = document.createElement("td");
                                labelCell.innerHTML = text;
                                labelCell.colSpan = 2;
                                taxRow.appendChild(labelCell);
                            };
                            addCell(label);
                            addCell(accounting.formatMoney(value));
                            document.getElementById("budgetTaxTable").appendChild(taxRow);
                        };

                        addRow("Projected income", cityBudgetState.totalTaxState.projectedIncome);
                        addRow("Projected expenses", (0 - cityBudgetState.totalCityServiceState.projectedExpenses));
                        addRow("Total income", cityBudgetState.totalTaxState.projectedIncome - cityBudgetState.totalCityServiceState.projectedExpenses);

                        $("#budgetTaxTable button").button();
                    })();
                });

                simulation.on("submitCityBudgetValue", function (e) {
                    document.getElementById("currentFundsLabel").innerHTML = "Current funds: " + accounting.formatMoney(e.currentAmount);
                    document.getElementById("projectedIncomeLabel").innerHTML = "Projected income: $ " + accounting.formatMoney(e.projectedIncome);
                    cityBudgetStateService.loadNewState(e.cityBudgetState);
                });

            })();

            simulation.on("submitAndDraw", function (zoneInfo) {
                drawZoneInfo(zoneInfo);
            });

            simulation.on("submitZoneInfos", function (zoneInfos) {
                for (var i in zoneInfos) {
                    if (zoneInfos.hasOwnProperty(i)) {
                        var zoneInfo = zoneInfos[i];
                        drawZoneInfo(zoneInfo);
                    }
                }
            });

            simulation.on("submitDataMeterInfos", function (zoneDataMeterInfos) {

                for (var i in zoneDataMeterInfos) {
                    if (zoneDataMeterInfos.hasOwnProperty(i)) {
                        var zoneDataMeterInfo = zoneDataMeterInfos[i];

                        if (zoneDataMeterInfo.colour !== "") {
                            var context = canvasLayer5.getContext("2d");
                            context.clearRect(zoneDataMeterInfo.x * 25, zoneDataMeterInfo.y * 25, 25, 25);
                            context.globalAlpha = 0.5;
                            context.beginPath();
                            context.fillStyle = zoneDataMeterInfo.colour;
                            context.rect(zoneDataMeterInfo.x * 25, zoneDataMeterInfo.y * 25, 25, 25);
                            context.fill();
                        } else {
                            canvasLayer5.getContext("2d").clearRect(zoneDataMeterInfo.x * 25, zoneDataMeterInfo.y * 25, 25, 25);
                        }
                    }
                }
            });

            var processGraphDefinitions = function (graphDefinitions) {
                var graphTabs = document.getElementById("graphTabs");
                var graphTabHeads = document.getElementById("graphTabHeads");

                var refreshChartFunctions = [];

                var drawGraphDefinition = function (graphDefinition, counter) {
                    (function () {
                        var li = document.createElement("li");
                        var a = document.createElement("a");
                        li.appendChild(a);
                        a.href = "#graphTabs-" + counter;
                        a.innerHTML = graphDefinition.title;
                        graphTabHeads.appendChild(li);
                    })();

                    (function () {
                        var div = document.createElement("div");
                        div.id = "graphTabs-" + counter;

                        var img = document.createElement("img");
                        div.appendChild(img);

                        var drawChart = null;

                        (function () {
                            var localGraphDef = graphDefinition;

                            drawChart = function () {
                                img.src = "/graph/" + localGraphDef.webId + "/" + new Date().getTime();
                            };
                        })();

                        refreshChartFunctions.push(drawChart);

                        graphTabs.appendChild(div);
                    })();
                };

                var counter = 1;
                for (var g in graphDefinitions) {
                    if (graphDefinitions.hasOwnProperty(g)) {
                        var graphDefinition = graphDefinitions[g];
                        drawGraphDefinition(graphDefinition, counter);
                        counter++;
                    }
                }

                $("#refreshChartsButton").click(function () {
                    for (var f in refreshChartFunctions) {
                        if (refreshChartFunctions.hasOwnProperty(f)) {
                            refreshChartFunctions[f]();
                        }
                    }
                });
                $("#graphTabs").tabs();
            };

            simulation.on("submitMenuStructure", function (request) {

                processGraphDefinitions(request.graphDefinitions);

                var createButton = function (text, target, clickHandler) {
                    var newButtonElement = document.createElement("button");
                    newButtonElement.innerHTML = text;
                    target.appendChild(newButtonElement);
                    newButtonElement.addEventListener("click", clickHandler);
                    return newButtonElement;
                };
                var overlaySelectionDiv = document.getElementById("overlaySelectionDiv");

                (function () {
                    var registerDataMeter = function (dataMeter) {
                        createButton(dataMeter.name, overlaySelectionDiv, function (e) {
                            simulation.invoke("joinDataMeterGroup", dataMeter.webId);
                            currentDataMeter = dataMeter;
                            canvasLayer5.getContext("2d").clearRect(0, 0, canvasLayer5.width, canvasLayer5.height);

                            if (currentDataMeter.webId !== 0) {
                                simulation.invoke("requestZonesFor",currentDataMeter.webId);
                            }
                        });
                    };

                    var dataMeterInstances = request.dataMeterInstances;

                    registerDataMeter(currentDataMeter);

                    for (var d in dataMeterInstances) {
                        if (dataMeterInstances.hasOwnProperty(d)) {
                            var dataMeter = dataMeterInstances[d];
                            registerDataMeter(dataMeter);
                        }
                    }
                })();


                var incomingButtonDefinitions = request.buttonDefinitions;
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

                var keysAndButtons = [];

                function handleKeyPress(e) {
                    keysAndButtons[String.fromCharCode(e.which)].click();
                }

                window.addEventListener("keypress", handleKeyPress, false);

                var registerButton = function (inputButtonDefinition) {
                    var x = inputButtonDefinition;
                    return function () {
                        document.getElementById("currentButtonLabel").innerHTML = x.buttonDefinition.name +
                            " (Costs: " + accounting.formatMoney(x.buttonDefinition.cost) + ")";
                        currentButton = x.buttonDefinition;
                    };
                };

                for (var i in buttonDefinitionStates) {
                    if (buttonDefinitionStates.hasOwnProperty(i)) {
                        var buttonDefinitionState = buttonDefinitionStates[i];
                        var buttonBar = document.getElementById("buttonBar");
                        if (!buttonDefinitionState.drawn) {
                            var newButtonElement = document.createElement("button");
                            newButtonElement.innerHTML = "(" + buttonDefinitionState.buttonDefinition.keyChar.toUpperCase() + ") " + buttonDefinitionState.buttonDefinition.name;
                            buttonBar.appendChild(newButtonElement);

                            keysAndButtons[buttonDefinitionState.buttonDefinition.keyChar] = newButtonElement;

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
                $("input[type=submit], button").button();
            });

            (function () {
                var previouslyDrawnVehicleTiles = [];
                simulation.on("submitVehicleStates", function (vehicleStates) {

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

                    var selectCanvasFor = function (vehicleState) {
                        return function () { return vehicleState.isShip ? canvasLayer2 : canvasLayer4; };
                    };

                    for (var i in vehicleStates) {
                        if (vehicleStates.hasOwnProperty(i)) {
                            var vehicleState = vehicleStates[i];
                            cancelObj = drawZoneInfoForBitmapLayer(vehicleState,
                                function (x) { return x.bitmapId; },
                                selectCanvasFor(vehicleState),
                                function (x) { return x.pointOne; },
                                true);

                            previouslyDrawnVehicleTiles.push(cancelObj);
                        }
                    }
                });
            })();

            simulation.start().then(function () {

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
                                };

                                this.activateNetworkZoning = function () {
                                    this.reset();
                                    isNetworkZoning = true;
                                };

                                this.activateNetworkDemolishing = function () {
                                    this.reset();
                                    isNetworkDemolishing = true;
                                };

                                this.getIsNetworkZoning = function () {
                                    return isNetworkZoning;
                                };

                                this.getIsNetworkDemolishing = function () {
                                    return isNetworkDemolishing;
                                };

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
                                };
                            };

                            clickAndDragState = new ClickDragState();
                        }());

                        var consumeZone = function (button, cell) {
                            if (lastConsumedCell === null ||
                                lastConsumedCell.x !== cell.x ||
                                lastConsumedCell.y !== cell.y || lastConsumedCell.button !== button) {
                                simulation.invoke("consumeZone", { name: button.name, x: cell.x, y: cell.y });
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
                        };

                        var previousHighlight = null;

                        canvasLayer6.addEventListener("mousemove", function (evt) {
                            var mousePos = getMousePos(canvasLayer1, evt);
                            var cell = { x: Math.floor(mousePos.x / 25), y: Math.floor(mousePos.y / 25) };
                            currentFocusedCell = cell;

                            if (previousHighlight === null || (
                                    previousHighlight.button !== currentButton ||
                                    previousHighlight.cell.x !== cell.x ||
                                    previousHighlight.cell.y !== cell.y ||
                                    previousHighlight.clickDragState !== clickAndDragState.getStateAsString())) {

                                if (previousHighlight !== null)
                                    previousHighlight.clear();
                                if (clickAndDragState.getIsNetworkZoning()) {
                                    consumeZone(currentButton, currentFocusedCell);
                                } else if (clickAndDragState.getIsNetworkDemolishing()) {
                                    consumeZone(clearButton, currentFocusedCell);
                                }

                                previousHighlight = {
                                    cell: cell,
                                    clear: function () {
                                        var context = canvasLayer6.getContext("2d");
                                        context.clearRect(((cell.x + currentButton.horizontalCellOffset) * 25) - 100, ((cell.y + currentButton.verticalCellOffset) * 25) - 100, (currentButton.widthInCells * 25) + 200, (currentButton.heightInCells * 25) + 200);
                                    },
                                    draw: function () {
                                        var context = canvasLayer6.getContext("2d");
                                        context.globalAlpha = 0.5;
                                        context.beginPath();
                                        context.strokeStyle = "red";
                                        context.lineWidth = 1;
                                        context.rect((cell.x + currentButton.horizontalCellOffset) * 25, (cell.y + currentButton.verticalCellOffset) * 25, currentButton.widthInCells * 25, currentButton.heightInCells * 25);
                                        context.stroke();
                                    },
                                    clickDragState: clickAndDragState.getStateAsString(),
                                    button: currentButton
                                };

                                previousHighlight.draw();
                            }

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

                simulation.invoke("requestMenuStructure");
            });
        });
    };
});