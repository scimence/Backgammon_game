using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Runtime.InteropServices;


namespace 五子棋单机版
{
    public partial class Form1 : Form
    {
        public  int M, N, border, dis, num;                 //行数、列数、边框距、棋盘格大小、当前已下棋子数、玩家总数
        private C_Num player;                               //用循环数标记玩家
        private QiPan qipan;                                //使用棋盘类进行棋盘中数据的记录
        private int[] win;                                  //标记依次获胜的各个玩家
        private bool[] quit;                                //标记各个玩家是否已经退出
        private bool[] auto;                                //标记个玩家是否已托管

        private Graphics g;
        private Pen pen1;
        private Image[] image = {五子棋单机版.Properties.Resources.标记 , 五子棋单机版.Properties.Resources.黑棋, 
                                    五子棋单机版.Properties.Resources.红棋,五子棋单机版.Properties.Resources.白棋,
                                    五子棋单机版.Properties.Resources.绿棋, 五子棋单机版.Properties.Resources.黄棋,
                                    五子棋单机版.Properties.Resources.紫棋,五子棋单机版.Properties.Resources.蓝棋};
        private string[] str = { "黑方", "红方", "白方", "绿方", "黄方", "紫方", "蓝方"};
        private SoundPlayer sound = new SoundPlayer(Properties.Resources.sound);
        private double s = 1;                               //棋盘放大的倍数
        private Image[] backImage = 
        {
            五子棋单机版.Properties.Resources._1,五子棋单机版.Properties.Resources._2,五子棋单机版.Properties.Resources._3,五子棋单机版.Properties.Resources._4,五子棋单机版.Properties.Resources._5,
            五子棋单机版.Properties.Resources._6,五子棋单机版.Properties.Resources._7,五子棋单机版.Properties.Resources._8,五子棋单机版.Properties.Resources._9,五子棋单机版.Properties.Resources._10,
            五子棋单机版.Properties.Resources._11,五子棋单机版.Properties.Resources._12,五子棋单机版.Properties.Resources._13,五子棋单机版.Properties.Resources._14,五子棋单机版.Properties.Resources._15,
            五子棋单机版.Properties.Resources._16,五子棋单机版.Properties.Resources._17
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetInfo();                                      //设置棋盘
            Tip();
        }

        
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int X, Y;

            //用鼠标单击事件获取落子的坐标
            X = (int)((e.Y - border + dis / 2) / dis);      //计算在棋盘中的行坐标
            Y = (int)((e.X - border + dis / 2) / dis);      //列坐标
            putChess(X, Y);                                 //在该坐标处落下棋子
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int X, Y;
            X = (int)((e.Y - border + dis / 2) / dis);      //计算在棋盘中的行坐标
            Y = (int)((e.X - border + dis / 2) / dis);      //列坐标
            label1.Text = "行:" + X.ToString() + "  列:" + Y.ToString();

            int i = player.getNext(1), j = player.getNext(2);
            label2.Text = str[i - 1] + "  " + qipan.ToString(qipan.getStyle(X, Y, i)) + "\n"
                + str[j - 1] + "  " + qipan.ToString(qipan.getStyle(X, Y, j)) + "\n"
                + "\n排序数 " + qipan.sortStyle(X, Y, i).ToString();// +"  " + qipan.doubleBigger(X, Y, i, 21, j, null).ToString() + " " + qipan.biggerThan(X, Y, i, 31)[0].ToString();
            //if (X == 6 && Y == 9) 
            //    label5.Text = qipan.doubleBiggerTwice(i)[0].ToString();
            //int[] t = qipan.doubleBiggerTwice(i);
            ////label5.Text = " " + (t / 10).ToString() + "  " + (t % 10).ToString();
            ////if (t != 0) qipan.doubleBiggerTwice(i);
            //if (t != null)
            //    label5.Text = t[0].ToString() + "  " + t[1].ToString();
            //else label5.Text = "";

