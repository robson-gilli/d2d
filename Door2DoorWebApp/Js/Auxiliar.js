//
//
//
String.prototype.padLeft = function (l, c) {
    return Array(l - this.length + 1).join(c || ' ') + this;
}

//
//
//
function dateToString(date) {
    var sDate = '';

    var dateTime = Date.parse(date);
    var day = "" + dateTime.getDate();
    var month = 1 + dateTime.getMonth();
    var hours = "" + dateTime.getHours();
    var minutes = "" + dateTime.getMinutes();

    //sDate = pad.substring(0, pad.length - day.length) + day + "/" +
    //    pad.substring(0, pad.length - month.length) + month + "/" +
    //    dateTime.getFullYear() + " " +
    //    pad.substring(0, pad.length - hours.length) + hours + ":" +
    //    pad.substring(0, pad.length - minutes.length) + minutes;

    //string prototype
    sDate = day.padLeft(2, '0') + "/" +
			month.toString().padLeft(2, '0') + "/" +
			dateTime.getFullYear() + " " +
			hours.padLeft(2, '0') + ":" +
			minutes.padLeft(2, '0');

    return sDate;
};

//
// retorna diferenca em dias, minutos e segundos.
//
function getTimeDiff(initialDate, finalDate) {
    var date = { days: 0, hours: 0, minutes: 0, seconds: 0 };

    if (finalDate < initialDate) {
        finalDate.setDate(finalDate.getDate() + 1);
    }
    var msec = finalDate - initialDate;//milliseconds

    var hh = Math.floor(msec / 1000 / 60 / 60);
    if (hh >= 24) { // cacula dias.
        var dd = Math.floor(hh / 24);
        date.days = dd;
        hh = hh - (dd * 24);
    }
    date.hours = hh;

    msec -= hh * 1000 * 60 * 60;
    var mm = Math.floor(msec / 1000 / 60);
    date.minutes = mm;

    msec -= mm * 1000 * 60;
    var ss = Math.floor(msec / 1000);

    msec -= ss * 1000;
    date.seconds = ss;

    return date;
};
