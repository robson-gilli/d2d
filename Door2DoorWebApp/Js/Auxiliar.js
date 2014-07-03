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

//
// soma C# timespans que tenha formato d.HH:mm:ss e devolve no mesmo formato
//
function sumTimeSpan(times) {
    var days = 0;
    var hours = 0;
    var minutes = 0;
    var seconds = 0;

    for (var i = 0; i < times.length; i++) {
        var pointIndex = times[i].indexOf('.') == -1 ? 0 : times[i].indexOf('.') + 1;

        var h = parseInt(times[i].substring(pointIndex, pointIndex + 2));
        var m = parseInt(times[i].substring(pointIndex + 3, pointIndex + 5));
        var s = parseInt(times[i].substring(pointIndex + 6, pointIndex + 8));
        var d = pointIndex == 0 ? 0 : parseInt(times[i].substring(0, pointIndex - 1));

        var rest = 0;
        var sumM = 0;
        var sumH = 0;
        var sumD = 0;
        var sumS = seconds + s;
        if (sumS >= 60) {
            rest = sumS;
            while (rest >= 60) {
                sumM += 1;
                sumS -= 60;
                if ((sumS - 60) < 60)
                    break;
                rest = sumS - 60;
            }
        }
        seconds = sumS;

        sumM += minutes + m;
        if (sumM >= 60) {
            rest = sumM;
            while (rest >= 60) {
                sumH += 1;
                sumM -= 60;
                if ((sumM - 60) < 60)
                    break;
                rest = sumM - 60;
            }
        }
        minutes = sumM;

        sumH += hours + h;
        if (sumH >= 24) {
            rest = sumH;
            while (rest >= 24) {
                sumD += 1;
                sumH -= 24;
                if ((sumH - 24) < 24)
                    break;
                rest = sumH - 24;
            }
        }
        hours = sumH;
        days += sumD + d;
    }
    
    return (days == 0 ? '' : days + '.') + hours.toString().padLeft(2, '0') + ':' + minutes.toString().padLeft(2, '0') + ':' + seconds.toString().padLeft(2, '0');

}