            //for (int i = 1; i < num; i++)
            //    label2.Text += str[i - 1] + qipan.ToString(qipan.getStyle(X, Y, i)) + "\n";
            //label2.Text += "\n排序数 " + qipan.sortStyle(X, Y, player.getNext(1)).ToString();
            //label2.Text = "\n黑方" + qipan.ToString(qipan.getStyle(X, Y, 1)) + ", 白方" + qipan.ToString(qipan.getStyle(X, Y, 2))
            //    //+ "\n黑方最大棋子数：" + qipan.getPlayerNum(X, Y, 1) //+ "  最大连续棋子数" + qipan.getPlayerNum2(X, Y, 1)
            //    //+ "\n方向1：" + qipan.ourSite(X, Y, 1, 4, 1) + "   方向5：" + qipan.ourSite(X, Y, 5, 4, 1)
            //    + "\n排序数：" //+ qipan.sortStyle2(X, Y) + " ; " 
            //    + qipan.sortStyle(X, Y, player.getNext(1)).ToString();
            //    //+ "\nlive()=" + qipan.Live(X, Y, 2) +" haveDoubleLive()="+ qipan.haveDoubleLive(X,Y,2,0);
            
        }
        
        private void putChess(int x, int y)                 //玩家在棋盘上坐标(x,y)处落下棋子
        {
            int prePlayer;
            if (0 <= x && x < M && 0 <= y && y < N)         //下棋子的位置在棋盘之内时，在棋盘上绘制棋子
            {
                //测试区
                if (qipan.player[x,y] == 0)                 //如果该处无棋子
                {
                    if (!checkBox2.Checked) sound.Play();   //播放声音
                    qipan.put(x,y,player.getNext(1));       //下一个玩家落下棋子,在棋盘中记录该信息
                    drawpic(qipan.player[x, y], x, y);      //绘制该玩家对应的棋子
                    drawpic(0, x, y);                       //在棋子上绘制标记
                    if (qipan.num > 1)                      //重新绘制下的前一颗棋子，去除标记
                    {
                        prePlayer= qipan.player[qipan.X[qipan.num - 2], qipan.Y[qipan.num - 2]];
                        drawpic(prePlayer, qipan.X[qipan.num - 2], qipan.Y[qipan.num - 2]);
                    }

                    if (qipan.fiveNum(x, y))                //当其中一方获胜时
                    {
                        int playerN = qipan.player[x, y];
                        MessageBox.Show(str[playerN - 1] + "胜出");

                        win[++win[0]] = playerN;            //依次记录获胜的玩家信息
                        quit[playerN - 1] = true;           //获胜用户退出
                        saveQipan();                        //保存棋盘信息
                        
                        if (win[0] == win.Length - 1)
                        {
                            if (DialogResult.Yes == MessageBox.Show("再来一盘？", "信息", MessageBoxButtons.YesNo))
                                again();//再来一盘
                            else
                            {
                                button8_Click(null, null);
                            }
                        }
                    }
                    pass();                                 //下一位玩家下棋
                }
            }
        }

        private void drawpic(int pic, int x, int y)         //在棋盘上坐标(x,y)处绘制下标为pic的image图像
        {
            int X = y * dis + border - dis / 2,             //计算在棋盘上的像素横坐标
                Y = x * dis + border - dis / 2;             //纵坐标
            g.DrawImage(image[pic], X, Y, (int)(image[pic].Width * s), (int)(image[pic].Height * s));//按s倍大小绘制
            
        }

        private void drawQipan()                            //绘制棋盘
        {
            if (pen1 != null)
            {
                pen1.Width = 2;
                g.DrawRectangle(pen1, border, border, dis * N - dis, dis * M - dis);    //绘制外边框

                pen1.Width = 1;
                for (int i = 1; i <= M - 2; i++)                                         //绘制行
                    g.DrawLine(pen1, border, border + dis * i, border + dis * N - dis, border + dis * i);
                for (int i = 1; i <= N - 2; i++)                                         //绘制列
                    g.DrawLine(pen1, border + dis * i, border, border + dis * i, border + dis * M - dis);
            }
        }

        private void pass()                                 //让一子 或 交出下子权给下一位玩家
        {
            if (player != null)
            {
                player.next(1);                                 //下一位玩家
                Tip();                                          //提示落子
            }
        }

