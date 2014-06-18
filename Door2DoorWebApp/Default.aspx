<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Door2DoorWebApp.Default"  %>

<!DOCTYPE html>
<html>
<head>
    <title>Door2Door as never before</title>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no">
    <meta charset="utf-8">

    <link href="Css/Style.css" rel="Stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css" />
</head>
<body>
    <div style="width: 100%; height: 100%;">
        <div style="background-color: gray; padding-bottom: 1px;" id="divSearchBox">
            <input id="txtOrigem" class="controleOrigem" type="text" placeholder="De onde você pretende sair?" />
            <input id="txtDestino" class="controleOrigem" type="text" placeholder="Para onde você vai?" />
            <input id="btnBuscar" type="button" onclick="buscar(true);" value="Buscar" style="height: 32px;" />
            <p>
                <input type="text" id="datePicker" placeholder="Que dia você tem que estar lá?" class="dataChegada" readonly='readonly'/>
                <input type="text" id="timePicker" placeholder="E que horas?" class="horaChegada" readonly="readonly"/>
                <label>
                    <input type="checkbox" id="chkIncludePublicTransport" />Incluir ônibus intermunicipal e transporte público</label>
            </p>
            <p />
        </div>

        <div id="mapa" style="width: 100%;">
            <div id="map-canvas" style="width: 50%; height: 100%; float: left"></div>
            <div id="pano-canvas" style="width: 50%; height: 100%; float: right"></div>
        </div>

        <div style="width: 100%; display: none" id="divResults">
            <div id="accordion-resizer" class="ui-widget-content">
                <div id="divDetalhesItinerario" style="width: 100%; height: 498px; float: left;"></div>
            </div>
        </div>
        <form id="frmChangeItin" name="frmChangeItin" method="post" style="display:none" target="iFrameChangeItin">
            <input type="hidden" name="r2r_resp" id="hidr2r_resp" />
            <input type="hidden" name="arrdate" id="hidarrdate" />
            <input type="hidden" name="segmentIndex" id="hisegmentIndex" />
            <input type="hidden" name="routeIndex" id="hidrouteIndex" />
        </form>

        <div style="display: none" id="divFlightOptionsAlternatives" title="">
    </div>
    <%-- This div stores json with the POST submited to the current page --%>
    <div id='divJSONRqParams' style="display:none"><asp:Literal ID='litJsonRq' runat='server'/></div>
    <%-- This form will be populated to post results to the net page --%>
    <form id="form1" method="post" style="display:none">
        <input type="hidden" name="chosenItin" id="hidchosenItin" />
    </form>
    
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places"></script>
    <script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"></script>
    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>

    <script src="Js/TimePicker/jquery.ui.timepicker.js"></script>
    <script src="Js/Date/date.js"></script>
    <script src="Js/Initialize.js"></script>
    <script src="Js/Door2Door.js"></script>
    <script src="Js/Auxiliar.js"></script>
    <script src="Js/Busca.js"></script>
    <script src="Js/Messaging.js"></script>
</body>
</html>