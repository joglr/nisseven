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
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Runtime.InteropServices;

SpeechSynthesizer? ss = null;
var rand = new Random();
var doneString = "done";

Console.Clear();

var WriteSnow = (String write) =>
{
  Console.WriteLine($"⛄ {write}");
};


int giftsPerPerson = 0;

Console.Clear();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
  ss = new SpeechSynthesizer();
  var pb = new PromptBuilder();
  pb.AppendText("Merry christmas", PromptEmphasis.Strong);
  ss.Speak(pb);
}

while (giftsPerPerson == 0)
{
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
  if (input == null)
  {
    Console.WriteLine($"Please input a name or write {doneString} when done");
    continue;
  }
  input = input.Trim();

  if (input == "")
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
  if (givers.Exists(x => x.ToLower() == input.ToLower()))
  {
    feedback = $"{input} is already on santas list";
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
  var others = new Queue<string>(receivers.Where(p => p != person && !toGiveTo.Contains(p)));

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

  map.Add(person.ToLower(), toGiveTo.ToArray());
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

Console.Clear();
Console.WriteLine("Done. Press enter to continue.");
Console.ReadLine();

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || ss == null)
{

  while (true)
  {
    Console.Clear();
    Console.WriteLine("Type your name to know who you should gift to");
    var person = Console.ReadLine();
    if (person != null && person != "" && map.TryGetValue(person.ToLower(), out var yourReceivers))
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
} else {
  Console.Clear();
  ss.Speak("Everyone, close your eyes.");
  Console.WriteLine("Everyone, close your eyes.");
  Thread.Sleep(5000);
  Console.Clear();
  foreach (var tuple in map)
  {
      if (tuple.Value == null) throw new Exception("Something went wrong! :(");
      Console.Clear();
      ss.Speak($"{tuple.Key}. Please look at the screen");

      Console.WriteLine($"{tuple.Key}, you should gift these people:");
      Console.WriteLine(String.Join(", ", tuple.Value));
      Thread.Sleep(10000);
      Console.Clear();
      ss.Speak($"{tuple.Key}. Close your eyes");

  }
}
