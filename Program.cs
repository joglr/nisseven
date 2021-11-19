/*
CMLTool
Opsætningsface:
  Indskrivning af personer
  Pulje af personer
  Personerne
*/
using System;
using System.Collections.Generic;

Console.Clear();

var WriteSnow = (String write) => {
  Console.WriteLine($"⛄ {write}");
};

var doneString = "miskidut";

Console.WriteLine("Welcome christmas-nisse!🎅🏼");
Console.WriteLine("");
Console.WriteLine($"Write {doneString} to exit!!");
Console.WriteLine("Add new name:");
var done = false;
List<string> persons = new List<string>();

while (done == false)
{
  var input = Console.ReadLine();
  if (input == null || input == "")
  {
    Console.WriteLine($"Please input a name or write {doneString} when done");
    continue;
  }
  if (input.ToLower() == doneString)
  {
    done = true;
    continue;
  }
  persons.Add(input);
}
foreach (var person in persons)
{
  Console.WriteLine(person);
}
