//
//
//
function buscar(originalRoute) {
	//tem que escolher origem e destino
    if (_placeOrigem && _placeDestino && _placeOrigem.geometry && _placeDestino.geometry) {
        _resp = null;
        if (originalRoute) {
            _chosenRoute = new Array(2);
            _chosenRoute[0] = null;
            _chosenRoute[1] = null;
            _chosenLeg = null;
        };
        //loading
	    $("#mapa").hide();
	    $("#divResults").show();
	    $("#divDetalhesItinerario").text("Please wait...");
	    $("#divDetalhesItinerario").accordion("option", "active", false);
	    $("#divDetalhesItinerario").accordion("refresh");

	    $("#divDetalhesItinerarioVolta").text("Please wait...");
	    $("#divDetalhesItinerarioVolta").accordion("option", "active", false);
	    $("#divDetalhesItinerarioVolta").accordion("refresh");

	    $("#divTotais").text("Please wait...");

	    var arrivalDateServer = _dataChegada.getFullYear() + "-" + (_dataChegada.getMonth() + 1).toString().padLeft(2, '0') + "-" + _dataChegada.getDate().toString().padLeft(2, '0') + "T" + _dataChegada.getHours().toString().padLeft(2, '0') + ":" + _dataChegada.getMinutes().toString().padLeft(2, '0') + ":00";
	    var returnDateServer = $('#rdIdaeVolta').is(':checked') ? _dataRetorno.getFullYear() + "-" + (_dataRetorno.getMonth() + 1).toString().padLeft(2, '0') + "-" + _dataRetorno.getDate().toString().padLeft(2, '0') + "T" + _dataRetorno.getHours().toString().padLeft(2, '0') + ":" + _dataRetorno.getMinutes().toString().padLeft(2, '0') + ":00" : null;

	    var d2dSender = new Door2DoorSender({
	        desiredReturnDate: returnDateServer,
	        desiredArrivalDate: arrivalDateServer,
	        includePublicTransport: buildSearchRequestFlags(),
	        oriLat: _placeOrigem.geometry.location.k,
	        oriLng: _placeOrigem.geometry.location.A,
	        oriType: (_placeOrigem.types != null && _placeOrigem.types.length > 0) ? _placeOrigem.types[0] : "",
	        destLat: _placeDestino.geometry.location.k,
	        destLng: _placeDestino.geometry.location.A,
	        destType: (_placeDestino.types != null && _placeDestino.types.length > 0) ? _placeDestino.types[0] : "",
	        chosenRoute: _chosenRoute,
	        successCallback: handleSuccessSearch,
	        errorCallback: function (xhr, status, errorThrown) {
	            $("#divDetalhesItinerario").text("There was an error :( See console for details.");
	            console.log("Error: " + errorThrown);
	            console.log("Status: " + status);
	            console.dir(xhr);
	        },
	        completeCallback: function (xhr, status) {
	            console.log("Status: " + status);
	            console.dir(xhr);
	        }
	    });
		d2dSender.getd2d();
	}
	else {
	    $("#divDetalhesItinerario").text("Please choose origin and destination...");
	};
};

//
//handles event triggered when successfull ajax call is returned from supplier
//
function handleSuccessSearch(d2d) {
    if (d2d != null) {
        _resp = d2d;
        renderResult();
        renderTotals();
    };
};

