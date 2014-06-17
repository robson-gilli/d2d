/******************************************* Door2DoorSender *******************************************
- Door2DoorSender:  Communicates with server to get a list of possible routes.
    - requestOptions: General request data
********************************************************************************************************/
function Door2DoorSender(requestOptions) {

    // atributos a serem passados para a api do rome2rio
    var _reqAtt = {
        url: '/api/getd2d',
        data: {
            desiredArrivalDate: requestOptions.desiredArrivalDate,
            flags: {
                includePublicTransp: buildSearchRequestFlags(requestOptions)
            },
            oriLocation: {
                lat: requestOptions.oriLat,
                lng: requestOptions.oriLng,
                type: requestOptions.oriType
            },
            destLocation: {
                lat: requestOptions.destLat,
                lng: requestOptions.destLng,
                type: requestOptions.destType
            },
            chosenRoute: {
                itineraryIndex: requestOptions.chosenRoute == null ? null : requestOptions.chosenRoute[0],
                segmentIndex: requestOptions.chosenRoute == null ? null : requestOptions.chosenRoute[1],
                routeIndex: requestOptions.chosenRoute == null ? null : requestOptions.chosenRoute[2]
            }
        },
        // code to run if the request succeeds;
        // the response is passed to the function
        success: requestOptions.successCallback,
        // code to run if the request fails; the raw request and
        // status codes are passed to the function
        error: requestOptions.errorCallback,
        // code to run regardless of success or failure
        // we don't need this, but what the hell
        complete: requestOptions.completeCallback
    };

    //
    //
    //
    function buildSearchRequestFlags(requestOptions) {
        return requestOptions.includePublicTransport;
    };

    //
    //
    //
    var requestOK = function () {
        return _reqAtt.data &&
                _reqAtt.data.flags != null &&
                _reqAtt.data.oriLocation &&
                _reqAtt.data.destLocation &&
                _reqAtt.data.oriLocation.lat &&
                _reqAtt.data.oriLocation.lng &&
                _reqAtt.data.oriLocation.type &&
                _reqAtt.data.destLocation.lat &&
                _reqAtt.data.destLocation.lng &&
                _reqAtt.data.destLocation.type &&
                _reqAtt.success &&
                _reqAtt.error &&
                _reqAtt.complete;
    };

    //
    // Malandragem. Possibilita chamada de callback assincrona passando variavais do escopo do caller(reqAtt) via encapsulamento
    // Credits to http://www.stefanolocati.it/blog/?p=1413
    var generateSuccessHandler = function (reqAtt) {
        return function (resp) {
            // call consumer callback
            reqAtt.success(resp);
        };
    };

    //
    //
    //
    this.getd2d = function () {
        if (requestOK()) {
            //
            // Using the core $.ajax() method
            //
            $.ajax({
                url: _reqAtt.url,
                data: _reqAtt.data,
                type: 'POST',
                dataType: 'json',
                success: generateSuccessHandler(_reqAtt),
                error: _reqAtt.error,
                complete: _reqAtt.complete
            });
        }
        else {
            _reqAtt.error(null, "request_error", "Something wrong with the request");
        };
    };
};
/********************************************************************************************************
******************************************* /Door2DoorSender ********************************************
*********************************************************************************************************/
