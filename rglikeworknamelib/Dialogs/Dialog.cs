﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dialogs
{
    public class DialogData {
        
    }

    public class DialogDataBase {
        public static Dictionary<string, DialogData> data;

        public DialogDataBase() {
            data = new Dictionary<string, DialogData>();

            var temp = ParsersCore.ParseDirectory(Settings.GetDialogDataDirectory(), DialogParser.Parser);

            foreach (var pair in temp) {
                data.Add(pair.Key, (DialogData)pair.Value);
            }
        }
    }
}
