﻿namespace Drydock.Utilities{
    internal class Pair<TA, TB>{
        public TA Val1;
        public TB Val2;

        public Pair(TA val1, TB val2){
            Val1 = val1;
            Val2 = val2;
        }

        public Pair(){
            Val1 = default(TA);
            Val2 = default(TB);
        }
    }
}