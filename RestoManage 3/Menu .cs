using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;

namespace RestoManage_3
{
    public enum Categories
    {
        MainDish = 100,   //ID range for Main Dish (100-199)
        Appetizers = 200, //ID range for Appetizers (200-299)
        Beverages = 300,  //ID range for Beverages (300-399)
        Desserts = 400    //ID range for Desserts (400-499)
    }
    public class Menu : Inventory
    {
        public void OptionsForAddMenu()
        {
            Console.Clear();
            string selectedIndex;
            do
            {
                selectedIndex = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose a category to add items:\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                        //.PageSize(10)
                        .AddChoices("Main Dish", "Appetizers", "Beverages", "Desserts", "Exit")
                );

                switch (selectedIndex)
                {
                    case "Main Dish":
                        WriteMenuItems(Categories.MainDish);
                        break;
                    case "Appetizers":
                        WriteMenuItems(Categories.Appetizers);
                        break;
                    case "Beverages":
                        WriteMenuItems(Categories.Beverages);
                        break;
                    case "Desserts":
                        WriteMenuItems(Categories.Desserts);
                        break;
                    case "Exit":
                        break;
                    default:
                        Console.Write("Invalid input\nPress ANY key to continue");
                        Console.ReadKey();
                        break;
                }
            }   while (selectedIndex != "Exit");
        }

        private void WriteMenuItems(Categories category)
        {
            Console.Clear();
            IreadFile(filePathA); // Load existing inventory

            IdGenerator idGenerator = new IdGenerator();

            AnsiConsole.Markup($"[gold1]You have chosen {category}[/]\n");
            AnsiConsole.Markup("[yellow dim](Item Name) - (itemPrice)[/] e.g. [yellow]Banana - 10[/]\n");
            AnsiConsole.Markup("When finished inputting, write [green]done[/]\n");

            string done = "done";
            string input;

            while (true)
            {
                Console.Write("\nEnter Item Details: ");
                input = Console.ReadLine();

                if (input.ToLower() == done)
                    break;

                string[] items = input.Split("-");
                if (items.Length == 2)
                {
                    try
                    {
                        string itemName = items[0].Trim();
                        double itemPrice = double.Parse(items[1].Trim());

                        // Check if the item already exists by name (manual iteration)
                        bool itemExists = false;
                        foreach (var entry in inventory)
                        {
                            if (entry.Value.Item1.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                            {
                                itemExists = true;
                                break;
                            }
                        }

                        if (itemExists)
                        {
                            Console.WriteLine($"Item '{itemName}' already exists in the inventory. Skipping.");
                            continue;
                        }

                        // Generate a new ID and add the item
                        int itemId = idGenerator.GenerateItemID(category, inventory);
                        inventory[itemId] = (itemName, 0, itemPrice);
                        Console.WriteLine($"Item '{itemName}' added successfully with ID {itemId}!");
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid price format. Please enter a numeric value for price.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Format. Use (Item Name) - (itemPrice).");
                }
            }

            // Write updated inventory to file
            writeFile(filePathA);

            // Reload inventory and display it
            IreadFile(filePathA);
            DisplayInventory();

            Console.WriteLine("Press ANY key to continue.");
            Console.ReadKey();
        }

        public void RemoveMenuItem()
        {
            Console.Clear();
            IreadFile(filePathA);
            DisplayInventory();

            while (true)
            {
                Console.Write("Enter the ID number you want to remove from Inventory: ");
                if (!int.TryParse(Console.ReadLine(), out int itemId) || !inventory.ContainsKey(itemId))
                {
                    Console.WriteLine("Invalid Item ID. Please try again.");
                    Console.WriteLine("Press ANY key to continue.");
                    Console.ReadKey();
                    continue;
                }

                inventory.Remove(itemId);
                writeFile(filePathA);

                Console.Clear();
                IreadFile(filePathA);
                DisplayInventory();

                Console.WriteLine("Item removed successfully!");
                Console.WriteLine("Press ANY key to continue.");
                Console.ReadKey();
                break;
            }
        }
    }

    public class IdGenerator
    {
        private Dictionary<Categories, int> categoryCounter = new Dictionary<Categories, int>
        {
            { Categories.MainDish, 1 },
            { Categories.Appetizers, 1 },
            { Categories.Beverages, 1 },
            { Categories.Desserts, 1 }
        };

        public int GenerateItemID(Categories category, Dictionary<int, (string, int, double)> inventory)
        {
            int baseID = (int)category;

            // Find the current highest key for the given category
            int maxID = baseID;
            foreach (var key in inventory.Keys)
            {
                if (key >= baseID && key < baseID + 100)
                {
                    maxID = Math.Max(maxID, key);
                }
            }

            if (maxID >= baseID + 99)
            {
                throw new InvalidOperationException("Category limit exceeded for item IDs.");
            }

            return maxID + 1; // Return the next sequential ID
        }
    }
}
