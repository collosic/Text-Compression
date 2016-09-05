using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class Arguments
    {
        // private member data
        private string[] args;
        private List<string> arguments;
        public string fileName { get; private set; }

        // constructor
        public Arguments(string[] incomingArguments)
        {
            args = new string[incomingArguments.Length];
            incomingArguments.CopyTo(args, 0);
            arguments = new List<string>();
        }

        public void checkArguments ()
        {
            int argsLength = args.Length;

            // check and see if no arguments were entered
            if (argsLength < 1)
            {
                throw new ArgumentException("no arguments passed");
            }

            // check for minimum # of args and separate them if needed
            for (int i = 0, j = 0; i < argsLength; i++)
            {
                if (i == (argsLength - 1))
                {
                    // we need to determine if help alone was requested
                    if (!verifyDash(args[i]))
                    {
                        fileName = args[argsLength - 1];
                    }
                    else
                    {
                        arguments.Add(args[i][1].ToString());
                    }
                }
                else if (args[i].Length == 2)
                {
                    if (!verifyDash(args[i]))
                    {
                        throw new ArgumentException(args[i]);
                    }
                    arguments.Add(args[i][1].ToString());
                }
                else if (args[i].Length > 2)
                {
                    // here a user has concatenated arguments ex. -cae
                    separateArgs(args[i], ref j);
                }
                else if (args[i].Length < 1)
                {
                    Console.WriteLine("Invalid number of parameters, use /h for help");
                    System.Environment.Exit(1);
                }
                else
                {
                    arguments.Add(args[i][1].ToString());
                    if (arguments[0] != "h")
                    {
                        Console.WriteLine("Invalid number of parameters, use /h for help");
                        System.Environment.Exit(1);
                    }
                }
            }

        }
        private bool verifyDash (string arg)
        {
            return arg[0] == '/' || arg[0] == '-' ? true : false;
        }

        public void display()
        {
            for (int i = 0; i < arguments.Count; i++)
            {
                Console.WriteLine("{0}", arguments[i]);
            }
            
            Console.WriteLine("{0}", fileName);
        }

        private void separateArgs(string arg, ref int j)
        {
            // check that the argument starts with -
            if (!verifyDash(arg))
            {
                throw new ArgumentException(arg);
            }
            
            for (int i = 1; i < arg.Length; i++)
            {
                if (!Char.IsLetter(arg[i]))
                {
                    throw new ArgumentException(arg);
                }
                arguments.Add(arg[i].ToString());
            }
        }
        public List<string> getArgs()
        {
            return arguments;
        }
    }
}
