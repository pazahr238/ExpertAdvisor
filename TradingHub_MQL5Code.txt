//+------------------------------------------------------------------+
//|                                                   TradingHub.mq5 |
//|                               Copyright Reza Pazahr - Ali Pazahr |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright Reza Pazahr - Ali Pazahr"
#property link      "https://www.mql5.com"
#property version   "2.0"
//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
//---------- Define Global Variables ---------------------

datetime lastBarTime = 0;
string ellipseName;
string StrctStartDirection = "";
double arrHighLow[2][100][2];  
bool FoundStartingPoint = false;



int OnInit()
  {
//--- create timer
   lastBarTime = iTime(_Symbol, PERIOD_CURRENT, 0);
   
   //string templateName = "C:\\Users\\Pazahr\\AppData\\Roaming\\MetaQuotes\\Terminal\\065434634B76DD288A1DDF20131E8DDB\\MQL5\\Profiles\\Templates";
   //if (ChartApplyTemplate(0, templateName))
   //{
   //   Print("Saaaaaaaaallllllllaaaaaaaaaaaaaaaammmmmmmm");
   //}
   
   return(INIT_SUCCEEDED);
//---
  }
  
  
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//--- destroy timer
   EventKillTimer();
   
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
   datetime currentBarTime = iTime(_Symbol, PERIOD_CURRENT, 0);
   
   // Check if a new bar has started
   if(currentBarTime != lastBarTime)
     {
      lastBarTime = currentBarTime; // Update the last bar time
      OnBar();                       // Call the OnBar function
     }
     
     
  }
//+------------------------------------------------------------------+
//| Timer function                                                   |
//+------------------------------------------------------------------+
void OnTimer()
  {
//---
   
  }
//+------------------------------------------------------------------+
void OnBar()
  {
   // Code to execute on a new bar
int StrctStartingPoint;


 

    //Comment("A new bar has started at: ", TimeToString(lastBarTime, TIME_DATE|TIME_MINUTES));
   if (!FoundStartingPoint) 
    {
        StrctStartingPoint = FindStartingPointTraversal(); // Find the starting point
         
        
        //Comment(CheckCondition(StrctStartingPoint-1,StrctStartingPoint,StrctStartingPoint+1));
        

        FoundStartingPoint = true;  // Prevent finding the same starting point in the next OnTick
        
        
        Comment("Starting Point = " + (string)StrctStartingPoint);
        //////Comment("StartingPoint = " + StrctStartingPoint + " - Condition = " + (string)CheckCondition(74,75,76));
        
        
        
//////        for (int j = StrctStartingPoint; j > 1 ; j--)
//////        {
//////            Print("j = " + (string)j + " - Condition = " + (string)CheckCondition(j-1,j,j+1));
//////        
//////        
//////        }
        
        
        //TesterStop();
        

        
        
        Traverse(StrctStartingPoint);  // Initialize high & low points

        //DrawCircle(3 , "low");
         
        for (int j = 0; j <= StrctStartingPoint; j++)
        {

            if ((arrHighLow[0][j][0] == 0) && (arrHighLow[1][j][0] == 0)) break;

            
            

            // Check for non-zero price in arrHighLow array
            
            
            // Check if it's a high point and draw a circle
            if (arrHighLow[0][j][0] != 0)
            {
                // Print("j = ", j, " - High");
                DrawCircle((int)arrHighLow[0][j][1], "high");
                Print("High = ", arrHighLow[0][j][0], " - Candle Number high = ", (int)arrHighLow[0][j][1]);
            }

            // Check if it's a low point and draw a circle
            if (arrHighLow[1][j][0] != 0)
            {
                // Print("j = ", j, " - Low");
                DrawCircle((int)arrHighLow[1][j][1], "low");
                Print("Low = ", arrHighLow[1][j][0], " - Candle Number low = ", (int)arrHighLow[1][j][1]);
            }

            // Uncomment to see details
            
        }

        // Display message box for starting point
        //MessageBox("Starting point: " + IntegerToString(StrctStartingPoint));
        
      DrawCircle(1, "high");
      DrawCircle(2, "high");
      DrawCircle(3, "high");
   
    }
   
   
   

   
   
   //Comment(p2);
   //Comment(CheckCondition(p1, p2, p3));
   
  }
//+------------ Starting Point --------------------------------------------------+

