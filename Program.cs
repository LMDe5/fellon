using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace semestrovka2
{
    struct Measurement
    {
        public int Value;
        public int Operations;
        public long TimeTicks;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random(42);
            int[] numbers = new int[10000];
            for (int i = 0; i < numbers.Length; i++)
                numbers[i] = rand.Next(1, 1_000_000);

            RedBlackTree tree = new RedBlackTree();
            List<Measurement> insertMeasurements = new List<Measurement>();
            List<Measurement> searchMeasurements = new List<Measurement>();
            List<Measurement> deleteMeasurements = new List<Measurement>();

            Console.WriteLine("Вставка 10000 элементов...");
            for (int i = 0; i < numbers.Length; i++)
            {
                tree.ResetOperations();
                Stopwatch sw = Stopwatch.StartNew();
                tree.Insert(numbers[i]);
                sw.Stop();
                insertMeasurements.Add(new Measurement
                {
                    Value = numbers[i],
                    Operations = tree.OperationCount,
                    TimeTicks = sw.ElapsedTicks
                });
                if ((i + 1) % 1000 == 0)
                    Console.WriteLine($"  Вставлено {i + 1} элементов");
            }

            var searchValues = numbers.OrderBy(x => rand.Next()).Take(100).ToList();
            Console.WriteLine("Поиск 100 случайных элементов...");
            foreach (int val in searchValues)
            {
                tree.ResetOperations();
                Stopwatch sw = Stopwatch.StartNew();
                bool found = tree.Find(val);
                sw.Stop();
                if (!found)
                    Console.WriteLine($"Предупреждение: {val} не найден (должен быть)");
                searchMeasurements.Add(new Measurement
                {
                    Value = val,
                    Operations = tree.OperationCount,
                    TimeTicks = sw.ElapsedTicks
                });
            }

            var deleteValues = numbers.OrderBy(x => rand.Next()).Distinct().Take(1000).ToList();
            Console.WriteLine("Удаление 1000 случайных элементов...");
            foreach (int val in deleteValues)
            {
                tree.ResetOperations();
                Stopwatch sw = Stopwatch.StartNew();
                bool deleted = tree.Delete(val);
                sw.Stop();
                if (!deleted)
                    Console.WriteLine($"Предупреждение: {val} не удалён");
                deleteMeasurements.Add(new Measurement
                {
                    Value = val,
                    Operations = tree.OperationCount,
                    TimeTicks = sw.ElapsedTicks
                });
            }

            double avgInsertOps = insertMeasurements.Average(m => m.Operations);
            double avgInsertTimeUs = insertMeasurements.Average(m => m.TimeTicks) / (double)Stopwatch.Frequency * 1_000_000.0;
            double avgSearchOps = searchMeasurements.Average(m => m.Operations);
            double avgSearchTimeUs = searchMeasurements.Average(m => m.TimeTicks) / (double)Stopwatch.Frequency * 1_000_000.0;
            double avgDeleteOps = deleteMeasurements.Average(m => m.Operations);
            double avgDeleteTimeUs = deleteMeasurements.Average(m => m.TimeTicks) / (double)Stopwatch.Frequency * 1_000_000.0;

            Console.WriteLine("\n========== РЕЗУЛЬТАТЫ ==========");
            Console.WriteLine($"Вставка:   ср. операций = {avgInsertOps:F2}, ср. время = {avgInsertTimeUs:F3} мкс");
            Console.WriteLine($"Поиск:     ср. операций = {avgSearchOps:F2}, ср. время = {avgSearchTimeUs:F3} мкс");
            Console.WriteLine($"Удаление:  ср. операций = {avgDeleteOps:F2}, ср. время = {avgDeleteTimeUs:F3} мкс");

            SaveMeasurementsToCsv("insertions.csv", insertMeasurements);
            SaveMeasurementsToCsv("searches.csv", searchMeasurements);
            SaveMeasurementsToCsv("deletions.csv", deleteMeasurements);

            Console.WriteLine("\nИзмерения сохранены в CSV-файлы.");
        }

        static void SaveMeasurementsToCsv(string fileName, List<Measurement> data)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Value,Operations,TimeTicks");
                foreach (Measurement m in data)
                    writer.WriteLine($"{m.Value},{m.Operations},{m.TimeTicks}");
            }
        }
    }
}