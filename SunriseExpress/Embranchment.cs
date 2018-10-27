using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunriseExpress
{
    public class Embranchment
    {
        public string Name { get; set; }

        private static IList<Embranchment> _embranchments;
        public static IList<Embranchment> Embranchments
        {
            get
            {
                if (_embranchments == null)
                {
                    _embranchments = new List<Embranchment>();
                }
                return _embranchments;
            }
        }

        public Embranchment()
        {
        }

        public Embranchment(string name)
        {
            this.Name = name;
        }

        public static Embranchment AddEmbranchment(string name)
        {
            var embranchment = Embranchments.Where(a => a.Name.Equals(name)).FirstOrDefault();
            if (embranchment == null)
            {
                embranchment = new Embranchment(name);
                Embranchments.Add(embranchment);
            }
            return embranchment;
        }
    }

    public class EmbranchmentShipping
    {
        public Embranchment From { get; set; }

        public Embranchment To { get; set; }

        public int Duration { get; set; }

        private static IList<EmbranchmentShipping> _embranchmentShippings;
        public static IList<EmbranchmentShipping> EmbranchmentShippings
        {
            get
            {
                if (_embranchmentShippings == null)
                {
                    _embranchmentShippings = new List<EmbranchmentShipping>();
                }
                return _embranchmentShippings;
            }
        }

        public EmbranchmentShipping()
        {
        }

        public EmbranchmentShipping(Embranchment from, Embranchment to, int duration)
        {
            this.From = from;
            this.To = to;
            this.Duration = duration;
        }

        public static void AddEmbranchmentShippings(Embranchment from, Embranchment to, int weighting)
        {
            if (!EmbranchmentShippings.Any(a => a.From == from && a.To == to))
            {
                EmbranchmentShippings.Add(new EmbranchmentShipping(from, to, weighting));
            }
        }

        /// <summary>
        /// 发货到指定路径的时间
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <returns>发货时间</returns>
        public static int ShippingToEmbranchment(string path)
        {
            var pathNodes = path.Split('-');
            int sumDuration = 0;
            for (int i = 0; i < pathNodes.Length; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                var embranchmentShipping = EmbranchmentShippings
                    .Where(a => a.From.Name.Equals(pathNodes[i - 1]) && a.To.Name.Equals(pathNodes[i]))
                    .FirstOrDefault();
                if (embranchmentShipping != null)
                {
                    sumDuration += embranchmentShipping.Duration;
                }
                else
                {
                    sumDuration = 0;
                    break;
                }
            }
            return sumDuration;
        }
    }

    public class EmbranchmentShippingNode
    {
        public Embranchment From { get; set; }

        public Embranchment To { get; set; }

        /// <summary>
        /// 源到目票的路径
        /// </summary>
        public IList<EmbranchmentShipping> Paths { get; set; }
        
        /// <summary>
        /// 递归出一个节点下指定次数的所有路径
        /// </summary>
        /// <param name="nodes">收集并反回所有节点</param>
        /// <param name="start">起始节点</param>
        /// <param name="parent">父级节点</param>
        /// <param name="level">递归层次</param>
        /// <param name="stop">终结次数</param>
        public static void ErgodicNodeFromStart(ref IList<EmbranchmentShippingNode> nodes,
            Embranchment start,
            EmbranchmentShippingNode parent, int level, int stop)
        {
            if (nodes == null)
            {
                nodes = new List<EmbranchmentShippingNode>();
            }
            if (level < stop)
            {
                //首次获取起始节点，后面取父级节点
                IList<EmbranchmentShipping> shippings = null;
                if (parent == null)
                {
                    shippings = EmbranchmentShipping.EmbranchmentShippings.Where(a => a.From.Name.Equals(start.Name)).ToList();
                }
                else
                {
                    shippings = EmbranchmentShipping.EmbranchmentShippings.Where(a => a.From.Name.Equals(parent.To.Name)).ToList();
                }
                foreach (var embranchmentShipping in shippings)
                {
                    var node = new EmbranchmentShippingNode();
                    node.From = embranchmentShipping.From;
                    node.To = embranchmentShipping.To;
                    if (parent == null)
                    {
                        node.Paths = new List<EmbranchmentShipping>();
                        node.Paths.Add(embranchmentShipping);
                    }
                    else
                    {
                        var paths = parent.Paths.ToList();
                        paths.Add(embranchmentShipping);
                        node.Paths = paths;
                    }
                    nodes.Add(node);
                    ErgodicNodeFromStart(ref nodes, start, node, level + 1, stop);
                }
            }
        }
    }
}
