using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Door2DoorResponse;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption;
using Door2DoorCore.Exceptions;

/// <summary>
///     Provides Door2Door routes
/// </summary>
namespace Door2DoorCore
{
    /// <summary>
    ///     <para>
    ///     The Door2Door Class Library searches for itineraries from and to any location.
    ///     </para>
    ///     <para>
    ///     Just inform the coordinates for the origin and destination, the desired arrival date at the destination and a list of routes will be returned.
    ///     </para>
    ///     <para>
    ///     The routes may include several means of transportation, including walking, flights, taxis, cars, buses, trains, ferrys trams and more.
    ///     </para>
    ///     <para>
    ///     A desired return date can also be specified. In this case two sets of routes will be returned, one for the outbound and another for the inbound segment.
    ///     </para>
    ///     <para>
    ///     You can also filter wheter you want or not include public transportation on the response.
    ///     </para>
    ///     <para>
    ///     NOTE: All schedules and pricing are estimatives, It's impossible to predict with perfect accuracy the correct time walking or the price of the taxi, for instance.
    ///     </para>
    /// </summary>
    public class Door2Door : IDisposable
    {
        private D2DRequest _req;

        private Door2DoorResponse _resp;
        /// <summary>
        ///     Routes and schedules for the desired itinerary request. <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </summary>
        public Door2DoorResponse Resp
        {
            get { return _resp; }
        }
        
        /// <summary>
        ///     Constructor of the class. <see cref="Door2DoorCore.Door2Door"/>
        /// </summary>
        /// <param name="d2dReq">
        ///     <para><see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/></para>
        ///     Parameters for the request include coordinates of origin and destination,
        ///     arrival and return dates, external flight options and filters.
        /// </param>
        public Door2Door(D2DRequest d2dReq)
        {
            if (RequestIsOK(d2dReq))
            {
                _req = d2dReq;
            }
            else
            {
                throw new D2DRequestException("Please check your request.");
            }
        }

        /// <summary>
        ///     Verifies if all necessary data from the request are correctly informed
        /// </summary>
        /// <param name="d2dReq">
        ///     Route request. <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>
        /// </param>
        /// <returns>
        ///     Whether the request is ok or not
        /// </returns>
        private bool RequestIsOK(D2DRequest d2dReq)
        {
            return d2dReq.flags != null &&
                    d2dReq.desiredArrivalDate != DateTime.MinValue && d2dReq.desiredArrivalDate != DateTime.MaxValue &&
                    d2dReq.oriLocation != null &&
                    d2dReq.destLocation != null;
        }

        /// <summary>
        ///     <para>After instantiating the <see cref="Door2DoorCore.Door2Door"/> class, you should invoke this method.</para>
        ///     <para>It calculates all possible routes, itineraries and schedules for the given request.</para>
        /// </summary>
        /// <returns>
        ///     <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>. Complete itinerary with indicative pricing and schedules.
        /// </returns>
        public Door2DoorResponse GetResponse()
        {
            using (Rome2RioComm comm = new Rome2RioComm(_req))
            {
                _resp = comm.Download();
            }
            BuildCompleteItinerarySchedule();
            return _resp;
        }

