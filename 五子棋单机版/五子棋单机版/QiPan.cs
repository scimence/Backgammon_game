using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace 五子棋单机版
{

    class QiPan
    {
        private int Row, Line;                             //棋盘行列数
        private int players;                               //记录玩家的数目
        private int[] count;                               //记录各玩家的棋子数

        public int num;                                    //记录棋盘中的棋子数目
        public int[] X, Y;                                 //记录棋子坐标，按落子次序依次存储
        public int[,] player;                              //记录棋盘的所有棋子，为哪个玩家所下

        private int[, ,] style;                            //存储棋型

        //按指定的行列数和玩家数构建棋盘，初始构造
        public QiPan(int row, int line, int players)
        {
            Row = row;
            Line = line;
            count= new int[players];

            num = 0;
            this.players = players;
            X = new int[row * line];
            Y = new int[row * line];
            player = new int[row, line];

            style = new int[players, row, line];   //1到players存储各个空位置处周边各色棋子的最大棋型棋子数目
        }

        //清空棋盘中存储的数据
        public void clear()
        {
            for (int i = 0; i < players; i++) count[i] = 0;
            num = 0;                           //计数值清零
            for (int i = 0; i < X.Length; i++) //坐标清零
            {
                X[i] = 0;
                Y[i] = 0;
            }
            for (int i = 0; i < Row; i++)      //清空 棋子和棋型
                for (int j = 0; j < Line; j++)
                {
                    player[i, j] = 0;
                    clearStyle(i, j);
                }
        }
        /*测试通过
         * 测试位置：Form1.again()
        */

        //清空指定玩家的棋型数据
        public void clear(int playerNum)
        {
            for (int i = 0; i < Row; i++)      //清空棋型
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] == 0)
                    {
                        style[playerNum - 1, i, j] = 0;                 //清除最大棋型
                    }
                }
        }
        /*未测试 通过
         * 测试位置：Form1.
        */

        //清除棋型信息
        private void clearStyle(int x, int y)
        {
            for (int k = 0; k < players; k++)
                style[k, x, y] = 0;
        }
        /*测试通过
         * 测试位置：clear()
        */

        //将棋型转化成字符串
        public string ToString(int qixing)
        {   //十位表示棋子数，个位为棋型状态 2活棋、1冲棋、0死棋中的一个
            string str = "死冲活";
            int m = qixing / 10, n = qixing % 10;
            if (n > 2 || n < 0) return "";
            return str[n] + m.ToString();
        }
        /*测试通过
         * 测试位置：Form1.Tip
        */

        //在棋盘坐标(x, y)处落棋（棋型player1）
        public void put(int x, int y, int player1)
        {
            if (num < X.Length && player[x, y] == 0)        //棋盘未满且当前位置无棋
            {
                count[player1-1]++;                         //棋子数加一
                player[x, y] = player1;                     //记录棋子
                X[num] = x;                                 //记录坐标
                Y[num] = y;
                clearStyle(x, y);                           //清空该坐标处的棋型信息
                refreshStyle(x, y);                         //更新周边空位置棋型
                num++;                                      //棋子计数
            }
        }
        /*测试通过
         * 测试位置：Form1.putChess()
        */

        //悔棋，回退
        public void retreat()
        {
            if (num > 0)
            {
                num--;                                          //棋子总数减一
                count[player[X[num], Y[num]] - 1]--;            //玩家棋子减一
                player[X[num], Y[num]] = 0;                     //清除棋子记录
                saveStyle(X[num], Y[num]);                      //更新该空位置的棋型
                refreshStyle(X[num], Y[num]);                   //更新周边空位置棋型
            }
        }
        /*测试通过
         * 测试位置：Form1.button4_Click(）
        */

        //判断坐标(x,y)周围是否形成五子
        public bool fiveNum(int x, int y)
        {
            int[] Z = new int[8];               //分别记录八个方向上依次相邻的棋子个数
            int[] A;                            //记录坐标

            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 4; j++)    //在每个方向上依次取4个棋子与当前棋子比较
                {
                    A = way(x, y, i, j);        //获取方向i第j颗棋子的坐标
                    if (A[0] != -1)             //正确获取坐标
                        if (player[A[0], A[1]] == player[x, y]) Z[i - 1]++;//统计棋子
                        else break;             //与当前坐标的棋子不同
                    else break;                 //超出棋盘范围
                }
                if (Z[i - 1] == 4) return true; //当某个方向上有四个相邻的棋子时
            }

            //当有四个相邻的棋子时，与当前棋子构成五子
            if (Z[0] + Z[4] >= 4 || Z[1] + Z[5] >= 4 || Z[2] + Z[6] >= 4 || Z[3] + Z[7] >= 4)
                return true;
            else return false;
        }
        /*测试通过
         * 测试位置：Form1.putChess()
        */

        //保存空位置棋型信息（保存空位置(x,y)周边的棋型到style数组中）
        private void saveStyle(int x, int y)
        {
            for (int i = 0; i < players; i++)
                style[i, x, y] = getStyle(x, y, i + 1);         //保存最大棋型
        }
        /*测试通过
         *测试位置：put()
         *      saveStyle(x, y);
                string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")\n";
                for (int i = 0; i < players; i++)
                    str2 += "玩家" + (i + 1).ToString() + "的最大棋型：" + ToString(style[i, x, y]) + "; 周围棋子数：" + style[i + players, x, y].ToString() + "\n";
                str2 += "各方棋子总数：" + style[2 * players, x, y].ToString() + "\n";
                MessageBox.Show(str2);
       */

        //更新棋型信息（更新该坐标周围空位置的棋型）
        private void refreshStyle(int x, int y)
        {                                           //下标判断
            if (!(0 <= x && x < Row && 0 <= y && y < Line)) return;

            int[] A;                                //记录坐标

            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 5; j++)        //在每个方向上依次取5个坐标与当前坐标比较
                {
                    A = way(x, y, i, j);            //获取方向i第j个坐标
                    if (A[0] != -1)                 //正确获取坐标
                    {
                        if (player[A[0], A[1]] == 0)//&& inRange(A[0], A[1], x, y))//是空位置且邻近
                        {
                            saveStyle(A[0], A[1]);  //更新该空位置的棋型信息
                        }
                    }
                    else break;                     //不在棋盘范围内
                }
            }
        }
        /*测试通过
         *测试位置：put()
         *      string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")\n";
                for (int i = 0; i < players; i++)
                    str2 += "玩家" + (i + 1).ToString() + "的最大棋型：" + ToString(style[i, x, y]) + "; 周围棋子数：" + style[i + players, x, y].ToString() + "\n";
                str2 += "各方棋子总数：" + style[2 * players, x, y].ToString() + "\n";
                MessageBox.Show(str2);
         */

        //计算坐标(x,y)在方向w的第num个位置的坐标
        private int[] way(int x, int y, int w, int num)
        {   //(X,Y)为8个单位方向向量，分别为朝0度、45度、90度、135度 到315度方向变化
            int[] X = { 0, -1, -1, -1, 0, 1, 1, 1 },
                  Y = { 1, 1, 0, -1, -1, -1, 0, 1 },
                  A = { -1, -1 };
            if (w < 1 || w > 8) return A;   //方向需在[1,8]之间, 5到8分别为1到4的反方向
            A[0] = x + X[w - 1] * num;        //计算行坐标
            A[1] = y + Y[w - 1] * num;        //计算列坐标
            if (0 <= A[0] && A[0] < Row && 0 <= A[1] && A[1] < Line) return A;//在棋盘的坐标范围则返回正确坐标
            else { A[0] = -1; return A; }
        }
        /*测试通过
         * 测试位置：getStyle(,,,)、fiveNum(,)等
        */

        //判断棋子（ 坐标(x,y)在w方向，1到num个坐标范围内playerNum的个数, 连续或非连续 ）
        private int sameNum(int x, int y, int w, int num, int playerNum, bool continuation)
        {
            int tmp = 0;
            int[] A;
            for (int i = 1; i <= num; i++)      //在w方向统计有效位数
            {
                A = way(x, y, w, i);
                if (A[0] != -1)                 //在棋盘范围内
                {
                    if (player[A[0], A[1]] == playerNum) tmp++;
                    else if (player[A[0], A[1]] != 0) break;//其他值在中间
                    else
                    {
                        if (continuation) break;//continuation==true为要求连续 
                    }
                }
                else break;
            }
            return tmp;
        }
        /*测试通过
         * 测试位置：ourSite(）
        */

        //判断有效坐标数（ 坐标(x,y)在w方向 1到num 个坐标范围内己方棋子或空位置数，仅含playerNum或0）
        public int ourSite(int x, int y, int w, int num, int playerNum)
        {
            //return sameNum(x, y, w, num, playerNum, false) + sameNum(x, y, w, num, 0, false);
            int tmp = 0;
            int[] A;
            for (int i = 1; i <= num; i++)      //在w方向统计有效位数
            {
                A = way(x, y, w, i);
                if (A[0] != -1)                 //在棋盘范围内
                {
                    if (player[A[0], A[1]] == playerNum || player[A[0], A[1]] == 0) tmp++;
                    else break;//其他值在中间
                }
                else break;
            }
            return tmp;
        }
        /*测试通过
         * 测试位置：getStyle(,,,)
        */

        //判断玩家棋子数（ 在正反w方向包含坐标(x,y) 的num个坐标范围内，不要求连续的最大棋子数
        public int getPlayerNum(int x, int y, int w, int num, int playerNum)
        {
            int max = 0, tmp = 0;
            int[] A;
            for (int i = 0; i < num; i++)
            {
                A = way(x, y, w + 4, i);        //依次朝反方向平移
                if (A[0] != -1)                 //在棋盘范围内
                {
                    tmp = sameNum(A[0], A[1], w, num - 1, playerNum, false);    //计算w方向的棋子数
                    if (player[A[0], A[1]] == playerNum) tmp++;                 //判断自身棋型
                    else if (player[A[0], A[1]] != 0) break;                    //不为空退出
                }
                else break;

                if (max < tmp) max = tmp;       //判断最大棋型
            }
            return max;
        }
        /*测试通过
         * 测试位置：getStyle(,,,)
        */

        //计算玩家在坐标处的 最大棋子数
        public int getPlayerNum(int x, int y, int playerNum)
        {
            int max = 0, num = 0;
            for (int i = 1; i <= 4; i++)                    //分别判断4个方向
            {
                num = getPlayerNum(x, y, i, 5, playerNum);  //计算棋子数
                if (max < num) max = num;                   //记录最大棋型
            }
            return max;
        }


        //计算坐标(x,y)在w*45度方向棋子playerNum的最大棋型  十位为棋子个数，个位为棋型状态(活棋)、0(死棋)、1(冲棋)
        private int getStyle(int x, int y, int w, int playerNum)
        {
            int num1, num2, tmp;
            int[] A;

            if (4 < w && w <= 8) w -= 4;        //限定w在1到4之间
            tmp = ourSite(x, y, w, 4, playerNum) + ourSite(x, y, w + 4, 4, playerNum);  //判断在空位置处w的正反方向有效坐标数

            if (tmp < 4) return 0;                                      //两边的有效位小于4为死棋
            else if (tmp == 4)                                          //两边的有效位等于4为冲棋
            {   //计算最大棋子数
                return getPlayerNum(x, y, w, 5, playerNum) * 10 + 1;
            }
            else
            {
                num1 = getPlayerNum(x, y, w, 5, playerNum);             //计算最大棋子数
                num2 = sameNum(x, y, w, num1, playerNum, true);         //计算w方向最大连续棋子数
                tmp = sameNum(x, y, w + 4, num1 - num2, playerNum, true);   //计算w反方向连续棋子数

                if (num1 != num2 + tmp)
                    return num1 * 10 + 1;                 //为冲棋
                else   //连续棋子端至少有棋子数加1个有效位，且棋子与当前坐标相邻，则为活棋
                {
                    A = way(x, y, w, num2 + 1);
                    if (!(A[0] != -1 && player[A[0], A[1]] == 0)) return num1 * 10 + 1; //冲棋

                    A = way(x, y, w + 4, tmp + 1);
                    if (A[0] != -1 && player[A[0], A[1]] == 0) return num1 * 10 + 2; //活棋
                    else return num1 * 10 + 1; //冲棋
                }
            }
        }
        /*未测试 通过
         * 测试位置：getStyle(,,)
        */

        //获取坐标(x,y)处playerNum的最大棋型
        public int getStyle(int x, int y, int playerNum)
        {
            if (x < 0 || y < 0) return 0;

            int max = 0, num = 0;
            for (int i = 1; i <= 4; i++)            //分别判断4个方向的棋型
            {
                num = getStyle(x, y, i, playerNum); //获取棋型
                if (max < num) max = num;           //记录最大棋型
            }
            return max;
        }
        /*测试通过
         * 测试位置：Form1.putChess()
        string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")的最大棋型：\n";
                    str2 +=qipan.getStyle(x, y,1).ToString() + "\n";
                MessageBox.Show(str2);
        */

        //获取玩家的最大棋型（随机获取最大的某一个）
        public int[] getMaxStyle(int playerNum)
        {
            Random rnd = new Random();
            int[] max = { -1, -1, 0, playerNum };   //行列坐标，最大棋型，玩家号
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                    if (max[2] < style[playerNum - 1, i, j] || (max[2] == style[playerNum - 1, i, j] && rnd.Next(5) > 3))
                    {
                        max[0] = i;
                        max[1] = j;
                        max[2] = style[playerNum - 1, i, j];
                    }
            return max;
        }
        /*测试通过
         * 测试位置：Form1.Tip()
        */


        //获取坐标(x,y)处棋型的排序数，按棋型从大到小组合成一个八位的整数
        public int sortStyle(int x, int y, int playerNum)
        {
            int[] tmp = new int[players * 4], tmp2 = new int[4];
            int n, sortNum = 0, tmp3=0;

            for (int k = 1; k <= players; k++)          //对于每个玩家
                for (int i = 1; i <= 4; i++)            //分别判断4个方向的棋型
                {
                    tmp3 = getStyle(x, y, i, k);        //获取棋型
                    if (k == playerNum)
                    {
                        if (tmp3 % 10 == 1) tmp3 += 2;  //3
                        else if (tmp3 % 10 == 2) tmp3 += 5;//7
                    }
                    else if (tmp3 % 10 == 2) tmp3 += 3; //5

                    tmp[(k - 1) * 4 + i - 1] = tmp3;    //己方棋型，相同棋子数时优先级大于对家， 两家冲棋、活棋权值分别为1、5；3、7
                }

            for (int j = 0; j < tmp.Length; j++)
            {
                n = tmp[j] / 10;                        //判断棋子数
                if (n == 0 || n > 4) continue;
                tmp2[n - 1] += tmp[j];                  //统计各棋子数的棋型和
            }

            for (int j = 3; j >= 0; j--)                //组成排序数
                sortNum = sortNum * 100 + tmp2[j];
            return sortNum;
        }
   

        ////获取坐标(x,y)处棋型的排序数，按棋型从大到小组合成一个八位的整数
        //public int sortStyle(int x, int y, int playerNum)
        //{
        //    int[] tmp = new int[players * 4];

        //    for (int k = 1; k <= players; k++)          //对于每个玩家
        //        for (int i = 1; i <= 4; i++)            //分别判断4个方向的棋型
        //        {
        //            tmp[(k - 1) * 4 + i - 1] = getStyle(x, y, i, k);    //获取棋型
        //            if (k == playerNum) tmp[(k - 1) * 4 + i - 1] += 4;  //己方棋型，相同棋子数时优先级大于其他玩家
        //        }

        //    for (int j = 0; j < 4; j++)                  //获取前四大棋型
        //        for (int k = j + 1; k < players * 4; k++)
        //            if (tmp[j] < tmp[k])                //交换
        //            {
        //                tmp[j] += tmp[k];
        //                tmp[k] = tmp[j] - tmp[k];
        //                tmp[j] -= tmp[k];
        //            }

        //    int sortNum = tmp[0];                       //组成排序数
        //    for (int j = 1; j < 4; j++)
        //        sortNum = sortNum * 100 + tmp[j];
        //    return sortNum;
        //}
        ///*未测试 通过
        // * 测试位置：Form1.putChess()
        //string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")的最大棋型：\n";
        //            str2 +=qipan.getStyle(x, y,1).ToString() + "\n";
        //        MessageBox.Show(str2);
        //*/


        //在坐标处试探性的落入棋子，判断是否会达到styleNum以上棋型（要求该位置为空，返回该位置两边的空位置和方向）
        public int[] biggerThan(int x, int y, int playerNum, int styleNum)
        {
            int[] A, B = { -1, -1, 0, -1, -1, 0};
            if (!(0 <= x && x < Row && 0 <= y && y < Line)) return B; //下标判断
            if (player[x, y] != 0) return B;                //空位置判断

            int num, w;

            player[x, y] = playerNum;                       //在该坐标落棋
            for (int i = 1; i <= 4; i++)
            {
                w = i;

            backwords:
                num = sameNum(x, y, w, 4, playerNum, true); //判断i方向连续棋子个数
                A = way(x, y, w, num + 1);                  //获取方向i第num + 1个位置的坐标
                if (A[0] != -1)                             //正确获取坐标
                {
                    if (player[A[0], A[1]] == 0)            //是空位置
                    {
                        if (getStyle(A[0], A[1], w, playerNum) > styleNum)//判断w方向棋型是否超过活styleNum
                        {
                            //if (num > 0)                    //仅获取有棋子的那边空位位置
                            {
                                if (B[0] == -1) { B[0] = A[0]; B[1] = A[1]; B[2] = w; }
                                else { B[3] = A[0]; B[4] = A[1]; B[5] = w; }
                            }
                            w += 4;
                            if (w <= 8) goto backwords;
                        }
                    }

                    if (B[0] != -1)                    //正确获取坐标
                    {
                        player[x, y] = 0;              //取走棋子
                        return B;
                    }
                }

                w += 4;
                if (w <= 8) goto backwords;                 //i方向的反向在计算一次
            }
            player[x, y] = 0;                               //取走棋子
            B[0] = -1;
            return B;
        }
        /*未测试 通过
                * 测试位置：Form1.putChess()
               string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")的最大棋型：\n";
                           str2 +=qipan.getStyle(x, y,1).ToString() + "\n";
                       MessageBox.Show(str2);
               */

        //在坐标处试探性的落入棋子，判断是否会达到两个方向棋型大于styleNum（以该点为交点的双活棋点，要求该位置为空）
        public bool doubleBigger(int x, int y, int playerNum, int styleNum, int playerNum2, int[] B)//B为更大棋型空位置
        {
            if (!(0 <= x && x < Row && 0 <= y && y < Line)) return false; //下标判断
            if (player[x, y] != 0) return false;  //空位置判断

            int num, w, count = 0;
            int[] A, tmp={0,0};

            player[x, y] = playerNum;             //在该坐标落棋
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    w = i + j * 4;              //计算方向
                    num = sameNum(x, y, w, 4, playerNum, true);     //判断i方向连续棋子个数
                    A = way(x, y, w, num + 1);                      //获取方向i第num + 1个位置的坐标
                    if (A[0] != -1 && player[A[0], A[1]] == 0)      //正确获取坐标  且 是空位置
                        tmp[j] = getStyle(A[0], A[1], w, playerNum);//获取w方向棋型
                    else tmp[j] = 0;
                }

                if (tmp[0] < tmp[1]) tmp[0] = tmp[1];               //取较大的棋型
                if (tmp[0] > styleNum)
                {
                    if (B == null || B[0] == -1) count++;           //记录更大棋型
                    else if (biggerThan(B[0], B[1], playerNum2, tmp[0])[0] == -1)
                        count++;                                    //要求对家堵棋后不会有更大棋型
                }

                if (count > 1)
                {
                    player[ x, y] = 0;
                    return true;
                }
            }
            player[x, y] = 0;                     //取走棋子
            return false;
        }
        /*未测试 通过
                * 测试位置：Form1.putChess()
               string str2 = "当前位置(" + x.ToString() + "," + y.ToString() + ")的最大棋型：\n";
                           str2 +=qipan.getStyle(x, y,1).ToString() + "\n";
                       MessageBox.Show(str2);
               */

        private int doubleBigger(int site, int playerNum, int styleNum)
        {
            return doubleBigger(site / 1000, site % 1000, playerNum, styleNum);
        }
        //返回包含较大较小方向的整数
        public int doubleBigger(int x, int y, int playerNum, int styleNum)
        {
            if (!(0 <= x && x < Row && 0 <= y && y < Line)) return 0; //下标判断
            if (player[x, y] != 0) return 0;  //空位置判断

            int num, w, count = 0, w1 = 0, w2 = 0;
            int[] A, tmp = { 0, 0 };

            player[x, y] = playerNum;             //在该坐标落棋
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    w = i + j * 4;              //计算方向
                    num = sameNum(x, y, w, 4, playerNum, true);     //判断i方向连续棋子个数
                    A = way(x, y, w, num + 1);                      //获取方向i第num + 1个位置的坐标
                    if (A[0] != -1 && player[A[0], A[1]] == 0)      //正确获取坐标  且 是空位置
                        tmp[j] = getStyle(A[0], A[1], w, playerNum) * 10 + w;        //获取w方向棋型,同时附带方向
                    else tmp[j] = 0;
                }

                if ((int)(tmp[0] / 10) < (int)(tmp[1] / 10)) tmp[0] = tmp[1]; //取较大的棋型
                if ((int)(tmp[0] / 10) > styleNum)
                {
                    count++;
                    if (w1 == 0) w1 = tmp[0];       //记录信息
                    else w2 = tmp[0];
                }

                if (count > 1)
                {
                    player[x, y] = 0;             //返回包含较大较小方向的整数
                    return w1 > w2 ? 10 * (w1 % 10) + (w2 % 10) : 10 * (w2 % 10) + (w1 % 10);
                }
            }
            player[x, y] = 0;                     //取走棋子
            return 0;
        }
       

        //判断玩家是否存在某个位置落入棋子后可以达到两个双活棋
        //1、先判断活2位置，在活2和其临近空位置
        //2、方向找可达到双活2的位置
        //    3、在该位置找较小棋型方向，

        //获取活2的临近可以成活3的空位置
        private int[] getWsite2(int site, int playerNum)
        {//获取活2的临近空位置
            int[] A, B;
            int x = site / 1000, y = site % 1000;   //活2位置
            for (int w = 1; w <= 4; w++)
            {
                A = way(x, y, w, 1);                //该坐标两边的坐标
                B = way(x, y, w+4, 1);
                if (A[0] == -1 || B[0] == -1) continue;
                if (player[A[0], A[1]] == playerNum && player[B[0], B[1]] == playerNum)
                {                                   //该位置两边有己方棋子
                    A = way(x, y, w, 2);
                    B = way(x, y, w + 4, 2);
                    if (A[0] == -1 || B[0] == -1) continue;
                    int[] C = { A[0], A[1], B[0], B[1] };
                    return C;
                }
                else if (player[A[0], A[1]] == 0 && player[B[0], B[1]] == 0)
                    continue;                       //两边都无棋子
                else
                {
                    if (player[A[0], A[1]] == 0)    //单边有两个棋子
                    {
                        B = way(x, y, w + 4, 2);
                        if (B[0] == -1) continue;
                        if (player[B[0], B[1]] == playerNum)
                        {
                            int[] C = { A[0], A[1], -1 };
                            return C;
                        }
                    }
                    else                           //另一边有两个棋子
                    {
                        A = way(x, y, w, 2);
                        if (A[0] == -1) continue;
                        if (player[A[0], A[1]] == playerNum) return B;
                        {
                            int[] C = { B[0], B[1], -1 };
                            return C;
                        }
                    }
                }
            }
            int[] D = { -1, -1, -1 };
            return D;
        }

        //对getWsite1的结果进行整合
        private int[] getWsite11(int siteWay, int playerNum)
        {
            if (siteWay == 0) return null;
            int[] A = getWsite1(siteWay, playerNum);
            if (A[0] == -1 || player[A[0], A[1]] != 0 || A[2] == -1 || player[A[2], A[3]] != 0 || A[4] == -1 || player[A[4], A[5]] != 0) return null;

            int[] B = { siteWay / 10, A[0] * 1000 + A[1], A[2] * 1000 + A[3], A[4] * 1000 + A[5] };
            return B;
        }

        //获取该位置与活1的临近可以成活3的空位置
        private int[] getWsite1(int siteWay,int playerNum)
        {
            int[] A = { -1, -1 }, B = { -1, -1 }, C = { -1, -1 };
            int w= siteWay % 10, x, y;
            siteWay /= 10;
            x = siteWay / 1000;
            y = siteWay % 1000;
            if (w > 4) w -= 4;

            A = way(x, y, w, 1);        //w1为玩家棋子
            if (A[0] != -1 && player[A[0], A[1]] == playerNum)
            {
                A = way(x, y, w, 2);
                B = way(x, y, w, 3);
                C = way(x, y, w+4, 1);
                int[] D = {A[0],A[1],B[0],B[1],C[0],C[1]};
                return D;
            }

            B = way(x, y, w, 2);        //w2
            if(B[0]!=-1 && player[B[0],B[1]] == playerNum)
            {
                B = way(x, y, w, 3);
                C = way(x, y, w + 4, 1);
                int[] D = { A[0], A[1], B[0], B[1], C[0], C[1] };
                return D;
            }

            B = way(x, y, w+4, 1);      //(w+4)1
            if (B[0] != -1 && player[B[0], B[1]] == playerNum)
            {
                B = way(x, y, w + 4, 2);
                C = way(x, y, w + 4, 3);
                int[] D = { A[0], A[1], B[0], B[1], C[0], C[1] };
                return D;
            }

            C = way(x, y, w + 4, 3);    //(w+4)2
            int[] E = { A[0], A[1], B[0], B[1], C[0], C[1] };
            return E;
        }

        private int equal(int[] A, int[] B)
        {
            if ((A == null || B == null)) return 0;
            for(int i=0; i<4; i++)
                for(int j=0; j<4; j++)
                    if (A[i] == B[j]) return  A[i];
            return 0;
        }

        //该函数是用于判断这样的位置，在该位置放入一颗棋子后，在棋盘中可同时出现两个双活棋位置，而在这该棋子落入之前，一个双活棋位置都没有
        public int[] doubleBiggerTwice(int playerNum)   
        {
            int num = 0, p = 0, num2=0, tmp3 =0;
            int[] D = { -1, -1 };
            int[] site = new int[Row * Line / 2], tmp, site2 = new int[Row * Line / 4];
            //活2位置判断，存储到site中，num计数
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] != 0) continue;   //该位置有棋子，跳过
                    if (style[playerNum - 1, i, j] >= 22) site[num++] = i * 1000 + j;  //统计和记录活2及以上棋型位置
                }
            if (num < 2) return D;                      //至少有两个活2点


            while (p < num)                          //获取活3和活2的交点
            {
                //判断当前坐标点
                tmp3 = doubleBigger(site[p], playerNum, 21);
                if ( tmp3!= 0) site2[num2++] = site[p]*10+tmp3%10;  //保存坐标，并附加较小的方向

                //获取临近可成活3的临近位置
                tmp = getWsite2(site[p], playerNum);
                //判断临近位置
                if (tmp[0] != -1)
                {
                    //第一个临近活3位置判断
                    tmp3 = doubleBigger(tmp[0], tmp[1], playerNum, 21);
                    if (tmp3 != 0) site2[num2++] = (tmp[0] * 1000 + tmp[1]) * 10 + tmp3 % 10;  //保存坐标，并附加较小的方向

                    //第一个临近活3位置判断
                    if (tmp.Length > 2 && tmp[2] != -1)
                    {
                        tmp3 = doubleBigger(tmp[2], tmp[3], playerNum, 21);
                        if (tmp3 != 0) site2[num2++] = (tmp[2] * 1000 + tmp[3]) * 10 + tmp3 % 10;  //保存坐标，并附加较小的方向
                    }
                }
                p++;
            }

            for(int i=0; i<num2 -1; i++)               //去除重复坐标
                for (int j = i + 1; j < num2; j++)
                    if ((int)(site2[i] / 10) == (int)(site2[j] / 10) && site2[i] != 0) site2[j] = 0;


            int q = 0;
            int[][] site3 = new int[num2][];
            p = 0;
            while (p < num2)                           //统计较小活2可成活3方向的空位置
            {
                if (site2[p++] > 0)
                    site3[q] = getWsite11(site2[q++], playerNum);
            }

            for (int i = 0; i < num2 - 1; i++)         //判断坐标是否有交点
                for (int j = i + 1; j < num2; j++)
                {
                    num = equal(site3[i], site3[j]);
                    if (num > 0)
                    {
                        int[] A = { num / 1000, num % 1000 };
                        return A;
                    }
                }

            return D;
        }



        //获取坐标周围大于指定棋型的空位置（4子距离范围以内）
        private SiteQueue getSiteQueue(int x, int y, int playerNum, int styleNum, int playerNum2)
        {
            SiteQueue Q = new SiteQueue();
            int[] A, B;

            //if (exceptW > 8) return null;
            //else if (exceptW > 4 || exceptW < 1) exceptW -= 4;//方向限定在1到4

            for (int i = 1; i <= 8; i++)
            {
                //if (i == exceptW || i == exceptW + 4) continue;//正反exceptW方向不统计
                for (int j = 1; j <= 4; j++)        //在每个方向上依次取5个坐标与当前坐标比较
                {
                    A = way(x, y, i, j);            //获取方向i第j个坐标
                    if (A[0] != -1)                 //正确获取坐标
                    {
                        if (player[A[0], A[1]] == 0)//是更大棋型空位置
                        {
                            B=biggerThan(A[0], A[1], playerNum, styleNum);
                            if (B[0] != -1)
                            {
                                if (biggerThan(B[0], B[1], playerNum2, styleNum + 1)[0] != -1) continue;//排除该位置,对家堵棋后会有更大棋型
                                Q.In(A);            //坐标入队
                                Q.In2(B[0], B[1]);  //更大棋型空位置入队
                                if (B[3] == -1) Q.In3(-1, -1);
                                else Q.In3(B[3], B[4]);   //另一个空位置入队
                            }
                        }
                        else if (player[A[0], A[1]] != 0 && player[A[0], A[1]] != playerNum) break;//遇到非玩家棋子
                    }
                    else break;                     //不在棋盘范围内
                }
            }

            if (Q.length() > 0) return new SiteQueue(Q);
            else return null;
        }

        ////判断坐标周围是否存在双活棋位置,且有一方向大于styleNum
        //private int[] think(int x, int y, int playerNum, int styleNum, int exceptW, int playerNum2)
        //{
        //    SiteQueue Q = getSiteQueue(x, y, playerNum, styleNum+1, exceptW, playerNum2);  //获取更大棋型坐标队列
        //    int[] A = { -1, -1 };

        //    if (Q == null) return A;    //队列为空时
        //    while (!Q.empty())          //队列中有元素
        //    {
        //        A = Q.Out();            //依次获取坐标
        //        if (doubleBigger(A[0], A[1], playerNum, 31))
        //            return A;           //若为双活棋位置，则返回坐标
        //    }
        //    A[0] = -1;
        //    return A;
        //}

        //判断坐标周围 是否存在双活棋
        private int[] think(int x, int y, int playerNum, int styleNum, int playerNum2)
        {
            int[] A = { -1, -1 }, B = { -1, -1 }, C = { -1, -1, 0 }, B2;

            C = biggerThan(x, y, playerNum, styleNum);

            //对家落棋后可达到同样大棋型
            if (C[0] != -1 && biggerThan(C[0], C[1], playerNum2, styleNum)[0] != -1) return A;
            if (C[3] != -1 && biggerThan(C[3], C[4], playerNum2, styleNum)[0] != -1) return A;

            player[x, y] = playerNum;   //落棋
            if (C[0] != -1) player[C[0], C[1]] = playerNum2;//落入对家的棋子
            if (C[3] != -1) player[C[3], C[4]] = playerNum2;//落入对家的棋子

            SiteQueue Q = getSiteQueue(x, y, playerNum, styleNum, playerNum2);//获取坐标周围大于styleNum的所有空位置

            if (Q != null)
            {
                //判断坐标周围是否存在双活棋位置
                while (!Q.empty())          //队列中有元素
                {
                    A = Q.Out();            //依次获取坐标
                    B = Q.Out2();           //获取空位置
                    B2 = Q.Out3();
                    if (doubleBigger(A[0], A[1], playerNum, 31, playerNum2, B) 
                        && doubleBigger(A[0], A[1], playerNum, 31, playerNum2, B2)) goto last;
                }

                Q.again();                  //再次输出队列中的坐标
                while (!Q.empty())          //队列中有元素
                {
                    A = Q.Out();            //依次获取坐标
                    if (think(A[0], A[1], playerNum, styleNum,playerNum2)[0] != -1)          //判断该坐标周围是否有双活棋
                        goto last;
                }
            }

            A[0] = -1;

            last:
            if (C[0] != -1) player[C[0], C[1]] = 0;//取走对家的棋子
            if (C[3] != -1) player[C[3], C[4]] = 0;
            player[x, y] = 0;               //取走棋子
            return A;
        }


        ////判断坐标周围 2步以后是否存在双活棋
        //private int[] think2(int x, int y, int playerNum, int styleNum)
        //{
        //    int[] A = think1(x, y, playerNum, styleNum);
        //    if (A[0] != -1) return A;   //先判断是否存在双活棋

        //    SiteQueue Q = getSiteQueue(x, y, playerNum, styleNum);  //获取坐标队列

        //    if (Q == null) return A;    //队列为空时
        //    while (!Q.empty())          //队列中有元素
        //    {
        //        A = Q.Out();            //依次获取坐标

        //        player[A[0], A[1]] = playerNum;//在该坐标处落棋
        //        if (think1(A[0], A[1], playerNum, styleNum)[0] != -1)//判断该坐标周围是否有双活棋
        //        {
        //            player[A[0], A[1]] = 0; //取走棋子
        //            return A;               //若为双活棋位置，则返回坐标
        //        }
        //        player[A[0], A[1]] = 0; //取走棋子
        //    }

        //    A[0] = -1;
        //    return A;
        //}

        ////判断坐标周围 3步以后是否存在双活棋
        //private int[] think3(int x, int y, int playerNum, int styleNum)
        //{
        //    int[] A = think2(x, y, playerNum, styleNum);
        //    if (A[0] != -1) return A;   //先判断是否存在双活棋

        //    SiteQueue Q = getSiteQueue(x, y, playerNum, styleNum);  //获取坐标队列

        //    if (Q == null) return A;    //队列为空时
        //    while (!Q.empty())          //队列中有元素
        //    {
        //        A = Q.Out();            //依次获取坐标

        //        player[A[0], A[1]] = playerNum;//在该坐标处落棋
        //        if (think2(A[0], A[1], playerNum, styleNum)[0] != -1)//判断该坐标周围是否有双活棋
        //        {
        //            player[A[0], A[1]] = 0; //取走棋子
        //            return A;               //若为双活棋位置，则返回坐标
        //        }
        //        player[A[0], A[1]] = 0; //取走棋子
        //    }

        //    A[0] = -1;
        //    return A;
        //}

        //思考算法

        private int[] think(int playerNum, int styleNum, int playerNum2)
        {
            int[] A = { -1, -1 }, B, C;                        //记录坐标
            if (count[playerNum - 1] < 4) return A;         //玩家棋子数少于4

            SiteQueue Q = getStyleSite(playerNum, styleNum+1);//获取大于该棋型的所有坐标

            if (Q == null) return A;                        //无该棋型
            while (!Q.empty())                              //先判断是否为双向活棋
            {
                A = Q.Out();                                //依次取出坐标
                B = Q.Out2();
                C = Q.Out3();
                if (doubleBigger(A[0], A[1], playerNum, 31, playerNum2, B) 
                    && doubleBigger(A[0], A[1], playerNum, 31, playerNum2, C))
                    return A;
            }

            //判断玩家是否拥有某个位置放入棋子可以同时达到两个方向的双活棋
            if (styleNum <= 31)
            {
                A = doubleBiggerTwice(playerNum);
                if (A[0] != -1) return A;
            }

            Q.again();
            while (!Q.empty())
            {
                A = Q.Out();
                B = think(A[0], A[1], playerNum, styleNum,playerNum2);//获取棋子周边双活棋位置
                if (B[0] != -1) return A;
            }
            A[0] = -1;
            return A;
        }

        //变式双活棋的思考算法
        private int[] think2(int playerNum, int styleNum, int playerNum2)
        {
            int[] A = { -1, -1 }, B = { -1, -1 }, B2, C, C2;//记录坐标
            if (count[playerNum - 1] < 4) return A;         //玩家棋子数少于4

            SiteQueue Q = getStyleSite(playerNum, styleNum + 1);//获取大于该棋型的所有坐标

            if (Q == null) return A;                        //无该棋型
            bool f = false;
            while (!Q.empty())                              //先判断是否为双向活棋
            {
                A = Q.Out();                                //依次取出坐标
                C = Q.Out2();
                C2 = Q.Out3();
                if (biggerThan(A[0], A[1], playerNum2, styleNum - 1)[0] != -1) continue;//该处对家可达到相同棋型
                for (int i = 1; i <= 8; i++)
                {
                    B = way(A[0], A[1], i, 1);              //坐标周围的棋子
                    if (B[0] == -1 || player[B[0], B[1]] != 0) continue;//不存在或不为空

                    f = false;
                    player[B[0], B[1]] = playerNum;         //落入棋子
                    if (doubleBigger(A[0], A[1], playerNum, 31, playerNum2, C) 
                        && doubleBigger(A[0], A[1], playerNum, 31, playerNum2, C2))
                    {
                        player[A[0], A[1]] = playerNum2;    //双活棋点落入对家棋子
                        B2 = think(playerNum, styleNum, playerNum2);    //存在别的双活棋点，非C/C2这两个空位置
                        if (B2[0] != -1 && (B2[0] != C[0] || B2[1] != C[1]) && (B2[0] != C2[0] || B2[1] != C2[1]))
                            f = true;
                        player[A[0], A[1]] = 0;
                    }
                    else
                    {
                        player[A[0], A[1]] = playerNum2;    //落入对家棋子
                        if(think(B[0], B[1], playerNum, styleNum, playerNum2)[0]!=-1)
                            f = true;                       //判断坐标B周围是否有双活棋
                        player[A[0], A[1]] = 0;
                    }
                    player[B[0], B[1]] = 0;

                    if (f) return B;
                }
            }
            A[0] = -1;
            return A;
        }

        //判断玩家playerNum是否拥有棋型styleNum， 返回该棋型的坐标
        public int[] haveStyle(int playerNum, int styleNum)
        {
            int[] tmp = { -1, -1 };
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                    if (styleNum == style[playerNum - 1, i, j])
                    {                   //拥有该棋型
                        tmp[0] = i;
                        tmp[1] = j;
                        return tmp;     //返回该棋型的坐标
                    }
            return tmp;                 //返回该棋型的坐标
        }
        /*未测试 通过
         * 测试位置：Form1.Tip()
        */

        //获取玩家该棋型和该棋型以上的所有空位置的坐标
        private SiteQueue getStyleSite(int playerNum, int styleNum)
        {
            SiteQueue Q = new SiteQueue();
            int[] tmp = { -1, -1 }, B;

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] != 0) continue;   //该位置有棋子，跳过
                    if (styleNum - 11 <= style[playerNum - 1, i, j])
                    {
                        B = biggerThan(i, j, playerNum, styleNum - 1);
                        if (B[0] != -1)
                        {
                            tmp[0] = i;
                            tmp[1] = j;
                            Q.In(tmp);                //该棋型的坐标入队
                            Q.In2(B[0], B[1]);        //棋型空位置入队
                            if (B[3] == -1) Q.In3(-1,-1);
                            else Q.In3(B[3], B[4]);   //另一个空位置入队
                        }
                    }
                    //if (styleNum - 10 <= style[playerNum - 1, i, j])
                    //{                       //拥有该棋型
                    //    if (styleNum - 10 <= getStyle(i, j, playerNum))//重新计算该位置棋型
                    //    {
                    //        tmp[0] = i;
                    //        tmp[1] = j;
                    //        Q.In(tmp);      //该棋型的坐标入队
                    //    }
                    //}
                    //else if (styleNum - 10 == style[playerNum - 1, i, j] + 1)
                    //{
                    //    if (biggerThan(i, j, playerNum, styleNum - 1)[0] != -1)
                    //    {
                    //        tmp[0] = i;
                    //        tmp[1] = j;
                    //        Q.In(tmp);      //该棋型的坐标入队
                    //    }
                    //}
                }

            if (Q.length() > 0) return new SiteQueue(Q);
            else return null;
        }


        //玩家2达到活三时，选择更有益于自身的落点
        private int[] betterThree(int x, int y, int playerNum, int playerNum2)
        {
            int[] A = biggerThan(x, y, playerNum2, 32);         //获取活3棋型的空位坐标
            int[] D = { x, y, 0 };
            bool [] flag={false,false,false,true,true,true};    //前3个false表示不存在玩家2的活棋坐标，后3个true表示存在玩家的活棋

            int[]  B= { x, y, A[0], A[1], A[3], A[4] };         //活3的3个空位置坐标
            if (x - A[0] == 1 || x - A[0] == -1) { B[2] = B[4]; B[3] = B[5]; B[4] = -1; }//排除与(x,y)相邻的坐标
            else if (x - A[3] == 1 || x - A[3] == -1) B[4] = -1;

            for (int i = 0; i < 6; i += 2)
            {
                if (B[i] == -1) break;
                player[B[i], B[i+1]] = playerNum;               //落入玩家棋子

                if (think(playerNum2, 32, playerNum)[0] != -1) flag[i / 2] = true;//存在玩家2的活棋

                if (flag[i / 2])  flag[i / 2 + 3] = false; 
                else if(think(playerNum, 31, playerNum2)[0] == -1) flag[i / 2 + 3] = false;   //该位置放入棋子后不存在玩家的双活棋

                player[B[i], B[i + 1]] = 0;                     //取走棋子

            }

            for (int i = 0; i < 3; i++)                         //优先选择玩家2无活棋，玩家有活棋位置
            {
                if (B[i * 2] == -1) break;
                if (!flag[i] && flag[i + 3])
                {
                    int[] C = { B[i * 2], B[i * 2 + 1], 1 };
                    return C;
                }
            }

            for (int i = 0; i < 3; i++)                         //没有，则选择玩家2无活棋位置
            {
                if (B[i * 2] == -1) break;
                if (!flag[i])
                {
                    int[] C = { B[i * 2], B[i * 2 + 1], 1 };
                    return C;
                }
            }

            return D;                                           //依然没有
        }


        //获取玩家playerNum较理想的落棋点, playerNum2为后一位玩家
        public int[] getBetterSite3(int playerNum, int playerNum2)
        {
            int[] A = { Row / 2, Line / 2 }, B = { -1, -1 }, B2 = { -1, -1 }, C = { -1, -1 };
            int sortNum, tmp;
            Random rnd = new Random();

            if (num == 0)                           //没有棋子时,随机生成坐标
            {
                A[0] -= rnd.Next(Row / 3 - 2);
                A[1] -= rnd.Next(Line / 3 - 2);
                return A;
            }

            int[] p = getMaxStyle(playerNum);       //获取玩家自身的最大棋型
            int[] p2 = getMaxStyle(playerNum2);     //获取下一玩家的最大棋型

            //冲4或活4以上棋型直接返回坐标
            if (p[2] > 32) return p;
            if (p2[2] > 32) return p2;

            if (p[2] == 32) return p;               //玩家达到活3返回坐标

            if (p2[2] == 32)                        //下一位玩家达到活3
            {
                A = think(playerNum, 32, playerNum2);
                if (A[0] != -1) return A;           //玩家有更大的棋型

                C = betterThree(p2[0], p2[1], playerNum, playerNum2);
                if (C[2] == 1) return C;            //返回更有益于玩家的位置
                else
                {                                   //进一步比较
                    p = p2;
                    goto last;
                }
            }

            if (p[2] == 31)
            {
                B = think(playerNum, 32, playerNum2);
                if (B[0] != -1) return B;
                else B = think2(playerNum, 32, playerNum2);
                if (B[0] != -1) return B;
            }

            if (p2[2] == 31)
            {
                B2 = think(playerNum2, 32, playerNum);
                if (B2[0] != -1) return B2;
                //else B2 = think2(playerNum2, 32, playerNum);
                if (B2[0] != -1)
                {
                    p = p2;
                    goto last;
                }
            }

            if (p[2] >= 21)
            {
                B = think(playerNum, 31, playerNum2);
                if (B[0] != -1) return B;
            }

            if (p2[2] >= 21)
            {
                B2 = think(playerNum2, 31, playerNum);
                if (B2[0] != -1) return B2;
            }

            if (p[2] >= 21)
            {
                B = think2(playerNum, 31, playerNum2);
                if (B[0] != -1) return B;
            }
            if (p2[2] >= 21)
            {
                B2 = think2(playerNum2, 31, playerNum);
                if (B2[0] != -1) return B2;
            }

            if (p[2] - 9 >= p2[2]) goto checkLive;       //玩家棋型更大
            else if (p[2] >= p2[2]) goto last;

            if (p2[2] > p[2] && p2[2] % 10 == 2)   //在后一位玩家的棋型更大且为活棋时
            {
                p = p2;
                goto last;
            }

            if (p2[2] - p[2] > 18 && p2[2] % 10 == 1)//在后一位玩家的棋型更大且为冲棋时
            {
                p = p2;
                goto last;
            }

            checkLive:
            if (p[2] % 10 == 1)                     //计算玩家的小一阶活棋
            {
                A = haveStyle(playerNum, p[2] - 9);
                if (A[0] != -1)
                {
                    p[0] = A[0];
                    p[1] = A[1];
                    p[2] -= 9;
                }
                else p = p2;                      //没有小一阶的活棋则堵对家棋子
            }

            last:
            sortNum = 0;//排序数
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] == 0 && style[p[3] - 1, i, j] == p[2])//确定为空位置且在该位置取得最大棋型
                    {
                        tmp = sortStyle(i, j, playerNum);
                        if (tmp > sortNum || (tmp == sortNum && rnd.Next(7) > 4))//若排序数较大,或排序数相同时随机选择是否替换
                        {
                            sortNum = tmp;          //记录最大排序数
                            p[0] = i;               //记录坐标
                            p[1] = j;
                        }
                    }
                }
            /**/
            return p;
        }


        public int[] getBetterSite2(int playerNum, int playerNum2)
        {
            int[] A = { Row / 2, Line / 2 }, B = { -1, -1 }, B2 = { -1, -1 }, C = { -1, -1 };
            int sortNum, tmp;
            Random rnd = new Random();

            if (num == 0)                           //没有棋子时,随机生成坐标
            {
                A[0] -= rnd.Next(Row / 3 - 2);
                A[1] -= rnd.Next(Line / 3 - 2);
                return A;
            }

            int[] p = getMaxStyle(playerNum);       //获取玩家自身的最大棋型
            int[] p2 = getMaxStyle(playerNum2);     //获取下一玩家的最大棋型

            //冲4或活4以上棋型直接返回坐标
            if (p[2] > 32) return p;
            if (p2[2] > 32) return p2;

            if (p[2] == 32) return p;               //玩家达到活3返回坐标

            if (p2[2] == 32)                        //下一位玩家达到活3
            {
                A = think(playerNum, 32, playerNum2);
                if (A[0] != -1) return A;           //玩家有更大的棋型

                C = betterThree(p2[0], p2[1], playerNum, playerNum2);
                if (C[2] == 1) return C;            //返回更有益于玩家的位置
                else
                {                                   //进一步比较
                    p = p2;
                    goto last;
                }
            }

            if (p[2] == 31)
            {
                B = think(playerNum, 32, playerNum2);
                if (B[0] != -1) return B;
            }

            if (p[2] >= 21)
            {
                B = think(playerNum, 31, playerNum2);
                if (B[0] != -1) return B;
            }

            if (p[2] - 9 >= p2[2]) goto checkLive;       //玩家棋型更大
            else if (p[2] >= p2[2]) goto last;

            if (p2[2] > p[2] && p2[2] % 10 == 2)   //在后一位玩家的棋型更大且为活棋时
            {
                p = p2;
                goto last;
            }

            if (p2[2] - p[2] > 18 && p2[2] % 10 == 1)//在后一位玩家的棋型更大且为冲棋时
            {
                p = p2;
                goto last;
            }

        checkLive:
            if (p[2] % 10 == 1)                     //计算玩家的小一阶活棋
            {
                A = haveStyle(playerNum, p[2] - 9);
                if (A[0] != -1)
                {
                    p[0] = A[0];
                    p[1] = A[1];
                    p[2] -= 9;
                }
                else p = p2;                      //没有小一阶的活棋则堵对家棋子
            }

        last:
            sortNum = 0;//排序数
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] == 0 && style[p[3] - 1, i, j] == p[2])//确定为空位置且在该位置取得最大棋型
                    {
                        tmp = sortStyle(i, j, playerNum);
                        if (tmp > sortNum || (tmp == sortNum && rnd.Next(7) > 4))//若排序数较大,或排序数相同时随机选择是否替换
                        {
                            sortNum = tmp;          //记录最大排序数
                            p[0] = i;               //记录坐标
                            p[1] = j;
                        }
                    }
                }
            /**/
            return p;
        }


        public int[] getBetterSite1(int playerNum, int playerNum2)
        {
            int[] A = { Row / 2, Line / 2 }, B = { -1, -1 }, B2 = { -1, -1 }, C = { -1, -1 };
            int sortNum, tmp;
            Random rnd = new Random();

            if (num == 0)                           //没有棋子时,随机生成坐标
            {
                A[0] -= rnd.Next(Row / 3 - 2);
                A[1] -= rnd.Next(Line / 3 - 2);
                return A;
            }

            int[] p = getMaxStyle(playerNum);       //获取玩家自身的最大棋型
            int[] p2 = getMaxStyle(playerNum2);     //获取下一玩家的最大棋型

            //冲4或活4以上棋型直接返回坐标
            if (p[2] > 32) return p;
            if (p2[2] > 32) return p2;

            if (p[2] == 32) return p;               //玩家达到活3返回坐标

            if (p2[2] == 32)                        //下一位玩家达到活3
            {
                C = betterThree(p2[0], p2[1], playerNum, playerNum2);
                if (C[2] == 1) return C;            //返回更有益于玩家的位置
                else
                {                                   //进一步比较
                    p = p2;
                    goto last;
                }
            }
            
            if (p[2] - 9 >= p2[2]) goto checkLive;  //玩家棋型更大
            else if (p[2] >= p2[2]) goto last;

            if (p2[2] > p[2] && p2[2] % 10 == 2)    //在后一位玩家的棋型更大且为活棋时
            {
                p = p2;
                goto last;
            }

            if (p2[2] - p[2] > 18 && p2[2] % 10 == 1)//在后一位玩家的棋型更大且为冲棋时
            {
                p = p2;
                goto last;
            }

        checkLive:
            if (p[2] % 10 == 1)                     //计算玩家的小一阶活棋
            {
                A = haveStyle(playerNum, p[2] - 9);
                if (A[0] != -1)
                {
                    p[0] = A[0];
                    p[1] = A[1];
                    p[2] -= 9;
                }
                else p = p2;                      //没有小一阶的活棋则堵对家棋子
            }

        last:
            sortNum = 0;//排序数
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Line; j++)
                {
                    if (player[i, j] == 0 && style[p[3] - 1, i, j] == p[2])//确定为空位置且在该位置取得最大棋型
                    {
                        tmp = sortStyle(i, j, playerNum);
                        if (tmp > sortNum || (tmp == sortNum && rnd.Next(7) > 4))//若排序数较大,或排序数相同时随机选择是否替换
                        {
                            sortNum = tmp;          //记录最大排序数
                            p[0] = i;               //记录坐标
                            p[1] = j;
                        }
                    }
                }
            /**/
            return p;
        }
    }
}
