using MFramework;
using MFramework.DLC;
using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MBinarySearchTreeNode
    {
        #region Internal Field
        internal MBinarySearchTree list;

        internal object item;

        internal MBinarySearchTreeNode parent;

        internal MBinarySearchTreeNode left;

        internal MBinarySearchTreeNode right;
        #endregion

        #region Public Field
        public MBinarySearchTree List => list;

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

        public MBinarySearchTreeNode Parent => parent;

        public MBinarySearchTreeNode Left => left;

        public MBinarySearchTreeNode Right => right;
        #endregion

        #region Constructor
        public MBinarySearchTreeNode(object value)
        {
            item = value;
        }

        internal MBinarySearchTreeNode(object value, MBinarySearchTree list)
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

    public class MBinarySearchTree : IEnumerable
    {
        #region Internal Field
        private MBinarySearchTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MBinarySearchTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MBinarySearchTree(){ }
        #endregion

        #region Function
        public void Add(object o)
        {
            MBinarySearchTreeNode newNode = new MBinarySearchTreeNode(o, this);

            InternalAdd(newNode);
        }
        public void Add(MBinarySearchTreeNode newNode)
        {
            ValidateNewNode(newNode);
            newNode.list = this;

            InternalAdd(newNode);
        }

        public void Remove(object o)
        {
            MBinarySearchTreeNode node = Search(o);

            InternalRemove(node);
        }
        public void Remove(MBinarySearchTreeNode node)
        {
            ValidateNode(node);

            InternalRemove(node);
        }

        public bool Contains(object o)
        {
            return Search(o) != null;
        }

        public object[] Sort()
        {
            return InOrder();
        }

        private void InternalAdd(MBinarySearchTreeNode newNode)
        {
            if (root == null)
            {
                root = newNode;
                count++;
                return;
            }

            IComparable num = newNode.item as IComparable;
            if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

            MBinarySearchTreeNode cur = root;

            while (true)
            {
                if (num.CompareTo(cur.item) > 0)//num > cur.item
                {
                    if (cur.right == null)
                    {
                        newNode.parent = cur;
                        cur.right = newNode;
                        count++;
                        return;
                    }

                    cur = cur.right;//����ֵ����������
                }
                else if (num.CompareTo(cur.item) < 0)//num < cur.item
                {
                    if (cur.left == null)
                    {
                        newNode.parent = cur;
                        cur.left = newNode;
                        count++;
                        return;
                    }

                    cur = cur.left;//����ֵ��С��������
                }
                else
                {
                    throw new Exception();//һ�Ŷ����������в���������ͬ����
                }
            }
        }
        private void InternalRemove(MBinarySearchTreeNode node)
        {
            //�ڵ�Ϊ��˵��û��ɾ������
            if (node == null)
            {
                throw new Exception();
            }

            //��Ϊ0(ΪҶ�ӽڵ�)
            if (node.left == null && node.right == null)
            {
                InvalidNode(node);
            }
            //��Ϊ1
            else if (node.left == null || node.right == null)
            {
                MBinarySearchTreeNode childNode = node.left ?? node.right;

                if (node == root)
                {
                    root = childNode;
                }
                else
                {
                    if (node.parent.left == node)//�ýڵ�Ϊ��ڵ�
                    {
                        node.parent.left = childNode;
                    }
                    else if (node.parent.right == node)//�ýڵ�Ϊ�ҽڵ�
                    {
                        node.parent.right = childNode;
                    }
                }
                node.Invalidate();//���ڵ��Ѿ����������ڵ㣬����Ҫʹ��InvalidNode()(���ڸ��ڵ�Ҳ������)
            }
            //��Ϊ2
            else
            {
                MBinarySearchTreeNode tempNode = node.right;
                while (tempNode.left != null)
                {
                    tempNode = tempNode.left;
                }
                node.item = tempNode.item;
                InvalidNode(tempNode);
            }

            count--;
        }

        private MBinarySearchTreeNode Search(object o)
        {
            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

            MBinarySearchTreeNode cur = root;

            while (cur != null)
            {
                if (num.CompareTo(cur.item) > 0)//num > cur.item
                {
                    cur = cur.right;//����ֵ����������
                }
                else if (num.CompareTo(cur.item) < 0)//num < cur.item
                {
                    cur = cur.left;//����ֵ��С��������
                }
                else
                {
                    break;//���Ǹ�ֵ����ȡ
                }
            }
            return cur;
        }

        private void ValidateNode(MBinarySearchTreeNode node)
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
        private void ValidateNewNode(MBinarySearchTreeNode node)
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
        private void InvalidNode(MBinarySearchTreeNode node)
        {
            //�����ڵ��⣬һ��ڵ㶼��Ҫ�����ڵ���ýڵ�Ͽ�����
            if (node != root)
            {
                MBinarySearchTreeNode parentNode = node.parent;

                //���ݽڵ��ڸ��ڵ�����ҶϿ�
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

        public IEnumerator GetEnumerator()
        {
            foreach (var item in InOrder())
            {
                yield return item;
            }
        }
        private object[] InOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalInOrder(root, list, 0);

            return list;
        }
        private int InternalInOrder(MBinarySearchTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }

        #endregion
    }

    public static class MBinarySearchTreeExtension
    {
        public static void Print(this MBinarySearchTree tree)
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
                MBinarySearchTreeNode node = (MBinarySearchTreeNode)queue.Dequeue();
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
                MBinarySearchTreeNode leftNode = node == null ? null : node.left;
                MBinarySearchTreeNode rightNode = node == null ? null : node.right;
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

        public static void SortPrint(this MBinarySearchTree tree)
        {
            MLog.Print("���: ");

            if (tree.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in tree)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}