using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HammingCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool GetBoolFromColor(Color color)
        {
            return color != Colors.MediumPurple;
        }
        private static Color GetColorFromBool(bool bul)
        {
            return bul ? Colors.Purple : Colors.MediumPurple;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            foreach (Button b in FindVisualChilds<Button>(this))
            {
                SetValueToControl(b, false);
            }
        }

        private void ChangeBitInData(object sender, RoutedEventArgs e)
        {
            if (sender is not Control c)
                return;
            SetValueToControl(c, !GetValueFromControl(c));
            CalculateCodeFromWord();

        }

        private void ChangeBitInCode(object sender, RoutedEventArgs e)
        {
            if (sender is not Control c)
                return;
            SetValueToControl(c, !GetValueFromControl(c));
            CalculateWordFromCode();
        }

        private void CalculateCodeFromWord()
        {
            bool[] word = FindVisualChilds<Button>(this)
                .Where(b => Grid.GetRow(b) == 0)
                .OrderBy(b => Grid.GetColumn(b))
                .Select(b => GetValueFromControl(b))
                .ToArray();
            bool[] code = MultiplyVectorByMatrix(word, matrixG);

            Button[] buttons = FindVisualChilds<Button>(this)
                .Where(b => Grid.GetRow(b) == 1)
                .OrderBy(b => Grid.GetColumn(b))
                .ToArray();
            if (buttons.Length != code.Length)
                return;
            for (int i = 0; i < buttons.Length; i++)
            {
                SetValueToControl(buttons[i], code[i]);
            }
        }

        private void CalculateWordFromCode()
        {
            bool[] code = FindVisualChilds<Button>(this)
                .Where(b => Grid.GetRow(b) == 3)
                .OrderBy(b => Grid.GetColumn(b))
                .Select(b => GetValueFromControl(b))
                .ToArray();
            bool[] check = MultiplyVectorByMatrix(code, matrixHT);
            for(int i = 0; i < code.Length; i++)
            {
                if(CompareArrays(check, matrixHT.GetRow(i)))
                    code[i] = !code[i];
            }
            bool[] word = code[0..4];
            Button[] buttons = FindVisualChilds<Button>(this)
                .Where(b => Grid.GetRow(b) == 4)
                .OrderBy(b => Grid.GetColumn(b))
                .ToArray();
            if (buttons.Length != word.Length)
                return;
            for (int i = 0; i < buttons.Length; i++)
            {
                SetValueToControl(buttons[i], word[i]);
            }

        }

        private bool GetValueFromControl(Control control)
        {
            Brush backgroundColor = control.Background;
            if (backgroundColor is not SolidColorBrush sbc)
                return false;

            Color color = sbc.Color;
            return GetBoolFromColor(color);
        }

        private void SetValueToControl(Control control, bool value)
        {
            Color newColor = GetColorFromBool(value);
            control.Background = new SolidColorBrush(newColor);
        }

        private bool[] MultiplyVectorByMatrix(bool[] vector, bool[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            if (rows != vector.Length)
            {
                return Array.Empty<bool>();
            } 
            bool[] result = new bool[columns];
            for (int i = 0; i < columns; i++)
            {
                result[i] = false;
                for (int j = 0; j < rows; j++)
                {
                    result[i] ^= vector[j] & matrix[j, i];
                }
            }
            return result;
        }

        private static bool[,] matrixG = new bool[4, 7]
        {
            {true, false, false, false, true, true, true},
            {false, true, false, false, true, true, false},
            {false, false, true, false, false, true, true},
            {false, false, false, true, true, false, true}
        };

        private static bool[,] matrixHT = new bool[7, 3]
        {
            {true, true, true},
            {true, true, false},
            {false, true, true},
            {true, false, true},
            {true, false, false},
            {false, true, false},
            {false, false, true}
        };

        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }
        private bool CompareArrays<T>(T[] a, T[] b) where T : notnull
        {
            if (a.Length != b.Length) 
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }
            return true;

        }
    }
}
