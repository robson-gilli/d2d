﻿//
// Everybody hates global variables 
//
var _placeOrigem;
var _placeDestino;
var _dataChegada;
var _timeSelected;
var _panorama;
var _d2d;

//
// document ready
//
$(document).ready(function(){

	_dataChegada = new Date();
	_timeSelected = false;
	_d2d = null;

	google.maps.event.addDomListener(window, 'load', initialize);

	//configura calendario
	$("#datePicker").datepicker({
		dateFormat: 'dd/mm/yy',
		inline: true,
		minDate: new Date(),
		maxDate: new Date(2015, 12, 31),
		useSelect: true,
		onSelect: handleDatePicker
	});

	//configura campo de hora
	$('#timePicker').timepicker({
		onSelect: function(time, inst){
			_timeSelected = true;
		},
		onClose: handleTimePicker
	});

	//ajusta o tamanho da div do streetview
	applyMapContainerHeight();
});

//
// initialize jquery and other components
//
function initialize(){
    var autocompleteOrigem = new google.maps.places.Autocomplete(document.getElementById('txtOrigem'));
    var autocompleteDestino = new google.maps.places.Autocomplete(document.getElementById('txtDestino'));

    //listener do autocomplete de origem
    google.maps.event.addListener(autocompleteOrigem, 'place_changed', function () {
        _placeOrigem = autocompleteOrigem.getPlace();
        if (!_placeOrigem.geometry) {
            return;
        };
        
        $("#divResults").hide();
        //$("#divDetalhesItinerario").hide();
        //$("#map-results").hide();
        $("#mapa").show();
        setPanorama(_placeOrigem.geometry.location.k, _placeOrigem.geometry.location.A);
    });

    //listener do autocomplete de destino
    google.maps.event.addListener(autocompleteDestino, 'place_changed', function () {
        _placeDestino = autocompleteDestino.getPlace();
        if (!_placeDestino.geometry) {
            return;
        };


        $("#divDetalhesItinerario").accordion({ heightStyle: "content", collapsible: true, active: false });
        $("#divResults").hide();
        //$("#divDetalhesItinerario").hide();
        //$("#map-results").hide();
        $("#mapa").show();
        setPanorama(_placeDestino.geometry.location.k, _placeDestino.geometry.location.A);
    });

    //monta o mapa inicial
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