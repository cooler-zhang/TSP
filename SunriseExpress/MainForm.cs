using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunriseExpress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //初始化原始数据
            this.txtGraph.Text = "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7";
            this.txtInputPath.Text = "A-B-C\r\nA-D\r\nA-D-C\r\nA-E-B-C-D\r\nA-E-D\r\nC-C\r\nA-C\r\nA-C\r\nB-B\r\nC-C";
            btnImport_Click(null, null);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var graphs = txtGraph.Text.Trim().Split(',');
                foreach (var graph in graphs)
                {
                    var processGraph = graph.Trim();
                    string fromName = processGraph.Substring(0, 1);
                    var from = Embranchment.AddEmbranchment(fromName);
                    string toName = processGraph.Substring(1, 1);
                    var to = Embranchment.AddEmbranchment(toName);
                    int weighting = Convert.ToInt32(processGraph.Substring(2, 1));
                    EmbranchmentShipping.AddEmbranchmentShippings(from, to, weighting);
                }
                MessageBox.Show("导入完成.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化数据的格式不正确." + ex.Message);
            }
        }

        private void btnCaculate_Click(object sender, EventArgs e)
        {
            lbxResult.Items.Clear();
            try
            {
                var paths = txtInputPath.Text.Replace("\r\n", ",").Split(',');

                for (int i = 0; i < paths.Length; i++)
                {
                    var message = string.Empty;
                    var path = paths[i].Trim();
                    if (i <= 4)
                    {
                        int sumWeighting = EmbranchmentShipping.ShippingToEmbranchment(path);
                        if (sumWeighting > 0)
                        {
                            message = sumWeighting.ToString();
                        }
                        else
                        {
                            message = "NO SUCH PATH";
                        }
                    }
                    else if (i == 5)
                    {
                        var pathNodes = path.Split('-');
                        var from = pathNodes[0];
                        var to = pathNodes[1];
                        var start = Embranchment.Embranchments.Where(a => a.Name.Equals(from)).FirstOrDefault();
                        IList<EmbranchmentShippingNode> result = null;
                        //递归起始节点4次，遍历出该节点下的路径
                        EmbranchmentShippingNode.ErgodicNodeFromStart(ref result, start, null, 0, 3);
                        message = result.Where(a => a.To.Name.Equals(to)).Count().ToString();
                    }
                    else if (i == 6)
                    {
                        var pathNodes = path.Split('-');
                        var from = pathNodes[0];
                        var to = pathNodes[1];
                        var start = Embranchment.Embranchments.Where(a => a.Name.Equals(from)).FirstOrDefault();
                        IList<EmbranchmentShippingNode> result = null;
                        //递归起始节点4次，遍历出该节点下的路径
                        EmbranchmentShippingNode.ErgodicNodeFromStart(ref result, start, null, 0, 4);
                        var plans = result.Where(a => a.Paths.Count == 4 && a.To.Name.Equals(to)).ToList();
                        message = plans.Count.ToString();
                    }
                    else if (i == 7 || i == 8)
                    {
                        var pathNodes = path.Split('-');
                        var from = pathNodes[0];
                        var to = pathNodes[1];
                        var start = Embranchment.Embranchments.Where(a => a.Name == from).FirstOrDefault();
                        IList<EmbranchmentShippingNode> result = null;
                        //递归起始节点10次，遍历出该节点下的路径,过滤出源到目标的最短发货时间
                        EmbranchmentShippingNode.ErgodicNodeFromStart(ref result, start, null, 0, 10);
                        var plans = result.Where(a => a.To.Name.Equals(to)).ToList();
                        int minDuration = 0;
                        int sumDuration = 0;
                        foreach (var plan in plans)
                        {
                            sumDuration = plan.Paths.Sum(a => a.Duration);
                            if (minDuration == 0 || minDuration > sumDuration)
                            {
                                minDuration = sumDuration;
                            }
                        }
                        message = minDuration.ToString();
                    }
                    else if (i == 9)
                    {
                        var pathNodes = path.Split('-');
                        var from = pathNodes[0];
                        var to = pathNodes[1];
                        var start = Embranchment.Embranchments.Where(a => a.Name == from).FirstOrDefault();
                        IList<EmbranchmentShippingNode> result = null;
                        //递归起始节点10次，遍历出该节点下的路径,过滤出源到目标的权重范围内的路径
                        EmbranchmentShippingNode.ErgodicNodeFromStart(ref result, start, null, 0, 10);
                        var plans = result.Where(a => a.To.Name.Equals(to)).ToList();
                        message = plans.Where(a => a.Paths.Sum(b => b.Duration) < 30).Count().ToString();
                    }
                    lbxResult.Items.Add(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("可能输入数据的格式不正确." + ex.Message);
            }
        }
    }
}
