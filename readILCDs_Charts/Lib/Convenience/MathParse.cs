/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;


namespace Greet.ConvenienceLib
{
    /// <summary>
    /// String parser that can evaluate forumlas entered as strings
    /// </summary>
    [Serializable]
    public static class MathParse
    {
        public static CultureInfo Nfi = new CultureInfo("en-US");

        #region parser

        /// <summary>
        /// Evaluates given string and returns the result as a double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static KeyValuePair<string, double> Parse(string str)
        {
            try
            {
                double res;

                if (HasOps(ref str))
                {
                    Handle_Paren(ref str);
                    Handle_Functions(ref str);
                    Handle_Scientific(ref str);
                    Handle_Pow(ref str);
                    Handle_Div_Mult(ref str);
                    Handle_Add_Sub(ref str);
                    Handle_Booleans(ref str);

                    if (str.Contains("$"))
                    {
                        string group = TakeOutGroup(ref str);
                        Double.TryParse(str, System.Globalization.NumberStyles.Number, Nfi, out res);
                        return new KeyValuePair<string, double>(group, res);
                    }
                }

                if (Double.TryParse(str, System.Globalization.NumberStyles.Number, Nfi, out res))
                    return new KeyValuePair<string, double>("", res);
                else
                    return new KeyValuePair<string, double>("", Double.NaN);
            }
            catch
            {
                return new KeyValuePair<string, double>("", Double.NaN);
            }
        }

        public static bool HasOps(ref string str)
        {
            double temp;
            str = str.Replace(" ", "");   //Remove whitespace
            str = str.TrimStart('=');
            return !Double.TryParse(str, System.Globalization.NumberStyles.Number, Nfi, out temp);
        }

        public static void Handle_Paren(ref string str)
        {
            int firstParen = -1;    //Index of first parenthesis
            int lastParen = -1;
            string replaceStr, newStr;
            while (GetParenBlock(str, ref firstParen, ref lastParen))
            {

                replaceStr = str.Substring(firstParen, lastParen - firstParen + 1);
                newStr = "";
                List<String> split = new List<string>();
                int split_number = 0;
                split.Add("");
                int sub_brackets_count = 0;
                string search_string = replaceStr;
                for (int i = 1; i < search_string.Length - 1; i++)
                {
                    if (search_string[i] == '(')
                    {
                        sub_brackets_count++;
                        split[split_number] += search_string[i];
                    }
                    else if (search_string[i] == ')')
                    {
                        sub_brackets_count--;
                        split[split_number] += search_string[i];
                    }
                    else if (search_string[i] == ',' && sub_brackets_count == 0)
                    {
                        split_number++;
                        split.Add("");
                    }
                    else
                    {
                        split[split_number] += search_string[i];
                    }
                }

                for (int i = 0; i < split.Count; i++)
                {
                    KeyValuePair<string, double> result = Parse(split[i].ToString());
                    newStr += result.Value.ToStringFull();
                    if (String.IsNullOrEmpty(result.Key) == false)
                        newStr += "$" + result.Key + "$";
                    newStr += ",";
                }
                newStr = newStr.TrimEnd(',');

                //try to detect funtion
                int function_name_lenght = firstParen - 1;
                string function_name = "";
                while (function_name_lenght >= 0 && str[function_name_lenght] != '*' && str[function_name_lenght] != '/' && str[function_name_lenght] != '+' && (str[function_name_lenght] != '-'))
                { //if matches, mean that we have a function name in front of the first bracket, or a missing operator between a number and a bracket
                    function_name = str[function_name_lenght] + function_name;
                    function_name_lenght--;
                }


                if (function_name != "")
                {
                    int loc = str.IndexOf(replaceStr);
                    str = str.Remove(loc, replaceStr.Length).Insert(loc, "{" + newStr + "}");
                }
                else
                {
                    int loc = str.IndexOf(replaceStr);
                    str = str.Remove(loc, replaceStr.Length).Insert(loc, newStr);
                }
                str = str.Replace("--", "+");    //Handle double negative after paren recurse
            }
        }

