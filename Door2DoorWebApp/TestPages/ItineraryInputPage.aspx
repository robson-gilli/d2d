<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItineraryInputPage.aspx.cs" Inherits="Door2DoorWebApp.TestPages.ItineraryInputPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css" />
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places"></script>
    <script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"></script>
    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>

    <script src="/Js/Auxiliar.js"></script>
    <script src="/Js/Date/date.js"></script>
    <script src="TestJs/main.js"></script>

</head>
<body>

    <div style="display: none" id="divAlternatives" title="">
        <div id="divInvalidFlightOptions"></div>
        <table id="tdOpcoesVoo" class="ui-widget ui-widget-content">
            <thead>
                <tr class="ui-widget-header ">
                    <th>Escolha</th>
                    <th>Numero</th>
                    <th>Cia</th>
                    <th>Origem</th>
                    <th>Partida</th>
                    <th>Destino</th>
                    <th>Chegada</th>
                    <th>Preço</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>


    <%-- This div stores json with the POST submited to the current page --%>
    <div id='divJSONRqParams' style="display:none"><asp:Literal ID='litJsonRq' runat='server'/></div>

</body>
</html>