        private void Tip()                                  //给出下一位玩家的提示信息
        {
            int[] site;
            if (quit[player.getNext(1) - 1]) { pass(); return; }//该玩家已退出时
            
            if (auto[player.getNext(1) - 1])                //如果该玩家已托管则自动调用程序落棋
            {   //获取最优落棋点
                if (comboBox4.SelectedText.Trim().Equals("初级"))
                    site = qipan.getBetterSite1(player.getNext(1), player.getNext(2));
                else if (comboBox4.SelectedText.Trim().Equals("中级"))
                    site = qipan.getBetterSite2(player.getNext(1), player.getNext(2));
                else site = qipan.getBetterSite3(player.getNext(1), player.getNext(2));
                putChess(site[0], site[1]);
            }

            pictureBox2.Image = image[player.getNext(1)];   //给出下一位玩家下棋提示
            label3.Text = "等待" + str[player.getNext(1) - 1] + "落棋";

            if (qipan.num > 0)
            {
                label4.Text = "";
                for (int i = 0; i < num; i++)
                    label4.Text += str[i] + "最大棋型：" + qipan.ToString(qipan.getMaxStyle(i + 1)[2]) + "\n";//  下一步可达到\n" +qipan.ToString(qipan.checkMaxStyle(i + 1)[0]) + "\n";
                int[] //p = qipan.getMaxStyleAll(player.getNext(1)),
                    q = qipan.getMaxStyle(player.getNext(1));
                //label5.Text = //"所有最大棋型：" + str[p[3] - 1] + qipan.ToString(p[2]) + " 坐标(" + p[0].ToString() + "," + p[1].ToString() + ")"+ 
                //    "玩家棋型信息：\n" + q[2].ToString() + "  " + q[0].ToString() + "  " + q[1].ToString() + "  " + q[3].ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pass();
        }

        private void button4_Click(object sender, EventArgs e)//悔棋
        {
            int preX, preY, prePlayer;
            for (int i = 0; i < num;i++ )
                if (qipan.num > 0)
                {
                    preX = qipan.X[qipan.num - 1];                //获取最后一颗棋子的横坐标
                    preY = qipan.Y[qipan.num - 1];                //纵坐标
                    prePlayer = qipan.player[preX, preY];         //该棋子的所有者

                    if (qipan.fiveNum(preX, preY))
                    {
                        win[0]--;
                        quit[prePlayer - 1] = false;
                    }
                    Eraser(preX, preY, prePlayer);                //用背景图像擦除最后一颗棋子
                    qipan.retreat();                              //棋盘后退一步
                    player.pre(1);                                //退回前一位玩家
                    if (qipan.num > 0) drawpic(0, qipan.X[qipan.num - 1], qipan.Y[qipan.num - 1]);//在前一颗棋子上绘制标记
                }
            Tip();                                        //提示
        }

        private void Eraser(int x, int y, int player)         //使用背景图像重新绘制棋盘的指定区域，实现棋盘上棋子的擦除
        {
            Image image1 = pictureBox1.BackgroundImage, image2 = image[player];
            GraphicsUnit units = GraphicsUnit.Pixel;          //图像单元
            int X = y * dis + border - dis / 2,               //计算像素横坐标
                Y = x * dis + border - dis / 2;               //纵坐标

            //因为pictureBox1的背景图像是拉伸显示的，所以在指定区域用背景擦出棋子时要计算出该擦出区域在背景图像中的对应区域
            double x0 = (double)image1.Width / pictureBox1.Width,   //计算横坐标拉伸比
                   y0 = (double)image1.Height / pictureBox1.Height; //纵坐标拉伸比

            Rectangle desRect = new Rectangle(X, Y, (int)(image2.Width * s), (int)(image2.Height * s)); //棋盘中要擦除的位置，s为放大倍数
            Rectangle srcRect = new Rectangle((int)(X * x0), (int)(Y * y0), (int)(desRect.Width* x0), (int)(desRect.Height*y0));//计算在背景图像中的对应区域

            g.DrawImage(image1, desRect, srcRect, units);     //用背景图像重画区域rect1

            drawQipanLine(x, y);                              //重绘棋盘坐标(x, y)处周边的棋盘线
        }

        private void drawQipanLine(int x, int y)            //重绘棋盘坐标(x, y)处周边的棋盘线
        {
            int wid1=1,wid2=1;                              //分别标记水平线、垂直线的宽度

            int X = border + dis * y,                       //计算水平像素
                Y = border + dis * x;                       //垂直像素位置

            if (x == 0 || x == M - 1) wid1 = 2;             //棋盘线宽度控制
            if (y == 0 || y == N - 1) wid2 = 2;

            int x1, x2, y1, y2;                             //棋盘线范围控制
            y1 = (x == 0) ? Y : Y - dis / 2;                //0行处,垂直方向y1不可超过边界
            y2 = (x == M - 1) ? Y : Y + dis / 2;            //最后一行
            x1 = (y == 0) ? X : X - dis / 2;                //第一列
            x2 = (y == N - 1) ? X : X + dis / 2;            //最后一列

            pen1.Width = wid1;
            g.DrawLine(pen1, x1, Y, x2, Y);                 //水平棋盘线

            pen1.Width = wid2;
            g.DrawLine(pen1, X, y1, X, y2);                 //竖直棋盘线
        }


        private void button5_Click(object sender, EventArgs e)
        {
            again();
        }

        private void again()
        {
            g.DrawImage(pictureBox1.BackgroundImage, 0, 0, pictureBox1.Width, pictureBox1.Height);  //清空棋盘图形上的棋子,用背景图像重新绘制
            //pictureBox1.Image = pictureBox1.BackgroundImage;
            qipan.clear();                      //清空棋盘上的棋子
            drawQipan();                        //重绘制棋盘
            win[0] = 0;
            for (int i = 0; i < num; i++)
            {
                quit[i] = false;
                auto[i] = false;
            }
            button7.Enabled = true;            //托管可用
        }

        //设置按钮
        private void button6_Click(object sender, EventArgs e)
        {
            if (qipan.num > 0)  //棋盘中有棋子时不可改变行列数、玩家数
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                label16.Text = "玩家已落棋，行列数\n玩家数 不可再更改";
            }
            else
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                label16.Text = "";
            }

