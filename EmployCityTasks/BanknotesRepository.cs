namespace EmployCityTasks
{
    /// <summary>
    /// Simple single-thread static repository.
    /// </summary>
    internal static class BanknotesRepository
    {
        private static readonly string InputFilePath = "../../../../Input.txt";

        private static Dictionary<int, int> Money { get; set; }
        
        private static int TotalAmountOfMoney { get; set; }

        static BanknotesRepository()
        {
            Money = GetStoredMoneyAmounts();
            TotalAmountOfMoney = 0;
            foreach(var key in Money.Keys)
            {
                TotalAmountOfMoney += key * Money[key];
            }
        }
        internal static bool TryRemoveAmount(int amount, out IEnumerable<(int value, int amount)> result)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Can't remove negative value");
            }

            if (amount > TotalAmountOfMoney || Money.Keys.Count == 0)
            {
                result = new List<(int, int)>();
                return false;
            }

            int restingSum = amount;

            var stack = new Stack<(List<(int value, int amount)> usedBanknotes, int restingSum, int iterValueIndex, int iterAmount)>();
            List<(int value, int amount)> prevUsedBanknotes;
            var orderedKeys = Money.Keys.Order().ToList();

            var minKeyIndex = 0;
            var minKey = orderedKeys[minKeyIndex];
            for (int banknoteAmount = Money[minKey]; banknoteAmount >= 0; --banknoteAmount)
            {
                var tempRestingSum = restingSum - banknoteAmount * minKey;
                prevUsedBanknotes = new List<(int value, int amount)>();
                prevUsedBanknotes.Add((minKey, banknoteAmount));
                stack.Push((prevUsedBanknotes, tempRestingSum, minKeyIndex, banknoteAmount));
            }
            
            while(stack.Count > 0)
            {
                var data = stack.Pop();
                if (data.restingSum > 0)
                {
                    List<(int value, int amount)> curUsedBanknotes;
                    if (data.iterValueIndex < orderedKeys.Count - 1)
                    {
                        int nextBanknoteValue = orderedKeys[data.iterValueIndex + 1];
                        for (int nextBanknoteAmount = Money[nextBanknoteValue]; nextBanknoteAmount >= 0; --nextBanknoteAmount)
                        {
                            curUsedBanknotes = new List<(int value, int amount)>(data.usedBanknotes);
                            curUsedBanknotes.Add((nextBanknoteValue, nextBanknoteAmount));
                            restingSum = data.restingSum - nextBanknoteValue * nextBanknoteAmount;
                            stack.Push((curUsedBanknotes, restingSum, data.iterValueIndex + 1, nextBanknoteAmount));
                        }
                    }
                }
                else if (data.restingSum == 0)
                {
                    result = data.usedBanknotes.Where(x => x.amount > 0).ToList();
                    stack.Clear();

                    foreach(var elem in result)
                    {
                        Money[elem.value] -= elem.amount;
                        TotalAmountOfMoney -= elem.amount;
                        if (Money[elem.value] == 0)
                        {
                            Money.Remove(elem.value);
                        }
                    }

                    return true;
                }
            }
            result = new List<(int, int)>();
            return false;
        }

        internal static Dictionary<int, int> GetStoredMoneyAmounts()
        {
            var result = new Dictionary<int, int>();

            using (StreamReader reader = File.OpenText(InputFilePath))
            {
                string? inputLine = string.Empty;
                while ((inputLine = reader.ReadLine()) != null)
                {
                    int value; int amount;
                    var inputs = inputLine.Split(' ');
                    try
                    {
                        if (inputs == null || inputs.Length != 2)
                        {
                            throw new ArgumentException("Threre must be exactly two integer values in a line.");
                        }
                        value = int.Parse(inputs[0]);
                        amount = int.Parse(inputs[1]);

                        if (value <= 0)
                        {
                            throw new ArgumentException("Negative banknote value");
                        }
                        if (amount <= 0)
                        {
                            throw new ArgumentException("Negative banknote amount");
                        }

                        if (!result.ContainsKey(value))
                        {
                            result.Add(value, 0);
                        }
                        result[value] += amount;
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
                }
            }

            return result;
        }
    }
}