int FindStartingPointTraversal() 
{
    double HigherTFPrice = 0;
    string DirectionType = "";
    int NCandle = 0;
    int MonthlyBars = iBars(_Symbol, PERIOD_MN1);
    int H4Bars = iBars(_Symbol, PERIOD_H4);

    //-------------------- Check which Direction Type we have ------------------
    for (int i = 2; i <= 200 && i < MonthlyBars; i++)
    {
        double MonthlyLow = iLow(_Symbol, PERIOD_MN1, i);
        double MonthlyHigh = iHigh(_Symbol, PERIOD_MN1, i);
        
        if ((MonthlyLow < iLow(_Symbol, PERIOD_MN1, i - 1)) && (MonthlyLow < iLow(_Symbol, PERIOD_MN1, i + 1)))
        {
            DirectionType = "Bullish";
            HigherTFPrice = MonthlyLow;
            break;
        }
        
        if ((MonthlyHigh > iHigh(_Symbol, PERIOD_MN1, i - 1)) && (MonthlyHigh > iHigh(_Symbol, PERIOD_MN1, i + 1)))
        {
            DirectionType = "Bearish";
            HigherTFPrice = MonthlyHigh;
            break;
        }
    }

    //-------------------- Bullish ------------------------
    if (DirectionType == "Bullish")
    {
        for (int i = 2; i <= 1000 && i < H4Bars; i++) 
        {
            double H4Low = iLow(_Symbol, PERIOD_H4, i);

            if ((H4Low == HigherTFPrice) && (H4Low < iLow(_Symbol, PERIOD_H4, i - 1)) && (H4Low < iLow(_Symbol, PERIOD_H4, i + 1)))
            {
                NCandle = i;
                StrctStartDirection = "Bullish"; // shows we will start a bullish direction in structure time frame
                break;
            }
        }
    }

    //-------------------- Bearish ------------------------
    if (DirectionType == "Bearish")
    {                
        for (int i = 2; i <= 1000 && i < H4Bars; i++) 
        {
            double H4High = iHigh(_Symbol, PERIOD_H4, i);

            if ((H4High == HigherTFPrice) && (H4High > iHigh(_Symbol, PERIOD_H4, i - 1)) && (H4High > iHigh(_Symbol, PERIOD_H4, i + 1)))
            {
                NCandle = i;
                StrctStartDirection = "Bearish";  // shows we will start a bearish direction in structure time frame
                break;
            }
        }
    }    
    
    return NCandle;
}


