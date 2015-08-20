using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global
namespace ZDV.LinQ.Tree
{
    public static class HierarchyWalker
    {
        public static IEnumerable<TResult> Ancestors<TResult>(TResult startNode, Func<TResult, TResult> parentSelector, bool includeSelf = false)
        {
            var currentNode = includeSelf ? startNode : parentSelector(startNode);
            while (currentNode != null)
            {
                yield return currentNode;
                currentNode = parentSelector(currentNode);
            }
        }

        public static IEnumerable<TResult> Descendants<TResult>(TResult startNode,
            Func<TResult, IEnumerable<TResult>> childSelector, bool includeSelf = false,
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

        private static IEnumerable<TResult> DepthFirstDescendants<TResult>(TResult startNode,
            Func<TResult, IEnumerable<TResult>> childSelector, bool includeSelf)
        {
            if (includeSelf)
                yield return startNode;

            foreach (var child in childSelector(startNode))
                foreach(var node in DepthFirstDescendants(child, childSelector, true))
                    yield return node;
        }

        private static IEnumerable<TResult> BreadthFirstDescendants<TResult>(TResult startNode,
            Func<TResult, IEnumerable<TResult>> childSelector, bool includeSelf)
        {
            var queue = new Queue<TResult>();
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

        private static void EnqueueChildren<TResult>(Queue<TResult> queue, TResult node, Func<TResult, IEnumerable<TResult>> childSelector)
        {
            foreach (var childNode in childSelector(node))
                queue.Enqueue(childNode);
        }

        public static IEnumerable<TResult> Siblings<TResult>(TResult startNode,
            Func<TResult, TResult> parentSelector, Func<TResult, IEnumerable<TResult>> childSelector,
            bool includeSelf = false)
        {
            var parent = parentSelector(startNode);
            if (parent == null)
            {
                if (includeSelf)
                    yield return startNode;
                yield break;
            }

            foreach (var child in childSelector(parent))
                if (includeSelf || !startNode.Equals(child))
                    yield return child;
        }

        #region Aliases
        public static IEnumerable<TResult> AncestorsAndSelf<TResult>(TResult startNode,
            Func<TResult, TResult> parentSelector)
        {
            return Ancestors(startNode, parentSelector, true);
        }

        public static IEnumerable<TResult> DescendantsAndSelf<TResult>(TResult startNode,
            Func<TResult, IEnumerable<TResult>> childSelector)
        {
            return Descendants(startNode, childSelector, true);
        }

        public static IEnumerable<TResult> SiblingsAndSelf<TResult>(TResult startNode,
            Func<TResult, TResult> parentSelector, Func<TResult, IEnumerable<TResult>> childSelector)
        {
            return Siblings(startNode, parentSelector, childSelector, true);
        }
        #endregion
    }
}
