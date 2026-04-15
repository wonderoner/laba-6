using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TriangleAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
  
            Console.Write("Введите сторону A: ");
            string sideA = Console.ReadLine();

            Console.Write("Введите сторону B: ");
            string sideB = Console.ReadLine();

            Console.Write("Введите сторону C: ");
            string sideC = Console.ReadLine();

            var (triangleType, coordinates) = TriangleProcessor.Process(sideA, sideB, sideC);

            Console.WriteLine($"\nРезультат: {triangleType}");
            Console.WriteLine($"Координаты: A{coordinates[0]}, B{coordinates[1]}, C{coordinates[2]}");
        }
    }

    public static class TriangleProcessor
    {
        private static readonly string logFile = "triangle_log.txt";

        public static (string triangleType, List<(int, int)> coordinates) Process(string inputA, string inputB, string inputC)
        {
      
            if (!float.TryParse(inputA, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float sideA) ||
                !float.TryParse(inputB, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float sideB) ||
                !float.TryParse(inputC, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float sideC))
            {
              
                var invalidResult = ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });
                LogRequest(inputA, inputB, inputC, invalidResult.triangleType, invalidResult.coordinates, false, "Нечисловые входные данные");
                return invalidResult;
            }

         
            if (sideA <= 0 || sideB <= 0 || sideC <= 0)
            {
                var invalidResult = ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });
                LogRequest(inputA, inputB, inputC, invalidResult.triangleType, invalidResult.coordinates, false, "Стороны должны быть положительными числами");
                return invalidResult;
            }


            if (sideA + sideB <= sideC || sideA + sideC <= sideB || sideB + sideC <= sideA)
            {
                var invalidResult = ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });
                LogRequest(inputA, inputB, inputC, invalidResult.triangleType, invalidResult.coordinates, false, "Не выполняется неравенство треугольника");
                return invalidResult;
            }

 
            string triangleType;
            const float epsilon = 0.0001f;

            if (Math.Abs(sideA - sideB) < epsilon && Math.Abs(sideB - sideC) < epsilon)
                triangleType = "равносторонний";
            else if (Math.Abs(sideA - sideB) < epsilon || Math.Abs(sideA - sideC) < epsilon || Math.Abs(sideB - sideC) < epsilon)
                triangleType = "равнобедренный";
            else
                triangleType = "разносторонний";


            var coordinates = CalculateCoordinates(sideA, sideB, sideC);


            LogRequest(inputA, inputB, inputC, triangleType, coordinates, true, null);

            return (triangleType, coordinates);
        }

        private static List<(int, int)> CalculateCoordinates(float a, float b, float c)
        {

            float maxSide = Math.Max(a, Math.Max(b, c));
            float scale = 80.0f / maxSide; 

            float xA = 10;
            float yA = 90; 

            float xB = xA + a * scale;
            float yB = yA;

            float dx = (a * a + b * b - c * c) / (2 * a);
            float dy = (float)Math.Sqrt(b * b - dx * dx);

            float xC = xA + dx * scale;
            float yC = yA - dy * scale;

 
            int ixA = (int)Math.Round(Math.Clamp(xA, 0, 100));
            int iyA = (int)Math.Round(Math.Clamp(yA, 0, 100));
            int ixB = (int)Math.Round(Math.Clamp(xB, 0, 100));
            int iyB = (int)Math.Round(Math.Clamp(yB, 0, 100));
            int ixC = (int)Math.Round(Math.Clamp(xC, 0, 100));
            int iyC = (int)Math.Round(Math.Clamp(yC, 0, 100));

            return new List<(int, int)> { (ixA, iyA), (ixB, iyB), (ixC, iyC) };
        }

        private static void LogRequest(string inputA, string inputB, string inputC,
            string triangleType, List<(int, int)> coordinates, bool isSuccess, string errorMessage)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{timestamp}] Параметры: ({inputA}, {inputB}, {inputC}) | " +
                            $"Результат: {triangleType} | Координаты: {string.Join(", ", coordinates)}";

            if (!isSuccess && !string.IsNullOrEmpty(errorMessage))
                logEntry += $" | Ошибка: {errorMessage}";

            Console.WriteLine(logEntry);

            try
            {
                File.AppendAllText(logFile, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось записать в файл лога: {ex.Message}");
            }
        }
    }
}