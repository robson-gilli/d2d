﻿//
// Everybody hates global variables 
//
var _placeOrigem;
var _placeDestino;
var _dataChegada;
var _dataRetorno;

var _timeSelected;
var _timeReturnSelected;

var _panorama;
var _resp;
var _chosenRoute;
var _reqObj;//handles request policies

var _chosenLeg;
var autocompleteOrigem;
var autocompleteDestino;

//disable all clickable elements untill page is loaded
jQuery("#divSearchBox").find("input, select, button, textarea").attr("disabled", true);

//
// dom ready
//
$(document).ready(function(){
    _dataChegada = new Date();
    _dataRetorno = new Date();
    _timeSelected = false;
    _timeReturnSelected = false;
	_resp = null;
	_chosenRoute = new Array(2);
	_chosenRoute[0] = null;
	_chosenRoute[1] = null;
	_chosenLeg = null;

    // releasing controls.
	$("#divSearchBox").find("input, select, button, textarea").attr("disabled", false);

    // policies informed via POST from the previous page
	getRequestObject();

	if (_reqObj && !_reqObj.incPublicTransp) {
	    $('#chkIncludePublicTransport').attr('checked', false);
	    $('#chkIncludePublicTransport').attr('disabled', true);
	}

	google.maps.event.addDomListener(window, 'load', initializeGoogle);

	//configura calendario
	$("#datePicker").datepicker({
	    dateFormat: 'dd/mm/yy',
	    inline: true,
	    minDate: new Date(),
	    maxDate: new Date(2015, 12, 31),
	    useSelect: true,
	    onSelect: handleDatePicker,
	    minDate: _reqObj ? _reqObj.minDepDate : 0,
	    maxDate: _reqObj ? _reqObj.maxDepDate : "+1y"
	});
    //configura calendario
	$("#datePickerVolta").datepicker({
	    dateFormat: 'dd/mm/yy',
	    inline: true,
	    minDate: new Date(),
	    maxDate: new Date(2015, 12, 31),
	    useSelect: true,
	    onSelect: handleDatePickerVolta,
	    minDate: _reqObj ? _reqObj.minDepDate : 0,
	    maxDate: _reqObj ? _reqObj.maxDepDate : "+1y"
	});

	//configura campo de hora
	$('#timePicker').timepicker({
		onSelect: function(time, inst){
			_timeSelected = true;
		},
		onClose: handleTimePicker
	});
    //configura campo de hora
	$('#timePickerVolta').timepicker({
	    onSelect: function (time, inst) {
	        _timeReturnSelected = true;
	    },
	    onClose: handleTimePickerVolta
	});

	$('#datePickerVolta').css('visibility', 'hidden');
	$('#timePickerVolta').css('visibility', 'hidden');
	$('#rdVoltaDateKindDepartureAt').css('visibility', 'hidden');
	$('#rdVoltaDateKindArrivalAt').css('visibility', 'hidden');
	$('#lblVoltaDateKindDepartureAt').css('visibility', 'hidden');
	$('#lblVoltaDateKindArrivalAt').css('visibility', 'hidden');
	$('#btnChoseStay').css('visibility', 'hidden');

	


	$('#rdSomenteIda').click(function(){
	    $('#datePickerVolta').css('visibility', 'hidden');
	    $('#timePickerVolta').css('visibility', 'hidden');
	    $('#rdVoltaDateKindDepartureAt').css('visibility', 'hidden');
	    $('#rdVoltaDateKindArrivalAt').css('visibility', 'hidden');
	    $('#lblVoltaDateKindDepartureAt').css('visibility', 'hidden');
	    $('#lblVoltaDateKindArrivalAt').css('visibility', 'hidden');
	    $('#btnChoseStay').css('visibility', 'hidden');

	});

	$('#rdIdaeVolta').click(function () {
	    $('#datePickerVolta').css('visibility', 'visible');
	    $('#timePickerVolta').css('visibility', 'visible');
	    $('#rdVoltaDateKindDepartureAt').css('visibility', 'visible');
	    $('#rdVoltaDateKindArrivalAt').css('visibility', 'visible');
	    $('#lblVoltaDateKindDepartureAt').css('visibility', 'visible');
	    $('#lblVoltaDateKindArrivalAt').css('visibility', 'visible');
	    $('#btnChoseStay').css('visibility', 'visible');

	});

	//ajusta o tamanho da div do streetview
	applyMapContainerHeight();
});

//
//
//
function getRequestObject() {
    var jsonString = $('#divJSONRqParams').text();
    if (jsonString != null && jsonString != '') {
        _reqObj = jQuery.parseJSON(jsonString);
        _reqObj.minDepDate = new Date(_reqObj.minDepDate.substring(0, 4), parseInt(_reqObj.minDepDate.substring(5, 7)) - 1, _reqObj.minDepDate.substring(8, 10));
        _reqObj.maxDepDate = new Date(_reqObj.maxDepDate.substring(0, 4), parseInt(_reqObj.maxDepDate.substring(5, 7)) - 1, _reqObj.maxDepDate.substring(8, 10));
    };
};