        public static void Handle_Scientific(ref string str)
        {
            //Handle scientific notation
            int start = 0;          //Set substring start to beginning of str
            int end = str.Length - 1;   //Set substring end to end of str
            double test;
            for (int i = 1; i < str.Length; i++)
            {
                if ((str[i] == 'E' || str[i] == 'e'))
                {   //Get the numbers before and after the E for the operation
                    if (((i - 1) >= 0) && Double.TryParse(str[i - 1].ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, Nfi, out test))
                    {
                        GetNumsForOperation(str, ref start, ref end, i, 1);
                        DoOperation(ref str, ref start, ref end, ref i, 1, "ShiftDeci");
                    }
                }
            }
        }

        private static Regex rhandle_functions = new Regex("[a-zA-Z0-9]*{");
        public static void Handle_Functions(ref string str)
        {
            string function_name;
            bool unkown_funtion = false;
            Match match;
            while ((match = rhandle_functions.Match(str)).Success && unkown_funtion == false)
            {
                function_name = match.Value.TrimEnd('{').ToLower();
                for (int i = 0; i < str.Length - function_name.Length; i++)
                {
                    int bracket_count = 0;
                    int end_function_bracket = -1;
                    if (str.Substring(i, function_name.Length).ToLower() == function_name)
                    {
                        for (int j = i + function_name.Length; j < str.Length; j++)
                        {
                            if (str[j] == '{')
                                bracket_count++;
                            if (str[j] == '}')
                                bracket_count--;
                            if (bracket_count == 0)
                            {
                                end_function_bracket = j;
                                break;
                            }
                        }
                        string parameters = str.Substring(i + function_name.Length + 1, end_function_bracket - i - function_name.Length - 1);
                        string[] parameters_array = parameters.Split(',');
                        string to_be_evaluated = str.Substring(i, end_function_bracket - i + 1);

                        switch (function_name)
                        {
                            case "ln":
                                str = str.Replace(to_be_evaluated, Math.Log(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "e":
                                str = str.Replace(to_be_evaluated, Math.Exp(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "sin":
                                str = str.Replace(to_be_evaluated, Math.Sin(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "cos":
                                str = str.Replace(to_be_evaluated, Math.Cos(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "tan":
                                str = str.Replace(to_be_evaluated, Math.Tan(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "asin":
                                str = str.Replace(to_be_evaluated, Math.Asin(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "acos":
                                str = str.Replace(to_be_evaluated, Math.Acos(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "atan":
                                str = str.Replace(to_be_evaluated, Math.Atan(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "log":
                                str = str.Replace(to_be_evaluated, Math.Log10(Convert.ToDouble(RemoveGroup(parameters_array[0]), Nfi)).ToStringFull());
                                break;
                            case "zfactor":
                                break;
                            case "if":
                                if(parameters_array[0] == "0") //check if the condition is 0 meaning false
                                    str = str.Replace(to_be_evaluated, RemoveGroup(parameters_array[2])); //replace by the argument for false
                                else
                                    str = str.Replace(to_be_evaluated, RemoveGroup(parameters_array[1])); //replace by the argument for trueIssues
                                break;
                            default:
                                unkown_funtion = true;
                                break;
                        }
                        break;
                    }
                }
            }
        }

        public static void Handle_Pow(ref string str)
        {
            //Handle power operations
            int start = 0;          //Set substring start to beginning of str
            int end = str.Length - 1;   //Set substring end to end of str
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '^')
                {   //Get the numbers before and after the ^ for the operation
                    GetNumsForOperation(str, ref start, ref end, i, 1);
                    DoOperation(ref str, ref start, ref end, ref i, 1, "Pow");
                }
            }
        }

        public static void Handle_Div_Mult(ref string str)
        {
            //Handle division and multiplication
            int start = 0;          //Set substring start to beginning of str
            int end = str.Length - 1;   //Set substring end to end of str
            bool pass_unit_definition = false;
            for (int operatorLocation = 0; operatorLocation < str.Length; operatorLocation++)
            {
                if (str[operatorLocation] == '$')
                {
                    pass_unit_definition = !pass_unit_definition;
                    continue;
                }
                if ((str[operatorLocation] == '/' || str[operatorLocation] == '*') && pass_unit_definition == false)
                {
                    GetNumsForOperation(str, ref start, ref end, operatorLocation, 1);

                    if (str[operatorLocation] == '/')
                    {
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Div");
                    }
                    else
                    {
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Mult");
                    }
                }
            }
        }

        public static void Handle_Add_Sub(ref string str)
        {
            //Handle addition and subtraction
            int start = 0;          //Set substring start to beginning of str
            int end = str.Length - 1;   //Set substring end to end of str
            bool pass_unit_definition = false;
            for (int operatorLocation = 1; operatorLocation < str.Length; operatorLocation++)
            {
                if (str[operatorLocation] == '$')
                {
                    pass_unit_definition = !pass_unit_definition;
                    continue;
                }
                if ((str[operatorLocation] == '+' || str[operatorLocation] == '-') && pass_unit_definition == false)
                {
                    GetNumsForOperation(str, ref start, ref end, operatorLocation, 1);

                    if (str[operatorLocation] == '+')
                    {
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Add");
                    }
                    else
                    {
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Sub");
                    }

                }
            }
        }

        public static void Handle_Booleans(ref string str)
        {
            int start = 0;          //Set substring start to beginning of str
            int end = str.Length - 1;   //Set substring end to end of str
            for (int operatorLocation = 1; operatorLocation < str.Length - 1; operatorLocation++)
            {
                switch (str.Substring(operatorLocation, 2))
                {
                    case "==":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "Equal");
                        break;
                    case "!=":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "NotEqual");
                        break;
                    case "&&":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "And");
                        break;
                    case "||":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "Or");
                        break;
                    case "<=":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "InfEqual");
                        break;
                    case ">=":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 2);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 2, "SupEqual");
                        break;
                }
                switch (str.Substring(operatorLocation, 1))
                {
                    case "<":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 1);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Inf");
                        break;
                    case ">":
                        GetNumsForOperation(str, ref start, ref end, operatorLocation, 1);
                        DoOperation(ref str, ref start, ref end, ref operatorLocation, 1, "Sup");
                        break;
                }
            }
        }

        private static bool GetParenBlock(string str, ref int firstParen, ref int lastParen)
        {
            int parenCnt = 0;       //Parentheses counter
            bool hasParen = false;
            firstParen = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    parenCnt++;
                    if (firstParen == -1)
                    {
                        firstParen = i;
                        hasParen = true;
                    }
                }
                else if (str[i] == ')')
                {
                    lastParen = i;      //lastParen will be index of last close paren when cnt = 0
                    parenCnt--;
                    if (parenCnt == 0)
                        break;
                }
            }
            return hasParen;
        }

        private static void GetNumsForOperation(string str, ref int start, ref int end, int operatorLocation, int operatorLength)
        {
            bool pass_unit_definition = false;
            for (int s = operatorLocation - 1; s > 0 && start == 0; s--)
            {
                if (str[s] == '$')
                {
                    pass_unit_definition = !pass_unit_definition;
                    continue;
                }
                if (isOp(str, s) && pass_unit_definition == false)
                    start = s + 1;
            }

            for (int s = operatorLocation + operatorLength; s < str.Length && end == str.Length - 1; s++)
            {
                if (str[s] == '$')
                {
                    pass_unit_definition = !pass_unit_definition;
                    continue;
                }
                if (isOp(str, s) && pass_unit_definition == false)
                    end = s - 1;
            }
        }

       

        private static void DoOperation(ref string str, ref int start, ref int end, ref int operatorLocation, int operator_length, string operation)
        {
            string retStr = "";
            string num1 = str.Substring(start, operatorLocation - start);
            string num2 = str.Substring(operatorLocation + operator_length, end - (operatorLocation + operator_length - 1));

            switch (operation)   //Do the operation on num1, num2
            {
                case "ShiftDeci":
                    retStr = ShiftDeci(num1, num2); break;
                case "Pow":
                    retStr = Pow(num1, num2); break;
                case "Div":
                    retStr = Div(num1, num2); break;
                case "Mult":
                    retStr = Mult(num1, num2); break;
                case "Add":
                    retStr = Add(num1, num2); break;
                case "Sub":
                    retStr = Sub(num1, num2); break;
                case "Equal":
                    retStr = BoolEqual(num1, num2); break;
                case "NotEqual":
                    retStr = BoolNotEqual(num1, num2); break;
                case "And":
                    retStr = BoolAnd(num1, num2); break;
                case "Or":
                    retStr = BoolOr(num1, num2); break;
                case "InfEqual":
                    retStr = BoolInfEqual(num1, num2); break;
                case "SupEqual":
                    retStr = BoolSupEqual(num1, num2); break;
                case "Inf":
                    retStr = BoolInf(num1, num2); break;
                case "Sup":
                    retStr = BoolSup(num1, num2); break;
            }

            //Remove the old operation and replace it with the result
            str = str.Remove(start, end - start + 1);
            str = str.Insert(start, retStr);

            //Reset indices
            operatorLocation = 0;
            start = 0;
            end = str.Length - 1;
        }

        private static bool isOp(string str, int s)
        {   //Returns true if the character is an operator 
            //Also checks for + or - as postive or negative indicators
            return str[s] == '*' || str[s] == '/' || str[s] == '^' || str[s] == '>' || str[s] == '<' || str[s] == '=' ||
                (str[s] == '+' && s > 0 && !isOp(str, s - 1) && str[s - 1] != 'e' && str[s - 1] != 'E') ||
                (str[s] == '-' && s > 0 && !isOp(str, s - 1) && str[s - 1] != 'e' && str[s - 1] != 'E');
        }

        private static string ShiftDeci(string num, string exp)
        {

            string str;
            TakeOutGroup(ref exp);
            if (!num.Contains('.'))
            {
                num = num.Insert(num.Length, ".");
            }

            int deciIndex = num.IndexOf('.');

            int numsBefore = deciIndex;
            int numsAfter = num.Length - numsBefore - 1;

            bool shiftLeft = (exp[0] == '-');                //Direction to shift
            int expInt = Math.Abs(Convert.ToInt32(exp));     //Exp w/o sign
            char[] chArr;

            if (shiftLeft)
            {
                int length = numsAfter + 1 + Math.Max(numsBefore, expInt);            //Length of new string
                chArr = new char[length];
                for (int i = 0; i < length; i++)                 //Initialize to 0
                {
                    chArr[i] = '0';
                }

                int arrInd = length - 1;
                bool dot_inserted = false;
                for (int numInd = num.Length - 1; numInd >= 0; numInd--)
                {
                    if (num[numInd] != '.' && arrInd != deciIndex - expInt)
                    {
                        chArr[arrInd] = num[numInd];
                        arrInd--;
                        if (dot_inserted == false && numInd == 0)
                        {
                            chArr[0] = '.';
                        }
                    }
                    else if (arrInd == deciIndex - expInt)
                    {
                        chArr[arrInd] = '.';
                        arrInd--;
                        numInd++;
                        dot_inserted = true;
                    }
                }
            }
            else
            {
                int length = numsBefore + 1 + Math.Max(numsAfter, expInt);   //Length of new string
                chArr = new char[length];
                for (int i = 0; i < length; i++)                 //Initialize to 0
                {
                    chArr[i] = '0';
                }

                int arrInd = 0;
                bool dot_inserted = false;
                for (int numInd = 0; numInd < num.Length; numInd++)
                {   //Copy digits of num into chArr shifted left
                    if (num[numInd] != '.' && arrInd != deciIndex + expInt)
                    {
                        chArr[arrInd] = num[numInd];
                        arrInd++;
                        if (dot_inserted == false && numInd == num.Length - 1)
                        {
                            chArr[length - 1] = '.';
                        }
                    }
                    else if (arrInd == deciIndex + expInt)
                    {
                        chArr[arrInd] = '.';
                        arrInd++;
                        numInd--;
                        dot_inserted = true;
                    }
                }
            }
            str = new string(chArr);
            return str;
        }

        private static string Pow(string x, string y)
        {
            string group_x = TakeOutGroup(ref x);
            string group_y = TakeOutGroup(ref y);

            //no units are propagated on the Pow operation
            return (Math.Pow(Convert.ToDouble(x, Nfi), Convert.ToDouble(y, Nfi))).ToStringFull();

        }
        private static string Div(string x, string y)
        {
            string group_x = TakeOutGroup(ref x);
            string group_y = TakeOutGroup(ref y);

            string combined_unit_group = group_x + "/" + group_y.TrimEnd('/').TrimStart('/');

            return (Convert.ToDouble(x, Nfi) / Convert.ToDouble(y, Nfi)).ToStringFull() + "$" + combined_unit_group + "$";
        }
        private static string Mult(string x, string y)
        {
            string group_x = TakeOutGroup(ref x);
            string group_y = TakeOutGroup(ref y);

            string combined_unit_group = (group_x + "*" + group_y).TrimEnd('*').TrimStart('*');

            return (Convert.ToDouble(x, Nfi) * Convert.ToDouble(y, Nfi)).ToStringFull() + "$" + combined_unit_group + "$";
        }

        private static string Add(string x, string y)
        {
            string group_x = TakeOutGroup(ref x);
            string group_y = TakeOutGroup(ref y);

            string combined_unit_group = "";
            if (group_x == group_y)
                combined_unit_group = group_x;

            return (Convert.ToDouble(x, Nfi) + Convert.ToDouble(y, Nfi)).ToStringFull() + "$" + combined_unit_group + "$";
        }
        private static string Sub(string x, string y)
        {
            string group_x = TakeOutGroup(ref x);
            string group_y = TakeOutGroup(ref y);

            string combined_unit_group = "";
            if (group_x == group_y)
                combined_unit_group = group_x;
            else if (group_x == "")
                combined_unit_group = group_y;
            else if (group_y == "")
                combined_unit_group = group_x;

            return (Convert.ToDouble(x, Nfi) - Convert.ToDouble(y, Nfi)).ToStringFull() + "$" + combined_unit_group + "$";
        }

        private static string BoolEqual(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (a == b)
                return "1";
            else
                return "0";
        }
        private static string BoolNotEqual(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (a != b)
                return "1";
            else
                return "0";
        }
        private static string BoolAnd(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if ((a == "1" || a.ToLower() == "true") && (b == "1" || b.ToLower() == "true"))
                return "1";
            else
                return "0";
        }
        private static string BoolOr(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if ((a == "1" || a.ToLower() == "true") || (b == "1" || b.ToLower() == "true"))
                return "1";
            else
                return "0";
        }
        private static string BoolInfEqual(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (Convert.ToDouble(a, Nfi) <= Convert.ToDouble(b, Nfi))
                return "1";
            else
                return "0";
        }
        private static string BoolSupEqual(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (Convert.ToDouble(a, Nfi) >= Convert.ToDouble(b, Nfi))
                return "1";
            else
                return "0";
        }
        private static string BoolInf(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (Convert.ToDouble(a, Nfi) < Convert.ToDouble(b, Nfi))
                return "1";
            else
                return "0";
        }
        private static string BoolSup(string a, string b)
        {
            string group_x = TakeOutGroup(ref a);
            string group_y = TakeOutGroup(ref b);

            if (Convert.ToDouble(a, Nfi) > Convert.ToDouble(b, Nfi))
                return "1";
            else
                return "0";
        }

        public static string TakeOutGroup(ref string str)
        {
            Match match = Regex.Match(str, @"([\+\-0-9/.]*)(\$[a-zA-Z0-9/\*_ ]*\$)?");
            if (String.IsNullOrEmpty(match.Groups[1].Value))
            {
                str = "NaN";
                return "";
            }
            else
            {
                str = match.Groups[1].Value;
                return match.Groups[2].Value.Replace("$","");
            }
            
        }

        private static string RemoveGroup(string str)
        {

            Match match = Regex.Match(str, @"([\+\-0-9/.]*)([/$]?)([a-zA-Z0-9/_ ]*)([/$]?)(.*)");
            return String.IsNullOrEmpty(match.Groups[1].Value) == false ? match.Groups[1].Value : "";
        }

        #endregion parser

        #region inverter
        private static string retStr;
        private static int valInd;
        private static bool init = false;
        /// <summary>
        /// Removes parentheses and switches multiplications and divisions for the unit expressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string InvertExpression(string expression)
        {
            string inverted = expression.Replace("(", "").Replace(")", "");
            inverted = inverted.Replace("*", "~").Replace("/", "*").Replace("~", "/");
            return inverted;
        }
        /// <summary>
        /// Inverts the operations done in the given expression string. Used to get the fromDefault expression for units from a toDefaultStr.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Invert(string str)
        {
            valInd = str.IndexOf("val");
            Initialize(ref str);
            HandleAddSub(ref str);
            HandleDivMult(ref str);
            HandlePow(ref str);
            HandleParen(ref str);

            return retStr;
        }
        private static void Initialize(ref string str)
        {
            if (!init)
            {
                retStr = "val";
                str = str.Replace(" ", "");
                init = true;
            }
        }
        private static void HandleAddSub(ref string str)
        {
            bool goLeft = true;
            for (int i = valInd; i < str.Length; )      //Look left then right of val
            {
                if (str[i] == '+' && !isInParen(str, i))    //Ignore if it is in parentheses
                {
                    AddSub(ref str, '-', ref i);
                }
                else if (str[i] == '-' && !isInParen(str, i))   //Ignore if it is in parentheses
                {
                    AddSub(ref str, '+', ref i);
                }
                if (i == 0)
                {
                    goLeft = false;
                    i = valInd + 2;
                }
                if (goLeft) i--; else i++;
            }
        }
        private static bool isInParen(string str, int start)
        {
            int parenCnt = 0;
            for (int i = start; i >= 0; i--)
            {
                if (str[i] == ')')
                    parenCnt++;
                else if (str[i] == '(')
                    parenCnt--;
                if (parenCnt == -1)      //There is an open paren left of the number
                    return true;
            }
            return false;

        }
        private static void AddSub(ref string str, char op, ref int i)
        {
            int j;
            if (i > valInd)
            {
                for (j = i + 1; j < str.Length; j++)
                {
                    if (isOper(str, j, true))
                        break;
                }
                if (j != str.Length)
                    retStr = "(" + retStr + op + "(" + str.Substring(i + 1) + "))";
                else
                    retStr = "(" + retStr + op + str.Substring(i + 1) + ")";
                str = str.Remove(i);
            }
            else
            {
                for (j = i - 1; j >= 0; j--)
                {
                    if (isOper(str, j, true))
                        break;
                }
                if (j != -1)
                    retStr = "(" + retStr + op + "(" + str.Substring(0, i) + "))";
                else
                    retStr = "(" + retStr + op + str.Substring(0, i) + ")";
                str = str.Remove(0, i + 1);
            }
            valInd = str.IndexOf("val");
            i = valInd;
        }
        private static void HandleDivMult(ref string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '*' && !isInParen(str, i))      //Handle multiplication
                {
                    MultDiv(ref str, ref i, '/');
                }
                else if (str[i] == '/' && !isInParen(str, i))         //Handle Division
                {
                    MultDiv(ref str, ref i, '*');
                }
            }
        }
        private static void MultDiv(ref string str, ref int i, char op)
        {   //GoLeft - Division always looks right for the number, multi depends on if it is before or after val
            bool goLeft = false;

            if (i > valInd)     //Going right
            {
                retStr = retStr + op + GetNumAfterOp(ref str, i, goLeft);
            }
            else        //Going left
            {
                if (op == '*' && (str[i + 1] == 'v' || (str[i + 2] == 'v')))    //Division by val
                {
                    retStr = "(" + GetNumAfterOp(ref str, i, true) + "/" + retStr + ")";
                }
                else
                {
                    if (op == '/') { goLeft = true; }       //Multiplication before val, go left for num
                    retStr = retStr + op + GetNumAfterOp(ref str, i, goLeft);
                }
            }
            i = 0;
            valInd = str.IndexOf("val");
        }
        private static string GetNumAfterOp(ref string str, int start, bool goLeft)
        {
            string ret = "";
            start = goLeft ? start - 1 : start + 1;         //Set start 1 ahead or behind the operator
            int j;

            if (str[start] == '(' || str[start] == ')')     //Get quantity
            {
                char open = str[start];
                char close = open == '(' ? ')' : '(';
                int parenCnt = 0;
                for (j = start; ; )
                {
                    if (str[j] == open)
                        parenCnt++;
                    else if (str[j] == close)
                        parenCnt--;
                    if (parenCnt == 0)
                    {
                        if (goLeft)
                        {
                            ret = str.Substring(j, start + 1);
                            str = str.Remove(j, start + 2);
                        }
                        else
                        {
                            ret = str.Substring(start, j - start + 1);
                            str = str.Remove(start - 1, j - start + 1);
                        }
                    }
                    if (goLeft)
                    {
                        j--;
                        if (j < 0) break;
                    }
                    else
                    {
                        j++;
                        if (j > str.Length - 1) break;
                    }
                }
                return ret;
            }
            else            //Not a quantity
            {
                for (j = start; ; )         //Move in direction until op then move back one
                {
                    if (isOper(str, j, false)) { break; }
                    if (goLeft)
                    {
                        j--;
                        if (j < 0) break;
                    }
                    else
                    {
                        j++;
                        if (j > str.Length - 1) break;
                    }
                }
                if (goLeft)
                {
                    j++;
                    ret = str.Substring(j, start - j + 1);
                    str = str.Remove(j, start - j + 2);
                    return ret;
                }
                else
                {
                    j--;
                    ret = str.Substring(start, j - start + 1);
                    str = str.Remove(start - 1, j - start + 2);
                    return ret;
                }
            }
        }
        private static void HandlePow(ref string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '^' && !isInParen(str, i))      //Handle power
                {
                    retStr = "(" + retStr + ")^(1/" + GetNumAfterOp(ref str, i, false) + "))";
                    i = 0;
                    valInd = str.IndexOf("val");
                }
            }
        }
        private static void HandleParen(ref string str)
        {   //Every layer of recursion handles a set of parentheses until there are none
            int parenCnt = 0;
            int firstParen = -1;
            int lastParen;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    if (firstParen == -1)
                        firstParen = i;
                    parenCnt++;
                }
                else if (str[i] == ')')
                {
                    lastParen = i;
                    parenCnt--;

                    if (parenCnt == 0)
                    {
                        Invert(str.Substring(firstParen + 1, lastParen - firstParen - 1));        //Recurse with outer parentheses removed
                    }
                }
            }
            init = false;
        }
        private static bool isOper(string str, int i, bool incPow)
        {
            bool isOp = str[i] == '*' || str[i] == '/' || str[i] == '+' || (str[i] == '-' && (i == 0 || str[i - 1] != '('));
            if (incPow) isOp = isOp || str[i] == '^';
            return isOp;
        }

        #endregion inverter
    }
}