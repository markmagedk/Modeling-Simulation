# 📦 Inventory Simulation System  

This project is a **C# console application** that simulates an inventory system. It helps manage stock levels, order timing, and demand variations using **probability distributions**.  

## ✨ Features  
✔️ Reads inventory settings from a file  
✔️ Uses **demand** and **lead time** probability distributions  
✔️ Simulates daily inventory changes  
✔️ Calculates **average ending inventory** and **average shortage quantity**  

## 📂 How It Works  
1. **Read Input File** 📄  
   - Loads inventory settings from a text file (like `OrderUpTo`, `ReviewPeriod`, etc.).  
   - Parses demand and lead time distributions.  

2. **Generate Simulation Table** 📊  
   - Simulates inventory changes for each day.  
   - Uses **random values** to determine demand and order arrival.  

3. **Calculate Performance Measures** 📈  
   - Computes the average ending inventory.  
   - Computes the average shortage quantity.  

## 📌 Key Classes  
- `SimulationSystem` → Manages the whole process.  
- `Distribution` → Stores probability data for demand & lead time.  
- `SimulationCase` → Represents daily inventory state.  
- `PerformanceMeasures` → Stores final performance results.  

## 🛠 How to Run  
1. Open the project in **Visual Studio**.  
2. Make sure the input file is in the correct folder (`TestCases/`).  
3. Run the program and check the output.  

## 📜 Example Input Format  
```
OrderUpTo  
100  
ReviewPeriod  
5  
StartInventoryQuantity  
50  
NumberOfDays  
30  
DemandDistribution  
10, 0.2  
20, 0.5  
30, 0.3  
LeadDaysDistribution  
1, 0.5  
2, 0.3  
3, 0.2  
```

## 📝 Notes  
- The simulation uses **random numbers**, so results may vary.  
- You can adjust the **inventory levels and demand probabilities** in the input file.
