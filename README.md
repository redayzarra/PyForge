# PyForge Compiler

PyForge is a custom Python compiler built using .NET and C#. It provides an interactive shell for real-time Python code execution and supports a variety of commands for enhanced user interaction. PyForge is designed to parse Python code, evaluate expressions, and provide detailed diagnostics to help debug and improve your Python scripts.

## Table of Contents
1. [Features](#features)
2. [Technology Stack](#technology-stack)
3. [Setup and Installation](#setup-and-installation)
4. [Using the Compiler](#using-the-compiler)
5. [Contributing](#contributing)
6. [License](#license)

## Features

- **Interactive Python Shell**: Directly type and execute Python expressions.
- **Parse Tree Visualization**: Toggle the visibility of the syntax tree for a detailed understanding of parsing.
- **Detailed Diagnostics**: Receive detailed error messages and highlights to easily locate and fix issues.
- **Command Support**: Includes commands for clearing the screen, showing/hiding the parse tree, testing and exiting the compiler.
- **Variable Management**: Persistent variable states across commands within a session.
- **Color-Coded Output**: Utilizes different console colors to enhance readability and differentiate output types.

## Technology Stack

- **Language**: C#
- **Framework**: .NET 7.0
- **Testing**: xUnit.net test suite with MSBuild

## Setup and Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/redayzarra/PyForge.git
   ```

2. **Navigate to the project**:
   ```bash
   cd PyForge
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Configure PowerShell scripts**:
   To make running the compiler and tests easier, you can add custom functions to your PowerShell profile:

   - Open your PowerShell profile in Notepad:
  
   
     ```powershell
     notepad $PROFILE
     ```
   - Add the following functions to the end of the file:
  
     
     ```powershell
     function run-compiler {
         Set-Location "C:\YOUR\PATH\TO\PyForge\main"
         clear
         .\run.bat
     }

     function test-compiler {
         Set-Location "C:\YOUR\PATH\TO\PyForge"
         clear
         dotnet build
         dotnet test .\Tests\Tests.csproj
     }
     ```
   - Save and close Notepad. To load these new functions, restart PowerShell, or reload your profile:
     ```powershell
     . $PROFILE
     ```

5. **Run the PowerShell functions**:
   Once you've set up the PowerShell functions, you can easily run your compiler or execute the tests with the following commands in PowerShell:
   - To run the compiler:
     
     ```powershell
     run-compiler
     ```
   - To run the tests:
     
     ```powershell
     test-compiler
     ```

## Using the Compiler

1. To start the compiler, navigate to the main project folder and run the following in the terminal:

    ```powershell
    run-compiler
    ```

2. This will open the interactive shell. You can start typing Python expressions immediately. Use the following commands within the shell:

- `showTree()`: Show the parse tree of the entered expression.
- `hideTree()`: Hide the parse tree.
- `clear()`, `cls`: Clear the console.
- `reset()`: Reset all variables and clear the console.
- `run()`: Rerun the compiler without needing to exit.
- `test()`: Test the compiler using custom tests.

<div align="center">
 
  <img src="https://github.com/redayzarra/PyForge/assets/113388793/d096aec2-e0d9-4cc2-85d0-eb618d7cf576" alt="Compiler testing output">

</div>


- `exit()`: Exit the compiler.


## Contributing

Contributions are welcome! If you have suggestions or improvements, please fork the repository and submit a pull request. For major changes or enhancements, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT license. Please see the [LICENSE](https://github.com/redayzarra/PyForge/blob/master/LICENSE) file for details.
