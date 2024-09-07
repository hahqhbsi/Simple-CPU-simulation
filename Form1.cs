using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 计算机原理
{

    public partial class Form1 : Form
    {
        static int[] R = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //寄存器值数组 
        static int[] NC = new int[2] { 0, 0 };//内存
        static int i = 0, ja = 30;           //PC指令顺序数
        static string[] orderSet = new string[] { "add", "sub", "mul", "INC", "DEC", "JMP", "JC", "mov", "ldi", "ld", "nop", "st" };       //指令集字符串数组
        StreamWriter sw = new StreamWriter("输出文件.txt");  //输出文件流
                                                         //D:\\desktop\\
        public Form1()
        {
            InitializeComponent();
        }
        private void timer1_Tick(object sender, EventArgs e)    //自律执行过程
        {
            if (i < richTextBox_order.Lines.Length)
            {
                richTextBox_order.SelectionBackColor = Color.White;
                richTextBox_order.Select(richTextBox_order.GetFirstCharIndexFromLine(i), richTextBox_order.Lines[i].Length);
                richTextBox_order.SelectionBackColor = Color.LightSteelBlue;
                select(richTextBox_order.Lines[i]);
            }
            else
            {
                timer1.Enabled = false;
                MessageBox.Show("指令执行完毕！");
                saveNUM();
                sw.Close();
            }
        }
        private void btn_read_Click(object sender, EventArgs e)  //读文件指令,选择文件
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(Dialog.FileName, Encoding.Default);
                richTextBox_order.Text = sr.ReadToEnd();//一次性返回整个文件
                sr.Close();//读文件
            }
            R0.Text = "00000000";//初始化
            R1.Text = "00000000";
            R2.Text = "00000000";
            R3.Text = "00000000";
            R4.Text = "00000000";
            R5.Text = "00000000";
            R6.Text = "00000000";
            R7.Text = "00000000";
            text00B1.Text = "00000000";
            text00B2.Text = "00000000";
            PC.Text = "0000000000000000";
            BUS.Text = "00000000";
            IR.Text = "00000000";
            MAR.Text = "00000000";
            MDR.Text = "00000000";
            DMR.Text = "0000000000000000"; 
            PSW.Text = "00000000";
            V.Text = "0";
            N.Text = "0";
            Z.Text = "0";
            C.Text = "0";
            code.Items.Clear();
        }
        private void btn_singleexe_Click(object sender, EventArgs e)  //执行一次
        {
            if (i < richTextBox_order.Lines.Length)
            {
                richTextBox_order.SelectionBackColor = Color.White;
                richTextBox_order.Select(richTextBox_order.GetFirstCharIndexFromLine(i), richTextBox_order.Lines[i].Length);
                richTextBox_order.SelectionBackColor = Color.LightSteelBlue;
                select(richTextBox_order.Lines[i]);//是个函数
            }
            else
            {
                MessageBox.Show("指令执行完毕！\n请记录好实验数据后退出实验", "提示");
                saveNUM();//
                sw.Close();//sw是输出文件流
            }
        }
        private void btn_exit_Click(object sender, EventArgs e)//退出程序
        {
            Application.Exit();
        }
        private void btn_auto_Click(object sender, EventArgs e)//自律执行
        {
            timer1.Enabled = true;
        }
        private void btn_stop_Click(object sender, EventArgs e)//暂停自律
        {
            timer1.Enabled = false;
        }
        public void select(string s)
        {
            switch (find_select(s))
            {
                case 0:
                    ADD(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 1:
                    SUB(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 2:
                    MUL(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 3:
                    INC(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 4:
                    DEC(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 5:
                    JMP(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 6:
                    JC(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 7:
                    MOV(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 8:
                    LDI(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 9:
                    LD(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 10:
                    NOP(s);
                    ja = ja + 2;
                    i++;
                    break;
                case 11:

                    ST(s);
                    ja = ja + 2;
                    i++;
                    break;
            }
            chagejcq_num();
            saveMAIN(i);
        }
        private string pswNUM(int x, int y)    //检查状态寄存器
        {
            string s1, s2, psw = "   ";
            s1 = Convert.ToString(x, 2).PadLeft(8, '0');
            s2 = Convert.ToString(y + x, 2).PadLeft(8, '0');
            s1 = s1.Substring(s1.Length - 8, 8);
            s2 = s2.Substring(s2.Length - 8, 8);
            if (x + y < 0)
            {
                psw += "N, ";
                N.Text = "1";
            }//负数标志位：若运算结果是负数，则为1；否则，为0
            else N.Text = "0";

            if (x + y == 0)
            {
                psw += "0000" + "";
                Z.Text = "1";
            }//0标志位：若运算结果是0，则为1；否则，为0
            else Z.Text = "0";
            if (x + y > 0 && s2.Length > 8) psw += "C, ";
            if (x + y < 0 && s2.Length > 8) psw += "V, ";
            if (x + y < 0 && s2.Length <= 8 || x + y >= 0 && s2.Length > 8) psw += "S, ";
            int a;
            a = Convert.ToInt32(N.Text, 16) + Convert.ToInt32(V.Text, 16);
            if (a == 1)
            {
             
            }
            psw = psw.Remove(psw.Length - 2);
            return psw;
        }
        private void saveNUM()     //保存寄存器值
        {
            string s = "", s1;
            s += "数据存储器/寄存器组的值为:\n";
            for (int j = 0; j < 8; j++)
            {

                s1 = R[j].ToString("X").PadLeft(2, '0');
                s += "R" + j + ": " + s1.Substring(s1.Length - 2, 2) + "\n";
            }
            for (int j = 0; j < 2; j++)
            {
                s1 = NC[j].ToString("X").PadLeft(2, '0');
                int we = j + 1;
                s += "00B" + we + ": " + s1.Substring(s1.Length - 2, 2) + "\n";
            }
            sw.Write(s);
        }
        private int find_select(string s)  //解析指令，找到指令对用的函数代表数字
        {
            string order = "";
            for (int j = 0; j < s.Length; j++)
            {
                if (s[j] != ' ')
                {
                    order += s[j];
                }
                else
                    break;
            }
            for (int j = 0; j < 12; j++)
            {
                if (orderSet[j] == order)//指令集字符串数组,对上那个用那个
                {
                    return j;
                }
            }
            return -1;
        }
        private void saveMAIN(int x)       //保存每个周期主要寄存器值
        {
            string s = "";
            s += "第" + Convert.ToString(x) + "个周期:\n";
            s += "PC: " + PC.Text + "\n";
            s += "DMR: " + DMR.Text + "\n\n";
            s += "IR: " + IR.Text + "\n";
            s += "MAR: " + MAR.Text + "\n";
            s += "MDR: " + MDR.Text + "\n";
            s += "SR: " + PSW.Text + "\n";
            s += "BUS: " + BUS.Text + "\n\n";
            s +="R0:"+ Convert.ToString(R[0] , 16).PadLeft(2, '0') + "\n";
            s += "R1:" + Convert.ToString(R[1], 16).PadLeft(2, '0') + "\n";
            s += "R2:" + Convert.ToString(R[2], 16).PadLeft(2, '0') + "\n";
            s += "R3:" + Convert.ToString(R[3], 16).PadLeft(2, '0') + "\n";
            s += "R4:" + Convert.ToString(R[4], 16).PadLeft(2, '0') + "\n";
            s += "R5:" + Convert.ToString(R[5], 16).PadLeft(2, '0') + "\n";
            s += "R6:" + Convert.ToString(R[6], 16).PadLeft(2, '0') + "\n";
            s += "R7:" + Convert.ToString(R[7], 16).PadLeft(2, '0') + "\n";
            s += "00B1:" + Convert.ToString(NC[0], 16).PadLeft(2, '0') + "\n";
            s += "00B2:" + Convert.ToString(NC[1], 16).PadLeft(2, '0') + "\n";
            int pcValue = Convert.ToInt32(PC.Text, 2); // 将二进制字符串转换为十进制整数
            string hexString = pcValue.ToString("X").PadLeft(4, '0'); // 将十进制整数转换为十六进制字符串并填充到8位
            s += "pc:" + hexString + "\n\n";

            sw.Write(s);
        }
        private void chagejcq_num()         //修改总过程图中寄存器值
        {
            textBox_BUS.Text = "   " + BUS.Text;
            textBox_IR.Text = IR.Text;
            textBox_MAR.Text = "   " + MAR.Text;
            textBox_MDR.Text = "   " + MDR.Text;
            textBox_PC.Text = PC.Text;
        }
        private void change(int RX, string s)    //修改主窗体寄存器值，运算结果
        {
            string ss = Convert.ToString(RX, 2).PadLeft(8, '0');
            if (s == "R0") R0.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R1") R1.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R2") R2.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R3") R3.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R4") R4.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R5") R5.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R6") R6.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R7") R7.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "PC") PC.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "NC1") text00B2.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "NC0") text00B1.Text = ss.Substring(ss.Length - 8, 8);
  
        }
        /* private string pswmul(int x, int y)             //mul指令状态寄存器特判
         {
             string s1, s2, psw = "   ";
             s1 = Convert.ToString(x, 2).PadLeft(8, '0');
             s2 = Convert.ToString(y * x, 2).PadLeft(8, '0');
             s1 = s1.Substring(s1.Length - 8, 8);
             s2 = s2.Substring(s2.Length - 8, 8);
             if (x + y < 0)
             {
                 psw += "N, ";
                 N.Text = "1";
             }//负数标志位：若运算结果是负数，则为1；否则，为0
             else N.Text = "0";

             if (x + y == 0)
             {
                 psw += "000" + "";
                 Z.Text = "1";
             }//0标志位：若运算结果是0，则为1；否则，为0
             else Z.Text = "0";
             if (x + y > 0 && s2.Length > 8) psw += "C, ";
             if (x + y < 0 && s2.Length > 8) psw += "V, ";
             //if (x + y < 0 && s2.Length <= 8 || x + y >= 0 && s2.Length > 8) psw += "S, ";
             psw = psw.Remove(psw.Length - 2);
             return psw;
         }*/
        private string srPro(int x, int y)             //检查状态寄存器,回头改成分开的位数
        {
            string s1, s2, psw = "   ";
            s1 = Convert.ToString(x, 2).PadLeft(8, '0');
            s2 = Convert.ToString(y + x, 2).PadLeft(8, '0');
            s1 = s1.Substring(s1.Length - 8, 8);
            s2 = s2.Substring(s2.Length - 8, 8);
            if (x + y < 0) { psw += "N, "; N.Text = "1"; }
            else N.Text = "0";
            if (x + y == 0) { psw += "Z, "; Z.Text = "1"; }
            else Z.Text = "0";
            if (x + y > 0 && s2.Length > 8) psw += "C, ";
            if (x + y < 0 && s2.Length > 8) psw += "V, ";
            if (x + y < 0 && s2.Length <= 8 || x + y >= 0 && s2.Length > 8) psw += "S, ";
            psw = psw.Remove(psw.Length - 2);
            return psw;
        }
        private void ADD(string s)     //执行ADD指令
        {
            int j = 4, t = 0, temp = 0, num, num2;
            string s1, s2, jqm, czs, jcq, psw, dzm, ac, md, news, news2;
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');

            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                    s1 += s[j];
                else
                    break;
            }


            if (s[j + 1] == '@')
            {

                s2 = s.Substring(j + 3, 2);
                num = R[s2[1] - '0'];
                if (num == 177)
                {
                    num2 = 7;
                    R[num2] = R[num2] + 1;
                }
                else if (num == 178)
                {
                    num2 = 7;
                    R[num2] = R[num2] + 1;
                }
                else num2 = R[num];
                news = "R" + num;
                R[s1[1] - '0'] = R[s1[1] - '0'] + R[num2 - 1];
                news2 = "R" + num2;
                change(R[s1[1] - '0'], news);
                change_add(R[s2[1] - '0'] - 1, s2);
                psw = srPro(R[s1[1] - '0'], R[s2[1] - '0']);
                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                if (s2[1] - '0' > 7) jqm = Convert.ToString(dzm.Substring(4) + "00" + czs + "000" + jcq);
                else jqm = Convert.ToString("0001000" + jcq + "000" + czs);
                md = ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(8, '0');
                ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;//psw;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = md.Substring(md.Length - 8, 8);
                DMR.Text = md.PadLeft(16, '0');

            }
            else
            {
                s2 = s.Substring(j + 2);
                psw = srPro(R[s1[1] - '0'], R[s2[1] - '0']);
                R[s1[1] - '0'] += R[s2[1] - '0'];
                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                if (s2[1] - '0' > 7) jqm = Convert.ToString(dzm.Substring(4) + "00" + czs + "000" + jcq);
                else jqm = Convert.ToString("0001000" + jcq + "000" + czs);
                md = ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(8, '0');
                ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                PSW.Text = psw;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = md.Substring(md.Length - 8, 8);
                DMR.Text = md.PadLeft(16, '0');
                change(R[s1[1] - '0'], s1);
            }


        }//不能识别@呢
        private void SUB(string s)    //执行SUB指令okokokok
        {
            int j = 4, temp = 0, t = 0, num;
            string s1, s2, jqm, czs, jcq, psw, dzm, ac, md, news;
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            s1 = s.Substring(j + 1, 2);
            if (s[j] == '(')
            {

                s2 = s.Substring(j + 5, 2);
                num = R[s1[1] - '0'];
                if (num == 177)
                {
                    NC[0] = NC[0] - R[s2[1] - '0'];
                    news = "NC2";
                    change(NC[1], news);
                }
                else if (num == 178)//00B2
                {

                    NC[1] = NC[1] - R[s2[1] - '0'];
                    news = "NC1";
                    change(NC[1], news);
                }
                //  R[s1[1] - '0'] -= R[s2[1] - '0'];
                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                jqm = Convert.ToString("0010000" + jcq + "000"+czs);
                ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                md = ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = md.Substring(md.Length - 8, 8);
                PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
                DMR.Text = md;
            }
            else
            {
                s2 = s.Substring(j + 2);
                
                R[s1[1] - '0'] -= R[s2[1] - '0'];
                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(4, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(4, '0');
                jqm = Convert.ToString("00001000" + jcq + czs);
                ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                md = ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = md.Substring(md.Length - 8, 8);
                PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
                DMR.Text = md;
                change(R[s1[1] - '0'], s1);
            }
            //ac.Substring(ac.Length - 16, 16);

        }
        private void MUL(string s)     //执行mul指令，第二个间隔不用改吗
        {
            int j = 4;
            string s1, s2, jqm, czs, jcq, psw, dzm, t = "0", ac, md;
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                    s1 += s[j];
                else
                    break;
            }
            s2 = s.Substring(j + 2);
            //  psw = pswmul(R[s1[1] - '0'], R[s2[1] - '0']);

            R[s1[1] - '0'] *= R[s2[0] - '0'];
            czs = Convert.ToString(s2[0] - '0', 2).PadLeft(3, '0');
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            jqm = Convert.ToString("0011000" + jcq + "000" + czs);
            md = ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(8, '0');
            ac = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(16, '0');
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = dzm;
            IR.Text = jqm;
            PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
            MAR.Text = czs.PadLeft(8, '0');
            MDR.Text = md.Substring(md.Length - 8, 8);
            DMR.Text = md.PadLeft(16, '0');
            change(R[s1[1] - '0'], s1);

        }
        private void NOP(string s) //执行nop指令
        {
            string jqm, dzm;
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            jqm = Convert.ToString("0000" + "00000000" + "0000");
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = "00000000";
            IR.Text = "00000000";
            MAR.Text = "00000000";
            MDR.Text = "00000000";
            DMR.Text = "0000000000000000";

        }
        private void MOV(string s)   //执行mov指令，数据传送
        {
            int j = 4;
            string s1, s2, jqm, czs, jcq, dzm, t = "0", de;
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                {
                    s1 += s[j];

                }
                else
                    break;
            }
            if (s[j - 5] == '(' && s[j - 1] == '+')
            {
                de = s1 = s1.Substring(1, 2);
                s2 = s.Substring(j + 1, 2);
                int numm = R[s1[1] - '0'];
                R[numm] = R[s2[1] - '0'];
                R[s1[1] - '0'] = R[s1[1] - '0'] + 1;

                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
                s1 = "R" + numm;
                jcq = Convert.ToString(de[1] - '0', 2).PadLeft(3, '0');
                jqm = Convert.ToString("1010000" + jcq + "000" + czs);
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = czs.PadLeft(8, '0');
                DMR.Text  = czs.PadLeft(16, '0');
                change(R[s1[1] - '0'], s1);
                change_add(R[de[1] - '0']-1, de);


            }
            else
            {
                s2 = s.Substring(j + 2);
                R[s1[1] - '0'] = R[s2[1] - '0'];
                czs = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                jqm = Convert.ToString("1010000" + jcq + "000" + czs);
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs.PadLeft(8, '0');
                MDR.Text = czs.PadLeft(8, '0');
               
                DMR.Text =  czs.PadLeft(16, '0');
                change(R[s1[1] - '0'], s1);

            }

        }
        private void change_add(int RX, string s)//自增用
        {
            string ss = Convert.ToString(RX + 1, 2).PadLeft(8, '0');
            if (s == "R0") R0.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R1") R1.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R2") R2.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R3") R3.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R4") R4.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R5") R5.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R6") R6.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "R7") R7.Text = ss.Substring(ss.Length - 8, 8);
            else if (s == "PC") PC.Text = ss.Substring(ss.Length - 8, 8);
        }
        private void LDI(string s)    //执行ldi指令  完成任务
        {
            int j = 4;
            string s1, s2, jqm, czs, jcq, dzm, jcq2, t = "0";
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                {
                    s1 += s[j];
                }
                else
                    break;
            }
            s2 = s.Substring(j + 3);
            R[s1[1] - '0'] = Convert.ToInt32(s2, 16);
            czs = Convert.ToString(Convert.ToInt32(s2, 16), 2).PadLeft(8, '0');
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            string czs1 = czs.Substring(0, 4);
            string czs2 = czs.Substring(4, 4);
            jqm = Convert.ToString("1110" + czs + "0" + jcq);
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = dzm;
            IR.Text = jqm;
            MAR.Text = czs;
            MDR.Text = czs;
            DMR.Text = Convert.ToString(Convert.ToInt32(s2, 16), 2).PadLeft(16, '0');
            if (R[s1[1] - '0'] < 0) t = "1";
            PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;// t + Convert.ToString(R[s1[1] - '0'], 2).PadLeft(15, '0');
            change(R[s1[1] - '0'], s1);
        }
        private void JC(string s)    //执行jc
        {
            int j = 3, count = 0, t = 0;
            string s1, s2, jqm, czs, jcq, psw, dzm, ac, md, pc,dzmpc;
         
            dzmpc = Convert.ToString(ja + 2, 2).PadLeft(16, '0');
            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                    s1 += s[j];
                else
                    break;
            }
            s2 = s.Substring(j + 1);
            t = R[s1[1] - '0'];
            dzm = Convert.ToString((t), 2).PadLeft(8, '0');
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            //PC.Text = dzmpc;
            if (s2 == "N" || s2 == "Z")
            {
                if (N.Text == "1")
                {
                   // ja = ja + 2 - 2 * count;
                    jqm = Convert.ToString("1000" + "000" + jcq + "****00");
                    code.Items.Add(jqm);
                    PC.Text = dzmpc;// Convert.ToString(ja + 2-2*count, 2).PadLeft(16, '0');
                    pc = "PC";
                   // change(R[s1[1] - '0'], pc);
                    BUS.Text = dzm;
                    IR.Text = jqm;
                    MAR.Text = "00000000";
                    MDR.Text = "00000000";
                  DMR.Text = "0000000000000000";
                    i = R[s1[1] - '0'] / 2 - 17;
                }
                if (Z.Text == "1")
                {
                    //ja = ja + 2 - 2 * count;
                    jqm = Convert.ToString("1000" + "000" + jcq + "****01");
                    code.Items.Add(jqm);
                    //dzm = "R" +t;
                    PC.Text = Convert.ToString(ja + 2 - 2 * count, 2).PadLeft(16, '0');
                    pc = "PC";
                   // change(R[s1[1] - '0'], pc);
                    BUS.Text = dzm;
                    IR.Text = jqm;
                    MAR.Text = "00000000";
                    MDR.Text = "00000000";
                    DMR.Text = "0000000000000000";
                    i = R[s1[1] - '0'] / 2 - 17;
                    ja = 46;
                }
                if (s2 == "N" && N.Text != "1")
                {
                    jqm = Convert.ToString("1000" + "000" + jcq + "****00");
                    code.Items.Add(jqm);
                    PC.Text = dzmpc;
                    BUS.Text = dzm;
                    IR.Text = jqm;
                    MAR.Text = "00000000";
                    MDR.Text = "00000000";
                    DMR.Text = "0000000000000000";
                    count++;
                    // i = t;
                }
                if (s2 == "Z" && Z.Text != "1")
                {
                    jqm = Convert.ToString("1000" + "000" + jcq + "****01");
                    code.Items.Add(jqm);
                    PC.Text = dzmpc;
                    BUS.Text = dzm;
                    IR.Text = jqm;
                    MAR.Text = "00000000";
                    MDR.Text = "00000000";
                    DMR.Text = "0000000000000000";
                    count++;
                    // i = t;
                }
            }
            /*else if (s2 == "C")
            {
                jqm = Convert.ToString("1000" + "000" + jcq + "CC1000");
                code.Items.Add(jqm);
               // PC.Text = dzmpc;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = "00000000";
                MDR.Text = "00000000";
                DMR.Text = "0000000000000000";
                // i = t;
            }
            else if (s2 == "V")
            {
                jqm = Convert.ToString("1000" + "000" + jcq + "VV1100");
                code.Items.Add(jqm);
              //  PC.Text = dzmpc;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = "00000000";
                MDR.Text = "00000000";
                DMR.Text = "0000000000000000";
                // i = t;
            }*/
          

        }
        private void LD(string s)   //执行ld指令
        {
            int j = 3;
            string s1, s2, jqm, czs, jcq, dzm, t = "0";
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            for (; j < s.Length; j++)
            {
                if (s[j] != ',')
                    s1 += s[j];
                else
                    break;
            }
            if (s[j + 1] == 'X')
            {
                if (R7.Text == "10110001")//00B1
                {
                    R[s1[1] - '0'] = NC[0];
                }
                else if (R7.Text == "10110010")
                {
                    R[s1[1] - '0'] = NC[1];
                }

                s2 = "1";
                czs = Convert.ToString(R[1], 2).PadLeft(8, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                jqm = Convert.ToString("1001" + czs + "1" + jcq);
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs;
                MDR.Text = czs;
               
                DMR.Text  = Convert.ToString(R[1], 2).PadLeft(16, '0');
                change(R[s1[1] - '0'], s1);
            }
            else
            {
                s2 = s.Substring(j + 2);
                R[s1[1] - '0'] = R[Convert.ToInt32(s2, 16)];
                czs = Convert.ToString(R[Convert.ToInt32(s2, 16)], 2).PadLeft(8, '0');
                jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
                jqm = Convert.ToString("1001" + czs + "1" + jcq);
                code.Items.Add(jqm);
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = czs;
                MDR.Text = czs;

                DMR.Text  = Convert.ToString(R[1], 2).PadLeft(16, '0');
                change(R[s1[1] - '0'], s1);
            }


        }
        private void JMP(string s)   //PC是t？
        {
            int j = 4, t;
            string s1, s2, jqm, czs, jcq, dzm;
            s1 = s2 = "";
            s1 = s.Substring(j, 2);
            t = R[s1[1] - '0'];

            // code.Items.Add(R[s1[1] - '0']);
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            jqm = Convert.ToString("0111" + "kkkkkkkk" + "k" + jcq);
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = dzm;
            IR.Text = jqm;
            MAR.Text = "00000000";
            MDR.Text = "00000000";
            DMR.Text ="0000000000000000";
            //code.Items.Add(i);
            i = R[s1[1] - '0'] / 2 - 16 - 1;
            //code.Items.Add(i);
        }
        private void INC(string s)   //执行+1
        {
            int j = 4, temp = 1;
            string s1, s2, jqm, czs, jcq, psw, dzm, t = "0";
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            s1 = s.Substring(j, 2);
            s2 = s1;
            R[s1[1] - '0'] += 1;
            psw = srPro(R[s1[1] - '0'], 1);
            czs = Convert.ToString(R[s1[1] - '0'], 2).PadLeft(8, '0');
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            jqm = Convert.ToString("0100000" + jcq + "******");
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = dzm;
            IR.Text = jqm;
            MAR.Text = czs;
            MDR.Text = czs;
            PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
            DMR.Text = Convert.ToString(R[1], 2).PadLeft(16, '0');
            change_add(R[s1[1] - '0'], s1);
            change(R[s1[1] - '0'], s1);
        }
        private void DEC(string s)   //自减
        {
            int j = 4, tt;
            string s1, s2, jqm, czs, jcq, psw, dzm, t = "0";
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            s1 = s.Substring(j, 2);
            R[s1[1] - '0']--;
            //if ((i + 1) * 2 < 8) R[R[s1[1] - '0'] + (i + 1) * 2]--;
            czs = Convert.ToString(R[s1[1] - '0'] + (i + 1) * 2, 2).PadLeft(8, '0');
            psw = srPro(R[s1[1] - '0'], 0);
            jcq = Convert.ToString(s1[1] - '0', 2).PadLeft(3, '0');
            jqm = Convert.ToString("0101000" + jcq + "******");
            //jqm = Convert.ToString(dzm.Substring(4) + czs + jcq + "1");
            code.Items.Add(jqm);
            PC.Text = "00000000" + dzm;
            BUS.Text = dzm;
            IR.Text = jqm;
            MAR.Text = czs;
            MDR.Text = czs;
            DMR.Text  = czs.PadLeft(16, '0');
            PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
            change(R[s1[1] - '0'], s1);

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void ST(string s)
        {

            int j = 5;
            string s1, s2, jqm, czs, jcq, dzm, t = "0";
            dzm = Convert.ToString(ja + 2, 2).PadLeft(8, '0');
            s1 = s2 = "";
            s1 = "00001110";
            s2 = s.Substring(j);
          //  czs = Convert.ToString(R[s1[1] - '0'] + (i + 1) * 2, 2).PadLeft(8, '0');
            jcq = Convert.ToString(s2[1] - '0', 2).PadLeft(3, '0');
            jqm = Convert.ToString("1111" + "******" + "000"+jcq);
            code.Items.Add(jqm);
            if (R7.Text == "10110001")//00B1
            {

                NC[0] = R[s2[1] - '0'];
                text00B1.Text = Convert.ToString(R[s2[1] - '0'] + 1, 2).PadLeft(8, '0');
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = Convert.ToString(s2[1] - '0', 2).PadLeft(8, '0');
                MDR.Text = s1;
                DMR.Text = s1.PadLeft(16, '0');
                PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
            }
            else if (R7.Text == "10110010")//00B2
            {

                NC[1] = R[s2[1] - '0'];
                text00B2.Text = Convert.ToString(R[s2[1] - '0'], 2).PadLeft(8, '0');
                PC.Text = "00000000" + dzm;
                BUS.Text = dzm;
                IR.Text = jqm;
                MAR.Text = Convert.ToString(s2[1] - '0', 2).PadLeft(8, '0');
                MDR.Text = s1;
                PSW.Text = "0000" + V.Text + N.Text + Z.Text + C.Text;
                DMR.Text = s1.PadLeft(16, '0');
            }
        }
    }
}
