﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jarg
{
    public static class Version {
        public static int release = 0;
        public static int Major = 1;
        public static int Cores = 14;
        public static int Minor = 1;
        public static int Commit = 29;

        public static string GetShort() {
            return string.Format("v{3}.{0}.{1}.{2}",Major,Cores,Minor,release);
        }
        public static string GetLong() {
            return string.Format("v{4}.{0}.{1}.{2}-{3}", Major, Cores, Minor,Commit,release);
        }

        public static string GetMeaning() {
            return "v<release>.<major>.<cores count>.<minor>-<git commit number>";
        }
    }
}
