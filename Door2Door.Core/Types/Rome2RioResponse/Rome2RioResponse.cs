﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Door2DoorCore.Types;

namespace Door2DoorCore.Types.Rome2RioResponse
{

    public class Airport
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pos")]
        public string Pos { get; set; }
    }

    public class Airline
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("iconSize")]
        public string IconSize { get; set; }

        [JsonProperty("iconOffset")]
        public string IconOffset { get; set; }
    }

    public class Aircraft
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }
    }

    public class Agency
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("iconSize")]
        public string IconSize { get; set; }

        [JsonProperty("iconOffset")]
        public string IconOffset { get; set; }
    }

    public class IndicativePrice
    {

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isFreeTransfer")]
        public int IsFreeTransfer { get; set; }
    }

    public class Stop
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pos")]
        public string Pos { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class IndicativePrice2
    {

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isFreeTransfer")]
        public int IsFreeTransfer { get; set; }

        [JsonProperty("nativePrice")]
        public decimal NativePrice { get; set; }

        [JsonProperty("nativeCurrency")]
        public string NativeCurrency { get; set; }
    }

    public class IndicativePrice3
    {

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isFreeTransfer")]
        public int IsFreeTransfer { get; set; }

        [JsonProperty("nativePrice")]
        public decimal NativePrice { get; set; }

        [JsonProperty("nativeCurrency")]
        public string NativeCurrency { get; set; }
    }

    public class Line
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("agency")]
        public string Agency { get; set; }

        [JsonProperty("frequency")]
        public int Frequency { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("days")]
        public int Days { get; set; }
    }

    public class Codeshare
    {

        [JsonProperty("airline")]
        public string Airline { get; set; }
    }

    public class Hop
    {

        [JsonProperty("sName")]
        public string SName { get; set; }

        [JsonProperty("sPos")]
        public string SPos { get; set; }

        [JsonProperty("tName")]
        public string TName { get; set; }

        [JsonProperty("tPos")]
        public string TPos { get; set; }

        [JsonProperty("frequency")]
        public int Frequency { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("indicativePrice")]
        public IndicativePrice3 IndicativePrice { get; set; }

        [JsonProperty("lines")]
        public Line[] Lines { get; set; }

        [JsonProperty("sCode")]
        public string SCode { get; set; }

        [JsonProperty("tCode")]
        public string TCode { get; set; }

        [JsonProperty("sTime")]
        public string STime { get; set; }

        [JsonProperty("tTime")]
        public string TTime { get; set; }

        [JsonProperty("flight")]
        public string Flight { get; set; }

        [JsonProperty("airline")]
        public string Airline { get; set; }

        [JsonProperty("aircraft")]
        public string Aircraft { get; set; }

        [JsonProperty("sTerminal")]
        public string STerminal { get; set; }

        [JsonProperty("codeshares")]
        public Codeshare[] Codeshares { get; set; }

        [JsonProperty("dayChange")]
        public int? DayChange { get; set; }

    }

    public class IndicativePrice4
    {

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isFreeTransfer")]
        public int IsFreeTransfer { get; set; }
    }

    public class Leg
    {

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("hops")]
        public Hop[] Hops { get; set; }

        [JsonProperty("days")]
        public int? Days { get; set; }

        [JsonProperty("indicativePrice")]
        public IndicativePrice4 IndicativePrice { get; set; }
    }

    public class Itinerary
    {

        [JsonProperty("legs")]
        public Leg[] Legs { get; set; }

        [JsonProperty("isHidden")]
        public int? IsHidden { get; set; }

        [JsonProperty("validForSchedule")]
        public bool ValidForSchedule { get; set; }

        [JsonProperty("outerItinerary")]
        public bool OuterItinerary { get; set; }
    }

    public class Segment
    {

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("isMajor")]
        public int IsMajor { get; set; }

        [JsonProperty("isImperial")]
        public int IsImperial { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("sName")]
        public string SName { get; set; }

        [JsonProperty("sPos")]
        public string SPos { get; set; }

        [JsonProperty("tName")]
        public string TName { get; set; }

        [JsonProperty("tPos")]
        public string TPos { get; set; }

        [JsonProperty("vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("indicativePrice")]
        public IndicativePrice2 IndicativePrice { get; set; }

        [JsonProperty("subkind")]
        public string Subkind { get; set; }

        [JsonProperty("sCode")]
        public string SCode { get; set; }

        [JsonProperty("tCode")]
        public string TCode { get; set; }

        [JsonProperty("itineraries")]
        public Itinerary[] Itineraries { get; set; }

        //manually inserted
        [JsonProperty("arrivalDateTime")]
        public DateTime? ArrivalDateTime { get; set; }
        [JsonProperty("departureDateTime")]
        public DateTime? DepartureDateTime { get; set; }
        [JsonProperty("chosenItinerary")]
        public int? ChosenItinerary { get; set; }
        [JsonProperty("frequency")]
        public TimeSpan? Frequency { get; set; }
    }

    public class Route
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("indicativePrice")]
        public IndicativePrice IndicativePrice { get; set; }

        [JsonProperty("stops")]
        public Stop[] Stops { get; set; }

        [JsonProperty("segments")]
        public Segment[] Segments { get; set; }
    }

}

namespace Door2DoorCore.Types.Rome2RioResponse
{

    public class Rome2RioResponse
    {

        [JsonProperty("serveTime")]
        public int ServeTime { get; set; }

        [JsonProperty("airports")]
        public Airport[] Airports { get; set; }

        [JsonProperty("airlines")]
        public Airline[] Airlines { get; set; }

        [JsonProperty("aircrafts")]
        public Aircraft[] Aircrafts { get; set; }

        [JsonProperty("agencies")]
        public Agency[] Agencies { get; set; }

        [JsonProperty("routes")]
        public Route[] Routes { get; set; }
    }

}
