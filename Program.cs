namespace SP_HW_CriticalSections
{
    internal class Program
    {
        private static readonly object LockObject = new();
        private static bool _generationComplete = false;
        private static int[] _numbers;
        private static int[] _sums;
        private static int[] _products;

        static void Main()
        {
            Thread generatorThread = new(GenerateNumbers);
            Thread sumThread = new(SumNumbers);
            Thread productThread = new(ProductNumbers);

            generatorThread.Start();
            sumThread.Start();
            productThread.Start();

            generatorThread.Join();
            sumThread.Join();
            productThread.Join();

            int count = 1;
            using (StreamWriter writer = new("results.txt"))
                for (int i = 0; i < _numbers.Length; i += 2)
                {
                    writer.WriteLine($"Pair {count}: numb 1 = {_numbers[i]}, numb 2 = {_numbers[i + 1]}");
                    writer.WriteLine("\n");
                    count++;
                }

            using (StreamWriter writer = new("results.txt", true))
                for (int i = 0; i < _numbers.Length/2; i++)
                    writer.WriteLine($"Pair {i + 1}: Sum = {_sums[i]}, Product = {_products[i]}");

            Console.WriteLine("Результаты записаны в файл results.txt.");
        }

        static void GenerateNumbers()
        {
            Random random = new();
            int numberOfPairs = 10;
            _numbers = new int[numberOfPairs * 2];

            for (int i = 0; i < numberOfPairs * 2; i++)
                _numbers[i] = random.Next(1, 101);

            lock (LockObject)
                _generationComplete = true;
        }

        static void SumNumbers()
        {
            while (true)
                lock (LockObject)
                    if (_generationComplete)
                    {
                        int numberOfPairs = _numbers.Length / 2;
                        _sums = new int[numberOfPairs];

                        for (int i = 0; i < numberOfPairs; i++)
                            _sums[i] = _numbers[i * 2] + _numbers[i * 2 + 1];
                        break;
                    }
        }

        static void ProductNumbers()
        {
            while (true)
                lock (LockObject)
                    if (_generationComplete)
                    {
                        int numberOfPairs = _numbers.Length / 2;
                        _products = new int[numberOfPairs];

                        for (int i = 0; i < numberOfPairs; i++)
                            _products[i] = _numbers[i * 2] * _numbers[i * 2 + 1];
                        break;
                    }
        }
    }
}