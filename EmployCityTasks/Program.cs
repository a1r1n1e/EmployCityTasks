namespace EmployCityTasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter sum you want to get:");
                string? input = Console.ReadLine();
                if (input == null || input == string.Empty)
                {
                    break;
                }

                int sumToRemove;
                try
                {
                    sumToRemove = int.Parse(input);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine($"{ae.Message}");
                    break;
                }
                catch (FormatException fe)
                {
                    Console.WriteLine($"{fe.Message}");
                    break;
                }
                catch (OverflowException oe)
                {
                    Console.WriteLine($"{oe.Message}");
                    break;
                }



                IEnumerable<(int value, int amount)> result;
                bool isSuccess = BanknotesRepository.TryRemoveAmount(amount: sumToRemove, result: out result);

                if (isSuccess)
                {
                    Console.WriteLine("You got:");
                    foreach (var valueAmountPair in result)
                    {
                        Console.WriteLine($"{valueAmountPair.value} {valueAmountPair.amount}");
                    }
                }
                else
                {
                    Console.WriteLine("Can't get such amount of money.");
                }
                Console.WriteLine();
            }
        }
    }
}