//
// initialize jquery and other components
//
function initializeGoogle() {

    var mapOptions = { bounds: new google.maps.LatLngBounds() };
    if (_reqObj != null && _reqObj.allowInter == false) {
        mapOptions = {
            bounds: new google.maps.LatLngBounds(),
            componentRestrictions: {
                country: 'br'
            }
        };
    }
    autocompleteOrigem = new google.maps.places.Autocomplete(document.getElementById('txtOrigem'), mapOptions);
    autocompleteDestino = new google.maps.places.Autocomplete(document.getElementById('txtDestino'), mapOptions);

    //listener do autocomplete de origem
    google.maps.event.addListener(autocompleteOrigem, 'place_changed', function () {
        _placeOrigem = autocompleteOrigem.getPlace();
        if (!_placeOrigem.geometry) {
            return;
        };
        
        $("#divResults").hide();
        $("#mapa").show();
        setPanorama(_placeOrigem.geometry.location.lat(), _placeOrigem.geometry.location.lng());
    });

    //listener do autocomplete de destino
    google.maps.event.addListener(autocompleteDestino, 'place_changed', function () {
        _placeDestino = autocompleteDestino.getPlace();
        if (!_placeDestino.geometry) {
            return;
        };

        $("#divTabs").tabs();
        $("#divDetalhesItinerario").accordion({ heightStyle: "content", collapsible: true, active: false });
        $("#divDetalhesItinerarioVolta").accordion({ heightStyle: "content", collapsible: true, active: false });

        $("#divResults").hide();
        $("#mapa").show();
        setPanorama(_placeDestino.geometry.location.lat(), _placeDestino.geometry.location.lng());
    });

    //monta o mapa inicial com o 
    setPanorama(-23.5732882, -46.6416702);
};

//
//handles event triggered when a date is selected
//
function handleDatePicker(){
	var day = $("#datePicker").datepicker('getDate').getDate();
	var month = $("#datePicker").datepicker('getDate').getMonth();
	var year = $("#datePicker").datepicker('getDate').getFullYear();

	_dataChegada = new Date(year, month, day, _dataChegada.getHours(), _dataChegada.getMinutes());

	$("#datePickerVolta").datepicker('option', 'minDate', _dataChegada);
	$("#datePickerVolta").datepicker('option', 'setDate', _dataChegada);
};

//
//handles event triggered when a date is selected
//
function handleDatePickerVolta() {
    var day = $("#datePickerVolta").datepicker('getDate').getDate();
    var month = $("#datePickerVolta").datepicker('getDate').getMonth();
    var year = $("#datePickerVolta").datepicker('getDate').getFullYear();

    _dataRetorno = new Date(year, month, day, _dataRetorno.getHours(), _dataRetorno.getMinutes());
};

//
//handles event triggered when time is selected
//
function handleTimePicker(time, inst) {
	if (_timeSelected) {
		_dataChegada = new Date(_dataChegada.getFullYear(), _dataChegada.getMonth(), _dataChegada.getDate(), inst.hours, inst.minutes);
	};
	_timeSelected = false;
};

//
//handles event triggered when time is selected
//
function handleTimePickerVolta(time, inst) {
    if (_timeReturnSelected) {
        _dataRetorno = new Date(_dataRetorno.getFullYear(), _dataRetorno.getMonth(), _dataRetorno.getDate(), inst.hours, inst.minutes);
    };
    _timeReturnSelected = false;
};


//
//
//
function setPanorama(lat, lng) {

    // rua do paraiso, 148 - Sao Paulo / SP
    var office = new google.maps.LatLng(lat, lng);
    var mapOptions = {
        center: office,
        zoom: 14,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var map = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);

    var panoramaOptions = {
        position: office,
        pov: {
            heading: 34,
            pitch: 10,
            zoom: 1
        }
    };
    _panorama = new google.maps.StreetViewPanorama(document.getElementById("pano-canvas"), panoramaOptions);
    map.setStreetView(_panorama);

    google.maps.event.addListener(_panorama, 'position_changed', function () {
        handlePanoramaPosition_Changed();
    });
};

//
// recebe nova latitude e longitude sempre que a posicao é alterada no pama ou no streetview
//
function handlePanoramaPosition_Changed() {
    var positionCell = _panorama.getPosition();
    //$('#txtOrigem').val(_panorama.location.description);
    //autocomplete.set('place', void (0));

};

//
//ajusta o tamanho da div do streetview
//
$(window).resize(function () {
    applyMapContainerHeight();
});

//
// ajusta o tamanho da div do streetview 
//
function applyMapContainerHeight() {
    var height = $(window).height() - $("#divSearchBox").height() - 2;
    $("#mapa").height(height);
};