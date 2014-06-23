//
// Para cada rota, adiciona informacoes sobre horarios de partida ao JSON.
// Recebe o JSON route e o horario de chegada ao destino
//      arrivalDateTime => DateTime da chegada desejada
//      vooEscolhido => Caso algum voo ja tenha sido escolhido pela tela de alteracao de voo, utiliza-o
function buildItinerarySchedule(route, arrivalDateTime, vooEscolhido) {
	// diferenca entre a data da viagem e agora
	// irá ajudar no calculo do tempo para sair da origem
	var weeklyFrequency = 0;
	var frequency;
	var routePrice = 0;
	if (route.segments != null && route.segments.length > 0) {
		//adiciona data de chegada ao destino final
		route.segments[route.segments.length - 1].arrivalDateTime = null;
		route.segments[route.segments.length - 1].departureDateTime = null;

		for (var i = route.segments.length - 1; i >= 0; i--) {
			//pega data do prox ponto (i + 1) ou do ponto de chegada
			var arrivalDateNextStop = i == route.segments.length - 1 ? arrivalDateTime : route.segments[i + 1].departureDateTime;

		    // Se for voo procura um voo que possibilite chegada no horário
            // no futuro devera pegar de uma fonte externa
			if (route.segments[i].kind == 'flight') {
			    var achouVoo = false;
			    var itineraryDates = null;
			    route.segments[i].itinerarioEscolhido = null; //escolhido automaticamente

			    if (vooEscolhido != null) { // é uma alteração de itinerario, um voo foi alterado
			        if (vooEscolhido[i] == i) {
			            achouVoo = true;
			            route.segments[i].itinerarioEscolhido = vooEscolhido[0];
			            var itinerary = route.segments[i].itineraries[vooEscolhido[0]].legs[0];
			            //verifica se itinerario atual bate com o horario do proximo segmento
			            itineraryDates = calcItineraryDates(itinerary, arrivalDateNextStop);
			        };
			    } else {
			        // para cada opcao de voo
			        for (var j = 0; j < route.segments[i].itineraries.length; j++) {
			            route.segments[i].itineraries[j].validoParaHorario = false; // valido para horario de chegada informado?

			            var itinerary = route.segments[i].itineraries[j].legs[0];
			            //verifica se itinerario atual bate com o horario do proximo segmento
			            var currentItineraryDates = matchflightToSchedule(itinerary, arrivalDateNextStop);

			            // pode ter ou nao voo chegando no dia para chegar no horario
			            if (currentItineraryDates.arrivalDateTime != null) {
			                achouVoo = true;
			                route.segments[i].itineraries[j].validoParaHorario = true;// valido para horario de chegada informado?
			                if (itineraryDates == null) {
			                    itineraryDates = currentItineraryDates;
			                    route.segments[i].itinerarioEscolhido = j;//indice do itinerario escolhido automatico
			                } else {
			                    if (currentItineraryDates.arrivalDateTime > itineraryDates.arrivalDateTime) {
			                        itineraryDates = currentItineraryDates;
			                        route.segments[i].itinerarioEscolhido = j;//indice do itinerario escolhido automatico
			                    };
			                };
			            };
			        };
			    };

			    if (achouVoo) { // tinha um voo que encaixava no horario
			        route.segments[i].departureDateTime = itineraryDates.departureDateTime;
			        route.segments[i].arrivalDateTime = itineraryDates.arrivalDateTime;
			    } else { // Nao tinha nenhum voo que chegaria no horario naquele dia, pegar o voo com maior horario de chegada e determinar que é no dia anterior
			        // acha o itinerario que tem a hora de chegada maior
			        var latestItinerary = findLatestItinerary(route.segments[i]);

			        var itineraryDates = calcItineraryDates(latestItinerary, arrivalDateNextStop);

			        route.segments[i].departureDateTime = itineraryDates.departureDateTime;
			        route.segments[i].arrivalDateTime = itineraryDates.arrivalDateTime;
			    };
			    
			    // calcula o preco do segmento
			    if (route.segments[i].indicativePrice != null && route.segments[i].indicativePrice.price != null) {
			        if (route.segments[i].itineraries[route.segments[i].itinerarioEscolhido].legs[0].indicativePrice != null && route.segments[i].itineraries[route.segments[i].itinerarioEscolhido].legs[0].indicativePrice.price != null) {
			            routePrice += route.segments[i].itineraries[route.segments[i].itinerarioEscolhido].legs[0].indicativePrice.price;
			        };
			    };

			} else { // qualquer tipo de transporte que nao seja voo

                //faz o calculo do preco
			    if (route.segments[i].indicativePrice != null && route.segments[i].indicativePrice.price != null) {
			        routePrice += route.segments[i].indicativePrice.price;
			    };

			    //calcula frequencia semanal
			    weeklyFrequency = getWeeklyFrequency(route.segments[i]);
			    //calcula a frequencia total do segmento em horas e minutos
			    frequency = calcFrequency(weeklyFrequency);

			    //duracao total em minutos, somando frequencia
			    var duration = route.segments[i].duration + frequency.minutes + (frequency.hours * 60);

			    var anticipation = 0; // em minutos
                // se o proximo segmento é voo, calcular chegada com antecipacao
			    if (i < route.segments.length - 1 && route.segments[i + 1].kind == 'flight') {
			        anticipation = 120;
			    };

			    //pega data do prox ponto (i + 1) ou do ponto de chegada
			    var departureDate = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate(), arrivalDateNextStop.getHours(), arrivalDateNextStop.getMinutes() - duration, arrivalDateNextStop.getSeconds());
			    var arrivalDate = new Date(departureDate.getFullYear(), departureDate.getMonth(), departureDate.getDate(), departureDate.getHours(), departureDate.getMinutes() + duration, departureDate.getSeconds());
			    departureDate.setMinutes(departureDate.getMinutes() - anticipation);
			    arrivalDate.setMinutes(arrivalDate.getMinutes() - anticipation);

			    route.segments[i].departureDateTime = departureDate;
			    route.segments[i].arrivalDateTime = arrivalDate;
			};
		};
	};

	route.indicativePrice.price = routePrice;

	return route;
};

