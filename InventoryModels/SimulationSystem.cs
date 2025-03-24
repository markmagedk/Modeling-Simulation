using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.CodeDom;
using System.Collections.Specialized;
using System.Threading;

namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }   // M (el maximum value ely t2dary tshelyha fl ma5zan)
        public int ReviewPeriod { get; set; }   // N (el cycle kol kam youm ll orderat)
        public int NumberOfDays { get; set; }   // stopping conditions ll table
        public int StartInventoryQuantity { get; set; }  // bad2yn ba kam talaga fl awal khales
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }


        public void Read_File(string filename)
        {

            SimulationTable.Clear();
         
            string[] lines = File.ReadAllLines(@"C:\Users\lenovo\OneDrive\Documents\GitHub\Modeling-and-Simulation\InventorySimulation\TestCases\" + filename + ".txt");
            string l = "";

            foreach (string line in lines)
            {
                
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                if (trimmedLine == "OrderUpTo")
                    l = "OrderUpTo";
                else if (l == "OrderUpTo")
                {
                    OrderUpTo = Int32.Parse(trimmedLine);  
                    l = "";
                }
                else if (trimmedLine == "ReviewPeriod")
                    l = "ReviewPeriod";
                else if (l == "ReviewPeriod")
                {
                    ReviewPeriod = Int32.Parse(trimmedLine); 
                    l = "";
                }
                else if (trimmedLine == "StartInventoryQuantity")
                    l = "StartInventoryQuantity";
                else if (l == "StartInventoryQuantity")
                {
                    StartInventoryQuantity = Int32.Parse(trimmedLine);  
                    l = "";
                }
                else if (trimmedLine == "StartLeadDays")
                    l = "StartLeadDays";
                else if (l == "StartLeadDays")
                {
                    StartLeadDays = Int32.Parse(trimmedLine); 
                    l = "";
                }
                else if (trimmedLine == "StartOrderQuantity")
                    l = "StartOrderQuantity";
                else if (l == "StartOrderQuantity")
                {
                    StartOrderQuantity = Int32.Parse(trimmedLine); 
                    l = "";
                }
                else if (trimmedLine == "NumberOfDays")
                    l = "NumberOfDays";
                else if (l == "NumberOfDays")
                {
                    NumberOfDays = Int32.Parse(trimmedLine);  
                    Console.WriteLine($"NumberOfDays: {NumberOfDays}");  
                    l = "";
                }
                else if (trimmedLine == "DemandDistribution")
                {
                    break;
                }
                // END LOOP
            }

            Daily_Demand_Distribution_Table(lines);
            Lead_Time_Distribution_Table(lines);

            apply_simulation();

            calc_performing_mesaures();
        }




        public void Daily_Demand_Distribution_Table(string[] data)
        {
         
            string l = "";

            int c = 0;   

            for (int i = 0; i < data.Length; i++)
            {
                Distribution Demand_Table = new Distribution();
                if (data[i] == "DemandDistribution")
                {
                    l = "DemandDistribution";
                }
                else if (l == "DemandDistribution")
                {
                    if (data[i] == "")
                        break;
                    string[] split = data[i].Split(',');
                    Demand_Table.Value = Int32.Parse(split[0]);
                    Demand_Table.Probability = Decimal.Parse(split[1]);
                    if (c == 0)
                    {
                        Demand_Table.CummProbability = Demand_Table.Probability;
                        Demand_Table.MinRange = 1;
                        Demand_Table.MaxRange = Decimal.ToInt32(Demand_Table.Probability * 100);
                    }
                    else
                    {
                        Demand_Table.CummProbability = Demand_Table.Probability + DemandDistribution[c - 1].CummProbability;
                        Demand_Table.MinRange = Decimal.ToInt32(DemandDistribution[c - 1].CummProbability * 100) + 1;
                        Demand_Table.MaxRange = Decimal.ToInt32(Demand_Table.CummProbability * 100);
                    }
                    c++;
                    DemandDistribution.Add(Demand_Table);
                }
                //END LOOP
            }
            //END FUNCTION
        }

        public void Lead_Time_Distribution_Table(string[] data)
        {
 
            string l = "";

            int c = 0;       

            for (int i = 0; i < data.Length; i++)
            {
                Distribution Lead_Days_Table = new Distribution();

                if (data[i] == "LeadDaysDistribution")
                {
                    l = "LeadDaysDistribution";
                }
                else if (l == "LeadDaysDistribution")
                {
                    if (data[i] == "")
                        break;
                    string[] split = data[i].Split(',');
                    Lead_Days_Table.Value = Int32.Parse(split[0]);
                    Lead_Days_Table.Probability = Decimal.Parse(split[1]);
                    if (c == 0)
                    {
                        Lead_Days_Table.CummProbability = Lead_Days_Table.Probability;
                        Lead_Days_Table.MinRange = 1;
                        Lead_Days_Table.MaxRange = Decimal.ToInt32(Lead_Days_Table.Probability * 100);
                    }
                    else
                    {
                        Lead_Days_Table.CummProbability = Lead_Days_Table.Probability + LeadDaysDistribution[c - 1].CummProbability;
                        Lead_Days_Table.MinRange = Decimal.ToInt32(LeadDaysDistribution[c - 1].CummProbability * 100) + 1;
                        Lead_Days_Table.MaxRange = Decimal.ToInt32(Lead_Days_Table.CummProbability * 100);
                    }
                    c++;
                    LeadDaysDistribution.Add(Lead_Days_Table);
                }
                //END LOOP
            }
            //END FUNCTION
        }


        public void apply_simulation()
        {
            

            SimulationTable.Clear();

            for (int i = 0; i < NumberOfDays; i++)
            {
                SimulationTable.Add(new SimulationCase());
                SimulationTable[i].Day = i + 1;
                SimulationTable[i].DayWithinCycle = i % ReviewPeriod + 1;
                SimulationTable[i].Cycle = i / ReviewPeriod + 1;


            }

            
            calculating_full_simulation();
            

        }

        public void calculating_full_simulation()
        {
            Random random = new Random();
            int cnt = StartLeadDays - 1;
            int lastorder = StartOrderQuantity;

            for (int i = 0; i < NumberOfDays; i++)
            {
             
                //First Case

                if (i == 0)
                {
                   
                    SimulationTable[0].BeginningInventory = StartInventoryQuantity;

                    SimulationTable[0].RandomDemand = random.Next(1, 101);

                    for (int j = 0; j < DemandDistribution.Count; j++)
                    {
                        if (SimulationTable[0].RandomDemand >= DemandDistribution[j].MinRange && SimulationTable[0].RandomDemand <= DemandDistribution[j].MaxRange)
                        {
                            SimulationTable[0].Demand = DemandDistribution[j].Value;
                        }
                    }

                    if (SimulationTable[0].BeginningInventory < SimulationTable[0].Demand)
                    {
                        SimulationTable[0].EndingInventory = 0;
                        SimulationTable[0].ShortageQuantity = (SimulationTable[0].Demand - SimulationTable[0].BeginningInventory);
                    }
                    else
                    {
                        SimulationTable[0].EndingInventory = SimulationTable[0].BeginningInventory - SimulationTable[0].Demand;
                        SimulationTable[0].ShortageQuantity = 0;
                    }

                    SimulationTable[0].OrderQuantity = 0;
                    SimulationTable[0].RandomLeadDays = 0;
                    SimulationTable[0].LeadDays = 0;
                    SimulationTable[0].DaysUntilOrderArrives = cnt;
                    cnt--;



                }
                else{

                    if(cnt == -1)
                    {
                        SimulationTable[i].BeginningInventory = SimulationTable[i - 1].EndingInventory + lastorder;
                    }else
                    {
                        SimulationTable[i].BeginningInventory = SimulationTable[i - 1].EndingInventory;
                    }
                    

                    SimulationTable[i].RandomDemand = random.Next(1, 101);

                    for (int t = 0; t < DemandDistribution.Count; t++)
                    {
                        if (SimulationTable[i].RandomDemand >= DemandDistribution[t].MinRange && SimulationTable[i].RandomDemand <= DemandDistribution[t].MaxRange)
                        {
                            SimulationTable[i].Demand = DemandDistribution[t].Value;
                        }
                    }

                    if (SimulationTable[i].BeginningInventory <= SimulationTable[i].Demand)
                    {
                        SimulationTable[i].EndingInventory = 0;
                        SimulationTable[i].ShortageQuantity = (SimulationTable[i].Demand - SimulationTable[i].BeginningInventory) + SimulationTable[i - 1].ShortageQuantity;
                    }
                    else
                    {
                        if(SimulationTable[i].BeginningInventory - (SimulationTable[i].Demand + SimulationTable[i - 1].ShortageQuantity) < 0)
                        {
                            SimulationTable[i].EndingInventory = 0;
                            SimulationTable[i].ShortageQuantity = (SimulationTable[i].Demand + SimulationTable[i - 1].ShortageQuantity) - SimulationTable[i].BeginningInventory;
                        }
                        else
                        {
                            SimulationTable[i].EndingInventory = SimulationTable[i].BeginningInventory - (SimulationTable[i].Demand + SimulationTable[i - 1].ShortageQuantity);
                            SimulationTable[i].ShortageQuantity = 0;
                        }

                
                            
                            
           

                    }


                    if(SimulationTable[i].DayWithinCycle == ReviewPeriod)
                    {

                        SimulationTable[i].OrderQuantity = (OrderUpTo - SimulationTable[i].EndingInventory) + SimulationTable[i].ShortageQuantity;
                        lastorder = SimulationTable[i].OrderQuantity;
                        SimulationTable[i].RandomLeadDays = random.Next(1, 11);

                            //Lead Days
                            for (int p = 0; p < LeadDaysDistribution.Count; p++)
                            {
                                if (SimulationTable[i].RandomLeadDays >= LeadDaysDistribution[p].MinRange && SimulationTable[i].RandomLeadDays <= LeadDaysDistribution[p].MaxRange)
                                {
                                    SimulationTable[i].LeadDays = LeadDaysDistribution[p].Value;
                                }
                            }

                        SimulationTable[i].DaysUntilOrderArrives = SimulationTable[i].LeadDays;
                        cnt = SimulationTable[i].LeadDays - 1;
                       

                    }
                    else
                    {

                        SimulationTable[i].OrderQuantity = 0;
                        SimulationTable[i].RandomLeadDays = 0;
                        SimulationTable[i].LeadDays = 0;
                        if (cnt > 0)
                        {
                            SimulationTable[i].DaysUntilOrderArrives = cnt;
                            cnt--;
                        }
                        else
                        {
                            SimulationTable[i].DaysUntilOrderArrives = 0;
                            cnt--;
                        }

                    }


                }

                
                


            }
        }


        void calc_performing_mesaures()
        {
            decimal Inventory = 0;
            decimal Shortage = 0;

            for(int i = 0; i<NumberOfDays; i++)
            {
                Inventory += SimulationTable[i].EndingInventory;
                Shortage += SimulationTable[i].ShortageQuantity;
            }

            PerformanceMeasures.EndingInventoryAverage = Inventory / NumberOfDays;
            PerformanceMeasures.ShortageQuantityAverage = Shortage / NumberOfDays;

        }


    }
}
