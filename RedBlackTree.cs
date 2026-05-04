using System;

namespace semestrovka2
{
    public enum Color { Red, Black }

    public class Node
    {
        public int Value;
        public Color Color;
        public Node Left, Right, Parent;

        public Node(int value, Color color, Node nil)
        {
            Value = value;
            Color = color;
            Left = nil;
            Right = nil;
            Parent = nil;
        }

        public Node() { } 
    }

    public class RedBlackTree
    {
        private Node root;
        private Node nil;
        public int OperationCount { get; private set; }

        public RedBlackTree()
        {
            nil = new Node();
            nil.Color = Color.Black;
            nil.Left = nil.Right = nil.Parent = nil;
            root = nil;
            OperationCount = 0;
        }

        public void ResetOperations()
        {
            OperationCount = 0;
        }

        public bool Find(int value)
        {
            Node current = root;
            while (current != nil)
            {
                OperationCount++;
                if (value == current.Value)
                {
                    return true;
                }
                OperationCount++;
                if (value < current.Value)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }
            return false;
        }

        public void Insert(int value)
        {
            Node newNode = new Node(value, Color.Red, nil);
            Node parent = nil;
            Node current = root;

            while (current != nil)
            {
                parent = current;
                OperationCount++;
                if (value < current.Value)
                {
                    current = current.Left;
                    OperationCount++;
                }
                else if (value > current.Value)
                {
                    current = current.Right;
                    OperationCount++;
                }
                else
                {
                    return;
                }
            }

            newNode.Parent = parent;
            if (parent == nil)
            {
                root = newNode;
            }
            else if (value < parent.Value)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }

            FixInsert(newNode);
        }

        private void FixInsert(Node node)
        {
            while (node.Parent.Color == Color.Red)
            {
                Node parent = node.Parent;
                Node grandParent = parent.Parent;
                if (grandParent == nil) break;

                if (parent == grandParent.Left)
                {
                    Node uncle = grandParent.Right;
                    if (uncle.Color == Color.Red)
                    {
                        parent.Color = Color.Black;
                        uncle.Color = Color.Black;
                        grandParent.Color = Color.Red;
                        node = grandParent;
                        OperationCount += 3;
                    }
                    else
                    {
                        if (node == parent.Right)
                        {
                            node = parent;
                            RotateLeft(node);
                            OperationCount++;
                        }
                        parent.Color = Color.Black;
                        grandParent.Color = Color.Red;
                        RotateRight(grandParent);
                        OperationCount += 3;
                    }
                }
                else
                {
                    Node uncle = grandParent.Left;
                    if (uncle.Color == Color.Red)
                    {
                        parent.Color = Color.Black;
                        uncle.Color = Color.Black;
                        grandParent.Color = Color.Red;
                        node = grandParent;
                        OperationCount += 3;
                    }
                    else
                    {
                        if (node == parent.Left)
                        {
                            node = parent;
                            RotateRight(node);
                            OperationCount++;
                        }
                        parent.Color = Color.Black;
                        grandParent.Color = Color.Red;
                        RotateLeft(grandParent);
                        OperationCount += 3;
                    }
                }
            }
            root.Color = Color.Black;
            OperationCount++;
        }

        private void RotateLeft(Node x)
        {
            Node y = x.Right;
            x.Right = y.Left;
            if (y.Left != nil)
            {
                y.Left.Parent = x;
            }
            y.Parent = x.Parent;
            if (x.Parent == nil)
            {
                root = y;
            }
            else if (x == x.Parent.Left)
            {
                x.Parent.Left = y;
            }
            else
            {
                x.Parent.Right = y;
            }
            y.Left = x;
            x.Parent = y;
        }

        private void RotateRight(Node x)
        {
            Node y = x.Left;
            x.Left = y.Right;
            if (y.Right != nil)
            {
                y.Right.Parent = x;
            }
            y.Parent = x.Parent;
            if (x.Parent == nil)
            {
                root = y;
            }
            else if (x == x.Parent.Right)
            {
                x.Parent.Right = y;
            }
            else
            {
                x.Parent.Left = y;
            }
            y.Right = x;
            x.Parent = y;
        }

