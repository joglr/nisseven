/*
CMLTool
Opsætningsface:
  Indskrivning af personer
  Pulje af personer
  Personerne

  TODO: Check if name already exists
  Everyone, close your eyes
  {person}, wake up! Look at the screen
  Speech synthesis
*/
using System;
using System.Collections.Generic;

var rand = new Random();

Console.Clear();

var WriteSnow = (String write) =>
{
  Console.WriteLine($"⛄ {write}");
};

var doneString = "miskidut";

int giftsPerPerson = 0;
while (giftsPerPerson == 0)
{
  Console.Clear();
  Console.WriteLine("Welcome christmas-nisse!🎅🏼");
  Console.WriteLine();
  Console.WriteLine("Please enter amount of gifts per person");
  var input = Console.ReadLine();
  int.TryParse(input, out giftsPerPerson);
}


var done = false;
List<string> givers = new List<string>();
string feedback = "";

while (done == false)
{
  Console.Clear();
  if (feedback.Length > 0) Console.WriteLine(feedback);
  feedback = "";
  Console.Write(givers.Count == 0 ? "Add new name" : "Add another name");
  Console.Write($" (or write {doneString} to exit):");
  Console.WriteLine();
  Console.WriteLine(givers.Count == 0 ? "" : String.Join(", ", givers));
  Console.WriteLine();
  var input = Console.ReadLine();
  if (input == null || input == "")
  {
    Console.WriteLine($"Please input a name or write {doneString} when done");
    continue;
  }
  if (input.ToLower() == doneString)
  {
    if (givers.Count < giftsPerPerson + 1)
    {
      feedback = $"Need {giftsPerPerson + 1 - givers.Count} more people to continue";
      continue;
    }
    done = true;
    continue;
  }
  givers.Add(input);
}

var pool = new List<string>();

for (int i = 0; i < giftsPerPerson; i++)
{
  foreach (var person in givers)
  {
    pool.Add(person);
  }
}

var receivers = new Queue<string>();

foreach (var person in pool.OrderBy(_ => rand.Next()))
{
  receivers.Enqueue(person);
}

var map = new Dictionary<string, string[]>();

foreach (var person in givers)
{
  List<string> toGiveTo = new List<string>();

  var mes = new Queue<string>(receivers.Where(p => p == person || toGiveTo.Contains(p)));
  // foreach(var p in mes) System.Console.WriteLine($"me: {p}");
  var others = new Queue<string>(receivers.Where(p => p != person && !toGiveTo.Contains(p)));
  // foreach(var p in others) System.Console.WriteLine($"others: {p}");

  for (int i = 0; i < giftsPerPerson; i++)
  {
    string personToGiveTo = "";

    while (personToGiveTo == "")
    {
      if (toGiveTo.Contains(others.Peek()))
      {
        // Shuffle the bag
        others = new Queue<string>(others.OrderBy(_ => rand.Next()));
        continue;
      }
      personToGiveTo = others.Dequeue();
    }

    toGiveTo.Add(personToGiveTo);
  }

  map.Add(person, toGiveTo.ToArray());
  receivers = new Queue<string>(mes.Concat(others).OrderBy(_ => rand.Next()));
}

// DEBUG CODE:
// foreach (var tuple in map.AsEnumerable())
// {
//   Console.WriteLine();
//   Console.WriteLine($"{tuple.Key} should gift to:");
//   foreach (var r in tuple.Value)
//   {
//     Console.WriteLine($" - {r}");
//   }
// }

Console.ReadLine();

while (true)
{
  Console.Clear();
  Console.WriteLine("Type your name to know who you should gift to");
  var person = Console.ReadLine();
  if (person != null && person != "" && map.TryGetValue(person, out var yourReceivers))
  {
    if (yourReceivers == null) throw new Exception("Something went wrong! :(");

    for (int i = 0; i < 10; i++)
    {
      Console.Clear();
      Console.WriteLine("You should gift these people:");
      Console.WriteLine(String.Join(", ", yourReceivers));
      Console.WriteLine($"This message will self destruct in {10 - i} seconds");
      Thread.Sleep(1000);
    }

    Console.WriteLine();
  }
}
