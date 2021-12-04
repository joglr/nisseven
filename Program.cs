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
using System.Text;
using System.Text.Json;

SpeechSynthesizer? ss = null;
var rand = new Random();
var doneString = "done";

Console.Clear();

var WriteSnow = (string text) =>
{
  Console.WriteLine($"⛄ {text}");
};

var WriteAndSpeak = (string text, bool clear) =>
{
  if (clear) Console.Clear();
  Console.WriteLine(text);
  if (ss != null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ss.SpeakAsync(text);
};


int giftsPerPerson = 0;

Console.Clear();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
  ss = new SpeechSynthesizer();
  var pb = new PromptBuilder();
  pb.AppendText("Merry christmas", PromptEmphasis.Strong);
  ss.SpeakAsync(pb);
}

Dictionary<string, string[]> giftMap;
var arguments = Environment.GetCommandLineArgs();

if (arguments.Length > 1 && arguments[1].EndsWith(".secret"))
{
  // Load map from file
  WriteAndSpeak("Loading from file", false);
  var base64 = File.ReadAllText(arguments[1].Trim());
  var json = Convert.FromBase64String(base64);
  var result = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);
  if (result == null) throw new Exception("Error reading file");
  else giftMap = result;
}
else
{
  giftMap = new Dictionary<string, string[]>();
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
    var msg = givers.Count == 0 ? "Add new name" : "Add another name";
    Console.WriteLine($"{msg} (or write {doneString} to exit):");
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
    if (givers.Exists(x => x == input))
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

    giftMap.Add(person, toGiveTo.ToArray());
    receivers = new Queue<string>(mes.Concat(others).OrderBy(_ => rand.Next()));
  }

  var str = JsonSerializer.Serialize(giftMap);

  if (str != null) {

  // Convert the variable str to Base64 string
  var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
  File.WriteAllText("nisseven.secret", base64);
  } else {
    Console.WriteLine("Error serializing data");
  }
}
WriteAndSpeak("Done. Press enter to continue.", true);
Console.ReadLine();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && ss != null)
{
  Console.Clear();
  Console.WriteLine("Everyone, close your eyes.");
  ss.Speak("Everyone, close your eyes.");
  Thread.Sleep(2000);
  Console.Clear();
  foreach (var tuple in giftMap)
  {
    if (tuple.Value == null) throw new Exception("Something went wrong! :(");
    Console.Clear();
    var pb = new PromptBuilder();
    pb.AppendText(tuple.Key, PromptEmphasis.Moderate);
    pb.AppendText("Please look at the screen");
    ss.Speak(pb);

    Console.WriteLine($"{tuple.Key}, you should gift these people:");
    Console.WriteLine(String.Join(", ", tuple.Value));
    Thread.Sleep(5000);
    Console.Clear();
    ss.Speak($"{tuple.Key}. Close your eyes");
    Thread.Sleep(1000);

  }
}
else
{
  // Fallback if no voice is available
  while (true)
  {
    Console.Clear();
    Console.WriteLine("Type your name to know who you should gift to");
    var person = Console.ReadLine();
    if (person != null && person != "" && giftMap.TryGetValue(person, out var yourReceivers))
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
}