//
//
//
function renderTotals() {
    $("#divTotais").text("");
    var htmlResult = '';
    var indiceRouteIda = $('input[name=rdRouteIda]:checked').val();
    var routeIda = _resp.legResponse[0].routes[indiceRouteIda];
    var indiceRouteVolta;
    var routeVolta;
    var custoTotalCarro = 0, custoTotalOnibus = 0, custoTotalTrem = 0, custoTotalVoo = 0, custoTotal = 0;

    custoTotalCarro = routeIda.routeTotals.totalPriceOfCar;
    custoTotalOnibus = routeIda.routeTotals.totalPriceOfBus;
    custoTotalTrem = routeIda.routeTotals.totalPriceOfTrain;
    custoTotalVoo = routeIda.routeTotals.totalPriceOfFlight;
    custoTotal = routeIda.routeTotals.totalPriceOfTrain + routeIda.routeTotals.totalPriceOfCar + routeIda.routeTotals.totalPriceOfBus + routeIda.routeTotals.totalPriceOfFlight;

    if (_resp.legResponse.length > 1) {
        indiceRouteVolta = $('input[name=rdRouteVolta]:checked').val();
        routeVolta = _resp.legResponse[1].routes[indiceRouteVolta];

        custoTotalCarro += routeVolta.routeTotals.totalPriceOfCar;
        custoTotalOnibus += routeVolta.routeTotals.totalPriceOfBus;
        custoTotalTrem += routeVolta.routeTotals.totalPriceOfTrain;
        custoTotalVoo += routeVolta.routeTotals.totalPriceOfFlight;
        custoTotal += routeVolta.routeTotals.totalPriceOfTrain + routeVolta.routeTotals.totalPriceOfCar + routeVolta.routeTotals.totalPriceOfBus + routeVolta.routeTotals.totalPriceOfFlight;
    }
    //
    // TEMPOS
    //
    htmlResult = '<table>';
    htmlResult += '     <tr class="ui-widget-header ">'
    htmlResult += '         <th>             </th>';
    htmlResult += '         <th>Distancia Total</th>';
    htmlResult += '         <th>Tempo em trens</th>';
    htmlResult += '         <th>Tempo em automóveis</th>';
    htmlResult += '         <th>Tempo em voos</th>';
    htmlResult += '         <th>Tempo em onibus</th>';
    htmlResult += '         <th>Tempo a pé</th>';
    htmlResult += '         <th>Tempo esperando</th>';
    htmlResult += '     </tr>';
    htmlResult += '     <tr>';
    htmlResult += '         <td><strong>Ida</strong></th>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalDistance.toFixed(2) + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeOnTrain + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeOnCar + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeOnFlight + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeOnBus + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeOnWalk + '</td>';
    htmlResult += '         <td align="right">' + routeIda.routeTotals.totalTimeWaiting + '</td>';
    htmlResult += '     </tr>';
    if (_resp.legResponse.length > 1) {
        htmlResult += '     <tr>';
        htmlResult += '         <td><strong>Volta</strong></th>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalDistance.toFixed(2) + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeOnTrain + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeOnCar + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeOnFlight + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeOnBus + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeOnWalk + '</td>';
        htmlResult += '         <td align="right">' + routeVolta.routeTotals.totalTimeWaiting + '</td>';
        htmlResult += '     </tr>';
    }
    htmlResult += '</table><br/><br/>';
    //
    // CUSTOS
    //
    htmlResult += '<table>';
    htmlResult += '     <tr class="ui-widget-header ">'
    htmlResult += '         <th>             </th>';
    htmlResult += '         <th>Custo total com trens</th>';
    htmlResult += '         <th>Custo total com carro </th>';
    htmlResult += '         <th>Custo total com ônibus</th>';
    htmlResult += '         <th>Custo total com voos</th>';
    htmlResult += '         <th>Custo total</th>';
    htmlResult += '     </tr>';
    htmlResult += '     <tr>';
    htmlResult += '         <td><strong>Ida</strong></th>';
    htmlResult += '         <td align="right">R$ ' + routeIda.routeTotals.totalPriceOfTrain.toFixed(2) + '</td>';
    htmlResult += '         <td align="right">R$ ' + routeIda.routeTotals.totalPriceOfCar.toFixed(2) + '</td>';
    htmlResult += '         <td align="right">R$ ' + routeIda.routeTotals.totalPriceOfBus.toFixed(2) + '</td>';
    htmlResult += '         <td align="right">R$ ' + routeIda.routeTotals.totalPriceOfFlight.toFixed(2) + '</td>';
    htmlResult += '         <td align="right">R$ ' + (routeIda.routeTotals.totalPriceOfTrain + routeIda.routeTotals.totalPriceOfCar + routeIda.routeTotals.totalPriceOfBus + routeIda.routeTotals.totalPriceOfFlight).toFixed(2) + '</td>';
    htmlResult += '     </tr>';
    if (_resp.legResponse.length > 1) {
        htmlResult += '     <tr>';
        htmlResult += '         <td><strong>Volta</strong></th>';
        htmlResult += '         <td align="right">R$ ' + routeVolta.routeTotals.totalPriceOfTrain.toFixed(2) + '</td>';
        htmlResult += '         <td align="right">R$ ' + routeVolta.routeTotals.totalPriceOfCar.toFixed(2) + '</td>';
        htmlResult += '         <td align="right">R$ ' + routeVolta.routeTotals.totalPriceOfBus.toFixed(2) + '</td>';
        htmlResult += '         <td align="right">R$ ' + routeVolta.routeTotals.totalPriceOfFlight.toFixed(2) + '</td>';
        htmlResult += '         <td align="right">R$ ' + (routeVolta.routeTotals.totalPriceOfTrain + routeVolta.routeTotals.totalPriceOfCar + routeVolta.routeTotals.totalPriceOfBus + routeVolta.routeTotals.totalPriceOfFlight).toFixed(2) + '</td>';
        htmlResult += '     </tr>';
    }
    htmlResult += '     <tr>';
    htmlResult += '         <td><strong>Total</strong></th>';
    htmlResult += '         <td align="right"><strong>R$ ' + custoTotalTrem.toFixed(2) + '</strong></td>';
    htmlResult += '         <td align="right"><strong>R$ ' + custoTotalCarro.toFixed(2) + '</strong></td>';
    htmlResult += '         <td align="right"><strong>R$ ' + custoTotalOnibus.toFixed(2) + '</strong></td>';
    htmlResult += '         <td align="right"><strong>R$ ' + custoTotalVoo.toFixed(2) + '</strong></td>';
    htmlResult += '         <td align="right"><strong>R$ ' + custoTotal.toFixed(2) + '</strong></td>';
    htmlResult += '     </tr>';
    htmlResult += '</table><br/><br/>';

    if (_resp.legResponse.length > 1) {

        var dataSaida = Date.parse(routeIda.segments[0].departureDateTime);
        var dataChegada = Date.parse(routeVolta.segments[routeVolta.segments.length - 1].arrivalDateTime);
        var duracaoTotalViagem = getTimeDiff(dataSaida, dataChegada);

        var dataChegadaDestino = Date.parse(routeIda.segments[routeIda.segments.length - 1].arrivalDateTime);
        var dataSaidaDestino = Date.parse(routeVolta.segments[0].departureDateTime);
        var duracaoDestino = getTimeDiff(dataChegadaDestino, dataSaidaDestino);


        htmlResult += '<table>';
        htmlResult += '     <tr class="ui-widget-header ">'
        htmlResult += '         <th>Tempo total da viagem</th>';
        htmlResult += '         <th>Tempo total no destino</th>';
        htmlResult += '     </tr>';
        htmlResult += '     <tr>'
        htmlResult += '         <td>' + duracaoTotalViagem.days + '.' + duracaoTotalViagem.hours.toString().padLeft(2, '0') + ':' + duracaoTotalViagem.minutes.toString().padLeft(2, '0') + ':' + duracaoTotalViagem.seconds.toString().padLeft(2, '0') + '</td>';
        htmlResult += '         <td>' + duracaoDestino.days + '.' + duracaoDestino.hours.toString().padLeft(2, '0') + ':' + duracaoDestino.minutes.toString().padLeft(2, '0') + ':' + duracaoDestino.seconds.toString().padLeft(2, '0') + '</td>';
        htmlResult += '     </tr>';


    }
    $("#divTotais").html(htmlResult);
    $('input[name=rdRouteVolta]').change(onRouteChoiceChanged);
    $('input[name=rdRouteIda]').change(onRouteChoiceChanged);
}

