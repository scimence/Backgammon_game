using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 五子棋单机版
{
    class C_Num                     //循环数,从1到Num再到1，循环变化
    {
        public int num, p;

        public C_Num(int n)
        {
            num = n;
            p = n;
        }

        public int next(int n)      //获取下一个数，当前数的位置后移n位
        {
            p += n;
            while (p > num) p -= num;
            return p;
        }

        public int pre(int n)       //获取前一个数，当前数的位置前移n位
        {
            p -= n;
            while (p < 1) p += num;
            return p;
        }

        public int getNext(int n)   //返回循环数的后n位数，当前的数位置不变
        {
            int tmp = p;
            tmp += n;
            while (tmp > num) tmp -= num;
            return tmp;
        }

        public int getPre(int n)    //返回循环数的前n位数，当前的数位置不变
        {
            int tmp = p;
            tmp -= n;
            while (tmp < 1) tmp += num;
            return tmp;
        }
    }
}
