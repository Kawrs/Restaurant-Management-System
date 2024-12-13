using RestoManage_3;
using Spectre.Console;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace restoManage
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            Customer customer = new Customer();
            Staff staff = new Staff();
            Inventory myInventory = new Inventory();
            OrderHistory history = new OrderHistory();

            string staffPassword = "1234";
            string managerPassword = "qwerty";;
            string passwords;

            do
            {
                Console.Clear();
                AnsiConsole.Write(new Markup("[gold1]" +
                    "██████╗ ███████╗███████╗████████╗ ██████╗ ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗" +
                    "\n██╔══██╗██╔════╝██╔════╝╚══██╔══╝██╔═══██╗████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝" +
                    "\n██████╔╝█████╗  ███████╗   ██║   ██║   ██║██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  " +
                    "\n██╔══██╗██╔══╝  ╚════██║   ██║   ██║   ██║██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  " +
                    "\n██║  ██║███████╗███████║   ██║   ╚██████╔╝██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗" +
                    "\n╚═╝  ╚═╝╚══════╝╚══════╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝[/]\n\n"));
                Console.Write("Welcome Customer!!\n");
                passwords = AnsiConsole.Prompt(
                    new TextPrompt<string>("Press ANY key to Continue")
                    .Secret(' ')
                    .AllowEmpty());

                if(passwords == staffPassword)
                {
                    string selectedIndexStaff;
                    do
                    {
                        Console.Clear();
                        staff.DisplayTable();
                        selectedIndexStaff = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose an Option you want to do.\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                                .PageSize(10)
                                .AddChoices(new[] {
                                            "Delete Whole Order", "Exit"
                                 }));

                        switch (selectedIndexStaff)
                        {
                            case "Delete Whole Order": //whole Order kay e delete
                                {
                                    staff.deleteOrder();
                                    break;
                                }
                            case "Exit":
                                {
                                    continue;
                                }
                            default:
                                {
                                    Console.WriteLine("Invalid Input");
                                    Console.Write("\nPress Any Key to Continue");
                                    Console.ReadLine();
                                    Console.Clear();
                                    break;
                                }
                        }

                    } while (selectedIndexStaff != "Exit");
                    continue;
                }
                else if (passwords == managerPassword)
                {
                    string selectedIndexManager;
                    do
                    {
                        Console.Clear();
                        selectedIndexManager = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose an Option you want to do.\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                                .PageSize(10)
                                .AddChoices(new[] {
                                            "Menu", "Order History", "Sales Report", "Inventory", "Exit"
                                 }));
                        switch (selectedIndexManager)
                        {
                            case "Menu":                                         //menu
                                {
                                    string selectedIndexMenu;
                                    do
                                    {
                                        Console.Clear();
                                        selectedIndexMenu = AnsiConsole.Prompt(
                                            new SelectionPrompt<string>()
                                                .Title("Choose an Option you want to do.\n[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]")
                                                .PageSize(10)
                                                .AddChoices(new[] {
                                                            "Add/Create Menu", "Remove Menu Item", "Exit"
                                                 }));
                                        switch (selectedIndexMenu)
                                        {
                                            case "Add/Create Menu":                         //create menu
                                                {
                                                    menu.OptionsForAddMenu();
                                                    break;
                                                }
                                            case "Remove Menu Item":
                                                {
                                                    menu.RemoveMenuItem();
                                                    break;
                                                }
                                            case "Exit": continue; //EXIT
                                            default: Console.WriteLine("Invalid input"); break;
                                        }

                                    } while (selectedIndexMenu != "Exit");
                                    break;
                                }
                            case "Order History": //Order History
                                history.orderHistory();
                                break;
                            case "Sales Report": //sales report
                                {
                                    salesReport();
                                    break;
                                }
                            case "Inventory": //inventory
                                {
                                    myInventory.StartInventory();
                                    break;
                                }
                            case "Exit": //exit
                                continue;
                            default: Console.WriteLine("Invalid input"); break;
                        }
                    } while (selectedIndexManager != "Exit");
                    continue;
                }
                else if(string.IsNullOrWhiteSpace(passwords))
                {
                    customer.customer();
                }
                

            } while (passwords.ToLower() != "exit");

            void salesReport()
            {
                int day, month, year, week;
                Console.Clear();
                string selectedIndex;
                do
                {
                    Console.Clear();
                    selectedIndex = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[grey](Move [yellow]UP[/] and [yellow]DOWN[/] to select the option you want to choose and press ENTER)[/]\nSales Report")
                                .PageSize(10)
                                .AddChoices(new[] {
                                "Monthly", "Weekly", "Daily", "Exit"
                                }));

                    switch (selectedIndex)
                    {
                        case "Monthly":
                            {
                                Console.Clear();
                                while (true)
                                {
                                    AnsiConsole.Write(new Markup("Enter the Month you would like to view that month's sales [yellow](in 'MM' format)[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out month) || month <= 0 || month > 12)
                                    {
                                        Console.WriteLine("Invalid Month. Please enter a valid month from 1-12");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }

                                    while (true)
                                    {
                                        AnsiConsole.Write(new Markup("Enter the Year you would like to view that month's sales [yellow](in 'yyyy')[/]: "));
                                        if (!int.TryParse(Console.ReadLine(), out year) || year <= 0)
                                        {
                                            Console.WriteLine("Invalid Year. Please enter positive number in 'yyyy' format");
                                            Console.Write("\nPress ANY key to continue");
                                            Console.ReadKey();
                                            continue;
                                        }
                                        break;
                                    }
                                    break;
                                }
                                
                                MonthlySalesReport myMonthlySales = new MonthlySalesReport(year, month);
                                myMonthlySales.GenerateSales();
                                break;
                            }
                        case "Weekly":
                            {
                                Console.Clear();

                                while (true)
                                {
                                    // Prompt for year
                                    AnsiConsole.Write(new Markup("Enter the Year you would like to view sales for [yellow](in 'yyyy')[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out year) || year <= 0)
                                    {
                                        Console.WriteLine("Invalid Year. Please enter a positive number in 'yyyy' format.");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }

                                    // Prompt for month
                                    AnsiConsole.Write(new Markup("Enter the Month you would like to view sales for [yellow](in 'MM' format)[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out month) || month <= 0 || month > 12)
                                    {
                                        Console.WriteLine("Invalid Month. Please enter a valid month from 1-12.");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }
                                    WeeklySalesReport myWeeklySales = new WeeklySalesReport(year, month);
                                    myWeeklySales.GenerateSales();
                                    break;
                                }
                            }
                            break;
                        case "Daily":
                            {
                                Console.Clear();
                                DateTime date;
                                while (true)
                                {
                                    AnsiConsole.Write(new Markup("Enter the Day you would like to view that day's sales [yellow](in 'dd' format)[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out day) || day <= 0 || day > 31)
                                    {
                                        Console.WriteLine("Invalid Day. Please enter a valid day from 1 - 31");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                    }

                                    AnsiConsole.Write(new Markup("Enter the Month you would like to view that month's sales [yellow](in 'MM' format)[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out month) || month <= 0 || month > 12)
                                    {
                                        Console.WriteLine("Invalid Month. Please enter a valid month from 1-12");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }

                                    AnsiConsole.Write(new Markup("Enter the Year you would like to view that month's sales [yellow](in 'yyyy')[/]: "));
                                    if (!int.TryParse(Console.ReadLine(), out year) || year <= 0)
                                    {
                                        Console.WriteLine("Invalid Year. Please enter positive number in 'yyyy' format");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }

                                    //validate date
                                    if (day > DateTime.DaysInMonth(year, month))
                                    {
                                        Console.WriteLine($"Invalid Date. The day {day} is not valid for the month {month} and year {year}.");
                                        Console.Write("\nPress ANY key to continue");
                                        Console.ReadKey();
                                        continue;
                                    }
                                    else //if valid generate sales
                                    {
                                        date = new DateTime(year, month, day);
                                        DailySalesReport dailySales = new DailySalesReport(date);
                                        dailySales.GenerateSales();
                                    }
                                    break;
                                }
                                break;
                            }
                        case "Exit": break;
                        default:
                            {
                                Console.WriteLine("Invalid Option");
                                Console.WriteLine("\nPress ANY key to Continue");
                                Console.ReadKey();
                                break;
                            }
                    }
                } while (selectedIndex != "Exit");
            }

        }
    }
}
