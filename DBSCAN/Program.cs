using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBSCAN
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<ScanPoint, ScanPoint, double> calDistance = new Func<ScanPoint, ScanPoint, double>((p1, p2) =>
            {
                return Math.Sqrt(Math.Pow(p1.X - p2.X, 2.0) + Math.Pow(p1.Y - p2.Y, 2.0));
            });
            Random rand = new Random();
            List<ScanPoint> points = new List<ScanPoint>();
            int size = 15;
            int pointNum = 30;
            for (int i = 0; i < pointNum; )
            {
                int x = rand.Next(size);
                int y = rand.Next(size);
                if (points.FirstOrDefault(p => ((int)p.X == x) && ((int)p.Y == y)) == null)
                {
                    i++;
                    points.Add(new ScanPoint() { X = x, Y = y });
                }
            }
            Console.WriteLine(@"/******************Before DBSCAN(#:Point)********************/");
            int[,] array = new int[size, size];
            foreach (var p in points)
            {
                array[(int)p.X, (int)p.Y] = p.ClusterNum;
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (array[i, j] == 0)
                    {
                        Console.Write('*');
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.Write('#');
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
            Console.WriteLine(@"/*After DBSCAN(N:Noise,Upper Case:Core Point,Lower Case:Edge Point)*/");
            double radius = 3.0;
            int minPts = 3;
            DBSCAN dbscan = new DBSCAN(points, calDistance);
            dbscan.ImplementAlgorithm(radius, minPts);
            foreach (var p in points)
            {
                if (p.PointType == PointType.Noise)
                    array[(int)p.X, (int)p.Y] = -100;
                else if (p.PointType == PointType.Core)
                    array[(int)p.X, (int)p.Y] = p.ClusterNum + 1;
                else
                    array[(int)p.X, (int)p.Y] = (p.ClusterNum + 1) * 100;
            }
            char coreChar = 'A';
            char edgeChar = 'a';
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (array[i, j] == 0)
                    {
                        Console.Write('*');
                        Console.Write(' ');
                    }
                    else if (array[i, j] == -100)
                    {
                        Console.Write('N');
                        Console.Write(' ');
                    }
                    else if (array[i, j] >= 100)
                    {
                        int edge = array[i, j] / 100 - 1;
                        Console.Write(Convert.ToChar(edgeChar + edge));
                        Console.Write(' ');
                    }
                    else
                    {
                        int core = array[i, j] - 1;
                        Console.Write(Convert.ToChar(coreChar + core));
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
            Console.ReadLine();
        }
    }
}
