using System;

namespace Course.Api {
    public static class Pathway
    {
        public static String Resolve(String pathway) {
            return AppDomain.CurrentDomain.BaseDirectory + pathway;
        }
    }
}