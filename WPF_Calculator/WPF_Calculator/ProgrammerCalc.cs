using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF_Calculator
{
    class ProgrammerCalc : INotifyPropertyChanged
    {
        private int _result = 0;
        private int _rightOperand = 0;
        private string _numberShown = "0";
        private string _equationString = "";
        private string _lastOperation = "";

        bool _equalPressed = false;

        private string _copiedText = "";

        private string _hexNumber = "";
        private string _decNumber = "";
        private string _octNumber = "";
        private string _binNumber = "";

        private bool _isHex = false;
        private bool _isDec = true;
        private bool _isOct = false;
        private bool _isBin = false;

        private ObservableCollection<int> _memoryList = new ObservableCollection<int>();

        public ObservableCollection<int> MemoryList
        {
            get => _memoryList;
            set
            {
                _memoryList = value;
            }
        }

        public string HexNumber
        {
            get => _hexNumber;
            set
            {
                _hexNumber = value;
                NotifyPropertyChanged("HexNumber");
            }
        }

        public string DecNumber
        {
            get => _decNumber;
            set
            {
                _decNumber = value;
                NotifyPropertyChanged("DecNumber");
            }
        }

        public string OctNumber
        {
            get => _octNumber;
            set
            {
                _octNumber = value;
                NotifyPropertyChanged("OctNumber");
            }
        }

        public string BinNumber
        {
            get => _binNumber;
            set
            {
                _binNumber = value;
                NotifyPropertyChanged("BinNumber");
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

        public int Result
        {
            get => _result;
            set
            {
                _result = value;
                NotifyPropertyChanged("Result");
            }
        }

        public int RightOperand
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
                ModifyDecNumber();
                ModifyBinNumber();
                ModifyOctNumber();
                ModifyHexNumber();
                NotifyPropertyChanged("NumberShown");
            }
        }

        public void ModifyDecNumber()
        {
            if (long.TryParse(NumberShown, out long number))
            {
                DecNumber = Convert.ToString(number, 10);
            }
        }

        public void ModifyBinNumber()
        {
            if (long.TryParse(NumberShown, out long number))
            {
                BinNumber = Convert.ToString(number, 2);
            }
        }

        public void ModifyOctNumber()
        {
            if (long.TryParse(NumberShown, out long number))
            {
                OctNumber = Convert.ToString(number, 8);
            }
        }

        public void ModifyHexNumber()
        {
            if (long.TryParse(NumberShown, out long number))
            {
                HexNumber = Convert.ToString(number, 16);
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
            RightOperand = int.Parse(NumberShown);
        }

        public void ClearRightOperand()
        {
            NumberShown = "0";
            RightOperand = 0;
        }

        public void ClearResult()
        {
            NumberShown = "0";
            EquationString = "";
            Result = 0;
        }

        public void DeleteDigit()
        {
            if (NumberShown.Length > 0)
            {
                NumberShown = NumberShown.Substring(0, NumberShown.Length - 1);
                if (NumberShown.Length > 0)
                    RightOperand = int.Parse(NumberShown);
            }
            if (NumberShown.Length == 0)
            {
                NumberShown = "0";
                RightOperand = 0;
            }
        }

        public void Squared()
        {
            if (EquationString != "")
            {
                if (EquationString.Contains("="))
                    EquationString = Result.ToString();
                EquationString = $"({EquationString}^2)";
            }
            else
                EquationString = $"({RightOperand}^2)";

            RightOperand = (int)Math.Pow(RightOperand, 2);
            NumberShown = RightOperand.ToString();
        }

        public void LeftBitShift()
        {
            RightOperand = RightOperand << 1;
            NumberShown = RightOperand.ToString();
        }

        public void RightBitShift()
        {
            RightOperand = RightOperand >> 1;
            NumberShown = RightOperand.ToString();
        }

        public void ReverseSign()
        {
            if (RightOperand != 0)
            {
                RightOperand = -RightOperand;
                NumberShown = RightOperand.ToString();
            }
        }

        private void SetOperation(string operation, string displaySymbol)
        {
            if (_equalPressed)
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
            SetOperation("÷", "÷");
        }
        
        public void Modulo()
        {
            SetOperation("%", "%");
        }

        public void Equals()
        {
            if (_lastOperation != "")
            {
                int rightOperandBeforeOperation = RightOperand;
                PerformLastOperation();
                EquationString = EquationString + rightOperandBeforeOperation.ToString() + " = ";
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
                case "%":
                    if (RightOperand != 0)
                        Result %= RightOperand;
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

        public void ToggleDec()
        {
            if (_isDec)
                return;
            _isDec = true;
            _isHex = false;
            _isOct = false;
            _isBin = false;
        }

        public void ToggleHex()
        {
            if (_isHex)
                return;
            _isDec = false;
            _isHex = true;
            _isOct = false;
            _isBin = false;
        }

        public void ToggleOct()
        {
            if (_isOct)
                return;
            _isDec = false;
            _isHex = false;
            _isOct = true;
            _isBin = false;
        }

        public void ToggleBin()
        {
            if (_isBin)
                return;
            _isDec = false;
            _isHex = false;
            _isOct = false;
            _isBin = true;
        }

        // Copy, paste, cut

        public void CopyToClipboard()
        {
            _copiedText = NumberShown;
        }

        public void PasteToClipboard()
        {
            if (_copiedText != "")
            {
                NumberShown = _copiedText;
                RightOperand = int.Parse(NumberShown);
            }
        }

        public void CutToClipboard()
        {
            _copiedText = NumberShown;
            NumberShown = "0";
            RightOperand = 0;
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
