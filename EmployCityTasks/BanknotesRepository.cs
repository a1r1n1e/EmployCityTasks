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
        internal static bool TryGetMoney(int moneyToRemove, out IEnumerable<(int value, int amount)> result)
        {
            if (moneyToRemove < 0)
            {
                result = new List<(int, int)>();
                return false;
            }

            if (moneyToRemove > TotalAmountOfMoney || Money.Keys.Count == 0)
            {
                result = new List<(int, int)>();
                return false;
            }

            int currentSum = 0;
            var currentUsed = new Dictionary<int, int>();
            foreach (var key in Money.Keys)
            {
                currentUsed.Add(key, Money[key]);
                currentSum += key * Money[key];
            }

            var sortedMoneyKeys = Money.Keys.OrderDescending().ToArray();

            int curValueIndex = sortedMoneyKeys.Length - 1;
            while(curValueIndex >= 0)
            {
                var sourceValue = sortedMoneyKeys[curValueIndex];
                var curAmount = currentUsed[sourceValue];
                if (curAmount == 0)
                {
                    result = new List<(int, int)>();
                    return false;
                }
                while (curAmount >= 0)
                {
                    if (currentSum > moneyToRemove)
                    {
                        --curAmount;
                        if (curAmount >= 0)
                        {
                            currentUsed[sourceValue] = curAmount;
                            currentSum -= sourceValue;
                        }
                    }
                    else if (currentSum == moneyToRemove)
                    {
                        result = currentUsed.Select(x => (x.Key, x.Value)).Where(x => x.Value != 0).ToList();
                        foreach (var elem in result)
                        {
                            Money[elem.value] -= elem.amount;
                            if (Money[elem.value] == 0)
                            {
                                Money.Remove(elem.value);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        if (curValueIndex == sortedMoneyKeys.Length -1)
                        {
                            break;
                        }
                        ReAddRest(sourceDict: Money, orderedSourceKeys: sortedMoneyKeys, tempDict: currentUsed, decreasedKeyIndex: curValueIndex);
                        for(int tmpIndex = curValueIndex + 1; tmpIndex < sortedMoneyKeys.Length; ++ tmpIndex)
                        {
                            var tmpValue = sortedMoneyKeys[tmpIndex];
                            currentSum += tmpValue * currentUsed[tmpValue];
                        }

                        curValueIndex = sortedMoneyKeys.Length - 1;
                        sourceValue = sortedMoneyKeys[curValueIndex];
                        curAmount = currentUsed[sourceValue];
                    }
                }
                --curValueIndex;
            }

            result = new List<(int, int)>();
            return false;
        }

        private static void ReAddRest(Dictionary<int, int> sourceDict, int[] orderedSourceKeys, Dictionary<int, int> tempDict, int decreasedKeyIndex)
        {
            for (int keyToReAddIndex = decreasedKeyIndex + 1; keyToReAddIndex < orderedSourceKeys.Length; ++keyToReAddIndex)
            {
                var key = orderedSourceKeys[keyToReAddIndex];
                tempDict[key] = sourceDict[key];
            }
        }

        private static int CalculateTotal(Dictionary<int, int> amountsByValues)
        {
            int result = 0;

            foreach (var elem in amountsByValues)
            {
                result += elem.Key * elem.Value;
            }

            return result;
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
                            throw new ArgumentException("Non-positive banknote value");
                        }
                        if (amount <= 0)
                        {
                            throw new ArgumentException("Non-positive banknote amount");
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
