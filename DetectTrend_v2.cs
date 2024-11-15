using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;


namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class FindHigherTrendDirection : Robot
    {
        [Parameter("Body Range Percent", Group = "Candles", DefaultValue = 50, MinValue = 30, MaxValue = 100, Step = 1)]
        public double BodyRangePercent { get; set; } 
        
        double lastprice_bid = 0;
        double lastprice_ask = 0;
        bool TrendFlag = false;
        
        protected override void OnStart()
        {
           lastprice_ask = Bars.Last(1).High;
           lastprice_bid = Bars.Last(1).Low;
           
        }

        protected override void OnTick()
        {
        
         
         double BRP = Math.Round(Math.Abs((Bars.Last(1).Open - Bars.Last(1).Close) / (Bars.Last(1).High - Bars.Last(1).Low)), 2) * 100;
        
       
        //MessageBox.Show(BRP.ToString());
         if (Bars.Last(1).Close < Bars.Last(1).Open) // Bearish Trend 
         {
         
            if ((Symbol.Bid < Bars.Last(1).Low) && (BRP >= BodyRangePercent)  && (Symbol.Bid <= lastprice_bid)) 
             {
               //MessageBox.Show("Higher Trend Direction is Bearish - time: " + Bars.OpenTimes.Last(1).ToString());
               TrendFlag = false; // bearish
               if (Symbol.Bid < lastprice_bid) lastprice_bid = Symbol.Bid;
             }
             
            if (Bars.Last(1).Close < Symbol.Bid) 
            {
                TrendFlag = true;
                lastprice_bid = 0;
            }
         }
         
         else // Bullish Trend 
         
         {
            if ((Symbol.Ask > Bars.Last(1).High) && (BRP >= BodyRangePercent)  && (Symbol.Ask >= lastprice_ask))
             {
               //MessageBox.Show("Higher Trend Direction is Bullish - time: " + Bars.OpenTimes.Last(1).ToString());
               TrendFlag = true; // bullish
               if (Symbol.Ask < lastprice_ask) lastprice_ask = Symbol.Ask;
             }
             
            if (Bars.Last(1).Close > Symbol.Ask) 
            {
                TrendFlag = false;
                lastprice_ask = 0;
            }
         }
           
           
         
        }

        protected override void OnBarClosed()
        {
           
        }

        protected override void OnStop()
        {
            
        }
        
        protected override void OnBar()
        {
        
         
        
        }
    }
}