//
//
//
function calcItineraryDates(itinerary, arrivalDateNextStop){
    var itinerarySchedule = { departureDateTime: null, arrivalDateTime: null };
    // hora de partida do primeiro voo da opcao atual
    var sTime = itinerary.hops[0].sTime.split(':');
    //hora de chegada do ultimo voo da opcao atual
    var tTime = itinerary.hops[itinerary.hops.length - 1].tTime.split(':');

    var arrDayChange = 0;
    var tempArrivalDate = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate(), tTime[0], tTime[1]);
    if (tempArrivalDate >= arrivalDateNextStop) {//tem que ser no dia anterior
        arrDayChange = 1;
    }

    var depDayChange = arrDayChange;
    for (var j = 0; j < itinerary.hops.length; j++) {
        if (itinerary.hops[j].dayChange != null) {
            depDayChange += itinerary.hops[j].dayChange;
        };
    };

    tempArrivalDate.setDate(tempArrivalDate.getDate() - arrDayChange);
    itinerarySchedule.arrivalDateTime = tempArrivalDate;
    itinerarySchedule.departureDateTime = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate() - depDayChange, sTime[0], sTime[1]);
    
    return itinerarySchedule;
};


//
//
//
function findLatestItinerary(segment) {
    var latestItinerary = null;
    for (var i = 0; i < segment.itineraries.length; i++) {
        var itinerary = segment.itineraries[i].legs[0];
        segment.itineraries[i].validoParaHorario = true;

        //hora de chegada do ultimo voo da opcao atual
        var tTime = itinerary.hops[itinerary.hops.length - 1].tTime.split(':');

        // horario do itinerario atual
        var itineraryDate = new Date();
        itineraryDate.setHours(tTime[0], tTime[1], 0, 0);

        if (latestItinerary == null) { // é o primeiro
            latestItinerary = itinerary;
            segment.itinerarioEscolhido = i;
        } else {
            var latestItineraryTime = latestItinerary.hops[latestItinerary.hops.length - 1].tTime.split(':');
            var latestItineraryDate = new Date();
            latestItineraryDate.setHours(latestItineraryTime[0], latestItineraryTime[1], 0, 0);
            if (latestItineraryDate < itineraryDate) {// achou uma opcao mais tarde
                latestItinerary = itinerary;
                segment.itinerarioEscolhido = i;
            };
        };
    };
    return latestItinerary;
};


