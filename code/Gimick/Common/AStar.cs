using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{
    // Fields
    public enum SearchState
    {
        Searching,
        Incomplete,
        Completed,
        Error

    }
    readonly List<AStarNode> _openList = new();
    public readonly AStarNode StartPoint;
    public readonly AStarNode EndPoint;
    SearchState _searchState;

    // Props

    public readonly Dictionary<WayPoint, AStarNode> NodeTable = new();

    // Constructors

    public AStar(IEnumerable<WayPoint> wayPoints, WayPoint startPoint, WayPoint endPoint)
    {
        foreach (var p in wayPoints)
        {
            if (p is null)
            {
                continue;
            }

            AStarNode sn = new(p);
            if (!p.CanMove)
            {
                sn.Status = NodeStatus.Exclude;
            }
            NodeTable[p] = sn;
        }

        StartPoint = NodeTable[startPoint];
        EndPoint = NodeTable[endPoint];

        if (!NodeTable.ContainsKey(startPoint))
        {
            throw new ArgumentException($"{nameof(wayPoints)}�̒���" +
                $"{nameof(startPoint)}���܂܂�Ă��܂���B");
        }

        AStarNode node = NodeTable[startPoint];
        node.Open(null, NodeTable[endPoint]);
        _openList.Add(node);
    }

    // Public Methods
    public SearchState SearchAll()
    {
        SearchState state;
        while (true)
        {
            state = SearchOneStep();
            if (state != SearchState.Searching)
            {
                break;
            }
        }

        return state;
    }

    public SearchState SearchOneStep()
    {
        if (_searchState != SearchState.Searching)
        {
            return _searchState;
        }

        AStarNode parentNode = GetMinCostNode();
        if (parentNode is null)
        {
            _searchState = SearchState.Incomplete;
            return _searchState;
        }

        parentNode.Status = NodeStatus.Close;
        _openList.Remove(parentNode);

        if (parentNode.WayPoint == EndPoint.WayPoint)
        {
            _searchState = SearchState.Completed;
            return _searchState;
        }

        foreach (WayPoint aroundPoint in parentNode.WayPoint.Rerations)
        {
            if (!NodeTable.ContainsKey(aroundPoint))
            {
                _searchState = SearchState.Error;
                return _searchState;
            }

            AStarNode tmpNode = NodeTable[aroundPoint];
            if (tmpNode.Status != NodeStatus.None)
            {
                continue;
            }

            tmpNode.Open(parentNode, EndPoint);

            _openList.Add(tmpNode);

            if (tmpNode.WayPoint == EndPoint.WayPoint)
            {
                tmpNode.Status = NodeStatus.Close;
                _openList.Remove(tmpNode);

                _searchState = SearchState.Completed;
                return _searchState;
            }
        }

        return _searchState;
    }

    public WayPoint[] GetRoute()
    {
        if (_searchState != SearchState.Completed)
        {
            throw new InvalidOperationException("");
        }

        AStarNode tmpNode = EndPoint;
        IEnumerable<WayPoint> f()
        {
            while (tmpNode.Parent != null)
            {
                yield return tmpNode.WayPoint;
                tmpNode = tmpNode.Parent;
            }

            yield return StartPoint.WayPoint;
        }

        var resultArray = f().ToArray();
        Array.Reverse(resultArray);
        return resultArray;
    }

    public AStarNode GetMinCostNode()
    {
        if (_openList.Count == 0)
        {
            return null;
        }

        AStarNode minCostNode = _openList[0];
        if (_openList.Count > 1)
        {
            for (int i = 1; i < _openList.Count; i++)
            {
                AStarNode tmpNode = _openList[i];
                if (minCostNode.Score > tmpNode.Score)
                {
                    minCostNode = tmpNode;
                }
            }
        }
        return minCostNode;
    }
}