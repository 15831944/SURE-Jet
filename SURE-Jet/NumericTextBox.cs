using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Simple_Grapher
{
    public class NumericTextBox : TextBox
    {
        bool allowSpace = false;
        bool firstChar = true;

        // Restricts the entry of characters to digits (including hex), the negative sign, 
        // the decimal point, and editing keystrokes (backspace). 
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            // Workaround for groupSeparator equal to non-breaking space 
            if (groupSeparator == ((char)160).ToString())
            {
                groupSeparator = " ";
            }

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if(keyInput.Equals(decimalSeparator))
            {
                if (noDecimals())
                {
                    //one decimal is good
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (keyInput.Equals(negativeSign))
            {
                if (noNegatives())
                {
                    //one decimal is good
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0) 
            //    { 
            //     // Let the edit control handle control and alt key combinations 
            //    } 
            else if (this.allowSpace && e.KeyChar == ' ')
            {

            }
            else
            {
                // Consume this invalid key and beep
                e.Handled = true;
                return;
                //    MessageBeep();
            }
            base.OnKeyPress(e);
        }

        private bool noDecimals()
        {
            if (this.SelectedText.Contains("."))
                return true;
            return !this.Text.Contains(".");
        }
        private bool noNegatives()
        {
            if (this.SelectedText.Contains("-"))
                return true;
            return !this.Text.Contains("-");
        }

        public int IntValue
        {
            get
            {
                return Int32.Parse(this.Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(this.Text);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }
    }
}
