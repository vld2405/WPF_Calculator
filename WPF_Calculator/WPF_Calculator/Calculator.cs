using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF_Calculator
{
    class Calculator : INotifyPropertyChanged
    {
        private double _result = 0.0;
        private double _rightOperand = 0.0;
        private string _numberShown = "0";
        private string _numberShownDigitGrouping = "0";
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
                NumberShownDigitGrouping = NumberShown;
            }
        }
        
        public string NumberShownDigitGrouping
        {
            get => _numberShownDigitGrouping;
            set
            {
                if(_digitGroupingFlag)
                    _numberShownDigitGrouping = double.TryParse(value, out double num) ? num.ToString("#,##0.########") : "0";
                else
                    _numberShownDigitGrouping = value;
                NotifyPropertyChanged("NumberShownDigitGrouping");
            }
        }

        public void AddDigit(int digit)
        {
            if(_equalPressed)
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
                if(NumberShown.Length > 0)
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
            if (RightOperand != 0)
            {
                if (EquationString != "")
                {
                    if (EquationString.Contains("="))
                        EquationString = Result.ToString();
                    EquationString = $"(1/{EquationString})";
                }
                else
                    EquationString = $"(1/{RightOperand})";



                RightOperand = 1 / RightOperand;
                NumberShown = RightOperand.ToString();
            }
        }

        public void Percent()
        {
            RightOperand = RightOperand / 100;
            NumberShown = RightOperand.ToString();
        }

        public void Squared()
        {
            if (RightOperand != 0)
            {
                if (EquationString != "")
                {
                    if (EquationString.Contains("="))
                        EquationString = Result.ToString();
                    EquationString = $"({EquationString}^2)";
                }
                else
                    EquationString = $"({RightOperand}^2)";

                RightOperand = Math.Pow(RightOperand, 2);
                NumberShown = RightOperand.ToString();
            }
        }

        public void SquareRoot()
        {
            if (EquationString != "")
            {
                if (EquationString.Contains("="))
                    EquationString = Result.ToString();
                EquationString = $"(√{EquationString})";
            }
            else
                EquationString = $"(√{RightOperand})";


            RightOperand = Math.Sqrt(RightOperand);
            NumberShown = RightOperand.ToString();
        }

        public void ReverseSign()
        {
            if(RightOperand != 0)
            {
                RightOperand = -RightOperand;
                NumberShown = RightOperand.ToString();
            }
        }

        private void SetOperation(string operation, string displaySymbol)
        {
            if(_equalPressed)
                _equalPressed = false;

            if (_lastOperation == "")
            {
                Result = RightOperand;
                EquationString = Result.ToString() + " " + displaySymbol + " ";
            }
            else if (_lastOperation != operation && NumberShown == "0")
            {
                EquationString = EquationString.Substring(0, EquationString.Length - 2) + displaySymbol + " ";
            }
            else if (_lastOperation != "")
            {
                PerformLastOperation();
                EquationString = Result.ToString() + " " + displaySymbol + " ";
            }

            _lastOperation = operation;
            NumberShown = "0";
            RightOperand = 0.0;
        }

        public void Addition()
        {
            SetOperation("+", "+");
        }

        public void Subtraction()
        {
            SetOperation("-", "-");
        }

        public void Multiplication()
        {
            SetOperation("*", "×");
        }

        public void Division()
        {
            SetOperation("/", "÷");
        }

        public void Equals()
        {
            if (_lastOperation != "")
            {
                double rightOperandBeforeOperation = RightOperand;
                PerformLastOperation();
                if (_rightOperand != 0)
                    EquationString = EquationString + rightOperandBeforeOperation.ToString() + " = ";
                else
                    EquationString = EquationString.Substring(0, EquationString.Length - 2);
                NumberShown = Result.ToString();
                RightOperand = Result;
                _lastOperation = "";
                _equalPressed = true;
            }
        }

        private void PerformLastOperation()
        {
            switch (_lastOperation) 
            {
                case "+":
                    Result += RightOperand;
                    break;
                case "-":
                    Result -= RightOperand;
                    break;
                case "*":
                    Result *= RightOperand;
                    break;
                case "/":
                    if (RightOperand != 0)
                        Result /= RightOperand;
                    else
                        MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
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
            _copiedText = NumberShown;
        }
        
        public void PasteToClipboard()
        {
            if(_copiedText != "")
            {
                NumberShown = _copiedText;
                RightOperand = double.Parse(NumberShown);
            }
        }

        public void CutToClipboard()
        {
            _copiedText = NumberShown;
            NumberShown = "0";
            RightOperand = 0.0;
        }

        public void ToggleGrouping()
        {
            _digitGroupingFlag = !_digitGroupingFlag;
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
