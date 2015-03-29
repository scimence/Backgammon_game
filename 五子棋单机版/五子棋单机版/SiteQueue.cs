using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 五子棋单机版
{
    class SiteQueue             //记录坐标的队列
    {
        public int len = 50;
        public int p = 0;       //队首
        private int q = 0;      //队尾
        private int[,] site;
        private int[] way;      //方向
        public SiteQueue()
        {
            site = new int[len, 6];
            way = new int[len];
        }

        public SiteQueue(SiteQueue Q)
        {
            if (Q.length() > 0)
            {
                len = Q.length();
                site = new int[len, 6];
                way = new int[len];
                while (!Q.empty())
                {
                    this.In(Q.Out());
                    wIn(Q.wOut());
                    In2(Q.Out2()[0], Q.Out2()[1]);
                    In3(Q.Out3()[0], Q.Out3()[1]);
                }
            }
        }

        public void In(int[] A) //入队(坐标入队)
        {
            site[q, 0] = A[0];
            site[q, 1] = A[1];
            q++;
        }

        public void wIn(int w)  //方向入队，在坐标入队之后操作
        {
            way[q - 1] = w;
        }

        public void In2(int x, int y)//更大棋型点坐标入队
        {
            site[q - 1, 2] = x;
            site[q - 1, 3] = y;
        }

        public void In3(int x, int y)//更大棋型点坐标入队
        {
            site[q - 1, 4] = x;
            site[q - 1, 5] = y;
        }

        public int[] Out()      //出队(第index个队列坐标出队)
        {
            int[] A = { site[p, 0], site[p, 1] };
            p++;
            return A;
        }

        public int wOut()       //方向出队
        {
            return way[p - 1];
        }

        public int[] Out2()//更大棋型点坐标入队
        {
            int[] A = { site[p - 1, 2], site[p - 1, 3] };
            return A;
        }
        public int[] Out3()//更大棋型点坐标入队
        {
            int[] A = { site[p - 1, 4], site[p - 1, 5] };
            return A;
        }
        

        public bool empty()     //空
        {
            if (p == q)return true;
            return false;
        }

        public void clear()     //清空
        {
            p = 0;
            q = 0;
        }

        public int length()        //队列中元素总数
        {
            return q;
        }

        public void again()
        {
            p = 0;
        }
    }
}
