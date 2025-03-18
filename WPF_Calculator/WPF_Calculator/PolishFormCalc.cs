using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF_Calculator
{
    class PolishFormCalc : INotifyPropertyChanged
    {
        private double _result = 0.0;
        private double _rightOperand = 0.0;
        private string _numberShown = "0";
        private string _equationString = "";
        private string _lastOperation = "";

        private string _copiedText = "";

        private bool _equalPressed = false;
        private bool _digitGroupingFlag = false;

        private ObservableCollection<double> _memoryList = new ObservableCollection<double>();

        public ObservableCollection<double> MemoryList
        {
            get => _memoryList;
            set
            {
                _memoryList = value;
            }
        }

        public string EquationString
        {
            get => _equationString;
            set
            {
                _equationString = value;
                NotifyPropertyChanged("EquationString");
            }
        }

        public double Result
        {
            get => _result;
            set
            {
                _result = value;
                NotifyPropertyChanged("Result");
            }
        }

        public double RightOperand
        {
            get => _rightOperand;
            set
            {
                _rightOperand = value;
                NotifyPropertyChanged("RightOperand");
            }
        }

        public string NumberShown
        {
            get => _numberShown;
            set
            {
                _numberShown = value;
                NotifyPropertyChanged("NumberShown");
            }
        }

        public void AddDigit(int digit)
        {
            if (_equalPressed)
            {
                _equalPressed = false;
                ClearResult();
            }

            if (NumberShown == "0")
                NumberShown = "";

            NumberShown += digit.ToString();
            RightOperand = double.Parse(NumberShown);
        }

        public void MakeDecimal()
        {
            if (!NumberShown.Contains("."))
                NumberShown += ".";
        }

        public void ClearRightOperand()
        {
            NumberShown = "0";
            RightOperand = 0.0;
        }

        public void ClearResult()
        {
            NumberShown = "0";
            EquationString = "";
            Result = 0.0;
        }

        public void DeleteDigit()
        {
            if (NumberShown.Length > 0)
            {
                NumberShown = NumberShown.Substring(0, NumberShown.Length - 1);
                if (NumberShown.Length > 0)
                    RightOperand = double.Parse(NumberShown);
            }
            if (NumberShown.Length == 0)
            {
                NumberShown = "0";
                RightOperand = 0.0;
            }
        }

        public void Inverted()
        {
            if(NumberShown != "0")
            {
                EquationString += $" 1 / {NumberShown} ";
                NumberShown = "0";
            }
        }

        public void Percent()
        {
            EquationString += NumberShown + " / 100 ";
            NumberShown = "0";
        }

        public void Squared()
        {
            EquationString += NumberShown + " ^ 2 ";
            NumberShown = "0";
        }

        public void Addition()
        {
            if (NumberShown == "0")
            {
                EquationString += " + ";
            }
            else
            {
                EquationString += NumberShown + " + ";
            }
            NumberShown = "0";
        }

        public void Subtraction()
        {
            if(NumberShown == "0")
            {
                EquationString += " - ";
            }
            else
            {
                EquationString += NumberShown + " - ";
            }
            NumberShown = "0";
        }

        public void Multiplication()
        {
            if (NumberShown == "0")
            {
                EquationString += " * ";
            }
            else
            {
                EquationString += NumberShown + " * ";
            }
            NumberShown = "0";
        }

        public void Division()
        {
            if (NumberShown == "0")
            {
                EquationString += " / ";
            }
            else
            {
                EquationString += NumberShown + " / ";
            }
            NumberShown = "0";
        }
        public void Equals()
        {
            if (_equalPressed)
                return;

            if (EquationString != "")
            {
                if (NumberShown != "0")
                {
                    EquationString += NumberShown;
                }
                EquationString += " = ";
            }
            else
            {
                EquationString = NumberShown + " = ";
            }

            Result = calculFormaPoloneza(FormaPoloneza(EquationString));
            NumberShown = Result.ToString();

            _equalPressed = true;
        }

        private int Priority(char c)
        {
            switch (c)
            {
                default:
                    return 0;
                case '+':
                    return 1;

                case '-':
                    return 1;

                case '*':
                    return 2;

                case '/':
                    return 2;

                case '^':
                    return 3;
            }
        }
        double Calculate(double a, double b, char op)
        {
            switch (op)
            {
                default:
                    return 0;
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    return a / b;
                case '^':
                    return Math.Pow(a, b);
            }
        }

        Queue<string> FormaPoloneza(string equationString)
        {
            Queue<string> result = new Queue<string>();
            Stack<char> operators = new Stack<char>();

            string[] tokens = equationString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (double.TryParse(token, out double number))
                {
                    result.Enqueue(token);
                }
                else if (token.Length == 1 && "+-*/^".Contains(token[0]))
                {
                    char op = token[0];

                    while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(op))
                    {
                        result.Enqueue(operators.Pop().ToString());
                    }

                    operators.Push(op);
                }
            }

            while (operators.Count > 0)
            {
                result.Enqueue(operators.Pop().ToString());
            }

            return result;
        }


        double calculFormaPoloneza(Queue<string> polish)
        {
            try
            {
                Stack<double> stack = new Stack<double>();

                while (polish.Count > 0)
                {
                    string token = polish.Dequeue();

                    if (double.TryParse(token, out double number))
                    {
                        stack.Push(number);
                    }
                    else if (token.Length == 1 && "+-*/^".Contains(token[0]))
                    {
                        char op = token[0];

                        if (stack.Count < 2)
                        {
                            throw new InvalidOperationException("Expresie invalidă - operatori insuficienți");
                        }

                        double b = stack.Pop();
                        double a = stack.Pop();
                        double result = Calculate(a, b, op);
                        stack.Push(result);
                    }
                }

                if (stack.Count != 1)
                {
                    throw new InvalidOperationException("Expresie invalidă - prea mulți operanzi");
                }

                return stack.Pop();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Eroare la calculul expresiei: " + ex.Message);
            }
        }


        public void MemoryClear()
        {
            _memoryList.Clear();
        }

        public void MemoryStore()
        {
            _memoryList.Add(RightOperand);
        }

        public void MemoryAdd(int selectedIndex)
        {
            _memoryList[selectedIndex] += RightOperand;
        }

        public void MemorySubtract(int selectedIndex)
        {
            _memoryList[selectedIndex] -= RightOperand;
        }

        public void MemoryRecall()
        {
            if (_memoryList.Count > 0)
            {
                RightOperand = _memoryList.Last();
                NumberShown = RightOperand.ToString();
            }
        }

        // Copy, paste, cut

        public void CopyToClipboard()
        {
            Clipboard.SetText(NumberShown);
        }

        public void PasteToClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                if (double.TryParse(text, out double pastedNumber))
                {
                    NumberShown = text;
                    RightOperand = pastedNumber;
                }
            }
        }

        public void CutToClipboard()
        {
            Clipboard.SetText(NumberShown);
            NumberShown = "0";
            RightOperand = 0.0;
        }


        public void ToggleGrouping()
        {
            _digitGroupingFlag = true;
        }

        // Handle property changes

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
