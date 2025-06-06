using currency.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace currency.Models
{
    public class Currency : ObservableObject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; } // 오늘의 환율
        public double PreviousRate { get; set; }  // 어제의 환율
        public double RateChange => PreviousRate == 0 ? 0 : Rate - PreviousRate;
        public double RateChangePercent => PreviousRate == 0 ? 0 : (Rate - PreviousRate) / PreviousRate * 100;
        public DateTime LastUpdate { get; set; }


        // 추가: 아이콘용 속성
        public string ChangeIcon => RateChange switch
        {
            > 0 => "▲",
            < 0 => "▼",
            _ => "-"
        };
    }

}