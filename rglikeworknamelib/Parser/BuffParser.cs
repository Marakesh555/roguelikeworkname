﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Parser
{
    class BuffParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<KeyValuePair<string, object>> Parser(string s)
        {
            var temp = new List<KeyValuePair<string, object>>();

            s = s.Remove(0, s.IndexOf('~'));

            s = Regex.Replace(s, "//.*\r\n", "");
            s = Regex.Replace(s, "//.*", "");

            string[] blocks = s.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n");
                    string[] header = lines[0].Split(',');

                    Type type = Type.GetType("rglikeworknamelib.Dungeon.Buffs." + header[0]);
                    temp.Add(new KeyValuePair<string, object>(header[1].Trim('\r'), new BuffData()));
                    if (type == null)
                    {
                        logger.Error(string.Format("Subclass of Buff \"{0}\" for {1} cannot be created", "rglikeworknamelib.Dungeon.Buffs." + header[0], header[1]));
                        type = typeof(Buff);
                    }
                    KeyValuePair<string, object> cur = temp.Last();
                    ((BuffData)cur.Value).EffectPrototype = type;

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Contains('=')) {
                            string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                            var finfo = typeof (BuffData).GetField(sstart);
                            var extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                               lines[i].Length - (lines[i].IndexOf('=') + 1)).Replace("\"","").Replace("\r","");

                            if (finfo != null) {
                                var converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                if (converter != null) {
                                    var converted = converter.ConvertFromString(extracted);
                                    finfo.SetValue(cur.Value, converted);
                                }
                            } else {
                                logger.Error(cur.Value.GetType().Name+" didnt contains field \""+sstart+"\", \""+extracted+"\" didn't assigned to ID "+header[0]);
                            }
                        }
                    }
                }
            }
            return temp;
        }
    }
}
