using System;
using System.Windows.Forms;
using gts;
using System.Xml;
using System.Collections.Generic;


namespace 三维平移台控制系统
{
    public partial class mainFrom : DevComponents.DotNetBar.OfficeForm
    {
        public mainFrom()
        {
            this.EnableGlass = false;
            InitializeComponent();
        }
        short Rtn;
        int  vel = 0;
        int[] pos= {0,0,0 };
        int[] pos1 = { 0, 0, 0 };
        bool flag = false;
        uint clk;
        double prfpos, prfvel, encpos, encvel;
        short AXIS = 1;

        double X = 0;
        double Y = 0;
        double Z = 0;


        private void buttonX1_Click(object sender, EventArgs e)
        {
          
            mc.GT_ZeroPos(AXIS, 1);
            //for (int i=0; i<3;i++)
            //{
            //    pos1[i] += pos[i];
            //}
            pos1[AXIS - 1] += pos[AXIS - 1];
            pos[AXIS-1] = 0;
           
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            //Rtn = mc.GT_Close();
            Rtn = mc.GT_PrfTrap(AXIS);
            mc.TTrapPrm trapprm;
            mc.GT_GetTrapPrm(AXIS, out trapprm);
            trapprm.acc = 0.1;//设置加速度
            trapprm.dec = 0.1;//设置减速度
            trapprm.smoothTime = 1;
            mc.GT_SetTrapPrm(AXIS, ref trapprm);
            pos[AXIS-1] += Convert.ToInt32(textBoxX1.Text)*500;
            vel = Convert.ToInt32(textBoxX2.Text)*2;
            mc.GT_SetPos(AXIS, pos[AXIS-1]);
            mc.GT_SetVel(AXIS, vel);
            // mc.GT_Update(AXIS);
            mc.GT_Update(1 << (AXIS - 1));
            //MessageBox.Show((1 << (AXIS - 1)).ToString());
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            //Rtn = mc.GT_Close();
            Rtn = mc.GT_PrfTrap(AXIS);
            mc.TTrapPrm trapprm;
            mc.GT_GetTrapPrm(AXIS, out trapprm);
            trapprm.acc = 0.1;
            trapprm.dec = 0.1;
            trapprm.smoothTime = 1;
            mc.GT_SetTrapPrm(AXIS, ref trapprm);
            pos[AXIS-1] -= Convert.ToInt32(textBoxX1.Text)*500;
            vel = Convert.ToInt32(textBoxX2.Text)*2;
            mc.GT_SetPos(AXIS, pos[AXIS-1]);
            mc.GT_SetVel(AXIS, vel);
            //mc.GT_Update(AXIS);
            mc.GT_Update(1 << (AXIS - 1));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if ()
            //{
            //    mc.GT_GetPrfPos(AXIS, out prfpos, 1, out clk);// profile 起始轴号
            //    mc.GT_GetEncPos(AXIS, out encpos, 1, out clk);// encoder 起始轴号
            //    textBox3.Text = prfpos.ToString();
            //    textBox5.Text = encpos.ToString();
            //    mc.GT_GetPrfVel(AXIS, out prfvel, 1, out clk);// 起始规划轴号
            //    mc.GT_GetEncVel(AXIS, out encvel, 1, out clk);// encoder 起始轴号
            //    textBox4.Text = prfvel.ToString();
            //    textBox6.Text = encvel.ToString();
            //}
            List<KeyValuePair<string, string>> settings = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>("", "");

            mc.GT_GetPrfPos(AXIS, out prfpos, 1, out clk);// profile 起始轴号
            mc.GT_GetPrfVel(AXIS, out prfvel, 1, out clk);// 起始规划轴号
            if (AXIS == 1)
            {
                labelX4.Text = "规划位置 : " + (prfpos / 500).ToString() + " mm";
                labelX5.Text = "规划速度 : " + (prfvel / 2).ToString() + " mm/s";
                kv = new KeyValuePair<string, string>("X", ((prfpos + pos1[0]) / 500 +X).ToString());
                settings.Add(kv);
                labelX10.Text = "距离原点 : " + ((prfpos + pos1[0])/500 + X).ToString() + " mm";
                if ((prfpos + pos1[0]) / 500 + X > 850)
                    mc.GT_Stop(AXIS, 0);

            }
            else if (AXIS == 2)
            {
                labelX6.Text = "规划位置 : " + (prfpos / 500).ToString() + " mm";
                labelX7.Text = "规划速度 : " + (prfvel / 2).ToString() + " mm/s";
                kv = new KeyValuePair<string, string>("Y", ((prfpos + pos1[1]) / 500 + Y).ToString());
                settings.Add(kv);
                labelX11.Text = "距离原点 : " + ((prfpos + pos1[1]) / 500 + Y).ToString() + " mm";
                if ((prfpos + pos1[1]) / 500 + Y > 500)
                    mc.GT_Stop(AXIS, 0);
            }
            else if (AXIS == 3)
            {
                labelX8.Text = "规划位置 : " + (prfpos / 500).ToString() + " mm";
                labelX9.Text = "规划速度 : " + (prfvel / 2).ToString() + " mm/s";
                kv = new KeyValuePair<string, string>("Z", ((prfpos + pos1[2]) / 500 + Z).ToString());
                settings.Add(kv);
                labelX12.Text = "距离原点 : " + ((prfpos + pos1[2]) / 500 + Z).ToString() + " mm";
                if ((prfpos + pos1[2]) / 500 + Z > 500)
                    mc.GT_Stop(1 << (AXIS - 1), 0);

            }
            SaveSettings(settings);

        }
        private void ReadConfigXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\config.xml");
            XmlNode settingNode = xmlDoc.DocumentElement;

