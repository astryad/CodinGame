using System;
using System.IO;
using System.Text;
using System.Collections;

/**
 * Send your busters out into the fog to trap ghosts and bring them home!
 **/

class SystemConsole : IConsole
{
    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }

    public string ReadLine()
    {
        return Console.ReadLine();
    }
}