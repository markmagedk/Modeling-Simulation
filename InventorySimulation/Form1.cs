using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryTesting;
using System.IO;
using InventoryModels;

namespace InventorySimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
            

        }
        SimulationSystem system;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string test_name = comboBox1.SelectedItem.ToString();

            dataGridView1.Rows.Clear();

            system = new SimulationSystem();

            system.Read_File(test_name);
            
            for (int i = 0; i < system.NumberOfDays; i++)
            {
                string[] row = {system.SimulationTable[i].Day.ToString(),system.SimulationTable[i].Cycle.ToString(),
                               system.SimulationTable[i].DayWithinCycle.ToString(),system.SimulationTable[i].BeginningInventory.ToString(),
                               system.SimulationTable[i].RandomDemand.ToString(),system.SimulationTable[i].Demand.ToString(),
                               system.SimulationTable[i].EndingInventory.ToString(), system.SimulationTable[i].ShortageQuantity.ToString(),
                               system.SimulationTable[i].OrderQuantity.ToString(),system.SimulationTable[i].RandomLeadDays.ToString(),
                               system.SimulationTable[i].LeadDays.ToString(),system.SimulationTable[i].DaysUntilOrderArrives.ToString()
                };

                dataGridView1.Rows.Add(row);
            }

            
            string result = TestingManager.Test(system, test_name+".txt");
            MessageBox.Show(result);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = system.PerformanceMeasures.EndingInventoryAverage.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = system.PerformanceMeasures.ShortageQuantityAverage.ToString();
        }

    }
}
