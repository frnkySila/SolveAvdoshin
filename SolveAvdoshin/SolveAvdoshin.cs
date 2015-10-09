﻿using System;

namespace SolveAvdoshin
{
	public class SolveAvdoshin
	{
		static string PrintEquation(int[] coefs)
		{
			string result = "";

			for(int i = 0; i < 9; i++) {
				result += i == 8 ? " = " : i == 0 ? "" : " ⨁ ";

				if(coefs[i] == -1)
					result += "___";
				else
					result += coefs[i].ToString("D");

				result += i == 8 ? "" : " x" + (7 - i).ToString("D");
			}

			return result;
		}

		static int ReadInt(string msg, Func<int, bool> constraint)
		{
			int res;

			Console.WriteLine("(ввести целое число от 0 до 255)");

			Console.Write(msg + ": ");

			while(!int.TryParse(Console.ReadLine(), out res) || !constraint(res)) {
				Console.Write("Хуйню ввел. Заново: ");
			}

			return res;
		}

		static int[] ConsoleInput()
		{
			//return new int[] { 220, 160, 85, 253, 210, 159, 103, 101, 72, };

			int[] coefs = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, };

			Console.Clear();

			for(int i = 0; i < 9; i++) {
				Console.WriteLine(PrintEquation(coefs) + "\n");

				coefs[i] = ReadInt("Вводи епт", x => 0 <= x && x <= 255);

				Console.Clear();
			}

			return coefs;
		}

		static int[] ReadArgs(string[] args)
		{
			int[] coefs = new int[9];

			if(args.Length == 0) {
				throw new ArgumentNullException();
			}
			else if(args.Length == 9) {

				for(int i = 0; i < 9; i++) {
					coefs[i] = int.Parse(args[i]);

					if(coefs[i] < 0 || coefs[i] > 255)
						throw new FormatException("Все аргументы должны быть целыми числами от 0 до 255");
				}
			}
			else {
				throw new FormatException("Аргументов то ли многовато, то ли маловато. Надо ровно 9");
			}

			return coefs;
		}

		static int Solve(int[] coefs)
		{
			Console.WriteLine(PrintEquation(coefs));

			int eqAnswer = AndXorEquation.SolveEq(coefs);

			return eqAnswer;
		}

		enum ExecutionMode { Interactive, CommandLine, NoEquation, };

		static bool ProcessCommandLineArgs(string[] args, out ExecutionMode? mode, out int a, out int b,
			out int[] coefs, out int n)
		{
			coefs = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, };
			mode = null;
			a = b = n = -1;

			for(int i = 0; i < args.Length; i++) {
				switch(args[i]) {
				case "-i":
					if(mode != null)
						return false;
					
					mode = ExecutionMode.Interactive;

					break;

				case "-p":
					if(mode != null)
						return false;
					
					mode = ExecutionMode.CommandLine;

					if(args.Length - i - 1 != 9)
						return false;

					for(int j = 0; j < 9; j++) {
						if(!int.TryParse(args[i + j + 1], out coefs[j]))
							return false;
						
						if(!(0 <= coefs[j] && coefs[j] <= 255))
							return false;
					}

					i += 9;

					break;

				case "-f":
					if(mode != null)
						return false;
					
					mode = ExecutionMode.NoEquation;

					if(args.Length - i - 1 != 1)
						return false;
					
					if(!int.TryParse(args[i + 1], out n))
						return false;

					if(!(0 <= n && n <= 255))
						return false;

					i += 1;
					
					break;

				case "-ex":
					if(a != -1)
						return false;

					a = 0;

					if(args.Length - i - 1 < 1)
						return false;

					if(args[i + 1].Contains("-")) {
						string[] ab = args[i + 1].Split('-');

						if(!int.TryParse(ab[0], out a) || !int.TryParse(ab[1], out b))
							return false;

						if(!(0 <= a && a <= 255) || !(0 <= b && b <= 255))
							return false;
					}
					else {
						if(!int.TryParse(args[i + 1], out a))
							return false;

						if(!(0 <= a && a <= 255))
							return false;
					}

					i += 1;

					break;

				default:
					int tmp;
					if(!int.TryParse(args[i], out tmp))
						return false;
					break;
				}
			}

			if(a == -1) {
				a = 1;
				b = 15;
			}

			return mode != null;
		}

		static void PrintAnswers(int n, int a, int b)
		{
			var functions = new Action<int>[] {
				BooleanFunctions.PrintMinimaInAvdoshinBases, 
				BooleanFunctions.PrintDerivatives, 
				BooleanFunctions.PrintExpressionsForDerivatives, 
				BooleanFunctions.Print2DirectionalDerivatives, 
				BooleanFunctions.PrintExpressionsFor2DirDerivatives, 
				BooleanFunctions.Print3DirectionalDerivative, 
				BooleanFunctions.PrintExpressionFor3DirDerivative, 
				BooleanFunctions.PrintMaclauren1XorAnd, 
				BooleanFunctions.PrintTailor1XorAnd, 
				BooleanFunctions.PrintMaclauren0EqOr, 
				BooleanFunctions.PrintTailor0EqOr, 
				BooleanFunctions.PrintClosedClasses, 
				BooleanFunctions.PrintRepresentBinariesInF,
			};

			a = Math.Max(a, 3);
			b = Math.Max(b, 3);

			for(int i = a; i <= b; i++) {
				Console.WriteLine("\n{0}", i ==3 ? "2-3." : i + ".\n");

				functions[i - 3](n);
			}
		}

		public static void Main(string[] args)
		{
			int[] coefs = { -1, -1, -1, -1, -1, -1, -1, -1, -1, };

			ExecutionMode? mode;
			int a, b;
			int n;

			if(ProcessCommandLineArgs(args, out mode, out a, out b, out coefs, out n)) {
				switch(mode) {
				case ExecutionMode.Interactive:
					coefs = ConsoleInput();

					goto case ExecutionMode.CommandLine;

				case ExecutionMode.CommandLine:
					n = Solve(coefs);

					Console.WriteLine("\n1.\n\nОтвет: " + n + "\n");

					break;
				case ExecutionMode.NoEquation:
					Console.WriteLine("\n1.\n\nДана функция: " + n + "\n");

					break;
				}

				if(a == -1 && b == -1)
					PrintAnswers(n, 3, 15);
				else if(a != -1 && b == -1)
					PrintAnswers(n, a, a);
				else
					PrintAnswers(n, a, b);

				Console.WriteLine();
			}
			else {
				Console.WriteLine(
					"usage: SolveAvdoshin [<options>] [<mode> ...]\n" +
					"\t-i\t\t\tввод коэффициентов с клавиатуры\n" +
					"\t-p <a7> ... <a1> <b>\tввод коэффициентов (чисел 9 штук)\n" +
					"\t-f <n>\t\t\tввод сразу номера функции\n" +
					"\n" +
					"\t-ex <a>\t\t\tрешать только <a>-е задание\n" +
					"\t-ex <a>-<b>\t\tрешать задания с <a> по <b> включительно\n" +
					"\t\t\t\t(прим.: всего заданий 15)");
			}


			/*catch(FormatException e) {
        Console.WriteLine("Osheebka: " + e.Message);
      }
      catch(Exception e) {
        Console.WriteLine("\nНепойманное исключение: " + e.Message);
        Console.WriteLine("\n---------------------------------------------------------------");
        Console.WriteLine("| Report at https://github.com/frnkySila/SolveAvdoshin/issues |");
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine("\n" + e.StackTrace);
      }*/
		}
	}
}

