using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoManage_3
{
    public class OrderHistory : Staff
    {
        private string filepath = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\OrderHistory.txt";
        public void orderHistory()
        {
            IreadFile(filepath);
            displayTable();
            Console.Write("Enter an entry that you want to search: ");
            string searchTerm = Console.ReadLine();
            searchOrders(searchTerm);
        }
        private void searchOrders(string searchTerm)
        {
            bool found = false;
            var table = new Table();
            table.AddColumns("[yellow]Reference Number[/]", "[yellow]Date[/]", "[yellow]Item ID[/]", "[yellow]Item Name[/]", "[yellow]Quantity[/]", "[yellow]Item Price[/]", "[yellow]Total Price[/]");

            foreach (var order in orders)
            {
                string refNum = order.Key;

                foreach (var item in order.Value)
                {
                    // Check if the search term matches any relevant fields
                    if (refNum.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        item.itemId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        item.itemName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        item.date.ToString("MM/dd/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        // Add all related rows in a loop
                        double totalPrice = item.quantity * item.itemPrice;
                        table.AddRow($"{refNum}", $"{item.date.ToString("MM/dd/yyyy")}", $"{item.itemId}", 
                            $"{item.itemName}", $"{item.quantity}", $"php {item.itemPrice:F2}", 
                            $"php {totalPrice:f2}");

                        found = true;
                    }
                }
            }

            if (found)
            {
                table.Alignment = Justify.Center;
                AnsiConsole.Write(table);
                Console.WriteLine("\nPress ANY key to continue");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"No matching results for '{searchTerm}'.");
                Console.WriteLine("\nPress ANY key to continue");
                Console.ReadKey();
            }

            orders.Clear();
        }


        private void displayTable()
        {
            var table = new Table();
            table.AddColumns("[yellow]Reference Number[/]", "[yellow]Date[/]", "[yellow]Item ID[/]", "[yellow]Item Name[/]", "[yellow]Quantity[/]", "[yellow]Item Price[/]", "[yellow]Total Price[/]");

            foreach (var order in orders) 
            {
                foreach (var item in order.Value) 
                {
                    double totalPrice = item.quantity * item.itemPrice;
                    table.AddRow($"{order.Key}", $"{item.date.ToString("MM/dd/yyyy")}", $"{item.itemId}", $"{item.itemName}", $"{item.quantity}", $"{item.itemPrice:F2}", $"{totalPrice:F2}");
                }
            }
            table.Alignment = Justify.Center;
            AnsiConsole.Write(table);
        }

    }
}