//
//
//
function onRouteChoiceChanged() {
    renderTotals();
}

//
//
//
function renderResult() {
    $("#divDetalhesItinerario").text("");
    $("#divDetalhesItinerarioVolta").text("");

    htmlResult = "";
	for (var i = 0; i < _resp.legResponse[0].routes.length; i++) {
	    var route = _resp.legResponse[0].routes[i];
	    var price = route.indicativePrice && route.indicativePrice.price != null && route.indicativePrice.price != 0 && !isNaN(route.indicativePrice.price) ? " - R$ " + route.indicativePrice.price + ",00" : '';
	    var checked = i == 0 ? 'checked' : '';
	    htmlResult += "<h3 id='route" + i + "'><input type='radio' name='rdRouteIda' " + checked + " value='" + i + "'/>Opção " + (i + 1) + " - " + route.name + price + "</h3><div>";

		htmlResult += renderStops(i, 0);

		htmlResult += "</div>";
	};
	$("#divDetalhesItinerario").html(htmlResult);
	$("#divDetalhesItinerario").accordion("refresh");

	if (_chosenRoute[0] != null) {
	    $("#divDetalhesItinerario").accordion("option", "active", parseInt(_chosenRoute[0].routeIndex));
	};

    /////////////////////////////////////////VOTA
	htmlResult = "";
	if (_resp.legResponse.length > 1) {
	    for (var i = 0; i < _resp.legResponse[1].routes.length; i++) {
	        var route = _resp.legResponse[1].routes[i];
	        var price = route.indicativePrice && route.indicativePrice.price != null && route.indicativePrice.price != 0 && !isNaN(route.indicativePrice.price) ? " - R$ " + route.indicativePrice.price + ",00" : '';
	        var checked = i == 0 ? 'checked' : '';

	        htmlResult += "<h3 id='route" + i + "'><input type='radio' name='rdRouteVolta' " + checked + " value='" + i + "'/>Opção " + (i + 1) + " - " + route.name + price + "</h3><div>";

	        htmlResult += renderStops(i, 1);

	        htmlResult += "</div>";
	    };
	    $("#divDetalhesItinerarioVolta").html(htmlResult);
	    $("#divDetalhesItinerarioVolta").accordion("refresh");

	    if (_chosenRoute[1] != null) {
	        $("#divDetalhesItinerarioVolta").accordion("option", "active", parseInt(_chosenRoute[1].routeIndex));
	    };
	}

	$("#divTabs").tabs("option", "heightStyle", "auto");
	$("#divTabs").tabs("refresh");

    // para poder colocar o radio button no header do accordion
	$('input[type=radio]').on('click', function (e) { e.stopPropagation(); });
};

