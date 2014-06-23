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

namespace Door2DoorCore
{
    /// <summary>
    /// 
    /// </summary>
    public class Door2Door : IDisposable
    {
        private D2DRequest _req;

        private List<Door2DoorResponse> _resp;
        public List<Door2DoorResponse> Resp
        {
            get { return _resp; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d2dReq"></param>
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
        /// 
        /// </summary>
        /// <param name="d2dReq"></param>
        /// <returns></returns>
        private bool RequestIsOK(D2DRequest d2dReq)
        {
            return d2dReq.flags != null &&
                    d2dReq.desiredArrivalDate != DateTime.MinValue && d2dReq.desiredArrivalDate != DateTime.MaxValue &&
                    d2dReq.oriLocation != null &&
                    d2dReq.destLocation != null;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Door2DoorResponse> GetResponse()
        {
            using (Rome2RioComm comm = new Rome2RioComm(_req))
            {
                _resp = comm.Download();
            }
            BuildCompleteItinerarySchedule();
            return _resp;
        }

        /// <summary>
        /// Builds a itinerary with schedules for each stop, based on the desired arrival date at the destination
        /// </summary>
        private void BuildCompleteItinerarySchedule()
        {
            for (int i = 0; i < _resp[0].Routes.Count(); i++)
            {
                Route route = _resp[0].Routes[i];
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

            if (_req.desiredReturnDate.HasValue) //ida e volta
            {
                for (int i = 0; i < _resp[1].Routes.Count(); i++)
                {
                    Route route = _resp[1].Routes[i];
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
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="dateTime"></param>
        /// <param name="vooEscolhido"></param>
        private void BuildItinerarySchedule(ref Route route, DateTime arrivalDateTime, bool vooEscolhido, int legIndex)
        {
            // diferenca entre a data da viagem e agora
            // irá ajudar no calculo do tempo para sair da origem
            int weeklyFrequency = 0;
            TimeSpan frequency = new TimeSpan(0, 0, 0);
            decimal routePrice = 0;
            if (route.Segments != null && route.Segments.Length > 0)
            {
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
                    if (route.Segments[i].Kind == "flight")
                    {
                        bool achouVoo = false;
                        ItineraryDates itineraryDates = new ItineraryDates();
                        route.Segments[i].ChosenItinerary = null; //escolhido automaticamente

                        if (vooEscolhido) // é uma alteração de itinerario, um voo foi alterado
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
                        }

                        if (!achouVoo)// Nao tinha nenhum voo que chegaria no horario naquele dia, pegar o voo com maior horario de chegada e determinar que é no dia anterior
                        { 
                            // acha o itinerario que tem a hora de chegada maior
                            Leg latestItinerary = FindLatestItinerary(ref route.Segments[i]);

                            itineraryDates = CalcItineraryDates(latestItinerary, arrivalDateNextStop);
                        }

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
                    else// qualquer tipo de transporte que nao seja voo
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
                }
            }
            route.IndicativePrice.Price = routePrice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="weeklyFrequency"></param>
        /// <returns></returns>
        private TimeSpan CalcFrequency(int weeklyFrequency)
        {
            int minutes = 0;
            int hours = 0;

            if (weeklyFrequency > 0)
            {
                const int DIAS_NA_SEMAMA = 7;
                const int HORAS_NA_SEMAMA = 168;
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
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        private TimeSpan? GetSegmentFrequency(Segment segment)
        {
            TimeSpan? freq = null;
            int horasTotal = 0;
            if (segment != null && segment.Itineraries != null && segment.Kind != "flight")
            {
                for (int i = 0; i < segment.Itineraries.Length; i++)
                {
                    if (segment.Itineraries[i].Legs != null)
                    {
                        for (int  j = 0; j < segment.Itineraries[i].Legs.Length; j++)
                        {
                            if (segment.Itineraries[i].Legs[j].Hops != null)
                            {
                                horasTotal = segment.Itineraries[i].Legs[j].Hops.Sum(e => e.Frequency);
                            }
                        }
                    }
                }
            }

            if (horasTotal > 0)
            {
                freq = CalcFrequency(horasTotal);
            }

            return freq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="itinerary"></param>
        /// <param name="arrivalDateNextStop"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="itinerary"></param>
        /// <param name="arrivalDateNextStop"></param>
        /// <returns></returns>
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