            if (radioButton3.Checked == true) comboBox5.Enabled = false;
            else comboBox5.Enabled = true;

            panel1.Visible = false;
            pictureBox1.Visible = false;
            tabControl1.Visible = true;
            resize();     //调整窗体大小
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int count=0;
            for (int i = 0; i < player.num; i++)
                if (auto[i]) count++;
            if (count == num - 1) button7.Enabled = false;

            auto[player.getNext(1) - 1] = true;     //当前的玩家托管
            Tip();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < player.num; i++)    //取消所有玩家的托管
                auto[i] = false;
            button7.Enabled = true;
        }


        private void refreshQipan()                  //刷新棋盘显示
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();                          //停止Timer1，只刷新一次

            if (qipan == null) return;
            drawQipan();
            if (qipan.num == 0) return;             //无棋子时返回
            int i;
            for (i = 0; i < qipan.num; i++)         //重绘所有棋子
                drawpic(qipan.player[qipan.X[i], qipan.Y[i]], qipan.X[i], qipan.Y[i]);
            drawpic(0, qipan.X[i - 1], qipan.Y[i - 1]); //在最后一颗棋子上绘制标记
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            refreshQipan();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            refreshQipan();
        }

        //调整窗体的大小, w和h为要设置的工作区宽和高
        private void resize()
        {
            if (pictureBox1.Visible)
            {
                pictureBox1.Left = 0;
                pictureBox1.Top = 0;
                Width = Width - ClientSize.Width + pictureBox1.Width;
                Height = Height - ClientSize.Height + pictureBox1.Height;
            }
            else
            {
                tabControl1.Left = 0;
                tabControl1.Top = 0;
                Width = Width - ClientSize.Width + tabControl1.Width;
                Height = Height - ClientSize.Height + tabControl1.Height;
            }

            panel1.Left = ClientSize.Width;
            if (pictureBox1.Visible && panel1.Visible)
            {
                Width += panel1.Width;
                if (panel1.Height > ClientSize.Height)
                {
                    Height = Height - ClientSize.Height + panel1.Height;
                    panel1.Top = 0;
                }
                else panel1.Top = (ClientSize.Height - panel1.Height) / 2;
            }
            //if (ClientSize.Width!= w)Width = w + Width - ClientSize.Width;//设置窗体内部工作区的宽度
            //if (ClientSize.Height != h) Height = h + Height - ClientSize.Height;
            ////CenterToScreen();                    //居中
        }

        //设置棋盘信息
        private void SetInfo()
        {
            dis = Convert.ToInt32(comboBox5.Text);          //棋盘格的大小
            if (dis < 6) dis = 5;
            border = dis * 3 / 4;                           //棋盘到窗体的边框距离

            s = (double)dis / Convert.ToInt32(comboBox5.Items[0].ToString());
            if (qipan == null || qipan.num == 0)            
            {
                M = Convert.ToInt32(comboBox1.Text.Trim());
                if (M < 6) M = 5;
                N = Convert.ToInt32(comboBox2.Text);            //棋盘大小为M*N
                if (N < 6) N = 5;
                num = Convert.ToInt32(comboBox3.Text);

                player = new C_Num(num);                        //初始化num个玩家
                qipan = new QiPan(M, N, num);                   //初始化一个M行N列的棋盘
                win = new int[num];
                quit = new bool[num];
                auto = new bool[num];
            }

            pictureBox1.Width = (N - 1) * dis + 2 * border; //根据行列数设置棋盘大小
            pictureBox1.Height = (M - 1) * dis + 2 * border;

            //string str = "jpg\\" + comboBox6.Text.Trim() + ".jpg";
            //if (System.IO.File.Exists(str))
            //    pictureBox1.BackgroundImage = Image.FromFile(str);
            pictureBox1.BackgroundImage = backImage[Convert.ToInt32(comboBox6.Text.Trim())-1];

            g = pictureBox1.CreateGraphics();               //在对象图片框中创建一个Graphics对象用于绘图
            pen1 = new Pen(label12.BackColor, 2);           
            

            if (checkBox1.Checked) panel1.Visible = false;
            else panel1.Visible = true ;

            if (checkBox3.Checked) TopMost = true;          //最前端显示棋盘
            else TopMost = false;

            if (radioButton1.Checked == true) FormBorderStyle = FormBorderStyle.None;        //无边框
            else if (radioButton3.Checked == true) FormBorderStyle = FormBorderStyle.Fixed3D; //固定大小

            if (FormBorderStyle == FormBorderStyle.None) 退出ToolStripMenuItem.Visible = true;
            else 退出ToolStripMenuItem.Visible = false;

            pictureBox1.Visible = true;
            resize();
        }

        //应用设置按钮
        private void button9_Click(object sender, EventArgs e)
        {
            SetInfo();
            tabControl1.Visible = false;
            pictureBox1.Visible = true;                     //显示棋盘
        }

        //仅限数字输入
        private void only_number_Input(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) >= 48 && Convert.ToInt32(e.KeyChar) < 58 //数字
                || Convert.ToInt32(e.KeyChar) == 8)      //包括删除键
            //|| (Convert.ToInt32(e.KeyChar) == 46))     //包括.
            { }
            else e.Handled = true;                       //标记为事件已处理
        }

        //禁止输入
        private void fobidInput(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        //棋盘颜色设置
        private void label12_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.ShowHelp = false;
            MyDialog.Color = label12.BackColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
                label12.BackColor = MyDialog.Color;
        }

        ////改变窗体的大小来改变棋盘
        //bool SizeChanging = false;
        //private void Form1_SizeChanged(object sender, EventArgs e)
        //{
        //    if (SizeChanging) return;
        //    SizeChanging = true;

        //    if (pictureBox1 == null || pictureBox1.Width == 0 || pictureBox1.Height == 0) return;

        //    double h = (double)ClientSize.Height / pictureBox1.Height,
        //    w = (double)ClientSize.Width / pictureBox1.Width,
        //    s = h <= w ? h : w;                                 //较小的长宽比

        //    if (s != 1 && pictureBox1.Visible == true)
        //    {
        //        dis = (int)(dis * s);
        //        border = dis * 3 / 4;                           //棋盘到窗体的边框距离
        //        this.s = (double)dis / Convert.ToInt32(comboBox5.Items[0].ToString());
        //        comboBox5.Text = dis.ToString();

        //        pictureBox1.Width = (N - 1) * dis + 2 * border; //根据行列数设置棋盘大小
        //        pictureBox1.Height = (M - 1) * dis + 2 * border;

        //        g = pictureBox1.CreateGraphics();
        //        resize();
        //    }
        //}

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true) comboBox5.Enabled = false;
            else comboBox5.Enabled = true;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //记录当前棋盘中的所有数据信息,包括设置和棋子信息
        private string setingStr()
        {
            //棋盘的设置信息
            string str = "";
            str += " " + comboBox1.Text.Trim(); //行数
            str += " " + comboBox2.Text.Trim(); //列数
            str += " " + comboBox3.Text.Trim(); //玩家数
            str += " " + label12.BackColor.R + " " + label12.BackColor.G + " " + label12.BackColor.B;//颜色
            str += " " + comboBox6.Text.Trim(); //背景
            str += " " + comboBox5.Text.Trim(); //棋盘格大小

            if (checkBox2.Checked) str += " 1"; //静音
            else str += " 0";
            if (checkBox3.Checked) str += " 1"; //前端显示
            else str += " 0";
            if (checkBox1.Checked) str += " 1"; //隐藏侧边栏
            else str += " 0";

            if (radioButton1.Checked) str += " 1";      //窗体样式设置
            else if (radioButton2.Checked) str += " 2";
            else str += " 3";

            return str;
        }
        private string styleStr()
        {
            int x, y;
            string str = "";
            //棋盘的棋子信息
            for(int i=0; i<qipan.num; i++)
            {
                x = qipan.X[i];
                y = qipan.Y[i];
                str += " " + x.ToString() + " " + y.ToString() + " " + qipan.player[x, y];
            }
            return str;
        }
        private string controlStr()
        {
            //棋型控制信息
            string str = "";
            for (int i = 0; i < num; i++)
            {
                str += " " + win[i].ToString();
                if (quit[i]) str += " 1";
                else str += " 0";
                if (auto[i]) str += " 1";
                else str += " 0";
            }
            return str;
        }
        //保存棋盘信息
        private void saveQipan()
        {
            if (qipan.num < 3 * num) return;                                                        //各玩家所下的棋子数目平均小于3则不予保存
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"FiveRecord\";          //设置当前目录
            if (!System.IO.Directory.Exists(CurDir)) System.IO.Directory.CreateDirectory(CurDir);   //该路径不存在时，在当前文件目录下创建文件夹FiveRecord

            try
            {   //不存在该文件时先创建
                System.IO.StreamWriter file1 = new System.IO.StreamWriter(CurDir + "五子棋信息记录." + DateTime.Now.Date.ToString("yyyy_MM_dd"), true);

                file1.WriteLine("Time: " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8)+"   ");
                file1.WriteLine(setingStr());   //设置信息
                file1.WriteLine(styleStr());    //棋型信息
                file1.WriteLine(controlStr());  //棋盘控制信息
                file1.Close();                                          //关闭文件
                file1.Dispose();                                        //释放对象
            }
            catch (Exception)
            {
                MessageBox.Show("保存棋盘数据失败");
                return;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveQipan();    //保存棋盘信息
            if (num > 0 && win[0] < win.Length - 1 && MessageBox.Show("确定退出游戏？", "退出提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                e.Cancel = true;                                                  //当选择不退出时，取消退出操作
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"FiveRecord\";
            string str = CurDir + "五子棋信息记录." + monthCalendar1.SelectionStart.ToString("yyyy_MM_dd");

            if (!System.IO.File.Exists(str))
            {
                listView1.Items.Clear();
                return;                                                         //如果文件不存在则退出
            }
            System.IO.StreamReader file1 = new System.IO.StreamReader(str);     //读取文件中的数据

            int num = 1;
            string strLine = file1.ReadLine();                                  //逐行读取文件中的数据
            ListViewItem tmp;
            listView1.Items.Clear();
            while (strLine != null)
            {
                if (strLine.Length > 4 && strLine.Substring(0, 4).Equals("Time"))           //读到时间信息时添加到List中
                {
                    tmp = listView1.Items.Add(num.ToString());
                    tmp.SubItems.Add(strLine.Substring(6, 10));
                    num++;
                    //listView1.Items.Add(num.ToString());                                  //添加新的列表项
                    //listView1.Items[num - 1].SubItems.Add(strLine.Substring(4, 12));      //添加时间到列表中
                }
                strLine = file1.ReadLine();
            }
            file1.Close();
            file1.Dispose();

            if (num > 1) button1.Visible = true;
            else button1.Visible = false;
        }

        private void setting(string str)
        {
            //棋盘的设置信息
            if (str.Trim().Equals("")) return;

            string[] set = str.Trim().Split(' ');
            comboBox1.Text = set[0]; //行数
            comboBox2.Text = set[1]; //列数
            comboBox3.Text = set[2]; //玩家数

            Color color1 = Color.FromArgb(Convert.ToInt32(set[3]), Convert.ToInt32(set[4]), Convert.ToInt32(set[5]));
            label12.BackColor = color1;//设置背景色

            comboBox6.Text = set[6]; //背景
            comboBox5.Text = set[7]; //棋盘格大小

            if(set[8].Equals("1"))checkBox2.Checked = true;//静音
            else checkBox2.Checked = false;
            if (set[9].Equals("1")) checkBox3.Checked = true;//前端显示
            else checkBox3.Checked = false;
            if (set[10].Equals("1")) checkBox1.Checked = true;//隐藏侧边栏
            else checkBox1.Checked = false;

            if (set[11].Equals("1")) radioButton1.Checked = true;//窗体样式设置
            else if (set[11].Equals("2")) radioButton2.Checked = true;
            else radioButton3.Checked = true;

            SetInfo();
        }
        private void styleSetting(string str)
        {
            if (qipan == null) return;
            if (str.Trim().Equals("")) return;

            int x, y, player;
            string[] tmp = str.Trim().Split(' ');

            //棋盘的棋子信息
            for (int i = 0; i < tmp.Length; i+=3)
            {
                x = Convert.ToInt32(tmp[i]);
                y = Convert.ToInt32(tmp[i+1]);
                player = Convert.ToInt32(tmp[i + 2]);
                qipan.put(x, y, player);
            }
        }
        private void controlSetting(string str)
        {
            if (qipan == null) return;
            //棋型控制信息
            string[] tmp = str.Trim().Split(' ');
            for (int i = 0; i < num; i++)
            {
                win[i] = Convert.ToInt32(tmp[i]);
                //if (tmp[i + 1].Equals("1")) quit[i] = true;
                //else quit[i] = false;
                if (tmp[i + 2].Equals("1")) auto[i] = true;
                else auto[i] = false;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string time ="";
            if (listView1.SelectedItems.Count > 0)
                time = listView1.SelectedItems[0].SubItems[1].Text.Trim();
            saveQipan();                                                        //保存棋盘信息

            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"FiveRecord\";
            string str = CurDir + "五子棋信息记录." + monthCalendar1.SelectionStart.ToString("yyyy_MM_dd");

            if (!System.IO.File.Exists(str)) return;                            //如果文件不存在则退出
            System.IO.StreamReader file1 = new System.IO.StreamReader(str);     //读取文件中的数据

            string strLine = file1.ReadLine();                                  //逐行读取文件中的数据
            while (strLine != null)
            {
                if (strLine.Length > 4 && strLine.Substring(0, 4).Equals("Time"))//读到时间信息时添加到List中
                {
                    if(strLine.Substring(6, 10).Trim().Equals(time))
                    {
                        qipan.num = 0;                                          //
                        tabControl1.Visible = false;
                        setting(file1.ReadLine());
                        styleSetting(file1.ReadLine());
                        controlSetting(file1.ReadLine());
                        break;
                    }
                }
                strLine = file1.ReadLine();
            }
            file1.Close();
            file1.Dispose();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string str = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"FiveRecord\";
            string str = CurDir + "五子棋信息记录." + monthCalendar1.SelectionStart.ToString("yyyy_MM_dd");

            if (!System.IO.File.Exists(str)) return;
            else System.IO.File.Delete(str);    //删除文件
            monthCalendar1_DateSelected(null, null);
            button1.Visible = false;            //隐藏
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked == false) return;
            //在边框范围内时，进行窗体移动
            if (e.Y < border - 5 || e.X < border - 5 || e.Y > pictureBox1.Height-border + 5 || e.X > pictureBox1.Width-border  - + 5)
            {
                //常量
                int WM_SYSCOMMAND = 0x0112;

                //窗体移动
                int SC_MOVE = 0xF010;
                int HTCAPTION = 0x0002;

                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            }
        }
        ////常量
        //int WM_SYSCOMMAND = 0x0112;
        ////改变窗体大小
        //int WMSZ_LEFT = 0xF001;
        //int WMSZ_RIGHT = 0xF002;
        //int WMSZ_TOP = 0xF003;
        //int WMSZ_TOPLEFT = 0xF004;
        //int WMSZ_TOPRIGHT = 0xF005;
        //int WMSZ_BOTTOM = 0xF006;
        //int WMSZ_BOTTOMLEFT = 0xF007;
        //int WMSZ_BOTTOMRIGHT = 0xF008;
        //ReleaseCapture();
        //SendMessage(this.Handle, WM_SYSCOMMAND, WMSZ_BOTTOM, 0);

        //SendMessage(this.Handle, WM_SYSCOMMAND, WMSZ_TOP, 0);

    }
}

