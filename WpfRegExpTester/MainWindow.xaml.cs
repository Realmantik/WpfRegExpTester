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
using System.Text.RegularExpressions;

namespace WpfRegExpTester
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {

            string outText = inputTxt.Text;
            bool replaced = false;

            rtb.SetText(inputTxt.Text);
            IEnumerable<TextRange> wordRanges = null;
            wordRanges = GetAllWordRangesEnh(rtb.Document, patternTxt.Text);

            foreach (TextRange wordRange in wordRanges)
            {
                if (Regex.IsMatch(wordRange.Text, patternTxt.Text))
                {
                    if (!string.IsNullOrEmpty(replaceTxt.Text))
                    {
                        outText = outText.Replace(wordRange.Text, replaceTxt.Text);
                        replaced = true;
                    }
                    else
                    {
                        wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    }
                }

            }
            if (replaced)
            {
                rtb.SetText(outText);
            }
            
        }

        public static IEnumerable<TextRange> GetAllWordRanges(FlowDocument document, string pattern)
        {
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    MatchCollection matches = Regex.Matches(textRun, pattern);
                    foreach (Match match in matches)
                    {
                        int startIndex = match.Index;
                        int length = match.Length;
                        TextPointer start = pointer.GetPositionAtOffset(startIndex);
                        TextPointer end = start.GetPositionAtOffset(length);
                        yield return new TextRange(start, end);
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        public  IEnumerable<TextRange> GetAllWordRangesEnh(FlowDocument document, string pattern)
        {
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    MatchCollection matches = null;
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    try
                    {
                        matches = Regex.Matches(textRun, pattern);
                    }
                    catch (Exception ex)
                    {
                        logTxt.Items.Add(ex.Message);
                        yield break;
                    }
                    

                    for (int i = 0; i < matches.Count; i++)
                    {
                        int startIndex = matches[i].Index;
                        int length = 1;

                        do
                        {
                            i++;
                            length++;
                            if (i + 1 >= matches.Count)
                            {
                                break;
                            }
                        }
                        while (matches[i].Index + 1 == matches[i + 1].Index);
                        TextPointer start = pointer.GetPositionAtOffset(startIndex);
                        TextPointer end = start.GetPositionAtOffset(length);
                        yield return new TextRange(start, end);
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }


    public static class Ext
    {
        public static void SetText(this RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd).Text;
        }
    }
}
