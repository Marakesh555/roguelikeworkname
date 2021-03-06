﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace rglikeworknamelib.Parser {
    public static class ParsersCore {
        public static Regex stringExtractor = new Regex("\".*\"");
        public static Regex intextractor = new Regex("[0-9]+");
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Color ParseStringToColor(string s) {
            string extractedstring = stringExtractor.Match(s).ToString();
            extractedstring = extractedstring.Substring(1, extractedstring.Length - 2);
            extractedstring = extractedstring.ToLower();

            switch (extractedstring) {
                case "white":
                    return Color.White;
                case "lightgray":
                    return Color.LightGray;
                case "darkgray":
                    return Color.DarkGray;
                case "black":
                    return Color.Black;
                case "brown":
                    return Color.Brown;
                case "yellow":
                    return Color.Yellow;
                case "blue":
                    return Color.Blue;
                case "darkblue":
                    return Color.DarkBlue;
                case "red":
                    return Color.Red;
                case "darkred":
                    return Color.DarkRed;
                case "green":
                    return Color.Green;
                case "darkgreen":
                    return Color.DarkGreen;
                default:
                    return Color.Transparent;
            }
        }

        public static Collection<Texture2D> LoadTexturesInOrder(string s, ContentManager content) {
            Collection<Texture2D> temp = new Collection<Texture2D>();

            StreamReader sr = new StreamReader(s, Encoding.Default);
            while (true) {
                string t = sr.ReadLine();
                if (t == null || t.Length < 3) break;
                temp.Add(content.Load<Texture2D>(t));
            }

            return temp;
        }

        public static Dictionary<string, Texture2D> LoadTexturesTagged(string s, ContentManager content) {
            Dictionary<string, Texture2D> temp = new Dictionary<string, Texture2D>();

            StreamReader sr = new StreamReader(s, Encoding.Default);
            while (true) {
                string t = sr.ReadLine();
                if (t == null || t.Length < 3) break;
                temp.Add(t.Substring(t.IndexOf(' ') + 1, t.Length - (t.IndexOf(' ') + 1)),
                         content.Load<Texture2D>(t.Substring(0, t.IndexOf(' '))));
            }

            return temp;
        }

        public static List<T> ParseDirectory<T>(string patch, Func<string, List<T>> parser) {
            try {
                string[] a = Directory.GetFiles(patch, "*.txt");
                var temp = new List<T>();
                foreach (string s in a) {
                    temp.AddRange(ParseFile(s, parser));
                }
                return temp;
            }
            catch (DirectoryNotFoundException e) {
                logger.ErrorException(e.StackTrace + " --- " + e.Message, e);
                throw;
            }
        }

        public static List<T> ParseFile<T>(string patch, Func<string, List<T>> parser) {
            try {
                var sr = new StreamReader(patch, Encoding.Default);
                string a = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                return parser(a);
            }
            catch (FileNotFoundException) {
                return new List<T>();
            }
        }

        public static List<T> UniversalParseDirectory<T>(string patch, Func<string, string, Type, List<T>> parser,
                                                         Type baseType = null) {
            try {
                string[] a = Directory.GetFiles(patch, "*.txt");
                var temp = new List<T>();
                foreach (string s in a) {
                    temp.AddRange(UnivarsalParseFile(s, parser, baseType));
                }
                return temp;
            }
            catch (DirectoryNotFoundException e) {
                logger.ErrorException(e.StackTrace + " --- " + e.Message, e);
                throw;
            }
        }

        public static List<T> UnivarsalParseFile<T>(string patch, Func<string, string, Type, List<T>> parser,
                                                    Type baseType = null) {
            try {
                var sr = new StreamReader(patch, Encoding.Default);
                string a = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                return parser(a, patch, baseType);
            }
            catch (FileNotFoundException) {
                return new List<T>();
            }
        }

        public static T CreateFromString<T>(string stringToCreateFrom) {
            T output = Activator.CreateInstance<T>();

            output = (T) Enum.Parse(typeof (T), stringToCreateFrom, true);

            return output;
        }
    }
}