using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBSCAN
{
    public class DBSCAN
    {
        private List<ScanPoint> _allPoints;
        public List<ScanPoint> AllPoints
        {
            get { return _allPoints ?? (_allPoints = new List<ScanPoint>()); }
            set { _allPoints = value; }
        }

        private Func<ScanPoint, ScanPoint, double> _calculateDistance;

        public DBSCAN(List<ScanPoint> allPoints, Func<ScanPoint, ScanPoint, double> calculateDistance)
        {
            _allPoints = allPoints;
            _calculateDistance = calculateDistance;
        }

        public void ImplementAlgorithm(double radius, int minPts)
        {
            int clusterNum = 0;
            foreach (var point in _allPoints)
            {
                if (point.VisitedType == VisitedType.Unvisited)
                {
                    point.VisitedType = VisitedType.Visited;
                    List<ScanPoint> nearPoints = GetRegionPoints(point, radius);
                    if (nearPoints.Count < minPts)
                        point.PointType = PointType.Noise;
                    else
                        ExpandCluster(point, nearPoints, clusterNum++, radius, minPts);
                }
            }
        }

        private void ExpandCluster(ScanPoint corePoint, List<ScanPoint> nearPoint, int clusterNum, double radius, int minPts)
        {
            corePoint.PointType = PointType.Core;
            corePoint.ClusterNum = clusterNum;
            foreach (var p in nearPoint)
            {
                if (p.VisitedType == VisitedType.Unvisited)
                {
                    p.VisitedType = VisitedType.Visited;
                    List<ScanPoint> neighborPoints = GetRegionPoints(p, radius);
                    if (neighborPoints.Count >= minPts)
                        ExpandCluster(p, neighborPoints, clusterNum, radius, minPts);
                    else
                    {
                        p.PointType = PointType.Edge;
                        p.ClusterNum = clusterNum;
                    }
                }
                else
                {
                    if (p.ClusterNum < 0)
                    {
                        p.PointType = PointType.Edge;
                        p.ClusterNum = clusterNum;
                    }
                }
            }
        }

        private List<ScanPoint> GetRegionPoints(ScanPoint corePoint, double radius)
        {
            List<ScanPoint> result = new List<ScanPoint>();
            List<ScanPoint> otherPoints = _allPoints.Except(new List<ScanPoint>() { corePoint }).ToList();
            foreach (var p in otherPoints)
            {
                if (_calculateDistance(corePoint, p) <= radius)
                    result.Add(p);
            }
            return result;
        }
    }

    public class ScanPoint
    {
        private int _clusterNum = -1;
        public int ClusterNum
        {
            get { return _clusterNum; }
            set { _clusterNum = value; }
        }

        private PointType _pointType = PointType.None;
        public PointType PointType
        {
            get { return _pointType; }
            set { _pointType = value; }
        }

        private VisitedType _visitedType = VisitedType.Unvisited;
        public VisitedType VisitedType
        {
            get { return _visitedType; }
            set { _visitedType = value; }
        }

        public double X { get; set; }
        public double Y { get; set; }
        public object Tag { get; set; }
    }

    public enum PointType
    {
        None,
        Noise,
        Core,
        Edge
    }

    public enum VisitedType
    {
        Unvisited,
        Visited
    }

}
