<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Door2DoorWebApp.Default"  Async="true"%>

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
                <input id="txtOrigem" class="controleOrigem" type="text" placeholder="Saindo de:" />
                <input id="txtDestino" class="controleOrigem" type="text" placeholder="Para:" />
                <input id="btnBuscar" type="button" onclick="buscar(true);" value="Buscar" style="height: 32px;" />
                <input type="radio" name="rdIdaVolta" id="rdSomenteIda" value="SomenteIda"  checked/>Somente Ida
                <input type="radio" name="rdIdaVolta" id="rdIdaeVolta" value="IdaeVolta"  />Ida e Volta
                <p>
                    <input type="text" id="datePicker" placeholder="Data:" class="dataChegada" readonly='readonly'/>
                    <input type="text" id="timePicker" placeholder="Hora:" class="horaChegada" readonly="readonly"/>

                    <input type="text" id="datePickerVolta" placeholder="Data:" class="dataChegada" readonly='readonly' style="visibility:hidden"/>
                    <input type="text" id="timePickerVolta" placeholder="Hora:" class="horaChegada" readonly="readonly" style="visibility:hidden"/>
                    <label><input type="checkbox" id="chkIncludePublicTransport" />Incluir transporte público</label>
                </p>
                <p>
                    <label id="lblIdaDateKindDepartureAt"><input type="radio" id="rdIdaDateKindDepartureAt" name="rdIdaDateKind" value="0"  />Horário de saída</label>
                    <label id="lblIdaDateKindArriveAt"><input type="radio" id="rdIdaDateKindArriveAt" name="rdIdaDateKind" value="1"  checked/>Horário de chegada&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                    <label id="lblVoltaDateKindDepartureAt" style="visibility:hidden"><input type="radio" id="rdVoltaDateKindDepartureAt"name="rdVoltaDateKind" value="0"  checked style="visibility:hidden"/>Horário de saída</label>
                    <label id="lblVoltaDateKindArrivalAt" style="visibility:hidden"><input type="radio" id="rdVoltaDateKindArrivalAt"name="rdVoltaDateKind" value="1"  style="visibility:hidden"/>Horário de chegada</label>
                    <input type="button" id="btnChoseStay" onclick="informarHotel();" value="+ Hotel" style="height: 32px;visibility:hidden;"/>
                </p>
            </div>

            <div id="mapa" style="width: 100%;">
                <div id="map-canvas" style="width: 50%; height: 100%; float: left"></div>
                <div id="pano-canvas" style="width: 50%; height: 100%; float: right"></div>
            </div>

            <div style="width: 100%; display: none" id="divResults">
                <div id="divTabs" style="width: 99%; height: 500px; float: left;">
                    <ul>
                        <li><a href="#divDetalhesItinerario">Ida</a></li>
                        <li><a href="#divDetalhesItinerarioVolta">Volta</a></li>
                        <li><a href="#divTotais">Totais</a></li>
                     </ul>
                    <div id="divDetalhesItinerario" style="width: 95%; height: 498px; float: left;">Please chose your itinerary above.</div>
                    <div id="divDetalhesItinerarioVolta" style="width: 95%; height: 498px; float: left;">Please chose your itinerary above.</div>
                    <div id="divTotais" style="width: 95%; height: 498px; float: left;">Please chose your itinerary above.</div>
                </div>
            </div>

            <div style="display: none" id="divFlightOptionsAlternatives" title=""></div>
            <div style="display: none" id="divChoseStay" title="Informe qual o seu hotel e o preço da estadia.">
                <input id="txtHotel" class="controleOrigem" type="text" placeholder="Digite seu hotel:" />
                <input id="txtValorHotel" class="controleOrigem" type="text" placeholder="Custo da estadia:" />

            </div>
        </div>

        <form id="frmChangeItin" name="frmChangeItin" method="post" style="display:none" target="iFrameChangeItin">
            <input type="hidden" name="r2r_resp" id="hidr2r_resp" />
            <input type="hidden" name="arrdate" id="hidarrdate" />
            <input type="hidden" name="segmentIndex" id="hisegmentIndex" />
            <input type="hidden" name="routeIndex" id="hidrouteIndex" />
            <input type="hidden" name="legIndex" id="hidlegIndex" />
        </form>

        <%-- This div stores json with the POST submited to the current page --%>
        <div id='divJSONRqParams' style="display:none"><asp:Literal ID='litJsonRq' runat='server'/></div>
        <%-- This form will be populated to post results to the net page --%>
        <form id="form1" method="post" style="display:none">
            <input type="hidden" name="chosenItin" id="hidchosenItin" />
        </form>

        <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places"></script>
        <script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>
        <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"></script>

        <script src="Js/TimePicker/jquery.ui.timepicker.js"></script>
        <script src="Js/Date/date.js"></script>
        <script src="Js/Initialize.js"></script>
        <script src="Js/Door2Door.js"></script>
        <script src="Js/Auxiliar.js"></script>
        <script src="Js/Busca.js"></script>
        <script src="Js/InformarHotel.js"></script>
        <script src="Js/Messaging.js"></script>
    </body>
</html>