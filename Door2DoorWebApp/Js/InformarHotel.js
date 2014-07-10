var _chosenStay = null;
var _autocompleteHotel;
function informarHotel() {
    if (_placeDestino) {
        $("#divChoseStay").dialog({
            autoOpen: false,
            width: 700, //hell yeah
            modal: true,
            closeOnEscape: true,
            open: autoCompleteHotel(),
            buttons: {
                "Escolher": function () {

                    hotelEscolhido();
                    $(this).dialog("close");
                },
                "Fechar": function () {
                    $(this).dialog("close");
                },
                "Cancelar": function () {
                    _chosenStay = null;
                    $('#txtHotel').val("");
                    $('#txtValorHotel').val("");
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
        $("#divChoseStay").dialog('open');
    } else {
        alert('Informe seu destino primeiro.');
    }
}

function autoCompleteHotel() {
    var options = {
        types: ['establishment']
    };

    _autocompleteHotel = new google.maps.places.Autocomplete(document.getElementById('txtHotel'), options);
}


function hotelEscolhido() {
    _chosenStay = {
        "location": {
            "lat": _autocompleteHotel.getPlace().geometry.location.lat(),
            "lng": _autocompleteHotel.getPlace().geometry.location.lng(),
            "type": "lodging"
        },
        totalPrice: parseInt($('#txtValorHotel').val()),
        completeStay: true,
        checkinDate: null,
        checkoutDate: null
    };

    var a = 0;

}