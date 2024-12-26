using MFramework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MFramework.DLC
{
    public class MBinaryTreeNode
    {
        #region Internal Field
        internal MBinaryTree list;

        internal object item;

        internal MBinaryTreeNode parent;

        internal MBinaryTreeNode left;

        internal MBinaryTreeNode right;
        #endregion

        #region Public Field
        public MBinaryTree List => list;

        public object Value
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        public MBinaryTreeNode Parent => parent;

        public MBinaryTreeNode Left => left;

        public MBinaryTreeNode Right => right;
        #endregion

        #region Constructor
        public MBinaryTreeNode(object value)
        {
            item = value;
        }

        internal MBinaryTreeNode(object value, MBinaryTree list)
        {
            this.list = list;
            item = value;
        }
        #endregion

        #region Function
        //ʹ�ڵ���Ч(û���������������ϵ����GC����)
        internal void Invalidate()
        {
            list = null;//����---list��ս�ͨ����ValidateNode()���Ӷ��޷������κβ���
            parent = null;
            left = null;
            right = null;
        }
        #endregion
    }

    public class MBinaryTree
    {
        #region Internal Field
        private MBinaryTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MBinaryTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MBinaryTree(object value) 
        {
            MBinaryTreeNode root = new MBinaryTreeNode(value, this);
            this.root = root;
            count++;
        }
        public MBinaryTree(MBinaryTreeNode root)
        {
            ValidateNewNode(root);
            this.root = root;
            count++;
        }
        #endregion

        #region Function
        public MBinaryTreeNode AddLeft(MBinaryTreeNode node, object value)
        {
            ValidateNode(node);

            MBinaryTreeNode newNode = new MBinaryTreeNode(value, this);

            InternalAddLeft(node, newNode);

            return newNode;
        }
        public void AddLeft(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalAddLeft(node, newNode);
            newNode.list = this;
        }

        public MBinaryTreeNode AddRight(MBinaryTreeNode node, object value)
        {
            ValidateNode(node);

            MBinaryTreeNode newNode = new MBinaryTreeNode(value, this);

            InternalAddRight(node, newNode);

            return newNode;
        }
        public void AddRight(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalAddRight(node, newNode);
            newNode.list = this;
        }

        public bool Remove(MBinaryTreeNode node)
        {
            ValidateNode(node);//�Ƴ�֮ǰ���ýڵ�϶�����Ч(����)��
            InternalRemove(node);

            return true;
        }

        public object[] LevelOrder()
        {
            object[] list = new object[count];

            MQueue queue = new MQueue();
            queue.Enqueue(root);

            int i = 0;
            while (queue.Count != 0)
            {
                MBinaryTreeNode node = (MBinaryTreeNode)queue.Dequeue();

                list[i++] = node.item;//��ֵ����������

                //ֻҪ���ӽڵ㣬�����������ҵ�˳�����node
                if (node.left != null)
                {
                    queue.Enqueue(node.left);
                }
                if (node.right != null)
                {
                    queue.Enqueue(node.right);
                }
            }

            return list;
        }

        public object[] PreOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalPreOrder(root, list, 0);

            return list;
        }
        public object[] InOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalInOrder(root, list, 0);

            return list;
        }
        public object[] PostOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalPostOrder(root, list, 0);

            return list;
        }

        public bool Contains(object o)
        {
            object[] list = PreOrder();
            foreach (var item in list)
            {
                if (o == null)
                {
                    if (item == null)
                    {
                        return true;
                    }
                }
                else if (item != null && item.Equals(o))
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateNode(MBinaryTreeNode node)
        {
            //�ڵ�������
            if (node == null)
            {
                throw new Exception();
            }
            //�ڵ���������Ӧ��Ϊͬһ��
            if (node.list != this)
            {
                throw new Exception();
            }
        }
        private void ValidateNewNode(MBinaryTreeNode node)
        {
            //�ڵ�������
            if (node == null)
            {
                throw new Exception();
            }
            //�½ڵ��listӦ��Ϊnull(���ⲿ������node��list��null��)
            if (node.list != null)
            {
                throw new Exception();
            }
        }
        private void InvalidNode(MBinaryTreeNode node)
        {
            //�����ڵ��⣬һ��ڵ㶼��Ҫ�����ڵ���ýڵ�Ͽ�����
            if (node != root)
            {
                MBinaryTreeNode parentNode = node.parent;

                //���ݽڵ��ڸ��ڵ��λ�öϿ�
                if (parentNode.left == node)
                {
                    parentNode.left = null;
                }
                else if (parentNode.right == node)
                {
                    parentNode.right = null;
                }
            }

            //�Ͽ��Լ�������
            node.Invalidate();
        }

        //Tip:��������Ӧ��ΪInternalAddLeftAfter()��Ϊ**���**
        //�����У�ǰ�岻��һ���ǳ�����Ĳ�����������ʵ��
        private void InternalAddLeft(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            newNode.parent = node;

            //����ڵ�ΪҶ�ӽڵ㣬Ϊ�����AddLast()
            if (node.left == null && node.right == null)
            {
                node.left = newNode;
                count++;
                return;
            }

            //һ�����Ϊ����
            newNode.left = node.left;
            node.left = newNode;
            count++;
        }
        private void InternalAddRight(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            newNode.parent = node;

            //����ڵ�ΪҶ�ӽڵ㣬Ϊ�����AddLast()
            if (node.left == null && node.right == null)
            {
                node.right = newNode;
                count++;
                return;
            }

            //һ�����Ϊ����
            newNode.right = node.right;
            node.right = newNode;
            count++;
        }
        private void InternalRemove(MBinaryTreeNode node)
        {
            MQueue queue = new MQueue();
            queue.Enqueue(node);

            //BFS���������¶Ͽ�����
            while (queue.Count != 0)
            {
                MBinaryTreeNode tempNode = (MBinaryTreeNode)queue.Dequeue();
                count--;

                //ֻҪ���ӽڵ㣬�����������ҵ�˳�����node
                if (tempNode.left != null)
                {
                    queue.Enqueue(tempNode.left);
                }
                if (tempNode.right != null)
                {
                    queue.Enqueue(tempNode.right);
                }

                InvalidNode(tempNode);//�Ͽ�����
            }
        }

        private int InternalPreOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            list[index] = root.item;
            int next = InternalPreOrder(root.left, list, index + 1);
            return InternalPreOrder(root.right, list, next + 1);
        }
        private int InternalInOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }
        private int InternalPostOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalPostOrder(root.left, list, index);
            next++;
            next = InternalPostOrder(root.right, list, next);
            next++;
            list[next] = root.item;
            return next;
        }

        #endregion
    }

    public static class MBinaryTreeExtension
    {
        public static void Print(this MBinaryTree tree)
        {
            MQueue queue = new MQueue();
            queue.Enqueue(tree.Root);

            int count = 0;
            int beforeCount = 0;
            int level = 0;
            string levelStr = "";
            bool isNotBottom = true;
            while (queue.Count != 0)
            {
                MBinaryTreeNode node = (MBinaryTreeNode)queue.Dequeue();
                count++;

                //��������������ͻ���
                if (count == 1 || count == beforeCount * 2)
                {
                    //���û����һ��(Ϊ�ײ�����һ��)���ͽ���
                    if (!isNotBottom) break;
                    isNotBottom = false;

                    beforeCount = count;
                    level++;

                    levelStr += $"\n��{level}: ";
                }
                //���
                if (node != null)
                {
                    levelStr += $"{node.Value} ";
                }
                else
                {
                    levelStr += "X ";
                }

                //������Σ�����node����(����node����Ϊ�գ�Ҳ����˵�ýڵ㲻���ɿսڵ�����������)
                MBinaryTreeNode leftNode = node == null ? null : node.left;
                MBinaryTreeNode rightNode = node == null ? null : node.right;
                queue.Enqueue(leftNode);
                queue.Enqueue(rightNode);

                //ֻҪ��һ������һ���ӽڵ㣬��˵��������ײ�
                if (leftNode != null || rightNode != null)
                {
                    isNotBottom = true;
                }
            }
            MLog.Print(MLog.ColorWord("---���������ӻ�---", UnityEngine.Color.black, true, false) + levelStr);
        }
    }
}