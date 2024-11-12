using cAlgo.API;
using System;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SNREngulfingPatternBot : Robot
    {
        
        [Parameter("Minimum Body Size (Pips)", Group = "Pattern Settings", DefaultValue = 200, MinValue = 1)]
        public int MinimumBodySizePips { get; set; }
        
        ///[Parameter("Quantity (Lots)", Group = "Volume", DefaultValue = 0.1, MinValue = 0.01, Step = 0.01)]
        ///public double Quantity { get; set; }  
        
        [Parameter("Risk %", Group = "Volume", DefaultValue = 0.5, MinValue = 0.01, Step = 0.01)]
        public double Risk { get; set; }

        [Parameter("Stop Loss (Pips)", Group = "Trade Parameters", DefaultValue = 100)]
        public int StopLossPips { get; set; }

        [Parameter("Take Profit (Pips)", Group = "Trade Parameters", DefaultValue = 650)]
        public int TakeProfitPips { get; set; }

        ///[Parameter("Number of Additional Orders", Group = "Advanced", DefaultValue = 0, MinValue = 0)]
        ///public int NumAdditionalOrders { get; set; }

        [Parameter("Spacing from Initial Order (Pips)", Group = "Advanced", DefaultValue = 50, MinValue = 1)]
        public int SpacingPips { get; set; }

        [Parameter("Initial Support Level", Group = "SNR Settings", DefaultValue = -1000000)]
        public double InitialSupportLevel { get; set; }

        [Parameter("Initial Resistance Level", Group = "SNR Settings", DefaultValue = 1000000)]
        public double InitialResistanceLevel { get; set; }

        double lotsize = 0;
        
        protected override void OnStart()
        {
        
        }

        protected override void OnBar()
        {
        // version 3
            var price = Bars.ClosePrices.Last(1);
            
            if (IsBullishEngulfing() && price > InitialSupportLevel)
                   PlaceOrders(TradeType.Buy, Bars.HighPrices.Last(2) - (Bars.HighPrices.Last(2) - Bars.LowPrices.Last(2)) / 2);
                else if (IsBearishEngulfing() && price < InitialResistanceLevel)
                   PlaceOrders(TradeType.Sell, Bars.LowPrices.Last(2) + (Bars.HighPrices.Last(2) - Bars.LowPrices.Last(2)) / 2);

        }

        private bool IsBullishEngulfing()
        {
            var previousBar = Bars.Last(2);
            var currentBar = Bars.Last(1);
            return currentBar.Close > currentBar.Open && previousBar.Close < previousBar.Open &&
                   (currentBar.Close - currentBar.Open) > MinimumBodySizePips * Symbol.PipSize;
        }

        private bool IsBearishEngulfing()
        {
            var previousBar = Bars.Last(2);
            var currentBar = Bars.Last(1);
            return currentBar.Close < currentBar.Open && previousBar.Close > previousBar.Open &&
                   (currentBar.Open - currentBar.Close) > MinimumBodySizePips * Symbol.PipSize;
        }

        private void PlaceOrders(TradeType tradeType, double entryPrice)
        {
    
          lotsize =  Math.Round((Risk * Account.Equity) / (100 * StopLossPips), 2);
          
          if (Account.FreeMargin > Account.Equity * 0.7)
          {
            var initialOrder = PlaceLimitOrder(tradeType, SymbolName, Symbol.QuantityToVolumeInUnits(lotsize), entryPrice, "EngulfingPattern", StopLossPips, TakeProfitPips);
          }
         
          
          
          //MessageBox.Show("Balance = " + Account.Balance.ToString() + " - Equity = " + Account.Equity.ToString() + " - StoplossPIP = " + StopLossPips.ToString() + " - TakeProfitPIP = " + TakeProfitPips.ToString() + " - lot size = " + lotsize.ToString() + " - entry price = " + entryPrice.ToString() + " - Month = " + Server.Time.Month.ToString() + " - day = " + Server.Time.Day.ToString());
        
            ///for (int i = 1; i <= NumAdditionalOrders; i++)
            ///{
            ///    double additionalPrice = entryPrice + i * SpacingPips * Symbol.PipSize;
            ///    PlaceLimitOrder(tradeType, SymbolName, Symbol.QuantityToVolumeInUnits(Quantity), additionalPrice, "EngulfingPattern", StopLossPips, TakeProfitPips);
            ///}
        }
    }
}
