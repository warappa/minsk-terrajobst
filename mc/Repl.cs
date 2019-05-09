using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Minsk
{
    internal abstract class Repl
    {
        private List<string> submissionHistory = new List<string>();
        private int submissionHistoryIndex;
        private bool done;

        public void Run()
        {
            while (true)
            {
                var text = EditSubmission();
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (!text.Contains(Environment.NewLine) & text.StartsWith("#"))
                {
                    EvaluateMetaCommand(text);
                }
                else
                {
                    EvaluateSubmission(text);
                }

                submissionHistory.Add(text);
                submissionHistoryIndex = 0;
            }
        }

        private sealed class SubmissionView
        {
            private readonly Action<string> lineRenderer;
            private readonly ObservableCollection<string> submissionDocument;
            private readonly int cursorTop;
            private int renderedLineCount;
            private int _currentLineIndex;
            private int _currentCharacter;

            public SubmissionView(Action<string> lineRenderer, ObservableCollection<string> submissionDocument)
            {
                this.lineRenderer = lineRenderer;
                this.submissionDocument = submissionDocument;
                submissionDocument.CollectionChanged += SumbissionDocumentChanged;
                cursorTop = Console.CursorTop;
                Render();
            }

            private void SumbissionDocumentChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                Render();
            }

            private void Render()
            {
                var lineCount = 0;

                Console.CursorVisible = false;

                foreach (var line in submissionDocument)
                {
                    Console.SetCursorPosition(0, cursorTop + lineCount);
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (lineCount == 0)
                    {
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.Write("| ");
                    }
                    Console.ResetColor();
                    lineRenderer(line);

                    Console.Write(new string(' ', Console.WindowWidth - line.Length));

                    lineCount++;
                }


                var numberOfBlankLines = renderedLineCount - lineCount;
                if (numberOfBlankLines > 0)
                {
                    var blankLine = new string(' ', Console.WindowWidth);
                    for (var i = 0; i < numberOfBlankLines; i++)
                    {
                        Console.SetCursorPosition(0, cursorTop + lineCount + i);
                        Console.WriteLine(blankLine);
                    }
                }

                renderedLineCount = lineCount;
                Console.CursorVisible = true;
                UpdateCursorPosition();
            }

            private void UpdateCursorPosition()
            {
                Console.CursorTop = cursorTop + CurrentLine;
                Console.CursorLeft = 2 + CurrentCharacter;
            }

            public int CurrentLine
            {
                get { return _currentLineIndex; }
                set
                {
                    if (_currentLineIndex != value)
                    {
                        _currentLineIndex = value;
                        _currentCharacter = Math.Min(submissionDocument[_currentLineIndex].Length, _currentCharacter);

                        UpdateCursorPosition();
                    }
                }
            }

            public int CurrentCharacter
            {
                get { return _currentCharacter; }
                set
                {
                    if (_currentCharacter != value)
                    {
                        _currentCharacter = value;
                        UpdateCursorPosition();
                    }
                }

            }
        }

        protected virtual void RenderLine(string line)
        {
            Console.Write(line);
        }

        private string EditSubmission()
        {
            done = false;
            var document = new ObservableCollection<string>() { "" };
            var view = new SubmissionView(RenderLine, document);
            while (!done)
            {
                var key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            view.CurrentLine = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLine].Length;

            Console.WriteLine();

            if (document.Count == 1 && document[0].Length == 0)
            {
                return null;
            }

            return string.Join(Environment.NewLine, document);
        }

        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == default(ConsoleModifiers))
            {
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        HandleEscape(document, view);
                        break;
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(document, view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                    default:

                        break;

                }
            }
            else if (key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
            }

            if (key.KeyChar >= ' ')
            {
                HandleTyping(document, view, key.KeyChar.ToString());
            }
        }

        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document[view.CurrentLine] = string.Empty;
            view.CurrentCharacter = 0;
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            var docText = string.Join(Environment.NewLine, document);
            if (docText.StartsWith("#") || IsCompleteSubmission(docText))
            {
                done = true;
                return;
            }

            InsertLine(document, view);
        }


        private void HandleControlEnter(ObservableCollection<string> document, SubmissionView view)
        {
            InsertLine(document, view);
        }

        private static void InsertLine(ObservableCollection<string> document, SubmissionView view)
        {
            var remainder = document[view.CurrentLine].Substring(view.CurrentCharacter);
            document[view.CurrentLine] = document[view.CurrentLine].Substring(0, view.CurrentCharacter);

            var lineIndex = view.CurrentLine + 1;
            document.Insert(lineIndex, remainder);
            view.CurrentCharacter = 0;
            view.CurrentLine = lineIndex;
        }
        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentCharacter > 0)
            {
                view.CurrentCharacter--;
            }
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            var line = document[view.CurrentLine];
            if (view.CurrentCharacter < line.Length)
            {
                view.CurrentCharacter++;
            }
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine > 0)
            {
                view.CurrentLine--;
            }
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine < document.Count - 1)
            {
                view.CurrentLine++;
            }
        }

        private void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLine;
            var start = view.CurrentCharacter;
            if (start == 0)
            {
                if (view.CurrentLine == 0)
                {
                    return;
                }

                var currentLine = document[view.CurrentLine];
                var previousLine = document[view.CurrentLine - 1];
                document.RemoveAt(view.CurrentLine);
                view.CurrentLine--;
                view.CurrentCharacter = document[view.CurrentLine].Length;
                document[view.CurrentLine] = previousLine + currentLine;

                return;
            }

            var line = document[lineIndex];

            var before = line.Substring(0, start - 1);
            var after = line.Substring(start);
            document[lineIndex] = before + after;
            view.CurrentCharacter--;
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLine;
            var line = document[lineIndex];
            var start = view.CurrentCharacter;
            if (start >= line.Length)
            {
                if (view.CurrentLine == document.Count - 1)
                {
                    return;
                }

                var nextLine = document[view.CurrentLine + 1];
                document[view.CurrentLine] += nextLine;
                document.RemoveAt(view.CurrentLine + 1);
                return;
            }

            var before = line.Substring(0, start);
            var after = line.Substring(start + 1);
            document[lineIndex] = before + after;
        }

        private void HandleEnd(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLine;
            var line = document[lineIndex];
            view.CurrentCharacter = line.Length;
        }

        private void HandleHome(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacter = 0;
        }

        private void HandleTab(ObservableCollection<string> document, SubmissionView view)
        {
            const int TabWidth = 4;
            var start = view.CurrentCharacter;
            var remainingSpaces = TabWidth - start % TabWidth;

            var line = document[view.CurrentLine];
            document[view.CurrentLine] = line.Insert(start, new string(' ', remainingSpaces));
            view.CurrentCharacter += remainingSpaces;
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            submissionHistoryIndex--;
            if (submissionHistoryIndex < 0)
            {
                submissionHistoryIndex = submissionHistory.Count - 1;
            }
            UpdateDocumentFromHistory(document, view);
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            submissionHistoryIndex++;
            if (submissionHistoryIndex >= submissionHistory.Count)
            {
                submissionHistoryIndex = 0;
            }
            UpdateDocumentFromHistory(document, view);
        }

        private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
        {
            if (submissionHistory.Count == 0)
                return;

            document.Clear();

            var historyItem = submissionHistory[submissionHistoryIndex];
            var lines = historyItem.Split(Environment.NewLine);

            foreach (var line in lines)
            {
                document.Add(line);
            }

            view.CurrentLine = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLine].Length;
        }


        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, string text)
        {
            var lineIndex = view.CurrentLine;
            var start = view.CurrentCharacter;
            document[lineIndex] = document[lineIndex].Insert(start, text);
            view.CurrentCharacter += text.Length;
        }

        protected void ClearHistory()
        {
            submissionHistory.Clear();
        }

        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid command {input}");
            Console.ResetColor();
        }

        protected abstract bool IsCompleteSubmission(string text);
        protected abstract void EvaluateSubmission(string text);
    }
}
