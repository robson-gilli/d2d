using Door2DoorCore.Interfaces;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore
{
    internal class Door2DoorRome2Rio : Door2DoorBase, IDoor2DoorProvider
    {
        public Door2DoorRome2Rio(D2DRequest d2d)
            : base(d2d){}
        public Door2DoorRome2Rio(D2DRequest d2d, int maxWalkingMinutes, int flightAntecipation, int minutesAfterFlight)
            : base(d2d,maxWalkingMinutes, flightAntecipation, minutesAfterFlight) { }


        public Door2DoorResponse GetResponse(bool getNewResponse)
        {
            if (getNewResponse || _resp == null)
            {
                using (Rome2RioComm comm = new Rome2RioComm(_req))
                {
                    _resp = comm.Download();
                }
            }
            BuildCompleteItinerarySchedule();
            return _resp;
        }

        public Door2DoorResponse GetResponse()
        {
            if (_resp == null)
            {
                using (Rome2RioComm comm = new Rome2RioComm(_req))
                {
                    _resp = comm.Download();
                }
            }
            BuildCompleteItinerarySchedule();
            return _resp;
        }


        protected override void BuildCompleteItinerarySchedule()
        {
            for (int i = 0; i < _resp.LegResponse[0].Routes.Count(); i++) //ida
            {
                Route route = _resp.LegResponse[0].Routes[i];
                route.ValidForSchedule = true;
                bool flightChosen = false;

                if (_req.chosenRoute != null && _req.chosenRoute[0] != null && _req.chosenRoute[0].flightSegment != null)
                {
                    if (_req.chosenRoute[0].routeIndex == i)
                    {
                        flightChosen = true;
                    }
                }

                if (_req.outboundDateKind == D2dRequestTripDateKind.arriveAt)
                {
                    // adiciona informacoes de horario aos segmentos da rota atual
                    BuildItineraryScheduleArrivingAt(ref route, _req.desiredOutboundDate, flightChosen, 0);
                }
                else
                {
                    // adiciona informacoes de horario aos segmentos da rota atual
                    BuildItineraryScheduleDepartingAt(ref route, _req.desiredOutboundDate, flightChosen, 0);
                }
            }

            if (_req.desiredInboundDate.HasValue) //volta
            {
                for (int i = 0; i < _resp.LegResponse[1].Routes.Count(); i++)
                {
                    Route route = _resp.LegResponse[1].Routes[i];
                    bool flightChosen = false;

                    if (_req.chosenRoute != null && _req.chosenRoute.Length > 1 && _req.chosenRoute[1] != null && _req.chosenRoute[1].flightSegment != null)
                    {
                        if (_req.chosenRoute[1].routeIndex == i)
                        {
                            flightChosen = true;
                        }
                    }

                    if (_req.inboundDateKind == D2dRequestTripDateKind.arriveAt)
                    {
                        BuildItineraryScheduleArrivingAt(ref route, _req.desiredInboundDate.Value, flightChosen, 1);
                    }
                    else
                    {
                        BuildItineraryScheduleDepartingAt(ref route, _req.desiredInboundDate.Value, flightChosen, 1);
                    }
                    // Pode acontecer de determinada rota de retorno não ser válida por inicar antes da chegada ao destino,
                    // nestes casos a rota é devolvida normalmente, mas é marcada como invalida.
                    // Se o tipo de data informada na ida for a data de partida, a volta pode ou nao ser valida, dependendo da ida escolhida,
                    // nestes casos todas as voltas sao possivelmente validas
                    if ((_req.outboundDateKind == D2dRequestTripDateKind.departureAt) ||
                        (_req.outboundDateKind == D2dRequestTripDateKind.arriveAt && route.Segments[0].DepartureDateTime > _req.desiredOutboundDate))
                    {
                        route.ValidForSchedule = true;
                    }
                }
            }
        }

        private void BuildItineraryScheduleDepartingAt(ref Route route, DateTime departureDateTime, bool flightChosen, int legIndex)
        {
            if (route.Segments != null && route.Segments.Length > 0)
            {
                // resumo com totais de tempo e precos da rota atual
                route.RouteTotals = new RouteTotals();
                decimal routePrice = 0;
                int weeklyFrequency = 0;
                TimeSpan frequency = new TimeSpan(0, 0, 0);

                DateTime departureDateCurrent = new DateTime(departureDateTime.Ticks);
                route.Segments[0].DepartureDateTime = departureDateCurrent;

                for (int i = 0; i < route.Segments.Length; i++)
                {
                    Segment seg = route.Segments[i];

                    if (seg.Kind == "flight")
                    {
                        bool achouVoo = false;
                        ItineraryDates itineraryDates = new ItineraryDates();
                        seg.ChosenItinerary = null; //escolhido automaticamente

                        if (flightChosen) // é uma alteração de itinerario, um voo foi alterado
                        {
                            if (_req.chosenRoute[legIndex].segmentIndex == i)
                            {
                                // a opcao de voo escolhida tem que bater com o horario do schedule
                                if (_req.chosenRoute[legIndex].flightSegment.flightLegs[0].departureDate >= departureDateCurrent)
                                {
                                    achouVoo = true;

                                    BuildINewtinFromChosenRoute(ref seg, legIndex);

                                    seg.DepartureDateTime = _req.chosenRoute[legIndex].flightSegment.flightLegs[0].departureDate;
                                    seg.ArrivalDateTime = _req.chosenRoute[legIndex].flightSegment.flightLegs[_req.chosenRoute[legIndex].flightSegment.flightLegs.Length - 1].arrivalDate;

                                    TimeSpan nextDepTime = new TimeSpan(departureDateCurrent.Hour, departureDateCurrent.Minute, 0);
                                    for (int j = 0; j < seg.Itineraries.Length - 1; j++)
                                    {
                                        int hour = int.Parse(seg.Itineraries[j].Legs[0].Hops[seg.Itineraries[j].Legs[0].Hops.Length - 1].STime.Substring(0, 2));
                                        int min = int.Parse(seg.Itineraries[j].Legs[0].Hops[seg.Itineraries[j].Legs[0].Hops.Length - 1].STime.Substring(3, 2));

                                        TimeSpan hopArrivalTime = new TimeSpan(hour, min, 0);
                                        route.Segments[i].Itineraries[j].ValidForSchedule = hopArrivalTime >= nextDepTime;
                                    }
                                    departureDateCurrent = seg.ArrivalDateTime.Value.AddMinutes(_minutesAfterFlight);
                                }
                            }
                        }
                        else
                        {
                            DateTime originalDepDate = departureDateCurrent.AddMinutes(_flightAntecipation);//duas horas de antecedencia no aeroporto -> melhorar
                            DateTime lastDepDate = departureDateCurrent.AddMinutes(_flightAntecipation);//duas horas de antecedencia no aeroporto -> melhorar
                            for (int j = 0; j < seg.Itineraries.Length; j++)
                            {
                                Itinerary itinerary = seg.Itineraries[j];
                                Leg leg = itinerary.Legs[0];

                                // hora de partida do primeiro voo da opcao atual
                                string[] sTime = leg.Hops[0].STime.Split(':');
                                //hora de chegada do ultimo voo da opcao atual
                                string[] tTime = leg.Hops[leg.Hops.Length - 1].TTime.Split(':');
                                DateTime tempDepDate = new DateTime(originalDepDate.Year, originalDepDate.Month, originalDepDate.Day, int.Parse(sTime[0]), int.Parse(sTime[1]), 0);
                                itinerary.ValidForSchedule = tempDepDate >= originalDepDate; // valido para horario de chegada informado?
                                
                                if ((!achouVoo && tempDepDate >= originalDepDate) || (achouVoo && tempDepDate < lastDepDate && tempDepDate >= originalDepDate))
                                {
                                    achouVoo = true; // sei q pelo menos um voo é valido

                                    if (!seg.DepartureDateTime.HasValue || tempDepDate < seg.DepartureDateTime.Value)
                                    {
                                        lastDepDate = tempDepDate;
                                        seg.DepartureDateTime = tempDepDate;
                                        seg.ChosenItinerary = j;
                                        int dayChange = 0;
                                        for (int k = 0; k < leg.Hops.Length; k++)
                                        {
                                            if (leg.Hops[k].DayChange.HasValue)
                                            {
                                                dayChange += leg.Hops[k].DayChange.Value;
                                            }
                                        }
                                        DateTime tempArrDate = new DateTime(originalDepDate.Year, originalDepDate.Month, originalDepDate.Day, int.Parse(tTime[0]), int.Parse(tTime[1]), 0);
                                        //eu peguei um erro na api,
                                        //em raros casos ele coloca a data de chegada antes da de saida mas nao acrescenta valor ao dayChange :-s
                                        if (tempArrDate <= tempDepDate && dayChange == 0)
                                        {
                                            dayChange = 1;
                                        }
                                        tempArrDate = tempArrDate.AddDays(dayChange);
                                        seg.ArrivalDateTime = tempArrDate;
                                        departureDateCurrent = tempArrDate.AddMinutes(_minutesAfterFlight); // tempo para sair do aeroporto;
                                    }
                                }
                            }
                        }

                        if (!achouVoo)
                        {
                            DateTime originalDepDate = departureDateCurrent.AddMinutes(_flightAntecipation);
                            if (originalDepDate.Day == departureDateCurrent.Day)
                            {
                                originalDepDate = new DateTime(departureDateCurrent.Year, departureDateCurrent.Month, departureDateCurrent.Day + 1, 0, 0, 0);
                                //originalDepDate = departureDateCurrent.Date;
                            }
                            DateTime lastDepDate = originalDepDate.Date;
                            for (int j = 0; j < seg.Itineraries.Length; j++)
                            {
                                Itinerary itinerary = seg.Itineraries[j];
                                Leg leg = itinerary.Legs[0];

                                itinerary.ValidForSchedule = true; // valido para horario de chegada informado?

                                // hora de partida do primeiro voo da opcao atual
                                string[] sTime = leg.Hops[0].STime.Split(':');
                                //hora de chegada do ultimo voo da opcao atual
                                string[] tTime = leg.Hops[leg.Hops.Length - 1].TTime.Split(':');

                                DateTime tempDepDate = new DateTime(originalDepDate.Year, originalDepDate.Month, originalDepDate.Day, int.Parse(sTime[0]), int.Parse(sTime[1]), 0);
                                if (j == 0 || tempDepDate < lastDepDate)
                                {
                                    itinerary.ValidForSchedule = true;
                                    achouVoo = true; // sei q pelo menos um voo é valido
                                    lastDepDate = tempDepDate;
                                    seg.DepartureDateTime = tempDepDate;
                                    seg.ChosenItinerary = j;
                                    int dayChange = 0;
                                    for (int k = 0; k < leg.Hops.Length; k++)
                                    {
                                        if (leg.Hops[k].DayChange.HasValue)
                                        {
                                            dayChange += leg.Hops[k].DayChange.Value;
                                        }
                                    }
                                    DateTime tempArrDate = new DateTime(originalDepDate.Year, originalDepDate.Month, originalDepDate.Day, int.Parse(tTime[0]), int.Parse(tTime[1]), 0);
                                    //eu peguei um erro na api,
                                    //em raros casos ele coloca a data de chegada antes da de saida mas nao acrescenta valor ao dayChange :-s
                                    if (tempArrDate <= tempDepDate && dayChange == 0)
                                    {
                                        dayChange = 1;
                                    }
                                    tempArrDate = tempArrDate.AddDays(dayChange);
                                    seg.ArrivalDateTime = tempArrDate;
                                    departureDateCurrent = tempArrDate.AddMinutes(_minutesAfterFlight); // tempo para sair do aeroporto
                                }
                            }
                        }

                        // calcula o preco do segmento
                        if (seg.IndicativePrice != null)
                        {
                            if (seg.Itineraries[seg.ChosenItinerary.Value].Legs[0].IndicativePrice != null)
                            {
                                routePrice += seg.Itineraries[seg.ChosenItinerary.Value].Legs[0].IndicativePrice.Price;
                            }
                        }
                    }
                    else
                    {
                        //  A API tem um bug, as vezes manda caminhar, 50 minutos, as vezes mais de uma hora
                        //  Eles ficaram de resolver, mas até lá, eu desconstruí a conta de taxi e caminhada deles.
                        //  Se for caminhada e tiver mais que x minutos, eu transformo em Taxi ;)
                        if (seg.Kind == "walk" && seg.Duration > _maxWalkingMinutes && route.Segments.Length > 1)
                        {
                            WalkIntoTaxiTransformation(ref seg);
                        }

                        //faz o calculo do preco
                        if (seg.IndicativePrice != null)
                        {
                            routePrice += seg.IndicativePrice.Price;
                        }
                        //calcula frequencia semanal
                        weeklyFrequency = GetWeeklyFrequency(seg);
                        //calcula a frequencia total do segmento em horas e minutos
                        frequency = CalcFrequency(weeklyFrequency);

                        seg.Frequency = GetSegmentFrequency(seg);

                        //duracao total em minutos, somando frequencia
                        int duration = seg.Duration + frequency.Minutes + (frequency.Hours * 60);

                        //pega data do prox ponto (i + 1) ou do ponto de chegada
                        DateTime departureDate = departureDateCurrent;
                        departureDateCurrent = departureDateCurrent.AddMinutes(duration);
                        DateTime arrivalDate = departureDateCurrent;

                        seg.DepartureDateTime = departureDate;
                        seg.ArrivalDateTime = arrivalDate;
                    }

                    // calcula o resumo da rota atual
                    CalcRouteTotals(ref route, i, true);
                }
                route.IndicativePrice.Price = routePrice;
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
        private void BuildItineraryScheduleArrivingAt(ref Route route, DateTime arrivalDateTime, bool flightChosen, int legIndex)
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

                    //route.Segments[i].Subkind = _path;

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
                            for (int j = 0; j < route.Segments[i].Itineraries.Length; j++)
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

                                itineraryDates = CalcItineraryArrivalDates(latestItinerary, arrivalDateNextStop);
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
                        //  A API tem um bug, as vezes manda caminhar, 50 minutos, as vezes mais de uma hora.
                        //  Eles ficaram de resolver, mas até lá, eu desconstruí a conta de taxi e caminhada deles.
                        //  Se for caminhada e tiver mais que x minutos, eu transformo em Taxi ;)
                        if (route.Segments[i].Kind == "walk" && route.Segments[i].Duration > _maxWalkingMinutes && route.Segments.Length > 1)
                        {
                            WalkIntoTaxiTransformation(ref route.Segments[i]);
                        }
                        
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
                            anticipation = _flightAntecipation;
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
                CalcRouteTotals(ref route, i, false);
                }
                route.IndicativePrice.Price = routePrice;
            }
        }

        private void WalkIntoTaxiTransformation(ref Segment segment)
        {
            segment.Kind = "car";
            segment.Vehicle = "taxi";

            if (segment.IndicativePrice == null)
            {
                segment.IndicativePrice = new IndicativePrice2();
            }

            segment.IndicativePrice.Currency = "BRL";
            // cada minuto andando corresponde à R$0,371428571428 de taxi(abstraia, abstraia)
            segment.IndicativePrice.Price = Math.Round(segment.Duration * 0.371428571428M, 0);
            // cada minuto andando equivale(abstraia, veja bem) à 0.142857142857 minutos em um taxi
            segment.Duration =  Convert.ToInt32(Math.Ceiling(decimal.Parse(segment.Duration.ToString()) * 0.142857142857M));
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
        private void CalcRouteTotals(ref Route route, int segmentIndex, bool departingAt)
        {
            Segment segment = route.Segments[segmentIndex];

            route.RouteTotals.TotalDistance += segment.Distance;

            if (departingAt)
            {
                if (segmentIndex > 0) // não for o primeiro segmento
                {
                    if (route.Segments[segmentIndex - 1].ArrivalDateTime.HasValue && segment.DepartureDateTime.HasValue)
                    {
                        route.RouteTotals.TotalTimeWaiting = route.RouteTotals.TotalTimeWaiting.Add(segment.DepartureDateTime.Value - route.Segments[segmentIndex - 1].ArrivalDateTime.Value);
                    }
                }
            }
            else
            {
                if (segmentIndex < route.Segments.Length - 1) // não for o ultimo segmento
                {
                    if (route.Segments[segmentIndex + 1].DepartureDateTime.HasValue && segment.ArrivalDateTime.HasValue)
                    {
                        route.RouteTotals.TotalTimeWaiting = route.RouteTotals.TotalTimeWaiting.Add(route.Segments[segmentIndex + 1].DepartureDateTime.Value - segment.ArrivalDateTime.Value);
                    }
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
                        for (int j = 0; j < segment.Itineraries[i].Legs.Length; j++)
                            if (segment.Itineraries[i].Legs[j].Hops != null)
                                horasTotal += segment.Itineraries[i].Legs[j].Hops.Sum(e => e.Frequency);

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
        ///     The current <see cref="Door2DoorCore.Types.Door2DoorResponse.Segment"/>.
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
        private ItineraryDates CalcItineraryArrivalDates(Leg itinerary, DateTime arrivalDateNextStop)
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
    }
}
