using FourteenNumbers;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Game Day, Target");

for (uint i = 0; i < 10000; i++)
{
    uint target = TargetValue.GetTarget(i);
    Console.WriteLine($"{i}, {target}");
}
