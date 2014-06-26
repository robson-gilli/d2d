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
    var leftMsec = finalDate - initialDate;//milliseconds

    var dd = Math.floor(leftMsec / 24 / 60 / 60 / 1000);// total de dias
    date.days = dd;
    leftMsec -= dd * 24 * 60 * 60 * 1000;

    var hh = Math.floor(leftMsec / 60 / 60 / 1000);//restante em horas
    date.hours = hh;
    leftMsec -= hh * 60 * 60 * 1000;

    var mm = Math.floor(leftMsec / 60 / 1000);//restante em horas
    date.minutes = mm;
    leftMsec -= mm * 60 * 1000;

    var ss = Math.floor(leftMsec / 1000);//restante em horas
    date.seconds = ss;
 
    return date;
};
