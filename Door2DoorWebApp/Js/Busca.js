//
//
//
function buscar(originalRoute) {
	//tem que escolher origem e destino
    if (_placeOrigem && _placeDestino && _placeOrigem.geometry && _placeDestino.geometry) {
        _resp = null;
        if (originalRoute) {
            _chosenRoute = new Array(2);
            _chosenLeg = null;
        };
        //loading
	    $("#mapa").hide();
	    $("#divResults").show();
	    $("#divDetalhesItinerario").text("Please wait...");
	    $("#divDetalhesItinerario").accordion("option", "active", false);
	    $("#divDetalhesItinerario").accordion("refresh");
	    $("#divDetalhesItinerarioVolta").accordion("option", "active", false);
	    $("#divDetalhesItinerarioVolta").accordion("refresh");


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
    };
};

//
//
//
function renderResult() {
    $("#divDetalhesItinerario").text("");
    $("#divDetalhesItinerarioVolta").text("");

    htmlResult = "";
	for (var i = 0; i < _resp[0].routes.length; i++) {
	    var route = _resp[0].routes[i];
	    var price = route.indicativePrice && route.indicativePrice.price != null && route.indicativePrice.price != 0 && !isNaN(route.indicativePrice.price) ? " - R$ " + route.indicativePrice.price + ",00" : '';

		htmlResult += "<h3 id='route" + i + "'>Opção " + (i + 1) + " - " + route.name + price + "</h3><div>";

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
	if (_resp.length > 1) {
	    for (var i = 0; i < _resp[1].routes.length; i++) {
	        var route = _resp[1].routes[i];
	        var price = route.indicativePrice && route.indicativePrice.price != null && route.indicativePrice.price != 0 && !isNaN(route.indicativePrice.price) ? " - R$ " + route.indicativePrice.price + ",00" : '';

	        htmlResult += "<h3 id='route" + i + "'>Opção " + (i + 1) + " - " + route.name + price + "</h3><div>";

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
};

//
//
//
function renderStops(routeIndex, legIndex) {
    var htmlResult = "";
    var route = _resp[legIndex].routes[routeIndex];
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
    var route = _resp[legIndex].routes[routeIndex];
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
    var route = _resp[legIndex].routes[routeIndex];
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