// Here "addEventListener" is for standards-compliant web browsers and "attachEvent" is for IE Browsers.
var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
var eventer = window[eventMethod];

// Now...
// if 
//    "attachEvent", then we need to select "onmessage" as the event. 
// if 
//    "addEventListener", then we need to select "message" as the event

var messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

// Listen to message from child IFrame window
eventer(messageEvent, function (e) {
//    if (e.origin == 'http://localhost:55105') { // for security
        $("#divFlightOptionsAlternatives").dialog("close");
        if (e.data != 'cancel') {
            _chosenRoute[_chosenLeg] = e.data;
            buscar(false);
        }
//    }

}, false);