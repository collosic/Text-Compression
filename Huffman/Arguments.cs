using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huffman
{
    public enum State
    {
        Help,
        Compress,
        Uncompress
    };

    public class Arguments
    {
        // private member data
        private List<char> options;
        private List<string> files;
        
        // private setters publuc getters
        public List<string> arguments { get; private set; }
        public State state { get; private set; }
        public bool isVerbose { get; private set; }
        public string incomingFileName { get; private set; }
        public string outgoingFileName { get; private set; }

        // constructor
        public Arguments(string[] incomingArguments)
        {
            options = new List<char>();
            files = new List<string>();
            arguments = new List<string>();
            arguments.AddRange(incomingArguments);
            state = State.Help;
            isVerbose = false;
            incomingFileName = null;
            outgoingFileName = null;
        }

        public void checkArguments ()
        {
            int argsLength = arguments.Count;

            // check and see if no arguments were entered
            if (argsLength < 2 || argsLength > 4)
            {
                throw new ArgumentException("Incorrect use of arguments");
            }

            // check for minimum # of args and separate them if needed
            foreach (string arg in arguments)
            {
                // If the currect argument has a / or - we determine a state or action
                if (verifyFlag(arg) && options.Count <= 2)
                {
                    separateOptions(arg);
                }
                else
                {
                    // Here we are likely have an incoming text or the output file name
                    if (files.Count <= 2)
                    {
                        files.Add(arg);
                    }
                }
            }

            // Here we need to make sure we have the correct options and files needed

            if (options.Count < 1 || options.Count > 2)
            {
                throw new Exception("Options error");
            }

            foreach (char o in options)
            {
                DetermineOptions(o);
            }

            if (files.Count < 1 || files.Count > 2)
            {
                throw new Exception("No source file detected");
            }
            if (files.Count == 1)
            {
                incomingFileName = files[0];
            }
            if (files.Count == 2)
            {
                incomingFileName = files[0];
                outgoingFileName = files[1];
            }

        }
        private bool verifyFlag (string arg)
        {
            // The Console application will support / and - for arguments and flags
            return arg[0] == '/' || arg[0] == '-' ? true : false;
        }

        private void separateOptions(string arg)
        {
            // remove the / or - and extract option(s)
            string characters = arg.Remove(0, 1);
            foreach (char c in characters)
            {
                if (!Char.IsLetter(c))
                {
                    throw new ArgumentException(c.ToString());
                }
                options.Add(c);
            }
        }

        private void DetermineOptions(char option)
        {
            switch (Char.ToLower(option))
            {
                case 'c':
                    state = State.Compress;
                    break;
                case 'u':
                    state = State.Uncompress;
                    break;
                case 'h':
                    state = State.Help;
                    break;
                case 'v':
                    isVerbose = true;
                    break;
                default:
                    throw new ArgumentException("'" + option + "' option not recognized");
            }
        }
    }
}
