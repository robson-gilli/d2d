using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Rome2RioResponse;
using Newtonsoft.Json;
using System.Globalization;

namespace Door2DoorWebApp.TestPages
{
    public partial class ItineraryInputPage : System.Web.UI.Page
    {
        struct RequestData
        {
            public Segment segment;
            public string nextArrDate;
            public int segmentIndex;
            public int routeIndex;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VerifyRequest();



            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void VerifyRequest()
        {
            Segment segment = new Segment();
            DateTime arrivalDT = new DateTime();
            int routeIndex = 0, segmentIndex = 0;
            if (Request.Form["r2r_resp"] != null)
            {
                segment = JsonConvert.DeserializeObject<Segment>(Request.Form["r2r_resp"]);//JsonConvert.DeserializeObject<Segment>(GetMockJson());
            }
            if (Request.Form["arrdate"] != null)
            {
                arrivalDT = DateTime.ParseExact(Request.Form["arrdate"], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            }
            if (Request.Form["segmentIndex"] != null)
            {
                segmentIndex = int.Parse(Request.Form["segmentIndex"]);
            }

            if (Request.Form["routeIndex"] != null)
            {
                routeIndex = int.Parse(Request.Form["routeIndex"]);
            }

            RequestData rd = new RequestData();
            rd.nextArrDate = arrivalDT.ToString("yyyy-MM-dd HH:mm");
            rd.segment = segment;
            rd.segmentIndex = segmentIndex;
            rd.routeIndex = routeIndex;

            litJsonRq.Text = JsonConvert.SerializeObject(rd);
        }








        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetMockJson()
        {
            return @"{
          'kind': 'flight',
          'isMajor': 1,
          'isImperial': 0,
          'distance': 18536.97,
          'duration': 1435,
          'sName': null,
          'sPos': null,
          'tName': null,
          'tPos': null,
          'vehicle': null,
          'path': null,
          'indicativePrice': {
            'price': 1700.0,
            'currency': 'BRL',
            'isFreeTransfer': 0,
            'nativePrice': 0.0,
            'nativeCurrency': null
          },
          'subkind': null,
          'sCode': 'GRU',
          'tCode': 'HND',
          'itineraries': [
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 680,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'CDG',
                      'sTime': '15:40',
                      'tTime': '08:00',
                      'flight': '457',
                      'airline': 'AF',
                      'aircraft': '77W',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 725,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'CDG',
                      'tCode': 'HND',
                      'sTime': '10:55',
                      'tTime': '06:00',
                      'flight': '272',
                      'airline': 'AF',
                      'aircraft': '772',
                      'sTerminal': '2E',
                      'codeshares': null,
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2500.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 685,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'HND',
                      'sTime': '12:10',
                      'tTime': '06:35',
                      'flight': '204',
                      'airline': 'NH',
                      'aircraft': '788',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 665,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'LHR',
                      'sTime': '16:15',
                      'tTime': '07:20',
                      'flight': '246',
                      'airline': 'BA',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'IB'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 710,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'LHR',
                      'tCode': 'HND',
                      'sTime': '11:30',
                      'tTime': '07:20',
                      'flight': '007',
                      'airline': 'BA',
                      'aircraft': '777',
                      'sTerminal': '5',
                      'codeshares': [
                        {
                          'airline': 'JL'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 700,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '22:15',
                      'tTime': '14:55',
                      'flight': '8070',
                      'airline': 'JJ',
                      'aircraft': '773',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 655,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'HND',
                      'sTime': '18:20',
                      'tTime': '12:15',
                      'flight': '716',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 3600.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 710,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'MUC',
                      'sTime': '16:45',
                      'tTime': '09:35',
                      'flight': '505',
                      'airline': 'LH',
                      'aircraft': '346',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 680,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'MUC',
                      'tCode': 'HND',
                      'sTime': '15:45',
                      'tTime': '10:05',
                      'flight': '714',
                      'airline': 'LH',
                      'aircraft': '346',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 655,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'HND',
                      'sTime': '18:20',
                      'tTime': '12:15',
                      'flight': '716',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 675,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'LHR',
                      'sTime': '23:50',
                      'tTime': '15:05',
                      'flight': '8084',
                      'airline': 'JJ',
                      'aircraft': '773',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LA'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 700,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'LHR',
                      'tCode': 'HND',
                      'sTime': '19:35',
                      'tTime': '15:15',
                      'flight': '278',
                      'airline': 'NH',
                      'aircraft': '77W',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'VS'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2600.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 635,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'YYZ',
                      'sTime': '20:00',
                      'tTime': '05:35',
                      'flight': '091',
                      'airline': 'AC',
                      'aircraft': '763',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JJ'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 775,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'YYZ',
                      'tCode': 'HND',
                      'sTime': '13:00',
                      'tTime': '14:55',
                      'flight': '005',
                      'airline': 'AC',
                      'aircraft': '788',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2500.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 700,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '22:15',
                      'tTime': '14:55',
                      'flight': '8070',
                      'airline': 'JJ',
                      'aircraft': '773',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 670,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'HND',
                      'sTime': '20:45',
                      'tTime': '14:55',
                      'flight': '224',
                      'airline': 'NH',
                      'aircraft': '77W',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 3600.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 650,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'KIX',
                      'sTime': '13:20',
                      'tTime': '07:10',
                      'flight': '740',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 75,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '08:10',
                      'tTime': '09:25',
                      'flight': '144',
                      'airline': 'NH',
                      'aircraft': '772',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'HA'
                        },
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 870,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'DXB',
                      'sTime': '01:25',
                      'tTime': '22:55',
                      'flight': '262',
                      'airline': 'EK',
                      'aircraft': '77W',
                      'sTerminal': '2',
                      'codeshares': null,
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 595,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'DXB',
                      'tCode': 'HND',
                      'sTime': '08:05',
                      'tTime': '23:00',
                      'flight': '312',
                      'airline': 'EK',
                      'aircraft': '77L',
                      'sTerminal': '3',
                      'codeshares': null,
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2400.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 620,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'MAD',
                      'sTime': '18:05',
                      'tTime': '09:25',
                      'flight': '908',
                      'airline': 'CA',
                      'aircraft': '330',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 690,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'MAD',
                      'tCode': 'PEK',
                      'sTime': '11:25',
                      'tTime': '04:55',
                      'flight': '908',
                      'airline': 'CA',
                      'aircraft': '330',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 185,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'PEK',
                      'tCode': 'HND',
                      'sTime': '08:45',
                      'tTime': '12:50',
                      'flight': '181',
                      'airline': 'CA',
                      'aircraft': '330',
                      'sTerminal': '3',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 17,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 0,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 685,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'CDG',
                      'sTime': '19:10',
                      'tTime': '11:35',
                      'flight': '459',
                      'airline': 'AF',
                      'aircraft': '772',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 715,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'CDG',
                      'tCode': 'HND',
                      'sTime': '23:25',
                      'tTime': '18:20',
                      'flight': '274',
                      'airline': 'AF',
                      'aircraft': '77W',
                      'sTerminal': '2E',
                      'codeshares': [
                        {
                          'airline': 'JL'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2500.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 650,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'KIX',
                      'sTime': '13:20',
                      'tTime': '07:10',
                      'flight': '740',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 70,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '11:05',
                      'tTime': '12:15',
                      'flight': '22',
                      'airline': '7G',
                      'aircraft': '320',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 650,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'KIX',
                      'sTime': '13:20',
                      'tTime': '07:10',
                      'flight': '740',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 75,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '11:00',
                      'tTime': '12:15',
                      'flight': '3822',
                      'airline': 'NH',
                      'aircraft': '320',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 675,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'LHR',
                      'sTime': '23:50',
                      'tTime': '15:05',
                      'flight': '8084',
                      'airline': 'JJ',
                      'aircraft': '773',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LA'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 705,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'LHR',
                      'tCode': 'HND',
                      'sTime': '19:15',
                      'tTime': '15:00',
                      'flight': '044',
                      'airline': 'JL',
                      'aircraft': '77W',
                      'sTerminal': '3',
                      'codeshares': [
                        {
                          'airline': 'BA'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 695,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'FRA',
                      'sTime': '18:40',
                      'tTime': '11:15',
                      'flight': '507',
                      'airline': 'LH',
                      'aircraft': '744',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 560,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'FRA',
                      'tCode': 'PEK',
                      'sTime': '13:55',
                      'tTime': '05:15',
                      'flight': '966',
                      'airline': 'CA',
                      'aircraft': '773',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        },
                        {
                          'airline': 'TP'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 185,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'PEK',
                      'tCode': 'HND',
                      'sTime': '08:45',
                      'tTime': '12:50',
                      'flight': '181',
                      'airline': 'CA',
                      'aircraft': '330',
                      'sTerminal': '3',
                      'codeshares': [
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 855,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'DOH',
                      'sTime': '02:45',
                      'tTime': '23:00',
                      'flight': '772',
                      'airline': 'QR',
                      'aircraft': '77L',
                      'sTerminal': '2',
                      'codeshares': null,
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 590,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'DOH',
                      'tCode': 'KIX',
                      'sTime': '01:35',
                      'tTime': '17:25',
                      'flight': '802',
                      'airline': 'QR',
                      'aircraft': '332',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JL'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 80,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '19:55',
                      'tTime': '21:15',
                      'flight': '148',
                      'airline': 'NH',
                      'aircraft': '737',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'CA'
                        },
                        {
                          'airline': 'ZH'
                        },
                        {
                          'airline': 'UA'
                        },
                        {
                          'airline': 'HA'
                        },
                        {
                          'airline': 'TG'
                        },
                        {
                          'airline': 'QR'
                        },
                        {
                          'airline': 'SC'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 750,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'LAX',
                      'sTime': '13:25',
                      'tTime': '21:55',
                      'flight': '062',
                      'airline': 'KE',
                      'aircraft': '77W',
                      'sTerminal': '1',
                      'codeshares': null,
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 700,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'LAX',
                      'tCode': 'HND',
                      'sTime': '01:20',
                      'tTime': '05:00',
                      'flight': '1005',
                      'airline': 'NH',
                      'aircraft': '777',
                      'sTerminal': 'B',
                      'codeshares': [
                        {
                          'airline': 'UA'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 118,
                  'indicativePrice': {
                    'price': 2500.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 855,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'DOH',
                      'sTime': '02:45',
                      'tTime': '23:00',
                      'flight': '772',
                      'airline': 'QR',
                      'aircraft': '77L',
                      'sTerminal': '2',
                      'codeshares': null,
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 590,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'DOH',
                      'tCode': 'KIX',
                      'sTime': '01:35',
                      'tTime': '17:25',
                      'flight': '802',
                      'airline': 'QR',
                      'aircraft': '332',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JL'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 70,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '21:20',
                      'tTime': '22:30',
                      'flight': '192',
                      'airline': 'JL',
                      'aircraft': '73H',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'QR'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 665,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'LHR',
                      'sTime': '16:15',
                      'tTime': '07:20',
                      'flight': '246',
                      'airline': 'BA',
                      'aircraft': '744',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'IB'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 705,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'LHR',
                      'tCode': 'HND',
                      'sTime': '19:15',
                      'tTime': '15:00',
                      'flight': '044',
                      'airline': 'JL',
                      'aircraft': '77W',
                      'sTerminal': '3',
                      'codeshares': [
                        {
                          'airline': 'BA'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 855,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'DOH',
                      'sTime': '02:45',
                      'tTime': '23:00',
                      'flight': '772',
                      'airline': 'QR',
                      'aircraft': '77L',
                      'sTerminal': '2',
                      'codeshares': null,
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 590,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'DOH',
                      'tCode': 'KIX',
                      'sTime': '01:35',
                      'tTime': '17:25',
                      'flight': '802',
                      'airline': 'QR',
                      'aircraft': '332',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JL'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 70,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'KIX',
                      'tCode': 'HND',
                      'sTime': '21:35',
                      'tTime': '22:45',
                      'flight': '150',
                      'airline': 'NH',
                      'aircraft': '767',
                      'sTerminal': '1',
                      'codeshares': [
                        {
                          'airline': 'TG'
                        },
                        {
                          'airline': 'QR'
                        }
                      ],
                      'dayChange': null
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 710,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'MUC',
                      'sTime': '16:45',
                      'tTime': '09:35',
                      'flight': '505',
                      'airline': 'LH',
                      'aircraft': '346',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'JJ'
                        },
                        {
                          'airline': 'NH'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 685,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'MUC',
                      'tCode': 'HND',
                      'sTime': '21:25',
                      'tTime': '15:50',
                      'flight': '276',
                      'airline': 'NH',
                      'aircraft': '788',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'LH'
                        }
                      ],
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2800.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': true
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 583,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'ATL',
                      'sTime': '23:00',
                      'tTime': '07:43',
                      'flight': '58',
                      'airline': 'DL',
                      'aircraft': '764',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'G3'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 329,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'ATL',
                      'tCode': 'SEA',
                      'sTime': '13:20',
                      'tTime': '15:49',
                      'flight': '2188',
                      'airline': 'DL',
                      'aircraft': '757',
                      'sTerminal': 'S',
                      'codeshares': [
                        {
                          'airline': 'AF'
                        },
                        {
                          'airline': 'AM'
                        },
                        {
                          'airline': 'AS'
                        },
                        {
                          'airline': 'AZ'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 619,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'SEA',
                      'tCode': 'HND',
                      'sTime': '20:01',
                      'tTime': '22:20',
                      'flight': '581',
                      'airline': 'DL',
                      'aircraft': '767',
                      'sTerminal': null,
                      'codeshares': null,
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 583,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'ATL',
                      'sTime': '23:00',
                      'tTime': '07:43',
                      'flight': '58',
                      'airline': 'DL',
                      'aircraft': '764',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'G3'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 345,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'ATL',
                      'tCode': 'SEA',
                      'sTime': '09:40',
                      'tTime': '12:25',
                      'flight': '139',
                      'airline': 'DL',
                      'aircraft': '763',
                      'sTerminal': 'S',
                      'codeshares': [
                        {
                          'airline': 'AS'
                        },
                        {
                          'airline': 'CZ'
                        },
                        {
                          'airline': 'G3'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 619,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'SEA',
                      'tCode': 'HND',
                      'sTime': '20:01',
                      'tTime': '22:20',
                      'flight': '581',
                      'airline': 'DL',
                      'aircraft': '767',
                      'sTerminal': null,
                      'codeshares': null,
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            },
            {
              'legs': [
                {
                  'url': null,
                  'host': null,
                  'hops': [
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 583,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'GRU',
                      'tCode': 'ATL',
                      'sTime': '23:00',
                      'tTime': '07:43',
                      'flight': '58',
                      'airline': 'DL',
                      'aircraft': '764',
                      'sTerminal': '2',
                      'codeshares': [
                        {
                          'airline': 'G3'
                        }
                      ],
                      'dayChange': 1
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 325,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'ATL',
                      'tCode': 'SEA',
                      'sTime': '11:05',
                      'tTime': '13:30',
                      'flight': '1223',
                      'airline': 'DL',
                      'aircraft': '753',
                      'sTerminal': 'S',
                      'codeshares': [
                        {
                          'airline': 'AS'
                        },
                        {
                          'airline': 'G3'
                        }
                      ],
                      'dayChange': null
                    },
                    {
                      'sName': null,
                      'sPos': null,
                      'tName': null,
                      'tPos': null,
                      'frequency': 0,
                      'duration': 619,
                      'indicativePrice': null,
                      'lines': null,
                      'sCode': 'SEA',
                      'tCode': 'HND',
                      'sTime': '20:01',
                      'tTime': '22:20',
                      'flight': '581',
                      'airline': 'DL',
                      'aircraft': '767',
                      'sTerminal': null,
                      'codeshares': null,
                      'dayChange': 1
                    }
                  ],
                  'days': 127,
                  'indicativePrice': {
                    'price': 2300.0,
                    'currency': 'BRL',
                    'isFreeTransfer': 0
                  }
                }
              ],
              'isHidden': 1,
              'validForSchedule': false
            }
          ],
          'arrivalDateTime': '2014-06-27T18:20:00',
          'departureDateTime': '2014-06-25T19:10:00',
          'chosenItinerary': 12,
          'frequency': null
        }";
        }
    }
}