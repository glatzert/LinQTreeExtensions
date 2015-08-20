using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ZDV.LinQ.Tree.Tests
{
    public class TreeNode
    {
        public TreeNode(int nodeId)
        {
            NodeId = nodeId;
            Children = new List<TreeNode>();
        }

        public int NodeId { get; private set; }
        public TreeNode Parent { get; set; }
        public ICollection<TreeNode> Children { get; private set; }
    }

    public class HierarchicalWalkerFixture : IDisposable
    {
        public Dictionary<int, TreeNode> Tree { get; private set; }

        public HierarchicalWalkerFixture()
        {
            var treeNodes = new List<TreeNode>
            {
                new TreeNode(1),
                new TreeNode(11),
                new TreeNode(111),
                new TreeNode(1111),
                new TreeNode(11111),
                new TreeNode(111111),
                new TreeNode(111112),
                new TreeNode(11112),
                new TreeNode(11113),
                new TreeNode(1112),
                new TreeNode(11121),
                new TreeNode(11122),
                new TreeNode(11123),
                new TreeNode(1113),
                new TreeNode(11131),
                new TreeNode(11132),
                new TreeNode(112),
                new TreeNode(1121),
                new TreeNode(1122),
                new TreeNode(1123),
                new TreeNode(12),
                new TreeNode(121),
                new TreeNode(1211),
                new TreeNode(1212),
                new TreeNode(1213),
                new TreeNode(1214),
                new TreeNode(1215),
                new TreeNode(12151),
                new TreeNode(12152),
                new TreeNode(12153),
                new TreeNode(122),
                new TreeNode(123),
                new TreeNode(1231),
                new TreeNode(1232),
                new TreeNode(13),
                new TreeNode(131),
                new TreeNode(1311),
                new TreeNode(1312),
                new TreeNode(132),
                new TreeNode(1321),
                new TreeNode(1322),
                new TreeNode(133),
                new TreeNode(1331),
                new TreeNode(13311),
                new TreeNode(133111),
                new TreeNode(133112),
                new TreeNode(1332)
            };
            
            Tree = treeNodes.ToDictionary(t => t.NodeId);
            
            foreach (var treeNode in Tree.Values)
            {
                var nodeIdString = treeNode.NodeId.ToString();
                int parentId;
                if (int.TryParse(nodeIdString.Substring(0, nodeIdString.Length -1), out parentId))
                {
                    var parentNode = Tree[parentId];
                    treeNode.Parent = parentNode;
                    parentNode.Children.Add(treeNode);
                }
            }
        }
        
        public void Dispose()
        {
            Tree.Clear();
        }
    }

    public class HierarchyWalkerTests : IClassFixture<HierarchicalWalkerFixture>
    {
        private readonly Dictionary<int, TreeNode> _tree;

        public HierarchyWalkerTests(HierarchicalWalkerFixture fixture)
        {
            _tree = fixture.Tree;
        }

        [Fact(DisplayName = "Test getting ancestors")]
        public void Ancestors()
        {
            var startNode = _tree[1111];
            var expectedNodes = new[] {111, 11, 1};

            var result = HierarchyWalker.Ancestors(startNode, n => n.Parent);
            var resultNodes = result.Select(n => n.NodeId).ToList();

            Assert.True(expectedNodes.Length == resultNodes.Count);
            Assert.True(expectedNodes.All(node => resultNodes.Contains(node)));
        }

        [Fact(DisplayName = "Test getting ancestors from root node.")]
        public void AncestorsFromRoot()
        {
            var startNode = _tree[1];
            
            var result = HierarchyWalker.Ancestors(startNode, n => n.Parent);
            Assert.True(!result.Any());
        }

        [Fact(DisplayName = "Test getting descendants")]
        public void Descendants()
        {
            var startNode = _tree[1111];
            var expectedNodes = new [] {11111, 11112, 11113, 111111, 111112};

            var result = HierarchyWalker.Descendants(startNode, n => n.Children);
            var resultNodes = result.Select(n => n.NodeId).ToList();

            Assert.True(expectedNodes.Length == resultNodes.Count);
            Assert.True(expectedNodes.All(node => resultNodes.Contains(node)));
        }

        [Fact(DisplayName = "Test getting descendants from leave node")]
        public void DescendantsFromLeave()
        {
            var startNode = _tree[111112];
            
            var result = HierarchyWalker.Descendants(startNode, n => n.Children);
            Assert.True(!result.Any());
        }
    }
}
