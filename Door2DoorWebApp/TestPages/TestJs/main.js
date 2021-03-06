﻿var _chosenIt = null;
var _postData=null;
//
// document ready
//
$(document).ready(function () {

    _postData = getPostData();

    var htmlResult = "";
    var pMessage = "";
    var trOptions;
    var radioOptions;
    var itinerary;
    var voos;
    var cias;
    var origens;
    var horariosPartida;
    var destinos;
    var horariosChegadas;
    var preco;
    var isLast = false;
    var checked;

    for (var i = 0; i < _postData.segment.itineraries.length; i++) {
        itinerary = _postData.segment.itineraries[i];
        voos = "";
        cias = "";
        origens = "";
        horariosPartida = "";
        destinos = "";
        horariosChegadas = "";
        preco = "R$ " + itinerary.legs[0].indicativePrice.price + ",00";

        for (var j = 0; j < itinerary.legs[0].hops.length; j++) {
            var isLast = j == itinerary.legs[0].hops.length - 1;
            voos += isLast ? itinerary.legs[0].hops[j].flight : itinerary.legs[0].hops[j].flight + "<br/>";
            cias += isLast ? itinerary.legs[0].hops[j].airline : itinerary.legs[0].hops[j].airline + "<br/>";
            origens += isLast ? itinerary.legs[0].hops[j].sCode : itinerary.legs[0].hops[j].sCode + "<br/>";
            horariosPartida += isLast ? itinerary.legs[0].hops[j].sTime : itinerary.legs[0].hops[j].sTime + "<br/>";
            destinos += isLast ? itinerary.legs[0].hops[j].tCode : itinerary.legs[0].hops[j].tCode + "<br/>";
            horariosChegadas += isLast ? itinerary.legs[0].hops[j].tTime : itinerary.legs[0].hops[j].tTime + "<br/>";
        };

        trOptions = ""
        radioOptions = "";

        // deixa selecionado segmento escolhido automaticamente
        checked = "";
        if (_postData.segment.chosenItinerary != null) {
            if (_postData.segment.chosenItinerary == i) {
                checked = "checked";
            }
        }

        //segmentos inva;lidos marcados em vermelho e indisponiveis para escolha
        if (!itinerary.validForSchedule) {
            trOptions = "style='color:red'";
            radioOptions = 'disabled';
            pMessage = "<p style='color:red'>Opções em vermelho não se encaixam no horário informado.</p>"
        }

        htmlResult += " <tr " + trOptions + ">";
        htmlResult += "     <td><input type='radio' name='rdEscolha' class='rdEscolha' value='" + i + "' " + checked + " " + radioOptions + "/></td>"; 
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

    $("#divAlternatives").dialog({
        autoOpen: false,
        width: 666, //hell yeah
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
        buttons: {
            "Confirmar escolha": function () {
                // troca a opção de voo
                confirmFlightOption();

                $(this).dialog("close");
            },
            Cancel: function () {
                cancelFlightOption();
                _chosenIt = _postData.segment.chosenItinerary;
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

    //datejs
    var date = Date.parse(_postData.segment.departureDateTime);
    $("#divAlternatives").dialog("option", "title", "Voos partindo dia " + date.toLocaleDateString());
    $("#divAlternatives").dialog("open");
    $(".rdEscolha").click(function (eventData) {
        onRdClick(eventData);
    });
});

//
//
//
function confirmFlightOption() {
    if (_chosenIt != null) {
        var chosenItin = _postData.segment.itineraries[_chosenIt];

        var chosenRoute = buildEmptyOuterFlightOption();
        chosenRoute.flightSegment.flightLegs.length = _postData.segment.itineraries[_chosenIt].legs[0].hops.length;
        chosenRoute.segmentIndex = _postData.segmentIndex;
        chosenRoute.routeIndex = _postData.routeIndex;
        chosenRoute.price = _postData.segment.itineraries[_chosenIt].legs[0].indicativePrice.price;
        chosenRoute.currency = 'BRL';

        var nextArrivalDate = Date.parse(_postData.segment.arrivalDateTime);

        for (var i = 0; i < _postData.segment.itineraries[_chosenIt].legs[0].hops.length; i++) {
            var hop = _postData.segment.itineraries[_chosenIt].legs[0].hops[i];
            var fLeg = buildEmptyOuterFlightLeg();

            var tempDate;
            if (i == 0) {
                tempDate = Date.parse(_postData.segment.departureDateTime.toString('yyyy-MM-ddTHH:mm:ss'));
            }

            tempDate.setHours(hop.sTime.substring(0, 2));
            tempDate.setMinutes(hop.sTime.substring(3, 5));
            fLeg.departureDate = tempDate.toString('yyyy-MM-ddTHH:mm:ss');

            if (hop.dayChange) {
                tempDate = tempDate.addDays(hop.dayChange);
            }
            tempDate.setHours(hop.tTime.substring(0, 2));
            tempDate.setMinutes(hop.tTime.substring(3, 5));
            fLeg.arrivalDate = tempDate.toString('yyyy-MM-ddTHH:mm:ss');

            fLeg.origin = hop.sCode;
            fLeg.destination = hop.tCode;
            fLeg.marketingAirline = hop.airline;
            fLeg.number = hop.flight;
            fLeg.duration = hop.duration;

            chosenRoute.flightSegment.flightLegs[i] = fLeg;
        };

        window.parent.postMessage(chosenRoute, "*");
        //window.parent.postMessage(_chosenIt + "|" + _postData.segmentIndex + "|" + _postData.routeIndex, "*");
    };
};

//
//
//
function buildEmptyOuterFlightOption() {
    var outerFlightSegment = {
        flightLegs: new Array()
    };
    var outerFlightOption = {
        flightSegment: outerFlightSegment,
        segmentIndex: 0,
        routeIndex: 0,
        price: 0,
        currency: ''
    };
    return outerFlightOption;
};

//
//
//
function buildEmptyOuterFlightLeg() {
    var outerFlightLeg = {
        origin: '',
        destination: '',
        number: '',
        marketingAirline: '',
        operatingAirline: '',
        departureDate: null,
        arrivalDate: null,
        fareClass: '',
        fareBasis: '',
        duration: 0,
        distance: 0
    };
    return outerFlightLeg;
};

//
//
//
function cancelFlightOption() {
        window.parent.postMessage('cancel', "*");
};

//
//
//
function onRdClick(eventData) {
    _chosenIt = eventData.target.value;
};

//
//
//
function getPostData() {
    var jsonString = $('#divJSONRqParams').text();
    var reqObj = { segment: null, nextArrDate: null, segmentIndex: null, routeIndex: null, legIndex: null };
    if (jsonString != null && jsonString != '') {
        reqObj = jQuery.parseJSON(jsonString);
        reqObj.nextArrDate = new Date(
                            reqObj.nextArrDate.substring(0, 4),
                            parseInt(reqObj .nextArrDate.substring(5, 7)) - 1,
                            reqObj.nextArrDate.substring(8, 10),
                            reqObj.nextArrDate.substring(11, 13),
                            reqObj.nextArrDate.substring(14, 16),
                            0, 0);
    };
    return reqObj;
};