//
//
//
function matchflightToSchedule(itinerary, arrivalDateNextStop) {
    var flightOption = { departureDateTime: null, arrivalDateTime: null };

    // hora de partida do primeiro voo da opcao atual
    var sTime = itinerary.hops[0].sTime.split(':');
    //hora de chegada do ultimo voo da opcao atual
    var tTime = itinerary.hops[itinerary.hops.length - 1].tTime.split(':');

    var tempArrivalDate = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate(), tTime[0], tTime[1]);
//    var departureDate = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate(), tTime[0], tTime[1]);

    if (tempArrivalDate <= arrivalDateNextStop) {
        //Achou um voo que chega antes do proximo segmento da viagem

        var dayChange = 0;
        for (var i = 0; i < itinerary.hops.length; i++) {
            if (itinerary.hops[i].dayChange != null) {
                dayChange += itinerary.hops[i].dayChange;
            };
        };

        var departureDate = new Date(arrivalDateNextStop.getFullYear(), arrivalDateNextStop.getMonth(), arrivalDateNextStop.getDate() - dayChange, sTime[0], sTime[1]);


        // calcula a duracao da opcao de voo
        //var duration = 0;
        //for (var i = 0; i < itinerary.hops.length; i++) {
        //    duration += itinerary.hops[i].duration;
        //};

        //departureDate.setMinutes(tempArrivalDate.getMinutes() - duration);//reze para que ele sempre some corretamente

        flightOption.departureDateTime = departureDate;
        flightOption.arrivalDateTime = tempArrivalDate;
    };
    return flightOption;
};

//
// 
//
function getWeeklyFrequency(segment) {
	var weeklyFrequency = 0;
	if (segment.itineraries != null && segment.kind != 'flight') {
	    if (segment.itineraries[0].legs != null && segment.itineraries[0].legs.length > 0) {
	        if (segment.itineraries[0].legs[0].hops != null) {
	            for (var i = 0; i < segment.itineraries[0].legs[0].hops.length; i++) {
	                if (segment.itineraries[0].legs[0].hops[i].frequency != null) {
	                    weeklyFrequency += segment.itineraries[0].legs[0].hops[i].frequency;
	                };
	            };
	        };
	    };
	};
	return weeklyFrequency;
};

//
// calcula frequencia em horas e minutos baseado no numero de viagens por semana (weeklyFrequency)
//
function calcFrequency(weeklyFrequency) {
	var freq = { hours: 0, minutes: 0 };

	if (weeklyFrequency > 0) {
	    var DIAS_NA_SEMAMA = 7;
	    var HORAS_NA_SEMAMA = 168;
	    var absHourFrequency = Math.floor(weeklyFrequency / HORAS_NA_SEMAMA);
	    var modHourFrequency = 0;

	    if (weeklyFrequency / DIAS_NA_SEMAMA > 24) { // mais do que um a cada hora
	        freq.minutes = Math.floor(60 / (weeklyFrequency / HORAS_NA_SEMAMA));
	    }
	    else if ((weeklyFrequency / DIAS_NA_SEMAMA < 24)) { // menos do que um a cada hora
	        var porDia = weeklyFrequency / DIAS_NA_SEMAMA;
	        var aCadaXHoras = Math.floor(24 / porDia);

	        freq.hours = aCadaXHoras;

	        if (aCadaXHoras != (24 / porDia)) { // nao é hora redonda
	            var aCadaXMinutos = (24 / porDia) - aCadaXHoras;
	            var min = Math.round(aCadaXMinutos * 60);
	            freq.minutes = min;
	        };
	    }
	    else { //==1
	        freq.hours = 1;
	    };
	};
	return freq;
};