        /// <summary>
        ///     Builds an itinerary with schedules for each stop, based on the desired arrival date at the destination
        /// </summary>
        private void BuildCompleteItinerarySchedule()
        {
            for (int i = 0; i < _resp.LegResponse[0].Routes.Count(); i++) //ida
            {
                Route route = _resp.LegResponse[0].Routes[i];
                route.ValidForSchedule = true;
                bool flightChosen = false;

                if (_req.chosenRoute != null && _req.chosenRoute[0].flightSegment != null)
                {
                    if (_req.chosenRoute[0].routeIndex == i)
                    {
                        flightChosen = true;
                    }
                }

                // adiciona informacoes de horario aos segmentos da rota atual
                BuildItinerarySchedule(ref route, _req.desiredArrivalDate, flightChosen, 0);
            }

            if (_req.desiredReturnDate.HasValue) //volta
            {
                for (int i = 0; i < _resp.LegResponse[1].Routes.Count(); i++)
                {
                    Route route = _resp.LegResponse[1].Routes[i];
                    bool flightChosen = false;

                    if (_req.chosenRoute != null && _req.chosenRoute.Length > 1 && _req.chosenRoute[1].flightSegment != null)
                    {
                        if (_req.chosenRoute[1].routeIndex == i)
                        {
                            flightChosen = true;
                        }
                    }

                    // adiciona informacoes de horario aos segmentos da rota atual
                    BuildItinerarySchedule(ref route, _req.desiredReturnDate.Value, flightChosen, 1);

                    // Pode acontecer de determinada rota de retorno não ser válida por inicar antes da chegada ao destino,
                    // nestes casos a rota é devolvida normalmente, mas é marcada como invalida.
                    if (route.Segments[0].DepartureDateTime > _req.desiredArrivalDate)
                    {
                        route.ValidForSchedule = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Completes the response with  schedule information.
        ///     This method also takes into account any external flight option that might have been informed in the request.
        /// </summary>
        /// <param name="route">
        ///     Route: <see cref="Door2DoorCore.Types.Door2DoorResponse.Route"/>
        /// </param>
        /// <param name="arrivalDateTime">
        ///     Desired arrival date at destination
        /// </param>
        /// <param name="flightChosen">
        ///     If an external flight was informed for this route. <see cref="Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption"/>
        /// </param>
        /// <param name="legIndex">
        ///     Whether this is the inbound(0) or outbound(1) leg of the trip
        /// </param>
        private void BuildItinerarySchedule(ref Route route, DateTime arrivalDateTime, bool flightChosen, int legIndex)
        {
            if (route.Segments != null && route.Segments.Length > 0)
            {
                // resumo com totais de tempo e precos da rota atual
                route.RouteTotals = new RouteTotals();

                // diferenca entre a data da viagem e agora
                // irá ajudar no calculo do tempo para sair da origem
                int weeklyFrequency = 0;
                TimeSpan frequency = new TimeSpan(0, 0, 0);
                decimal routePrice = 0;

                //adiciona data de chegada ao destino final
                route.Segments[route.Segments.Length - 1].ArrivalDateTime = null;
                route.Segments[route.Segments.Length - 1].DepartureDateTime = null;

                for (int i = route.Segments.Length - 1; i >= 0; i--)
                {
                    //pega data do prox ponto (i + 1) ou do ponto de chegada
                    DateTime arrivalDateNextStop = new DateTime();
                    arrivalDateNextStop = i == route.Segments.Length - 1 ? arrivalDateTime : route.Segments[i + 1].DepartureDateTime.Value;

                    // Se for voo procura um voo que possibilite chegada no horário
                    // no futuro devera pegar de uma fonte externa
/*voo*/             if (route.Segments[i].Kind == "flight")
                    {
                        bool achouVoo = false;
                        ItineraryDates itineraryDates = new ItineraryDates();
                        route.Segments[i].ChosenItinerary = null; //escolhido automaticamente

                        if (flightChosen) // é uma alteração de itinerario, um voo foi alterado
                        {
                            if (_req.chosenRoute[legIndex].segmentIndex == i)
                            {
                                // a opcao de voo escolhida tem que bater com o horario do schedule
                                if (_req.chosenRoute[legIndex].flightSegment.flightLegs[_req.chosenRoute[legIndex].flightSegment.flightLegs.Length - 1].arrivalDate <= arrivalDateNextStop)
                                {
                                    achouVoo = true;

                                    BuildINewtinFromChosenRoute(ref route.Segments[i], legIndex);

                                    itineraryDates.departureDateTime = _req.chosenRoute[legIndex].flightSegment.flightLegs[0].departureDate;
                                    itineraryDates.arrivalDateTime = _req.chosenRoute[legIndex].flightSegment.flightLegs[_req.chosenRoute[legIndex].flightSegment.flightLegs.Length - 1].arrivalDate;

                                    TimeSpan nextArrivalTime = new TimeSpan(arrivalDateNextStop.Hour, arrivalDateNextStop.Minute, 0);
                                    for (int j = 0; j < route.Segments[i].Itineraries.Length - 1; j++)
                                    {
                                        int hour = int.Parse(route.Segments[i].Itineraries[j].Legs[0].Hops[route.Segments[i].Itineraries[j].Legs[0].Hops.Length - 1].TTime.Substring(0, 2));
                                        int min = int.Parse(route.Segments[i].Itineraries[j].Legs[0].Hops[route.Segments[i].Itineraries[j].Legs[0].Hops.Length - 1].TTime.Substring(3, 2));

                                        TimeSpan hopArrivalTime = new TimeSpan(hour, min, 0);
                                        route.Segments[i].Itineraries[j].ValidForSchedule = hopArrivalTime <= nextArrivalTime;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // para cada opcao de voo
                            for (int  j = 0; j < route.Segments[i].Itineraries.Length; j++)
                            {
                                route.Segments[i].Itineraries[j].ValidForSchedule = false; // valido para horario de chegada informado?

                                Leg itinerary = route.Segments[i].Itineraries[j].Legs[0];
                                //verifica se itinerario atual bate com o horario do proximo segmento
                                ItineraryDates currentItineraryDates = MatchflightToSchedule(itinerary, arrivalDateNextStop);

                                // pode ter ou nao voo chegando no dia para chegar no horario
                                if (currentItineraryDates.arrivalDateTime != null)
                                {
                                    achouVoo = true;
                                    route.Segments[i].Itineraries[j].ValidForSchedule = true;// valido para horario de chegada informado?
                                    if (itineraryDates.arrivalDateTime == null)
                                    {
                                        itineraryDates = currentItineraryDates;
                                        route.Segments[i].ChosenItinerary = j;//indice do itinerario escolhido automatico
                                    }
                                    else
                                    {
                                        if (currentItineraryDates.arrivalDateTime > itineraryDates.arrivalDateTime)
                                        {
                                            itineraryDates = currentItineraryDates;
                                            route.Segments[i].ChosenItinerary = j; //indice do itinerario escolhido automatico
                                        }
                                    }
                                }
                            }

                            if (!achouVoo)// Nao tinha nenhum voo que chegaria no horario naquele dia, pegar o voo com maior horario de chegada e determinar que é no dia anterior
                            {
                                // acha o itinerario que tem a hora de chegada maior
                                Leg latestItinerary = FindLatestItinerary(ref route.Segments[i]);

                                itineraryDates = CalcItineraryDates(latestItinerary, arrivalDateNextStop);
                            }
                        }

                        // seta segmento com datas de chegada e partida
                        route.Segments[i].DepartureDateTime = itineraryDates.departureDateTime;
                        route.Segments[i].ArrivalDateTime = itineraryDates.arrivalDateTime;

                        // calcula o preco do segmento
                        if (route.Segments[i].IndicativePrice != null)
                        {
                            if (route.Segments[i].Itineraries[route.Segments[i].ChosenItinerary.Value].Legs[0].IndicativePrice != null)
                            {
                                routePrice += route.Segments[i].Itineraries[route.Segments[i].ChosenItinerary.Value].Legs[0].IndicativePrice.Price;
                            }
                        }
                    }
/*nao voo*/         else 
                    { 
                        //faz o calculo do preco
                        if (route.Segments[i].IndicativePrice != null)
                        {
                            routePrice += route.Segments[i].IndicativePrice.Price;
                        }

                        //calcula frequencia semanal
                        weeklyFrequency = GetWeeklyFrequency(route.Segments[i]);
                        //calcula a frequencia total do segmento em horas e minutos
                        frequency = CalcFrequency(weeklyFrequency);

                        route.Segments[i].Frequency = GetSegmentFrequency(route.Segments[i]);

                        //duracao total em minutos, somando frequencia
                        int duration = route.Segments[i].Duration + frequency.Minutes + (frequency.Hours * 60);

                        int anticipation = 0; // em minutos
                        // se o proximo segmento é voo, calcular chegada com antecipacao
                        if (i < route.Segments.Length - 1 && route.Segments[i + 1].Kind == "flight")
                        {
                            anticipation = 120;
                        }

                        //pega data do prox ponto (i + 1) ou do ponto de chegada
                        DateTime departureDate = arrivalDateNextStop.AddMinutes(-duration);
                        DateTime arrivalDate = departureDate.AddMinutes(duration);
                        departureDate = departureDate.AddMinutes(-anticipation);
                        arrivalDate = arrivalDate.AddMinutes(-anticipation);

                        route.Segments[i].DepartureDateTime = departureDate;
                        route.Segments[i].ArrivalDateTime = arrivalDate;
                    }

                    // calcula o resumo da rota atual
                    CalcRouteTotals(ref route, i);
                }
                route.IndicativePrice.Price = routePrice;
            }
        }

        /// <summary>
        ///     Total times and costs fo the informed segment
        /// </summary>
        /// <param name="route">
        ///      <see cref="Door2DoorCore.Types.Door2DoorResponse.Route"/>
        /// </param>
        /// <param name="segmentIndex">
        ///     Which segment of the informed route.
        /// </param>
        private void CalcRouteTotals(ref Route route, int segmentIndex)
        {
            Segment segment = route.Segments[segmentIndex];
            
            route.RouteTotals.TotalDistance += segment.Distance;

            if (segmentIndex < route.Segments.Length - 1) // não for o ultimo segmento
            {
                if (route.Segments[segmentIndex + 1].DepartureDateTime.HasValue && segment.ArrivalDateTime.HasValue)
                {
                    route.RouteTotals.TotalTimeWaiting = route.RouteTotals.TotalTimeWaiting.Add(route.Segments[segmentIndex + 1].DepartureDateTime.Value - segment.ArrivalDateTime.Value);
                }
            }

            switch (segment.Kind.ToLower())
            {
                case "flight":
                    if (segment.ChosenItinerary.HasValue)
                    {
                        route.RouteTotals.TotalPriceOfFlight += segment.Itineraries[segment.ChosenItinerary.Value].Legs[0].IndicativePrice.Price;
                        int totalDuration = segment.Itineraries[segment.ChosenItinerary.Value].Legs[0].Hops.Sum(e => e.Duration);
                        route.RouteTotals.TotalTimeOnFlight = route.RouteTotals.TotalTimeOnFlight.Add(new TimeSpan(0, totalDuration, 0));
                    }

                    break;
                case "bus":
                    if (segment.IndicativePrice != null)
                    {
                        route.RouteTotals.TotalPriceOfBus += segment.IndicativePrice.Price;
                    }
                    route.RouteTotals.TotalTimeOnBus = route.RouteTotals.TotalTimeOnBus.Add(new TimeSpan(0, segment.Duration, 0));

                    break;
                case "walk":
                    route.RouteTotals.TotalTimeOnWalk = route.RouteTotals.TotalTimeOnWalk.Add(new TimeSpan(0, segment.Duration, 0));

                    break;
                case "car":
                    if (segment.IndicativePrice != null)
                    {
                        route.RouteTotals.TotalPriceOfCar += segment.IndicativePrice.Price;
                    }
                    route.RouteTotals.TotalTimeOnCar = route.RouteTotals.TotalTimeOnCar.Add(new TimeSpan(0, segment.Duration, 0));

                    break;
                case "train":
                    if (segment.IndicativePrice != null)
                    {
                        route.RouteTotals.TotalPriceOfTrain += segment.IndicativePrice.Price;
                    }
                    route.RouteTotals.TotalTimeOnTrain = route.RouteTotals.TotalTimeOnTrain.Add(new TimeSpan(0, segment.Duration, 0));

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Builds a Itinerary <see cref="Door2DoorCore.Types.Door2DoorResponse.Itinerary"/> based on the D2DRequestChosenRoute on the request.
        /// </summary>
        /// <param name="segment">
        ///     Which segment the itinerary will be included
        /// </param>
        /// <param name="legIndex">
        ///     Whether this is the inbound(0) or outbound(1) leg of the trip
        /// </param>
        private void BuildINewtinFromChosenRoute(ref Segment segment, int legIndex)
        {
            Itinerary it = new Itinerary();
            it.Legs = new Leg[1];
            it.OuterItinerary = true;
            it.ValidForSchedule = true;
            it.Legs[0] = new Leg();
            it.Legs[0].Hops = new Hop[_req.chosenRoute[legIndex].flightSegment.flightLegs.Length];

            it.Legs[0].IndicativePrice = new IndicativePrice4();
            it.Legs[0].IndicativePrice.Price = _req.chosenRoute[legIndex].price;
            it.Legs[0].IndicativePrice.Currency = _req.chosenRoute[legIndex].currency;

            int duration = 0;
            for (int i = 0; i < _req.chosenRoute[legIndex].flightSegment.flightLegs.Length; i++)
            {
                Hop hop = new Hop();
                hop.SCode = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].origin;
                hop.TCode = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].destination;

                hop.STime = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].departureDate.ToString("HH:mm");
                hop.TTime = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].arrivalDate.ToString("HH:mm");

                hop.Flight = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].number;
                hop.Airline = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].marketingAirline;
                hop.Duration = _req.chosenRoute[legIndex].flightSegment.flightLegs[i].duration;

                hop.DayChange = (_req.chosenRoute[legIndex].flightSegment.flightLegs[i].arrivalDate - _req.chosenRoute[legIndex].flightSegment.flightLegs[i].departureDate).Days;

                duration += _req.chosenRoute[legIndex].flightSegment.flightLegs[i].duration;

                it.Legs[0].Hops[i] = hop;
            }
            segment.Duration = duration;

            Itinerary[] its = segment.Itineraries;
            Array.Resize(ref its, segment.Itineraries.Length + 1);
            segment.Itineraries = its;
            segment.Itineraries[segment.Itineraries.Length - 1] = it;
            segment.ChosenItinerary = segment.Itineraries.Length - 1;
        }

        /// <summary>
        ///     Calculetes a TimeSpan indicating a weekly frequency of a type of transportation.
        /// </summary>
        /// <param name="weeklyFrequency">
        ///     Estimated feequency per week. (How many times per week) 
        /// </param>
        /// <returns>
        ///     TimeSpan indicating the frequency. (How long untill the next transport to arrive)
        /// </returns>
        private TimeSpan CalcFrequency(int weeklyFrequency)
        {
            int minutes = 0;
            int hours = 0;

            if (weeklyFrequency > 0)
            {
                const int DIAS_NA_SEMAMA = 7; // 7 dias por semana
                const int HORAS_NA_SEMAMA = 168; //168 horas na semana
                decimal absHourFrequency = Math.Floor((decimal)(weeklyFrequency / HORAS_NA_SEMAMA));

                if (weeklyFrequency / DIAS_NA_SEMAMA > 24)// mais do que um a cada hora
                { 
                    minutes = int.Parse(Math.Floor((decimal)(60 / (weeklyFrequency / HORAS_NA_SEMAMA))).ToString());
                }
                else if (weeklyFrequency / DIAS_NA_SEMAMA < 24)// menos do que um a cada hora
                { 
                    decimal porDia = (decimal)weeklyFrequency / (decimal)DIAS_NA_SEMAMA;
                    decimal aCadaXHoras = Math.Floor((decimal)24 / (decimal)porDia);

                    hours = (int)aCadaXHoras;

                    if (aCadaXHoras != (24 / porDia))// nao é hora redonda
                    { 
                        decimal aCadaXMinutos = ((decimal)24 / (decimal)porDia) - aCadaXHoras;
                        decimal min = Math.Round(aCadaXMinutos * 60);
                        minutes = (int)min;
                    }
                }
                else//==1
                { 
                    hours = 1;
                }
            }
            return new TimeSpan(0, hours, minutes, 0, 0);
        }

        /// <summary>
        ///     Gets the total frequency (in minutes) of the segment
        /// </summary>
        /// <param name="segment">
        ///     Door2DoorCore.Types.Door2DoorResponse.Segment.
        /// </param>
        /// <returns>
        ///     Nullable TimeSpan indicating the total frequency. ('null' if the segment does not have a frequency attribute)
        /// </returns>
        private TimeSpan? GetSegmentFrequency(Segment segment)
        {
            TimeSpan? freq = null;
            int horasTotal = 0;
            if (segment != null && segment.Itineraries != null && segment.Kind != "flight")
                for (int i = 0; i < segment.Itineraries.Length; i++)
                    if (segment.Itineraries[i].Legs != null)
                        for (int  j = 0; j < segment.Itineraries[i].Legs.Length; j++)
                            if (segment.Itineraries[i].Legs[j].Hops != null)
                                horasTotal = segment.Itineraries[i].Legs[j].Hops.Sum(e => e.Frequency);

            if (horasTotal > 0)
                freq = CalcFrequency(horasTotal);

            return freq;
        }

        /// <summary>
        ///     Gets the weekly frequency (in minutes) of the segment. <see cref="CalcFrequency"/>
        /// </summary>
        /// <param name="segment">
        ///     Door2DoorCore.Types.Door2DoorResponse.Segment.
        /// </param>
        /// <returns>
        ///     int WeeklyFrequency
        /// </returns>
        private int GetWeeklyFrequency(Segment segment)
        {
            int weeklyFrequency = 0;
            if (segment.Itineraries != null && segment.Kind != "flight")
            {
                if (segment.Itineraries[0].Legs != null && segment.Itineraries[0].Legs.Length > 0)
                {
                    if (segment.Itineraries[0].Legs[0].Hops != null)
                    {
                        weeklyFrequency = segment.Itineraries[0].Legs[0].Hops.Sum(e => e.Frequency);
                    }
                }
            }
            return weeklyFrequency;
        }
    
        /// <summary>
        ///     Finds the Itinerary that arrives at the latest time.
        /// </summary>
        /// <param name="segment">
        ///     
        /// </param>
        /// <returns>
        ///     Leg (Itinerary)
        /// </returns>
        private Leg FindLatestItinerary(ref Segment segment)
        {
            Leg latestItinerary = null;
            for (int i = 0; i < segment.Itineraries.Length; i++)
            {
                Leg itinerary = segment.Itineraries[i].Legs[0];
                segment.Itineraries[i].ValidForSchedule = true;

                //hora de chegada do ultimo voo da opcao atual
                string[] tTime = itinerary.Hops[itinerary.Hops.Length - 1].TTime.Split(':');

                // horario do itinerario atual
                TimeSpan itineraryDate = new TimeSpan(int.Parse(tTime[0]), int.Parse(tTime[1]), 0);

                if (latestItinerary == null)// é o primeiro
                { 
                    latestItinerary = itinerary;
                    segment.ChosenItinerary = i;
                }
                else
                {
                    string[] latestItineraryTime = latestItinerary.Hops[latestItinerary.Hops.Length - 1].TTime.Split(':');
                    TimeSpan latestItineraryDate = new TimeSpan(int.Parse(latestItineraryTime[0]), int.Parse(latestItineraryTime[1]), 0);

                    if (latestItineraryDate < itineraryDate)// achou uma opcao mais tarde
                    {
                        latestItinerary = itinerary;
                        segment.ChosenItinerary = i;
                    }
                }
            }
            return latestItinerary;
        }

        /// <summary>
        ///     Tries to Match the given Itinerary to the desired Arrival Date at the destination.
        ///     Return an empty object if the itinerary does not match.
        /// </summary>
        /// <param name="itinerary">
        ///     Itinerary desired to be matched
        /// </param>
        /// <param name="arrivalDateNextStop">
        ///     DateTime to match
        /// </param>
        /// <returns>
        ///     <see cref="Door2DoorCore.Types.Door2DoorResponse"/> indicatin Arrival and Departure dates. Both 'null' if not valid.
        /// </returns>
        private ItineraryDates MatchflightToSchedule(Leg itinerary, DateTime arrivalDateNextStop)
        {
            ItineraryDates flightOption = new ItineraryDates();

            // hora de partida do primeiro voo da opcao atual
            string[] sTime = itinerary.Hops[0].STime.Split(':');
            //hora de chegada do ultimo voo da opcao atual
            string[] tTime = itinerary.Hops[itinerary.Hops.Length - 1].TTime.Split(':');

            DateTime tempArrivalDate = new DateTime(arrivalDateNextStop.Year, arrivalDateNextStop.Month, arrivalDateNextStop.Day, int.Parse(tTime[0]), int.Parse(tTime[1]), 0);

            if (tempArrivalDate <= arrivalDateNextStop)
            {
                //Achou um voo que chega antes do proximo segmento da viagem

                int dayChange = 0;
                for (int i = 0; i < itinerary.Hops.Length; i++)
                {
                    if (itinerary.Hops[i].DayChange.HasValue)
                    {
                        dayChange += itinerary.Hops[i].DayChange.Value;
                    }
                }

                DateTime departureDate = new DateTime(arrivalDateNextStop.Year, arrivalDateNextStop.Month, arrivalDateNextStop.Day, int.Parse(sTime[0]), int.Parse(sTime[1]), 0).AddDays(-dayChange);

                flightOption.departureDateTime = departureDate;
                flightOption.arrivalDateTime = tempArrivalDate;
            }
            return flightOption;
        }

        /// <summary>
        ///     Calculate departure and arrival dates of a valid itinerary
        /// </summary>
        /// <param name="itinerary">
        ///     Itinerary
        /// </param>
        /// <param name="arrivalDateNextStop">
        ///     Arrival date for the next itinerary
        /// </param>
        /// <returns>
        ///     Arrival and Departure dates
        /// </returns>
        private ItineraryDates CalcItineraryDates(Leg itinerary, DateTime arrivalDateNextStop)
        {
            ItineraryDates itinerarySchedule = new ItineraryDates();
            // hora de partida do primeiro voo da opcao atual
            string[] sTime = itinerary.Hops[0].STime.Split(':');
            //hora de chegada do ultimo voo da opcao atual
            string[] tTime = itinerary.Hops[itinerary.Hops.Length - 1].TTime.Split(':');

            int arrDayChange = 0;
            DateTime tempArrivalDate = new DateTime(arrivalDateNextStop.Year, arrivalDateNextStop.Month, arrivalDateNextStop.Day, int.Parse(tTime[0]), int.Parse(tTime[1]), 0);
            if (tempArrivalDate >= arrivalDateNextStop)//tem que ser no dia anterior
            {
                arrDayChange = 1;
            }

            int depDayChange = arrDayChange;
            for (int j = 0; j < itinerary.Hops.Length; j++)
            {
                if (itinerary.Hops[j].DayChange.HasValue)
                {
                    depDayChange += itinerary.Hops[j].DayChange.Value;
                }
            }

            tempArrivalDate = tempArrivalDate.AddDays(-arrDayChange);

            itinerarySchedule.arrivalDateTime = tempArrivalDate;
            itinerarySchedule.departureDateTime = new DateTime(arrivalDateNextStop.Year, arrivalDateNextStop.Month, arrivalDateNextStop.Day, int.Parse(sTime[0]), int.Parse(sTime[1]), 0).AddDays(-depDayChange);

            return itinerarySchedule;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _req = null;
            _resp = null;
        }
    }
}
