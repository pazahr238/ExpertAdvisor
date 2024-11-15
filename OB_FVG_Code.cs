using System;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class OrderBlock_with_ShowMessage : Robot
    {
        [Parameter("Max Candles to Check", Group = "Candles", DefaultValue = 100, MinValue = 4)]
        public int MaxCandleCheck { get; set; }    // hade aksar candle haei ke mire aghab tuye time trigger check mikone baraye peida kardane swing
        
        [Parameter("Body Range Percent in Higher TimeFrame", Group = "Candles", DefaultValue = 90, MinValue = 50, MaxValue = 100)]
        public int BodyRangePercentH { get; set; }  // taeine mizane momentum budane candle last(1) e timeframe e balatar
        
        [Parameter("Monthly Body Range Percent", Group = "Candles", DefaultValue = 50, MinValue = 50, MaxValue = 100)]
        public int BodyRangePercentM { get; set; }  // taeine mizane momentum budane candle mahane last(1) 
        
        [Parameter("Trigger TimeFrame Body Range Percent", Group = "Candles", DefaultValue = 85, MinValue = 50, MaxValue = 100)]
        public int BodyRangePercentT { get; set; }  // taeine mizane momentum budane candle Trigger Time Frame last(1) 
        
        [Parameter("Max Drop off Last Month %", Group = "Candles", DefaultValue = 20, MinValue = 1, MaxValue = 99)]
        public int Condition3Percent { get; set; }   // 
        
        [Parameter("Risk %", Group = "Risk", DefaultValue = 0.5, MinValue = 0.01, Step = 0.01)]
        public double Risk { get; set; }
        
        [Parameter("Extra value to Stop Loss", Group = "Risk", DefaultValue = 50, MinValue = 0)]
        public int RiskThreshold { get; set; }
        
        [Parameter("Extra value to Entry", Group = "Risk", DefaultValue = 30, MinValue = 0)]
        public double EntryThreshold { get; set; }
        
        [Parameter("Number of Candles Volume in the Higher Time Frame", Group = "Volume", DefaultValue = 100, MinValue = 1)]
        public int AverageCandlesVolumePeriod { get; set; }  // taeine mizane miyangine hajme chand candle akhare timeframe e balatar
        

        protected override void OnStart()
        {
           
        }

        protected override void OnTick()
        {
            
        }

        protected override void OnStop()
        {
            
        }
        
       
        
        private string CheckDirectionMonthly(string DirectionType) // Check first 3 conditions
        {
        
        string CandleType = "";
        Bars myBars = MarketData.GetBars(TimeFrame.Monthly, Symbol.Name);
        
            if (DirectionType == "Bullish")
            {
                if (myBars.Last(1).Close > myBars.Last(1).Open)     //sharte 1
                {
                    double BRP = Math.Round(Math.Abs((myBars.Last(1).Close - myBars.Last(1).Open) / (myBars.Last(1).High - myBars.Last(1).Low)), 2) * 100;
                    if (BRP >= BodyRangePercentM)                // sharte 2
                    {
                    double  DistanceHigh1High0M = myBars.Last(0).High - myBars.Last(1).High;
                    double  DistanceHigh0PriceM = myBars.Last(0).High - Symbol.Ask;
                    double  DistanceFraction = Math.Round((DistanceHigh0PriceM / DistanceHigh1High0M), 2) * 100;
                    
                        if ((DistanceHigh1High0M > 0) && (DistanceFraction <= Condition3Percent) )  // sharte 3
                        {
                            CandleType = "isBullish";
                            //MessageBox.Show("Found Bullish - Time = " + myBars.OpenTimes.Last(1).ToString());   
                        }
                    }
                }
            }
            
            if (DirectionType == "Bearish")
            {
                if (myBars.Last(1).Close < myBars.Last(1).Open)     //sharte 1
                {
                    double BRP = Math.Round(Math.Abs((myBars.Last(1).Open - myBars.Last(1).Close) / (myBars.Last(1).High - myBars.Last(1).Low)), 2) * 100;
                    if (BRP >= BodyRangePercentM)                // sharte 2
                    {
                    double  DistanceLow1Low0M = myBars.Last(1).Low - myBars.Last(0).Low;
                    double  DistanceLow0PriceM = myBars.Last(0).Low - Symbol.Bid;
                    double  DistanceFraction = Math.Round((DistanceLow0PriceM / DistanceLow1Low0M), 2) * 100;
                    
                        if ((DistanceLow1Low0M > 0) && (DistanceFraction <= Condition3Percent) )     // sharte 3
                        {
                            CandleType = "isBearish";
                            //MessageBox.Show("Found Bearish - Time = " + myBars.OpenTimes.Last(1).ToString()); 
                        }
                    }
                }
                
            }
            
         return CandleType;   
        }
        
        private void PlaceOrders(TradeType tradeType, double entryPrice)
        {
          
          int preStop = (int)((Bars.Last(3).High - Math.Min(Bars.Last(3).Low , Bars.Last(2).Low)) * 100000);
          int StopLossPips = preStop + RiskThreshold;
          int TakeProfitPips1 = preStop;
          int TakeProfitPips2 = preStop * 2;
          
          
          double lotsize =  Math.Round((Risk * Account.Equity) / (100 * StopLossPips), 2);          
          double HalfOrderLot1 = Math.Round((lotsize / 2), 2);        
          double HalfOrderLot2 = lotsize - HalfOrderLot1;
          
          
           /// MessageBox.Show("Entry Price = " + entryPrice.ToString() + " - stop = " + StopLossPips.ToString() + " - TP1 = " + TakeProfitPips1.ToString() + " - TP2 = " + TakeProfitPips2.ToString() + " - lot 1/1 =" + HalfOrderLot2 + " - lot 1/5 = " + HalfOrderLot1);          
            
          
          
          if (Account.FreeMargin > Account.Equity * 0.7)
          {
             var initialOrder1 = PlaceLimitOrder(tradeType, SymbolName, Symbol.QuantityToVolumeInUnits(HalfOrderLot2), entryPrice, "OrdeBlock_FVG", StopLossPips, TakeProfitPips1);
             var initialOrder2 = PlaceLimitOrder(tradeType, SymbolName, Symbol.QuantityToVolumeInUnits(HalfOrderLot1), entryPrice, "OrdeBlock_FVG", StopLossPips, TakeProfitPips2);
          }
        }
        
        private string FindSwing(string TradeType) // Fourth Condition
        {
        
        string myresult = "";
        double BRP = Math.Round(Math.Abs((Bars.Last(2).Close - Bars.Last(2).Open) / (Bars.Last(2).High - Bars.Last(2).Low)), 2) * 100;  // Calculate body size last(2)
        
            if (TradeType == "Bullish")
            {
                
            
                if ((BRP >= BodyRangePercentT) && (Bars.Last(2).Close > Bars.Last(2).Open) && (Bars.Last(3).High < Bars.Last(1).Low))  // body Momentum & Bullish & Has FVG
                 {
                    for (int i = 4; i <= MaxCandleCheck; i++)
                    {
                        if ((Bars.Last(i-1).High < Bars.Last(i).High) && (Bars.Last(i+1).High < Bars.Last(i).High) /* Detect Swing */ && (Bars.Last(i).High <= Bars.Last(2).Close) && (Bars.Last(i).High > Bars.Last(2).Open)  /* Swing between open,close last(2)*/  )
                        {
                          if ((Bars.Last(i).Close < Bars.Last(i).Open) || (Bars.Last(i-1).Close < Bars.Last(i-1).Open))  //  Candles i, i-1 should not be Bullish at the same time
                          {  
                            bool BotheringFound = false;
                            for (int j = 3; j<= i-2; j++)   // filter bothering candles between last(2) and Swing
                            {
                                
                                if (Bars.Last(i).High <= Bars.Last(j).High) 
                                {
                                    BotheringFound = true;
                                    break;
                                }
                                
                                
                            }
                            
                            if (! BotheringFound)
                            {
                                myresult = "HighSwing";
                                //MessageBox.Show("Found High Swing - Time = " + Bars.OpenTimes.Last(2).ToString() + " - BRP = " + BRP.ToString() + " - i = " + i.ToString());
                                
                                break;
                            }
                            
                          }
                        }
                    
                    
                    }
                
            
                 }
            
            
            }
            
            if (TradeType == "Bearish")
            {
                
                if ((BRP >= BodyRangePercentT) && (Bars.Last(2).Close < Bars.Last(2).Open) && (Bars.Last(1).High < Bars.Last(3).Low))   // body Momentum & Bearish & Has FVG
                 {
                    for (int i = 4; i <= MaxCandleCheck; i++)
                    {
                        if ((Bars.Last(i-1).Low > Bars.Last(i).Low) && (Bars.Last(i+1).Low > Bars.Last(i).Low) /* Detect Swing */ && (Bars.Last(i).Low >= Bars.Last(2).Close) && (Bars.Last(i).Low < Bars.Last(2).Open) /* Swing between open,close last(2)*/)
                        {
                          if ((Bars.Last(i).Close > Bars.Last(i).Open) || (Bars.Last(i-1).Close > Bars.Last(i-1).Open))  //  Candles i, i-1 should not be Bearish at the same time
                          {
                            bool BotheringFound = false;
                            for (int j = 3; j<= i-2; j++)   // filter bothering candles between last(2) and Swing
                            {
                                
                                if (Bars.Last(i).Low >= Bars.Last(j).Low) 
                                {
                                    BotheringFound = true;
                                    break;
                                }
                                
                                
                                
                            }
                            
                            if (! BotheringFound)
                            {
                                myresult = "LowSwing";
                                //MessageBox.Show("Found Low Swing - Time = " + Bars.OpenTimes.Last(2).ToString() + " - BRP = " + BRP.ToString() + " - i = " + i.ToString());
                                
                                break;
                            }
                          } 
                        }
                    
                    }
                
            
                 }
            
            }
            
          return myresult;  
        }
        
        
        bool CheckHighCandlesVolume() 
        {
        
        double S = 0;
        double AVR_Volume;
        bool myresult = false;
        
        for (int i = 1; i <= AverageCandlesVolumePeriod; i++)
            {
                S += Bars.TickVolumes.Last(i);
            }
            
            AVR_Volume = S/AverageCandlesVolumePeriod;
            
        if (AVR_Volume > Bars.TickVolumes.Last(1))
        myresult = true;
        
        return myresult;
        
        }
        
        
        protected override void OnBar()
        {
        //Check momentun:   double BRP = Math.Round(Math.Abs((Bars.Last(1).Open - Bars.Last(1).Close) / (Bars.Last(1).High - Bars.Last(1).Low)), 2) * 100;
        // BRP >= BodyRangePercent
        
        //check mahane shart 1,2,3:    Bars _marketSeries = MarketData.GetBars(TimeFrame.Monthly, Symbol.Name);
        //    MessageBox.Show(_marketSeries.Last(0).Close.ToString());
        
         if ((CheckHighCandlesVolume())/* ((CheckDirectionMonthly("Bullish") == "isBullish") */  &&  (FindSwing("Bullish") == "HighSwing"))//)
          {
                 //MessageBox.Show("Found High Swing - Time = " + Bars.OpenTimes.Last(1).ToString() + " - entry price = " + Bars.Last(3).High.ToString() + " + " + (EntryThreshold / 100000).ToString());    
                 
                 Notifications.PlaySound(@"E:\My musics\Alert Sounds\Telegram-Message.mp3");
                 
                 /*// Drawing a FVG_rectangle with custom properties
                 var startTime_FVG_Rec = Bars.OpenTimes.Last(3);
                 var endTime_FVG_Rec = Bars.OpenTimes.Last(1);
                 var startPrice_FVG_Rec = Bars.Last(3).High;
                 var endPrice_FVG_Rec = Bars.Last(1).Low;
                 
                 // Drawing a OB_rectangle with custom properties
                 var startTime_OB_Rec = Bars.OpenTimes.Last(3);
                 var endTime_OB_Rec = Bars.OpenTimes.Last(1);
                 var startPrice_OB_Rec = Bars.Last(3).Low;
                 var endPrice_OB_Rec = Bars.Last(1).Low;

                 var FVG_Rectangle = Chart.DrawRectangle("uniqueRectangleId", startTime_FVG_Rec, startPrice_FVG_Rec, endTime_FVG_Rec, endPrice_FVG_Rec, Color.Gray);
                 FVG_Rectangle.IsFilled = true;
                 
                 var OB_Rectangle = Chart.DrawRectangle("uniqueRectangleId", startTime_OB_Rec, startPrice_OB_Rec, endTime_OB_Rec, endPrice_OB_Rec, Color.Gray);
                 OB_Rectangle.IsFilled = false;
                 OB_Rectangle.Color = Color.FromArgb(80, OB_Rectangle.Color);*/
                 
                 MessageBox.Show("BUY - Time = " + Bars.OpenTimes.Last(1).ToString(), "Symbol = " + Symbol.Name.ToString());       
                 
                 //PlaceOrders(TradeType.Buy, Bars.Last(3).High + EntryThreshold / 100000);
                 
                 // >>> Pending Order Buy
          }
         
         if /* ((CheckDirectionMonthly("Bearish") == "isBearish") && */ (FindSwing("Bearish") == "LowSwing") //)
          {
                 //MessageBox.Show("Found Low Swing - Time = " + Bars.OpenTimes.Last(1).ToString() + " - entry price = " + (Bars.Last(3).Low - EntryThreshold / 100000).ToString());
                 
                 Notifications.PlaySound(@"D:\music\triad_gbd.mp3");
                                                
                 MessageBox.Show("SELL - Time = " + Bars.OpenTimes.Last(1).ToString(), "Symbol = " + Symbol.Name.ToString());
                 
                 //PlaceOrders(TradeType.Sell, Bars.Last(3).Low - EntryThreshold / 100000);
                                  
                 // >>> Pending Order Sell
          }
          
            
        }
    }
}
