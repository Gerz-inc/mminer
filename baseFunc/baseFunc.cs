using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Net;

//using System.Reflection;
//using System.Runtime.InteropServices;

namespace baseFunc
{
    public static class base_func
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void set_progress_bar_color(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }


        public static string json_escape(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Convert utf-8 to win1251
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string utf8ToWin1251(string msg)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(msg);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            return win1251.GetString(win1251Bytes);
        }

        /// <summary>
        /// Convert win1251 to UTF-8
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string win1251ToUtf8(string msg)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] win1251Bytes = utf8.GetBytes(msg);
            byte[] utf8Bytes = Encoding.Convert(win1251, utf8, win1251Bytes);

            return utf8.GetString(utf8Bytes);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getMd5Hash(string input)
        {
            try
            {
                MD5 md5Hasher = MD5.Create();

                // Преобразуем входную строку в массив байт и вычисляем хэш
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Создаем новый Stringbuilder (Изменяемую строку) для набора байт
                StringBuilder sBuilder = new StringBuilder();

                // Преобразуем каждый байт хэша в шестнадцатеричную строку
                for (int i = 0; i < data.Length; i++)
                {
                    //указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return (string)sBuilder.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// EXPLODE
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] explode(string separator, string source)
        {
            return source.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
 
        /// <summary>
        /// ОТПРАРСИТЬ ДАБЛ
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ParseDoubleSimple(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            Double ret = 0;
            try
            {
                ret = Double.Parse(s.Replace(".", ","));
            }
            catch (Exception ex)
            {
                ret = Double.Parse(s.Replace(",", "."));
            }
            return ret;
        }

        public static float ParseFloatSimple(string s, float def = 0)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            float ret = def;
            try
            {
                ret = float.Parse(s.Replace(".", ","));
            }
            catch (Exception ex)
            {
                ret = float.Parse(s.Replace(",", "."));
            }
            return ret;
        }

        public static float ParseFloat(object s, float def = 0)
        {
            if (s == null) return def;
            return ParseFloatSimple(s.ToString() == "" ? def.ToString() : s.ToString());
        }

        public static double ParseDouble(object s, double def = 0)
        {
            if (s == null) return def;
            return ParseDoubleSimple(s.ToString() == "" ? def.ToString() : s.ToString());
        }

        public static int ParseInt32(object s, int def = 0)
        {
            if (s == null) return def;
            return Int32.Parse(s.ToString() == "" ? def.ToString() : s.ToString());
        }

        public static short ParseInt16(object s, short def = 0)
        {
            if (s == null) return def;
            return Int16.Parse(s.ToString() == "" ? def.ToString() : s.ToString());
        }

        public static long ParseInt64(object s, long def = 0)
        {
            if (s == null) return def;
            return Int64.Parse(s.ToString() == "" ? def.ToString() : s.ToString());
        }

        public static string ParseString(object s, string def = "")
        {
            if (s == null) return def;
            return s.ToString();
        }

        public static string ParseStringMongoId(object s, string def = "")
        {
            if (s == null) return def;

            string ret = "";
            JObject obj = null;
            try
            {
                obj = JObject.Parse(s.ToString());
                ret = obj["$oid"].ToString();
            }
            catch (Exception ex) { }

            return ret;
        }

        public static string ParseStringDate(object s, string def = "")
        {
            if (s == null) return def;

            string dt = s.ToString().Trim();
            if (string.IsNullOrEmpty(dt)) return def;
            int type = 0;
            if (dt.Contains("-") && dt.Contains(":")) type = 0;
            else if (dt.Contains("-") && !dt.Contains(":")) type = 1;
            else if (!dt.Contains("-") && dt.Contains(":")) type = 2;

            DateTime dtm = ParseDateTime(dt);

            string ret = "";
            if (type == 0) ret = dtm.ToString("yyyy-MM-dd HH:mm:ss");
            else if (type == 1) ret = dtm.ToString("yyyy-MM-dd");
            else if (type == 2) ret = dtm.ToString("HH:mm:ss");

            if (dtm == DateTime.MinValue) ret = "";

            return ret;
        }

        public static DateTime ParseDateTime(object s)
        {
            DateTime t = DateTime.MinValue;
            if (s == null) return t;

            string dt = s.ToString().Trim();
            if (string.IsNullOrEmpty(dt)) return t;
            if (dt.Contains("."))
            {
                string[] exp = explode(".", dt);
                dt = exp[0];
            }

            int type = 0;
            if (dt.Contains("-") && dt.Contains(":")) type = 0;
            else if (dt.Contains("-") && !dt.Contains(":")) type = 1;
            else if (!dt.Contains("-") && dt.Contains(":")) type = 2;

            bool exep = false;
            try
            {
                if (type == 0)
                {
                    if (dt.CompareTo("0-00-00 00:00:00") == 0) return t;
                    t = DateTime.ParseExact(dt, "yyyy-M-d H:m:s", CultureInfo.InvariantCulture);
                }
                else if (type == 1)
                {
                    if (dt.CompareTo("0-00-00") == 0) return t;
                    t = DateTime.ParseExact(dt, "yyyy-M-d", CultureInfo.InvariantCulture);
                }
                else if (type == 2) t = DateTime.ParseExact(dt, "H:m:s", CultureInfo.InvariantCulture);
            }
            catch (Exception ex) { Console.WriteLine(dt + " " + ex.Message); exep = true; }

            if (exep)
            {
                try
                {
                    if (type == 0)
                    {
                        if (dt.CompareTo("0-00-00 00:00:00") == 0) return t;
                        t = DateTime.ParseExact(dt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    else if (type == 1)
                    {
                        if (dt.CompareTo("0-00-00") == 0) return t;
                        t = DateTime.ParseExact(dt, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    else if (type == 2) t = DateTime.ParseExact(dt, "HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex) { Console.WriteLine(dt + " " + ex.Message); }
            }

            return t;
        }

        /// <summary>
        /// Проверяет string состоящий из только ли цифр или нет
        /// </summary>
        /// <param name="str">проверяемая строка</param>
        /// <returns></returns>
        public static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            foreach (char c in str)
            {
                if (!Char.IsDigit(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет string содержит ли цифру
        /// </summary>
        /// <param name="str">проверяемая строка</param>
        /// <returns></returns>
        public static bool IsDigitsContains(string str)
        {
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// УСТАНОВЛЕН ЛИ БИТ
        /// </summary>
        /// <param name="num"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static bool isBitSetted(short num, int bit)
        {
            return (num & ((short)1 << bit)) != 0;
        }
        public static bool isBitSetted(int num, int bit)
        {
            return (num & ((int)1 << bit)) != 0;
        }
        public static bool isBitSetted(long num, int bit)
        {
            return (num & ((long)1 << bit)) != 0;
        }

        public static void setBit(ref short num, int bit)
        {
            num |= (short)((short)1 << bit);
        }
        public static void setBit(ref int num, int bit)
        {
            num |= 1 << bit;
        }
        public static void setBit(ref long num, int bit)
        {
            num |= (long)1 << bit;
        }

        public static void unsetBit(ref short num, int bit)
        {
            num &= (short)~(((short)1 << bit));
        }
        public static void unsetBit(ref int num, int bit)
        {
            num &= ~(1 << bit);
        }
        public static void unsetBit(ref long num, int bit)
        {
            num &= ~((long)1 << bit);
        }

        /// <summary>
        /// Высчитывает расстояние между двумя географическими точками точками
        /// </summary>
        /// <param name="a1">Широта первой точки</param>
        /// <param name="b1">Долгота первой точки</param>
        /// <param name="a2">Широта второй точки</param>
        /// <param name="b2">Долгота второй точки</param>
        /// <returns>Расстояние в метрах</returns>
        public static int distance_map(double a1, double b1, double a2, double b2)
        {

            // перевести координаты в радианы
            var lat1 = a1 * Math.PI / 180;
            var lat2 = a2 * Math.PI / 180;
            var long1 = b1 * Math.PI / 180;
            var long2 = b2 * Math.PI / 180;

            // косинусы и синусы широт и разницы долгот
            var cl1 = Math.Cos(lat1);
            var cl2 = Math.Cos(lat2);
            var sl1 = Math.Sin(lat1);
            var sl2 = Math.Sin(lat2);
            var delta = long2 - long1;
            var cdelta = Math.Cos(delta);
            var sdelta = Math.Sin(delta);

            // вычисления длины большого круга
            var y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            var x = sl1 * sl2 + cl1 * cl2 * cdelta;

            //
            var ad = Math.Atan2(y, x);
            var dist = ad * 6372795;

            return (int)Math.Round(dist);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Удаляет в предложении слова с указанной строкой, которая содержит подстроку
        /// </summary>
        /// <param name="source">входная строка</param>
        /// <param name="dropContains">удаляемые части содержат</param>
        /// /// <param name="count">число удаляемых частей</param>
        /// <returns>очищенная строка</returns>
        public static string trimWordContains(string source, string dropContains, int count)
        {
            if (source.Contains(dropContains))
            {
                string[] exp_num = explode(" ", source);
                source = "";
                foreach (string ss_ in exp_num)
                {
                    if (!ss_.Contains(dropContains) || count == 0)
                    {
                        if (source == "")
                        {
                            source += ss_;
                        }
                        else
                        {
                            source += " " + ss_;
                        }
                    }
                    else --count;
                }
            }
            return source;
        }

        /// <summary>
        /// Удаляет в предложении слова с указанной строкой
        /// </summary>
        /// <param name="source">входная строка</param>
        /// <param name="dropContains">удаляемые части содержат</param>
        /// /// <param name="count">число удаляемых частей</param>
        /// <returns>очищенная строка</returns>
        public static string trimWordFull(string source, string dropContains, int count)
        {
            if (source.Contains(dropContains))
            {
                string[] exp_num = explode(" ", source);
                source = "";
                foreach (string ss_ in exp_num)
                {
                    if (ss_ != dropContains || count == 0)
                    {
                        if (source == "")
                        {
                            source += ss_;
                        }
                        else
                        {
                            source += " " + ss_;
                        }
                    }
                    else --count;
                }
            }
            return source;
        }

        /// <summary>
        /// Проверяет пгользовательскую строку на наличие SQL внедрений - при обнаружениее возвращает пустую строку
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RealEscape(string str)
        {
            string str_ = str.ToLower();
            if (str_.Contains("delete") || str_.Contains("empty") || str_.Contains("truncate") || str_.Contains("empty") || str_.Contains("drop") || str_.Contains("table")
                 || str_.Contains("insert") || str_.Contains("update"))
                str_ = "";
            return str_;
        }

        /// <summary>
        /// Генерирует случайный пароль
        /// </summary>
        /// <returns></returns>
        public static string GenPass(int len)
        {
            Random rnd = new Random();
            string Password = "";

            for (int i = 0; i < len; ++i)
            {
                if (i % 3 == 0) Password += (char)rnd.Next(97, 122);
                else if (i % 3 == 1) Password += (char)rnd.Next(48, 57);
                else Password += (char)rnd.Next(65, 90);
            }
            return Password;
        }

        /// <summary>
        /// РАСЖАТЬ GZIP
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// ВВОД ТОЛЬКО ЦИФР
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void onlyDigitTextBox(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// ВВОД ТОЛЬКО ЦИФР И МИНУС
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void onlyDigitTextBoxMinus(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back && e.KeyChar != '-')
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// ВВОД ТОЛЬКО ЦИФР И ЗАПЯТОЙ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void onlyDigitAndComma(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back && e.KeyChar != ',')
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// УСТАНОВИТЬ ТЕКСТ ПО УМОЛЧАНИЮ НА ТЕКСТБОКС
        /// </summary>
        /// <param name="t"></param>
        /// <param name="deafultText"></param>
        public static void defaultTextInTextBox(TextBox t, string deafultText)
        {
            //ЧЁРНЫЙ - СЕРЫЙ ЦВЕТ
            t.TextChanged += (s, e) =>
            {
                if (!t.Focused && t.Text.CompareTo("") == 0 && deafultText.CompareTo("") != 0) t.Text = deafultText;

                if (t.Text.CompareTo(deafultText) == 0) t.ForeColor = Color.FromArgb(255, 150, 150, 150);
                else t.ForeColor = Color.FromArgb(255, 0, 0, 0);
            };

            //ПО КЛИКУ УБРАТЬ ДЕФОЛТНЫЙ ТЕКСТ
            t.GotFocus += (s, e) =>
            {
                if (t.Text.CompareTo(deafultText) == 0 || t.Text.CompareTo("") == 0) t.Text = "";
            };

            //ПРИ ПОТЕРЕ ФОКУСА ЕСЛИ ТАМ ПУСТО - УСТАНОВКА ДЕФОЛТНОГО ЗНАЧЕНИЯ
            t.LostFocus += (s, e) =>
            {
                if (t.Text.Trim().CompareTo("") == 0) t.Text = deafultText;
            };

            t.Text = deafultText;
            t.ForeColor = Color.FromArgb(255, 150, 150, 150);
        }

        /// <summary>
        /// УСТАНОВИТЬ ТЕКСТ ПО УМОЛЧАНИЮ НА КОМБОБОКС
        /// </summary>
        /// <param name="t"></param>
        /// <param name="deafultText"></param>
        public static void defaultTextInComboBox(ComboBox t, string deafultText, bool fix = true)
        {
            //ЧЁРНЫЙ - СЕРЫЙ ЦВЕТ
            t.TextChanged += (s, e) =>
            {
                ComboBox tb = s as ComboBox;
                if (tb.Text.CompareTo(deafultText) == 0) tb.ForeColor = Color.FromArgb(255, 150, 150, 150);
                else tb.ForeColor = Color.FromArgb(255, 0, 0, 0);
            };

            //ПРИ ПОТЕРЕ ФОКУСА ЕСЛИ ТАМ ПУСТО - УСТАНОВКА ДЕФОЛТНОГО ЗНАЧЕНИЯ
            t.LostFocus += (s, e) =>
            {
                if (t.Text.Trim().CompareTo("") == 0 || t.SelectedIndex == -1) t.Text = deafultText;
            };

            //ПРИ ИЗМ ИНДЕКСА
            t.SelectedIndexChanged += (s, e) =>
            {
                if (t.SelectedIndex == -1) t.Text = deafultText;
            };

            //ТОЛЬКО ВЫБОР ИЗ СПИСКА
            if (fix)
            {
                t.KeyPress += (s, e) =>
                {
                    e.Handled = true;
                };
            }

            //ПРИ РАСКРЫТИИ ТЕКТ ЧЁРНЫМ
            t.DropDown += (s, e) => { t.ForeColor = Color.FromArgb(255, 0, 0, 0); };

            t.DropDownClosed += (s, e) =>
            {
                if (t.Text.CompareTo(deafultText) == 0) t.ForeColor = Color.FromArgb(255, 150, 150, 150);
            };

            t.Text = deafultText;
            t.ForeColor = Color.FromArgb(255, 150, 150, 150);
        }

        /// <summary>
        /// СДЕЛАТЬ ВРЕМЯ ВИДА HH:mm ИЗ short ПРЕДСТАВЛЕНИЯ HHmm
        /// </summary>
        /// <param name="TIME_"></param>
        /// <returns></returns>
        public static string makeTimeFromShort(short TIME_)
        {
            string tm_ = TIME_.ToString();
            while (tm_.Length < 4) tm_ = "0" + tm_;
            tm_ = tm_.Insert(2, ":");
            return tm_;
        }

        public static string get_only_digits(string text_)
        {
            return Regex.Match(text_, @"\d+").Value;
        }
    }

    /// <summary>
    /// СТРУКТУРА СЕРВЕРА
    /// </summary>
    public class Server
    {
        public string IP { get; set; }
        public int PORT { get; set; }
        public string DB_PATH { get; set; }
        public string USER { get; set; }
        public string PASSWORD { get; set; }

        public Server(string IP_, int PORT_, string DB_PATH_, string USER_, string PASSWORD_)
        {
            IP = IP_;
            PORT = PORT_;
            DB_PATH = DB_PATH_;
            USER = USER_;
            PASSWORD = PASSWORD_;
        }
    }

    public class transliter
    {
        static Dictionary<string, string> rus_to_latin = new Dictionary<string, string>()
        {
            { "а", "a" },
            { "б", "b" },
            { "в", "v" },
            { "г", "g" },
            { "д", "d" },
            { "е", "e" },
            { "ё", "yo" },
            { "ж", "zh" },
            { "з", "z" },
            { "и", "i" },
            { "й", "j" },
            { "к", "k" },
            { "л", "l" },
            { "м", "m" },
            { "н", "n" },
            { "о", "o" },
            { "п", "p" },
            { "р", "r" },
            { "с", "s" },
            { "т", "t" },
            { "у", "u" },
            { "ф", "f" },
            { "х", "h" },
            { "ц", "c" },
            { "ч", "ch" },
            { "ш", "sh" },
            { "щ", "sch" },
            { "ъ", "j" },
            { "ы", "i" },
            { "ь", "j" },
            { "э", "e" },
            { "ю", "yu" },
            { "я", "ya" },
            { "А", "A" },
            { "Б", "B" },
            { "В", "V" },
            { "Г", "G" },
            { "Д", "D" },
            { "Е", "E" },
            { "Ё", "Yo" },
            { "Ж", "Zh" },
            { "З", "Z" },
            { "И", "I" },
            { "Й", "J" },
            { "К", "K" },
            { "Л", "L" },
            { "М", "M" },
            { "Н", "N" },
            { "О", "O" },
            { "П", "P" },
            { "Р", "R" },
            { "С", "S" },
            { "Т", "T" },
            { "У", "U" },
            { "Ф", "F" },
            { "Х", "H" },
            { "Ц", "C" },
            { "Ч", "Ch" },
            { "Ш", "Sh" },
            { "Щ", "Sch" },
            { "Ъ", "J" },
            { "Ы", "I" },
            { "Ь", "J" },
            { "Э", "E" },
            { "Ю", "Yu" },
            { "Я", "Ya" }
        };

        static Dictionary<string, string> rus_to_eng = new Dictionary<string, string>()
        {
            { "а", "f" },
            { "б", "," },
            { "в", "d" },
            { "г", "u" },
            { "д", "l" },
            { "е", "t" },
            { "ё", "`" },
            { "ж", ";" },
            { "з", "p" },
            { "и", "b" },
            { "й", "q" },
            { "к", "r" },
            { "л", "k" },
            { "м", "v" },
            { "н", "y" },
            { "о", "j" },
            { "п", "g" },
            { "р", "h" },
            { "с", "c" },
            { "т", "n" },
            { "у", "e" },
            { "ф", "a" },
            { "х", "[" },
            { "ц", "w" },
            { "ч", "x" },
            { "ш", "i" },
            { "щ", "o" },
            { "ъ", "]" },
            { "ы", "s" },
            { "ь", "m" },
            { "э", "'" },
            { "ю", "." },
            { "я", "z" },
            { "А", "F" },
            { "Б", "<" },
            { "В", "D" },
            { "Г", "U" },
            { "Д", "L" },
            { "Е", "T" },
            { "Ё", "~" },
            { "Ж", ":" },
            { "З", "P" },
            { "И", "B" },
            { "Й", "Q" },
            { "К", "R" },
            { "Л", "K" },
            { "М", "V" },
            { "Н", "Y" },
            { "О", "J" },
            { "П", "G" },
            { "Р", "H" },
            { "С", "C" },
            { "Т", "N" },
            { "У", "E" },
            { "Ф", "A" },
            { "Х", "{" },
            { "Ц", "W" },
            { "Ч", "X" },
            { "Ш", "I" },
            { "Щ", "O" },
            { "Ъ", "}" },
            { "Ы", "S" },
            { "Ь", "M" },
            { "Э", "'" },
            { "Ю", ">" },
            { "Я", "Z" }
        };

        static Dictionary<string, string> eng_to_rus = new Dictionary<string, string>()
        {
            { "f", "а" },
            { ",", "б" },
            { "d", "в" },
            { "u", "г" },
            { "l", "д" },
            { "t", "е" },
            { "`", "ё" },
            { ";", "ж" },
            { "p", "з" },
            { "b", "и" },
            { "q", "й" },
            { "r", "к" },
            { "k", "л" },
            { "v", "м" },
            { "y", "н" },
            { "j", "о" },
            { "g", "п" },
            { "h", "р" },
            { "c", "с" },
            { "n", "т" },
            { "e", "у" },
            { "a", "ф" },
            { "[", "х" },
            { "w", "ц" },
            { "x", "ч" },
            { "i", "ш" },
            { "o", "щ" },
            { "]", "ъ" },
            { "s", "ы" },
            { "m", "ь" },
            //{ "'", "э" },
            { ".", "ю" },
            { "z", "я" },
            { "F", "А" },
            { "<", "Б" },
            { "D", "В" },
            { "U", "Г" },
            { "L", "Д" },
            { "T", "Е" },
            { "~", "Ё" },
            { ":", "Ж" },
            { "P", "З" },
            { "B", "И" },
            { "Q", "Й" },
            { "R", "К" },
            { "K", "Л" },
            { "V", "М" },
            { "Y", "Н" },
            { "J", "О" },
            { "G", "П" },
            { "H", "Р" },
            { "C", "С" },
            { "N", "Т" },
            { "E", "У" },
            { "A", "Ф" },
            { "{", "Х" },
            { "W", "Ц" },
            { "X", "Ч" },
            { "I", "Ш" },
            { "O", "Щ" },
            { "}", "Ъ" },
            { "S", "Ы" },
            { "M", "Ь" },
            { "'", "Э" },
            { ">", "Ю" },
            { "Z", "Я" }
        };

        public static string ru_to_latin(string rus)
        {
            string source = rus;
            foreach (KeyValuePair<string, string> pair in rus_to_latin)
            {
                source = source.Replace(pair.Key, pair.Value);
            }

            return source;
        }

        public static string ru_to_en(string rus)
        {
            string source = rus;
            foreach (KeyValuePair<string, string> pair in rus_to_eng)
            {
                source = source.Replace(pair.Key, pair.Value);
            }

            return source;
        }

        public static string en_to_ru(string en)
        {
            string source = en;
            foreach (KeyValuePair<string, string> pair in eng_to_rus)
            {
                source = source.Replace(pair.Key, pair.Value);
            }

            return source;
        }
    }

    public class Prompt
    {
        /// <summary>
        /// Диалоговое окно ввода текста
        /// </summary>
        /// <param name="text">Текст перед полем ввода</param>
        /// <param name="caption">Название окна</param>
        /// <returns>Пользовательский ввод</returns>
        public static string ShowDialogPhone(Control parent, string text, string caption, string msg = "")
        {
            Form prompt = new Form();
            prompt.StartPosition = FormStartPosition.CenterParent;

            prompt.BackColor = parent.BackColor;
            prompt.Font = parent.Font;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            prompt.ForeColor = parent.ForeColor;

            prompt.Width = 128;
            prompt.Height = 110;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 10, Top = 3, Text = text, AutoSize = false, Width = 108 };
            MaskedTextBox textBox = new MaskedTextBox() { Left = 10, Top = 25, Width = 100, BorderStyle = BorderStyle.Fixed3D, Mask = "7(999) 000-00-00" };
            textBox.Click += (s, e) =>
            {
                if (s is MaskedTextBox)
                {
                    string phone = (s as MaskedTextBox).Text.Replace("_", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (phone.CompareTo("7") == 0)
                    {
                        (s as MaskedTextBox).SelectionStart = 0;
                        (s as MaskedTextBox).SelectionLength = 0;
                        (s as MaskedTextBox).ScrollToCaret();
                    }
                }
            };
            Button confirmation = new Button() { Text = "Ok", Left = 11, Width = 100, Top = 58 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);

            if (msg.CompareTo("") != 0) textBox.Text = msg;

            prompt.ShowDialog();
            return textBox.Text;
        }

        /// <summary>
        /// Диалоговое окно ввода текста
        /// </summary>
        /// <param name="text">Текст перед полем ввода</param>
        /// <param name="caption">Название окна</param>
        /// <returns>Пользовательский ввод</returns>
        public static string ShowDialog(Control parent, string text, string caption, string msg = "")
        {
            Form prompt = new Form();
            prompt.StartPosition = FormStartPosition.CenterParent;

            prompt.BackColor = parent.BackColor;
            prompt.Font = parent.Font;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            prompt.ForeColor = parent.ForeColor;

            prompt.Width = 212;
            prompt.Height = 115;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 8, Top = 8, Text = text, AutoSize = false, Width = 200, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right };
            TextBox textBox = new TextBox() { Left = 8, Top = 27, Width = 190, BorderStyle = BorderStyle.Fixed3D, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right };
            Button confirmation = new Button() { Text = "Ok", Left = 129, Width = 70, Top = 55, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(confirmation);
            prompt.AutoSize = true;

            if (msg.CompareTo("") != 0) textBox.Text = msg;

            textBox.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) prompt.Close();
            };

            //prompt.Load += (s, e) => { textBox.Focus(); };

            prompt.ShowDialog();
            return textBox.Text;
        }

        /// <summary>
        /// Рисует диалоговое окно с предварительно обозначенным списком для выбора элемента из списка
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text">Текст перед полем ввода</param>
        /// <param name="caption">Название окна</param>
        /// <returns>Индекс</returns>
        public static int ShowDialogListBox(Control parent, string text, string caption, Dictionary<int, string> List)
        {
            Form prompt = new Form();
            prompt.StartPosition = FormStartPosition.CenterParent;

            prompt.BackColor = parent.BackColor;
            prompt.Font = parent.Font;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            prompt.ForeColor = parent.ForeColor;

            prompt.Width = 350;
            prompt.Height = 220;
            prompt.Text = caption;

            Label textLabel = new Label() { Left = 10, Top = 3, Text = text, AutoSize = false, Width = 300 };
            ListBox listBox = new ListBox() { Left = 10, Top = 27, Width = 322, Height = 140 };
            Button confirmation = new Button() { Text = "Ok", Left = 233, Width = 100, Top = 165 };

            List<int> ind = new List<int>();

            foreach (KeyValuePair<int, string> p in List)
            {
                ind.Add(p.Key);
                listBox.Items.Add(p.Value);
            }

            confirmation.Click += (sender, e) => { if (listBox.SelectedIndex != -1) prompt.Close(); };
            listBox.DoubleClick += (sender, e) => { if (listBox.SelectedIndex != -1) prompt.Close(); };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(listBox);

            prompt.ShowInTaskbar = false;
            prompt.TopMost = true;
            prompt.BringToFront();
            prompt.ShowDialog();

            if (listBox.SelectedIndex != -1) return ind[listBox.SelectedIndex];
            else return -1;
        }




    }

    public class IniFile
    {
        string Path; //Имя файла.

        [DllImport("kernel32")] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        // С помощью конструктора записываем пусть до файла и его имя.
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string Read(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Section, string Key)
        {
            Write(Section, Key, null);
        }

        //Удаляем выбранную секцию
        public void DeleteSection(string Section)
        {
            Write(Section, null, null);
        }

        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Section, string Key)
        {
            string txt = Read(Section, Key);
            return txt.Length > 0;
        }
    }

    public class Json
    {
        // Запрос
        public static bool? Request(string query, out JObject response, bool escaped = false, int timeout = 10000)
        {
            response = new JObject();

            try
            {
                if (!escaped) query = Uri.EscapeUriString(query);
                Console.WriteLine("Request: " + query);
                //Trace.WriteLine("Request: " + query);

                // Send request
                Uri url = new Uri(query);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = timeout;
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string json_str = sr.ReadToEnd();

                // Maybe jsonp
                Regex r = new Regex("^[^(]+\\((.+)\\)$", RegexOptions.IgnoreCase);
                Match m = r.Match(json_str);
                if (m.Success)
                    json_str = m.Groups[1].Captures[0].ToString();

                // Parse response
                response = JObject.Parse(json_str);
                return response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Request error: " + query + "\n" + ex.Message);
                //Trace.WriteLine("Request error: " + query + "\n" + ex.Message);
                return null;
            }
        }
    }
}
