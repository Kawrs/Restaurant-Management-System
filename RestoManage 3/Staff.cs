using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RestoManage_3
{
    public class Staff : IReadFile
    {
        // Dashboard file path
        protected string filePath = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\Dashboard.txt";

        // Dictionary to store orders
        protected Dictionary<string, List<(DateTime date, int itemId, string itemName, int quantity, double itemPrice)>> orders = new();

        // Read the file and load data into the dictionary
        public void IreadFile(string filepath)
        {
            if (!File.Exists(filepath) || IsFileEmpty(filepath))
            {
                Console.WriteLine("The file is empty or it does not exist.");
                Console.Write("Press ANY key to continue");
                Console.ReadKey();
                return;
            }

            orders.Clear();
            using (StreamReader reader = new(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 6)
                    {
                        string refNum = parts[0].Trim();
                        DateTime date = DateTime.ParseExact(parts[1].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        int itemId = int.Parse(parts[2].Trim());
                        string itemName = parts[3].Trim();
                        int quantity = int.Parse(parts[4].Trim());
                        double itemPrice = double.Parse(parts[5].Trim());

                        if (!orders.ContainsKey(refNum))
                        {
                            orders[refNum] = new List<(DateTime, int, string, int, double)>();
                        }
                        orders[refNum].Add((date, itemId, itemName, quantity, itemPrice));
                    }
                }
            }

            CombineDuplicateKeys();
            Console.WriteLine("Orders loaded successfully.");
        }

        // Write the dictionary data back to the file
        void writeFile(string filepath) //for staff
        {
            using StreamWriter writer = new(filepath, append: false);
            foreach (var order in orders)
            {
                foreach (var item in order.Value)
                {
                    writer.WriteLine($"{order.Key}|{item.date:MM/dd/yyyy}|{item.itemId}|{item.itemName}|{item.quantity}|{item.itemPrice:F2}");
                }
            }

            Console.WriteLine("Dashboard updated successfully.");
        }

        // Combine duplicate dictionary keys by merging their values
        void CombineDuplicateKeys()
        {
            var combinedOrders = new Dictionary<string, List<(DateTime, int, string, int, double)>>();

            foreach (var order in orders)
            {
                if (!combinedOrders.ContainsKey(order.Key))
                {
                    combinedOrders[order.Key] = new List<(DateTime, int, string, int, double)>();
                }

                combinedOrders[order.Key].AddRange(order.Value);
            }

            // Update the original dictionary
            orders = combinedOrders;
        }

        // Display the orders in a formatted table
        public void DisplayTable()
        {
            IreadFile(filePath);

            var table = new Table();
            table.Title = new TableTitle("Dashboard");
            table.AddColumns("Reference Number", "Item ID", "Item Name", "Quantity");

            foreach (var order in orders)
            {
                foreach (var item in order.Value)
                {
                    table.AddRow(order.Key, item.itemId.ToString(), item.itemName, item.quantity.ToString());
                }
            }

            table.Alignment = Justify.Center;
            AnsiConsole.Write(table);
        }

        // Delete an order by reference number
        public void deleteOrder()
        {
            if (!File.Exists(filePath) || IsFileEmpty(filePath))
            {
                Console.WriteLine("The file is empty or it does not exist.");
                Console.Write("Press ANY key to continue");
                Console.ReadKey();
                return;
            }
            while (true)
            {
                Console.Write("Enter the Reference Number to delete an order: ");
                string refNum = Console.ReadLine();
                if (!orders.ContainsKey(refNum))
                {
                    Console.WriteLine("Invalid Reference Number. Please input a correct Reference Number.");
                    Console.Write("\nPress ANY key to continue");
                    Console.ReadKey();
                }
                else
                {
                    orders.Remove(refNum);
                    writeFile(filePath);
                    Console.WriteLine("Order deleted successfully.");
                    Console.Write("\nPress ANY key to continue");
                    Console.ReadKey();
                    break;
                }
            }
        }

        // Utility method to check if the file is empty
        private bool IsFileEmpty(string filepath)
        {
            return new FileInfo(filepath).Length == 0;
        }
    }
}
