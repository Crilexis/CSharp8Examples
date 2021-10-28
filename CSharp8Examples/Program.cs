using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace CSharp8Examples
{
    public enum DiasDaSemana
    {
        Segunda, Terça, Quarta, Quinta, Sexta, Sabado, Domingo
    }

    public class PrefenrenciasPessoais
    {
        public string Apelido { get; set; }

        public DiasDaSemana DiasDaSemana { get; set; }
    }

    class Program
    {
        //Digit separator
        public const long BillionsAndBillions = 100_000_000_000;

        static void Main(string[] args)
        {
            while (true)
            {
                ConsoleWrapper.WriteNewLine("Choose an example to present:");
                Console.WriteLine("0. Indexes");
                Console.WriteLine("1. Defaults");
                Console.WriteLine("2. Pattern and property matching");
                Console.WriteLine("3. Tuples and discarts");
                Console.WriteLine("4. Deconstructors and positional patterns");
                Console.WriteLine("5. Records");
                Console.WriteLine("99. End");

                // out variables
                //if (!int.TryParse(Console.ReadLine(), out int option))
                //    Console.WriteLine("Invalid option");

                string option = Console.ReadLine();

                Action vaiFilhao = option.Trim() switch
                {
                    "0" => Indexes,
                    "1" => Defaults,
                    "2" => PatternMathing,
                    "3" => Tuples,
                    "4" => DeconstructorsPositionPatterns,
                    "5" => Records,
                    "99" => End,
                    _ => () => ConsoleWrapper.WriteNewLine("Invalid option"),
                };

                vaiFilhao();
            }
        }

        static void End()
        {
            Environment.Exit(0);
        }

        static void Indexes()
        {
            string text = "C# Workshop";
            Console.WriteLine(text[0..2]);
            Console.WriteLine(text[..2]);
            Console.WriteLine(text[3..]);
            Console.WriteLine(text[..]);

            Console.WriteLine(text[..^8]);
            Console.WriteLine(text[..^4]);

            Console.WriteLine(text[^4..]);            

            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8 };            

            Console.WriteLine(string.Join(", ", numbers[..^1]));
            Console.WriteLine(string.Join(", ", numbers[^1..]));
            Console.WriteLine(string.Join(", ", numbers[^3..]));
            Console.WriteLine(string.Join(", ", numbers[3..5]));

            Person[] teachers = new[]
            {
                new Teacher("Vinicius", "Langbehn", 7),
                new Teacher("Sabrina", "Nawcki", 7),
                new Teacher("Nara", "Nascimento", 7),
                new Teacher("Guilherme", "Tragueta", 7),
                new Teacher("Julio", "Spader", 7),
                new Teacher("Alexandre", "Castoldi", 7)
            };

            foreach (var item in teachers[1..3])
                Console.WriteLine(item);

            foreach (var item in teachers[2..^2])
                Console.WriteLine(item);

            (int a, int b) = (0, 3);

            Range sasTeam = a..b;

            foreach (var item in teachers[sasTeam])
                Console.WriteLine(item);
        }

        static void PatternMathing()
        {
            double[] doubleNumbers = { 1.1, 2.2, 3, 4.5, 5.2, 6, 7.7, 8.4, 9, -1, -2.5, -3.1, -4 };

            IEnumerable<object> intEnumerable = (new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -2, -3, -4 }).Select(i => (object)i);
            IEnumerable<object> doubleEnumerable = doubleNumbers.Select(i => (object)i);

            ConsoleWrapper.WriteNewLine(SumPositiveNumbers(intEnumerable).ToString());
            ConsoleWrapper.WriteNewLine(SumPositiveNumbers(doubleEnumerable).ToString());

            Console.WriteLine(TakeFive("Hello, world!"));
            Console.WriteLine(TakeFive("Hi!"));
            Console.WriteLine(TakeFive(new[] { '1', '2', '3', '4', '5', '6', '7' }));
            Console.WriteLine(TakeFive(new[] { 'a', 'b', 'c' }));

            Console.WriteLine(ComputeSalesTax(new Address("WA"), 15.2M).ToString());
            Console.WriteLine(ComputeSalesTax(new Address("MN"), 10.6M).ToString());

            Console.WriteLine(IsAnyEndOnXAxis(new Segment(new Point(1, 0), new Point(0, 1))));
        }

        public static int SumPositiveNumbers(IEnumerable<object> sequence)
        {
            int sum = 0;

            foreach (var i in sequence)
            {
                sum += i switch
                {
                    0 => 0,
                    IEnumerable<int> childSequence => childSequence.Where(i => i > 0).Sum(),
                    int n when n > 0 => n,
                    int n when n <= 0 => 0,
                    double n when n > 0 => (int)Math.Round(n, 0),
                    double n when n < 0 => 0,
                    null => throw new NullReferenceException("Null found in sequence"),
                    _ => throw new InvalidOperationException("Unrecognized type"),
                };
            }

            return sum;
        }

        public static int SumPositiveNumbersOldSwitch(IEnumerable<object> sequence)
        {
            int sum = 0;
            foreach (var i in sequence)
            {
                switch (i)
                {
                    case 0:
                        break;
                    case IEnumerable<int> childSequence:
                        {
                            foreach (var item in childSequence)
                                sum += (item > 0) ? item : 0;
                            break;
                        }
                    case int n when n > 0:
                        sum += n;
                        break;
                    case int n when n < 0:
                        break;
                    case null:
                        throw new NullReferenceException("Null found in sequence");
                    default:
                        throw new InvalidOperationException("Unrecognized type");
                }
            }
            return sum;
        }

        static string TakeFive(object input) => input switch
        {
            string { Length: >= 5 } s => s[0..5],
            string s => s,

            ICollection<char> { Count: >= 5 } symbols => new string(symbols.Take(5).ToArray()),
            ICollection<char> symbols => new string(symbols.ToArray()),

            null => throw new ArgumentNullException(nameof(input)),
            _ => throw new ArgumentException("Not supported input type."),
        };

        static string TakeFiveIf(object input)
        {
            if (input is string { Length: >= 5 } s5)
                return s5[0..5];

            if(input != null)
            {
                if(input.GetType() == typeof(string))
                {
                    if(((string)input).Length > 5)
                    {
                        return ((string)input).Substring(0, 5);
                    }
                }
            }

            if (input is string s)
                return s;

            if (input is ICollection<char> { Count: >= 5 } symbols5)
                return new string(symbols5.Take(5).ToArray());

            if (input is ICollection<char> symbols)
                return new string(symbols.ToArray());

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            throw new ArgumentException("Not supported input type.");
        }

        public record Address(string State);

        public static decimal ComputeSalesTax(Address location, decimal salePrice) => location switch
        {
            { State: "WA" } => salePrice * 0.06M,
            { State: "MN" } => salePrice * 0.075M,
            { State: "MI" } => salePrice * 0.05M,
            _ => 0M
        };

        public record Segment(Point Start, Point End);

        static bool IsAnyEndOnXAxis(Segment segment) => segment is { Start: { Y: 0 } } or { End: { Y: 0 } };

        static bool IsAnyEndIF(Segment segment)
        {
            return (segment.Start.Y == 0 || segment.End.Y == 0);
        }

        static bool IsFromSchool(Person person) => person is Teacher or Student or Cleaner { Function: "Pool" };

        static bool IsTeacher(Person person)
        {
            if (person == null)
                return false;

            if (person.GetType() == typeof(Teacher))
                return true;

            return false;
        }


        static void Defaults()
        {
            T[] InitializeArray<T>(int length, T initialValue = default)
            {
                if (length < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), "Array length must be nonnegative.");
                }

                var array = new T[length];
                for (var i = 0; i < length; i++)
                {
                    array[i] = initialValue;
                }
                return array;
            }

            void Display<T>(T[] values) => Console.WriteLine($"[ {string.Join(", ", values)} ]");

            Display(InitializeArray<int>(3));
            Display(InitializeArray<ExpressionMembersExample>(3));
            Display(InitializeArray<bool>(4, default));

            System.Numerics.Complex fillValue = default;
            Display(InitializeArray(3, fillValue));
        }

        static void Tuples()
        {
            (string Alpha, string Beta) namedLetters = ("a", "b");
            ConsoleWrapper.WriteNewLine($"{namedLetters.Alpha}, {namedLetters.Beta}");

            var alphabetStart = (Alpha: "a", Beta: "b");
            Console.WriteLine($"{alphabetStart.Alpha}, {alphabetStart.Beta}");

            static (int min, int max) Range(int[] array) => (array.Min(), array.Max());

            int[] numbers = { 1, 2, 3, 5, 6, 7 };

            (int min, int max) = Range(numbers);
            Console.WriteLine(max);
            Console.WriteLine(min);

            (int min, int max) b = (numbers.Min(), numbers.Max());
            Console.WriteLine(b);

            (_, max) = Range(numbers.Where(x => x < 3).ToArray());
            Console.WriteLine(max);
            Console.WriteLine(min);
        }

        static (double Sum, int Count) SumAndCount(IEnumerable<int> numbers)
        {
            int sum = 0;
            int count = 0;

            foreach (int number in numbers)
            {
                sum += number;
                count++;
            }

            return (sum, count);
        }

        static void DeconstructorsPositionPatterns()
        {
            var numbers = new List<int> { 1, 2, 3 };
            if (SumAndCount(numbers) is (Sum: var sum, Count: > 0))
            {
                Console.WriteLine($"Sum of [{string.Join(" ", numbers)}] is {sum}");
            }

            var tuple = SumAndCount(numbers);
            if (tuple.Count > 0)
            {
                Console.WriteLine($"Sum of [{string.Join(" ", numbers)}] is {tuple.Sum}");
            }

            (var a, var b) = SumAndCount(numbers);
            if (b > 0)
            {
                Console.WriteLine($"Sum of [{string.Join(" ", numbers)}] is {a}");
            }


            Console.WriteLine(Classify(new (1, 0)));

            Console.WriteLine(Classify(new (0, 0)));

            Console.WriteLine(Classify(new (0, 1)));

            Console.WriteLine(Classify(new (1, 1)));


            Console.WriteLine(ClassifyNumber(13));
            
            Console.WriteLine(ClassifyNumber(-100));
            
            Console.WriteLine(ClassifyNumber(5.7));


            //#CHECK
            Console.WriteLine(PrintIfAllCoordinatesArePositive((1, 1)));

            Console.WriteLine(PrintIfAllCoordinatesArePositive((1, 1, 1)));

            Console.WriteLine(PrintIfAllCoordinatesArePositive((1, -1)));

            Console.WriteLine(PrintIfAllCoordinatesArePositive("Something"));


            //#CHECK
            Console.WriteLine(CheckIfInt("Something"));

            Console.WriteLine(CheckIfInt(1));

            Console.WriteLine(CheckIfInt(2.5));


            Console.WriteLine(IsLetter('a'));

            Console.WriteLine(IsLetter('?'));

            Console.WriteLine(IsLetter('.'));
        }

        public readonly struct Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y) => (X, Y) = (x, y);

            //public Point(int x, int y)
            //{
            //    X = x;
            //    Y = y;
            //}

            public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
        }

        static string Classify(Point point) => point switch
        {
            (0, 0) => "Origin",
            (1, 0) => "positive X basis end",
            (0, 1) => "positive Y basis end",
            _ => "Just a point",
        };

        static string ClassifyNumber(double measurement) => measurement switch
        {
            < -40.0 => "Too low",
            >= -40.0 and < 0 => "Low",
            >= 0 and < 10.0 => "Acceptable",
            >= 10.0 and < 20.0 => "High",
            >= 20.0 => "Too high",
            double.NaN => "Unknown",
        };

        

        public record Point2D(int X, int Y);
        public record Point3D(int X, int Y, int Z);

        static string PrintIfAllCoordinatesArePositive(object point) => point switch
        {
            Point2D(> 0, > 0) p => p.ToString(),
            Point3D(> 0, > 0, > 0) p => p.ToString(),
            _ => "Nope",
        };

        static bool CheckIfInt(object input) => input is int or not (float or double);

        static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');


        public abstract record Person(string FirstName, string LastName);
        public record Teacher(string FirstName, string LastName, int Grade) : Person(FirstName, LastName);
        public record Student(string FirstName, string LastName, int Grade) : Person(FirstName, LastName);
        public record Cleaner(string FirstName, string LastName, string Function) : Person(FirstName, LastName);

        public record Gardener(string FirstName, string LastName) : Person(FirstName, LastName)
        {
            public void Deconstruct(out string x, out string y, out double z) => (x, y, z) = (FirstName, LastName, Payment);

            public double Payment { get; set; }
        }

        static void Records()
        {
            Person teacher = new Teacher("Nancy", "Davolio", 3);
            Person student = new Student("Nancy", "Davolio", 3);
            Console.WriteLine(teacher == student);

            Student student2 = new Student("Nancy", "Davolio", 3);
            Console.WriteLine(student2 == student);

            Student student3 = student2 with { FirstName = "Jason" };
            Console.WriteLine(student3);

            var (firstName, lastName) = teacher;
            Console.WriteLine($"{firstName}, {lastName}");

            Gardener guilherme = new Gardener("Guilherme", "Tragueta") { Payment = 0 };
            guilherme.Payment = 10;

            (string n, string s, double p) = guilherme;

            var (fName, lName, grade) = (Teacher)teacher;
            Console.WriteLine($"{fName}, {lName}, {grade}");
        }
    }


    class ExpressionMembersExample
    {
        public ExpressionMembersExample(string label) => Label = label;

        private string label;
        public string Label
        {
            get => label;
            set => label = value ?? "Default label";
        }

        public override string ToString()
        {
            return Label;
        }
    }

    class ConsoleWrapper
    {
        public static void WriteNewLine(string message) => Console.WriteLine(Environment.NewLine + message);
    }
}