//+-------------- Check Condition -------------------------------------------------+
int CheckCondition(int NumCandle1, int NumCandle2, int NumCandle3)
{
    int NumCondition = 0;
    string symbol = Symbol();

    // Retrieve high, low, close, and open prices for each candle
    double High1 = iHigh(symbol, PERIOD_H4, NumCandle1);
    double Low1 = iLow(symbol, PERIOD_H4, NumCandle1);
    double Close1 = iClose(symbol, PERIOD_H4, NumCandle1);
    double Open1 = iOpen(symbol, PERIOD_H4, NumCandle1);

    double High2 = iHigh(symbol, PERIOD_H4, NumCandle2);
    double Low2 = iLow(symbol, PERIOD_H4, NumCandle2);
    double Close2 = iClose(symbol, PERIOD_H4, NumCandle2);
    double Open2 = iOpen(symbol, PERIOD_H4, NumCandle2);

    double High3 = iHigh(symbol, PERIOD_H4, NumCandle3);
    double Low3 = iLow(symbol, PERIOD_H4, NumCandle3);

    // Condition 1 , 7
    if ((High1 > High2) && (Low1 >= Low2) && (High2 > High3) && (Low2 > Low3))
    {
        NumCondition = 17;
    }

    // Condition 2 , 8
    if ((High1 < High2) && (Low1 > Low2) && (High2 > High3) && (Low2 > Low3))
    {
        NumCondition = 28;
    }

    // Condition 3
    if ((High1 < High2) && (Low1 < Low2) && (High2 > High3) && (Low2 > Low3))
    {
        NumCondition = 3;
    }

    // Condition 4 - 1 green & green
    if ((High1 > High2) && (Low1 < Low2) && (High2 > High3) && (Low2 > Low3) && (Close2 > Open2) && (Close1 > Open1))
    {
        NumCondition = 41;
    }

    // Condition 4 - 2 green & red
    if ((High1 > High2) && (Low1 < Low2) && (High2 > High3) && (Low2 > Low3) && (Close2 > Open2) && (Close1 < Open1))
    {
        NumCondition = 42;
    }

    // Condition 4 - 3 red & green
    if ((High1 > High2) && (Low1 < Low2) && (High2 > High3) && (Low2 > Low3) && (Close2 < Open2) && (Close1 > Open1))
    {
        NumCondition = 43;
    }

    // Condition 4 - 4 red & red
    if ((High1 > High2) && (Low1 < Low2) && (High2 > High3) && (Low2 > Low3) && (Close2 < Open2) && (Close1 < Open1))
    {
        NumCondition = 44;
    }

    // Condition 5 , 6 , 9
    if ((High1 == High2) && (High2 > High3) && (Low2 > Low3))
    {
        NumCondition = 569;
    }

    // Condition 10 , 16
    if ((High1 < High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 < High3))
    {
        NumCondition = 1016;
    }

    // Condition 11 , 17
    if ((High1 < High2) && (Low1 > Low2) && (Low2 < Low3) && (High2 < High3))
    {
        NumCondition = 1117;
    }

    // Condition 12
    if ((High1 > High2) && (Low1 > Low2) && (Low2 < Low3) && (High2 < High3))
    {
        NumCondition = 12;
    }

    // Condition 13 - 1 green & green
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 < High3) && (Close2 > Open2) && (Close1 > Open1))
    {
        NumCondition = 131;
    }

    // Condition 13 - 2 green & red
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 < High3) && (Close2 >= Open2) && (Close1 < Open1))
    {
        NumCondition = 132;
    }

    // Condition 13 - 3 red & green
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 < High3) && (Close2 < Open2) && (Close1 > Open1))
    {
        NumCondition = 133;
    }

    // Condition 13 - 4 red & red
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 < High3) && (Close2 <= Open2) && (Close1 < Open1))
    {
        NumCondition = 134;
    }

    // Condition 14 , 15 , 18
    if ((Low1 == Low2) && (Low2 < Low3) && (High2 < High3))
    {
        NumCondition = 141518;
    }

    // Condition 19 - 1 green & green
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 > Open2) && (Close1 > Open1))
    {
        NumCondition = 191;
    }

    // Condition 19 - 2 green & red
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 > Open2) && (Close1 < Open1))
    {
        NumCondition = 192;
    }

    // Condition 19 - 3 red & green
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 < Open2) && (Close1 > Open1))
    {
        NumCondition = 193;
    }

    // Condition 19 - 4 red & red
    if ((High1 > High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 < Open2) && (Close1 < Open1))
    {
        NumCondition = 194;
    }
    
    // Condition 20 - 1 green above
    if ((High1 > High2) && (Low1 > Low2) && (Low2 <= Low3) && (High2 > High3) && (Close2 > Open2))
    {
        NumCondition = 201;
    }
    
    // Condition 20 - 2 green below
    if ((High1 < High2) && (Low1 < Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 > Open2))
    {
        NumCondition = 202;
    }
    
    // Condition 21 - 1 red above
    if ((High1 > High2) && (Low1 > Low2) && (Low2 < Low3) && (High2 > High3) && (Close2 < Open2))
    {
        NumCondition = 211;
    }
    
    // Condition 21 - 2 red below
    if ((High1 < High2) && (Low1 < Low2) && (Low2 <= Low3) && (High2 > High3) && (Close2 < Open2))
    {
        NumCondition = 212;
    }
    
    // Condition 22 
    if ((High1 < High2) && (Low1 > Low2) && (Low2 < Low3) && (High2 > High3))
    {
        NumCondition = 22;
    }
    

    return NumCondition;
}
//+------------------Traverse---------------------------------+
int Traverse(int NofCandle) 
{
    ENUM_TIMEFRAMES  StructureTFBar = PERIOD_H4;
    int p1 = NofCandle - 1;
    int p2 = NofCandle;
    int p3 = NofCandle + 1;
    
    int NofHighLow = 0;
    double LastConditionPrice = 0;
    
    if (StrctStartDirection == "Bullish")
    {
        arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
        arrHighLow[1][NofHighLow][1] = p2;
        
        LastConditionPrice = iLow(Symbol(), StructureTFBar, p2);
        NofHighLow++;
        
        for (int i = NofCandle-1; i >= 2; i--)
        {
            switch (CheckCondition(p1, p2, p3))
            {
                case 17:
                
                    Print("Condition = 17 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    break;
                    
                case 28:

                    Print("Condition = 28 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p1--;
                    
                    break;
                    
                case 3:
                    if (LastConditionPrice != iHigh(Symbol(), StructureTFBar, p2))
                    {
                        arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                        arrHighLow[0][NofHighLow][1] = p2;
                        NofHighLow++;
                    }
                    
                    Print("Condition = 3 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                                        
                    
                    break;
                    
                case 41:
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                    arrHighLow[0][NofHighLow][1] = p2;
                    NofHighLow++;
                    
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                    arrHighLow[1][NofHighLow][1] = p1;
                    NofHighLow++;
                    
                    LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                    
                    Print("Condition = 41 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    
                    break;
                    
                case 42:
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                    arrHighLow[0][NofHighLow][1] = p1;
                    NofHighLow++;
                    LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                    
                    Print("Condition = 42 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    
                    break;
                    
                case 43:
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                    arrHighLow[0][NofHighLow][1] = p2;
                    NofHighLow++;
                    
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                    arrHighLow[1][NofHighLow][1] = p1;
                    NofHighLow++;
                    
                    LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);

                    Print("Condition = 43 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    
                    break;

                
                case 44:
                    // Retrieve and store High of the bar at index p2
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                    arrHighLow[0][NofHighLow][1] = p2;
                    NofHighLow++;
            
                    // Retrieve and store Low of the bar at index p2
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                    arrHighLow[1][NofHighLow][1] = p2;
                    NofHighLow++;
            
                    // Retrieve and store High of the bar at index p1
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                    arrHighLow[0][NofHighLow][1] = p1;
                    NofHighLow++;
            
                    // Update LastConditionPrice and other pointers
                    LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                    
                    Print("Condition = 44 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    break;
            
                case 569:
                    
                    Print("Condition = 569 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p2 = p1;
                    p1--;
                                        
                    break;
            
                case 1016:
                    
                    Print("Condition = 1016 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                                        
                    break;
            
                case 1117:

                    Print("Condition = 1117 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p1--;
                                        
                    break;
                
                case 12:
                
                    if (LastConditionPrice != iLow(Symbol(), StructureTFBar, p2))
                    {
                        arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                        arrHighLow[1][NofHighLow][1] = p2;
                        NofHighLow++;
                    }
                    
                    Print("Condition = 12 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                                        
                    break;
                    
                case 131:
                
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                    arrHighLow[1][NofHighLow][1] = p2;
                    NofHighLow++;
                    
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                    arrHighLow[0][NofHighLow][1] = p2;
                    NofHighLow++;
                    
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                    arrHighLow[1][NofHighLow][1] = p1;
                    NofHighLow++;
                    
                    LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                    
                    Print("Condition = 131 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p3 = p2;
                    p2 = p1;
                    p1--;
                    
                    
                    break;
                                       
                  case 132:
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                      arrHighLow[1][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p1);
                      arrHighLow[0][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iHigh(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 132 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                      
                      
                      break;
                  
                  case 133:
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p1);
                      arrHighLow[1][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iLow(NULL, StructureTFBar, p1);  
                      
                      Print("Condition = 133 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                  
                  case 134:
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                      arrHighLow[1][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p1);
                      arrHighLow[0][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iHigh(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 134 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                  
                  case 141518:
                  
                      Print("Condition = 141518 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p2 = p1;
                      p1--;
                      
                      break;
                  
                  case 191:
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                      arrHighLow[1][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p2);
                      arrHighLow[0][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p1);
                      arrHighLow[1][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iLow(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 191 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                  
                  case 192:
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                      arrHighLow[1][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p1);
                      arrHighLow[0][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iHigh(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 192 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                  
                  case 193:
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p2);
                      arrHighLow[0][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p1);
                      arrHighLow[1][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iLow(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 193 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                      
                      break;
                  
                  case 194:
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p2);
                      arrHighLow[0][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                      arrHighLow[1][NofHighLow][1] = p2;
                      NofHighLow++;
                  
                      arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p1);
                      arrHighLow[0][NofHighLow][1] = p1;
                      NofHighLow++;
                  
                      LastConditionPrice = iHigh(NULL, StructureTFBar, p1);
                      
                      Print("Condition = 194 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                  
                  case 201:
                  
                      Print("Condition = 201 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                      
                      break;
                      
                  case 202:
                      
                      if (LastConditionPrice != iHigh(Symbol(), StructureTFBar, p2))
                      {
                           arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p2);
                           arrHighLow[0][NofHighLow][1] = p2;
                           NofHighLow++;
                      }
                      
                      Print("Condition = 202 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                      
                  case 211:    ////////////////////// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                  
                      if (LastConditionPrice != iLow(Symbol(), StructureTFBar, p2))
                      {
                           Print("Umadddddddddddddddddddddddddddddddddddddd");
                           arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                           arrHighLow[1][NofHighLow][1] = p2;
                           NofHighLow++;
                      } 
                      
                      Print("Condition = 211 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                      
                  case 212:
                      
                      Print("Condition = 212 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                      p3 = p2;
                      p2 = p1;
                      p1--;
                                            
                      break;
                      
                  case 22:

                    Print("Condition = 22 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                    p1--;
                                        
                    break;    

            } // end of switch
        } // end of for loop
        
        Print("------------------ end : p1 = " + (string)p1 + " - p2 = " + (string)p2 + " - p3 = " + (string)p3);
        
    } // end of Bullish trend

//----------------- begin of Bearish Trend ----------------    
if (StrctStartDirection == "Bearish") 
{
        arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
        arrHighLow[0][NofHighLow][1] = p2;
        
        LastConditionPrice = iHigh(Symbol(), StructureTFBar, p2);
        NofHighLow++;
        
    for (int i = NofCandle-1; i >= 2; i--) 
    {
        switch (CheckCondition(p1, p2, p3)) 
        {
            case 17:
    
                Print("Condition = 17 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 28:
                
                Print("Condition = 28 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p1--;
                                
                break;

            case 3:
            
                if (LastConditionPrice != iHigh(Symbol(), StructureTFBar, p2))
                {
                    arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                    arrHighLow[0][NofHighLow][1] = p2;
                    NofHighLow++;
                }
                
                Print("Condition = 3 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;

            case 41:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 41 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;

            case 42:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 42 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 43:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 43 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;                
                
                break;

            case 44:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 44 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 569:

                Print("Condition = 569 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);            
                p2 = p1;
                p1--;    
            
                break;

            case 1016:
            
                Print("Condition = 1016 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 1117:
      
                Print("Condition = 1117 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p1--;
                                
                break;

            case 12:
            
                if (LastConditionPrice != iLow(Symbol(), StructureTFBar, p2))
                {
                    arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                    arrHighLow[1][NofHighLow][1] = p2;
                    NofHighLow++;
                }
                
                Print("Condition = 12 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 131:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 131 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;

            case 132:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 132 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;

            case 133:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 133 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                            
                break;

            case 134:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 134 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 141518:
            
                Print("Condition = 141518 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p2 = p1;
                p1--;
                                
                break;

            case 191:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 191 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 192:
            
                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 192 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;

            case 193:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p1);
                arrHighLow[1][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iLow(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 193 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;

            case 194:
            
                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p2);
                arrHighLow[0][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[1][NofHighLow][0] = iLow(Symbol(), StructureTFBar, p2);
                arrHighLow[1][NofHighLow][1] = p2;
                NofHighLow++;

                arrHighLow[0][NofHighLow][0] = iHigh(Symbol(), StructureTFBar, p1);
                arrHighLow[0][NofHighLow][1] = p1;
                NofHighLow++;
                
                LastConditionPrice = iHigh(Symbol(), StructureTFBar, p1);
                
                Print("Condition = 194 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;
                
            case 201:
            
                Print("Condition = 201 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                                
                break;
                
            case 202:
                
                if (LastConditionPrice != iHigh(Symbol(), StructureTFBar, p2))
                {
                     arrHighLow[0][NofHighLow][0] = iHigh(NULL, StructureTFBar, p2);
                     arrHighLow[0][NofHighLow][1] = p2;
                     NofHighLow++;
                }
                
                Print("Condition = 202 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;
                
            case 211:
            
                if (LastConditionPrice != iLow(Symbol(), StructureTFBar, p2))
                {
                     arrHighLow[1][NofHighLow][0] = iLow(NULL, StructureTFBar, p2);
                     arrHighLow[1][NofHighLow][1] = p2;
                     NofHighLow++;
                }
                
                Print("Condition = 211 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;
                
            case 212:
                
                Print("Condition = 212 - i = " + (string)i + " - p3 = " + (string)p3 + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p3 = p2;
                p2 = p1;
                p1--;
                
                break;
                
            case 22:

                Print("Condition = 22 - i = " + (string)i + " - p3 = " + (string)p3   + " - p2 = " + (string)p2 + " - p1 = " + (string)p1);
                p1--;
                                     
                break; 
                

        }
    }
}
    
    return 0;
}
//+--------------------------------------------------------+
//void DrawCircle(int CandleIndex, string HighLowType)
//{
//    // Ensure the data for the H4 timeframe is loaded
//    if (!RefreshRates())
//    {
//        Print("Error refreshing rates data.");
//        return;
//    }
//
//    // Get the open time and high/low price of the specified candle on H4 timeframe
//    datetime centerX = iTime(_Symbol, PERIOD_H4, CandleIndex); // Center time for the ellipse
//    double centerY = 0.0;
//
//    if (HighLowType == "high")
//    {
//        centerY = iHigh(_Symbol, PERIOD_H4, CandleIndex);
//    }
//    else
//    {
//        centerY = iLow(_Symbol, PERIOD_H4, CandleIndex);
//    }
//
//    // Define the ellipse width and height
//    double ellipseHeight = 0.001; // Adjustable height of the ellipse
//    double ellipseWidthMinutes = 1.2; // Width in minutes (adjustable)
//
//    // Calculate coordinates for the ellipse corners
//    datetime topLeftX = centerX - (datetime)(ellipseWidthMinutes * 60 / 2); // Left time (half width to the left)
//    double topLeftY = centerY + ellipseHeight / 2; // Top position (half height above center)
//
//    datetime bottomRightX = centerX + (datetime)(ellipseWidthMinutes * 60 / 2); // Right time (half width to the right)
//    double bottomRightY = centerY - ellipseHeight / 2; // Bottom position (half height below center)
//
//    // Ensure the ellipse's corners have correct ordering for drawing
//    if (topLeftX < bottomRightX)
//    {
//        string ellipseName = "Ellipse_" + IntegerToString(CandleIndex) + "_" + HighLowType;
//
//        // Draw the ellipse on the chart
//        ObjectCreate(0, ellipseName, OBJ_ELLIIPSE, 0, topLeftX, topLeftY);
//        ObjectSetInteger(0, ellipseName, OBJPROP_COLOR, clrYellow);
//        ObjectSetInteger(0, ellipseName, OBJPROP_WIDTH, 2);
//        ObjectSetInteger(0, ellipseName, OBJPROP_STYLE, STYLE_SOLID);
//        ObjectSetDouble(0, ellipseName, OBJPROP_PRICE2, bottomRightY);
//        ObjectSetInteger(0, ellipseName, OBJPROP_TIME2, bottomRightX);
//
//        Print("Ellipse drawn at time: ", TimeToString(centerX, TIME_DATE | TIME_MINUTES));
//    }
//}
//+-----------------------------------------------------+
void DrawCircle(int candle_index, string CandleType)
{
   ENUM_TIMEFRAMES StructureTFBar = PERIOD_H4;
   
   
   
   // Get the price based on CandleType (low or high)
   double price;
   if (CandleType == "low")
   {
      price = iLow(Symbol(), StructureTFBar, candle_index) - 45 * Point(); 
   }
   else
   {
      price = iHigh(Symbol(), StructureTFBar, candle_index) - 45 * Point(); 
   }
   
   // Get the time of the specified candle (for positioning the object)
   datetime candle_time = iTime(_Symbol, PERIOD_CURRENT, candle_index);

   // Create a unique name for the circle object
   string object_name;
   if (CandleType == "low")
   {
      object_name = "Circle_Low_" + IntegerToString(candle_index);
   }
   else
   {
      object_name = "Circle_High_" + IntegerToString(candle_index);
   }

   // Delete any existing object with the same name
   if(ObjectFind(0, object_name) >= 0)
      ObjectDelete(0, object_name);

   // Define a small offset to control the radius of the ellipse
   int time_offset = PeriodSeconds() / 96; // Adjust this for ellipse width
   double price_offset = 90 * Point();      // Adjust this for ellipse height

   datetime end_time = candle_time + time_offset; // Right edge of the ellipse
   double end_price = price + price_offset;       // Top edge of the ellipse

   // Create an ellipse with bounding points
   if(ObjectCreate(0, object_name, OBJ_ELLIPSE, 0, candle_time, price, end_time, end_price))
   {
      // Set circle properties
      ObjectSetInteger(0, object_name, OBJPROP_COLOR, clrYellow);        // Set the color of the circle
      ObjectSetInteger(0, object_name, OBJPROP_WIDTH, 2);             // Set the thickness of the circle border
      ObjectSetInteger(0, object_name, OBJPROP_STYLE, STYLE_SOLID);   // Set the line style of the circle
      
      Print("Circle created at time ", TimeToString(candle_time), " and price: ", DoubleToString(price, _Digits));
   }
   else
   {
      Print("Failed to create the circle on candle ", candle_index);
   }
}

//+-------------------------------------------------------
