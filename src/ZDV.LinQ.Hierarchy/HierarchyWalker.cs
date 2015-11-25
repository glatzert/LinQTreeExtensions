using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global
namespace ZDV.LinQ.Hierarchy
{
    public static class HierarchyWalker
    {
        public static IEnumerable<TNode> Ancestors<TNode>(TNode startNode, Func<TNode, TNode> parentSelector, bool includeSelf = false)
        {
            var currentNode = includeSelf ? startNode : parentSelector(startNode);
            while (currentNode != null)
            {
                yield return currentNode;
                currentNode = parentSelector(currentNode);
            }
        }

        public static IEnumerable<TNode> Descendants<TNode>(TNode startNode,
            Func<TNode, IEnumerable<TNode>> childSelector, bool includeSelf = false,
            DescendStrategy strategy = DescendStrategy.DepthFirst)
        {
            switch (strategy)
            {
                case DescendStrategy.BreadthFirst:
                    return BreadthFirstDescendants(startNode, childSelector, includeSelf);
                case DescendStrategy.DepthFirst:
                    return DepthFirstDescendants(startNode, childSelector, includeSelf);
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }

        private static IEnumerable<TNode> DepthFirstDescendants<TNode>(TNode startNode,
            Func<TNode, IEnumerable<TNode>> childSelector, bool includeSelf)
        {
            if (includeSelf)
                yield return startNode;

            var children = NullSaveFunc(startNode, childSelector);
            if(children == null)
                yield break;

            foreach (var child in children.Where(c => c != null))
                foreach(var node in DepthFirstDescendants(child, childSelector, true))
                    yield return node;
        }

        private static IEnumerable<TNode> BreadthFirstDescendants<TNode>(TNode startNode,
            Func<TNode, IEnumerable<TNode>> childSelector, bool includeSelf)
        {
            var queue = new Queue<TNode>();
            if (includeSelf)
                queue.Enqueue(startNode);
            
            EnqueueChildren(queue, startNode, childSelector);

            while (queue.Any())
            {
                var currentNode = queue.Dequeue();
                yield return currentNode;
                EnqueueChildren(queue, currentNode, childSelector);
            }
        }

        private static void EnqueueChildren<TNode>(Queue<TNode> queue, TNode node, Func<TNode, IEnumerable<TNode>> childSelector)
        {
            var children = NullSaveFunc(node, childSelector);
            if (children == null)
                return;

            foreach (var childNode in children.Where(c => c != null))
                queue.Enqueue(childNode);
        }

        private static TResult NullSaveFunc<TNode, TResult>(TNode node,
            Func<TNode, TResult> func)
        {
            try
            {
                return func(node);
            }
            catch (NullReferenceException)
            {
                return default(TResult);
            }
        }

        public static IEnumerable<TNode> Siblings<TNode>(TNode startNode,
            Func<TNode, TNode> parentSelector, Func<TNode, IEnumerable<TNode>> childSelector,
            bool includeSelf = false)
        {
            var parent = parentSelector(startNode);
            if (parent == null)
            {
                if (includeSelf)
                    yield return startNode;
                yield break;
            }

            foreach (var child in childSelector(parent).Where(c => c != null))
                if (includeSelf || !startNode.Equals(child))
                    yield return child;
        }

        #region Aliases
        public static IEnumerable<TNode> AncestorsAndSelf<TNode>(TNode startNode,
            Func<TNode, TNode> parentSelector)
        {
            return Ancestors(startNode, parentSelector, true);
        }

        public static IEnumerable<TNode> DescendantsAndSelf<TNode>(TNode startNode,
            Func<TNode, IEnumerable<TNode>> childSelector)
        {
            return Descendants(startNode, childSelector, true);
        }

        public static IEnumerable<TNode> SiblingsAndSelf<TNode>(TNode startNode,
            Func<TNode, TNode> parentSelector, Func<TNode, IEnumerable<TNode>> childSelector)
        {
            return Siblings(startNode, parentSelector, childSelector, true);
        }
        #endregion
    }
}
