# Advent Of Code
This repository provides resources for creating Advent of Code puzzle solutions. It requires .NET 5.0 on Windows 10 to access all functionality. 
Currently supports solutions written in C#, JavaScript (via [Jint](https://github.com/sebastienros/jint)), and Python (via [Python.NET](https://github.com/pythonnet/pythonnet)).

To get started, open the repository in VS Code, then run `dotnet build` to install dependencies. You'll also need to install the [Command Variables](https://marketplace.visualstudio.com/items?itemName=rioj7.command-variable) VS Code extension to be able to debug your code.

## Setting up files
To create the folder structure for a new year, run `dotnet run --y YYYY --l LANG --init`, where YYYY is the year you'd like to initialize and LANG is one of `cs`, `js`, or `py` (for C#, JavaScript, or Python, respectively). This will create a new folder under `src/Puzzles` for the given year, with subfolders for the puzzle inputs and solutions in the chosen language.

## Downloading inputs
Inputs are stored in the `Inputs` subdirectory for each year. When a test is run, the application will check the directory so see if the given input exists. If not, it will be downloaded automatically, so long as the user has logged into the Advent of Code website in Google Chrome on their machine recently. Alternatively, the user can put the session cookie information under `"cookie":` in the `config.json` file. Downloading inputs for dates in the future will not be attempted.

## Writing tests
Each test template contains a method or function named `solve`. This is the entry point for your solution. Python and JavaScript solutions will have a string variable named `input` containing the input. C# solutions have a `PuzzleInput` object that contains extra methods for parsing the input into different common puzzle structures.

Solutions are reported with the functions `SubmitPartOne` and `SubmitPartTwo`. Using these functions will store your answer and record the time the answer was found. 

## Running tests
Tests can be run with `dotnet run --y YYYY --d DD --l LANG`. Omitting the day argument will attempt to run all tests for the given year. The results will be displayed for each part, along with the milliseconds after starting the test that each part of the solution was reported.

If an argument is missing, the program will check the `config.json` file to see if a default has been set. This can be useful if you want to use the vscode debugger.

## Python users
To be able to run Python solutions, you'll need to update the path to your python library in `config.json`. On Windows, this is typically found under `C:\\PythonXX\\pythonXX.dll`.