using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global
namespace ZDV.LinQ.Hierarchy
{
    public static class EnumberableExtensions
    {
        public static IEnumerable<TResult> Ancestors<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, TResult> parentSelector, bool includeSelf = false)
        {
            return startNodes.SelectMany(startNode => HierarchyWalker.Ancestors(startNode, parentSelector, includeSelf));
        }

        public static IEnumerable<TResult> AncestorsAndSelf<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, TResult> parentSelector)
        {
            return Ancestors(startNodes, parentSelector, true);
        }
        
        public static IEnumerable<TResult> Descendants<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, IEnumerable<TResult>> childSelector, bool includeSelf = false)
        {
            return startNodes.SelectMany(startNode => HierarchyWalker.Descendants(startNode, childSelector, includeSelf));
        }

        public static IEnumerable<TResult> DescendantsAndSelf<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, IEnumerable<TResult>> childSelector)
        {
            return Descendants(startNodes, childSelector, true);
        }

        public static IEnumerable<TResult> Siblings<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, TResult> parentSelector, Func<TResult, IEnumerable<TResult>> childSelector, 
            bool includeSelf = false)
        {
            return startNodes.SelectMany(startNode => HierarchyWalker.Siblings(startNode, parentSelector, childSelector, includeSelf));
        }

        public static IEnumerable<TResult> SiblingsAndSelf<TResult>(this IEnumerable<TResult> startNodes,
            Func<TResult, TResult> parentSelector, Func<TResult, IEnumerable<TResult>> childSelector)
        {
            return Siblings(startNodes, parentSelector, childSelector, true);
        }
    }
}
