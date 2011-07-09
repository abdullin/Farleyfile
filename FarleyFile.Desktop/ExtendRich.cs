using System;
using System.Drawing;
using System.Windows.Forms;

namespace FarleyFile
{
    public static class ExtendRich
    {
        sealed class DisposableAction : IDisposable
        {
            readonly Action _dispose;

            public DisposableAction(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose();
            }
        }

        public static IDisposable Styled(this RichTextBox box, Color color, bool bold = false, int indent = 0)
        {
            var selectionColor = box.SelectionColor;
            var font = box.SelectionFont;
            var oldindent = box.SelectionIndent;

            if (color != Color.Empty)
            {
                box.SelectionColor = color;
            }
            if (bold)
            {
                box.SelectionFont = new Font(box.Font, box.Font.Style | FontStyle.Bold);
            }
            if (indent != 0)
            {
                box.SelectionIndent = indent;
            }


            return new DisposableAction(() =>
                {
                    box.SelectionColor = selectionColor;
                    box.SelectionFont = font;
                    box.SelectionIndent = oldindent;
                });
        }

        public static void AppendLine(this RichTextBox box, string format, params object[] args)
        {
            box.AppendText(string.Format(format, args));
            box.AppendText(Environment.NewLine);
        }

        public static void AppendLine(this RichTextBox box)
        {
            box.AppendText(Environment.NewLine);
        }
    }
}