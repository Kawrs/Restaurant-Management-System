using Spectre.Console;

namespace RestoManage_3
{
    public interface IReadFile
    {
        void IreadFile(string filepath);
    }
    public class Inventory : IReadFile
    {
        protected string filePathA = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\Inventory.txt";
        public Dictionary<int, (string itemName, int quantity, double itemPrice)> inventory = new Dictionary<int, (string, int, double)>();

        public void StartInventory()
        {
            IreadFile(filePathA);

            while (true)
            {
                Console.Clear();
                DisplayInventory();

                var selectedIndex = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option:\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                        .AddChoices("Add Quantity", "Remove Quantity", "Edit Item Name", "Edit Item Price", "Exit"));

                if (selectedIndex == "Exit")
                    break;

                int itemId = GetValidItemId();

                switch (selectedIndex)
                {
                    case "Add Quantity":
                        int addQuantity = GetPositiveInteger("Enter the quantity to add: ");
                        inventory[itemId] = (inventory[itemId].itemName, inventory[itemId].quantity + addQuantity, inventory[itemId].itemPrice);
                        break;

                    case "Remove Quantity":
                        int removeQuantity = GetPositiveInteger("Enter the quantity to remove: ");
                        inventory[itemId] = (inventory[itemId].itemName, Math.Max(0, inventory[itemId].quantity - removeQuantity), inventory[itemId].itemPrice);
                        break;

                    case "Edit Item Name":
                        Console.Write("Enter the new item name: ");
                        inventory[itemId] = (Console.ReadLine(), inventory[itemId].quantity, inventory[itemId].itemPrice);
                        break;

                    case "Edit Item Price":
                        double newPrice = GetPositiveDouble("Enter the new item price: ");
                        inventory[itemId] = (inventory[itemId].itemName, inventory[itemId].quantity, newPrice);
                        break;
                }

                writeFile(filePathA);
            }
        }
        public void IreadFile(string filepath)
        {
            if (!File.Exists(filepath) || IsFileEmpty(filepath)) //checks if file is empty or if it does not exists
            {
                Console.WriteLine("The file is empty or it does not exists");
                Console.Write("Press ANY key to continue");
                Console.ReadKey();
                return;
            }

            inventory.Clear();
            using (StreamReader reader = new(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('-');
                    if (parts.Length >= 4)
                    {
                        int itemId = int.Parse(parts[0].Trim());
                        string itemName = parts[1].Trim();
                        int quantity = int.Parse(parts[2].Trim());
                        double itemPrice = double.Parse(parts[3].Trim());

                        if (!inventory.ContainsKey(itemId)) //checking if it has the same itemId
                        {
                            inventory.Add(itemId, (itemName, quantity, itemPrice));
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }

            Console.WriteLine("Inventory loaded successfully");
        }

        public void writeFile(string filepath)
        {
            using StreamWriter writer = new(filepath, append: false);
            foreach (var item in inventory)
            {
                writer.WriteLine($"{item.Key}-{item.Value.itemName}-{item.Value.quantity}-{item.Value.itemPrice:F2}");
            }

            inventory.Clear();
            Console.WriteLine("Item added successfully to the Inventory.");
        }

        public static bool IsFileEmpty(string filePath)
        {
            return new FileInfo(filePath).Length == 0;
        }

        public void DisplayInventory()
        {
            IreadFile(filePathA);

            var table = new Table();

            table.Centered();

            table.Title(new TableTitle("[green bold]Menu[/]"));

            //MAIN DISH
            table.AddColumns("[yellow]Item Id[/]", "[yellow]Item Name[/]", "[yellow]QTY[/]", "[yellow]Item Price[/]");
            table.AddRow("[blue italic]Main Dish[/]");

            foreach (var item in inventory)
            {
                if (item.Key >= 101 && item.Key <= 199)
                {
                    table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"{item.Value.quantity}", $"php {item.Value.itemPrice:F2}");
                }

            }

            //APPETIZERS
            table.AddRow("[blue italic]Appetizer[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 201 && item.Key <= 299)
                {
                    table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"{item.Value.quantity}", $"php {item.Value.itemPrice:F2}");
                }
            }

            //BEVERAGES
            table.AddRow("[blue italic]Beverages[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 301 && item.Key <= 399)
                {
                    table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"{item.Value.quantity}", $"php {item.Value.itemPrice:F2}");
                }
            }

            //DESSERTS
            table.AddRow("[blue italic]Desserts[/]");
            foreach (var item in inventory)
            {
                if (item.Key >= 401 && item.Key <= 499)
                {
                    table.AddRow($"{item.Key}", $"{item.Value.itemName}", $"{item.Value.quantity}", $"php {item.Value.itemPrice:F2}");
                }
            }

            AnsiConsole.Write(table);
        }

        private int GetValidItemId()
        {
            while (true)
            {
                Console.Write("Enter the Item ID: ");
                if (int.TryParse(Console.ReadLine(), out int itemId) && inventory.ContainsKey(itemId))
                    return itemId;

                Console.WriteLine("Invalid Item ID. Please try again.\nPress ANY key to continue");
                Console.ReadKey();
            }
        }

        private int GetPositiveInteger(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                    return value;

                Console.WriteLine("Invalid input. Please enter a positive number.");
                Console.Write("Press ANY key to continue");

                Console.ReadKey();
            }
        }

        private double GetPositiveDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double value) && value > 0)
                    return value;

                Console.WriteLine("Invalid input. Please enter a positive number.");
                Console.Write("Press ANY key to continue");
                Console.ReadKey();
            }
        }
    }
}