//
//
//
function renderStops(routeIndex, legIndex) {
    var htmlResult = "";
    var route = _resp.legResponse[legIndex].routes[routeIndex];
	for (var i = 0; i < route.segments.length; i++) {
		var segment = route.segments[i];

		var sName = ''; //segment.kind == 'flight' ? segment.sCode : (segment.sName == "Origin" ? _placeOrigem.adr_address.split(',')[0] : segment.sName);
		var tName = ''; //segment.kind == 'flight' ? segment.tCode + "&nbsp;<a href='javascript:void(0);' style='color:blue' onclick='javascript:showFlightOptionsAlternatives(" + i + ", " + routeIndex + ")'>Alterar</a>" + confirmar : (segment.tName == "Destination" ? _placeDestino.adr_address.split(',')[0] : segment.tName);
		var confirmar = _reqObj == null ? "" : "&nbsp;/&nbsp;<a href='javascript:void(0);' style='color:blue' onclick='javascript:confirmFlightOption(" + i + ", " + routeIndex + "," + legIndex + ")'>Confirmar</a>";
		var kind = segment.kind == 'car' ? segment.vehicle : segment.kind;

		if (segment.kind == 'flight') {
		    sName = segment.sCode;
		    tName = segment.tCode + "&nbsp;<a href='javascript:void(0);' style='color:blue' onclick='javascript:showFlightOptionsAlternatives(" + i + ", " + routeIndex + "," + legIndex + ")'>Alterar</a>" + confirmar;
		} else {
		    if (legIndex == 1) {//ida e volta
		        sName = (segment.sName == "Origin" ? _placeDestino.adr_address.split(',')[0] : segment.sName);
		        tName = (segment.tName == "Destination" ? _placeOrigem.adr_address.split(',')[0] : segment.tName);
		    } else {
		        sName = (segment.sName == "Origin" ? _placeOrigem.adr_address.split(',')[0] : segment.sName);
		        tName = (segment.tName == "Destination" ? _placeDestino.adr_address.split(',')[0] : segment.tName);
		    }
		}



		htmlResult += "<br><strong>Transporte tipo:</strong> " +
            kind +
            " -  " +
            sName +
            " => " +
            tName;

		htmlResult += renderFrequency(segment);

		htmlResult += renderDuration(segment);

		htmlResult += renderPrice(segment);

		htmlResult += renderSchedule(segment);
	};
	return htmlResult;
};

