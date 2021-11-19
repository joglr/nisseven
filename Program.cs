/*
CMLTool
Opsætningsface:
  Indskrivning af personer
  Pulje af personer
  Personerne
*/
using System.Collections.Generic;
Console.Clear();
Console.WriteLine("Welcome christmas-nisse!");
Console.WriteLine("Write miskidut to exit!!");
Console.WriteLine("Add new name:");
var done = false;
List<string> persons = new List<string>();

while(done == false){
  var input = Console.ReadLine();
  if(input == "miskidut"){done = true; continue;}
  if(input == null){
    Console.WriteLine("Please input a name");
    continue;
  }
  persons.Add(input);
}
foreach(var person in persons){
  Console.WriteLine(person);
}
