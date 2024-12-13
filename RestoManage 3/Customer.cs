using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoManage_3
{
    public class Customer : Inventory
    {
        //Salesss reporttt
        private string filePathB = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\OrderHistory.txt";
        public string FilePathB
        {
            get { return filePathB; }
            set { filePathB = value; }
        }

        //Dashboard
        private string filePathC = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\Dashboard.txt";
        public string FilePathC
        {
            get { return filePathC; }
            set { filePathC = value; }
        }

        Dictionary<int, int> orderedItems = new Dictionary<int, int>();
        public void customer()
        {
            int itemId;
            int quantToBuy; //quantity to buy
            double recievedCash;
            double GrandTotal = 0;
            double change;
            

            Console.Clear();
            
            viewMenu();
            while (true)
            {
                Console.Write("Enter the Item Id you want to buy: ");
                if(!int.TryParse(Console.ReadLine(), out itemId) || !inventory.ContainsKey(itemId))
                {
                    Console.WriteLine("Invalid Item ID. Please try again.");
                    Console.WriteLine("\nPress ANY key to continue.\n");
                    Console.ReadKey();
                    continue;
                }
                if (inventory[itemId].quantity <= 0)
                {
                    Console.WriteLine("Item is unavailable. Please try again.");
                    Console.WriteLine("\nPress ANY key to continue.\n");
                    Console.ReadKey();
                    continue;
                }
                
                while (true)
                {
                    Console.Write("How many would you like to buy: ");
                    if (!int.TryParse(Console.ReadLine(), out quantToBuy) || quantToBuy <= 0)
                    {
                        Console.WriteLine($"\nInvalid quantity. Please enter a positive number.\nPlease try again");
                        Console.WriteLine("\nPress ANY key to continue.\n");
                        Console.ReadKey();
                        continue;
                    }
                    else if(quantToBuy > inventory[itemId].quantity)
                    {
                        Console.WriteLine($"\nNot enough quantity in the inventory. Please enter less than or equal to [{inventory[itemId].quantity}].\nPlease try again");
                        Console.WriteLine("\nPress ANY key to continue.\n");
                        Console.ReadKey();
                        continue;
                    }
                    else 
                    {
                        GrandTotal += grandTotal(quantToBuy, itemId);
                    }

                    break;
                }

                    var selectedIndex = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Would you like to buy again?\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                            .PageSize(10)
                            .AddChoices(new[] {
                                "Continue", "Exit"
                    }));
                if(selectedIndex == "Exit")
                {
                    while (true)
                    {
                        AnsiConsole.Write(new Markup($"The total of your purchase is [yellow]{GrandTotal}[/] enter the ammount of your payment: "));
                        if (!double.TryParse(Console.ReadLine(), out recievedCash) || recievedCash <= 0 || recievedCash < GrandTotal)
                        {
                            Console.WriteLine($"\nInvalid ammount. Please enter a positive number.\nPlease try again");
                            Console.WriteLine("\nPress ANY key to continue.\n");
                            Console.ReadKey();
                            continue;
                        }
                        else //change or ang sukli
                        {
                            change = recievedCash - GrandTotal;
                            break;
                        }
                    }
                    break;
                }
            }

            string refNumber = referenceNumber();
            receipt(GrandTotal, recievedCash, change, refNumber);
            writeHistoryAndSales(refNumber);
            updtInvntryDictionary();
            inventory.Clear();
            orderedItems.Clear();
        }

        private double grandTotal(int quantityToBuy, int itemId)
        {
            double itemPrice;
            double grandtotal = 0;

            if (inventory.TryGetValue(itemId, out var item)) 
            {
                itemPrice = item.itemPrice;
                // Update orderedItems to store quantities for multiple purchases
                if (orderedItems.ContainsKey(itemId))
                {
                    orderedItems[itemId] += quantityToBuy;  //If item exists, add quantity
                }
                else
                {
                    orderedItems[itemId] = quantityToBuy;  //If item is new, initialize with quantity
                }

                grandtotal += quantityToBuy * itemPrice;  //Calculate the total for the selected item
            }
            else
            {
                Console.WriteLine("Item ID not found in menu.");
                Console.Write("\nPress ANY key to continue");
                Console.ReadKey();
            }


            return grandtotal;
        }

        private void receipt(double grandtotal, double recievedCash, double change, string referenceNumber)
        {
            Console.Clear();
            DateTime time = DateTime.Now;
            string formattedDate = time.ToString("MM/dd/yyyy");

            var table = new Table();

            AnsiConsole.Write(new Markup("\n[gold1 bold]Thank you for purchasing![/]\n").Centered());
            table.Title(new TableTitle($"[gray]Reference #: {referenceNumber}   Date: {formattedDate}[/]"));
            table.AddColumns("[yellow]Item[/]", "[yellow]Qty[/]", "[yellow]Price[/]", "[yellow]Total[/]");
            
            foreach (var input in orderedItems) 
            {
                if (inventory.TryGetValue(input.Key, out var item))
                {
                    double totalPrice = input.Value * item.itemPrice;
                    table.AddRow($"{item.itemName}", $"{input.Value}", $"Php {item.itemPrice:F2}", $"Php {totalPrice:F2}");
                }
            }
            table.AddEmptyRow();
            table.AddRow("[yellow bold]Grand Total[/]", "", "", $"Php {grandtotal:F2}");
            table.AddRow("[yellow bold]Payment Received[/]", "", "", $"Php {recievedCash:F2}");
            table.AddRow("[yellow bold]Change[/]", "", "", $"Php {change:F2}");

            table.Alignment = Justify.Center;
            table.RoundedBorder();
            table.Border = TableBorder.Horizontal;

            AnsiConsole.Write(table);

            Console.WriteLine("\nPress ANY key to continue");
            Console.ReadKey();
        }

        private string referenceNumber()
        {
            string referenceNumber = DateTime.Now.ToString("yyMMdd") + "-" + new Random().Next(1000, 9999);
            return referenceNumber;
        }

        private void updtInvntryDictionary() //update inventory dictionary
        {
            foreach (var order in orderedItems)
            {
                int itemId = order.Key;
                int quantityToBuy = order.Value;

                
                if (inventory.TryGetValue(itemId, out var currentInventory))
                {
                    inventory[itemId] = (currentInventory.itemName, currentInventory.quantity - quantityToBuy, currentInventory.itemPrice);
                }
            }
            writeFile(filePathA);
        }

        private void viewMenu()
        {
            IreadFile(filePathA);

            var table = new Table();

            table.Centered();

            table.Title(new TableTitle("[green bold]Menu[/]"));

            //MAIN DISH
            table.AddColumns("[yellow]Item Id[/]", "[yellow]Item Name[/]", "[yellow]Item Price[/]");
            table.AddRow("[blue italic]Main Dish[/]");

            foreach (var item in inventory)
            {
                if (item.Key >= 101 && item.Key <= 199)
                {
                    if(item.Value.quantity <= 0)
                    {
                        table.AddRow($"[grey dim]{item.Key}[/]", $"[grey dim]{item.Value.itemName}[/]", $"[grey dim]php {item.Value.itemPrice:F2}[/]");
                    }
                    else
                    {
                        table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"php {item.Value.itemPrice:F2}");
                    }
                }
                    
            }

            //APPETIZERS
            table.AddRow("[blue italic]Appetizer[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 201 && item.Key <= 299)
                {
                    if (item.Value.quantity <= 0)
                    {
                        table.AddRow($"[grey dim]{item.Key}[/]", $"[grey dim]{item.Value.itemName}[/]", $"[grey dim]php {item.Value.itemPrice:F2}[/]");
                    }
                    else
                    {
                        table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"php {item.Value.itemPrice:F2}");
                    }
                }
            }

            //BEVERAGES
            table.AddRow("[blue italic]Beverages[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 301 && item.Key <= 399)
                {
                    if (item.Value.quantity <= 0)
                    {
                        table.AddRow($"[grey dim]{item.Key}[/]", $"[grey dim]{item.Value.itemName}[/]", $"[grey dim]php {item.Value.itemPrice:F2}[/]");
                    }
                    else
                    {
                        table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"php {item.Value.itemPrice:F2}");
                    }
                }
            }

            //DESSERTS
            table.AddRow("[blue italic]Desserts[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 401 && item.Key <= 499)
                {
                    if (item.Value.quantity <= 0)
                    {
                        table.AddRow($"[grey dim]{item.Key}[/]", $"[grey dim]{item.Value.itemName}[/]", $"[grey dim]php {item.Value.itemPrice:F2}[/]");
                    }
                    else
                    {
                        table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"php {item.Value.itemPrice:F2}");
                    }
                }
            }

            AnsiConsole.Write(table);
        }

        private void writeHistoryAndSales(string referenceNumber)
        {
            DateTime date = DateTime.Now;
            string formattedDate = date.ToString("MM/dd/yyyy");

            //Dashboard
            using (StreamWriter writer = new StreamWriter(FilePathC, append: true))
            {
                foreach (var input in orderedItems)
                {
                    if(inventory.TryGetValue(input.Key, out var item))
                    {
                        writer.WriteLine($"{referenceNumber}|{formattedDate}|{input.Key}|{item.itemName}|{input.Value}|{item.itemPrice:F2}");
                    } 
                }

                writer.Flush();
            }

            //SalesReport
            using (StreamWriter writer = new StreamWriter(FilePathB, append: true))
            {
                foreach (var input in orderedItems)
                {
                    if (inventory.TryGetValue(input.Key, out var item))
                    {
                        writer.WriteLine($"{referenceNumber}|{formattedDate}|{input.Key}|{item.itemName}|{input.Value}|{item.itemPrice:F2}");
                    }
                }

                writer.Flush();
            }
        }

    }
}
