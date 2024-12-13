using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestoManage_3
{
    public abstract class SalesReport : IReadFile
    {
        protected List<(DateTime salesDate, int itemId, string itemName, int quantity, double pricePerItem)> sales;
        protected List<(DateTime Date, int ItemId, string ItemName, int Quantity, double PricePerItem)> filteredSales;
        protected Dictionary<(int itemId, string itemName), (int totalQuantity, double totalSales)> filteredDuplicates;

        public SalesReport()
        {
            sales = new List<(DateTime, int, string, int, double)>();
            filteredSales = new List<(DateTime, int, string, int, double)>();
            filteredDuplicates = new Dictionary<(int, string), (int, double)>();
        }

        // Abstract method to generate the sales report
        public abstract void GenerateSales();

        public static bool IsFileEmpty(string filePath) //checking if there are inputs inside the file
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length == 0;
        }

        public void IreadFile(string filePath) //reads file for OrderHistory
        {
            DateTime salesDate = DateTime.MinValue;

            if (!File.Exists(filePath) || IsFileEmpty(filePath))
            {
                Console.WriteLine("The file is empty or doesn't exist.");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    char[] separators = { '|' };
                    string[] output = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    if (output.Length != 6)
                    {
                        Console.WriteLine("Invalid file format");
                        Console.ReadKey();
                    }

                    try
                    {
                        salesDate = DateTime.ParseExact(output[1].Trim(), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        int itemId = int.Parse(output[2].Trim());
                        string itemName = output[3].Trim();
                        int quantity = int.Parse(output[4].Trim());
                        double pricePerItem = double.Parse(output[5].Trim());

                        sales.Add((salesDate, itemId, itemName, quantity, pricePerItem));
                    }
                    catch (Exception e)
                    {
                        Console.Write($"Skipping invalid data in line: {line}\nError: {e.Message}");
                    }
                }
            }
        }

        // Shared methods for filtering and generating tables
        protected void FilterSales()
        {
            foreach (var item in filteredSales)
            {
                var key = (item.ItemId, item.ItemName);
                if (filteredDuplicates.ContainsKey(key))
                {
                    var current = filteredDuplicates[key];
                    filteredDuplicates[key] = (
                        current.totalQuantity + item.Quantity,current.totalSales + (item.Quantity * item.PricePerItem));
                }
                else
                {
                    filteredDuplicates[key] = (item.Quantity, item.Quantity * item.PricePerItem);
                }
            }
        }

        protected void GenerateTable(string title)
        {
            double grandTotalSales = 0;
            var table = new Table();

            table.Title(new TableTitle($"[gold1 bold]Sales for {title}[/]"));
            table.AddColumns("[yellow]Item Id[/]", "[yellow]Item Name[/]", "[yellow]Qty Sold[/]", "[yellow]Item Price[/]", "[blue]Total Sale[/]");

            foreach (var entry in filteredDuplicates)
            {
                var (itemId, itemName) = entry.Key;
                var (totalQuantity, totalSales) = entry.Value;
                grandTotalSales += totalSales;

                double pricePerItem = totalSales / totalQuantity;
                table.AddRow($"{itemId}", $"{itemName}", $"{totalQuantity}", $"php {pricePerItem:F2}", $"php {totalSales:F2}");
            }

            table.AddRow("", "", "", "[blue]Total Sales[/]", $"[steelblue1]php {grandTotalSales:F2}[/]");
            table.Alignment = Justify.Center;
            AnsiConsole.Write(table);

            Console.Write("\nPress ANY key to continue");
            Console.ReadKey();
        }
    }

    // Derived class for daily sales report
    public class DailySalesReport : SalesReport
    {
        private readonly DateTime date;

        string filePath = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\OrderHistory.txt";
        public DailySalesReport(DateTime date)
        {
            this.date = date;
        }

        public override void GenerateSales()
        {
            IreadFile(filePath);
            string title = date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);

            filteredSales = sales
                .Where(s => s.salesDate.Date == date.Date)
                .ToList();

            if (filteredSales.Count > 0)
            {
                FilterSales();
                GenerateTable(title);
            }
            else
            {
                AnsiConsole.Write(new Markup($"No sales found for [yellow]{title}[/]"));
                Console.Write("\nPress ANY key to continue");
                Console.ReadKey();
            }

            sales.Clear();
            filteredSales.Clear();
            filteredDuplicates.Clear();
        }
    }

    // Derived class for weekly sales report
    public class WeeklySalesReport : SalesReport
    {
        private DateTime startOfWeek, endOfWeek;

        int month, year;

        string title;
        string filePath = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\OrderHistory.txt";
        public WeeklySalesReport(int Year, int Month)
        {
            this.year = Year;
            this.month = Month;
        }

        public override void GenerateSales()
        {
            IreadFile(filePath);
            calculateWeek();

            string title = $"{startOfWeek:MMMM dd, yyyy} - {endOfWeek:MMMM dd, yyyy}";

            filteredSales = sales
                .Where(s => s.salesDate >= startOfWeek && s.salesDate <= endOfWeek)
                .ToList();

            if (filteredSales.Count > 0)
            {
                FilterSales();
                GenerateTable(title);
            }
            else
            {
                AnsiConsole.Write(new Markup($"No sales found for [yellow]{title}[/]"));
                Console.Write("\nPress ANY key to continue");
                Console.ReadKey();
            }

            sales.Clear();
            filteredSales.Clear();
            filteredDuplicates.Clear();
        }

        public void calculateWeek()
        {
            int week;

            // Define the start and end of the selected month
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Define the first week's start as the first day of the month
            DateTime firstWeekStart = firstDayOfMonth;
            DateTime firstWeekEnd = firstWeekStart.AddDays(6);

            // Ensure the first week's end doesn't go beyond the month's last day
            if (firstWeekEnd > lastDayOfMonth)
                firstWeekEnd = lastDayOfMonth;

            // Calculate subsequent weeks
            List<(DateTime start, DateTime end)> weeks = new List<(DateTime, DateTime)>();
            weeks.Add((firstWeekStart, firstWeekEnd));

            DateTime currentStart = firstWeekEnd.AddDays(1);

            while (currentStart <= lastDayOfMonth)
            {
                DateTime currentEnd = currentStart.AddDays(6);
                if (currentEnd > lastDayOfMonth)
                    currentEnd = lastDayOfMonth;

                weeks.Add((currentStart, currentEnd));
                currentStart = currentEnd.AddDays(1);
            }

            Console.WriteLine($"The selected month has {weeks.Count} week(s).");

            // Prompt for the week selection
            while(true)
            {
                AnsiConsole.Write(new Markup($"Enter the Week (1-{weeks.Count}) you would like to view sales for: "));
                if (!int.TryParse(Console.ReadLine(), out week) || week <= 0 || week > weeks.Count)
                {
                    Console.WriteLine($"Invalid Week. Please select a week between 1 and {weeks.Count}.");
                    Console.Write("\nPress ANY key to continue");
                    Console.ReadKey();
                    continue;
                }
                break;
            }
            

            // Assign the selected week's start and end dates
            startOfWeek = weeks[week - 1].start;
            endOfWeek = weeks[week - 1].end;

            title = $"{startOfWeek:MMMM dd, yyyy} - {endOfWeek:MMMM dd, yyyy}";
        }

    }

    // Derived class for monthly sales report
    public class MonthlySalesReport : SalesReport
    {
        private readonly int year;
        private readonly int month;

        string filePath = "C:\\Users\\Carla Abella\\Documents\\School Work 24-25\\programing\\RestoManage 3\\OrderHistory.txt";
        public MonthlySalesReport(int year, int month)
        {
            this.year = year;
            this.month = month;
        }

        public override void GenerateSales()
        {
            string title = new DateTime(year, month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            IreadFile(filePath);
            filteredSales = sales
                .Where(s => s.salesDate.Year == year && s.salesDate.Month == month)
                .ToList();

            if (filteredSales.Count > 0)
            {
                FilterSales();
                GenerateTable(title);
            }
            else
            {
                AnsiConsole.Write(new Markup($"No sales found for [yellow]{title}[/]."));
                Console.Write("\nPress ANY key to continue");
                Console.ReadKey();
            }

            sales.Clear();
            filteredSales.Clear();
            filteredDuplicates.Clear();
        }
    }
}
