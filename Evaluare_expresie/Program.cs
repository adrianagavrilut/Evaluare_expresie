using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluare_expresie
{
    class Program
    {
        //TODO: DONE tratarea situatiilor exceptionale date de exprsii eronate 
        //TODO: adaugarea posibilitatii de a evalua expresii care contin apeluri de functii matematice definite in tipul Math.(sin, cos, abs, etc.)
        //TODO: DONE adaugare posibilitatii de a accepta expresii sub orice forma. Implementarea Parsing-ului intr-un mod mai general
        //TODO: DONE operatorii pot fi numere reale(numere in virgula fixa, de ex.: 3.14)
        //TODO: operatorii pot fi si variabile a caror valoare se va lua dintr-o tabela de simboluri
        static void Main(string[] args)
        {
            //parsing
            string input = Console.ReadLine();
            ValidareExpresie(input);
            input = "(" + input + ")";
            string[] tokens = PerfectSplit(input);
            ValidareAtomi(tokens);
            // partea 1: transformarea expresiei din notatie infix in notatie postfix
            List<string> rpn = RPN(tokens); // Reverse Polish notation
            foreach(var item in rpn)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();
            //partea 2: evaluarea expresiei
            Console.WriteLine($"Valoarea expresiei este {EvaluateRPN(rpn)}");
        }

        private static List<string> RPN(string[] tokens)
        {
            Stack<string> stack = new Stack<string>();
            List<string> rpn = new List<string>();
            foreach (var token in tokens)
            {
                if (token == "(")
                {
                    stack.Push(token);
                }
                else if(IsOperator(token))
                {
                    string op;
                    while (stack.Peek() != "(" && Priority(stack.Peek()) >= Priority(token))
                    {
                        op = stack.Pop();
                        rpn.Add(op);
                    }
                    stack.Push(token);
                }
                else if(token == ")")
                {
                    string op;
                    op = stack.Pop();
                    while(op != "(")
                    {
                        rpn.Add(op);
                        op = stack.Pop();
                    }
                }
                else //operand
                {
                    rpn.Add(token);
                }
            }
            return rpn;
        }

        private static double EvaluateRPN(List<string> rpn)
        {
            Stack<double> stack = new Stack<double>();
            foreach (var item in rpn)
            {
                if (IsOperator(item))
                {
                    double op1, op2;
                    op2 = stack.Pop();
                    op1 = stack.Pop();
                    stack.Push(Operate(op1, op2, item));
                }
                else
                {
                    stack.Push(double.Parse(item));
                }
            }
            return stack.Pop();
        }

        private static int Priority(string op)
        {
            int retValue;
            switch (op)
            {
                case "+":
                case "-":
                    retValue = 1;
                    break;
                case "*":
                case "/":
                case "%":
                    retValue = 2;
                    break;
                default:
                    retValue = 0;
                    break;
            }
            return retValue;
        }

        private static double Operate(double op1, double op2, string item)
        {
            double retValue;
            switch (item)
            {
                case "+":
                    retValue = op1 + op2;
                    break;
                case "-":
                    retValue = op1 - op2;
                    break;
                case "*":
                    retValue = op1 * op2;
                    break;
                case "/":
                    retValue = op1 / op2;
                    break;
                //case "%":
                //    retValue = op1 % op2;
                //    break;
                default:
                    retValue = 0;
                    break;
            }
            return retValue;

        }

        private static bool IsOperator(string item)
        {
            switch (item)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "%":
                    return true;
                default:
                    return false;
            }
        }

        private static string[] PerfectSplit(string input)
        {
            char[] operators = { '+', '-', '*', '/', '%', '(', ')' };
            List<string> expresie = new List<string>();
            StringBuilder atom = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (!InArray(input[i], operators))
                {
                    atom.Append(input[i]);
                }
                else
                {
                    if(atom.ToString() != "")
                    {
                        expresie.Add(atom.ToString().Trim());
                        atom.Clear();
                    }
                    expresie.Add(input[i].ToString());
                    
                }
            }
            if(atom.ToString().Trim() != "")
            {
                expresie.Add(atom.ToString().Trim());
            }
            return expresie.ToArray();
        }

        private static bool InArray(char b, char[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if(a[i] == b)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SecvParanteze(string input)
        {
            int cP = 0;
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    cP++;
                }
                else if (input[i] == ')')
                {
                    cP--;
                }
                if (cP < 0)
                {
                    return false;
                }
            }
            if (cP == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method validates the expression before splitting, by checking the following rules
        /// - it check for invalid characters (except the defined operators and character space)
        /// - it checks if the paranthesis number and order is correct
        /// - it checks if the last character is a number or a paranthesis
        /// </summary>
        /// <param name="input">The expression string before splitting</param>
        /// <exception cref="Exception">Lauched of a validation is rule is broken</exception>
        private static void ValidareExpresie(string input)
        {
            char[] acceptedChars = { '(', ')', '+', '-', '/', '%', '*', '.', ' ' };
            char[] cifre = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < input.Length; i++)
            {
                if (!InArray(input[i], acceptedChars) && !InArray(input[i], cifre))
                {
                    throw new Exception("Expresia introdusa este invalida");
                }
            }
            if (input[input.Length - 1] != ')' && !InArray(input[input.Length - 1], cifre))
            {
                throw new Exception("Expresia introdusa este invalida");
            }
            if (!SecvParanteze(input))
            {
                throw new Exception("Expresia introdusa este invalida");
            }
        }

        private static void ValidareAtomi(string[] atomi)
        {
            char[] acceptedChars = {  '+', '-', '/', '%', '*', '.' };
            for (int i = 0; i < atomi.Length - 1; i++)
            {
                if (InArray(atomi[i][0], acceptedChars) && InArray(atomi[i + 1][0], acceptedChars))
                {
                    throw new Exception("Expresia introdusa este invalida");
                }
                if(CountOccurances('.', atomi[i]) > 1)
                {
                    throw new Exception("Expresia introdusa este invalida");
                }
                if (CountOccurances(' ', atomi[i]) > 0)
                {
                    throw new Exception("Expresia introdusa este invalida");
                }
            }
        }

        /// <summary>
        /// Counts the occurances of the character <c>needle</c> in the desired <c>haystack</c>
        /// </summary>
        /// <param name="needle">The character to be searched for</param>
        /// <param name="haystack">The string where to search</param>
        /// <returns>The number of occurances</returns>
        private static int CountOccurances(char needle, string haystack)
        {
            int contor = 0;
            for (int i = 0; i < haystack.Length; i++)
            {
                if(haystack[i] == needle)
                {
                    contor++;
                }
            }
            return contor;
        }


    }
}