            XmlElement e = settingNode.SelectSingleNode("X") as XmlElement;
            if (e == null)
            {
                X = 0;
            }
            else
            {
                double val = 0;
                if (!double.TryParse(e.InnerText, out val))
                {
                    X = 0;
                }
                else
                {
                    X = val;
                }
            }

            e = settingNode.SelectSingleNode("Y") as XmlElement;
            if (e == null)
            {
                Y = 0;
            }
            else
            {
                double val = 0;
                if (!double.TryParse(e.InnerText, out val))
                {
                    Y = 0;
                }
                else
                {
                    Y = val;
                }
            }

            e = settingNode.SelectSingleNode("Z") as XmlElement;
            if (e == null)
            {
                Z = 0;
            }
            else
            {
                double val = 0;
                if (!double.TryParse(e.InnerText, out val))
                {
                    Z = 0;
                }
                else
                {
                    Z = val;
                }
            }


        }

        private void mainFrom_Load(object sender, EventArgs e)
        {
            comboBoxEx1.SelectedIndex = 0;

            textBoxX1.Text = "4";
            textBoxX2.Text = "5";
           
           
            labelX4.Text = "规划位置 : " + (0).ToString() + " mm";
            labelX5.Text = "规划速度 : " + (0).ToString() + " mm/s";
           
           
            labelX6.Text = "规划位置 : " + (0).ToString() + " mm";
            labelX7.Text = "规划速度 : " + (0).ToString() + " mm/s";
           
            labelX8.Text = "规划位置 : " + (0).ToString() + " mm";
            labelX9.Text = "规划速度 : " + (0).ToString() + " mm/s";

          
            /*初始化*/
            Rtn = mc.GT_Open(0, 1);
            Rtn = mc.GT_Reset();
            Rtn = mc.GT_LoadConfig("GTS800.cfg"); ;
            Rtn = mc.GT_ClrSts(1, 8); //axis 起始轴号，count 

            ReadConfigXML();
            labelX10.Text = "距离原点 : " + (X).ToString() + " mm";
            labelX11.Text = "距离原点 : " + (Y).ToString() + " mm";
            labelX12.Text = "距离原点 : " + (Z).ToString() + " mm";

            labelX13.Text = "最大距离 : " + (850).ToString() + " mm";
            labelX14.Text = "最大距离 : " + (500).ToString() + " mm";
            labelX15.Text = "最大距离 : " + (500).ToString() + " mm";
            timer1.Enabled = true;
        }

        private void buttonX4_Click(object sender, EventArgs e)
        {
            mc.GT_Stop(1 << (AXIS - 1), 0);
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxEx1.SelectedItem.ToString())
            {
                case "X轴":
                    AXIS = 1; break;
                case "Y轴":
                    AXIS = 2; break;
                case "Z轴":
                    AXIS = 3; break;
                default:break;
            }
        }

        public static void SaveSettings(List<KeyValuePair<string, string>> settings)
        {
            try
            {
                string docPath = AppDomain.CurrentDomain.BaseDirectory + "\\config.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(docPath);
                XmlNode settingNode = xmlDoc.DocumentElement;
                if (settingNode == null)
                    return;

                foreach (var kv in settings)
                {
                    SetNodeValue(xmlDoc, settingNode, kv.Key, kv.Value);
                }

                xmlDoc.Save(docPath);
            }
            catch { }
        }

        private static void SetNodeValue(XmlDocument XmlDoc, XmlNode rootnode, string key, string value)
        {
            try
            {
                XmlElement e = rootnode.SelectSingleNode(key) as XmlElement;
                if (e == null)
                {
                    XmlNode node = XmlDoc.CreateNode(XmlNodeType.Element, key, "");
                    node.InnerText = value;
                    rootnode.AppendChild(node);
                }
                else
                {
                    e.InnerText = value;
                }
            }
            catch { }
        }

    }
}
