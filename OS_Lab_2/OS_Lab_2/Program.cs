StreamReader input = new StreamReader(@"C:\Users\DinaFormakidov\Desktop\3 course 2 sem\OP. Sys\OS\OS_Lab_2\input.txt");
int.TryParse(input.ReadLine(), out int x);
input.Close();

bool resultF = false, resultG = false;
//Parallel.Invoke(() => { resultF = F(x); }, () => { resultG = G(x); });

var t1 = new Thread(() => { resultF = F(x); });
var t2 = new Thread(() => { resultG = G(x); });
t2.Start(); t1.Start();
t2.Join();  t1.Join();

bool result = resultF || resultG;

Console.WriteLine($"Result: {result}");

StreamWriter output = new StreamWriter(@"C:\Users\DinaFormakidov\Desktop\3 course 2 sem\OP. Sys\OS\OS_Lab_2\output.txt");
output.WriteLine(result);
output.Close();

bool F(int x)
{
    Console.WriteLine("Start F");

    Console.WriteLine("End F");

    return x > 0;
}

bool G(int x)
{
    Console.WriteLine("Start G");
    int cnt = 0;

    while (true)
    {
        if (x > 5)
        {
            Console.WriteLine("End G");
            return x < 0;
        }
        else
        {
            if (cnt >= 10)
            {
                Console.WriteLine("Long time of response. Please use Tab to exit");

                bool exit = Console.ReadKey().Key == ConsoleKey.Tab;

                if (exit)
                {
                    Console.WriteLine("End G with no response");
                    return false;
                }
            }
            cnt++;
        }
    }
}