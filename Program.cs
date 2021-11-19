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

Console.WriteLine("Welcome christmas-nisse!🎅🏼");
Console.WriteLine("");
Console.WriteLine("Write miskidut to exit!!");
Console.WriteLine("Add new name:");
var done = false;
List<string> persons = new List<string>();

while (done == false)
{
  var input = Console.ReadLine();
  if (input == null || input == "")
  {
    Console.WriteLine("Please input a name");
    continue;
  }
  if (input.ToLower() == "miskidut")
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
