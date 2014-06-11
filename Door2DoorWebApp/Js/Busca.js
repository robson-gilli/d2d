//
//
//
function buscar(originalRoute) {

    //
	//tem que escolher origem e destino
    if (_placeOrigem && _placeDestino && _placeOrigem.geometry && _placeDestino.geometry) {
        _resp = null;
        if (originalRoute) {
            _chosenRoute = null;
        };
        //loading
	    $("#mapa").hide();
	    $("#divResults").show();
	    $("#divDetalhesItinerario").text("Please wait...");
	    $("#divDetalhesItinerario").accordion("option", "active", false);
	    $("#divDetalhesItinerario").accordion("refresh");

	    var dataServer = _dataChegada.getFullYear() + "-" + (_dataChegada.getMonth() + 1).toString().padLeft(2, '0') + "-" + _dataChegada.getDate().toString().padLeft(2, '0') + "T" + _dataChegada.getHours().toString().padLeft(2, '0') + ":" + _dataChegada.getMinutes().toString().padLeft(2, '0') + ":00";

	    var d2dSender = new Door2doorSender({
	        desiredArrivalDate: dataServer,
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
    if (d2d != null)
        _resp= d2d;

	renderResult();
	renderResultOnMap();
};

//
//
//
function renderResultOnMap() {
    var mapOptions = {
        center: new google.maps.LatLng(
            _placeDestino.geometry.location.k,
            _placeDestino.geometry.location.A),
            zoom: 13
    };
    //var map = new google.maps.Map(document.getElementById('map-results'), mapOptions);

};

//
//
//
function renderResult() {
    $("#divDetalhesItinerario").text("");
    htmlResult = "";
    htmlResult = "";
	for (var i = 0; i < _resp.routes.length; i++) {
	    var route = _resp.routes[i];
	    var price = route.indicativePrice && route.indicativePrice.price != null && route.indicativePrice.price != 0 && !isNaN(route.indicativePrice.price) ? " - R$ " + route.indicativePrice.price + ",00" : '';

		htmlResult += "<h3 id='route" + i + "'>Opção " + (i + 1) + " - " + route.name + price + "</h3><div>";

		htmlResult += renderStops(i);

		htmlResult += "</div>";
	};
	$("#divDetalhesItinerario").html(htmlResult);
	$("#divDetalhesItinerario").accordion("refresh");

	if (_chosenRoute != null) {
	    $("#divDetalhesItinerario").accordion("option", "active", parseInt(_chosenRoute[2]));
	};
};

//
//
//
function renderStops(routeIndex) {
    var htmlResult = "";
    var route = _resp.routes[routeIndex];
	for (var i = 0; i < route.segments.length; i++) {
		var segment = route.segments[i];
		var sName = segment.kind == 'flight' ? segment.sCode : (segment.sName == "Origin" ? _placeOrigem.adr_address.split(',')[0] : segment.sName);
		var tName = segment.kind == 'flight' ? segment.tCode + "&nbsp;<a href='javascript:void(0);' style='color:blue' onclick='javascript:showFlightOptionsAlternatives(" + i + ", " + routeIndex + ")'>Alterar</a>" : (segment.tName == "Destination" ? _placeDestino.adr_address.split(',')[0] : segment.tName);
		var kind = segment.kind == 'car' ? segment.vehicle : segment.kind;

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

	var frequency = { hours: 0, minutes: 0 };
	if (segment.frequency != null) {
	    frequency.hours = segment.frequency.split(':')[0];
	    frequency.hours = segment.frequency.split(':')[1];
	    ok = true;
	}

	var htmlResult = "";

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
// abre modal para alteracao de opcao de voo para determiado segmento(segmentIndex), de determinada rota(routeIndex)
//
function showFlightOptionsAlternatives(segmentIndex, routeIndex) {
    var route = _resp.routes[routeIndex];
    var htmlResult = "";
    var pMessage = "";
    for (var i = 0; i < route.segments[segmentIndex].itineraries.length; i++) {
        var itinerary = route.segments[segmentIndex].itineraries[i];
        var voos = "";
        var cias = "";
        var origens = "";
        var horariosPartida = "";
        var destinos = "";
        var horariosChegadas = "";
        var preco = "R$ " + itinerary.legs[0].indicativePrice.price + ",00";

        for (var j = 0; j < itinerary.legs[0].hops.length; j++) {
            var isLast = j == itinerary.legs[0].hops.length - 1;
            voos +=             isLast ? itinerary.legs[0].hops[j].flight   : itinerary.legs[0].hops[j].flight + "<br/>";
            cias +=             isLast ? itinerary.legs[0].hops[j].airline  : itinerary.legs[0].hops[j].airline + "<br/>";
            origens +=          isLast ? itinerary.legs[0].hops[j].sCode    : itinerary.legs[0].hops[j].sCode + "<br/>";
            horariosPartida +=  isLast ? itinerary.legs[0].hops[j].sTime    : itinerary.legs[0].hops[j].sTime + "<br/>";
            destinos +=         isLast ? itinerary.legs[0].hops[j].tCode    : itinerary.legs[0].hops[j].tCode + "<br/>";
            horariosChegadas += isLast ? itinerary.legs[0].hops[j].tTime    : itinerary.legs[0].hops[j].tTime + "<br/>";
        };

        // deixa selecionado segmento escolhido automaticamente
        var checked = "";
        if (route.segments[segmentIndex].chosenItinerary != null) {
            if (route.segments[segmentIndex].chosenItinerary == i) {
                checked = "checked";
            }
        }

        var trOptions = ""
        var radioOptions = "";
        
        //segmentos inva;lidos marcados em vermelho e indisponiveis para escolha
        if (!itinerary.validForSchedule) {
            trOptions = "style='color:red'";
            radioOptions = 'disabled';
            pMessage = "<p style='color:red'>Opções em vermelho não se encaixam no horário informado.</p>"
        }

        htmlResult += " <tr " + trOptions + ">";
        htmlResult += "     <td><input type='radio' name='rdEscolha' class='rdEscolha' value='" + i + "|" + segmentIndex + "|" + routeIndex + "' " + checked + " " + radioOptions + "/></td>"; // itineraryIndex|SegmentIndex|routeIndex
        htmlResult += "     <td>" + voos + "</td>";
        htmlResult += "     <td>" + cias + "</td>";
        htmlResult += "     <td>" + origens + "</td>";
        htmlResult += "     <td>" + horariosPartida + "</td>";
        htmlResult += "     <td>" + destinos + "</td>";
        htmlResult += "     <td>" + horariosChegadas + "</td>";
        htmlResult += "     <td>" + preco + "</td>";
        htmlResult += " </tr>";
    };

    $("#divInvalidFlightOptions").html(pMessage);
    $("#tdOpcoesVoo tbody").text("");
    $("#tdOpcoesVoo tbody").append(htmlResult);
    $("#divFlightOptionsAlternatives").dialog({
        autoOpen: false,
        width: 666,
        modal: true,
        closeOnEscape: false,
        open: function(event, ui) { $(".ui-dialog-titlebar-close").hide(); },
        buttons: {
            "Confirmar escolha": function () {
                // troca a opção de voo
                changeFlightOption();

                $(this).dialog("close");
            },
            Cancel: function () {
                _chosenRoute = null;
                $(this).dialog("close");
            }
        },
        close: function () {
            //
        },
        beforeClose: function (event, ui) {
            //
        }
    });

    //date.js
    var date = Date.parse(route.segments[segmentIndex].departureDateTime);
    $("#divFlightOptionsAlternatives").dialog("option", "title", "Voos partindo dia " + date.toLocaleDateString());
    $("#divFlightOptionsAlternatives").dialog("open");
    $(".rdEscolha").click(function (eventData) {
        handleRdClick(eventData);
    });
};


//
//
//
function handleRdClick(eventData) {
    // itineraryIndex|SegmentIndex|routeIndex
    _chosenRoute = eventData.target.value.split("|");
};


//
// efetua a troca de opcao de voo
//
function changeFlightOption() {
    if (_chosenRoute != null) {
        buscar(false);
    };

};