        public bool Delete(int value)
        {
            Node node = FindNode(value);
            if (node == nil)
            { 
                return false;
            }
            DeleteNode(node);
            return true;
        }

        private Node FindNode(int value)
        {
            Node current = root;
            while (current != nil)
            {
                OperationCount++;
                if (value == current.Value) return current;
                OperationCount++;
                if (value < current.Value) current = current.Left;
                else current = current.Right;
            }
            return nil;
        }

        private void DeleteNode(Node z)
        {
            Node y = z;
            Node x;
            Color yOriginalColor = y.Color;

            if (z.Left == nil)
            {
                x = z.Right;
                Transplant(z, z.Right);
            }
            else if (z.Right == nil)
            {
                x = z.Left;
                Transplant(z, z.Left);
            }
            else
            {
                y = Minimum(z.Right);
                yOriginalColor = y.Color;
                x = y.Right;
                if (y.Parent == z)
                {
                    x.Parent = y;
                }
                else
                {
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    y.Right.Parent = y;
                }
                Transplant(z, y);
                y.Left = z.Left;
                y.Left.Parent = y;
                y.Color = z.Color;
                OperationCount++;
            }

            if (yOriginalColor == Color.Black) 
            { 
                FixDelete(x);
            }
        }

        private void FixDelete(Node x)
        {
            while (x != root && x.Color == Color.Black)
            {
                if (x == x.Parent.Left)
                {
                    Node w = x.Parent.Right;
                    if (w.Color == Color.Red)
                    {
                        w.Color = Color.Black;
                        x.Parent.Color = Color.Red;
                        RotateLeft(x.Parent);
                        w = x.Parent.Right;
                        OperationCount += 4;
                    }
                    if (w.Left.Color == Color.Black && w.Right.Color == Color.Black)
                    {
                        w.Color = Color.Red;
                        x = x.Parent;
                        OperationCount++;
                    }
                    else
                    {
                        if (w.Right.Color == Color.Black)
                        {
                            w.Left.Color = Color.Black;
                            w.Color = Color.Red;
                            RotateRight(w);
                            w = x.Parent.Right;
                            OperationCount += 4;
                        }
                        w.Color = x.Parent.Color;
                        x.Parent.Color = Color.Black;
                        w.Right.Color = Color.Black;
                        RotateLeft(x.Parent);
                        x = root;
                        OperationCount += 5;
                    }
                }
                else
                {
                    Node w = x.Parent.Left;
                    if (w.Color == Color.Red)
                    {
                        w.Color = Color.Black;
                        x.Parent.Color = Color.Red;
                        RotateRight(x.Parent);
                        w = x.Parent.Left;
                        OperationCount += 4;
                    }
                    if (w.Right.Color == Color.Black && w.Left.Color == Color.Black)
                    {
                        w.Color = Color.Red;
                        x = x.Parent;
                        OperationCount++;
                    }
                    else
                    {
                        if (w.Left.Color == Color.Black)
                        {
                            w.Right.Color = Color.Black;
                            w.Color = Color.Red;
                            RotateLeft(w);
                            w = x.Parent.Left;
                            OperationCount += 4;
                        }
                        w.Color = x.Parent.Color;
                        x.Parent.Color = Color.Black;
                        w.Left.Color = Color.Black;
                        RotateRight(x.Parent);
                        x = root;
                        OperationCount += 5;
                    }
                }
            }
            x.Color = Color.Black;
            OperationCount++;
        }

        private void Transplant(Node u, Node v)
        {
            if (u.Parent == nil)
            {
                root = v;
            }
            else if (u == u.Parent.Left)
            {
                u.Parent.Left = v;
            }
            else
            {
                u.Parent.Right = v;
            }
            v.Parent = u.Parent;
        }

        private Node Minimum(Node node)
        {
            while (node.Left != nil)
            {
                node = node.Left;
            }
            return node;
        }
    }
}