//
//
//
function renderSchedule(segment) {
    var htmlResult = "";
	if ( segment.departureDateTime != null) {
	    htmlResult += "<br>&nbsp;&nbsp;&nbsp;&nbsp;<strong>Partir às:</strong> " +
             dateToString(segment.departureDateTime);

	    if (segment.arrivalDateTime != null) {
	        htmlResult += "<br>&nbsp;&nbsp;&nbsp;&nbsp;<strong>Chegar às:</strong> " +
                dateToString(segment.arrivalDateTime);
	    };
	};
	return htmlResult;
};

//
//
//
function renderPrice(segment) {
    var htmlResult = "";
    var harPrice = false;
    var price = "";
    if (segment.kind == 'flight') {
        harPrice = segment.itineraries[segment.chosenItinerary] != null && segment.itineraries[segment.chosenItinerary].legs[0].indicativePrice.price != 0;
        if (harPrice) {
            price = "R$ " + segment.itineraries[segment.chosenItinerary].legs[0].indicativePrice.price + ",00";
        };
    } else {
        hasPrice = segment.indicativePrice != null && segment.indicativePrice.price != null && segment.indicativePrice.price != 0;
        if (hasPrice)
            price = "R$ " + segment.indicativePrice.price + ",00";
    };

    if (hasPrice) {
		htmlResult += "<br>&nbsp;&nbsp;&nbsp;&nbsp;<strong>Preço:</strong> " +
            price;
	};
	return htmlResult;
};

//
//Calculo da duração, que é retornada n/minuto
//
function renderDuration(segment) {
	var horas = 0;
	var sHoras = "";
	var sMinutos = "";
	var duracao = "";
	var htmlResult = "";

	if (segment.duration != null) {
		if (segment.duration > 60) {

			horas = Math.floor(segment.duration / 60);
			if (horas > 1) {
				sHoras = horas + " horas ";
			}
			else {
				horas = 1;
				sHoras = "1 hora ";
			};

			if (segment.duration % 60 > 0) {
				var min = segment.duration - (60 * horas);
				sMinutos = min + " minutos";
			};

			duracao = sHoras + sMinutos;
		}
		else {
			duracao = segment.duration + " minutos";
		};
		
		htmlResult += "<br>&nbsp;&nbsp;&nbsp;&nbsp;<strong>Duracao:</strong> " +
             duracao;
	};
	return htmlResult;
};

//
//As frequencias são retornadas em semanas, entao é necessario calcular por hora 
//
function renderFrequency(segment) {
	var frequencia = "";
	var horas = "";
	var minutos = "";
	var ok = false;
	var htmlResult = "";

	var frequency = { hours: 0, minutes: 0 };
	if (segment.frequency != null) {
	    frequency.hours = parseInt(segment.frequency.split(':')[0]);
	    frequency.minutes = parseInt(segment.frequency.split(':')[1]);
	    ok = frequency.hours > 0 || frequency.minutes > 0;
	}

	if (ok > 0) {

		if (frequency.minutes == 1) {
		    minutos = frequency.hours > 0 ? " e 1 minuto" : "A cada 1 minuto";
		} else if (frequency.minutes > 1) {
		    minutos = frequency.hours > 0 ? " e " : "A cada ";
			minutos += frequency.minutes + " minutos";
		};

		if (frequency.hours == 1 && frequency.minutes == 0) {
			horas = "A cada 1 hora";
		}
		else {
			if (frequency.hours > 0)
				horas = "A cada " + frequency.hours + (frequency.hours == 1 ? " hora" : " horas");
		};

		frequencia = horas + minutos;

		htmlResult += "<br>&nbsp;&nbsp;&nbsp;&nbsp;<strong>Frequencia:</strong> " +
            frequencia;
	};
	return htmlResult;
};

//
//
//
function buildSearchRequestFlags() {
    return $('#chkIncludePublicTransport').is(':checked');
};

//
// Posta resultado do voo escolhido para a proxima pagina, definida via POST
//
function confirmFlightOption(segmentIndex, routeIndex, legIndex) {
    var route = _resp.legResponse[legIndex].routes[routeIndex];
    var segment = route.segments[segmentIndex].itineraries[route.segments[segmentIndex].chosenItinerary];
    _chosenLeg = legIndex;

    // post selected itinerary
    var postContent = JSON.stringify(segment);
    $('#hidchosenItin').val(postContent);
    $('#form1').attr('action', _reqObj.outputUrl);
    $('#form1').submit();
};

//
// abre modal para alteracao de opcao de voo para determiado segmento(segmentIndex), de determinada rota(routeIndex)
//
function showFlightOptionsAlternatives(segmentIndex, routeIndex, legIndex) {
    var route = _resp.legResponse[legIndex].routes[routeIndex];
    _chosenLeg = legIndex;

    /*********** <GAMBIARRA PARA DEMO SOMENTE, TROCAR ISTO POR CHAMADA REAL DE VOO> ***********/
    $("#divFlightOptionsAlternatives").html("");
    $("#divFlightOptionsAlternatives").append("<iframe id='iFrameChangeItin' name='iFrameChangeItin' style='width:100%; height:100%'></iframe>");
    $("#divFlightOptionsAlternatives").dialog({
        autoOpen: false,
        width: 777, //hell yeah
        height: window.innerHeight - 50,
        modal: true
    });

    var segment = route.segments[segmentIndex];
    // post selected itinerary
    var postContent = JSON.stringify(segment);

    var arrDate = '';
    if (segmentIndex < route.segments.length - 1) { // não é o ultimo
        arrDate = route.segments[segmentIndex + 1].departureDateTime;
    }
    $('#hidr2r_resp').val(postContent);
    $('#hidarrdate').val(arrDate);
    $('#hisegmentIndex').val(segmentIndex);
    $('#hidrouteIndex').val(routeIndex);
    $('#hidlegIndex').val(legIndex);

    $('#frmChangeItin').attr('action', _reqObj.iframeInputUrl);
    $('#frmChangeItin').submit();
    $("#divFlightOptionsAlternatives").dialog("open");
    /*********** </GAMBIARRA PARA DEMO SOMENTE, TROCAR ISTO POR CHAMADA REAL DE VOO> ***********/

};